using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.Shell;
using Wpf.Ui.Common;

namespace Clickwheel.DeviceHelper.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ActivateDarkMode();

            if (IsAdministrator())
            {
                Heading.Text = "Clickwheel Device Helper";
                Subhead.Text =
                    "This app will store a SysInfoExtended file on each attached iPod, allowing you to use applications that use the Clickwheel library.";
                ButtonText.Text = "Start";
            }
            else
            {
                Heading.Text = "Relaunch as Administrator?";
                Subhead.Text = "Clickwheel Device Helper needs to be run as Administrator in order to access your iPod.";
                ButtonText.Text = "Run as Administrator";

                var sii = new SHSTOCKICONINFO
                {
                    cbSize = (UInt32)Marshal.SizeOf(typeof(SHSTOCKICONINFO))
                };

                Marshal.ThrowExceptionForHR(PInvoke.SHGetStockIconInfo(SHSTOCKICONID.SIID_SHIELD,
                    SHGSI_FLAGS.SHGSI_ICON | SHGSI_FLAGS.SHGSI_SMALLICON,
                    ref sii));

                var shieldSource = Imaging.CreateBitmapSourceFromHIcon(
                    sii.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                PInvoke.DestroyIcon(sii.hIcon);

                uacButton.Source = shieldSource;
            }
        }

        private static bool DriveIsIPod(DriveInfo drive)
        {
            return Directory.Exists(Path.Join(drive.Name, "iPod_Control")) &&
                   Directory.Exists(Path.Join(drive.Name, "iPod_Control", "Device")) &&
                   Directory.Exists(Path.Join(drive.Name, "iPod_Control", "iTunes"));
        }

        private static void WriteSysInfoExtended(DriveInfo drive, string info)
        {
            File.WriteAllText(Path.Join(drive.Name, "iPod_Control", "Device", "SysInfoExtended"), info);
        }

        private static List<DriveInfo> GetSysInfoExtended()
        {
            var validIpods = new List<DriveInfo>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (DriveIsIPod(drive))
                {
                    try
                    {
                        var info = DeviceXml.Get(drive.Name);
                        if (info != null)
                        {
                            WriteSysInfoExtended(drive, info);
                            validIpods.Add(drive);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return validIpods;
        }

        private void RunAsAdministrator_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                var validIpods = GetSysInfoExtended();
                if (validIpods.Count > 0)
                {
                    var messageBox = new Wpf.Ui.Controls.MessageBox();

                    var stackPanel = new StackPanel();
                    var button = new Wpf.Ui.Controls.Button() { Content = "Close", Appearance = ControlAppearance.Secondary };
                    button.Click += (o, args) =>
                    {
                        messageBox.Close();
                        Application.Current.Shutdown();
                    };

                    stackPanel.Children.Add(button);
                    messageBox.Footer = stackPanel;

                    messageBox.Show("Success!", $"SysInfoExtended written to all attached iPods ({string.Join(", ", validIpods.Select(i => i.Name))}).");
                }
                else
                {
                    var messageBox = new Wpf.Ui.Controls.MessageBox();

                    var stackPanel = new StackPanel();
                    var button = new Wpf.Ui.Controls.Button() { Content = "Close", Appearance = ControlAppearance.Secondary };
                    button.Click += (o, args) =>
                    {
                        messageBox.Close();
                    };

                    stackPanel.Children.Add(button);
                    messageBox.Footer = stackPanel;

                    messageBox.Show("Error!", "Could not find any connected iPods.");
                }
            }
            else
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = Process.GetCurrentProcess().MainModule.FileName,
                        UseShellExecute = true,
                        Verb = "runas"
                    }
                };

                try
                {
                    proc.Start();
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        private unsafe void ActivateDarkMode()
        {
            var hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var useDarkMode = 1;
            PInvoke.DwmSetWindowAttribute((HWND)hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &useDarkMode, sizeof(int));
            PInvoke.DwmSetWindowAttribute((HWND)hWnd, (DWMWINDOWATTRIBUTE)1029, &useDarkMode, sizeof(int));

            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(this, Wpf.Ui.Appearance.BackgroundType.Mica, true, true);
            };
        }
    }
}