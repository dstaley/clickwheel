#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Clickwheel.Parsers;

namespace Clickwheel.IPodDevice.FileSystems
{
    /// <summary>
    /// Abstraction of a standard (disk-based) iPod file system.
    /// </summary>
    class StandardFileSystem : DeviceFileSystem
    {
        private FileStream? _fileLock;
        private const int FileCopyBufferSize = 131072; // 262144; //256k chunks

        public override event FileCopyProgressHandler FileCopyProgress;
        public override event EventHandler SyncCancelled;

        public StandardFileSystem(
            string name,
            string iTunesFolderPath,
            string iPodControlFolderPath,
            string artworkFolderPath,
            string photoFolderPath
        ) : base(name, iTunesFolderPath, iPodControlFolderPath, artworkFolderPath, photoFolderPath)
        { }

        public StandardFileSystem() { }

        public override void CopyFileToDevice(string source, string destination)
        {
            if (!destination.StartsWith(DriveLetter))
            {
                destination = DriveLetter + destination;
            }

            if (FailsafeMode)
            {
                File.Copy(source, destination, true);
                return;
            }

            long bytesTransferred = 0;

            using (var sourceFile = File.OpenRead(source))
            {
                using (var destinationFile = File.Create(destination))
                {
                    var copyBuffer = new byte[FileCopyBufferSize];

                    while (true)
                    {
                        var length = sourceFile.Read(copyBuffer, 0, copyBuffer.Length);
                        if (length <= 0)
                        {
                            break;
                        }

                        destinationFile.Write(copyBuffer, 0, length);
                        bytesTransferred += length;

                        if (FileCopyProgress != null)
                        {
                            FileCopyProgress(sourceFile.Length, bytesTransferred);
                        }
                    }
                }
            }
        }

        public override void CopyFileFromDevice(string source, string destination)
        {
            if (!source.StartsWith(DriveLetter))
            {
                source = DriveLetter + source;
            }

            if (FailsafeMode)
            {
                File.Copy(source, destination, true);
                return;
            }

            long bytesTransferred = 0;

            using (var sourceFile = File.OpenRead(source))
            {
                using (var destinationFile = File.Create(destination))
                {
                    var copyBuffer = new byte[FileCopyBufferSize];

                    while (true)
                    {
                        var length = sourceFile.Read(copyBuffer, 0, copyBuffer.Length);
                        if (length <= 0)
                        {
                            break;
                        }

                        destinationFile.Write(copyBuffer, 0, length);
                        bytesTransferred += length;

                        if (FileCopyProgress != null)
                        {
                            FileCopyProgress(sourceFile.Length, bytesTransferred);
                        }
                    }
                }
            }
        }

        public override bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public override bool DirectoryExists(string name)
        {
            return Directory.Exists(name);
        }

        public override void DeleteFile(string name)
        {
            try
            {
                File.SetAttributes(name, FileAttributes.Normal); //can prevent IO errors deleting readonly files
            }
            catch (Exception) { }
            File.Delete(name);
        }

        public override long GetFileLength(string name)
        {
            return new FileInfo(name).Length;
        }

        public override void CreateDirectory(string name)
        {
            Helpers.EnsureDirectoryExists(new DirectoryInfo(name));
        }

        public override void AcquireLock()
        {
            if (!File.Exists(ITunesLockPath))
            {
                File.Create(ITunesLockPath).Close();
            }
            if (_fileLock == null)
            {
                _fileLock = File.Open(
                    ITunesLockPath,
                    FileMode.Open,
                    FileAccess.Write,
                    FileShare.None
                );
            }
        }

        public override void ReleaseLock()
        {
            if (_fileLock != null)
            {
                _fileLock.Close();
                File.Delete(ITunesLockPath);
                _fileLock = null;
            }
        }

        public override long AvailableFreeSpace =>
            Math.Max(new IPodDriveInfo(DriveLetter).AvailableFreeSpace - 10485760, 0);

        public override long TotalSize => new IPodDriveInfo(DriveLetter).TotalSize;

        internal override IDeviceInfo QueryDeviceInfo(IPod iPod)
        {
            var deviceInfo = new XmlQueryDeviceInfo(iPod);
            deviceInfo.Read();
            return deviceInfo;
        }

        public override void StartSync()
        {
            // not implemented
        }

        public override void EndSync()
        {
            // not implemented
        }

        public override Stream OpenFile(string path, FileAccess mode)
        {
            return new FileStream(path, FileMode.OpenOrCreate, mode);
        }

        internal override DeviceFileSystem? GetDevice()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                var fs = GetDeviceByDrive(new IPodDriveInfo(drive));
                if (fs != null)
                {
                    return fs;
                }
            }
            return null;
        }

        internal override List<DeviceFileSystem> GetAllDevices()
        {
            var fsList = new List<DeviceFileSystem>();

            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                var fs = GetDeviceByDrive(new IPodDriveInfo(drive));
                if (fs != null)
                {
                    fsList.Add(fs);
                }
            }
            return fsList;
        }

        public DeviceFileSystem? GetDeviceByDrive(IPodDriveInfo drive)
        {
            if (
                drive.IsReady && drive.DriveType == DriveType.Fixed
                || drive.DriveType == DriveType.Removable
            )
            {
                if (DirectoryExists(Path.Combine(drive.Name, ITunesFolderPath)))
                {
                    var fsInstance = Clone();
                    fsInstance.DriveLetter = drive.Name;
                    return fsInstance;
                }
            }
            return null;
        }

        public override string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        private StandardFileSystem Clone()
        {
            var standardFS = new StandardFileSystem();
            base.Clone(standardFS);
            return standardFS;
        }
    }
}
