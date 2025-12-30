# ArtworkDB

The ArtworkDB file is only found on devices with color screens. It is created by iTunes 4.7 and up. This file, along with the thumbnail files iTunes creates, are what allows photos and album art to be displayed on iPod.

The structure of the ArtworkDB file is similar to the [Photo Database](<./Photo Database/index.md>). The Photo Database stores photos you add manually, the ArtworkDB stores album artwork for displaying when playing music.

Images are concatenated into *.ithmb files (thumbnails, basically).

Large thumbnails are stored separately from small thumbnails.

Large thumbnails are 140x140, small thumbnails are 56x56 - raw RGB565 packed color binary streams (bytes are swapped, little endian).

ArtworkDB Database layout:

```
<mhfd>
 <mhsd> (index = 1)
   <mhli>
     <mhii>
       <mhod> (type = 2) Info about full size thumbnail
         <mhni>
           <mhod> (type = 3)
       <mhod> (type = 2) Info about 'now playing' thumbnail
         <mhni>
           <mhod> (type = 3)
     <mhii>
       <mhod> (type = 2)
         <mhni>
           <mhod> (type = 3)
       <mhod> (type = 2)
         <mhni>
           <mhod> (type = 3)
      ...
 <mhsd> (index = 2)
   <mhla>
 <mhsd> (index = 3)
   <mhlf>
     <mhif>
     <mhif>
     ...
```

For more information on the structure of the ArtworkDB file, see [Photo Database](<./Photo Database/index.md>)
