namespace Murong_Xue
{
    [Flags]
    enum LogFlag
    {
        //--- TRUE
        NONE        = 0,
        DEBUG_SPAM  = 1 << 0,
        DEBUG       = 1 << 1,
        SPAM        = 1 << 2,
        WARN        = 1 << 3,
        NOTEWORTHY  = 1 << 4,
        FEEDBACK    = 1 << 5,
        ERROR       = 1 << 6,
        //---
        _EXCEPTION = ERROR | WARN,
        _DEBUG = DEBUG | DEBUG_SPAM,
        _INFO = FEEDBACK | NOTEWORTHY,
        //---
#if DEBUG
        DEFAULT = _INFO | _EXCEPTION | DEBUG,
#else
        DEFAULT = _INFO | _EXCEPTION,
#endif
        HEADLESS = FEEDBACK | ERROR | WARN,
        ALL = _DEBUG        //| DEBUG | DEBUG_SPAM
            | _EXCEPTION    //| ERROR | WARN
            | _INFO         //| NOTEWORTHY | FEEDBACK,
            | SPAM,
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
        private LogFlag ReportFilter = logFlags;
        public readonly string ReportIdentifier = "[" + identifier + "]"; // e.g. Program, FeedData, Config, &c

        /// <summary>
        /// Add a message to the log
        /// </summary>
        /// <param name="level">The level/flags applied to the message</param>
        /// <param name="msg">The contents of the message</param>
        public void Log(LogFlag level, string msg)
        {
            LogMsg _msg = new(level, ReportIdentifier, msg);
            if ((level & ReportFilter) != LogFlag.NONE)
                Task.Run(() => Logger.Log(_msg));
        }
        //----
        /// <summary>
        /// Set the log filter for the reporter
        /// </summary>
        /// <param name="flag">the new report filter</param>
        public void SetLogLevel(LogFlag flag)
        {
            Log(LogFlag.DEBUG, "Loglevel Set");
            this.ReportFilter = flag;
        }
    }
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
                Log(new LogMsg(LogFlag.NOTEWORTHY, "Logger", $"Path to logfile: [{filePath}]"));
                if (File.Exists(path.LocalPath))
                {
                    Log(new LogMsg(LogFlag.WARN, "Logger", $"!!! File already exists, deleting: [{path}]"));
                    File.Delete(path.LocalPath);
                }
                batchThread = new(BatchLoopFile);
            }
            else
            {
                Log(new LogMsg(LogFlag.ERROR, "Logger", $"!!! Path is null / not a file: [{path}]"));
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
