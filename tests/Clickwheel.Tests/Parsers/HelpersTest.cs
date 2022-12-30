using System;
using System.IO;
using Clickwheel.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clickwheel.Tests.Parsers
{
    [TestClass]
    public class HelpersTest
    {
        private void AssertEqualDates(DateTime expected, DateTime actual)
        {
            Assert.IsTrue(expected.Equals(actual), $"expected {expected}, but received {actual}");
        }

        [TestMethod]
        public void TestGetDateTimeFromTimeStamp()
        {
            AssertEqualDates(new DateTime(1904, 1, 1), Helpers.GetDateTimeFromTimeStamp(0));
            AssertEqualDates(new DateTime(1970, 1, 1), Helpers.GetDateTimeFromTimeStamp(2082844800));
            AssertEqualDates(new DateTime(2022, 1, 1), Helpers.GetDateTimeFromTimeStamp(3723840000));
        }

        [TestMethod]
        public void TestGetTimeStampFromDate()
        {
            Assert.AreEqual((uint)0, Helpers.GetTimeStampFromDate(new DateTime(1904, 1, 1)));
            Assert.AreEqual((uint)2082844800, Helpers.GetTimeStampFromDate(new DateTime(1970, 1, 1)));
            Assert.AreEqual((uint)3723840000, Helpers.GetTimeStampFromDate(new DateTime(2022, 1, 1)));
        }

        [TestMethod]
        public void TestiPodPathToStandardPath()
        {
            Assert.AreEqual(
                Path.Combine("iPod_Control", "iTunes", "iTunesDB"),
                Helpers.iPodPathToStandardPath("iPod_Control:iTunes:iTunesDB")
            );
        }

        [TestMethod]
        public void TestStandardPathToiPodPaths()
        {
            Assert.AreEqual("iPod_Control:iTunes:iTunesDB", Helpers.StandardPathToiPodPath(@"iPod_Control\iTunes\iTunesDB"));
            Assert.AreEqual("iPod_Control:iTunes:iTunesDB", Helpers.StandardPathToiPodPath(@"iPod_Control/iTunes/iTunesDB"));
        }

        [TestMethod]
        public void TestIntToITunesSDFormat()
        {
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0 }, Helpers.IntToITunesSDFormat(0));
            CollectionAssert.AreEqual(new byte[] { 0, 0, 255 }, Helpers.IntToITunesSDFormat(255));
            CollectionAssert.AreEqual(new byte[] { 255, 255, 254 }, Helpers.IntToITunesSDFormat(16777214));
            CollectionAssert.AreEqual(new byte[] { 255, 255, 255 }, Helpers.IntToITunesSDFormat(16777215));
        }

        [TestMethod]
        public void GetTimeString()
        {
            Assert.AreEqual("00:00:00", Helpers.GetTimeString(0));
            Assert.AreEqual("00:01:00", Helpers.GetTimeString(60));
            Assert.AreEqual("01:00:00", Helpers.GetTimeString(3600));
            Assert.AreEqual("99:59:59", Helpers.GetTimeString(359999));
        }

        [TestMethod]
        public void TestGetFileSizeString()
        {
            Assert.AreEqual("0MB", Helpers.GetFileSizeString(0, 0));
            Assert.AreEqual("1MB", Helpers.GetFileSizeString(1048576, 0));
            Assert.AreEqual("1MB", Helpers.GetFileSizeString(1048577, 0));
            Assert.AreEqual("1024MB", Helpers.GetFileSizeString(1073741824, 0));
            Assert.AreEqual("2GB", Helpers.GetFileSizeString(2147483648, 0));
            Assert.AreEqual("2.5GB", Helpers.GetFileSizeString(2684354560, 1));
            Assert.AreEqual("1.4GB", Helpers.GetFileSizeString(1476395008, 3));
        }
    }
}