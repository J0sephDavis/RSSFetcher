using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Murong_Xue.Logging
{
    internal class LogOutput
    {
        private readonly List<LogMsg> Buffer = []; // lock needed
        private List<LogMsg> buffer_copy = []; // no lock needed
        private readonly object BufferLock = new();
        //---
        private readonly Thread BufferThread;
        private readonly AutoResetEvent BufferEvent = new(false);
        //
        private bool StopThread = false;
        private StreamWriter? FileStream;
        private Uri? FilePath;
        // When InteractiveMode is enabled, only show the short strings in the viewport & decrease either buffer_timeout or threshold
        // OR whenever a msg if flagged interactive & added to the queue immediately end the wait
        public bool InteractiveMode { get; set; } = true;
        //
        const int BUFFER_THRESHOLD = 10;
        const int BUFFER_TIMEOUT = 5000;
        const int INTERACTIVE_TIMEOUT = 250;
        //
        public LogOutput()
        {
            BufferThread = new(Main);
        }
        public bool SetPath(Uri path)
        {
            if (FilePath != null)
            {
                Log(new(LogType.ERROR, LogMod.NORMAL, "LOGPUT", "SetPath FAILED, FilePath != null"));
                return false;
            }
            if (path?.IsFile != true)
            {
                Log(new(LogType.ERROR, LogMod.NORMAL, "LOGPUT", "SetPath FAILED, path does not lead to file"));
                return false;
            }
            Log(new(LogType.DEBUG, LogMod.SPAM | LogMod.VERBOSE, "LOGPUT", $"SetPath: [{path}]")); //traceval
            
            if (File.Exists(path.LocalPath))
            {
                Log(new(LogType.ERROR, LogMod.SPAM | LogMod.UNIMPORTANT, "LOGPUT", "Deleting previous logfile")); //warnspam
                File.Delete(path.LocalPath);
            }
            FilePath = path;
            FileStream = new(FilePath.LocalPath);
            return true;
        }
        public void Start()
        {
            BufferThread.Start();
        }
        public void Stop()
        {
            if (BufferThread.IsAlive)
            {
                StopThread = true;
                BufferEvent.Set();
                BufferThread.Join();
            }
            if (FileStream != null) //not very robust I assume
            { 
                FileStream.Dispose();
                FileStream = null;
            }
        }
        public void Log(LogMsg msg)
        {
            lock (BufferLock)
            {
                Buffer.Add(msg);
                if (Buffer.Count > BUFFER_THRESHOLD || (InteractiveMode && (msg & LogMod.INTERACTIVE) != 0))
                    BufferEvent.Set();
            }
        }
        private void WriteMsg()
        {
            lock (BufferLock)
            {
                buffer_copy = new(Buffer);
                Buffer.Clear();
            }
            if (buffer_copy.Count == 0) return;

            foreach (LogMsg msg in buffer_copy)
            {
                FileStream?.WriteLine(msg);
                Console.WriteLine(msg);
            }
        }
        private void Main()
        {
            int timeout_millis = BUFFER_TIMEOUT;
            bool current_mode = !InteractiveMode; //forces an update on the first cycle
            while (StopThread == false)
            {
                //if the interactive mode has changed
                if (current_mode != InteractiveMode)
                    timeout_millis = InteractiveMode ? INTERACTIVE_TIMEOUT : BUFFER_TIMEOUT;

                BufferEvent.WaitOne(timeout_millis);
                WriteMsg();
            }
            WriteMsg(); //stragglers
        }
    }
    /// <summary>
    /// Responsible for
    /// 1. Creating reporters (factory)
    /// 2. Propagating information to reporters
    ///     - Such as a new logging level
    ///     - If we ever use individual buffers for each reporter, it would be possible to flush all the reporters buffers here
    /// </summary>
    internal class ReporterManager(LogLevel _level)
    {
        LogLevel Level = _level;
        private readonly List<Reporter> reporters = [];
        public Reporter GetReporter(string name,
            LogType type = LogType.NONE,
            LogMod mod = LogMod.NONE)
        {
            LogLevel _level = Level | mod | type;
            lock (reporters)
            //https://stackoverflow.com/questions/266681/should-a-return-statement-be-inside-or-outside-a-lock
            {
                Reporter _r = new(_level, name);
                Subscribe(_r);
                return _r;
            }
        }
        private void Subscribe(Reporter reporter)
        {
            lock (reporters)
            {
                reporters.Add(reporter);
            }
        }
        private void NotifySubscribers()
        {
            lock (reporters)
            {
                foreach(var r in reporters)
                {
                    r.SetLogLevel(Level);
                }
            }
        }

        public void SetLogLevel(LogLevel _level)
        {
            Level = _level;
        }
        public void MaskLogLevel(LogLevel _level)
        {
            Level |= _level;
        }
    }
    internal class Logger
    {
        private static Logger? s_Logger = null;
        private readonly LogOutput LogPut;
        private readonly ReporterManager RepManager;
        public static Logger GetInstance()
        {
            s_Logger ??= new Logger();
            return s_Logger;
        }
        private Logger()
        {
            RepManager = new(new(LogType.DEFAULT, LogMod.DEFAULT));
            LogPut = new();
            LogPut.Start();
        }
        public void Dispose()
        {
            LogPut.Stop();
        }
        //------------------------------------
        public void SetPath(Uri path)
        {
            LogPut.SetPath(path);
        }
        //------------------------------------
        /// <summary>
        /// Add message to the log
        /// </summary>
        /// <param name="msg">The message</param>
        public static void Log(LogMsg msg)
        {
            GetInstance().LogPut.Log(msg);
        }
        public static Reporter RequestReporter(string ModuleName, LogType type = LogType.NONE, LogMod mod = LogMod.NONE)
        {
            return GetInstance().RepManager.GetReporter(ModuleName, type, mod);
        }
        public static void SetInteractiveMode(bool mode)
        {
            GetInstance().LogPut.InteractiveMode = mode;
        }
        public static void SetLogLevel(LogLevel _level)
        {
            GetInstance().RepManager.SetLogLevel(_level);
        }
        public static void MaskLogLevel(LogLevel _level)
        {
            GetInstance().RepManager.MaskLogLevel(_level);
        }
    }
}
