# Library Playlist Index (52)

The type 52 MHOD is only found as a child of the Main Library Playlist. It is an index of the mhit's ordered by the major categories in the Browse menu. The purpose of these mhod's is to speed up the operation of the Browse menu itself. This is how it displays the information so quickly when selecting one of the major categories, it's all presorted for the iPod in these MHOD's.

Note that this MHOD is not mandatory, however the iPod menu system will operate much slower without it (on large libraries), as it will have to build the information provided here on the fly. Therefore it is recommended to build this MHOD for anything more than trivial numbers of songs.

Essentially, every MHIT is numbered from 0 to the total number of songs-1. The type 52 MHOD contains a list of these index numbers using one of the strings contained in these MHIT's, ordered alphabetically.

To build one of these, take all your songs, order them alphabetically by one of these fields, then simply insert the index numbers of the ordered songs into the type 52 mhod.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header.
8 | total length | 4 | size of the header and all the index entries
12 | type | 4 | the type indicator  (52)
16 | unk1 | 4 | unknown (always zero)
20 | unk2 | 4 | unknown (always zero)
24 | index type | 4 | what this index is sorted on (see list below)
28 | count | 4 | number of entries. Always the same as the number of entries in the playlist, which is the same as the number of songs on the iPod.
32 | null padding | 40 | lots of padding
72 | index entries | 4 * count | The index entries themselves. This is an index into the mhit list, in order, starting from 0 for the first mhit.

Library Playlist Index mhods are **NOT** zero padded.

## Types

Type Number | Indexed Field
----------- | -------------
0x03 | Title
0x04 | Album, then Disc/Tracknumber, then Title
0x05 | Artist, then Album, then Disc/Tracknumber, then Title
0x07 | Genre, then Artist, then Album, then Disc/Tracknumber, then Title
0x12 | Composer, then Title
0x1d | Observed with iTunes 7.2, probably sorted by 'Show' first. Someone with TV shows on his iPod please fill in the secondary sort orders.
0x1e | Observed with iTunes 7.2, probably sorted by 'Season Number' first. Someone with TV shows on his iPod please fill in the secondary sort orders.
0x1f | Observed with iTunes 7.2, probably sorted by 'Episode Number' first. Someone with TV shows on his iPod please fill in the secondary sort orders.
0x23 | Observed with iTunes 7.3 (but may possibly exist in earlier versions, too), unknown what this does.
0x24 | Observed with iTunes 7.3 (but may possibly exist in earlier versions, too), unknown what this does.

(Note that the above list roughly matches the limit sort list.. I think that these lists are actually the same in some way.)
