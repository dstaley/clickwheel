using System.IO;

namespace Clickwheel.DatabaseHash
{
    internal static class DatabaseHasher
    {
        public static void Hash(FileStream file, IPod iPod)
        {
            if (iPod.DeviceInfo.FirewireId == null || iPod.DeviceInfo.FirewireId.Length != 16)
            {
                return;
            }

            byte[] hash = null;

            using var reader = new BinaryReader(file);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var contents = new byte[reader.BaseStream.Length];
            reader.Read(contents, 0, contents.Length);

            Zero(ref contents, 0x18, 8);
            Zero(ref contents, 0x32, 20);
            Zero(ref contents, 0x58, 20);

            hash = Hash58.GenerateDatabaseHash(iPod.DeviceInfo.FirewireId, contents);

            using var writer = new BinaryWriter(file);
            writer.Seek(0x58, SeekOrigin.Begin);
            writer.Write(hash, 0, hash.Length);

            if (iPod.ITunesDB.HashingScheme >= 2)
            {
                Zero(ref contents, 0x72, 46);

                var hashInfo = new HashInfo();
                hashInfo.ReadOrGenerate(iPod.FileSystem.HashInfoPath, iPod.DeviceInfo.FirewireId);
                hash = Hash72.GenerateDatabaseHash(hashInfo, contents);

                writer.Seek(0x72, SeekOrigin.Begin);
                writer.Write(hash, 0, 46);
            }
        }

        private static void Zero(ref byte[] buffer, int index, int length)
        {
            for (var i = index; i < index + length; i++)
            {
                buffer[i] = 0;
            }
        }
    }
}
