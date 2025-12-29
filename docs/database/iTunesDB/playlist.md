# Playlist (`mhyp`)

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhyp
4 | header length | 4 | size of the mhyp header
8 | total length | 4 | size of the header and all child records
12 | Data Object Child Count | 4 | number of Data Objects in the List
16 | Playlist Item Count | 4 | number of Playlist Items in the List
20 | Is Master Playlist flag | 1 | 1 if the playlist is the Master (Library) playlist, 0 if it is not. Only the Master (Library) Playlist should have a 1 here.
21 | unk | 3 | Probably three more flags, the first of which has been observed to have been set to 1 for some playlists.
24 | timestamp | 4 | time of the playlists creation
28 | persistent playlist ID | 8 | a unique, randomly generated ID for the playlist
36 | unk3 | 4 | Always zero?
40 | String MHOD Count | 2 | This appears to be the number of string MHODs (type < 50) associated with this playlist (typically 0x01).  Doesn't seem to be signficant unless you include Type 52 MHODs.  Formerly known as unk4.
42 | Podcast Flag | 2 | This is set to 0 on normal playlists and to 1 for the Podcast playlist. If set to 1, the playlist will not be shown under 'Playlists' on the iPod, but as 'Podcasts' under the Music menu.. The actual title of the Playlist does not matter. If more than one playlist is set to 1, they don't show up at all. They also don't show up if you set this to 1 on one playlist and 2 on the other. Please note that podcast playlists are organized slightly different than ordinary playlists (see below).
44 | List Sort Order | 4 | The field that the playlist will be sorted by. See list below.

The rest of the header is zero padded.

The structure of the Playlists are different than most others. Each Playlist looks like this, conceptually:

```
<mhyp>
   <mhod type=1>Playlist Name</mhod>
   <mhod type=50>Smart Playlist Info</mhod> (optional)
   <mhod type=51>Smart Playlist Rules</mhod> (optional)
   ...
   <mhip>Playlist Item</mhip>
          <mhod type=100>Position Indicator</mhod>
   <mhip>Playlist Item</mhip>
          <mhod type=100>Position Indicator</mhod>
   ...
</mhyp>
```

The point being that these are all considered in the MHYP for the size calculations. However, in the "Data Object" child count, ONLY those MHODs that come before the first MHIP are counted. The "position indicators" are children of the MHIPs (older firmwares had a bug in this respect).

## List Sort Order

```
1 - playlist order (manual sort order)
2 - ???
3 - songtitle
4 - album
5 - artist
6 - bitrate
7 - genre
8 - kind
9 - date modified
10 - track number
11 - size
12 - time
13 - year
14 - sample rate
15 - comment
16 - date added
17 - equalizer
18 - composer
19 - ???
20 - play count
21 - last played
22 - disc number
23 - my rating
24 - release date (I guess, it's the value for the "Podcasts" list)
25 - BPM
26 - grouping
27 - category
28 - description
29 - show
30 - season
31 - episode number
```

## Podcasts

The podcasts playlist is organized slightly differently, in the Type 3 MHSD. For example:

```
mhyp (MHODs: 2, MHIPs: 5, hidden: 0, list sort order: 0x18)
  mhod (type: 1, string: 'Podcasts')
  mhod (type: 100)
  mhip (MHODs: 1, groupflag: 256, groupid: 8232, trackid: 0, timestamp: 0, groupref: 0)
    mhod (type: 1, string: 'Example podcast')
  mhip (MHODs: 1, groupflag: 0, groupid: 8233, trackid: 8230, timestamp: 3206828281, groupref: 8232)
    mhod (type: 100)
  mhip (MHODs: 1, groupflag: 0, groupid: 8234, trackid: 8226, timestamp: 3206828379, groupref: 8232)
    mhod (type: 100)
  mhip (MHODs: 1, groupflag: 0, groupid: 8235, trackid: 8228, timestamp: 3206828327, groupref: 8232)
    mhod (type: 100)
  mhip (MHODs: 1, groupflag: 0, groupid: 8236, trackid: 8224, timestamp: 3206828394, groupref: 8232)
    mhod (type: 100)
```

The first mhip (probably identified by groupflag==256) contains the name of a podcast which will then appear as a submenu of the Podcasts menu on the iPod. The other mhips (which reference the episodes in that podcast) seem to use the groupref field to link to their 'parent' mhip (using the groupid field). At the same time the groupids of the episodes are unique as well, but don't seem to be used anywhere else.
