# Database (`mhbd`)

Offset | Field | Size | Value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhbd
4 | header length | 4 | size of the mhbd header. For dbversion <= 0x15 (iTunes 7.2 and earlier), the length is 0x68 (104 bytes). For dbversion 0x17-0x2f (iTunes 7.3 through 9.1), the size is 0xBC (188 bytes). For dbversion >= 0x30 (iTunes 9.2 and later), the size is 0xF4 (244 bytes).
8 | total length | 4 | size of the header and all child records (since everything is a child of MHBD, this will always be the size of the entire file)
12 | unknown | 4 | Seems to be 1 for uncompressed databases, 2 for compressed databases (iTunesCDB)
16 | version number | 4 | appears to be a version number of the database type. 0x09 = iTunes 4.2, 0x0a = iTunes 4.5, 0x0b = iTunes 4.7, 0x0c = iTunes 4.71/4.8, 0x0d = iTunes 4.9, 0x0e = iTunes 5, 0x0f = iTunes 6, 0x10 = iTunes 6.0.1(?), 0x11 = iTunes 6.0.2-6.0.4, 0x12 = iTunes 6.0.5., 0x13 = iTunes 7.0, 0x14 = iTunes 7.1, 0x15 = iTunes 7.2, 0x16 = ?, 0x17 = iTunes 7.3.0, 0x18 = iTunes 7.3.1-7.3.2., 0x19 = iTunes 7.4, 0x28 = iTunes 8.2.1, 0x2a = iTunes 9.0.1, 0x2e = iTunes 9.1, 0x30 = iTunes 9.2, 0x75 = macOS 26.
20 | number of children | 4 | the number of MHSD children. This has been observed to be 2 (iTunes 4.8 and earlier) or 3 (iTunes 4.9 and older), the third being the separate podcast library in iTunes 4.9. Also it has been observed to be 4 (iTunes 7.1, 7.2) or 5 (iTunes 7.3).
24 | id | 8 | appears to a 64 bit id value for this database
32 | unknown | 2 | always seems to be 1
38 | unknown | 8 | Observed in dbversion 0x11 and later. It was thought that this field is used to store some sort of starting point to generate the item's dbid, but this idea was thrown away.
48 | hashing scheme | 2 | Observed in dbversion 0x19 and later. Indicates the checksum/hashing scheme used: 0 = none, 1 = hash58 + hash72, 3 = hashAB. For hash58 + hash72 (value 1), the hash58 field at offset 88 and the hash72 field at offset 114 needs to be set. For hashAB (value 3), the hashAB field at offset 170 needs to be set.
50 | unknown | 20 | Observed in dbversion 0x19 and later for third-generation iPod nano and iPod classic. Meaning unknown so far.
70 | language | 2 | Observed in dbversion 0x13. It looks like this is a language id (langauge of the iTunes interface). For example for English(United States) this field has values 0x65 and 0x6E which is 'en'. The size of the filed might be bigger to distinguish different 'flavors' of a language.
72 | library persistent id | 8 | Observed in dbversion 0x14. This is a 64-bit Persistent ID for this iPod Library. This matches the value of "Library Persistent ID" seen in hex form (as a 16-char hex string) in the drag object XML when dragging a song from an iPod in iTunes.
80 | unknown | 4 | Observed values: 0x01 for third- and fourth-generation iPod nano (along with fourth-, fifth-, and sixth-generation iPod), 0x04 for fifth-, sixth-, and seventh-generation iPod nano.
84 | unknown | 4 |
88 | hash58 | 20 | HMAC-SHA1 hash using a key derived from the iPod's FirewireId. Required for dbversion 0x19 (third-generation iPod nano and iPod classic).
108 | timezone offset | 4 | Timezone offset in seconds.
112 | secondary hashing scheme | 2 | A second indicator of the checksum/hashing scheme used: 0 = none/hash58, 3 = hash58 + hash72, 4 = hashAB.
114 | hash72 | 46 | AES-128-CBC encrypted signature containing a SHA1 digest of the database. Required for devices with hashing scheme == 1.
160 | audio language | 2 | Audio language preference.
162 | subtitle language | 2 | Subtitle language preference.
164 | unknown | 2 | 
166 | unknown | 2 | 
168 | unknown | 2 | A warning is generated if this is non-zero for uncompressed databases.
170 | hashAB | 57 | Checksum/signature for sixth- and seventh-generation iPod nano. 1 byte header + 56 bytes signature. Required for devices with secondary hashing scheme == 4.

The rest of the header is zero padded.

The Database object contains two or three children, which are [DataSets](./dataset.md).
