# Chapter Data (17)

The chapter data defines where the chapter stops are in the track, as well as what info should be displayed for each section of the track. It seems that the format of this mhod changed significantly over time. The following is analysed from an iTunesDB version 0x13 (iTunes 7.0).

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header. This is always 0x18.
8 | total length | 4 | size of the mhod
12 | type | 4 | the type indicator  ( 0x11 = 17 )
16 | unk1 | 4 | unknown (always 0?)
20 | unk2 | 4 | unknown (always 0?)
24 | unk3 | 4 | unknown (always 0?) This is not part of the header (otherwise the header length would have been different), but it is also not part of the first atom.
28 | unk4 | 4 | unknown (always 0?) This is not part of the header (otherwise the header length would have been different), but it is also not part of the first atom.
32 | unk5 | 4 | unknown (always 0?) This is not part of the header (otherwise the header length would have been different), but it is also not part of the first atom.
36 | data | (total length - header length - 12) | The chapter stop data atoms, starting with a "sean" atom (see below). This part of the mhod is not little-endian like the rest of the file. It's big-endian/E.g. the value 0x0123 in a 4 byte word is found as 00 00 01 23. It is also arranged in a tree like structure, much like the iTunesDB itself is, only the information is in a slightly different arrangement.

## Chapter Data Atom Layout

The atoms appear to be arranged in the following layout:

```
sean (exactly one atom of this type is present,
        childcount is number of chapters + 1 for the hedr atom)
   chap (chapter indicator, childcount is always 1)
      name (contains a UTF16 string with name of the chapter or Artist name, 
            childcount is always 0)
   chap (next chapter)
      name (and so on)...
   ...
   hedr (signals the end, childcount is always 0)
```

There are multiple "chap" entries, one for each chapter. Older DBs seem to have a different, more complex layout, like this:

```
sean (?)
 chap (chapter indicator ?)
  name (contains a UTF16 string with name of the chapter or Artist name)
  ploc (position/location?)
   trak (specifies track number, perhaps? This should be obvious after looking at some examples)
  urlt (contains a UTF16 string with the name of the song in the chapter or some kind of subname)
  url  (contains a UTF8 string with a URL)
 chap (next chapter)
  name (and so on)...
 ...
hedr (signals the end)
```

## Chapter Data Atoms

Each atom consists of the following:

offset | field | size | value
------ | ----- | ---- | -----
0 | size | 4 | size of the atom and all its children Caution: This is different to all other structures in the file, where the name always comes first. Here, the length is the first word of the atom!
4 | atom name | 4 | the name of the atom
8 | startpos/unk1 | 4 | For chap atoms: the starting position of this chapter in ms, except for the very first chap where this field is 1 (not 0 as expected). 
For all other atoms: always 1?
12 | number of childs | 4 | the number of childs
16 | unk2 | 4 | always 0?
20 | data | varies | some kind of data or children

## Chapter Data String Atoms (UTF-16)

UTF-16 String entries in these atoms (like 'name') fit the following mold:

offset | field | size | value
------ | ----- | ---- | -----
0 | size | 4 | size of the atom and all its children (0x16 + 2*string length in characters, e.g. for a 8 char string this is 0x26)
4 | atom name | 4 | the name of the atom
8 | unknown | 4 | always 1?
12 | unk1 | 4 | always 0? (child count)
16 | unk2 | 4 | always 0?
20 | string length | 2 | string length in characters
22 | string | length *2 | UTF16 string, 2 bytes per character

## Chapter Data String Atoms (UTF-8)

UTF-8 entries fit the following mold:

offset | field | size | value
------ | ----- | ---- | -----
0 | size | 4 | size of the atom and all its children
4 | atom name | 4 | the name of the atom
8 | unknown | 4 | always 1?
12 | unknown | 4 | varies.. some kind of type?
16 | null | 8 | zeros
24 | string | size - 24 | UTF8 string, 1 byte per character

## Chapter Data `hedr` Atom

offset | field | size | value
------ | ----- | ---- | -----
0 | size | 4 | size of the atom, 0x1c
4 | atom name | 4 | the name of the atom, 'hedr'
8 | unk1 | 4 | always 1?
12 | number of childs | 4 | the number of childs, always 0
16 | unk2 | 4 | always 0
20 | unk3 | 4 | always 0
24 | unk4 | 4 | always 1
