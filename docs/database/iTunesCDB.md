# iTunesCDB

The iTunesCDB file is a compressed variant of the [iTunesDB](./iTunesDB/index.md) used on the fifth-, sixth-, and seventh-generation iPod nano. The file is located at `/iPod_Control/iTunes/iTunesCDB`.

## File Format

The iTunesCDB uses the same [mhbd header structure](./iTunesDB/database.md) as the standard iTunesDB for the first portion of the file. However, after the mhbd header, the remaining content (all child mhsd sections) is zlib-compressed.

### Structure

```
┌─────────────────────────────────┐
│ mhbd header (uncompressed)      │
│ - identifier: "mhbd"            │
│ - header_length                 │
│ - section_size (includes        │
│   compressed data length)       │
│ - version, id, hashing scheme,  │
│   etc.                          │
├─────────────────────────────────┤
│ zlib-compressed data            │
│ - Contains all mhsd children    │
│ - Same structure as iTunesDB    │
│   once decompressed             │
└─────────────────────────────────┘
```

## SQLite Databases

Devices that use iTunesCDB also require a set of SQLite databases located in the `/iPod_Control/iTunes/iTunes Library.itlp` folder. These databases must be kept in sync with the iTunesCDB content. 

### Database Files

Filename | Description
-------- | -----------
`Library.itdb` | Main library database containing tracks, albums, artists, playlists, and metadata
`Locations.itdb` | Maps track PIDs to file paths on the device
`Dynamic.itdb` | Runtime/dynamic data (play counts, ratings, etc.)
`Extras.itdb` | Chapter and lyric data
`Genius.itdb` | Metadata for the Genius feature
`Locations.itdb.cbk` | Integrity checksum file for Locations.itdb. Hash72 on iPod nano (5th generation) (166 bytes), and HashAB on iPod nano (6th generation) and iPod nano (7th generation) (177 bytes).
