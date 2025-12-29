# AlbumList (`mhla`)

Album List, first seen with iTunes 7.1. It was seen on iPod nano 2nd gen and Shuffle 2nd gen restored with iTunes 7.1.1, but not on iPod Video 30GB(restored with the same version of iTunes).

The Album List has Album Items as its children. The number of Album Items is the same all albums on iPod.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhla
4 | header length | 4 | size of the mhla header
8 | number of album items | 4 | the total number of songs in the Album List

The rest of the header is zero padded.
