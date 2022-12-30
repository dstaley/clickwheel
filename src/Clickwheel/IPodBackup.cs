using System;
using System.Collections.Generic;
using System.IO;

namespace Clickwheel
{
    /// <summary>
    /// Provides static methods for backing up and restoring the iPod database.
    /// </summary>
    public static class IPodBackup
    {
        private static bool _backupPerformed;
        private static string _overrideBackupsFolder;
        private static int _numberBackupsToKeep = 1;
        private static bool _enableBackups = true;

        /// <summary>
        /// If not set, this defaults to [ApplicationData]\Clickwheel\Backups.
        /// </summary>
        public static string BackupsFolder
        {
            get => _overrideBackupsFolder;
            set => _overrideBackupsFolder = value;
        }

        /// <summary>
        /// Number of backup files to keep before deleting old files.
        /// Defaults to 1
        /// </summary>
        public static int NumberBackupsToKeep
        {
            get => _numberBackupsToKeep;
            set => _numberBackupsToKeep = value;
        }

        /// <summary>
        /// Enable/disable backup creation. By default this is set to true (Enabled)
        /// </summary>
        public static bool EnableBackups
        {
            get => _enableBackups;
            set => _enableBackups = value;
        }

        /// <summary>
        /// Will backup the iPod's database (iTunesDB, ArtworkDB files) if it hasnt been backed up this session already.
        /// </summary>
        internal static void BackupDatabase(IPod iPod)
        {
            if (_backupPerformed || (_enableBackups == false))
            {
                return;
            }

            var backupFolder = GetBackupsFolder(iPod);

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
            else
            {
                var di = new DirectoryInfo(backupFolder);
                var backupFiles = new List<FileInfo>(di.GetFiles("*DB_*.spbackup"));

                while (backupFiles.Count > (NumberBackupsToKeep - 1) * 2 && backupFiles.Count > 0)
                {
                    var oldestFile = backupFiles[0];
                    foreach (var backupFile in backupFiles)
                    {
                        if (backupFile.CreationTime < oldestFile.CreationTime)
                        {
                            oldestFile = backupFile;
                        }
                    }
                    oldestFile.Delete();
                    backupFiles.Remove(oldestFile);
                }
            }

            if (NumberBackupsToKeep > 0)
            {
                _backupPerformed = true;
            }
        }

        /// <summary>
        /// Returns a list of Clickwheel backup files (files called *.spbackup in backups folder)
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetBackups(IPod iPod)
        {
            var backupFolder = GetBackupsFolder(iPod);

            if (!Directory.Exists(backupFolder))
            {
                return null;
            }
            else
            {
                var di = new DirectoryInfo(backupFolder);
                return di.GetFiles("ITunesDB_*.spbackup");
            }
        }

        private static string GetBackupsFolder(IPod iPod)
        {
            //FirewireId should only ever be null for 3rd gen (old) iPods which dont support the SCSI device query
            var firewireId = iPod.DeviceInfo.FirewireId ?? "";

            if (_overrideBackupsFolder != null)
            {
                return Path.Combine(_overrideBackupsFolder, firewireId);
            }
            else
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Clickwheel",
                    "Backups",
                    firewireId
                );
            }
        }
    }
}
