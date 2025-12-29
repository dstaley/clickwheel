# ImageName (`mhni`)

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhni
4 | header length | 4 | size of the mhni header (0x4c)
8 | total length | 4 | size of the header and all child records
12 | number of children | 4 | mhni headers have one mhod type 3 child
16 | correlation ID | 4 | corresponds to mhif's correlation id, it's used to generate the name of the file containing the image, see below
20 | ithmb offset | 4 | offset where the start of image data can be found in the .ithmb file corresponding to this image
24 | image size | 4 | size of the image in bytes
28 | vertical padding | 2 | approximate difference between scaled image height and pixmap height (signed)
30 | horizontal padding | 2 | approximate difference between scaled image width and pixmap width (signed)
32 | image height | 2 | The height of the image.
34 | image width | 2 | The width of the image.
36 | unknown | 4 | Always zero?
40 | image size | 4 | size of the image in bytes (same as 0x18), written by iTunes 7.4

The correlation ID gives us the name of the file containing the image. For example, if the correlation ID is 1016 in decimal, then the corresponding filename will be F1016_1.ithmb.

In general, (vertical padding +image height) ~ pixmap height - usually within one or two pixels, probably due to rounding error. For instance, on a PhotoPod, an original image with dimensions 1200h x 1600v will have an NTSC image with image height=480, image width=558, vertical padding=0, and horizontal padding=162, with 558+162 = 720, the actual width of the pixel map. For an image scaled to be contained entirely within the pixel map, such as the video image or the full-screen image the padding values are basically the total width of the black bars.

For the smallest thumbnails, you can have negative values for padding, because the pixel map is scaled to be contained within the image - you get a central "slice" with no black bars.

As noted, there appear to be some rounding errors when the padding values are calculated, as the sums are sometimes off by 1 to 2.

There is no indication in this object what the pixel format, actual pixel map dimensions or rotation of images will be, so this must be entirely derived from the image size.

Here are the dimensions and formats for all known image sizes:

size | height | width | format | description
---- | ------ | ----- | ------ | -----------
691200 | 480 | 720 | UYVY | PhotoPod and VideoPod NTSC image
153600 | 240 | 320 | RGB565_LE | VideoPod full screen
80000 | 200 | 200 | RGB565_LE | VideoPod album art big version
77440 | 176 | 220 | RGB565_BE_90 | PhotoPod full screen
46464 | 132 | 176 | RGB565_BE | Nano full screen
39200 | 140 | 140 | RGB565_LE | PhotoPod album art big version
22880 | 88 | 130 | RGB565_LE | PhotoPod and VideoPod video preview
20000 | 100 | 100 | RGB565_LE | VideoPod album art small version, Nano album art big version
6272 | 56 | 56 | RGB565_LE | PhotoPod album art small version
4100 | 41 | 50 | RGB565_LE | VideoPod list thumbnail
3528 | 42 | 42 | RGB565_LE | Nano album art small version
3108 | 37 | 42 | RGB565_LE | Nano list thumbnail
2520 | 30 | 42 | RGB565_LE | PhotoPod list thumbnail

where:
- UYVY is a byte stream where U,Y0,V,Y1 creates two YUV pixels of Y0,U,V and Y1,U,V, interlaced, all even fields, then all odd fields.
- RGB565_LE is a stream of byte-swapped 16-bit pixels ordered from top->bottom, left->right
- RGB565_BE_90 is a stream of 16-bit pixels ordered right to left, top to bottom

The "full screen" images are rotated because the iPod displays used are actually portrait, not landscape, and this format is just a memory dump of the frame buffer memory.
