# FileList (`mhlf`)

The File List has Files (Images) as its children.

field | size | value
----- | ---- | -----
header identifier | 4 | mhlf
header length | 4 | size of the mhlf header (0x5c)
number of files | 4 | the total number of files in the File List

The rest of the header is zero padded.
