namespace Murong_Xue
{
    [Flags]
    enum LogFlag
    {
        //---
        NONE = 0,
        NORMAL = 0,
        //--- Modifiers
        SPAM            = 1 << 0,
        VERBOSE         = 1 << 1,
        UNIMPORTANT     = 1 << 2,
        INTERACTIVE     = 1 << 3,
        
        _ALL_MODS = SPAM | VERBOSE | UNIMPORTANT | NORMAL,
        //--- Types
        DEBUG           = 1 << 4,
        ERROR           = 1 << 5,
        OUTPUT          = 1 << 6,

        _ALL_TYPES = DEBUG | ERROR | OUTPUT,
        //derived base
        _SETVAL = VERBOSE | SPAM,
        //Derived debug
        DEBUG_SPAM      = DEBUG | SPAM,
        DEBUG_WARN      = DEBUG | WARN,
        DEBUG_OBLIG     = DEBUG | UNIMPORTANT,
        DEBUG_SETVAL    = DEBUG | _SETVAL,
        //Derived error
        WARN            = ERROR | UNIMPORTANT,
        //Derived output
        CHATTER         = OUTPUT | UNIMPORTANT,

        //---
        _DEFAULT_MODS   = NORMAL,
#if DEBUG
        _DEFAULT = OUTPUT | ERROR | _DEFAULT_MODS,
        DEFAULT = _DEFAULT | DEBUG,
#else
        DEFAULT = OUTPUT | ERROR | _DEFAULT_MODS,
#endif
        ALL = _ALL_MODS | _ALL_TYPES,
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
        private LogFlag LogModifiers = logFlags & LogFlag._ALL_MODS;
        private LogFlag LogTypes = logFlags & LogFlag._ALL_TYPES; 
        public readonly string ReportIdentifier = "[" + identifier + "]"; // e.g. Program, FeedData, Config, &c

        /// <summary>
        /// Add a message to the log
        /// </summary>
        /// <param name="level">The level/flags applied to the message</param>
        /// <param name="msg">The contents of the message</param>
        public void Log(LogFlag level, string msg)
        {
            var modifier = level & LogModifiers;
            var type = level & LogTypes;
            //= logModifier when
            if ((modifier > 0 || modifier == LogModifiers) && type > 0)
            {
                LogMsg _msg = new(level, ReportIdentifier, msg);
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
            Log(LogFlag.DEBUG, $"Loglevel Set {flag}");
            this.LogModifiers = flag & LogFlag._ALL_MODS;
            this.LogTypes = flag & LogFlag._ALL_TYPES;
        }
    }
    /// <summary>
    /// Class representing a message to be logged
    /// </summary>
    /// <param name="severity">The flags that define this msg</param>
    /// <param name="identifier">The reporter who is sending this msg</param>
    /// <param name="content">The content of the msg</param>
    internal sealed class LogMsg(LogFlag severity, string identifier, string content)
    {
        public LogFlag SEVERITY => severity;
        public string IDENTIFIER => identifier;
        public string CONTENT => content;
        public string TIMESTAMP = DateTime.Now.ToLongTimeString();
        //TODO Prettify output here, someway 
        public override string ToString()
        {
            return $"[{TIMESTAMP}]{identifier}\t({severity})\t{content}";
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
                Log(new LogMsg(LogFlag.OUTPUT | LogFlag._SETVAL, "Logger", $"Log file: [{filePath}]"));
                if (File.Exists(path.LocalPath))
                {
                    Log(new LogMsg(LogFlag.OUTPUT | LogFlag.UNIMPORTANT, "Logger", $"Deleting previous {Path.GetFileName(path.LocalPath)}"));
                    File.Delete(path.LocalPath);
                }
                batchThread = new(BatchLoopFile);
            }
            else
            {
                Log(new LogMsg(LogFlag.ERROR, "Logger", $"Path does not lead to a file:\n\t[{(path == null ? "null" : path.LocalPath)}]"));
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
