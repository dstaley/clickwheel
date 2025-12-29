# DataSet (`mhsd`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhsd
4 | header length | 4 | size of the mhsd header
8 | total length | 4 | size of the header and all child records
12 | type | 4 | A type number (see "Types" below).

The rest of the header is zero padded.

Depending on the type of DataSet, it will contain either a Track List child or a Playlist List child. Order is not guaranteed. Example files have contained the type 3 MHSD before the type 2 MHSD. In order for the iPod to list podcasts the type 3 DataSet MUST come between the type 1 and type 2 DataSets.

## Types

```
1 = Track list - contains an MHLT
2 = Playlist List - contains an MHLP
3 = Podcast List - optional, probably. Contains an MHLP also. This MHLP is basically the same as the full playlist section, except it contains the podcasts in a slightly different way. See the Playlists section.
4 = Album List, first seen with iTunes 7.1.
5 = New Playlist List with Smart Playlists, first seen with iTunes 7.3.
```
