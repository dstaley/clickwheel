using System.Collections.Generic;
using System.IO;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers.Artwork
{
    // Implements a MHBA entry in ArtworkDB / PhotoDB
    /// <summary>
    /// An Image Album
    /// </summary>
    public class ImageAlbum : BaseDatabaseElement
    {
        int _id;

        private List<BaseMHODElement> _dataObjects = new List<BaseMHODElement>();
        private List<ImageAlbumItem> _images = new List<ImageAlbumItem>();

        internal ImageAlbum()
        {
            _requiredHeaderSize = 24;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);

            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhba");

            _sectionSize = reader.ReadInt32();
            var dataObjectCount = reader.ReadInt32();
            var imageCount = reader.ReadInt32();
            _id = reader.ReadInt32();

            base.ReadToHeaderEnd(reader);

            for (var i = 0; i < dataObjectCount; i++)
            {
                var mhod = new ArtworkStringMHOD();
                mhod.Read(iPod, reader);
                _dataObjects.Add(mhod);
            }

            for (var i = 0; i < imageCount; i++)
            {
                var item = new ImageAlbumItem();
                item.Read(iPod, reader);
                _images.Add(item);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_dataObjects.Count);
            writer.Write(_images.Count);
            writer.Write(_id);
            writer.Write(_unusedHeader);

            for (var i = 0; i < _dataObjects.Count; i++)
            {
                _dataObjects[i].Write(writer);
            }

            for (var i = 0; i < _images.Count; i++)
            {
                _images[i].Write(writer);
            }
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize;
            foreach (var mhod in _dataObjects)
            {
                size += mhod.GetSectionSize();
            }

            foreach (var item in _images)
            {
                size += item.GetSectionSize();
            }

            return size;
        }

        internal void ResolveImages(ImageList images)
        {
            foreach (var item in _images)
            {
                var art = images.GetArtById(item.ImageId);
                item.Artwork = art;
            }
        }

        /// <summary>
        /// Title of this Image Album
        /// </summary>
        public string Title
        {
            get
            {
                var name = base.GetDataElement(_dataObjects, MHODElementType.Title);
                if (string.IsNullOrEmpty(name))
                {
                    name = "Unnamed";
                }

                return name;
            }
        }

        /// <summary>
        /// Number of images in this album
        /// </summary>
        public int ImageCount => _images.Count;

        public IEnumerable<IPodImage> Images
        {
            get
            {
                foreach (var item in _images)
                {
                    if (item.Artwork != null)
                    {
                        yield return item.Artwork;
                    }
                }
            }
        }
    }
}
