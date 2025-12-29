# Letter Jump Table (53)

The Letter Jump Table is only found as a child of the Main Library Playlist.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header.
8 | total length | 4 | size of the header and all the index entries
12 | type | 4 | the type indicator  ( 53 )
16 | unk1 | 4 | unknown (always zero)
20 | unk2 | 4 | unknown (always zero)
24 | index type | 4 | what this index is sorted on (see list above)
28 | count | 4 | number of entries. Unused letters are left out and umlauts (at least German ones) are added after the letter they are derived from. Don't yet know about the order of the French accented letters.
32 | null padding | 8 | lots of padding
40 | index entries | 12 * count | The index entries themselves.

Letter Jump Table mhods are **NOT** zero padded.

## Table Entry

offset | field | size | value
------ | ----- | ---- | -----
0 | letter | 4 | The letter of this table entry. Looks like uppercase UTF-16LE with 2 padding null bytes (i.e. A would be 0x00000041 little endian / 0x41000000 big endian)
4 | header length | 4 | the number of the first entry in the corresponding MHOD52 index starting with this letter. Zero-based and incremented by one for each entry, not 4.
8 | total length | 4 | the count of entries starting with this letter in the corresponding MHOD52.
