# AlbumItem (`mhia`)

field | size | value
----- | ---- | -----
header identifier | 4 | mhia
header length | 4 | size of the mhia header (0x28)
total length? | 4 | probably the size of the header and all child records; as there aren't any child records this is equal to header length (40)
unk1 | 4 | seems to be zero
image id | 4 | the id of the mhii record this mhia refers to
