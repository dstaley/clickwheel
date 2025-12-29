# Podcast URL Types (15-16)

Introduced in db version 0x0d, MHOD's with type 15 and 16 hold the Enclosure and RSS URL for the Podcast. The string is probably UTF-8, but only Unicode symbols U+0000 through U+007F (a.k.a ASCII) have been observed.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header. This is always 0x18 for string type MHOD's.
8 | total length | 4 | size of the header and the string it contains
12 | type | 4 | the type indicator  ( 15 or 16 )
16 | unk1 | 4 | unknown (always 0?)
20 | unk2 | 4 | unknown (always 0?)
24 | string | (total length - header length) | The string.

Podcast URL mhods are **NOT** zero padded.
