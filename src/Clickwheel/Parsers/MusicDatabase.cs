using System;
using System.Diagnostics;
using System.IO;
using Clickwheel.DatabaseHash;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.iTunesCDB;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers
{
    internal class MusicDatabase : BaseDatabase
    {
        internal iTunesDBRoot DatabaseRoot;
        internal TrackList TracksList;
        internal PlaylistList PlaylistsList;

        public MusicDatabase(IPod iPod)
        {
            _iPod = iPod;
            var fs = _iPod.FileSystem;
            if (fs.FileExists(fs.CombinePath(fs.ITunesFolderPath, "iTunesCDB")))
            {
                _databaseFilePath = fs.CombinePath(fs.ITunesFolderPath, "iTunesCDB");
            }
            else
            {
                _databaseFilePath = fs.CombinePath(fs.ITunesFolderPath, "iTunesDB");
            }
        }

        public override int Version => DatabaseRoot.VersionNumber;

        internal int HashingScheme => DatabaseRoot.HashingScheme;

        public override void Parse()
        {
            if (!_iPod.FileSystem.FileExists(_databaseFilePath))
            {
                throw new InvalidIPodDriveException(
                    "iPod database not found in " + _databaseFilePath
                );
            }

            if (_iPod.FileSystem.GetFileLength(_databaseFilePath) == 0)
            {
                throw new InvalidIPodDriveException(
                    $"Database file at {_databaseFilePath} is empty. Please run iTunes with your iPod connected, then try again."
                );
            }

            if (_iPod.FileSystem.FileExists(_iPod.FileSystem.ITunesLockPath))
            {
                try
                {
                    _iPod.FileSystem.DeleteFile(_iPod.FileSystem.ITunesLockPath);
                }
                catch
                {
                    throw new ITunesLockException(_iPod.FileSystem.ITunesLockPath);
                }
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (_databaseFilePath.EndsWith("iTunesCDB"))
            {
                DatabaseRoot = new ITunesCDBRoot();
            }
            else
            {
                DatabaseRoot = new iTunesDBRoot();
            }

            ReadDatabase(DatabaseRoot);

            stopwatch.Stop();
            Trace.WriteLine(
                $"MusicDatabase: {_compatibility}, version {DatabaseRoot.VersionNumber}"
            );
            Trace.WriteLine($"MusicDatabase: HashingScheme {DatabaseRoot.HashingScheme}");

            Debug.WriteLine($"MusicDatabase: Parsed in {stopwatch.ElapsedMilliseconds} msec");

            var tracksContainer = (TrackListContainer)DatabaseRoot
                .GetChildSection(MHSDSectionType.Tracks)
                .GetListContainer();
            TracksList = tracksContainer.GetTrackList();

            PlaylistsList = DatabaseRoot.GetPlaylistList();
            PlaylistsList.ResolveTracks();
        }

        public override void Save()
        {
            AssertIsWritable();
            Debug.WriteLine("Saving MusicDatabase " + DateTime.Now);
            IPodBackup.BackupDatabase(_iPod);

            PlaylistListV2Container playlistV2Container = null;
            if (DatabaseRoot.GetChildSection(MHSDSectionType.PlaylistsV2) != null)
            {
                playlistV2Container = (PlaylistListV2Container)DatabaseRoot
                    .GetChildSection(MHSDSectionType.PlaylistsV2)
                    .GetListContainer();
                var playlistV2List = playlistV2Container.GetPlaylistsList();
                if (this.PlaylistsList != playlistV2List)
                {
                    //if we aren't already using the V2 playlist, sync it up here
                    playlistV2List.FollowChanges(this.PlaylistsList);
                }
            }

            PlaylistsList[0].ReIndex();

            WriteDatabase(DatabaseRoot);
        }

        public override void DoActionOnWriteDatabase(FileStream stream)
        {
            if (DatabaseRoot.VersionNumber >= 25)
            {
                DatabaseHasher.Hash(stream, _iPod);
            }
        }

        #region Properties

        public override bool IsDirty
        {
            get
            {
                if (TracksList.IsDirty || PlaylistsList.IsDirty)
                {
                    return true;
                }

                foreach (var t in TracksList)
                {
                    if (t.IsDirty)
                    {
                        return true;
                    }
                }
                foreach (var p in PlaylistsList)
                {
                    if (p.IsDirty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

    }
}
