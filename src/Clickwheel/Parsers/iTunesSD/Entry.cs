using System;
using System.IO;
using System.Text;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers.iTunesSD
{
    class Entry : BaseDatabaseElement
    {
        int _entrySize;
        int _unk1;
        byte[] _unk2;
        int _volume;
        int _unk3;
        string _fileName;
        bool _shuffleFlag;
        bool _bookmarkFlag;
        byte _unk4;

        public Entry(Track track)
        {
            _entrySize = 558;
            _unk1 = 0x5aa501;
            _unk2 = new byte[18];
            _unk3 = 0x200;
            _volume = 0x64;
            _fileName = "/" + track.FilePath;
            _shuffleFlag = true;
            _bookmarkFlag = false;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(Helpers.IntToITunesSDFormat(_entrySize));
            writer.Write(Helpers.IntToITunesSDFormat(_unk1));
            writer.Write(_unk2);
            writer.Write(Helpers.IntToITunesSDFormat(_volume));
            writer.Write(Helpers.IntToITunesSDFormat(GetFileType()));
            writer.Write(Helpers.IntToITunesSDFormat(_unk3));
            writer.Write(GetSDFormatFileName());
            writer.Write(_shuffleFlag);
            writer.Write(_bookmarkFlag);
            writer.Write(_unk4);
        }

        internal override int GetSectionSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private byte[] GetSDFormatFileName()
        {
            _fileName = _fileName.Replace("\\", "/");
            var bytes = new byte[522];
            var filename = UnicodeEncoding.Unicode.GetBytes(_fileName);
            filename.CopyTo(bytes, 0);
            return bytes;
        }

        private int GetFileType()
        {
            string extension = null;
            if (_fileName.Length > 3)
            {
                extension = _fileName.ToLower().Substring(_fileName.Length - 3);
            }

            if (extension == "mp3")
            {
                return 1;
            }
            else if (extension == "aac" || extension == "m4a")
            {
                return 2;
            }
            else if (extension == "wav")
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
    }
}
