using Clickwheel.DatabaseHash;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clickwheel.Tests.Parsers.iTunesDB.DatabaseHash
{
    [TestClass]
    public class HashInfoTest
    {
        [TestMethod]
        public void TestGenerate()
        {
            var info = new HashInfo();
            info.Generate("000A27001EE92D51");
            CollectionAssert.AreEqual(
                new byte[] { 0x48, 0x41, 0x53, 0x48, 0x76, 0x30 },
                info.Header);
            CollectionAssert.AreEqual(
                new byte[] { 0x00, 0x0a, 0x27, 0x00, 0x1e, 0xe9, 0x2d, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                info.Uuid);
            Assert.AreEqual(12, info.RndPart.Length);
            Assert.AreEqual(16, info.Iv.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Fixtures/HashInfo")]
        public void TestRead()
        {
            var info = new HashInfo();
            info.Read("HashInfo");
            CollectionAssert.AreEqual(
                new byte[] { 0x48, 0x41, 0x53, 0x48, 0x76, 0x30 },
                info.Header);
            CollectionAssert.AreEqual(
                new byte[] { 0x00, 0x0a, 0x27, 0x00, 0x1e, 0xe9, 0x2d, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                info.Uuid);
            Assert.AreEqual(12, info.RndPart.Length);
            Assert.AreEqual(16, info.Iv.Length);
        }
    }
}