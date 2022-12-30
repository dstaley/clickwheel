using System;
using System.Diagnostics;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers.Artwork
{
    internal class PhotoDB : BaseDatabase
    {
        private ArtworkDBRoot _databaseRoot;
        private ImageAlbumList _albumList;
        private bool _isDirty = false;

        public PhotoDB(IPod iPod)
        {
            _iPod = iPod;
            _databaseFilePath = iPod.FileSystem.PhotoDBPath;
        }

        public override void Parse()
        {
            if (!_iPod.FileSystem.FileExists(_databaseFilePath))
            {
                return;
            }

            _databaseRoot = new ArtworkDBRoot();
            try
            {
                ReadDatabase(_databaseRoot);
                var imageList = (
                    (ImageListContainer)_databaseRoot
                        .GetChildSection(MHSDSectionType.Images)
                        .GetListContainer()
                ).ImageList;
                _albumList = (
                    (ImageAlbumListContainer)_databaseRoot
                        .GetChildSection(MHSDSectionType.Albums)
                        .GetListContainer()
                ).ImageAlbumList;

                foreach (var art in imageList.Images())
                {
                    art.IsPhotoFormat = true;
                }

                _albumList.ResolveImages(imageList);
            }
            catch (Exception ex)
            {
                //Swallow any PhotoDB parsing issues for now. We never write this file out anyway.
                DebugLogger.LogException(ex);
            }
            Trace.WriteLine("PhotoDB: " + _compatibility);
        }

        public override void Save()
        {
            if (_databaseRoot == null)
            {
                return;
            }

            AssertIsWritable();
            Debug.WriteLine("Saving PhotoDB " + DateTime.Now);
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

        public ImageAlbumList PhotoAlbumList => _albumList;

        public override int Version => 0;
    }
}
