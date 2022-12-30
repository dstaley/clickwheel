using System.IO;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers.Artwork
{
    //Implements a MHNI ArtworkDB element
    /// <summary>
    /// Represents a single Artwork image size for a single Track. Each track with artwork will have at least 1 ArtworkFormat.
    /// </summary>
    public class IPodImageFormat : BaseDatabaseElement
    {
        private int _childCount,
            _unk1;
        private uint _formatId,
            _fileOffset,
            _imageSize,
            _iThmbBlockSize;
        private short _verticalPadding,
            _horizontalPadding,
            _height,
            _width;

        private uint _computedWidth,
            _computedHeight;
        private ArtworkStringMHOD _childElement;

        /// <summary>
        /// True if this format is a photo, otherwise its cover art
        /// </summary>
        public bool IsPhoto { get; set; }

        internal IPodImageFormat(bool isPhotoFormat)
        {
            _identifier = "mhni".ToCharArray();
            _requiredHeaderSize = 44;
            _headerSize = 76;
            IsPhoto = isPhotoFormat;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhni");

            _sectionSize = reader.ReadInt32();
            _childCount = reader.ReadInt32();
            _formatId = reader.ReadUInt32();
            _fileOffset = reader.ReadUInt32();
            _imageSize = reader.ReadUInt32();
            _verticalPadding = reader.ReadInt16();
            _horizontalPadding = reader.ReadInt16();
            _height = reader.ReadInt16();
            _width = reader.ReadInt16();
            _unk1 = reader.ReadInt32();
            _iThmbBlockSize = reader.ReadUInt32();

            // The image dimensions reported in db are only approximate. Real dimensions are looked up here based on image formatId Thanks http://ipodlinux.org/ITunesDB
            try
            {
                var fmt = iPod.DeviceInfo.SupportedArtworkFormats.Find(
                    f => f.FormatId == _formatId
                );
                if (fmt != null)
                {
                    _computedWidth = fmt.Width;
                    _computedHeight = fmt.Height;
                }
                else
                {
                    SupportedArtworkFormat.GetArtworkDimensions(
                        _formatId,
                        out _computedWidth,
                        out _computedHeight
                    );
                }
            }
            catch (UnsupportedArtworkFormatException ex)
            {
                DebugLogger.LogException(ex);
            }

            ReadToHeaderEnd(reader);

            if (_childCount > 0)
            {
                _childElement = new ArtworkStringMHOD();
                _childElement.Read(iPod, reader);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_childCount);
            writer.Write(_formatId);
            writer.Write(_fileOffset);
            writer.Write(_imageSize);
            writer.Write(_verticalPadding);
            writer.Write(_horizontalPadding);
            writer.Write(_height);
            writer.Write(_width);
            writer.Write(_unk1);
            writer.Write(_iThmbBlockSize);
            writer.Write(_unusedHeader);

            if (_childElement != null)
            {
                _childElement.Write(writer);
            }
        }

        internal override int GetSectionSize()
        {
            if (_childElement != null)
            {
                return _headerSize + _childElement.GetSectionSize();
            }
            else
            {
                return _headerSize;
            }
        }

        public string FileName
        {
            get
            {
                if (_childElement != null)
                {
                    if (_childElement.Data.StartsWith(":"))
                    {
                        return _childElement.Data.Substring(1);
                    }
                    else
                    {
                        return _childElement.Data;
                    }
                }
                else
                {
                    return $"F{_formatId}_1.ithmb";
                }
            }
        }

        public uint FileOffset
        {
            get => _fileOffset;
            set => _fileOffset = value;
        }

        public uint ImageSize
        {
            get => _imageSize;
            set => _imageSize = value;
        }

        public uint ImageBlockSize
        {
            get => _iThmbBlockSize;
            set => _iThmbBlockSize = value;
        }

        public uint FormatId => _formatId;

        public uint Width => _computedWidth;

        public uint Height => _computedHeight;

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        internal void Create(IPod iPod, SupportedArtworkFormat format, byte[] imageData)
        {
            _iPod = iPod;
            _unusedHeader = new byte[_headerSize - _requiredHeaderSize];

            _imageSize = (uint)imageData.Length;
            _iThmbBlockSize = format.IThmbBlockSize == 0 ? _imageSize : format.IThmbBlockSize;
            _width = (short)format.Width;
            _computedWidth = format.Width;
            _height = (short)format.Height;
            _computedHeight = format.Height;
            _formatId = format.FormatId;

            string iThmbName;
            uint offset = 0;
            _iPod.ArtworkDB.GetIThmbRepository(this, out iThmbName, out offset);
            _fileOffset = offset;

            _childElement = new ArtworkStringMHOD();
            _childElement.Create(Helpers.StandardPathToiPodPath(iThmbName));
            _childCount = 1;

            var fs = _iPod.FileSystem.OpenFile(
                Path.Combine(_iPod.FileSystem.ArtworkFolderPath, FileName),
                FileAccess.ReadWrite
            );
            var writer = new BinaryWriter(fs);
            writer.Seek((int)_fileOffset, SeekOrigin.Begin);
            writer.Write(imageData);
            writer.Close();
        }

        internal void UpdateImageData(byte[] imageData)
        {
            string iThmbName;
            var firstFreeOffset = _iPod.ArtworkDB.GetNextFreeBlockInIThmb(
                FileName,
                _iThmbBlockSize
            );
            if (firstFreeOffset < _fileOffset)
            {
                uint offset = 0;
                _iPod.ArtworkDB.GetIThmbRepository(this, out iThmbName, out offset);
                _childElement.Data = Helpers.StandardPathToiPodPath(iThmbName);
                _fileOffset = offset;
            }

            var fs = _iPod.FileSystem.OpenFile(
                Path.Combine(_iPod.FileSystem.ArtworkFolderPath, FileName),
                FileAccess.ReadWrite
            );

            var writer = new BinaryWriter(fs);
            writer.Seek((int)_fileOffset, SeekOrigin.Begin);

            writer.Write(imageData);
            writer.Close();
        }

        /// <summary>
        /// Returns true if this is a full resolution format. Full resolution images
        /// are stored as the original image files, rather than packed into ithmb files.
        /// The size is reported as 1000x1000 but this is just a placeholder as the size is not known
        /// until the image file is opened.
        /// </summary>
        public bool IsFullResolution => FormatId == 1;
    }
}
