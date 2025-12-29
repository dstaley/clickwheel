# Smart Playlist (50)

A slightly more complex MHOD. These are any MHOD with a "type" that is 50. This MHOD defines the stuff in the Smart playlist that is not the "rules". Basically all the checkboxes and such. It's pretty straightforward.

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header.
8 | total length | 4 | total length of the mhod
12 | type | 4 | the type indicator  ( 50 )
16 | unk1 | 4 | unknown
20 | unk2 | 4 | unknown
24 | live update | 1 | Live Update flag. 0x01 = on, 0x00 = off
25 | check rules | 1 | Rules enable flag. 0x01 = on, 0x00 = off. When this is enabled, Rules from the type 51 MHOD will be used.
26 | check limits | 1 | Limits enable flag. 0x01 = on, 0x00 = off. When this is enabled, Limits listed below will actually be used.
27 | limit type | 1 | Limit Type. See below for the list of limit types.
28 | limit sort | 1 | Limit Sort. See below for the list of limit sorting types.
29 | unknown | 3 | always zero bytes
32 | limit value | 4 | The actual value used for the limit
36 | match checked only | 1 | match checked only flag, 0x01 = on, 0x00 = off. When this is enabled, only songs marked as "checked" will be matched. Checked is a field in the mhit.
37 | reverse limit sort | 1 | Reverse the Limit Sort flag. 0x01 = on, 0x00 = off. When this is enabled, the sort will be reversed. More on this below.

The `mhod` IS zero padded at the end (58 null bytes)

## Limit types

Value | Description
----- | -----------
1 | Minutes
2 | Megabytes
3 | Songs
4 | Hours
5 | Gigabytes

## Limit sort types

Value | Description
----- | -----------
0x02 | Random
0x03 | Song Name (alphabetical)
0x04 | Album (alphabetical)
0x05 | Artist (alphabetical)
0x07 | Genre (alphabetical)
0x10 | Most Recently Added
0x14 | Most Often Played
0x15 | Most Recently Played
0x17 | Highest Rating

When the Reverse Limit Sort flag is set, the sort will be reversed. So most recently added becomes least recently added, and highest rating becomes lowest rating, and so on. It's just reversing the sorted list before applying the limit to it.
