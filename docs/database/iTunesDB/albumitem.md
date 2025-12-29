# AlbumItem (mhia)

Usually mhia has two child strings: album title and artist name.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhia
4 | header length | 4 | size of the mhia header.  Length is 0x58
8 | total length | 4 | size of the header and all child records
12 | number of strings | 4 | number of strings (mhods) that are children of this mhia
16 | unknown | 2 | (previously long length 4 with possibly album ID)
18 | album id for track | 2 | album ID (v. 0x18 file) (previously long length 4)
20 | unknown | 8 | timestamp? (v. 0x18 file)
28 | unknown | 4 | always 2? (v. 0x18 file)
