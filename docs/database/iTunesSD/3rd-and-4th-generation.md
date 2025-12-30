# iTunesSD (third- and fourth-generation)

Just like in the iTunesDB the default size for integer numbers seems to be 32 bit. In earlier iTunesSD files it was a rather odd 24 bit. Like the iTunesDB, third-generation iPod shuffle's iTunesSD is little endian. Earlier iTunesSD files were big endian.

Little endian means that the numbers start with the lowest byte. So a value of 0x12345678 (decimal 305419896) will be written in the iTunesSD file as 78 56 34 12. Apparently the apple developers liked to look at their files with a hex viewer to. At least they choose the magic numbers for their headers in a way that their ASCII representation tells you its purpose. So the database header starts with a magic number 0x73686462 (dec. 1936221282). Translating those bytes one by one into ASCII you get "s" "h" "d" "b". The little endian storage format changes the byte order so that the first characters of the new iTunesSD are in fact "bdhs", but now you know how we came up with the names for those separate elements.

Here's the general layout of an iTunesSD file:

- bdhs Shuffle Database
  - hths Tracks Header
    - rths Track1
    - rths Track2
    - ...
  - hphs Playlists Header
    - lphs Playlist1
    - lphs Playlist2
    - ...

## Database 

Field | Size | Description | Data | Hexdump
----- | ---- | ----------- | ---- | -------
header_id | 4 | Header | shdb | 62 64 68 73
unknown_1 | 4 | Version number? | 0x02000003 Old values:0x02010001Gen 2:0x0106000x010800 | 03 00 00 02
header_length | 4 | size of this header | 64 | 40 00 00 00
total_no_of_tracks | 4 |  | 126 | 7e 00 00 00
total_no_of_playlists | 4 |  | 10 | 0a 00 00 00
unknown_2 | 8 |  | 0x0000 0000 0000 0000 | 00 00 00 00 00 00 00 00
max_volume | 1 | 0x00 do not limit the volume
0x03 is the min setting
0x20 is the max setting | 0x00 | 00
voiceover_enabled | 1 | Only applies for tracks, not for playlists. | 1 | 01
unknown_3 | 2 |  | 0x0000 | 00 00
total_no_of_tracks2 | 4 | Does not include podcasts or audiobooks in the count. | 126 | 7e 00 00 00
track_header_chunk_offset | 4 |  | 0x00000040 | 40 00 00 00
playlist_header_chunk_offset | 4 |  | 0x0000b964 | 64 b9 00 00
unknown_4 | 20 |  | 0x0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 | 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00

## Tracks Header

Field | Size | Data | Hexdump
----- | ---- | ---- | -------
header_id | 4 | shth | 68 74 68 73
total_length | 4 | 524 | 0c 02 00 00
number_of_tracks | 4 | 126 | 7e 00 00 00
unknown_1 | 8 | 0x0000 0000 0000 0000 | 00 00 00 00 00 00 00 00
offset_of_track_chunk_0 | 4 | 0x0000024c | 4c 02 00 00
offset_of_track_chunk_1 | 4 | 0x000003c0 | c0 03 00 00
offset_of_track_chunk_2 | 4 | 0x00000534 | 34 05 00 00
offset_of_track_chunk_3 | 4 | 0x000006a8 | a8 06 00 00
offset_of_track_chunk_4 | 4 | 0x0000081c | 1c 08 00 00
... | ... | ... | ...

## TrackX

