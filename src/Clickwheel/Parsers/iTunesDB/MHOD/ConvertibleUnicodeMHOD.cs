using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    class ConvertibleUnicodeMHOD<T> : UnicodeMHOD where T : Helpers.IStringConvertible<T>
    {
        public T ResolvedData {
            get;
            private set;
        }
        
        internal override void Read(IPod iPod, BinaryReader reader) {
            base.Read(iPod, reader);
            ResolvedData = T.DecodeFromString(Data);
        }

        internal override void Write(BinaryWriter writer) {
            Data = ResolvedData.EncodeAsString();
            base.Write(writer);
        }
    }
}