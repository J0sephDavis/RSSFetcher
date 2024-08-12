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
        DEFAULT = _INFO | _EXCEPTION,
        ALL = NONE 
            | _DEBUG        //| DEBUG | DEBUG_SPAM
            | _EXCEPTION    //| ERROR | WARN
            | _INFO         //| NOTEWORTHY | FEEDBACK,
            | SPAM,
    };
    //REPORTS to the logger when anything happens
    internal class Reporter
    {
        private Logger _Logger = Logger.GetInstance();
        private LogFlag ReportFilter;
        private string ReportIdentifier; // e.g. Program, FeedData, Config, &c
        public Reporter(LogFlag logFlags, string identifier)
        {
            ReportFilter = logFlags;
            ReportIdentifier = "[" + identifier + "]";
        }
        public void Log(LogFlag level, string msg)
        {
            if ((level & ReportFilter) != LogFlag.NONE)
                _Logger.Log($"{ReportIdentifier} [{level}] {msg}");
        }

    }
    //For now, just handles what is print to the console, no special stuff yet
    //TODO log buffer
    //TODO async
    internal class Logger
    {
        private static Logger s_Logger;
        private Logger() { }
        public static Logger GetInstance()
        {
            if (s_Logger == null)
                s_Logger = new();
            return s_Logger;
        }
        //------------------------------------
        public void Log(string msg)
        {
            Console.WriteLine(msg.ToString());
        }
    }
}
