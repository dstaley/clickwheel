# AlbumList (`mhla`)

The Album List has no children in the case of the `ArtworkDB` file, and 1 or more children for the `Photo Database` file: 1 child for the Photo Library and possibly some more children for additional photo albums.

Field | Size | Value
----- | ---- | -----
header identifier | 4 | mhla
header length | 4 | size of the mhla header (0x5c)
number of children | 4 | the total number of children in the Album List (no children in ArtworkDB, 1 or more children in Photo Database)

The rest of the header is zero padded.

For the Photo Database the layout looks like this, for example:

```
mhsd (type: 2)
  mhla (children: 3)
    mhba (number of mhods: 1, number of mhias: 5, unk3: 0x10000)
      mhod (string: "My Pictures")
      mhia (image id: 100)
      mhia (image id: 101)
      mhia (image id: 102)
      mhia (image id: 103)
      mhia (image id: 104)
    mhba (number of mhods: 1, number of mhias: 2, unk3: 0x60000)
      mhod (string: "Folder A")
      mhia (image id: 100)
      mhia (image id: 101)
    mhba (number of mhods: 1, number of mhias: 3, unk3: 0x60000)
      mhod (string: "Folder B")
      mhia (image id: 102)
      mhia (image id: 103)
      mhia (image id: 104)
```

In this case "My Pictures" was the folder that was selected in iTunes to synchronize photos with, and it contained 2 folders "Folder A" and "Folder B" with 2 and 3 photos respectively. The iPod will show this on its Photo menu as a submenu called "Photo Library" (containing all 5 photos), a submenu called "Folder A" with the first 2 photos and a submenu "Folder B" with the other 3 photos.

Note that the string MHODs are zero-padded to a length that is a multiple of 4 so that in the Photo Database all objects start on a 4-byte boundary. The iPod won't list any photo or photo album otherwise.
