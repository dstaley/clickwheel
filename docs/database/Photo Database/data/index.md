# Data (`mhod`)

The MHODs found in the ArtworkDB and Photo Database files are significantly different than those found in the normal iTunesDB files.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the MHOD header (0x18)
8 | total length | 4 | size of the header and content, including child records
12 | type | 2 | type of the MHOD, see below
14 | unknown | 1 | unknown, always 0 so far
15 | padding length | 1 | all MHODs must be zero-padded so that the length is a multiple of 4, this field contains the number of padding bytes added (e.g. 0, 1, 2 or 3). WARNING! This field was always set to 2 for a while. To avoid parser crash, the best way is to ignore it when parsing.

The rest of the header is zero padded.

There are 2 groups of types of MHODs in the ArtworkDB: container MHODs contain a MHNI as a child, while 'normal' string MHODs contain a string.

**Attention:** Sometimes it seems that the MHBAs in iPod (5th generation) and iPod nano Photo Database have a second MHOD child which, although being identified by a type of 2, is a string (and not container) MHOD. This second string MHOD in photo album is usually found in Photo Database files generated on Macs, probably by iPhoto, and contains an UTF-8 string describing a transition effect such as "Dissolve". However in Photo Database files generated on PCs for example by iTunes 6 for a 30 GB iPod (5th generation) this does not happen, and there is only one type-1 string MHOD as child, just like with iPod photo Photo Database files.

Type | Group | Description
---- | ----- | -----------
1 | string | Album name (in the Photo Database)
2 | container | Thumbnail image
3 | string | File name
5 | container | Full Resolution image (in the Photo Database)

## Container MHODs

MHODs with type 2 contain a MHNI that (contains a type 3 MHOD that) references a thumbnail. MHODs with type 5 contain a MHNI that (contains a type 3 MHOD that) references a full resolution image (in the Photo Database).

## String MHODs

The content of string MHODs (probably all types except 2 and 5, although only 1 and 3 have been observed so far) is structured again with something like a sub-header:

Field | Size | Value
----- | ---- | -----
string length | 4 | length in bytes of the string (e.g. after encoding)
unknown | 4 | might be the string encoding: 0,1 == UTF-8; 2 == UTF-16-LE. Observed values are: 1 in type 1 MHODs and 2 in type 3 MHODs.
unknown | 4 | always zero?
content | variable | the actual, encoded string content
padding | 0..3 | zero to three bytes of padding to get the length of the whole MHOD to a multiple of 4, note that this is not included in the string length but is included in the total length
