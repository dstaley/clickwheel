using System.Collections.Generic;
using System.IO;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.iTunesDB;
using SixLabors.ImageSharp;

namespace Clickwheel.Parsers.Artwork
{
    // Implements a MHLI entry in ArtworkDB / PhotoDB
    /// <summary>
    /// List of iPod images
    /// </summary>
    class ImageList : BaseDatabaseElement
    {
        private List<IPodImage> _childSections;

        public ImageList()
        {
            _requiredHeaderSize = 12;
            _childSections = new List<IPodImage>();
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhli");

            var imageCount = reader.ReadInt32();

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < imageCount; i++)
            {
                var mhii = new IPodImage();
                mhii.Read(iPod, reader);
                _childSections.Add(mhii);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_childSections.Count);
            writer.Write(_unusedHeader);

            for (var i = 0; i < _childSections.Count; i++)
            {
                _childSections[i].Write(writer);
            }
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize;
            for (var i = 0; i < _childSections.Count; i++)
            {
                size += _childSections[i].GetSectionSize();
            }
            return size;
        }

        internal IPodImage GetArtByTrackId(long dbId)
        {
            return _childSections.Find(a => a.TrackDBId == dbId && a.UsedCount > 0);
        }

        internal IPodImage GetArtById(uint id)
        {
            return _childSections.Find(a => a.Id == id);
        }

        internal void AddNewArtwork(Track track, Image image)
        {
            if (_iPod.DeviceInfo.SupportedArtworkFormats.Count == 0)
            {
                throw new NoSupportedArtworkException();
            }
            var newTrackArt = new IPodImage();
            newTrackArt.Create(_iPod, track, image);
            _childSections.Add(newTrackArt);

            foreach (var format in newTrackArt.Formats)
            {
                track.Artwork.Add(format);
            }
            track.ArtworkIdLink = newTrackArt.Id;
        }

        /// <summary>
        /// Enumerates each image in this ImageList.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPodImage> Images()
        {
            foreach (var art in _childSections)
            {
                yield return art;
            }
        }

        internal void RemoveArtwork(IPodImage existingArt)
        {
            _childSections.Remove(existingArt);
        }
    }
}
