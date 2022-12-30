using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.iTunesDB;
using SixLabors.ImageSharp;

namespace Clickwheel.Parsers.Artwork
{
    internal class ArtworkDB : BaseDatabase
    {
        private ArtworkDBRoot _databaseRoot;
        private ImageList _artworkList;
        private IThmbFileList _iThmbFileList;
        private bool _isDirty = false;

        public ArtworkDB(IPod iPod)
        {
            _iPod = iPod;
            _databaseFilePath = iPod.FileSystem.ArtworkDBPath;
        }

        public override void Parse()
        {
            if (!_iPod.FileSystem.FileExists(_databaseFilePath))
            {
                if (_iPod.DeviceInfo.SupportedArtworkFormats.Count > 0)
                {
                    Trace.WriteLine("ArtworkDB not found - importing empty ArtworkDB");
                    var tempPath = Path.GetTempFileName();
                    using (
                        var dbStream = Assembly
                            .GetExecutingAssembly()
                            .GetManifestResourceStream("Clickwheel.Resources.ArtworkDB-empty")
                    )
                    {
                        using (var file = File.Create(tempPath))
                        {
                            dbStream.CopyTo(file);
                        }
                    }
                    _iPod.FileSystem.CreateDirectory(_iPod.FileSystem.ArtworkFolderPath);
                    _iPod.FileSystem.CopyFileToDevice(tempPath, _databaseFilePath);
                    File.Delete(tempPath);
                }
                else
                {
                    return; //no ArtworkDB and no SupportsArtworkFormats > we don't need to do anything.
                }
            }

            _databaseRoot = new ArtworkDBRoot();
            ReadDatabase(_databaseRoot);
            Trace.WriteLine("ArtworkDB: " + _compatibility);

            _artworkList = (
                (ImageListContainer)_databaseRoot
                    .GetChildSection(MHSDSectionType.Images)
                    .GetListContainer()
            ).ImageList;
            _iThmbFileList = (
                (IThmbFileListContainer)_databaseRoot
                    .GetChildSection(MHSDSectionType.Files)
                    .GetListContainer()
            ).FileList;

            //Match up the artwork to our track objects
            foreach (var track in _iPod.Tracks)
            {
                var artwork = GetTrackArtForTrack(track);
                if (artwork != null)
                {
                    foreach (var format in artwork.Formats)
                    {
                        track.Artwork.Add(format);
                    }
                }
            }
        }

        public override void Save()
        {
            if (_databaseRoot == null)
            {
                return;
            }

            AssertIsWritable();
            Debug.WriteLine("Saving ArtworkDB " + DateTime.Now);
            WriteDatabase(_databaseRoot);
            _isDirty = false;
        }

        public override bool IsDirty => _isDirty;

        public override void AssertIsWritable()
        {
            if (_databaseRoot == null)
            {
                throw new ArtworkDBNotFoundException();
            }
            base.AssertIsWritable();
        }

        public ImageList ArtworkList => _artworkList;

        public override int Version => 0;

        public uint NextImageId
        {
            get => _databaseRoot.NextImageId;
            set => _databaseRoot.NextImageId = value;
        }

        internal void SetArtwork(Track track, Image image)
        {
            if (_iPod.DeviceInfo.SupportedArtworkFormats.Count == 0)
            {
                return;
            }

            AssertIsWritable();

            var existingArt = GetTrackArtForTrack(track);
            if (existingArt == null)
            {
                if (_iPod.FileSystem.AvailableFreeSpace <= 0)
                {
                    throw new OutOfDiskSpaceException("Your iPod does not have enough free space.");
                }

                _artworkList.AddNewArtwork(track, image);
            }
            else
            {
                existingArt.Update(image);
                track.Artwork.Clear();
                track.Artwork.AddRange(existingArt.Formats);
                track.ArtworkIdLink = existingArt.Id;
            }
            _isDirty = true;
        }

        internal void RemoveArtwork(Track track)
        {
            if (_iPod.DeviceInfo.SupportedArtworkFormats.Count == 0)
            {
                return;
            }

            if (_databaseRoot == null)
            {
                return;
            }

            var shouldRemove = true;
            if (track.ArtworkIdLink != 0)
            {
                var tracksUsingArtwork = _iPod.Tracks.FindAll(
                    delegate(Track t)
                    {
                        return t.ArtworkIdLink == track.ArtworkIdLink;
                    }
                );
                shouldRemove = tracksUsingArtwork.Count <= 1;
            }

            var existingArt = GetTrackArtForTrack(track);
            if (existingArt != null)
            {
                AssertIsWritable();
                if (shouldRemove)
                {
                    _artworkList.RemoveArtwork(existingArt);
                }

                track.Artwork.Clear();
                _isDirty = true;
            }
        }

        internal void GetIThmbRepository(
            IPodImageFormat format,
            out string fileName,
            out uint fileOffset
        )
        {
            fileName = "";
            var foundFile = false;
            foreach (var file in _iThmbFileList.Files())
            {
                if (file.FormatId == format.FormatId)
                {
                    foundFile = true;
                    break;
                }
            }
            if (!foundFile)
            {
                //If we didnt find a file ref for specified formatId, create one.
                _iThmbFileList.AddIThmbFile(format);
            }

            fileOffset = 0;
            foreach (var file in _iThmbFileList.Files())
            {
                if (file.FormatId == format.FormatId)
                {
                    for (var i = 1; ; i++)
                    {
                        fileName = $@"F{file.FormatId}_{i}.ithmb";
                        var iThmbPath = Path.Combine(_iPod.FileSystem.ArtworkFolderPath, fileName);

                        if (!_iPod.FileSystem.FileExists(iThmbPath))
                        {
                            fileOffset = 0;
                            return;
                        }

                        //dont let the iThmb file get above 200MB
                        fileOffset = GetNextFreeBlockInIThmb(fileName, format.ImageBlockSize);
                        if (fileOffset < 209715200)
                        {
                            return;
                        }
                    }
                }
            }
        }

        internal uint GetNextFreeBlockInIThmb(string fileName, uint iThmbBlockSize)
        {
            var offsets = new List<uint>();
            foreach (var artwork in _artworkList.Images())
            {
                foreach (var fmt in artwork.Formats)
                {
                    if (fmt.FileName == fileName)
                    {
                        offsets.Add(fmt.FileOffset);
                    }
                }
            }
            offsets.Sort();
            var lastOffset = iThmbBlockSize * -1;
            foreach (var offset in offsets)
            {
                if (lastOffset + iThmbBlockSize < offset)
                {
                    break;
                }

                lastOffset = offset;
            }
            return (uint)(lastOffset + (long)iThmbBlockSize);
        }

        private IPodImage GetTrackArtForTrack(Track track)
        {
            IPodImage artwork;

            if (track.ArtworkIdLink != 0)
            {
                artwork = _artworkList.GetArtById(track.ArtworkIdLink);
                if (artwork != null)
                {
                    return artwork;
                }
            }

            //Failsafe (older database)
            artwork = _artworkList.GetArtByTrackId(track.DBId);
            return artwork;
        }
    }
}
