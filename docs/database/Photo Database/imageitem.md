# ImageItem (`mhii`)

MHOD type 2. Not a string like it is in iTunesDB, but still defines location of the file in question. Its mhni child record contains everything that is needed about the image file.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhii
4 | header length | 4 | size of the mhii header (0x98)
8 | total length | 4 | size of the header and all child records
12 | number of children | 4 | In ArtworkDB there are 2 children: one mhod type 2 record for the full sized thumbnail, and one mhod type 2 record for the now-playing sized thumbnail. In Photo Database there are: a child for every thumbnail type (2 on Nanoes, 4 on Photo/Color/Video iPods) + a child for the reference to the full resolution image (if chosen to include it). In Photo Database files generated on Macs, probably by iPhoto, sometimes there could be an additional child, a type-1 string MHOD containing an UTF-8 string of a label for the image, usually found as first child just after the MHII header.
16 | id | 4 | First mhii is 0x40, second is 0x41, ... (on mobile phones the first mhii appears to be 0x64, second 0x65, ...)
20 | Song ID | 8 | Unique ID corresponding to the 'dbid' field in the iTunesDB mhit record, this is used to map ArtworkDB items to iTunesDB items.
28 | unknown4 | 4 | Seems to always be 0
32 | rating | 4 | Rating from iPhoto * 20
36 | unknown6 | 4 | Seems to always be 0
40 | originalDate | 4 | Seems to always be 0 in ArtworkDB; timestamp in Photo Database (creation date of file)
44 | digitizedDate | 4 | Seems to always be 0 in ArtworkDB; timestamp in Photo Database (date when the picture was taken, probably from EXIF information)
48 | source image size | 4 | Size in bytes of the original source image

The rest of the header is zero padded.

```
header_size = 0x18
total_size
type = 2
unk1 = 0
unk2 = 0
```
