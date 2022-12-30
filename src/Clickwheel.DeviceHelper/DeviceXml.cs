using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.Storage.IscsiDisc;
using Windows.Win32.System.IO;
using Windows.Win32.System.Ioctl;
using Microsoft.Win32.SafeHandles;

namespace Clickwheel.DeviceHelper
{
    public class DeviceXml
    {
        public static string? Get(string drive)
        {
            var hDevice = (SafeFileHandle)null;
            try
            {
                var deviceFromDrive = GetDeviceFromDrive(new DriveInfo(drive));
                var array = new byte[102400];
                var destinationIndex = 0;
                hDevice = PInvoke.CreateFile(deviceFromDrive,
                                             FILE_ACCESS_FLAGS.FILE_ALL_ACCESS,
                                             FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                                             null,
                                             FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                                             0,
                                             null);
                if (hDevice.IsInvalid)
                {
                    throw new Exception("Could not access the iPod windows device. Make sure you have administrator rights on your computer.");
                }

                unsafe
                {
                    var structure = new ScsiPassThroughWithBuffers();
                    structure.spt.Length = (ushort) sizeof(SCSI_PASS_THROUGH);
                    structure.spt.PathId = 0;
                    structure.spt.TargetId = 1;
                    structure.spt.Lun = 0;
                    structure.spt.CdbLength = 6;
                    structure.spt.SenseInfoLength = 32;
                    structure.spt.DataIn = 1;
                    structure.spt.DataTransferLength = byte.MaxValue;
                    structure.spt.TimeOutValue = 2U;
                    structure.spt.DataBufferOffset = (nuint) (structure.ucDataBuf - (byte*) &structure);
                    structure.spt.SenseInfoOffset = (uint) (structure.ucSenseBuf - (byte*) &structure);
                    structure.spt.Cdb[0] = 0x12;
                    structure.spt.Cdb[1] |= 1;
                    structure.spt.Cdb[2] = 0xc0;
                    structure.spt.Cdb[4] = byte.MaxValue;

                    var nOutBufferSize1 = structure.ucDataBuf - (byte*) &structure + structure.spt.DataTransferLength;
                    var lpOverlapped = new NativeOverlapped();
                    var bytesReturned = (uint) 0;
                    if (!PInvoke.DeviceIoControl(
                            hDevice,
                            PInvoke.IOCTL_SCSI_PASS_THROUGH,
                            &structure,
                            (uint) sizeof(SCSI_PASS_THROUGH),
                            &structure,
                            (uint) nOutBufferSize1,
                            &bytesReturned,
                            (OVERLAPPED*) &lpOverlapped
                        ))
                    {
                        throw new Exception(
                            $"A '{Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()).Message}' error occured while calling DeviceIoControl"
                        );
                    }

                    var num2 = structure.ucDataBuf[4];
                    var num3 = structure.ucDataBuf[3 + structure.ucDataBuf[3]];
                    for (var index = num2; index <= num3; ++index)
                    {
                        structure.spt.Length = (ushort) sizeof(SCSI_PASS_THROUGH);
                        structure.spt.PathId = 0;
                        structure.spt.TargetId = 1;
                        structure.spt.Lun = 0;
                        structure.spt.CdbLength = 6;
                        structure.spt.SenseInfoLength = 32;
                        structure.spt.DataIn = 1;
                        structure.spt.DataTransferLength = byte.MaxValue;
                        structure.spt.TimeOutValue = 2U;
                        structure.spt.DataBufferOffset = (nuint) (structure.ucDataBuf - (byte*) &structure);
                        structure.spt.SenseInfoOffset = (uint) (structure.ucSenseBuf - (byte*) &structure);
                        structure.spt.Cdb[0] = 18;
                        structure.spt.Cdb[1] |= 1;
                        structure.spt.Cdb[2] = index;
                        structure.spt.Cdb[4] = byte.MaxValue;
                        var nOutBufferSize2 =
                            structure.ucDataBuf - (byte*) &structure + structure.spt.DataTransferLength;
                        var pBytesReturned = (uint) 0;
                        if (!PInvoke.DeviceIoControl(
                                hDevice,
                                PInvoke.IOCTL_SCSI_PASS_THROUGH,
                                &structure,
                                (uint) sizeof(SCSI_PASS_THROUGH),
                                &structure,
                                (uint) nOutBufferSize2,
                                &pBytesReturned,
                                (OVERLAPPED*) &lpOverlapped
                            ))
                        {
                            throw new Exception(
                                $"A '{Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()).Message}' error occured while calling DeviceIoControl"
                            );
                        }

                        while (destinationIndex + structure.ucDataBuf[3] > array.Length)
                        {
                            Array.Resize(ref array, array.Length * 2);
                        }

                        for (var i = 0; i < structure.ucDataBuf[3]; i++)
                        {
                            array[destinationIndex + i] = structure.ucDataBuf[4 + i];
                        }
                        destinationIndex += structure.ucDataBuf[3];
                    }
                    if (destinationIndex == 0)
                    {
                        return null;
                    }

                    array[destinationIndex] = 0;
                    Array.Resize(ref array, destinationIndex + 1);
                    return Encoding.UTF8.GetString(array);
                }
            }
            finally
            {
                hDevice.Close();
            }
        }

        private static string GetDeviceFromDrive(DriveInfo driveInfo)
        {
            var output = new STORAGE_DEVICE_NUMBER();
            var hDevice = (SafeFileHandle)null;
            try
            {
                unsafe
                {
                    hDevice = PInvoke.CreateFile(
                        @"\\.\" + driveInfo.ToString().Substring(0, 2),
                        FILE_ACCESS_FLAGS.FILE_GENERIC_READ,
                        FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                        null,
                        FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                        0,
                        null
                    );
                    if (hDevice.IsInvalid)
                    {
                        throw new Exception("Drive handle invalid");
                    }

                    uint retByte;
                    var lpOverlapped = new NativeOverlapped();
                    if (!PInvoke.DeviceIoControl(
                            hDevice,
                            PInvoke.IOCTL_STORAGE_GET_DEVICE_NUMBER,
                            null,
                            0,
                            &output,
                            (uint) sizeof(STORAGE_DEVICE_NUMBER),
                            &retByte,
                            (OVERLAPPED*) &lpOverlapped
                        ))
                    {
                        throw new Exception(
                            $"A '{Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()).Message}' error occured while calling DeviceIoControl"
                        );
                    }

                    return @"\\.\PhysicalDrive" + output.DeviceNumber;
                }
            }
            finally
            {
                hDevice.Close();
            }
        }
    }
}