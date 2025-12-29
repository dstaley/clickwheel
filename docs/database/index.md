# iPod Databases

The database directory on the iPod is `/iPod_Control/iTunes/`. This is a hidden folder (attrib +h), but is not otherwise protected.

## Files

<!-- prettier-ignore-start -->
Filename | Description
-------- | -----------
[iTunesDB](./iTunesDB/index.md) | This is the primary database for the iPod. It contains all information about the songs that the iPod is capable of playing, as well as the playlists. It's never written to by the Apple iPod firmware. During an autosync, iTunes completely overwrites this file.
[iTunesCDB](./iTunesCDB.md) | A compressed variant of iTunesDB used on newer iPods (e.g., iPod nano (5th generation)). Uses zlib compression and requires companion SQLite databases.
[iTunesSD](./iTunesSD/index.md) | This is the primary database for iPod shuffle.
[Play Counts](./playcounts.md) | This is the return information file for the iPod. It contains all information that is available to change via the iPod, with regards to the song. When you autosync, iTunes reads this file and updates the iTunes database accordingly. After it does this, it erases this file, so as to prevent it from duplicating data by mistake. The iPod will create this file on playback if it is not there.
[ArtworkDB](./ArtworkDB.md) | This is where data about artwork is stored on iPod. The artwork itself is stored in the `\iPod_Control\Artwork` folder. On 5th generation iPods the ArtworkDB is stored in the Artwork folder along with the data.
[Photo Database](<./Photo Database/index.md>) | This is where data about photos is stored on iPod.
<!-- prettier-ignore-end -->

## Endian Note

With some exceptions, most of the data descriptions are actually stored in the file in little endian byte order. Meaning that you have these representations of the data when looking at the file in a hex editor:

```
"01 00 00 00" = decimal 1
"10 00 00 00" = decimal 16
"00 01 00 00" = decimal 256
```

This means when you read the file, you need to reverse the bytes in memory to make sense of them. Keep this in mind when trying to make sense of an iTunesDB using a hex editor.

The exceptions are noted where appropriate. But if you are looking at a piece of a file and can't understand the value it holds, try reversing the byte order.

Note that this means the values themselves are in reversed byte order. The order of the fields described is the same as in the documentation. So if you see that field A is before field B in a table, then when you look at the file with a hex editor, field A really will come before field B.

## Credits

The documentation contained within the `docs/database` directory was originally taken from the [iPod Linux Wiki](http://www.ipodlinux.org/ITunesDB). The documentation for the [3rd and 4th generation iTunesSD database]() was taken from [nims11/IPod-Shuffle-4g](https://github.com/nims11/IPod-Shuffle-4g/blob/master/docs/iTunesSD3gen.md).
