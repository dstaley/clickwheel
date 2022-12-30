using System.Collections.Generic;
using System.IO;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers
{
    /// <summary>
    /// Internally used by Clickwheel. Should not be used from external code.
    /// </summary>
    public abstract class BaseDatabaseElement
    {
        protected char[] _identifier;
        protected int _headerSize;
        protected int _sectionSize;
        protected byte[] _unusedHeader;
        protected int _requiredHeaderSize;
        protected IPod _iPod;

        protected bool ValidateHeader(string validIdentifier)
        {
            var strIdentifier = new string(_identifier);
            if (strIdentifier != validIdentifier)
            {
                throw new ParseException(
                    validIdentifier + " expected, but " + strIdentifier + " found",
                    null
                );
            }

            if (_headerSize < _requiredHeaderSize)
            {
                throw new UnsupportedITunesVersionException(
                    $"Expected {strIdentifier} section with length {_requiredHeaderSize}, but found length {_headerSize}.",
                    CompatibilityType.NotWritable
                );
            }
            return true;
        }

        protected void ReadToHeaderEnd(BinaryReader reader)
        {
            var unusedHeaderSize = _headerSize - _requiredHeaderSize;
            _unusedHeader = new byte[unusedHeaderSize];
            _unusedHeader = reader.ReadBytes(unusedHeaderSize);
        }

        internal virtual void Read(IPod iPod, BinaryReader reader)
        {
            _iPod = iPod;
        }

        internal abstract void Write(BinaryWriter writer);
        internal abstract int GetSectionSize();

        internal StringMHOD GetChildByType(List<BaseMHODElement> children, int type)
        {
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] is StringMHOD && children[i].Type == type)
                {
                    return (StringMHOD)children[i];
                }
            }
            return null;
        }

        internal string GetDataElement(List<BaseMHODElement> children, int type)
        {
            var mhod = GetChildByType(children, type);
            if (mhod != null)
            {
                return mhod.Data;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
