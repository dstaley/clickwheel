# Data (`mhod`)

The Data object is used in many places in the iTunesDB file, and there are many forms of it.

Value | Description
----- | -----------
1 | Title
2 | Location (this string should be less than 112 bytes/56 UTF-16 chars (not including the terminating \0) or the iPod will skip the song when trying to play it)
3 | Album
4 | Artist
5 | Genre
6 | Filetype
7 | EQ Setting
8 | Comment
9 | Category - This is the category ("Technology", "Music", etc.) where the podcast was located. Introduced in db version 0x0d.
12 | Composer
13 | Grouping
14 | Description text (such as podcast show notes). Accessible by selecting the center button on the iPod, where this string is displayed along with the song title, date, and timestamp. Introduced in db version 0x0d.
15 | Podcast Enclosure URL. Note: this is either a UTF-8 or ASCII encoded string (NOT UTF-16). Also, there is no mhod::length value for this type. Introduced in db version 0x0d.
16 | Podcast RSS URL. Note: this is either a UTF-8 or ASCII encoded string (NOT UTF-16). Also, there is no mhod::length value for this type. Introduced in db version 0x0d.
17 | Chapter data. This is a m4a-style entry that is used to display subsongs within a mhit. Introduced in db version 0x0d.
18 | Subtitle (usually the same as Description). Introduced in db version 0x0d.
19 | Show (for TV Shows only). Introduced in db version 0x0d?
20 | Episode # (for TV Shows only). Introduced in db version 0x0d?
21 | TV Network (for TV Shows only). Introduced in db version 0x0d?
22 | Album Artist. Introduced in db version 0x13?
23 | Artist name, for sorting. Artists with names like "The Beatles" will be in here as "Beatles, The". Introduced in db version 0x13?
24 | Appears to be a list of keywords pertaining to a track. Introduced in db version 0x13?
25 | Locale for TV show? (e.g. "us-tv||0|", v.0x18)
27 | Title, for sorting.
28 | Album, for sorting.
29 | Album-Artist, for sorting.
30 | Composer, for sorting.
31 | TV-Show, for sorting.
32 | Unknown, created by iTunes 7.1 for video tracks. Binary field, no string.
50 | Smart Playlist Data
51 | Smart Playlist Rules
52 | Library Playlist Index
53 | Letter Jump Table
100 | Seems to vary. iTunes uses it for column sizing info as well as an order indicator in playlists.
200 | Album (in Album List, iTunes 7.1)
201 | Artist (in Album List, iTunes 7.1)
202 | Artist, for sorting (in Album List, iTunes 7.1)
203 | Podcast Url (in Album List, iTunes 7.1)
204 | TV Show (in Album List, v. 0x18)
