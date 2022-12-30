using System.Collections.Generic;
using System.IO;
using Clickwheel.IPodDevice.FileSystems;

namespace Clickwheel
{
    /// <summary>
    /// Contains Clickwheel-specific methods.
    /// </summary>
    public static class Clickwheel
    {
        private static List<DeviceFileSystem> _registeredFileSystems = new List<DeviceFileSystem>();

        static Clickwheel()
        {
            var iPodProfile = new StandardFileSystem(
                "IPod",
                Path.Combine("iPod_Control", "iTunes"),
                @"iPod_Control",
                Path.Combine("iPod_Control", "Artwork"),
                @"Photos"
            );
            iPodProfile.ParseDbFilesLocally = true;
            _registeredFileSystems.Add(iPodProfile);
        }

        /// <summary>
        /// List of device FileSystems Clickwheel will use when searching for iPods. This list can be updated
        /// dynamically before calling GetConnectediPod().
        /// </summary>
        public static List<DeviceFileSystem> RegisteredFileSystems => _registeredFileSystems;
    }
}
