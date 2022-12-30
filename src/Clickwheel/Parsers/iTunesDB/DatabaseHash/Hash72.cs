using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace Clickwheel.DatabaseHash
{
    [SuppressMessage(
        "Microsoft.Cryptography",
        "CA5350:Do Not Use Weak Cryptographic Algorithms",
        Justification = "iPods require SHA1"
    )]
    [SuppressMessage(
        "Security",
        "CA5401:Do not use CreateEncryptor with non-default IV",
        Justification = "iPods depend on using the same IV"
    )]
    internal static class Hash72
    {
        private static readonly byte[] AES_KEY = { 0x61, 0x8c, 0xa1, 0x0d, 0xc7, 0xf5, 0x7f, 0xd3, 0xb4, 0x72, 0x3e, 0x08, 0x15, 0x74, 0x63, 0xd7 };

        public static byte[] GenerateDatabaseHash(HashInfo info, byte[] iTunesDB)
        {
            using var sha1 = SHA1.Create();
            var sha1Digest = sha1.ComputeHash(iTunesDB);

            var hash = CalculateHash(sha1Digest, info.RndPart, info.Iv);
            return hash;
        }

        public static byte[] CalculateHash(byte[] digest, byte[] rndPart, byte[] iv)
        {
            var signature = new byte[46];
            var plaintext = new byte[32];

            Array.Copy(digest, plaintext, 20);
            Array.Copy(rndPart, 0, plaintext, 20, 12);

            signature[0] = 0x01;
            signature[1] = 0x00;

            Array.Copy(rndPart, 0, signature, 2, 12);

            var output = EncryptWithAes(plaintext, AES_KEY, iv);

            Array.Copy(output, 0, signature, 14, 32);

            return signature;
        }

        private static byte[] EncryptWithAes(byte[] plaintext, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var bw = new BinaryWriter(cs))
            {
                bw.Write(plaintext);
            }

            return ms.ToArray();
        }
    }
}
