using System.IO;

namespace Clickwheel.IPodDevice.FileSystems
{
    public class IPodDriveInfo
    {
        private DriveInfo _driveInfo;
        private DirectoryInfo _directoryInfo;

        public IPodDriveInfo(DriveInfo driveInfo)
        {
            _driveInfo = driveInfo;
        }

        public IPodDriveInfo(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public IPodDriveInfo(string drivePath)
        {
            _driveInfo = new DriveInfo(drivePath);
        }

        public bool IsReady => _driveInfo?.IsReady ?? true;

        public DriveType DriveType => _driveInfo?.DriveType ?? DriveType.Fixed;

        public string Name => _driveInfo?.Name ?? _directoryInfo.FullName;

        public long TotalSize =>
            _driveInfo?.TotalSize ?? new DriveInfo(_directoryInfo.Root.FullName).TotalSize;

        public long AvailableFreeSpace =>
            _driveInfo?.AvailableFreeSpace
            ?? new DriveInfo(_directoryInfo.Root.FullName).AvailableFreeSpace;
    }
}
