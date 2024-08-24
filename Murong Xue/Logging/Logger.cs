using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Murong_Xue.Logging.OutputHandling;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue.Logging
{
    internal class Logger
    {
        private static Logger? s_Logger = null;
        private readonly LogOutputManager LogPut;
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
