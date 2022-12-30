using System;
using System.IO;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers
{
    internal abstract class BaseDatabase
    {
        public event EventHandler DatabaseWritten;

        protected CompatibilityType _compatibility;
        protected IPod _iPod;
        protected string _databaseFilePath;

        public IPod iPod => _iPod;

        public CompatibilityType Compatibility
        {
            get => _compatibility;
            set => _compatibility = value;
        }

        public abstract int Version { get; }
        public abstract void Parse();
        public abstract void Save();
        public abstract bool IsDirty { get; }

        protected void ReadDatabase(BaseDatabaseElement root)
        {
            var parseFilePath = GetParseFileName();

            var fs = new FileStream(parseFilePath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(fs);

            try
            {
                root.Read(iPod, reader);
                reader.Close();
                _compatibility = TestCompatibility(parseFilePath, root);
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
                var message =
                    $"The iPod database '{Path.GetFileName(_databaseFilePath)}' could not be read. Please run iTunes with your iPod connected, then try again. (Error at 0x{reader.BaseStream.Position.ToString("X")})";
                throw new ParseException(message, ex);
            }
            finally
            {
                reader.Close();
                CleanUpParseFile(parseFilePath);
            }
        }

        protected void WriteDatabase(BaseDatabaseElement root)
        {
            var tempDB = Path.GetTempFileName();
            var fs = new FileStream(tempDB, FileMode.Create, FileAccess.ReadWrite);
            var writer = new BinaryWriter(fs);
            root.Write(writer);
            writer.Flush();
            DoActionOnWriteDatabase(fs);

            if (fs.CanWrite)
            {
                writer.Flush();
            }
            writer.Close();

            //overwrite real database with temp
            _iPod.FileSystem.CopyFileToDevice(tempDB, _databaseFilePath);

            if (DatabaseWritten != null)
            {
                DatabaseWritten(this, null);
            }
        }

        public virtual void DoActionOnWriteDatabase(FileStream fileStream) { }

        public virtual void AssertIsWritable()
        {
            string msg;
            switch (_compatibility)
            {
                case CompatibilityType.NotWritable:
                    msg =
                        $"Your iPod ({_iPod.DeviceInfo.Family}, database version {Version}) is not writable. All iPod update features are disabled, but you can still copy files to your computer.";
                    throw new UnsupportedIPodException(msg);
                case CompatibilityType.UnsupportedNewDeviceOrFirmware:
                    msg =
                        "Looks like you have a new iPod! This version of Clickwheel does not fully support it yet. You can only copy files from your iPod to your computer.";
                    throw new UnsupportedIPodException(msg);
                case CompatibilityType.SourceDoesntMatchOutput:
                    msg =
                        "The iPod database failed to pass the Clickwheel compatibility test. All iPod update features are disabled. Upgrading the iPod to the latest iTunes version may fix the issue.";
                    throw new UnsupportedITunesVersionException(msg, _compatibility);
            }
        }

        internal CompatibilityType TestCompatibility(string dbFilePath, BaseDatabaseElement root)
        {
            var tempDB = Path.GetTempFileName();
            var fs = new FileStream(tempDB, FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(fs);
            root.Write(writer);
            writer.Close();
            return Helpers.TestCompatibility(dbFilePath, tempDB);
        }

        protected string GetParseFileName()
        {
            if (_iPod.FileSystem.ParseDbFilesLocally)
            {
                var parseFilePath = Path.GetTempFileName();
                _iPod.FileSystem.CopyFileFromDevice(_databaseFilePath, parseFilePath);
                return parseFilePath;
            }
            else
            {
                return _databaseFilePath;
            }
        }

        protected void CleanUpParseFile(string parseFileUsed)
        {
            if (_iPod.FileSystem.ParseDbFilesLocally)
            {
                File.Delete(parseFileUsed);
            }
        }
    }
}
