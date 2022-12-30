using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Clickwheel.DatabaseHash;
using Clickwheel.Parsers.iTunesDB;
using Microsoft.Data.Sqlite;

namespace Clickwheel.Parsers.iTunesCDB
{
    class Entry
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }

    class AlbumEntry : Entry
    {
        public long ArtistId { get; set; }
    }

    [SuppressMessage(
        "Microsoft.Cryptography",
        "CA5350:Do Not Use Weak Cryptographic Algorithms",
        Justification = "iPods require SHA1"
    )]
    class SqliteTables
    {
        protected IPod _iPod;
        protected List<Entry> _genres;
        protected List<AlbumEntry> _albums;
        protected List<Entry> _artists;
        protected List<Entry> _baseLocations;

        protected Dictionary<string, int> _locationKinds = new Dictionary<string, int>();

        protected int _nextGenreId = 1,
            _nextKindId = 1,
            _nextBaseLocationId = 1;
        protected long _nextAlbumId = 1,
            _nextArtistId = 1;
        protected bool _updatedLocationsDb,
            _updatedDynamicDb;
        protected SqliteConnection _libraryConnection,
            _locationsConnection,
            _dynamicConnection;
        protected SqliteCommand _insertItemCommand,
            _insertLocationCommand,
            _insertVideoInfoCommand;

        protected string _localLibraryFile,
            _localLocationsFile,
            _localDynamicFile;

        public SqliteTables(IPod iPod)
        {
            _iPod = iPod;

            Trace.WriteLine("Downloading and connecting to sqlite databases...");

            Initialize();

            _localLibraryFile = Path.Combine(iPod.Session.TempFilesPath, "library.itdb");
            _localLocationsFile = Path.Combine(iPod.Session.TempFilesPath, "locations.itdb");
            _localDynamicFile = Path.Combine(iPod.Session.TempFilesPath, "dynamic.itdb");

            if (!File.Exists(_localLibraryFile))
            {
                _iPod.FileSystem.CopyFileFromDevice(
                    Path.Combine(
                        _iPod.FileSystem.ITunesFolderPath,
                        "iTunes Library.itlp",
                        "Library.itdb"
                    ),
                    _localLibraryFile
                );
                _iPod.FileSystem.CopyFileFromDevice(
                    Path.Combine(
                        _iPod.FileSystem.ITunesFolderPath,
                        "iTunes Library.itlp",
                        "Locations.itdb"
                    ),
                    _localLocationsFile
                );
                _iPod.FileSystem.CopyFileFromDevice(
                    Path.Combine(
                        _iPod.FileSystem.ITunesFolderPath,
                        "iTunes Library.itlp",
                        "Dynamic.itdb"
                    ),
                    _localDynamicFile
                );
            }

            _libraryConnection = new SqliteConnection("Data Source=" + _localLibraryFile + ";");
            _libraryConnection.Open();

            _locationsConnection = new SqliteConnection("Data Source=" + _localLocationsFile + ";");
            _locationsConnection.Open();

            _dynamicConnection = new SqliteConnection("Data Source=" + _localDynamicFile + ";");
            _dynamicConnection.Open();

            Trace.WriteLine("Done downloading and connecting to sqlite databases");
        }

        public virtual void Initialize()
        { }

        public void Save()
        {
            Trace.WriteLine("Uploading sqlite databases...");
            _libraryConnection.Close();
            _locationsConnection.Close();
            _dynamicConnection.Close();
            _libraryConnection.Dispose();
            _locationsConnection.Dispose();
            _dynamicConnection.Dispose();
            SqliteConnection.ClearAllPools();

            Thread.Sleep(200);
            Trace.WriteLine("Done closing SQLite connections...");

            _iPod.FileSystem.CopyFileToDevice(
                _localLibraryFile,
                Path.Combine(
                    _iPod.FileSystem.ITunesFolderPath,
                    "iTunes Library.itlp",
                    "Library.itdb"
                )
            );
            Trace.WriteLine("Uploaded Library sqlite database");
            if (_updatedLocationsDb)
            {
                _iPod.FileSystem.CopyFileToDevice(
                    _localLocationsFile,
                    Path.Combine(
                        _iPod.FileSystem.ITunesFolderPath,
                        "iTunes Library.itlp",
                        "Locations.itdb"
                    )
                );
                Trace.WriteLine("Uploaded Locations sqlite database");
            }
            if (_updatedDynamicDb)
            {
                _iPod.FileSystem.CopyFileToDevice(
                    _localDynamicFile,
                    Path.Combine(
                        _iPod.FileSystem.ITunesFolderPath,
                        "iTunes Library.itlp",
                        "Dynamic.itdb"
                    )
                );
                Trace.WriteLine("Uploaded Dynamic sqlite database");
            }
            Trace.WriteLine("Done uploading sqlite databases");
        }

        protected virtual void ReadLookupTables()
        {
            _baseLocations = new List<Entry>();
            _genres = new List<Entry>();
            _albums = new List<AlbumEntry>();
            _artists = new List<Entry>();

            using (
                var cmd = new SqliteCommand(
                    "select id, path from base_location order by id",
                    _locationsConnection
                )
            )
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        if (reader.IsDBNull(1))
                        {
                            continue;
                        }

                        var path = reader.GetString(1);
                        _baseLocations.Add(new Entry { Id = id, Value = path });
                        _nextBaseLocationId = id + 1;
                    }
                }
            }

            using (
                var cmd = new SqliteCommand(
                    "select id, genre from genre_map order by id",
                    _libraryConnection
                )
            )
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                        {
                            _genres.Add(new Entry { Id = id, Value = reader.GetString(1) });
                        }

                        _nextGenreId = id + 1;
                    }
                }
            }

            using (
                var cmd = new SqliteCommand(
                    "select pid, name from artist order by pid",
                    _libraryConnection
                )
            )
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt64(0);
                        if (!reader.IsDBNull(1))
                        {
                            _artists.Add(new Entry { Id = id, Value = reader.GetString(1) });
                        }

                        _nextArtistId = id + 1;
                    }
                }
            }

            using (
                var cmd = new SqliteCommand(
                    "select pid, artist_pid, name from album order by pid",
                    _libraryConnection
                )
            )
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt64(0);
                        if (!reader.IsDBNull(2))
                        {
                            _albums.Add(
                                new AlbumEntry
                                {
                                    Id = id,
                                    ArtistId = reader.GetInt64(1),
                                    Value = reader.GetString(2)
                                }
                            );
                        }

                        _nextAlbumId = id + 1;
                    }
                }
            }

            using (
                var cmd = new SqliteCommand(
                    "select id, kind from location_kind_map order by id",
                    _libraryConnection
                )
            )
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                        {
                            _locationKinds.Add(reader.GetString(1), id);
                        }

                        _nextKindId = id + 1;
                    }
                }
            }
        }

        public virtual SqliteCommand GetUpdateTrackCommand()
        {
            var updateCmd = new SqliteCommand();
            updateCmd.CommandText =
                "update item set "
                + "year=@year, is_compilation=@is_compilation"
                + ",sort_title=@sort_title,title=@title"
                + ",track_number=@track_number, disc_number=@disc_number,genre_id=@genre_id"
                + ",sort_artist=@sort_artist,artist_pid=@artist_pid,artist=@artist"
                + ",sort_album=@sort_album,album_pid=@album_pid,album=@album"
                + ",album_artist=@album_artist"
                + ",is_song=@is_song,is_audio_book=@is_audio_book,is_music_video=@is_music_video,is_movie=@is_movie,is_tv_show=@is_tv_show,is_ringtone=@is_ringtone"
                + ",date_modified=@date_modified,remember_bookmark=@remember_bookmark,artwork_status=@artwork_status,artwork_cache_id=@artwork_cache_id"
                + " where pid=@pid";

            return updateCmd;
        }

        public virtual SqliteCommand GetInsertTrackCommand()
        {
            var command = new SqliteCommand();
            command.CommandText =
                "insert into item (pid, media_kind, is_song, is_audio_book, is_music_video, is_movie, is_tv_show, is_ringtone"
                + ",is_voice_memo, is_rental, is_podcast, date_modified, date_backed_up, year, content_rating, content_rating_level, is_compilation"
                + ",is_user_disabled, remember_bookmark, exclude_from_shuffle, artwork_status, artwork_cache_id, start_time_ms, total_time_ms, track_number"
                + ",track_count, disc_number, disc_count, bpm, relative_volume, genius_id, genre_id, category_id, album_pid, artist_pid, composer_pid, title"
                + ",artist, album, album_artist, composer, comment, description, description_long, in_songs_collection"
                + ",title_blank,artist_blank,album_artist_blank,album_blank,composer_blank, grouping_blank) "
                + " VALUES (@pid, @media_kind, @is_song, @is_audio_book, @is_music_video, @is_movie, @is_tv_show, @is_ringtone"
                + ",0, 0, 0, @date_modified, 0, @year, 0, 0, @is_compilation"
                + ",0, @remember_bookmark, 0, @artwork_status, @artwork_cache_id, 0, @total_time_ms, @track_number"
                + ",@track_count, @disc_number, @disc_count, 0, 0, 0, @genre_id, 0, @album_pid, @artist_pid, @composer_pid, @title"
                + ",@artist, @album, @album_artist, @composer, @comment, @description, @description_long, 1"
                + ",0,0,0,0,0,1);"
                + "insert into avformat_info(item_pid) values (@pid)";
            return command;
        }

        public virtual List<SqliteParameter> GetTrackParameters()
        {
            var parameters = new List<SqliteParameter>();
            parameters.Add(new SqliteParameter("@pid", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_song", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_audio_book", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_music_video", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_movie", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_tv_show", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_ringtone", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@date_modified", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@remember_bookmark", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@artwork_status", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@artwork_cache_id", SqliteType.Integer));

            parameters.Add(new SqliteParameter("@year", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@is_compilation", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@track_number", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@disc_number", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@genre_id", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@album_pid", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@artist_pid", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@title", SqliteType.Text));
            parameters.Add(new SqliteParameter("@sort_title", SqliteType.Text));
            parameters.Add(new SqliteParameter("@artist", SqliteType.Text));
            parameters.Add(new SqliteParameter("@sort_artist", SqliteType.Text));
            parameters.Add(new SqliteParameter("@album", SqliteType.Text));
            parameters.Add(new SqliteParameter("@sort_album", SqliteType.Text));
            parameters.Add(new SqliteParameter("@album_artist", SqliteType.Text));
            parameters.Add(new SqliteParameter("@album_artist_pid", SqliteType.Integer));

            parameters.Add(new SqliteParameter("@media_kind", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@total_time_ms", SqliteType.Real));
            parameters.Add(new SqliteParameter("@track_count", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@disc_count", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@composer_pid", SqliteType.Integer));
            parameters.Add(new SqliteParameter("@composer", SqliteType.Text));
            parameters.Add(new SqliteParameter("@comment", SqliteType.Text));
            parameters.Add(new SqliteParameter("@description", SqliteType.Text));
            parameters.Add(new SqliteParameter("@description_long", SqliteType.Text));
            return parameters;
        }

        public virtual void FillTrackParameters(
            SqliteCommand cmd,
            Track track,
            SqliteTransaction transaction
        )
        {
            cmd.Parameters["@pid"].Value = track.DBId;
            cmd.Parameters["@is_song"].Value = track.MediaType == MediaType.Audio ? 1 : 0;
            cmd.Parameters["@is_audio_book"].Value = track.MediaType == MediaType.Audiobook ? 1 : 0;
            cmd.Parameters["@is_music_video"].Value =
                track.MediaType == MediaType.MusicVideo ? 1 : 0;
            cmd.Parameters["@is_movie"].Value = track.MediaType == MediaType.Video ? 1 : 0;
            cmd.Parameters["@is_tv_show"].Value = track.MediaType == MediaType.TVShow ? 1 : 0;
            cmd.Parameters["@is_ringtone"].Value = track.MediaType == MediaType.Ringtone ? 1 : 0;
            cmd.Parameters["@date_modified"].Value = 0; // t.DateLastModified.TimeStamp;
            cmd.Parameters["@remember_bookmark"].Value = track.RememberPlaybackPosition ? 1 : 0;
            cmd.Parameters["@artwork_status"].Value = track.Artwork.Count > 0 ? 2 : 0;
            cmd.Parameters["@artwork_cache_id"].Value = track.ArtworkIdLink;
            cmd.Parameters["@year"].Value = track.Year;
            cmd.Parameters["@is_compilation"].Value = track.IsCompilation ? 1 : 0;
            cmd.Parameters["@track_number"].Value = track.TrackNumber;
            cmd.Parameters["@disc_number"].Value = track.DiscNumber;
            cmd.Parameters["@genre_id"].Value = GetGenreId(track.Genre, transaction);

            var artistId = GetArtistId(track.Artist, transaction);
            var albumArtistId = GetAlbumArtistId(track.AlbumArtist, transaction);
            cmd.Parameters["@artist_pid"].Value = artistId;
            cmd.Parameters["@album_artist_pid"].Value = albumArtistId;
            cmd.Parameters["@album_pid"].Value = GetAlbumId(track, albumArtistId, transaction);
            cmd.Parameters["@title"].Value = track.Title;
            cmd.Parameters["@sort_title"].Value = track.SortTitle;
            cmd.Parameters["@sort_artist"].Value = track.SortArtist;
            cmd.Parameters["@sort_album"].Value = track.SortAlbum;
            cmd.Parameters["@artist"].Value = track.Artist;
            cmd.Parameters["@album"].Value = track.Album;
            cmd.Parameters["@album_artist"].Value = track.AlbumArtist;

            cmd.Parameters["@media_kind"].Value = track.MediaType;
            cmd.Parameters["@total_time_ms"].Value = (double)track.Length.MilliSeconds;
            cmd.Parameters["@track_count"].Value = 0;
            cmd.Parameters["@disc_count"].Value = track.TotalDiscCount;
            cmd.Parameters["@composer_pid"].Value = 0;
            cmd.Parameters["@composer"].Value = DBNull.Value;
            cmd.Parameters["@comment"].Value = track.Comment;
            cmd.Parameters["@description"].Value = track.DescriptionText;
            cmd.Parameters["@description_long"].Value = DBNull.Value;
        }

        public void UpdateTracks(List<Track> tracks)
        {
            Trace.WriteLine("Updating tracks in sqlite databases...");

            ReadLookupTables();

            var updateCommand = GetUpdateTrackCommand();
            updateCommand.Parameters.AddRange(GetTrackParameters().ToArray());
            updateCommand.Connection = _libraryConnection;

            using (var transaction = _libraryConnection.BeginTransaction())
            {
                var existsCmd = new SqliteCommand(
                    "select count(*) from item where pid = @pid",
                    _libraryConnection,
                    transaction
                );
                existsCmd.Parameters.Add(new SqliteParameter("@pid", SqliteType.Integer));

                foreach (var t in tracks)
                {
                    existsCmd.Parameters["@pid"].Value = t.DBId;

                    var itemExists = ((long)existsCmd.ExecuteScalar()) > 0;

                    if (!itemExists)
                    {
                        InsertNewTrack(t, transaction);
                    }
                    else
                    {
                        FillTrackParameters(updateCommand, t, transaction);
                        updateCommand.Transaction = transaction;
                        var rows = updateCommand.ExecuteNonQuery();
                    }
                }

                existsCmd.Dispose();

                RemoveDeletedTracks();

                Cleanup(transaction);
                transaction.Commit();
            }

            updateCommand.Dispose();
            if (_insertItemCommand != null)
            {
                _insertItemCommand.Dispose();
                _insertItemCommand = null;
                _insertLocationCommand.Dispose();
                _insertLocationCommand = null;
                _insertVideoInfoCommand.Dispose();
                _insertVideoInfoCommand = null;
            }

            Trace.WriteLine("Done updating tracks in sqlite databases");
        }

        /// <summary>
        /// Insert a new track into the Sqlite database. Library.itdb and Locations.itdb are both updated.
        /// </summary>
        private void InsertNewTrack(Track track, SqliteTransaction transaction)
        {
            if (_insertItemCommand == null)
            {
                _insertItemCommand = GetInsertTrackCommand();
                _insertItemCommand.Connection = _libraryConnection;
                _insertItemCommand.Transaction = transaction;
                _insertItemCommand.Parameters.AddRange(GetTrackParameters().ToArray());

                _insertLocationCommand = new SqliteCommand(
                    "insert into location (item_pid, sub_id, base_location_id, location_type, location, extension, kind_id, file_size)"
                        + "values (@item_pid, 0, @base_location_id, @location_type, @location, @extension, @kind_id, @file_size)",
                    _locationsConnection
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@item_pid", SqliteType.Integer)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@base_location_id", SqliteType.Integer)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@location_type", SqliteType.Integer)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@location", SqliteType.Text)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@extension", SqliteType.Integer)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@kind_id", SqliteType.Integer)
                );
                _insertLocationCommand.Parameters.Add(
                    new SqliteParameter("@file_size", SqliteType.Integer)
                );

                _insertVideoInfoCommand = new SqliteCommand(
                    "insert into video_info (item_pid, has_alternate_audio, has_subtitles, characteristics_valid"
                        + ",has_closed_captions, is_self_contained, is_compressed, is_anamorphic, season_number, audio_language, audio_track_index, audio_track_id"
                        + ",subtitle_language, subtitle_track_index, subtitle_track_id, episode_sort_id) values (@item_pid, 0,0,0,0,1,0,0,0,0,0,0,0,0,0,0)",
                    _libraryConnection
                );
                _insertVideoInfoCommand.Parameters.Add(
                    new SqliteParameter("@item_pid", SqliteType.Integer)
                );
            }

            FillTrackParameters(_insertItemCommand, track, transaction);
            var rows = _insertItemCommand.ExecuteNonQuery();

            _insertLocationCommand.Parameters["@item_pid"].Value = track.DBId;
            _insertLocationCommand.Parameters["@base_location_id"].Value = GetBaseLocation(
                track.FilePath
            ).Id;
            _insertLocationCommand.Parameters["@location_type"].Value = 1179208773;

            var location = GetBaseLocation(track.FilePath);
            _insertLocationCommand.Parameters["@location"].Value = track.FilePath[
                (location.Value.Length + 1)..
            ].Replace("\\", "/", StringComparison.InvariantCulture);

            _insertLocationCommand.Parameters["@extension"].Value = GetExtensionId(track.FilePath);
            _insertLocationCommand.Parameters["@kind_id"].Value = GetKindId(
                track.FileType,
                transaction
            );
            _insertLocationCommand.Parameters["@file_size"].Value = track.FileSize.ByteCount;
            rows = _insertLocationCommand.ExecuteNonQuery();

            if (track.IsVideo)
            {
                _insertVideoInfoCommand.Parameters["@item_pid"].Value = track.DBId;
                _insertLocationCommand.Transaction = transaction;
                _insertVideoInfoCommand.ExecuteNonQuery();
            }
            _updatedLocationsDb = true;
        }

        /// <summary>
        /// Remove all tracks which have been deleted this session.
        /// </summary>
        private void RemoveDeletedTracks()
        {
            Trace.WriteLine("Removing tracks from sqlite databases...");
            var deleteLocationCmd = new SqliteCommand(
                "delete from location where item_pid=@pid",
                _locationsConnection
            );
            var deleteItemCmd = new SqliteCommand(
                "delete from item where pid=@pid;delete from item_to_container where item_pid=@pid;"
                    + "delete from avformat_info where item_pid=@pid;delete from video_characteristics where item_pid=@pid;/*delete from video_info where item_pid=@pid*/",
                _libraryConnection
            );
            deleteLocationCmd.Parameters.Add(new SqliteParameter("@pid", SqliteType.Integer));
            deleteItemCmd.Parameters.Add(new SqliteParameter("@pid", SqliteType.Integer));

            foreach (var track in _iPod.Session.DeletedTracks)
            {
                deleteItemCmd.Parameters["@pid"].Value = track.DBId;
                deleteLocationCmd.Parameters["@pid"].Value = track.DBId;
                deleteItemCmd.ExecuteNonQuery();
                deleteLocationCmd.ExecuteNonQuery();
                Trace.WriteLine("Deleted " + track.Title + " from sqlite database");
                _updatedLocationsDb = true;
            }
            deleteItemCmd.Dispose();
            deleteLocationCmd.Dispose();
            Trace.WriteLine("Done removing tracks from sqlite databases");
        }

        public virtual void Cleanup(SqliteTransaction transaction)
        {
            //delete entries from album table where there are no matching remaining
            var sql = "delete from album where pid not in " + "(select album_pid from item)";

            var cmd = new SqliteCommand(sql, _libraryConnection, transaction);
            var rows = cmd.ExecuteNonQuery();
            cmd.Dispose();

            //delete entries from artist table where there are no matching remaining
            sql = "delete from artist where pid not in " + "(select artist_pid from item)";
            cmd = new SqliteCommand(sql, _libraryConnection, transaction);
            rows = cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static int GetExtensionId(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            return ext switch
            {
                ".mp3" => 1297101600,
                ".mp4" => 1297101856,
                ".m4a" => 1295270176,
                ".m4r" => 1295274528,
                ".m4v" => 1295275552,
                ".wav" => 1463899680,
                ".aif" => 1095321120,
                _ => 1297101600
            };
        }

        protected virtual Entry GetBaseLocation(string path)
        {
            path = path.Replace("\\", "/", StringComparison.InvariantCulture);
            var entry = _baseLocations.Find(
                l => path.StartsWith(l.Value, StringComparison.InvariantCultureIgnoreCase)
            );
            if (entry == null)
            {
                using (
                    var cmd = new SqliteCommand(
                        "insert into base_location (id, path) values(@id, @path)",
                        _locationsConnection
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@id", _nextBaseLocationId);
                    if (
                        path.Contains(
                            "itunes_control/ringtones",
                            StringComparison.InvariantCultureIgnoreCase
                        )
                    )
                    {
                        cmd.Parameters.AddWithValue("@path", "iTunes_Control/Ringtones");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@path", "iTunes_Control/Music");
                    }

                    cmd.ExecuteNonQuery();
                }
                entry = new Entry { Id = _nextBaseLocationId++, Value = path };
                _baseLocations.Add(entry);
            }
            return entry;
        }

        protected virtual long GetGenreId(string genre, SqliteTransaction transaction)
        {
            var entry = _genres.Find(
                g => g.Value.Equals(genre, StringComparison.OrdinalIgnoreCase)
            );
            if (entry == null)
            {
                using (
                    var cmd = new SqliteCommand(
                        "insert into genre_map (id, genre) values(@id, @genre)",
                        _libraryConnection,
                        transaction
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@id", _nextGenreId);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.ExecuteNonQuery();
                }
                entry = new Entry { Id = _nextGenreId++, Value = genre };
                _genres.Add(entry);
            }
            return entry.Id;
        }

        protected virtual long GetAlbumId(Track track, long artistId, SqliteTransaction transaction)
        {
            var entry = _albums.Find(
                a =>
                    a.Value.Equals(track.Album, StringComparison.OrdinalIgnoreCase)
                    && a.ArtistId == artistId
            );

            if (entry == null)
            {
                using (
                    var cmd = new SqliteCommand(
                        "insert into album (pid, kind, name, artist_pid, artwork_status, artwork_item_pid) values(@pid, @kind, @name, @artist_pid, 0, 0)",
                        _libraryConnection,
                        transaction
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@pid", _nextAlbumId);
                    cmd.Parameters.AddWithValue("@kind", 2); //music?
                    cmd.Parameters.AddWithValue("@name", track.Album);
                    cmd.Parameters.AddWithValue("@artist_pid", artistId);
                    cmd.ExecuteNonQuery();
                }
                entry = new AlbumEntry
                {
                    Id = _nextAlbumId++,
                    ArtistId = artistId,
                    Value = track.Album
                };
                _albums.Add(entry);
            }
            return entry.Id;
        }

        protected virtual long GetArtistId(string artist, SqliteTransaction transaction)
        {
            var artistEntry = _artists.Find(
                a => a.Value.Equals(artist, StringComparison.OrdinalIgnoreCase)
            );
            if (artistEntry == null)
            {
                using (
                    var cmd = new SqliteCommand(
                        "insert into artist (pid, kind, name) values(@pid, @kind, @name)",
                        _libraryConnection,
                        transaction
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@pid", _nextArtistId);
                    cmd.Parameters.AddWithValue("@kind", 2); //music?
                    cmd.Parameters.AddWithValue("@name", artist);
                    cmd.ExecuteNonQuery();
                }
                artistEntry = new Entry { Value = artist, Id = _nextArtistId++ };
                _artists.Add(artistEntry);
            }
            return artistEntry.Id;
        }

        protected virtual long GetAlbumArtistId(string artist, SqliteTransaction transaction)
        {
            return GetArtistId(artist, transaction);
        }

        private int GetKindId(string kind, SqliteTransaction transaction)
        {
            if (!_locationKinds.ContainsKey(kind))
            {
                using (
                    var cmd = new SqliteCommand(
                        "insert into location_kind_map (id, kind) values(@id, @kind)",
                        _libraryConnection,
                        transaction
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@id", _nextKindId);
                    cmd.Parameters.AddWithValue("@kind", kind); //music?
                    cmd.ExecuteNonQuery();
                }
                _locationKinds.Add(kind, _nextKindId);
                _nextKindId++;
            }
            return _locationKinds[kind];
        }

        /// <summary>
        /// Syncs up the Sqlite playlists with the iTunesCDB playlists.
        /// </summary>
        /// <param name="playlists"></param>
        public void UpdatePlaylists(List<Playlist> playlists)
        {
            Trace.WriteLine("Updating playlists in sqlite databases...");
            var select = new SqliteCommand(
                "select item_pid from item_to_container where container_pid = @pid",
                _libraryConnection
            );

            var insert = new SqliteCommand(
                "insert into item_to_container (item_pid, container_pid) values (@item_pid, @container_pid)",
                _libraryConnection
            );
            insert.Parameters.Add(new SqliteParameter("@item_pid", SqliteType.Integer));
            insert.Parameters.Add(new SqliteParameter("@container_pid", SqliteType.Integer));

            var delete = new SqliteCommand(
                "delete from item_to_container where item_pid = @item_pid and container_pid = @container_pid",
                _libraryConnection
            );
            delete.Parameters.Add(new SqliteParameter("@item_pid", SqliteType.Integer));
            delete.Parameters.Add(new SqliteParameter("@container_pid", SqliteType.Integer));

            var transaction = _libraryConnection.BeginTransaction();

            foreach (var pl in playlists)
            {
                long count = 0;
                // Handle creation/update of playlist
                using (
                    var cmd = new SqliteCommand(
                        "select count(*) from container where pid = @pid",
                        _libraryConnection,
                        transaction
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@pid", (long)pl.Id);
                    count = (long)cmd.ExecuteScalar();
                }
                if (count == 0)
                {
                    CreatePlaylist(pl, transaction);
                }
                else
                {
                    using (
                        var cmd = new SqliteCommand(
                            "update container set name = @name where pid = @pid",
                            _libraryConnection,
                            transaction
                        )
                    )
                    {
                        cmd.Parameters.AddWithValue("@pid", (long)pl.Id);
                        cmd.Parameters.AddWithValue("@name", pl.Name);
                        cmd.ExecuteNonQuery();
                    }
                }

                var sqlItemIds = new List<long>();

                var select2 = new SqliteCommand(
                    "select item_pid from item_to_container where container_pid = @pid",
                    _libraryConnection,
                    transaction
                );
                select2.Parameters.AddWithValue("@pid", (long)pl.Id);
                using (var reader = select2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sqlItemIds.Add(reader.GetInt64(0));
                    }
                }
                select2.Dispose();

                // Update tracks just added to this playlist
                var updatedIds = new List<long>();
                foreach (var t in pl.Tracks)
                {
                    if (!sqlItemIds.Contains(t.DBId))
                    {
                        updatedIds.Add(t.DBId);
                    }
                }

                if (updatedIds.Count > 0)
                {
                    insert.Parameters["@container_pid"].Value = (long)pl.Id;
                    insert.Transaction = transaction;
                    foreach (var id in updatedIds)
                    {
                        insert.Parameters["@item_pid"].Value = id;
                        insert.ExecuteNonQuery();
                    }
                }

                // Remove tracks just removed from this playlist
                updatedIds.Clear();
                foreach (var id in sqlItemIds)
                {
                    if (!pl.ContainsTrack(id))
                    {
                        updatedIds.Add(id);
                    }
                }

                if (updatedIds.Count > 0)
                {
                    delete.Parameters["@container_pid"].Value = (long)pl.Id;
                    delete.Transaction = transaction;
                    foreach (var id in updatedIds)
                    {
                        delete.Parameters["@item_pid"].Value = id;
                        delete.ExecuteNonQuery();
                    }
                }
            }

            select.Dispose();
            insert.Dispose();
            delete.Dispose();

            //Delete playlists weve deleted from iTunesCDB
            delete = new SqliteCommand(
                "delete from container where pid=@container_pid; delete from item_to_container where container_pid=@container_pid",
                _libraryConnection,
                transaction
            );
            delete.Parameters.Add(new SqliteParameter("@container_pid", SqliteType.Integer));

            foreach (var playlist in _iPod.Session.DeletedPlaylists)
            {
                delete.Parameters["@container_pid"].Value = (long)playlist.Id;
                delete.ExecuteNonQuery();
            }
            delete.Dispose();
            transaction.Commit();
            transaction.Dispose();

            Trace.WriteLine("Done updating playlists in sqlite databases");
        }

        protected virtual void CreatePlaylist(Playlist pl, SqliteTransaction transaction)
        {
            using (
                var cmd = new SqliteCommand(
                    "insert into container (pid, distinguished_kind, name, parent_pid, media_kinds, workout_template_id, is_hidden, smart_is_folder)"
                        + "values (@pid, 0, @name, 0, @media_kinds, 0, 0, 0)",
                    _libraryConnection,
                    transaction
                )
            )
            {
                cmd.Parameters.AddWithValue("@pid", (long)pl.Id);
                cmd.Parameters.AddWithValue("@name", pl.Name);
                cmd.Parameters.AddWithValue("@media_kinds", 1);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// This is where the magic happens. Without this, the iPod will refuse to accept any changes.
        /// </summary>
        public void UpdateLocationsCbk()
        {
            if (!_updatedLocationsDb)
            {
                return;
            }

            Trace.WriteLine("Updating locations cbk file...");

            using var sha1 = SHA1.Create();
            using var locations = File.Open(_localLocationsFile, FileMode.Open);
            var reader = new BinaryReader(locations);

            var sha1Buffer = new byte[((reader.BaseStream.Length / 1024)) * 20];
            var ptr = 0;
            while (true)
            {
                var buffer = reader.ReadBytes(1024);
                var hashed = sha1.ComputeHash(buffer);
                Array.Copy(hashed, 0, sha1Buffer, ptr, 20);
                ptr += 20;
                if (reader.BaseStream.Position + 1024 > reader.BaseStream.Length)
                {
                    break;
                }
            }
            var finalSha1Hash = sha1.ComputeHash(sha1Buffer);
            reader.Close();

            var hashInfo = new HashInfo();
            hashInfo.Generate(_iPod.DeviceInfo.FirewireId);
            var calculatedHash = Hash72.CalculateHash(finalSha1Hash, hashInfo.RndPart, hashInfo.Iv);
            var tempCbk = Path.GetTempFileName();
            var writer = new BinaryWriter(File.Open(tempCbk, FileMode.Create));
            writer.Write(calculatedHash);
            writer.Write(finalSha1Hash);
            writer.Write(sha1Buffer);
            writer.Close();
            _iPod.FileSystem.CopyFileToDevice(
                tempCbk,
                Path.Combine(
                    _iPod.FileSystem.ITunesFolderPath,
                    "iTunes Library.itlp",
                    "Locations.itdb.cbk"
                )
            );
            File.Delete(tempCbk);

            Trace.WriteLine("Done updating locations cbk file");
        }

        protected static string GetSortString(string item)
        {
            if (item.StartsWith("The ", StringComparison.InvariantCulture))
            {
                return item[4..];
            }
            else if (item.StartsWith("A ", StringComparison.InvariantCulture))
            {
                return item[2..];
            }

            return item;
        }
    }
}
