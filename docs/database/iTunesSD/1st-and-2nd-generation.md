# iTunesSD (1st and 2nd generation)

The iTunesSD file on 1st and 2nd generation iPod shuffle is in a big-endian byte order. It consists of a header followed by a bunch of entries, one after the other. The format is much simpler than the iTunesDB. Only the iPod Shuffle is known to use this file at the moment. The Shuffle uses only this file for playing songs, but nevertheless a valid iTunesDB must be present on the device. When connecting to iTunes, only the iTunesDB is read back, not the iTunesSD.

## Header

Field | Size | Value
----- | ---- | -----
num songs | 3 | Number of song entries in the file.
unknown | 3 | 0x010600? iTunes 7.2 puts 0x010800 here
header size | 3 | size of the header (0x12, 18 byte header)
unknown | 9 | possibly zero padding

The rest of header is **NOT** zero padded.

## Entry

Field | Size | Value
----- | ---- | -----
size of entry | 3 | Always 0x22e (558 bytes)
unk1 | 3 | unknown (always 0x5aa501 ?)
starttime | 3 | Start Time, in 256 millisecond increments - e.g. 60 seconds = 0xea (234 dec). The reason for this is that the iPodShuffle has only a simplistic "clock". Every millisecond it increments an 8 bit counter. When the counter overflows, this causes an interrupt or something like that which causes it to increment this "clock" value. Very simple clock, easy to do in a an 8-bit register. Basically multiply whatever value you find here by 0.256 to convert it to seconds. Leaving this as zero means it plays from the beginning of the file.
unk2 | 3 | unknown (always 0?)
unk3 | 3 | Unknown, but seems to be associated with start time (start time of 0xea resulted in unk3 = 0x1258ee)
stoptime | 3 | Stop Time, also in 256 millisecond increments - e.g. 120 seconds = 0x1d4 (468 dec). Leaving this as zero means it'll play to the end of the file.
unk4 | 3 | unknown
unk5 | 3 | Unknown, but seems to be associated with stop time (stop time of 0x1d4 resulted in unk5 = 0x24a830)
volume | 3 | Volume - ranges from 0x00 (-100%) to 0x64 (0%) to 0xc8 (100%)
file_type | 3 | 0x01 = MP3, 0x02 = AAC, 0x04 = WAV
unk6 | 3 | 0x200?
filename | 522 | filename of the song, padded at the end with 0's, in UTF-16.  Note: forward slashs are used here, not colons like in the iTunesDB - for example "/iPod_Control/Music/F00/Song.mp3".
shuffleflag | 1 | If this value is 0x00, the song will be skipped in while the player is in shuffle mode.  Any other value (iTunes uses 0x01) will allow it be played in both normal and shuffle modes.  By default, iTunes 4.7.1 sets this flag to 0x00 for audiobooks (.m4b and .aa), so they aren't played in shuffle mode.
bookmarkflag | 1 | If this value is 0x00, the song will not be bookmarkable (i.e. its playback position won't be saved when switching to a different song).  Any other value will make it bookmarkable.  Unlike hard drive based iPods, all songs can be marked as bookmarkable - not just .m4b and .aa. However, iTunes might not use this bookmark information for songs other than actual audiobooks. By default, iTunes 4.7.1 sets this flag to 0x01 for audiobooks (.m4b and .aa), and 0x00 for everything else.
unknownflag | 1 | This has never been observed to be anything other than 0x00, and setting it other values seemed to no effect.
