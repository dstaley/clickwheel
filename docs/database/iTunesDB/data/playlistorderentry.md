# Playlist Order Entry (100)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header. This is always 0x18 for this type of MHOD.
8 | total length | 4 | size of the header and it's data. This is always 0x2C for this type of MHOD.
12 | type | 4 | the type indicator  ( 100 )
16 | unk1 | 4 | unknown, always 0
20 | unk2 | 4 | unknown, always 0
24 | position | 4 | Position of the song in the playlist. These numbers do not have to be sequentially ordered, numbers can be skipped.
28 | unknown | 16 | zero padding
