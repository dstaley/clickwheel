# Database (`mhbd`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhbd
4 | header length | 4 | size of the mhbd header. For dbversion <= 0x15 (iTunes 7.2 and earlier), the length is 0x68. For dbversion >= 0x17 (iTunes 7.3 and later), the size is 0xBC.
8 | total length | 4 | size of the header and all child records (since everything is a child of MHBD, this will always be the size of the entire file)
12 | unknown | 4 | always seems to be 1
16 | version number | 4 | appears to be a version number of the database type. 0x09 = iTunes 4.2, 0x0a = iTunes 4.5, 0x0b = iTunes 4.7, 0x0c = iTunes 4.71/4.8, 0x0d = iTunes 4.9, 0x0e = iTunes 5, 0x0f = iTunes 6, 0x10 = iTunes 6.0.1(?), 0x11 = iTunes 6.0.2-6.0.4, 0x12 = iTunes 6.0.5., 0x13 = iTunes 7.0, 0x14 = iTunes 7.1, 0x15 = iTunes 7.2, 0x16 = ?, 0x17 = iTunes 7.3.0, 0x18 = iTunes 7.3.1-7.3.2., 0x19 = iTunes 7.4.
20 | number of children | 4 | the number of MHSD children. This has been observed to be 2 (iTunes 4.8 and earlier) or 3 (iTunes 4.9 and older), the third being the separate podcast library in iTunes 4.9. Also it has been observed to be 4 (iTunes 7.1, 7.2) or 5 (iTunes 7.3).
24 | id | 8 | appears to a 64 bit id value for this database
32 | unknown | 2 | always seems to be 2
38 | unknown | 8 | Observed in dbversion 0x11 and later. It was thought that this field is used to store some sort of starting point to generate the item's dbid, but this idea was thrown away.
48 | unknown | 2 | Observed in dbversion 0x19 and later, and must be set to 0x01 for the new iPod Nano 3G (video) and iPod Classics. The obscure hash at offset 88 needs to be set as well.
50 | unknown | 20 | Observed in dbversion 0x19 and later for the new iPod Nano 3G (video) and iPod Classics. Meaning unknown so far.
70 | language | 2 | Observed in dbversion 0x13. It looks like this is a language id (langauge of the iTunes interface). For example for English(United States) this field has values 0x65 and 0x6E which is 'en'. The size of the filed might be bigger to distinguish different 'flavors' of a language.
72 | library persistent id | 8 | Observed in dbversion 0x14. This is a 64-bit Persistent ID for this iPod Library. This matches the value of "Library Persistent ID" seen in hex form (as a 16-char hex string) in the drag object XML when dragging a song from an iPod in iTunes.
88 | obscure hash | 20 | Observed in dbversion 0x19 for iPod Nano 3G (video) and iPod Classics.

The rest of the header is zero padded.

The Database object contains two or three children, which are [DataSets](./dataset.md).
