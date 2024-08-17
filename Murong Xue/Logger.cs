using System.Reflection.Emit;

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

        ALL = SPAM | VERBOSE | UNIMPORTANT | NORMAL,
        _VS = VERBOSE | SPAM,
    }
    [Flags]
    enum LogType
    {
        NONE = 0,
        NORMAL = 1 << 0,
        DEBUG = 1 << 1,
        ERROR = 1 << 2,
        OUTPUT = 1 << 3,

        ALL = DEBUG | ERROR | OUTPUT,
    }
    [Flags]
    enum LogFlag
    {
        WARN            = LogType.ERROR | LogMod.UNIMPORTANT,
        CHATTER         = LogType.OUTPUT | LogMod.UNIMPORTANT,
        DEBUG_S         = LogType.DEBUG | LogMod.SPAM,
        DEBUG_SV        = LogType.DEBUG | LogMod._VS,
        DEBUG_V         = LogType.DEBUG | LogMod.VERBOSE,
#if DEBUG
        _DEFAULT = LogType.OUTPUT | LogType.ERROR | LogMod.NORMAL,
        DEFAULT = _DEFAULT | LogType.DEBUG,
#else
        DEFAULT = LogType.OUTPUT | LogType.ERROR | LogMod.NORMAL,
#endif
        ALL = LogMod.ALL | LogType.ALL,
    };
    //REPORTS to the logger when anything happens
    /// <summary>
    /// Reporter constructor, reporters handle interfacing with the logger class and scheduling messages.
    /// Every class should have its own reporter
    /// </summary>
    /// <param name="logFlags">The mask/filter applied to all incoming logs (will be overriden if a new log level is set in the Config class)</param>
    /// <param name="identifier">This reporters identity, e.g.,"Program" & "DownloadHandler"</param>
    internal class Reporter(LogFlag logFlags, string identifier)
    {
        private LogMod LogModifiers = (LogMod)logFlags & LogMod.ALL;
        private LogType LogTypes = (LogType)logFlags & LogType.ALL; 
        public readonly string ReportIdentifier = "[" + identifier + "]"; // e.g. Program, FeedData, Config, &c

        /// <summary>
        /// Add a message to the log
        /// </summary>
        /// <param name="level">The level/flags applied to the message</param>
        /// <param name="msg">The contents of the message</param>
        public void Log(LogFlag level, string msg)
        {
        }
        public void Log(LogType type, LogMod mod, string msg)
        {
            var _modifier = mod & LogModifiers;
            var _type = type & LogTypes;
            //= logModifier when
            if ((_modifier > 0 || _modifier == LogModifiers) && _type > 0)
            {
                LogMsg _msg = new((LogFlag)type | (LogFlag)mod, ReportIdentifier, msg);
                Task.Run(() => Logger.Log(_msg));
            }
        }
        //----
        /// <summary>
        /// Set the log filter for the reporter
        /// </summary>
        /// <param name="flag">the new report filter</param>
        public void SetLogLevel(LogFlag flag)
        {
            Log(LogFlag.DEBUG_SV, $"Loglevel Set {flag}");
            this.LogModifiers = (LogMod)flag & LogMod.ALL;
            this.LogTypes = (LogType)flag & LogType.ALL;
        }
    }
    /// <summary>
    /// Class representing a message to be logged
    /// </summary>
    /// <param name="severity">The flags that define this msg</param>
    /// <param name="identifier">The reporter who is sending this msg</param>
    /// <param name="content">The content of the msg</param>
    internal class LogMsg
    {
        public LogFlag SEVERITY;
        public string IDENTIFIER;
        public string CONTENT;
        public string TIMESTAMP = DateTime.Now.ToLongTimeString();
        public LogMsg(LogFlag severity, string identifier, string content)
        {
            SEVERITY = severity;
            IDENTIFIER = identifier;
            CONTENT = content;
        }
        public LogMsg(LogType type, LogMod mod, string identifier, string content)
        {
            SEVERITY = (LogFlag)type | (LogFlag)mod;//for now.. make dedicated vars for em;
            IDENTIFIER = identifier;
            CONTENT = content;
        }
        public LogMsg(LogType type, string identifier, string content)
        {
            LogFlag mod = (LogFlag)LogMod.NORMAL;
            SEVERITY = (LogFlag)type | mod;
            IDENTIFIER = identifier;
            CONTENT = content;
        }
        //TODO Prettify output here, someway 
        public override string ToString()
        {
            return $"[{TIMESTAMP}]{IDENTIFIER}\t({SEVERITY})\t{CONTENT}";
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
                LogFlag flag = (LogFlag)LogType.OUTPUT | (LogFlag)LogMod._VS;
                Log(new LogMsg(flag, "Logger", $"Log file: [{filePath}]"));
                if (File.Exists(path.LocalPath))
                {
                    Log(new LogMsg(((LogFlag)LogType.OUTPUT | (LogFlag)LogMod.UNIMPORTANT), "Logger", $"Deleting previous {Path.GetFileName(path.LocalPath)}"));
                    File.Delete(path.LocalPath);
                }
                batchThread = new(BatchLoopFile);
            }
            else
            {
                Log(new LogMsg(LogType.ERROR, "Logger", $"Path does not lead to a file:\n\t[{(path == null ? "null" : path.LocalPath)}]"));
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
                    //TODO: write to file
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
