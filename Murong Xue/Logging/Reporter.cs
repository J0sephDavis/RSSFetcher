using System.Diagnostics;

namespace Murong_Xue.Logging
{
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
        [Conditional("DEBUG")]
        public void Debug(string msg)
        {
            Log(LogType.DEBUG, LogMod.NORMAL, msg);
        }
        [Conditional("DEBUG")]
        public void DebugVal(string msg)
        {
            Log(LogType.DEBUG, LogMod.VERBOSE, msg);
        }
        [Conditional("DEBUG")]
        public void Trace(string msg)
        {
            Log(LogType.DEBUG, LogMod.SPAM, msg);
        }
        [Conditional("DEBUG")]
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
        public void Interactive(string msg)
        {
            Log(LogType.OUTPUT, LogMod.INTERACTIVE, msg);
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
        public void SetLogLevel(LogLevel _level)
        {
            if (_level != LogMod.NONE)
                logLevel.Set(_level.GetLMod());
            if (_level != LogType.NONE)
                logLevel.Set(_level.GetLType());
            TraceVal($"New flags {logLevel}");
        }
        public void MaskLogLevel(LogType type, LogMod mod)
        {
            logLevel.Mask(mod);
            logLevel.Mask(type);
            TraceVal($"New flags {logLevel}");
        }
    }
}
