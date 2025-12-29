# FileImage (`mhif`)

Field | Size | Value
----- | ---- | -----
header identifier | 4 | mhif
header length | 4 | size of the mhif header (0x7c)
total length | 4 | size of the header and all child records
unknown1 | 4 | always seems to be 0
correlation id | 4 | used to link this entry with a file and an Image Name, see Image Name for more details.
image size | 4 | size of the image in bytes. A full sized thumbnail is 39,200 bytes, a 'Now Playing' thumbnail is 6,272 bytes on the iPod Photo/Color. On the iPod Nano, a full sized thumbnail is 20,000 bytes while a 'Now Playing' thumbnail is 3,528 bytes.

The rest of the header is zero padded.
