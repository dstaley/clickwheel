using System.IO;
using System.Text;
using Clickwheel.DatabaseHash;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clickwheel.Tests.Parsers.iTunesDB.DatabaseHash
{
    [TestClass]
    public class Hash58Test
    {
        [TestMethod]
        [DeploymentItem(@"Fixtures/ipod-test-db-hashed.db")]
        public void TestHash58()
        {
            byte[] db = File.ReadAllBytes("ipod-test-db-hashed.db");
            byte[] result = Hash58.GenerateDatabaseHash("000A27001A26973B", db);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.AppendFormat("{0:x}", b);
            }
            Assert.AreEqual("9134cb64dfa38c16dbcfb023768ddcaf8fbe34a2", sb.ToString());
        }
    }
}
