# Photo Database

The Photo Database is created by iTunes, and is stored in "/Photos/Photo Database". The Photo Database looks similar to the ArtworkDB but has additional entries in the mhla object (mhba and mhia, see below) as well as different Thumbnails. The mhiis in the Photo Database look like this for example:

```
mhii (children: 5, id: 117, srcimgsize: 0)
  mhod (type: 5)
    mhni (children: 1, corrid: 1, ithmb offset: 0, imgsize: 1567395, imgdim: 0x0)
      mhod (type: 3, length: 80, string: u':Full Resolution:2005:08:06:simg3609.jpg')
  mhod (type: 2)
    mhni (children: 1, corrid: 1019, ithmb offset: 7603200, imgsize: 691200, imgdim: 0x2c801e0)
      mhod (type: 33554435, length: 42, string: u':Thumbs:F1019_1.ithmb')
  mhod (type: 2)
    mhni (children: 1, corrid: 1020, ithmb offset: 851840, imgsize: 77440, imgdim: 0xa500e1)
      mhod (type: 33554435, length: 42, string: u':Thumbs:F1020_1.ithmb')
  mhod (type: 2)
    mhni (children: 1, corrid: 1009, ithmb offset: 27720, imgsize: 2520, imgdim: 0x29001f)
      mhod (type: 33554435, length: 42, string: u':Thumbs:F1009_1.ithmb')
  mhod (type: 2)
    mhni (children: 1, corrid: 1015, ithmb offset: 251680, imgsize: 22880, imgdim: 0x7b0058)
      mhod (type: 33554435, length: 42, string: u':Thumbs:F1015_1.ithmb')
```

The type 5 mhod references the full resolution image and is probably only there if the corresponding check box in iTunes was checked. The type 2 mhods reference different types of thumbnail versions:

1. The first thumbnail (imgdim: 0x2c801e0, which decodes as 712x480) is of dimension 720x480 and in YUV 4:2:2 format, interlaced. It consists of two half-frames concatenated together. This is probably used for the TV-out (dimension, color format and interlacing look like NTSC).
1. The second thumbnail (imgdim: 0xa500e1, which decodes as 165x225) is of dimension 176x220 and in RGB 565 format, but rotated 90° CCW. This is the image that is actually displayed on the iPod screen.
1. The third thumbnail (imgdim: 0x29001f(41x31)) is 42x30, RGB 565 with swapped bytes (e.g. it's more like GBRG 3553). This is the image that is used as a thumbnail.
1. The fourth thumbnail (imgdim: 0x7b0058(123x88)) is 130x88, RGB 565 with swapped bytes

There is a *.ithmb file per resolution (in the directory "/Photos/Thumbs/"), that concatenates all thumbnails with that resolution.

On an iPod video (5G) there are 4 different thumbnails type:

1. 720x480 interlaced UYVY (YUV 4:2:2) - used for TV output - 691200 bytes each single thumbnail
1. 320x240 byte swapped RGB565 - used for fullscreen on the iPod - 153600 bytes each single thumbnail
1. 130x88 byte swapped RGB565 - used on the iPod during slideshow, when current photo is displayed on TV - 22880 bytes each single thumbnail
1. 50x41 byte swapped RGB565 - used on the iPod when listing and during slideshow - 4100 bytes each single thumbnail

Dimensions of the fields in the Photo Database are very important. Only one total length field or one padding field with wrong value could be enough to make the Photo Database file completely unusable: then nothing will be displayed on the iPod, no photo albums, no photos.

Here follows a complete structure for a Photo Database file working on an iPod video 5G:

```
 'mhfd', 132, 1384, 0, 1, 3, 0, 102, 0, 0, 0, 0, 2, 0, 0, 0, 0
 'mhsd', 96, 1276, 1
 'mhli', 92, 1
 'mhii', 152, 1088, 5, 100, 102, 0, 0, 0, 0, 3221487006, 3221487006, 0
 'mhod', 24, 216, 5, 0, 0
 'mhni', 76, 192, 1, 1, 0, 0, 0, 0, 0, 0
 'mhod', 24, 116, 3, 0, 2
 78, 2, 0, ':Full Resolution:2006:01:22:DSC00090.JPG'
 'mhod', 24, 180, 2, 0, 0
 'mhni', 76, 156, 1, 1019, 0, 691200, 0, 0, 480, 712
 'mhod', 24, 80, 3, 0, 2
 42, 2, 0, ':Thumbs:F1019_1.ithmb'
 'mhod', 24, 180, 2, 0, 0
 'mhni', 76, 156, 1, 1024, 0, 153600, 0, 0, 240, 320
 'mhod', 24, 80, 3, 0, 2
 42, 2, 0, ':Thumbs:F1024_1.ithmb'
 'mhod', 24, 180, 2, 0, 0
 'mhni', 76, 156, 1, 1015, 0, 22880, 0, 0, 88, 123
 'mhod', 24, 80, 3, 0, 2
 42, 2, 0, ':Thumbs:F1015_1.ithmb'
 'mhod', 24, 180, 2, 0, 0
 'mhni', 76, 156, 1, 1036, 0, 4100, 0, 0, 41, 53
 'mhod', 24, 80, 3, 0, 2
 42, 2, 0, ':Thumbs:F1036_1.ithmb'
 'mhsd', 96, 660, 2
 'mhla', 92, 2
 'mhba', 148, 232, 1, 1, 101, 0, 65536, 0, 0, 0, 0, 0, 0, 0, 100
 'mhod', 24, 44, 1, 0, 1
 7, 1, 0, 'Library'
 'mhia', 40, 40, 0, 100
 'mhba', 148, 240, 1, 1, 102, 0, 393216, 0, 0, 0, 0, 0, 0, 0, 101
 'mhod', 24, 52, 1, 0, 3
 13, 1, 0, 'A Photo Album'
 'mhia', 40, 40, 0, 100
 'mhsd', 96, 684, 3
 'mhlf', 92, 4
 'mhif', 124, 124, 0, 1019, 691200
 'mhif', 124, 124, 0, 1015, 22880
 'mhif', 124, 124, 0, 1024, 153600
 'mhif', 124, 124, 0, 1036, 4100
```

## Data Types

- [Data File](./datafile.md)
- [DataSet](./dataset.md)
- [ImageList](./imagelist.md)
- [ImageItem](./imageitem.md)
- [ImageName](./imagename.md)
- [AlbumList](./albumlist.md)
- [PhotoAlbum](./photoalbum.md)
- [AlbumItem](./albumitem.md)
- [FileList](./filelist.md)
- [FileImage](./fileimage.md)
- [Data (mhod)](./data/index.md)
