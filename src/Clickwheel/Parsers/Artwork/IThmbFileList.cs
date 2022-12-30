using System.Collections.Generic;
using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    // Implements a MHLF entry in ArtworkDB
    /// <summary>
    /// List of ithmb artwork files
    /// </summary>
    class IThmbFileList : BaseDatabaseElement
    {
        private List<IThmbFile> _childSections;

        public IThmbFileList()
        {
            _requiredHeaderSize = 12;
            _childSections = new List<IThmbFile>();
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhlf");

            var fileCount = reader.ReadInt32();

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < fileCount; i++)
            {
                var file = new IThmbFile();
                file.Read(iPod, reader);
                _childSections.Add(file);
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

        /// <summary>
        /// Enumerates each IThmbFile in this list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IThmbFile> Files()
        {
            foreach (var file in _childSections)
            {
                yield return file;
            }
        }

        internal void AddIThmbFile(IPodImageFormat format)
        {
            var newFile = new IThmbFile();
            newFile.Create(format.ImageSize, format.FormatId);
            _childSections.Add(newFile);
        }
    }
}
