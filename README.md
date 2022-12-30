<p align="center">
<img width="300" src="https://raw.githubusercontent.com/dstaley/clickwheel/main/clickwheel-wordmark-on-light.svg#gh-light-mode-only" />
<img width="300" src="https://raw.githubusercontent.com/dstaley/clickwheel/main/clickwheel-wordmark-on-dark.svg#gh-dark-mode-only" />
</p>

Clickwheel is a modern cross-platform iPod management API for .NET.

## Installing

Clickwheel is available via NuGet.

| Package Name | Version (NuGet) |
| ------------ | --------------- |
| `Clickwheel` | [![NuGet](https://img.shields.io/nuget/v/Clickwheel.svg)](https://www.nuget.org/packages/Clickwheel/) |

## Usage

```csharp
using Clickwheel;

var ipod = IPod.GetConnectedIPod();
var track = new NewTrack
{
    FilePath = "01 Run Away With Me.m4a",
    Album = "E•MO•TION",
    Artist = "Carly Rae Jepsen",
    AlbumArtist = "Carly Rae Jepsen",
    Title = "Run Away With Me",
    IsVideo = false,
    ArtworkFile = "album.jpg",
    Year = 2015,
    DiscNumber = 1,
    TotalDiscCount = 1,
    TrackNumber = 1,
    AlbumTrackCount = 12,
    Genre = "Pop",
};

var addedTrack = ipod.Tracks.Add(track);
addedTrack.Rating = new IPodRating(5);

var playlist = ipod.Playlists.Add("National Anthems");
playlist.AddTrack(addedTrack);

// Write changes to iPod
ipod.SaveChanges();
```

## Compatibility

| Family       | Generation | Supported         | Supported Firmware Version        |
| ------------ | ---------- | ----------------- | --------------------------------- |
| iPod         | 1st        | Supported         |                                   |
|              | 2nd        | Supported         |                                   |
|              | 3rd        | Supported         |                                   |
|              | 4th        | Supported         | 3.1.1 (Monochrome), 1.2.1 (Color) |
|              | 5th        | Supported         | 1.3                               |
|              | 6th        | Supported         | 1.1.2                             |
| iPod mini    | 1st        | Supported         |                                   |
|              | 2nd        | Supported         | 1.4.1                             |
| iPod nano    | 1st        | Supported         | 1.3.1                             |
|              | 2nd        | Supported         | 1.1.3                             |
|              | 3rd        | Supported         | 1.1.3                             |
|              | 4th        | Supported         | 1.0.4                             |
|              | 5th        | Supported         | 1.0.2                             |
|              | 6th        | Not Supported[^1] |                                   |
|              | 7th        | Not Supported[^1] |                                   |
| iPod shuffle | 1st        | Supported         | 1.1.5                             |
|              | 2nd        | Supported         | 1.0.4                             |
|              | 3rd        | Not Supported[^2] |                                   |
|              | 4th        | Not Supported[^2] |                                   |
| iPod touch   | All        | Not Supported[^3] |                                   |


[^1]: Uses [HashAB](#hashab)
[^2]: VoiceOver generation currently unsupported
[^3]: Clickwheel doesn't support iOS devices

## Extended SysInfo

iPods that support album artwork store some additional system attributes which are accessible via either a SCSI INQUIRY command or a USB Control Transfer (starting with the 6th generation iPod, the 3rd generation iPod nano, and the 3rd generation iPod shuffle). Clickwheel requires this data to be present in a file named `SysInfoExtended` in the iPod's `iPod_Control/Device` folder for proper album artwork support and database hashing. How this file is created depends on your operating system.

### Windows

On Windows, you can use [Clickwheel Device Helper](https://github.com/dstaley/clickwheel/blob/main/src/Clickwheel.DeviceHelper.GUI/README.md) to create the `SysInfoExtended` file for all attached iPods.

### Linux

On Linux, you can use `ipod-read-sysinfo-extended` to obtain the `SysInfoExtended` file. This command is provided by the following packages:

| Distro        | Package         |
| ------------- | --------------- |
| Arch          | `libgpod`       |
| Debian/Ubuntu | `libgpod4`      |
| Fedora        | `libgpod`       |
| openSUSE      | `libgpod-tools` |

### macOS

Unfortunately, modern versions of macOS do not support issuing SCSI INQUIRY requests to attached USB devices from userspace. If you're using an iPod that supports retrieving the extended SysInfo via a USB Control Transfer (any iPod released in 2007 or later), you can use [this](https://github.com/dstaley/ipod-read-sysinfo-extended-macos) program to write the `SysInfoExtended` file. Otherwise, you'll need to use a Windows or Linux device.

## HashAB

HashAB is the name given to the as-of-yet unbroken database hashing scheme used on the 6th and 7th generation iPod nanos. Clickwheel does not support devices that use HashAB.