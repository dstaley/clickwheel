# ImageList (`mhli`)

The Image List has Image Items as its children. The number of Image Items is the same as the number of images.

field | size | value
----- | ---- | -----
header identifier | 4 | mhli
header length | 4 | size of the mhli header (0x5c)
number of images | 4 | the total number of images in the Image List
