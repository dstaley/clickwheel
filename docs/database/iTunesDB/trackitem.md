# TrackItem (`mhit`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhit
4 | header length | 4 | size of the mhit header.  For dbversion <= 0x0b (iTunes 4.7 and earlier), the length is 0x9c. For dbversion >= 0x0c (iTunes 4.71 and later), the size is 0xf4. For dbversion = 0x12 (iTunes 6.0.5), 0x13 (iTunes 7.0) the size is 0x148. For dbversion >= 0x14 (iTunes 7.1) the size is 0x184.
8 | total length | 4 | size of the header and all child records
12 | number of strings | 4 | number of strings (mhods) that are children of this mhit
16 | unique id | 4 | unique ID for a track (referenced in playlists)
20 | visible | 4 | If this value is 1, the song is visible on the iPod.  All other values cause the file to be hidden.  Was previously known as unk1.
24 | filetype | 4 | This appears to always be 0 on first- through fourth-generation hard drive-based iPods. For the iTunesDB that is written to fifth-generation iPod and iPod shuffle, iTunes 4.7.1 (and greater) writes out the file's type as an ANSI string padded with spaces.  For example, an MP3 file has a filetype of 0x4d503320 -> 0x4d = 'M', 0x50 = 'P', 0x33 = '3', 0x20 = <space>. AAC is 0x41414320 & "new" AAC which is used by iTunes 7, M4A, is 0x4D344120. Protected AAC files (purchased from iTunes Store) are M4P = 0x4D345020. Was previously known as unk2. This really is an integer field and is reversed in iTunesDB used in mobile phones with reversed endianess.
28 | type1 | 1 | CBR MP3s are type 0x00, VBR MP3s are type 0x01, AAC are type 0x00
29 | type2 | 1 | CBR MP3s are type 0x01, VBR MP3s are type 0x01, AAC are type 0x00 (type1 and type2 used to be one 2 byte field, but by it doesn't get reversed in the reversed endian iTunesDB for mobile phones, so it must be two fields).
30 | compilation flag | 1 | 1 if the flag is on, 0 if the flag is off
31 | stars/rating | 1 | the rating of the track * 20. Note that the iPod does not update this value here when you change the rating. See the Play Counts file for more information.
32 | last modified time | 4 | last modified time of the track
36 | size | 4 | size of the track, in bytes
40 | length | 4 | length of the track, in milliseconds
44 | track number | 4 | the track number of the track (the 9 in 9/15)
48 | total tracks | 4 | the total number of tracks on this album (the 15 in 9/15)
52 | year | 4 | year of the track
56 | bitrate | 4 | bitrate of the track (ie, 128, 320, etc)
60 | sample rate | 4 | sample rate of the track (ie. 44100) multiplied by 0x10000.
64 | volume | 4 | Volume adjustment field. This is a value from -255 to 255 that will be applied to the track on playback. If you adjust the volume slider in iTunes track info screen, this is what you are adjusting.
68 | start time | 4 | time, in milliseconds, that the song will start playing at
72 | stop time | 4 | time, in milliseconds, that the song will stop playing at
76 | soundcheck | 4 | The SoundCheck value to apply to the song, when SoundCheck is switched on in the iPod settings. The value to put in this field can be determined by the equation: X = 1000 * 10 ^ (-.1 * Y) where Y is the adjustment value in dB and X is the value that goes into the SoundCheck field. The value 0 is special, the equation is not used and it is treated as "no Soundcheck" (basically the same as the value 1000). This equation works perfectly well with ReplayGain derived data instead of the iTunes SoundCheck derived information.
80 | play count | 4 | play count of the song. Note that the iPod does not update this value here. See the Play Counts file for more information.
84 | play count 2 | 4 | Also stores the play count of the song. Don't know if it ever differs from the above value.
88 | last played time | 4 | time the song was last played. Note that the iPod does not update this value here. See the Play Counts file for more information.
92 | disc number | 4 | disc number, for multi disc sets
96 | total discs | 4 | total number of discs, for multi disc sets.
100 | userid | 4 | Apple Store/Audible User ID (for DRM'ed files only, set to 0 otherwise).  Previously known as unk5.
104 | date added | 4 | date added to the iPod or iTunes (not certain which)
108 | bookmark time | 4 | the point, in milliseconds, that the track will start playing back at. This is used for AudioBook filetypes (.AA and .M4B) based on the file extension. Note that there is also a bookmark value in the play counts file that will be set by the iPod and can be used instead of this value. See the Play Counts file for more information.
112 | dbid | 8 | Unique 64 bit value that identifies this song across the databases on the iPod.  For example, this id joins an iTunesDB mhit with a ArtworkDB mhii.  iTunes  appears to randomly create this value for a newly formatted iPod, then increments it by 1 for each additional song added.  Previously known as unk7 and unk8.
120 | checked | 1 | 0 if the track is checked, 1 if it is not (in iTunes)
121 | application rating | 1 | This is the rating that the song had before it was last changed, sorta. If you sync iTunes and the iPod, and they have different (new) ratings, the rating from iTunes will go here and the iPod rating will take precedence and go into the normal rating field. I'm uncertain what exactly this is for, but it's always set to what the iTunes rating is before each sync.
122 | BPM | 2 | the BPM of the track
124 | artwork count | 2 | The number of album artwork items put into the tags of this song. Even if you don't put any artwork items into the tags of the song, this value must at least be 1 for the iPod to display any artwork stored in the ithmb files.
126 | unk9 | 2 | unknown, but always seems to be 0xffff for MP3/AAC songs, 0x0 for uncompressed songs (like WAVE format), 0x1 for Audible
128 | artwork size | 4 | The total size of artwork (in bytes) attached to this song (i.e. put into the song as tags).  Observed in iPodDB version 0x0b and with iPod photo as well as with iPodDB version 0x0d and iPod nano.
132 | unk11 | 4 | unknown
136 | sample rate 2 | 4 | The sample rate of the song expressed as an IEEE 32 bit floating point number. It's uncertain why this is here.
140 | date released | 4 | date/time added to music store? For podcasts this corresponds to the release date as displayed to the right of the podcast title. Formerly known as unk13.
144 | unk14/1 | 2 | unknown, but MPEG-1 Layer-3 songs appear to be always 0x000c, MPEG-2 Layer 3 songs (extrem low bitrate) appear to be 0x0016, MPEG-2.5 Layer 3 songs are 0x0020, AAC songs are always 0x0033, Audible files are 0x0029, WAV files are 0x0000.
146 | unk14/2 | 2 | probably 1 if played on or more times in iTunes and 0 otherwise (at least for MP3 -- the value has been observed to be always 1 for AAC and Audible files, and always 0 for WAV files?)
148 | unk15 | 4 | unknown - used for Apple Store DRM songs (always 0x01010100?), zero otherwise
152 | unk16 | 4 | unknown
156 | Skip Count | 4 | Number of times the track has been skipped. Formerly unknown 17 (added in dbversion 0x0c)
160 | Last Skipped | 4 | Date/time last skipped. Formerly unknown 18 (added in dbversion 0x0c)
164 | has_artwork | 1 | added in dbversion 0xd. Seems to be set to 0x02 for tracks without associated artwork (even if artwork is present, it will not be shown on the iPod) and 0x01 for tracks with associated artwork.
165 | skip_when_shuffling | 1 | sets "Skip When Shuffling" when set to 0x1 (added in dbversion 0xd, formerly known as flag2)
166 | remember_playback_position | 1 | sets "Remember Playback Position" when set to 0x1 (added in dbversion 0xd). Note that Protected AAC files (.m4b extension) and Audible files (.aa extension) do not set this flag or the previous one (skip_when_shuffling), and yet are always bookmarkable and are never included in the song shuffle. To determine if a file is bookmarkable, therefore, check the file type first. If it's not an .m4b or .aa, then check this flag in iTunesDB. (Formerly known as flag3)
167 | flag4 | 1 | some kind of "Podcast" flag (added in dbversion 0xd)? When this flag is set to 0x1 then the "Now playing" page will not show the artist name, but only title and album. When additionally has_artwork is 0x2 then there will be a new sub-page on the "Now playing" page with information about the podcast/song. If the track item is a kind of podcast then this flag must be set to 0x1 or 0x2, otherwise this flag must be set to 0x0. If this flag do not follow this, it might be removed from iTunesDB when user change there iPod to sync podcasts/songs in iTunes.
168 | dbid2 | 8 | Until dbversion 0x12, same data as dbid above (added in dbversion 0x0c). Since 0x12, this field value differs from the dbid one.
176 | lyrics flag | 1 | set to 0x01 if lyrics are stored in the MP3 tags ("USLT"), 0 otherwise.
177 | movie file flag | 1 | if 0x1, it is a movie file. Otherwise, it is an audio file.
178 | played_mark | 1 | added in dbversion 0x0c, first values observed in 0x0d.  Observed to be 0x01 for non-podcasts. With podcasts, a value of 0x02 marks this track with a bullet as 'not played' on the iPod, irrespective of the value of play count above. A value of 0x01 removes the bullet. Formerly known as unk20.
179 | unk17 | 1 | unknown - added in dbversion 0x0c. So far always 0.
180 | unk21 | 4 | unknown (added in dbversion 0x0c)
184 | pregap | 4 | Number of samples of silence before the songs starts (for gapless playback).
188 | sample count | 8 | Number of samples in the song (for gapless playback).
196 | unk25 | 4 | unknown (added in dbversion 0x0c)
200 | postgap | 4 | Number of samples of silence at the end of the song (for gapless playback).
204 | unk27 | 4 | unknown - added in dbversion 0x0c, first values observed in 0x0d.  Appears to be 0x1 for files encoded using the MP3 encoder, 0x0 otherwise.
208 | Media Type | 4 | (formerly known as unk28; added in dbversion 0x0c). It seems that this field denotes the type of the file on (e.g.) fifth-generation iPod. It must be set to 0x00000001 for audio files, and set to 0x00000002 for video files. If set to 0x00, the files show up in both, the audio menus ("Songs", "Artists", etc.) and the video menus ("Movies", "Music Videos", etc.). It appears to be set to 0x20 for music videos, and if set to 0x60 the file shows up in "TV Shows" rather than "Movies". See "Types" below for a summary of observed types. **Caution:** Even if a track is marked as "Audiobook" here (value 0x08), it will not show up in the "Audiobooks" menu on the iPod. Only *.aa and *.m4b are shown there by recent firmwares. One proven exception: On iPod nano they show if they have the correct media type set here and the MHIT also has a chapter data mhod!
212 | season number | 4 | the season number of the track, for TV shows only. Previously known as unk29. (added in dbversion 0x0c)
216 | episode number | 4 | the episode number of the track, for TV shows only - although not displayed on the iPod, the episodes are sorted by episode number. Previously known as unk30. (added in dbversion 0x0c)
220 | unk31 | 4 | unknown (added in dbversion 0x0c). Has something to do with protected files - set to 0x0 for non-protected files.
224 | unk32 | 4 | unknown (added in dbversion 0x0c)
228 | unk33 | 4 | unknown (added in dbversion 0x0c)
232 | unk34 | 4 | unknown (added in dbversion 0x0c)
236 | unk35 | 4 | unknown (added in dbversion 0x0c)
240 | unk36 | 4 | unknown (added in dbversion 0x0c)
244 | unk37 | 4 | unknown (added in dbversion 0x13)
248 | gaplessData | 4 | The size in bytes from first Synch Frame (which is usually the XING frame that includes the LAME tag) until the 8th before the last frame. The gapless playback does not work for MP3 files if this is set to zero. Maybe the iPod prepares the next track when rest 8 frames in the actual track. For AAC tracks, this may be zero. (added in dbversion 0x13)
252 | unk38 | 4 | unknown (added in dbversion 0x0c)
256 | gaplessTrackFlag | 2 | if 1, this track has gapless playback data (added in dbversion 0x13)
258 | gaplessAlbumFlag | 2 | if 1, this track does not use crossfading in iTunes (added in dbversion 0x13)
260 | unk39 | 20 | Appears to be a hash, not checked by the iPod
288 | unk40 | 4 | unknown (seen set to 0xbf)
300 | unk41 | 4 | unknown
304 | unk42 | 4 | unknown (always 0x00 ?)
308 | unk43 | 4 | unknown (previously length 8, seen as 0x818080808080)
312 | unk44 | 2 | unknown (previously length 8, seen as 0x818080808080)
314 | AlbumID | 2 | album id from the album list (previously unknown length 8, seen as 0x818080808080)
352 | mhii-link | 4 | Setting this offset to != 0 triggers the 'Right-Pane-Artwork-Slideshow' on late 2007 iPods (third-generation iPod nano) and causes the iPod to use this value to do artwork lookups (dbid_1 will be ignored!). This value should be set to the id of the corresponding ArtworkDB mhii (Offset 16)

The rest of the header is zero padded.

The MHIT is followed by several Data Objects which have string types. At minimum, it must have a Location type MHOD, in order to tell the iPod where the file is located on the iPod itself. It always has a FileType MHOD as well, although it's not totally necessary.

## Types

```
0x00 00 00 00 - Audio/Video 
0x00 00 00 01 - Audio
0x00 00 00 02 - Video
0x00 00 00 04 - Podcast
0x00 00 00 06 - Video Podcast
0x00 00 00 08 - Audiobook
0x00 00 00 20 - Music Video
0x00 00 00 40 - TV Show (shows up ONLY in TV Shows
0x00 00 00 60 - TV Show (shows up in the Music lists as well)
```
