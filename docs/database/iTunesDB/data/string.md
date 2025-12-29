# String Types (0-14)

The simplest form of MHOD. These are any MHOD with a "type" that is less than 15.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header. This is always 0x18 for string type MHOD's.
8 | total length | 4 | size of the header and the string it contains
12 | type | 4 | the type indicator
16 | unk1 | 4 | unknown
20 | unk2 | 4 | unknown
24 | position | 4 | In type 100 mhod's in playlists, this is where the playlist order info is. It does not seem to be significant in string mhod's (except for location - see following notes). Note: This field does not exist in ArtworkDB string dohms. This was observed to be 2 for inversed endian ordered iTunesDBs for mobile phones with UTF8 strings and 1 for standard iPod iTunesDBs with UTF16 strings. Note: If you leave this set to zero on the type 2 (location) string MHOD of a Song (mhit) record, the track will show on the menu, but will not play.
28 | length | 4 | Length of the string, in bytes. If the string is UTF-16, each char takes two bytes. The string in the iTunesDB is not NULL-terminated either. Keep this in mind.  Be careful with very long strings - it has been observed that strings longer than ~512 characters will cause the iPod to continously reboot when it attempts to read the database.
32 | unknown | 4 | unknown. It was thought that this was string encoding: 0 == UTF-16, 1 == UTF-8, however, recent iTunesDB files have had this set to 1 even with UTF-16 strings. Therefore this is definitely incorrect, and the correct meaning has not yet been discovered yet.
36 | unk4 | 4 | unknown
40 | string | length | The string.

String mhods are **NOT** zero padded.
