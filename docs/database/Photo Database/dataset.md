# DataSet (`mhsd`)

This is basically the same as the MHSD element in the iTunes DB. Depending on the index, the Data Set either contains an Image List (mhli) child, an Album List (mhla) child or a File List child (mhlf).

Field | Size | Value
----- | ---- | -----
header identifier | 4 | mhsd
header length | 4 | size of the mhsd header (0x60)
total length | 4 | size of the header and all child records
index | 4 | An index number. This value is 1 if the child is an Image List, 2 if the child is an Album List, or 3 if it's a File List.

The rest of the header is zero padded.
