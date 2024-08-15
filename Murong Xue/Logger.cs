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
    internal class Reporter
    {
        private LogFlag ReportFilter;
        public readonly string ReportIdentifier; // e.g. Program, FeedData, Config, &c
        /// <summary>
        /// Reporter constructor, reporters handle interfacing with the logger class and scheduling messages.
        /// Every class should have its own reporter
        /// </summary>
        /// <param name="logFlags">The mask/filter applied to all incoming logs (will be overriden if a new log level is set in the Config class)</param>
        /// <param name="identifier">This reporters identity, e.g.,"Program" & "DownloadHandler"</param>
        public Reporter(LogFlag logFlags, string identifier)
        {
            ReportFilter = logFlags;
            ReportIdentifier = "[" + identifier + "]";
        }
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
    internal static class Logger
    {
        private static readonly List<LogMsg> bufferedMsgs = [];
        private static readonly Object buffLock = new();
        private static readonly AutoResetEvent batchEvent = new(false);
        private static Thread batchThread = new(BatchLoop);
        private static bool StopLoop = false;
        const int BUFFER_THRESHOLD = 10; // msgs
        const int BUFFER_TIMEOUT = 5; // seconds
        /// <summary>
        /// MUST be called on exit to flush the buffer and stop the batch thread from running
        /// </summary>
        public static void Quit()
        {
            StopLoop = true;
            batchEvent.Set();
            batchThread.Join();
        }
        /// <summary>
        /// Must be called on start to begin the batch thread which will clear the log buffer
        /// </summary>
        public static void Start()
        {
            batchThread.Start();
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
        private static void BatchLoop()
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
    }
}
