using System;
using System.Collections.Generic;
using Clickwheel.Parsers.Artwork;

namespace Clickwheel.IPodDevice.FileSystems
{
    /// <summary>
    /// Holds information about the iPod (Type, FirewireId, Serial #, supported artwork formats etc.)
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// Exception which occured while retrieving device information
        /// </summary>
        Exception ReadException { get; }

        /// <summary>
        /// FirewireId of iPod. Used to generate iTunesDB database hash.
        /// </summary>
        string FirewireId { get; }

        /// <summary>
        /// Serial number of iPod.
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Type of iPod.
        /// </summary>
        IPodFamily Family { get; }

        /// <summary>
        /// If Family is unknown, FamilyId can be used until Clickwheel is updated to include the new value in the IPodFamily enum.
        /// </summary>
        int FamilyId { get; }

        /// <summary>
        /// List of artwork formats supported by this iPod.
        /// </summary>
        List<SupportedArtworkFormat> SupportedArtworkFormats { get; }

        /// <summary>
        /// List of photo formats supported by this iPod.
        /// </summary>
        List<SupportedArtworkFormat> SupportedPhotoFormats { get; }

        /// <summary>
        /// Most disk-based iPod's can provide a device descriptor when queried. This is the raw result. Useful if you need more information about
        /// the iPod than IDeviceInfo provides.
        /// </summary>
        string RawDeviceDescriptor { get; }

        string OSVersion { get; }
    }
}
