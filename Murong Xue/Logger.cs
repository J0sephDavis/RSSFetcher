using System.Runtime.CompilerServices;

namespace Murong_Xue
{
    [Flags]
    enum LogMod
    {
        NONE = 0,
        NORMAL = 1 << 0,
        SPAM = 1 << 1,
        VERBOSE = 1 << 2,
        UNIMPORTANT = 1 << 3,
        INTERACTIVE = 1 << 4,

        ALL = SPAM | VERBOSE | UNIMPORTANT | NORMAL | INTERACTIVE,
#if DEBUG
        DEFAULT = NORMAL | UNIMPORTANT,
#else
        DEFAULT = NORMAL,
#endif
    }
    [Flags]
    enum LogType
    {
        NONE = 0,
        DEBUG = 1 << 0,
        ERROR = 1 << 1,
        OUTPUT = 1 << 2,

        ALL = DEBUG | ERROR | OUTPUT,
#if DEBUG
        _DEFAULT = OUTPUT | ERROR,
        DEFAULT = _DEFAULT | DEBUG
#else
        DEFAULT = OUTPUT | ERROR
#endif
    }
    class LogLevel(LogType type, LogMod mod)
    {
        protected LogMod _modifier = mod;
        protected LogType _type = type;
        //------ getters/setters
        public LogMod GetLMod()
        {
            return _modifier;
        }
        public LogType GetLType()
        {
            return _type;
        }
        //------ string formatting
        protected static string Mod_ToString(LogMod mod)
        {
            string test = string.Empty;
            test += (mod & LogMod.NORMAL) > 0       ? "N" : "_";
            test += (mod & LogMod.SPAM) > 0         ? "S" : "_";
            test += (mod & LogMod.VERBOSE) > 0      ? "V" : "_";
            test += (mod & LogMod.UNIMPORTANT) > 0  ? "U" : "_";
            test += (mod & LogMod.INTERACTIVE) > 0  ? "I" : "_";
            return test;
        }
        override public string ToString()
        {
            return $"[{_type}({Mod_ToString(_modifier)})]";
        }
        //------ Set
        public void Set(LogMod mod)
        {
            _modifier = mod;
        }
        public void Set(LogType type)
        {
            _type = type;
        }
        public void  Set(LogLevel level)
        {
            _type = level._type;
            _modifier = level._modifier;
        }
        //----- Mask
        public void Mask(LogMod mod)
        {
            _modifier |= mod;
        }
        public void Mask(LogType type)
        {
            _type |= type;
        }
        public void Mask(LogLevel level)
        {
            _type |= level._type;
            _modifier |= level._modifier;
        }
        //---- bitewise AND
        public static LogMod operator &(LogLevel a, LogMod b)
        {
            return a._modifier & b;
        }
        public static LogType operator &(LogLevel a, LogType b)
        {
            return a._type & b;
        }
        //---- bitewise OR
        public static LogLevel operator |(LogLevel a, LogLevel b)
        {
            return new LogLevel( //should `new` be used here?
                a._type | b._type,
                a._modifier | b._modifier);
        }
        public static LogLevel operator |(LogLevel a, LogMod b)
        {
            //TODO, reevaluate this, seems improper
            LogLevel tmp = new(a._type, a._modifier);
            tmp.Mask(b);
            return tmp;
        }
        public static LogLevel operator |(LogLevel a, LogType b)
        {
            LogLevel tmp = new LogLevel(a._type, a._modifier);
            tmp.Mask(b);
            return tmp;
        }
        //---- comparisons
        // We double up on these because implicit conversions
        // from LogMsg to LogLevel will be made
        // if there isn't a matching signature:
        // 1. msg == level
        public static bool operator ==(LogLevel a, LogMsg m)
        {
            return ((a._modifier & m._modifier) == m._modifier)
                && ((a._type & m._type) == m._type);
        }
        public static bool operator !=(LogLevel a, LogMsg m)
        {
            
            return ((a._modifier & m._modifier) != m._modifier)
                && ((a._type & m._type) != m._type);
        }
        // 2. message == level
        public static bool operator ==(LogMsg m, LogLevel a)
        {
            return ((a._modifier & m._modifier) == m._modifier)
                && ((a._type & m._type) == m._type);
        }
        public static bool operator !=(LogMsg m, LogLevel a)
        {
            return ((a._modifier & m._modifier) != m._modifier)
                && ((a._type & m._type) != m._type);
        }
        //---- comparisons to base types
        public static bool operator ==(LogLevel a, LogMod m)
        {
            return a._modifier == m;
        }
        public static bool operator !=(LogLevel a, LogMod m)
        {
            return a._modifier != m;
        }
        public static bool operator ==(LogLevel a, LogType t)
        {
            return a._type == t;
        }
        public static bool operator !=(LogLevel a, LogType t)
        {
            return a._type != t;
        }
    }
    //REPORTS to the logger when anything happens
    /// <summary>
    /// Reporter constructor, reporters handle interfacing with the logger class and scheduling messages.
    /// Every class should have its own reporter
    /// </summary>
    /// <param name="logFlags">The mask/filter applied to all incoming logs (will be overriden if a new log level is set in the Config class)</param>
    /// <param name="identifier">This reporters identity, e.g.,"Program" & "DownloadHandler"</param>
    internal class Reporter(LogLevel level, string identifier)
    {
        private readonly LogLevel logLevel = level;
        public readonly string ReportIdentifier = "[" + identifier + "]"; // e.g. Program, FeedData, Config, &c
        // <--- DEBUG --->
        public void Debug(string msg)
        {
            Log(LogType.DEBUG, LogMod.NORMAL, msg);
        }
        public void DebugVal(string msg)
        {
            Log(LogType.DEBUG, LogMod.VERBOSE, msg);
        }
        public void Trace(string msg)
        {
            Log(LogType.DEBUG, LogMod.SPAM | LogMod.VERBOSE, msg);
        }
        public void TraceVal(string msg)
        {
            Log(LogType.DEBUG, LogMod.SPAM | LogMod.VERBOSE, msg);
        }
        // <--- ERROR --->
        public void Error(string msg)
        {
            Log(LogType.ERROR, LogMod.NORMAL, msg);
        }
        public void Warn(string msg)
        {
            Log(LogType.ERROR, LogMod.UNIMPORTANT, msg);
        }
        public void WarnSpam(string msg)
        {
            Log(LogType.ERROR, LogMod.UNIMPORTANT | LogMod.SPAM, msg);
        }
        // <--- OUTPUT --->
        public void Spam(string msg)
        {
            Log(LogType.OUTPUT, LogMod.SPAM, msg);
        }
        //! Events that are bound to happen and output might be wanted. e.g., Feed downloaded
        public void Notice(string msg)
        {
            Log(LogType.OUTPUT, LogMod.UNIMPORTANT, msg); //Verbose?
        }
        public void Out(string msg)
        {
            Log(LogType.OUTPUT, LogMod.NORMAL, msg);
        }
        public void Log(LogType type, LogMod mod, string msg)
        {
            LogMsg log = new(type, mod, ReportIdentifier, msg);
            //mod == _modifier when _modifier = LogMod.NONE
            if (log == logLevel)
                SendLog(log);
        }
        private static void SendLog(LogMsg msg)
        {
            Task.Run(() => Logger.Log(msg));
        }
        //----
        /// <summary>
        /// Set log level
        /// </summary>
        /// <param name="type">ignored if NONE</param>
        /// <param name="mod">ignored if NONE</param>
        public void SetLogLevel(LogType type, LogMod mod)
        {
            TraceVal($"Set logging flags {_level}");
            if (_level != LogMod.NONE)
                logLevel.Set(_level.GetLMod());
            if (_level != LogType.NONE)
                logLevel.Set(_level.GetLType());
            TraceVal($"Current flags {logLevel}");
        }
        public void MaskLogLevel(LogType type, LogMod mod)
        {
            TraceVal($"Mask logging flags {type}, {mod}");
            logLevel.Mask(mod);
            logLevel.Mask(type);
            TraceVal($"Current flags {logLevel}");
        }
    }
    /// <summary>
    /// Class representing a message to be logged
    /// </summary>
    /// <param name="severity">The flags that define this msg</param>
    /// <param name="identifier">The reporter who is sending this msg</param>
    /// <param name="content">The content of the msg</param>
    internal class LogMsg(LogType type, LogMod mod, string identifier, string content) : LogLevel(type, mod)
    {
        public string IDENTIFIER = identifier;
        public string CONTENT = content;
        public string TIMESTAMP = DateTime.Now.ToLongTimeString();
        
