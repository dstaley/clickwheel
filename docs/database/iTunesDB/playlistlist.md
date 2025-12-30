# PlaylistList (`mhlp`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhlp
4 | header length | 4 | size of the mhlp header
8 | number of playlists | 4 | the number of playlists on the iPod. This includes the Library playlist.

The rest of the header is zero padded.

The PlaylistList has all the playlists as its children. The very first playlist *must* be the Library playlist. This is a normal playlist, but it has the special "hidden" bit set in it, and it contains all the songs on the iPod (in no particular order).
