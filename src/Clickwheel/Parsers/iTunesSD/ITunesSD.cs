using System.IO;

namespace Clickwheel.Parsers.iTunesSD
{
    class ITunesSD
    {
        IPod _iPod;
        Header _header;

        public ITunesSD(IPod iPod)
        {
            _iPod = iPod;
            _header = new Header(iPod);
        }

        public void Backup()
        {
            var iTunesSDPath = _iPod.FileSystem.ITunesSDPath;
            if (_iPod.FileSystem.FileExists(iTunesSDPath))
            {
                File.Copy(iTunesSDPath, iTunesSDPath + ".spbackup", true);
            }
        }

        public void Generate()
        {
            var iTunesSDPath = _iPod.FileSystem.ITunesSDPath;
            var fs = new FileStream(iTunesSDPath, FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(fs);

            _header.Write(writer);
            writer.Flush();
            writer.Close();
        }
    }
}
