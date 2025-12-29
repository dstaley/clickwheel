# Play Counts

The play count information, since the last sync with iTunes, is stored in the "Play Counts" file.

The play counts file is deleted by the iPod and rebuilt whenever it detects that the iTunesDB file has changed (immediately after a sync). So writing anything into this file yourself is more than a little pointless, as the iPod simply will overwrite it. This is the iPod's way of sending information back to iTunes (or whatever program you happen to sync with).

When iTunes syncs, the first thing it does is read the values that the iPod has put into this file, and merges them back into the iTunes Library.

## Header

The play count header indicates a valid play count file and specifies how many entries follow and the size of each entry record. The is an entry record for each song on the iPod; the entry position corresponding to the position of the song in the iTunesDB.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhdp
4 | header length | 4 | 0x60
8 | single entry length | 4 | 0x0c (firmware 1.3), 0x10 (firmware 2.0), 0x14 (with version 0x0d of iTunesDB), 0x1C (with version 0x13 of iTunesDB)
12 | number of entries | 4 | number of songs on iPod

The rest of the header is zero-padded.

## Entry

The entry record contains the play count data for each song, one record exists for each song on the iPod.

offset | field | size | value
------ | ----- | ---- | -----
0 | play count | 4 | number of played times since last sync
4 | last played | 4 | last played time. Set to zero in older firmwares, or to the value from iTunesDB in newer ones (anything with the "Music" menu).
8 | audio bookmark | 4 | position in file that the song was last paused/stopped at, in milliseconds. This works for audiobooks, podcasts, and seemingly anything else with the right bit set in the MHIT (unk19).
12 | rating | 4 | Rating given to song. Number of stars (1-5) * 0x14. Set to zero in older firmwares, or to the value from iTunesDB in newer ones (anything with the "Music" menu).
16 | unknown | 4 | Unknown, yet. Probably something to do with podcast bookmarking or some such.
20 | skip count | 4 | Number of times skipped since last sync. This field appears with firmware that supports the 0x13 version iTunesDB.
24 | last skipped | 4 | Last skipped date/time. Set to zero if never skipped. This field appears with firmware that supports the 0x13 version iTunesDB.

Older firmware versions had set the last played time and the rating to "zero" in this database, unless you changed either one on the iPod itself. Thus you could ignore zero entries.

Newer firmwares, however, set last played time and rating to be the same as what is in the iTunesDB file, by default. Meaning that you can't ignore zero entries for rating anymore (somebody could set the rating to a zero rating on the iPod). So you must compare the rating in Play Counts and the rating in iTunesDB to determine if it has actually changed and you need to update it on the PC (in a program on the PC that talks to the iPod that is).