        //TODO Prettify output here, someway 
        public override string ToString()
        {
            return $"[{TIMESTAMP}] {IDENTIFIER}\t{base.ToString()}\t{CONTENT}";
        }
    }
    internal class Logger
    {
        /* After several iterations and changes on this class, it can only be considered sloppy.
         * Should it be a static class?
         * Should it be a singleton?
         * How to ensure this is cleaned up on exit?
         *  -> Included in the Config.Dipose() (because Garbage collection may not/wont happen)
         */
        private static readonly List<LogMsg> bufferedMsgs = [];
        private static readonly Object buffLock = new();
        private static Logger? s_Logger = null;
        private static Reporter report = Config.OneReporterPlease("LOGGER");
        //----
        private readonly Thread batchThread;
        private static readonly AutoResetEvent batchEvent = new(false);
        private bool StopLoop = false;
        //----
        const int BUFFER_THRESHOLD = 10; // msgs
        const int BUFFER_TIMEOUT = 5; // seconds
        //----
        private Uri? filePath;
        public static Logger GetInstance(string? path = null)
        {
            s_Logger ??= new Logger(path == null ? null : new Uri(path));
            return s_Logger;
        }
        private Logger(Uri? path = null)
        {
            filePath = path;
            if (path?.IsFile == true)
            {
                report.TraceVal($"Log file: [{filePath}]");
                if (File.Exists(path.LocalPath))
                {
                    report.WarnSpam($"Deleting previous {Path.GetFileName(path.LocalPath)}");
                    File.Delete(path.LocalPath);
                }
                batchThread = new(BatchLoopFile);
            }
            else
            {
                Log(new LogMsg(LogType.ERROR, LogMod.NORMAL, "Logger", $"Path does not lead to a file:\n\t[{(path == null ? "null" : path.LocalPath)}]"));
                batchThread = new(BatchLoop);
            }
            batchThread.Start();
        }
        ~Logger() { Quit(); } //Probably won't be called because its the last thing to be done. GC won't take place
        public void Quit()
        {
            if (batchThread.IsAlive)
            {
                StopLoop = true;
                batchEvent.Set();
                batchThread.Join();
            }
        }
        //------------------------------------
        /// <summary>
        /// Add message to the log
        /// </summary>
        /// <param name="msg">The message</param>
        public static void Log(LogMsg msg)
        {
            lock (buffLock)
            {
                bufferedMsgs.Add(msg);
                if (bufferedMsgs.Count > BUFFER_THRESHOLD)
                    batchEvent.Set();
            }
        }
        /// <summary>
        /// The loggers main thread. Waits for the batchEvent flag to be set or BUFFER_TIMEOUT.
        /// Clears & creates a copy of the buffer and writes output to the console (WIP for files)
        /// </summary>
        private void BatchLoop() //Console only
        {
            List<LogMsg> copiedBuff = [];
            while (StopLoop == false)
            {
                batchEvent.WaitOne(TimeSpan.FromSeconds(BUFFER_TIMEOUT));
                //---
                lock (buffLock)
                {
                    copiedBuff = new(bufferedMsgs);
                    bufferedMsgs.Clear();
                }
                if (copiedBuff != null)
                {
                    foreach (LogMsg msg in copiedBuff)
                        Console.WriteLine(msg);
                    copiedBuff.Clear();
                }
            }
            //--- making sure the buffer is clear on exit
            lock (buffLock)
            {
                foreach (LogMsg msg in bufferedMsgs)
                    Console.WriteLine(msg);
                bufferedMsgs.Clear();
            }
        }
        private void BatchLoopFile() //file & console
        {
            using (StreamWriter file = new StreamWriter(filePath.LocalPath)) //when this is null, what happens?
            {
                List<LogMsg> copiedBuff = [];
                while (StopLoop == false)
                {
                    batchEvent.WaitOne(TimeSpan.FromSeconds(BUFFER_TIMEOUT));
                    //---
                    lock (buffLock)
                    {
                        copiedBuff = new(bufferedMsgs);
                        bufferedMsgs.Clear();
                    }
                    if (copiedBuff != null)
                    {
                        foreach (LogMsg msg in copiedBuff)
                        {
                            Console.WriteLine(msg);
                            file.WriteLine(msg);
                        }
                        copiedBuff.Clear();
                    }
                }
                //--- making sure the buffer is clear on exit
                lock (buffLock)
                {
                    foreach (LogMsg msg in bufferedMsgs)
                    {
                        Console.WriteLine(msg);
                        file.WriteLine(msg);
                    }
                    bufferedMsgs.Clear();
                }
            }
        }
    }
}
