# Smart Playlist Rule (51)

The most complex (and annoying) MHOD. These are any MHOD with a "type" that is 51. This MHOD defines all the rules in the Smart Playlist. If you've used iTunes, you know what a rule looks like. "Rating is less than 3 stars", for example. The rule consists of three parts. The first part is the "field". In our example, the field is "Rating". It's the very first thing in the rule and the first pull down box in iTunes. The second part is the "action". In our example, the action is "is less than". In iTunes, it's the second pull down box. The final part is the "value". In our example, it'd be "3 stars". Since stars are always multiplied by 20, it'd be "60" for our example, in the iTunesDB file.

**Important Note about endian-ness**: Smart Playlist Rules MHODs are NOT wholly little-endian. Everything after the "SLst" to the end of the MHOD is big-endian. This is important to remember, especially when dealing with the Action types.

offset | field | size | value
------ | ----- | ---- | -----
0 | header identifier | 4 | mhod
4 | header length | 4 | size of the mhod header.
8 | total length | 4 | size of the header and the rules it contains
12 | type | 4 | the type indicator  ( 51 )
16 | unk1 | 4 | unknown
20 | unk2 | 4 | unknown
24 | smart list rules identifier | 4 | "SLst" (note that this is the point at which bytes are no longer little-endian in the mhod.. it switches to big-endian at this point)
28 | unk5 | 4 | unknown
32 | number of rules | 4 | number of rules
36 | rules operator | 4 | 0 = AND (as in "Match All"), 1 = OR (as in "Match Any")
40 | padding | 120 | zero padding
160 | rules | whatever size the rules are | the rules themselves, one after another.

Smart Playlist Rule mhods are **NOT** zero padded.

## Smart Playlist Rule Fields

value | description | expected comparison
----- | ----------- | -------------------
0x02 | Song Name | String
0x03 | Album | String
0x04 | Artist | String
0x05 | Bitrate | Integer
0x06 | Sample Rate | Integer
0x07 | Year | Integer
0x08 | Genre | String
0x09 | Kind | String
0x0a | Date Modified | Timestamp
0x0b | Track Number | Integer
0x0c | Size | Integer
0x0d | Time | Integer
0x0e | Comment | String
0x10 | Date Added | Timestamp
0x12 | Composer | String
0x16 | Play Count | Integer
0x17 | Last Played | Timestamp
0x18 | Disc Number | Integer
0x19 | Stars/Rating | Integer (multiply by 20 for stars/rating)
0x1f | Compilation | Integer
0x23 | BPM | Integer
0x27 | Grouping | String (see special note)
0x28 | Playlist | Integer - the playlist ID number (see special note)
0x36 | Description | String
0x37 | Category | String
0x39 | Podcast | Integer
0x3c | Video Kind | Logic integer, works on mediatype
0x3e | TV Show | String
0x3f | Season Nr | Integer
0x44 | Skip Count | Integer
0x45 | Last Skipped | Timestamp
0x47 | Album Artist | String

Special Note about Grouping and Playlist fields - They don't work with Live Updating on 3rd gen iPods, yet. This might get fixed in a future firmware release. Maybe.

## Smart Playlist Rule Actions

The Action type is a 4 byte field. It is a bitmapped value, meaning that each bit of these four bytes has a different meaning.

## High byte

bit | description
--- | -----------
0 | The action is referring to a string value if set, not a string if not set
1 | NOT flag. If set, this negates the rule. Is becomes is not, contains becomes does not contain, and so forth.

## Low bytes

bit | description
--- | -----------
0 | Simple "IS" query
1 | Contains
2 | Begins with
3 | Ends with
4 | Greater Than
5 | Greater Than or Equal To
6 | Less Than
7 | Less Than or Equal To
8 | Is in the Range
9 | In the Last
10 | Is / Is Not (binary AND, only used for "Video Kind" so far)

## Actions

value | action
----- | ------
0x00000001 | Is Int (also Is Set in iTunes)
0x00000010 | Is Greater Than (also Is After in iTunes)
0x00000020 | Is Greater Than Or Equal To (not in iTunes)
0x00000040 | Is Less Than (also Is Before in iTunes)
0x00000080 | Is Less Than Or Equal To (not in iTunes)
0x00000100 | Is in the Range
0x00000200 | Is in the Last
0x00000400 | Is / Is Not (Binary AND, used for media type so far)
0x01000001 | Is String
0x01000002 | Contains
0x01000004 | Starts With
0x01000008 | Ends With
0x02000001 | Is Not Int (also Is Not Set in iTunes)
0x02000010 | Is Not Greater Than (not in iTunes)
0x02000020 | Is Not Greater Than Or Equal To (not in iTunes)
0x02000040 | Is Not Less Than (not in iTunes)
0x02000080 | Is Not Less Than Or Equal To (not in iTunes)
0x02000100 | Is Not In the Range (not in iTunes)
0x02000200 | Is Not In The Last
0x03000001 | Is Not
0x03000002 | Does Not Contain
0x03000004 | Does Not Start With (not in iTunes)
0x03000008 | Does Not End With (not in iTunes)