Field | Size | Description | Data | Hexdump
----- | ---- | ----------- | ---- | -------
header_id | 4 |  | shtr | 72 74 68 73
total_length | 4 |  | 372 | 74 01 00 00
start_at_pos_ms | 4 |  | 0 | 00 00 00 00
stop_at_pos_ms | 4 | Rythmbox IPod plugin sets this value always 0. | 112169 | 29 b6 01 00
volume_gain | 4 |  | 0x00000000 | 00 00 00 00
filetype | 4 | Type 1 are mpeg, mp3 files, Type 2 arere aac, mp4, m4a files, Type 4 are wav files | 1 (MP3) | 01 00 00 00
filename | 256 |  | /iPod_Control/Music/F02/NNCN.mp3 | 2f 69 50 6f 64 5f 43 6f 6e 74 72 6f 6c 2f 4d 75 73 69 63 2f 46 30 32 2f 4e 4e 43 4e 2e 6d 70 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
Bookmark | 4 | In milliseconds | 0x00000000 | 00 00 00 00
dont_skip_on_shuffle | 1 | If all songs in a playlist don't have this bit set the playlist is skipped when iPod is set to shuffle and a playlist is being chosen. It seems to be ignored when shuffling within a playlist. | 1 | 01
remember_playing_pos | 1 |  | 0 | 00
part_of_uninterruptable_album | 1 |  | 0 | 00
unknown_1 | 1 |  | 0x00 | 00
pregap | 4 |  | 0x240 = 576 | 40 02 00 00
postgap | 4 |  | 0xc9c= 3228 | 9c 0c 00 00
number_of_samples | 4 |  | 0x4b6c24 = 4942884 | 24 6c 4b 00
unknown_file_related_data1 | 4 |  | 0 | 00 00 00 00
gapless_data | 4 |  | 0x24a2a2 = 2400930 | a2 a2 24 00
unknown_file_related_data2 | 4 |  | 0 | 00 00 00 00
Album ID | 4 |  | 0x0000007f | 7f 00 00 00
track_number | 2 |  | 1 | 01 00
disc_number | 2 |  | 0 | 00 00
unknown_2 | 8 |  | 0x0000 0000 0000 0000 | 00 00 00 00 00 00 00 00
dbid | 8 | Serves as the filename for the voiceover | 0xdfa209b7ce6f2db9 | b9 2d 6f ce b7 09 a2 df
Artist ID | 4 |  | 0x00000146 | 46 01 00 00
unknown_3 | 32 |  | 0x0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 | 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00

## Playlist Header

Field | Size | Description | Data | Hexdump
----- | ---- | ----------- | ---- | -------
header_id | 4 |  | shph | 68 70 68 73
total_length | 4 |  |  | 20 00 00 00
number_of_playlists | 4? |  | 3 | 03 00 00 00
number_of_playlists_1 | 2 | The number of non-podcast playlists, 0xffff if all playlists are not podcast playlists. | 0xffff | ff ff
number_of_playlists_2 | 2 | The number of master playlists, 0xffff if all playlists are not master playlists. | 0x0100 | 01 00
number_of_playlists_3 | 2 | The number of non-audiobook playlists, 0xffff if all playlists are not audiobook playlists. | 0xffff | ff ff
unknown_2 | 2 |  |  | 00 00
offset_of_playlist_1 | 4 |  | 0x00015b14 | 14 5b 01 00
offset_of_playlist_2 | 4 |  | 0x00015ef0 | f0 5e 01 00
... | ... | ... | ... | ...

# PlaylistX

Field | Size | Description | Data | Hexdump
----- | ---- | ----------- | ---- | -------
header_id | 4 |  | shpl | 6c 70 68 73
total_length | 4 |  |  | dc 03 00 00
number_of_songs | 4 |  | 236 | ec 00 00 00
number_of_songs2 | 4 | Number of non podcast or audiobook songs. | 236 | ec 00 00 00
dbid | 8 | Serves as the filename for the voiceover | 6bed | 48 4d 19 eb 4e 34 ed 6b
type | 4 | 1 is the master playlist 2 is a normal playlist 3 is a podcast playlist 4 is a audiobook playlist | 2 | 02 00 00 00
unknown_1 | 16 |  |  | 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
playlist_track_1 | 4 |  | 118 | 76 00 00 00
playlist_track_2 | 4 |  | 119 | 77 00 00 00
... | ... | ... | ... | ...

A dbid of all zeros yields a voiceover of All songs. Also playlist dbids without a corresponding voiceover file will yield a voiceover of playlist n or audiobook n where n is the playlist number. iPod shuffle assumes the podcast playlist is last.
