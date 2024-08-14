using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Reporter(LogFlag logFlags, string identifier)
        {
            ReportFilter = logFlags;
            ReportIdentifier = "[" + identifier + "]";
        }
        public void Log(LogFlag level, string msg)
        {
            LogMsg _msg = new(level, ReportIdentifier, msg);
            if ((level & ReportFilter) != LogFlag.NONE)
                Task.Run(()=>Logger.Log(_msg));
        }
        //----
        public void SetLogLevel(LogFlag flag)
        {
            Log(LogFlag.DEBUG, "Loglevel Set");
            this.ReportFilter = flag;
        }
    }
    internal sealed class LogMsg(LogFlag severity, string identifier,string content)
    {
        public LogFlag SEVERITY => severity;
        public string IDENTIFIER => identifier;
        public string CONTENT => content;
        public override string ToString()
        {
            return $"!{identifier} [{severity}] {content}";
        }
    }
    //For now, just handles what is print to the console, no special stuff yet
    internal static class Logger
    {
        private static readonly List<LogMsg> bufferedMsgs = [];
        private static readonly Object buffLock = new();
        const int BUFFER_THRESHOLD = 5;
        public static void Quit()
        {
            lock (buffLock)
            {
                if (bufferedMsgs.Count > BUFFER_THRESHOLD)
                {
                    ProcessBatch(bufferedMsgs);
                    bufferedMsgs.Clear();
                }
            }
        }
        //------------------------------------
        public static void Log(LogMsg msg)
        {
            List<LogMsg>? copiedBuff = null;
            lock(buffLock)
            {
                bufferedMsgs.Add(msg);
                if (bufferedMsgs.Count > BUFFER_THRESHOLD)
                {
                    copiedBuff = new(bufferedMsgs);
                    bufferedMsgs.Clear();
                }
            }
            if (copiedBuff != null)
                Task.Run(() => ProcessBatch(copiedBuff));
        }
        //Process a batch of logs (SAVE & PRINT)
        private static void ProcessBatch(List<LogMsg> msgs)
        {
            foreach (LogMsg msg in msgs)
                Console.WriteLine(msg);
        }
    }
}
