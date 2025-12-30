# iTunesDB

The iTunesDB file consists of a sort of tree structure arranged into a flat file. Each "object" contains a header followed by some other objects. The header contains a couple of size fields that are used to define where the header ends and other objects begin. Here's the basic structure of it:

```
<mhbd> - This is a database
  <mhsd> - This is a list holder, which holds either a mhla
    <mhla> - This holds a list of albums
      <mhia> - This describes a particular Album Item
        <mhod> - These hold strings associated with an album title
        <mhod> - These hold strings associated with an artist name/title
      <mhia> - This is another album. And so on.
        <mhod>...
        <mhod>...
      ...
  <mhsd> - This is a list holder, which holds either a mhlt or an mhlp
    <mhlt> - This holds a list of all the songs on the iPod
      <mhit> - This describes a particular song
        <mhod>... - These hold strings associated with a song
        <mhod>... - Things like Artist, Song Title, Album, etc.
      <mhit> - This is another song. And so on.
        <mhod>...
        <mhod>...
      ...
  <mhsd> - Here's the list holder again.. This time, it's holding an mhlp
    <mhlp> - This holds a bunch of playlists. In fact, all the playlists.
      <mhyp> - This is a playlist.
        <mhod>... - These mhods hold info about the playlists like the name of the list.
        <mhip>... - This mhip holds a reference to a particular song on the iPod.
        ...
      <mhyp> - This is another playlist. And so on.
        <mhod>... - Note that the mhods also hold other things for smart playlists
        <mhip>...
        ...
      ...
```

## Chunk Encoding

Each chunk of the file is encoded in the following form:

Offset | Field | Size | Description
------ | ----- | ---- | -----------
0 | Chunk Type Identifier | 4 | A 4-byte string like "mhbd", "mhlt", etc. letting you know what you're working with. This identifies what type of chunk is at the current location.
4 | End of Type-specific Header | 4 | This is a little-endian encoded 32-bit value that points to the end of the chunk-specific header. Tells you where, relative to offset 0, the header section for this chunk ends. The header starts at offset 12 and runs through to the end of the type-specific header.
8 | End of Chunk or Number of Children | 4 | This is a little-endian encoded 32-bit value. It either points to the end of the current chunk, or the number of children the current chunk has. Usually, it's an "End of Chunk" offset: what the last offset in the current chunk is. That is, it tells you how long this chunk and all its children are. There are two big exceptions to this: "mhlt" and "mhlp" chunks. In both of these, this number is how many top-level children the mhlt/mhlp chunk has.

## Data Types

- [Database (mhbd)](./database.md)
- [DataSet (mhsd)](./dataset.md)
- [TrackList (mhlt)](./tracklist.md)
- [TrackItem (mhit)](./trackitem.md)
- [PlaylistList (mhlp)](./playlistlist.md)
- [Playlist (mhyp)](./playlist.md)
- [PlaylistItem (mhip)](./playlistitem.md)
- [AlbumList (mhla)](./albumlist.md)
- [AlbumItem (mhia)](./albumitem.md)
- [Data (mhod)](./data/index.md)
