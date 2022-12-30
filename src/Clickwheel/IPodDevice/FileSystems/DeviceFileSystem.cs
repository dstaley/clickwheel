using System;
using System.Collections.Generic;
using System.IO;

namespace Clickwheel.IPodDevice.FileSystems
{
    public delegate void FileCopyProgressHandler(long fileLength, long bytesTransferred);

    /// <summary>
    /// Abstraction of the file system used by a specific iPod. Currently, this is either standard or iPhone.
    /// Additional DeviceFileSystems can be added at runtime - see Clickwheel.RegisteredFileSystems
    /// </summary>
    public abstract class DeviceFileSystem
    {
        protected string _name,
            _iTunesFolderPath,
            _iPodControlFolderPath,
            _artworkFolderPath,
            _photoFolderPath;

        public IPod IPod { get; internal set; }
        public string DriveLetter { get; internal set; }
        protected bool _parseDbFilesLocally;
        public abstract event FileCopyProgressHandler FileCopyProgress;
        public abstract event EventHandler SyncCancelled;

        internal abstract DeviceFileSystem GetDevice();
        internal abstract List<DeviceFileSystem> GetAllDevices();
        public abstract void CopyFileToDevice(string source, string destination);
        public abstract void CopyFileFromDevice(string source, string destination);
        public abstract bool FileExists(string fileName);
        public abstract bool DirectoryExists(string name);
        public abstract void DeleteFile(string name);
        public abstract long GetFileLength(string name);
        public abstract void CreateDirectory(string name);
        public abstract void AcquireLock();
        public abstract void ReleaseLock();
        public abstract string CombinePath(string path1, string path2);

        //Returns the filesystem provider. For the StandardFileSystem, this is null. For the iPhoneFileSystem, this returns an IPhone object.
        public virtual object GetProvider()
        {
            return null;
        }

        /// <summary>
        /// Amount of free space on the filesystem minus 10MB to make sure we have space to save the iPod databases
        /// </summary>
        public abstract long AvailableFreeSpace { get; }
        public abstract long TotalSize { get; }
        internal abstract IDeviceInfo QueryDeviceInfo(IPod iPod);
        public abstract void StartSync();
        public abstract void EndSync();
        public abstract Stream OpenFile(string path, FileAccess mode);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">Name of this profile</param>
        /// <param name="iTunesFolderPath">Path of the iTunes folder</param>
        /// <param name="musicFolderPath">Path of the Music folder</param>
        /// <param name="expectedDriveType">DriveType the profile uses</param>
        internal DeviceFileSystem(
            string name,
            string iTunesFolderPath,
            string iPodControlFolderPath,
            string artworkFolderPath,
            string photoFolderPath
        )
        {
            _name = name;
            _iTunesFolderPath = iTunesFolderPath;
            _iPodControlFolderPath = iPodControlFolderPath;
            _artworkFolderPath = artworkFolderPath;
            _photoFolderPath = photoFolderPath;
            DriveLetter = "";
        }

        internal DeviceFileSystem() { }

        /// <summary>
        /// For disk-based iPods, failsafe mode causes files to be copied using standard File.Copy(), instead of buffer copying.
        /// There will be no FileCopyProgress events thrown. For iPhone/iTouch this has no effect.
        /// </summary>
        public bool FailsafeMode { get; set; }

        /// <summary>
        /// If this is true, the iTunesDB and ArtworkDB files will be copied to the system temp folder before parsing. Depending on the PC's hardware/conf,
        /// this might be slower or faster than parsing straight from the iPod drive.
        /// </summary>
        public bool ParseDbFilesLocally
        {
            get => _parseDbFilesLocally;
            set => _parseDbFilesLocally = value;
        }

        /// <summary>
        /// Name of the profile
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Path of the iPod's control folder (e.g. ipod_control)
        /// </summary>
        public string IPodControlPath
        {
            get => Path.Combine(DriveLetter, _iPodControlFolderPath);
            set => _iPodControlFolderPath = value;
        }

        /// <summary>
        /// Path of the iPod's iTunes folder (e.g. ipod_control\iTunes)
        /// </summary>
        public string ITunesFolderPath
        {
            get => Path.Combine(DriveLetter, _iTunesFolderPath);
            set => _iTunesFolderPath = value;
        }

        /// <summary>
        /// Path of the iPod's Artwork folder (e.g. ipod_control\Artwork)
        /// </summary>
        public string ArtworkFolderPath
        {
            get => Path.Combine(DriveLetter, _artworkFolderPath);
            set => _artworkFolderPath = value;
        }

        /// <summary>
        /// Path of the iPod's Photo folder (e.g. ipod_control\Photos)
        /// </summary>
        public string PhotoFolderPath
        {
            get => Path.Combine(DriveLetter, _photoFolderPath);
            set => _photoFolderPath = value;
        }

        /// <summary>
        /// The iPod's database file (e.g. ipod_control\iTunes\iTunesDB)
        /// </summary>
        public string ArtworkDBPath => Path.Combine(DriveLetter, _artworkFolderPath, "ArtworkDB");

        /// <summary>
        /// The iPod's database file (e.g. Photos\Photo Database)
        /// </summary>
        public string PhotoDBPath => Path.Combine(DriveLetter, _photoFolderPath, "Photo Database");

        /// <summary>
        /// The iPod Shuffle database file (e.g. ipod_control\iTunes\iTunesSD)
        /// </summary>
        public string ITunesSDPath => Path.Combine(DriveLetter, _iTunesFolderPath, "iTunesSD");

        /// <summary>
        /// The iPod Play Counts file (e.g. ipod_control\iTunes\Play Counts)
        /// </summary>
        public string PlayCountsPath => Path.Combine(DriveLetter, _iTunesFolderPath, "Play Counts");

        /// <summary>
        /// The iPod's lock flag file (e.g. ipod_control\iTunes\iTunesLock)
        /// </summary>
        public string ITunesLockPath => Path.Combine(DriveLetter, _iTunesFolderPath, "iTunesLock");

        internal string HashInfoPath => Path.Combine(DriveLetter, "HashInfo");

        internal void Clone(DeviceFileSystem newObject)
        {
            newObject._name = _name;
            newObject._iTunesFolderPath = _iTunesFolderPath;
            newObject._iPodControlFolderPath = _iPodControlFolderPath;
            newObject._artworkFolderPath = _artworkFolderPath;
            newObject._photoFolderPath = _photoFolderPath;
            newObject._parseDbFilesLocally = _parseDbFilesLocally;
        }
    }
}
