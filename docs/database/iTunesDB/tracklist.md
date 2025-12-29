# TrackList (`mhlt`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhlt
4 | header length | 4 | size of the mhlt header
8 | number of songs | 4 | the total number of songs in the TrackList

The rest of the header is zero padded.

The TrackList has TrackItems as its children. The number of TrackItems is the same as the number of songs.
