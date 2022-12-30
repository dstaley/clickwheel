using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Clickwheel
{
    /// <summary>
    /// Utility class to log events and exceptions to a file. Clickwheel will write some events to this log if enabled. User applications may
    /// also use this to log non-Clickwheel events.
    /// </summary>
    class TraceLogger : TraceListener
    {
        private StreamWriter _file;
        private object _lockObject;

        public TraceLogger(object locker)
        {
            _lockObject = locker;
        }

        public void SetFileStream(StreamWriter stream)
        {
            _file = stream;
        }

        public override void Write(string message)
        {
            if (_file == null)
            {
                return;
            }

            lock (_lockObject)
            {
                _file.Write(message);
                _file.Flush();
            }
        }

        public override void WriteLine(string message)
        {
            if (_file == null)
            {
                return;
            }

            lock (_lockObject)
            {
                _file.WriteLine(message);
                _file.Flush();
            }
        }
    }

    /// <summary>
    /// Utility class to log Trace.Write/WriteLine events and exceptions to a file.
    /// Clickwheel will write some events to this log if enabled. User applications may also use this to log non-Clickwheel events.
    /// </summary>
    public static class DebugLogger
    {
        private static bool _isLogging;
        private static StreamWriter _file;
        private static TraceLogger _traceLogger;
        private static object _lockObject;

        static DebugLogger()
        {
            _lockObject = new object();
            _traceLogger = new TraceLogger(_lockObject);
            Trace.Listeners.Add(_traceLogger);
        }

        /// <summary>
        /// Start logging.
        /// </summary>
        /// <param name="filename"></param>
        public static void StartLogging(string filename)
        {
            //Make sure we aren't already logging.
            StopLogging();

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                _file = File.CreateText(filename);
                _isLogging = true;
                _traceLogger.SetFileStream(_file);

                var libFileName = Assembly.GetExecutingAssembly().GetModules()[
                    0
                ].FullyQualifiedName;
                Trace.WriteLine("===============================");
                Trace.WriteLine(
                    "Clickwheel Version: " + FileVersionInfo.GetVersionInfo(libFileName).FileVersion
                );
                Trace.WriteLine("===============================");
                Trace.WriteLine(Environment.OSVersion.VersionString);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Stop all logging.
        /// </summary>
        public static void StopLogging()
        {
            Trace.WriteLine("Logging stopped");
            _traceLogger.SetFileStream(null);
            if (_file != null)
            {
                _file.Close();
                _file = null;
                _isLogging = false;
            }
        }

        /// <summary>
        /// Log an Exception. Will be prefaced with 'Exception: '
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            lock (_lockObject)
            {
                if (_isLogging)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                    _file.WriteLine("Exception: " + ex);
                    _file.Flush();
                }
            }
        }

        /// <summary>
        /// Log an Exception. Will be prefaced with 'Unhandled Exception: '
        /// </summary>
        /// <param name="ex"></param>
        public static void LogUnhandledException(object ex)
        {
            lock (_lockObject)
            {
                if (_isLogging)
                {
                    _file.WriteLine("Unhandled Exception: " + ex);
                    _file.Flush();
                }
            }
        }
    }
}
