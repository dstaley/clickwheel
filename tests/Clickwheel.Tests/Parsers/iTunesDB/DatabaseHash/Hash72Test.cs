using System.IO;
using Clickwheel.DatabaseHash;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clickwheel.Tests.Parsers.iTunesDB.DatabaseHash
{
    [TestClass]
    public class Hash72Test
    {
        [TestMethod]
        [DeploymentItem(@"Fixtures/HashInfo")]
        [DeploymentItem(@"Fixtures/ipod-test-db-hashed.db")]
        public void TestRead()
        {
            var info = new HashInfo();
            info.Read("HashInfo");
            CollectionAssert.AreEqual(
                new byte[]
                {
                    0x00, 0x0a, 0x27, 0x00, 0x1e, 0xe9, 0x2d, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                },
                info.Uuid);

            var db = File.ReadAllBytes("ipod-test-db-hashed.db");
            var checksum = Hash72.GenerateDatabaseHash(info, db);
            CollectionAssert.AreEqual(
                new byte[]
                {
                    0x01, 0x00, 0x53, 0x34, 0xA0, 0xA1, 0x1D, 0x64, 0xF1, 0xE7, 0x5C, 0xE5, 0xB6, 0x8C, 0xE1, 0x52,
                    0x06, 0x18, 0xD6, 0x47, 0xAC, 0x8F, 0x56, 0x2B, 0x02, 0x3C, 0x9A, 0x7C, 0xB5, 0x29, 0x1C, 0x9E,
                    0x06, 0x92, 0x13, 0x0B, 0x24, 0xF0, 0x7E, 0x7F, 0xBD, 0x09, 0x1F, 0x03, 0xD8, 0x36
                },
                checksum
            );
        }

        [TestMethod]
        public void TestReadOrGenerate()
        {
            var info = new HashInfo();
            var hashInfoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            info.ReadOrGenerate(hashInfoPath, "000A27001EE92D51");
            CollectionAssert.AreEqual(
                new byte[]
                {
                    0x00, 0x0a, 0x27, 0x00, 0x1e, 0xe9, 0x2d, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                },
                info.Uuid);

            var verifyInfo = new HashInfo();
            verifyInfo.Read(hashInfoPath);
            CollectionAssert.AreEqual(
                new byte[]
                {
                    0x00, 0x0a, 0x27, 0x00, 0x1e, 0xe9, 0x2d, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                },
                info.Uuid);
        }
    }
}