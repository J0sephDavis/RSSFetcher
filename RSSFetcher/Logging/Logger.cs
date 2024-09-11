using RSSFetcher.Logging.Output.Modules;
using RSSFetcher.Logging.OutputHandling;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher.Logging
{
    public class Logger : IDisposable
    {
        private readonly LogOutputManager LogPut;
        private readonly ReporterManager RepManager;
        // ---
        private static Logger? s_Logger = null;
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
        //-----MODULES------------------------
        public void AddModule(IOutputModule mod)
        {
            LogPut.AddModule(mod);
        }
        public void LogToFile(Uri path)
        {
            LogPut.SetPath(path);
        }
        public IOutputConsole? GetConsole()
        {
            return LogPut.GetConsole();
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
        public static InteractiveReporter RequestInteractive(string ModuleName, LogType type = LogType.NONE, LogMod mod = LogMod.NONE)
        {
            return GetInstance().RepManager.GetInteractiveReporter(ModuleName, type, mod);
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
