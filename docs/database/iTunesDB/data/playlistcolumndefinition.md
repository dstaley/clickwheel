# Playlist Column Definition (100)

This data object is at the beginning of every playlist (before any MHIP entries). Only iTunes puts it there, and only iTunes uses it. It contains information on what columns to display, and what size to display them as, when displaying the playlist in iTunes when the iPod is in a manual mode. This is absolutely optional. The iPod itself doesn't appear to use it in any way.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header. This is always 0x18 for this type of MHOD.
8 | total length | 4 | size of the header and it's data. This is always 0x288 for this type of MHOD.
12 | type | 4 | the type indicator  ( 100 )
16 | unk1 | 4 | unknown, always 0
20 | unk2 | 4 | unknown, always 0
24 | unk3 | 4 | unknown, always 0
28 | unk4 | 8 | unknown, appears to be 0x0088004A02EF0281 for everything except normal playlists. Some kind of identifier?
36 | unk8 | 4 | unknown, always 0
40 | unk9 | 2 | unknown, appears to be 130 for normal playlists and 200 for everything else
42 | unk10 | 2 | unknown, appears to be 3 for the iPod library and 1 for everything else
44 | sort type | 4 | the sort type for this playlist. Use this value to figure out which column is selected by mapping it to the correct column ID (listed below).
48 | number of columns | 4 | the number of columns visible in iTunes for this playlist. In iTunes 6, this value can be anywhere from 2 to 28.
52 | unknown1 | 2 | unknown (always 1?) (Selected column?)
54 | unknown2 | 2 | unknown (always 0?) (Column sort direction? asc/desc)
56 | column definitions | 16 * number of columns | the column definitions

After the column definitions, the rest of the MHOD is zero-padded.

## Column Definition

Each column definition only consists of an ID for the playlist and the sort direction for the column. The order they appear in this MHOD are the order they appear in iTunes, from left to right. The first two columns are always song position and title, in that order.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | ID | 2 | the ID for this column, see below for possible values
2 | width | 2 | the width of the column, in pixels
4 | sort direction | 4 | if equal to 0x1, the sort is reversed for this column. Set to 0x0 otherwise.
8 | unknown | 4 | seems to be null padding
12 | unknown | 4 | seems to be null padding

## Column IDs

ID | Description
-- | -----------
0x01 | position; leftmost column in all playlists
0x02 | Name
0x03 | Album
0x04 | Artist
0x05 | Bit Rate
0x06 | Sample Rate
0x07 | Year
0x08 | Genre
0x09 | Kind
0x0A | Date Modified
0x0B | Track Number
0x0C | Size
0x0D | Time
0x0E | Comment
0x10 | Date Added
0x11 | Equalizer
0x12 | Composer
0x14 | Play Count
0x15 | Last Played
0x16 | Disc Number
0x17 | My Rating
0x19 | Date Released (Podcasts group only)
0x1A | BPM
0x1C | Grouping
0x1E | Category
0x1F | Description
0x21 | Show
0x22 | Season
0x23 | Episode Number
