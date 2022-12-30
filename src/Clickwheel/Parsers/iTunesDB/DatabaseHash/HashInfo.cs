using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Clickwheel.DatabaseHash
{
    internal class HashInfo
    {
        public byte[] Header; // 6
        public byte[] Uuid; // 20
        public byte[] RndPart; // 12
        public byte[] Iv; // 16

        public void Read(string hashInfoPath)
        {
            if (!File.Exists(hashInfoPath))
            {
                throw new Exception($"Unable to find valid HashInfo at path {hashInfoPath}");
            }

            using var fs = new FileStream(
                hashInfoPath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite
            );
            using var reader = new BinaryReader(fs);
            Header = reader.ReadBytes(6);
            Uuid = reader.ReadBytes(20);
            RndPart = reader.ReadBytes(12);
            Iv = reader.ReadBytes(16);
        }

        public void ReadOrGenerate(string hashInfoPath, string firewireId)
        {
            if (!File.Exists(hashInfoPath))
            {
                Generate(firewireId);
                Write(hashInfoPath);
                return;
            }

            Read(hashInfoPath);
        }

        public void Generate(string firewireId)
        {
            if (firewireId.Length != 16)
            {
                throw new Exception("firewireId must be 16 characters long");
            }

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://ihash.marcan.st/")
            {
                Content = new FormUrlEncodedContent(
                    new Dictionary<string, string> { { "uuid", firewireId }, { "go", "Generate" } }
                )
            };
            var response = client.Send(request);

            using var reader = new BinaryReader(response.Content.ReadAsStream());
            Header = reader.ReadBytes(6);
            Uuid = reader.ReadBytes(20);
            RndPart = reader.ReadBytes(12);
            Iv = reader.ReadBytes(16);
        }

        private void Write(string hashInfoPath)
        {
            using var fs = new FileStream(
                hashInfoPath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite
            );
            using var writer = new BinaryWriter(fs);
            writer.Write(Header);
            writer.Write(Uuid);
            writer.Write(RndPart);
            writer.Write(Iv);
        }
    }
}