## Values

Values are generally pretty straightforward. For String rules, it's a string. For Integer and Timestamp rules, it's a bit more complicated. Furthermore, the "In the Last" type action requires a Units value as well.

So there are two major rule formats, the String Rule and the Non-String Rule.

### SPLRule String format

offset | field | size | value
------ | ----- | ---- | -----
0 | field | 4 | The Field type
4 | action | 4 | The Action type
8 | padding | 44 | zero padding
52 | length | 4 | length of the string, in bytes. Maximum length is 255
56 | string | length | the string in UTF-16 format (2 bytes per character)

Rules are NOT zero padded at the end.

### SPLRule Non-String format

offset | field | size | value
------ | ----- | ---- | -----
0 | field | 4 | The Field type
4 | action | 4 | The Action type
8 | padding | 44 | zero padding
52 | length | 4 | always 0x44 for non-string types
56 | from value | 8 | the from value in an 8 byte form (unsigned int64)
64 | from date | 8 | the from date in an 8 byte form (signed int64)
72 | from units | 8 | the from units in an 8 byte form (unsigned int64)
80 | to value | 8 | the to value in an 8 byte form (unsigned int64)
88 | to date | 8 | the to date in an 8 byte form (signed int64)
96 | to units | 8 | the to units in an 8 byte form (unsigned int64)
104 | unknown | 20 | unknown, used by all field types, unknown purpose

Rules are NOT zero padded at the end.

For integer type rules, the from and to values are the ones that you care about, the date = 0, and the units = 1.

```
Example: BPM is less than 150
field = 0x23 (BPM)
action = 0x00000040 (is less than)
from and to value = 150
from and to date = 0
from and to units = 1
```

```
Example: BPM is in the range 70 to 150
field = 0x23 (BPM)
action = 0x00000100 (is in the range)
from value = 70
to value = 150
from and to date = 0
from and to units = 1
```

For binary and type rules, the from and to values are the ones that you care about, the date = 0, and the units = 1.

```
Example: Video Kind is TV-Show
field = 0x3c (Video Kind)
action = 0x00000400 (Is / Is Not)
from and to value = 0x0040
from and to date = 0
from and to units = 1
```

```
Example: Video Kind is not TV-Show
field = 0x3c (Video Kind / mediatype)
action = 0x00000400 (Is / Is Not)
from and to value = 0x0e22
from and to date = 0
from and to units = 1
```

For the latter one would expect a value of 0x0022 (either Movie or Music Video). The additional 0x0e00 in the mask hints for further video types to come.

Timestamp type rules use the same format as integer type rules, only the from and to values are Mac timestamps. This is the number of seconds since 1/1/1904.

```
Example: Date Added is in the range 6/19/2004 to 6/20/2004
field = 0x10 (Date Added)
action = 0x00000100 (is in the range)
from value = bcfa83ff (6/19/2004) 
to value = 0xbcfbd57f (6/20/2004)
from and to date = 0
from and to units = 1
```

For "in the last" type rules, the from and to values are set to a constant of 0x2dae2dae2dae2dae. The from dates become the value, and the from units becomes the unit you're measuring in. The way to think of this is that it's saying "Today (2dae) plus this number of units".

```
Example: Last Played Time is in the last 2 weeks.
field = 0x17 (Last Played Time)
action = 0x00000200 (is in the last)
from value = 0x2dae2dae2dae2dae
from date = -2
from units = 604800 (number of seconds in a week)
to value = 0x2dae2dae2dae2dae
to date = 0
to units = 1
```

That rule is saying "Today minus 2 times this number of seconds" which is 2 weeks before "now", whatever now happens to be.
If you're creating your own rules in an iTunesDB, you may find it more convienent to leave the units set to "1" all the time and just put the number of seconds into the date field. This is perfectly acceptable and the iPod can handle it just fine. If you're not sure how to make a particular rule, create it or a similar one in iTunes and put it on the iPod, then examine the iTunesDB file to see how it did it.

But if you're programming the iPod side of things, you need to be able to correctly understand the units field and deal with it accordingly. The best way to do this is to always compare the contents of the field in question with (value+date*unit), and replacing the "value" with the current timestamp ("now") when it is equal to 3,291,618,603,768,360,366 (0x2dae2dae2dae2dae). This will work for *all* integer and timestamp comparisons if done correctly. It's also exactly what the Apple iPod firmware does. It's also why you have to set the time/date on an iPod for these smart playlists to work correctly (3rd gen and up).
Also, remember that all timestamps should be dealt with in Apple format, which is the number of seconds since 1/1/1904.
