## PlaylistItem (`mhip`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhip
4 | header length | 4 | size of the mhip header
8 | total length | 4 | size of the header and all child records
12 | Data Object Child Count | 4 | number of mhod following this Playlist Item (always 1 so far)
16 | Podcast Grouping Flag | 2 | Formerly unk1. 0x0 = normal file. 0x100 = Podcast Group. Podcast Groups will be followed by a single child, an MHOD type 1 string, which specifies the name of the Podcast Group. They will also have a 0 (zero) for the Track ID. This field used to be some kind of correlation ID or something, but this may have been a bug. In any case, the old way breaks iPods now, and this should be set to zero on all normal songs.
18 | unk4 | 1 | 0 or 1 in iTunes 7.2.
19 | unk5 | 1 | 0 or 8 in iTunes 7.2.
20 | Group ID (?) | 4 | Formerly unk2. A unique ID for the track. It appears it is made sure that this ID does not correspond to any real track ID. Doesn't seem to correlate to anything, but other bits reference it. See Podcast Grouping Reference below.
24 | track ID | 4 | the ID number of the track in the track list. See Track Item for more info
28 | timestamp | 4 | some kind of time stamp, possibly time the song was added to the playlist
32 | Podcast Grouping Reference | 4 | Formerly unk3. This is the parent group that this podcast should be listed under. It should be zero the rest of the time.

The rest of the header is zero padded.

For purposes of size calculations, Playlist Items have no children. However, every Playlist Item is invariably followed by a Data Object of type 100, which contains nothing but a number that is used to order/sort the playlist. See the [Playlist](./playlist.md) description for more information.

Please note that starting with iTunes 4.9 (mhbd file version number 0x0d) the Type 100 MHOD following the Playlist Item is considered a child of the Playlist Item and is included into the size calculation. The old behaviour was probably a bug in iTunes.
