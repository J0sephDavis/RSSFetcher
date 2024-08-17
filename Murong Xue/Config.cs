﻿namespace Murong_Xue
{
    internal class Config : IDisposable
    {
        private string DownloadDirectory;
        private Uri RSSConfigPath;
        private static LogType logType  = LogType.DEFAULT;
        private static LogMod logMod    = LogMod.DEFAULT;
        //---
        private static Config? s_Config = null;
        private static readonly Reporter report = new(logType,logMod, "CONFIG");
        private readonly Logger _Logger;
        private static readonly List<Reporter> Reporters = [ report ];

        private Config()
        {
            string rootDir = Path.GetDirectoryName(System.AppContext.BaseDirectory) + Path.DirectorySeparatorChar;
            DownloadDirectory = rootDir + @"Downloads" + Path.DirectorySeparatorChar;
            RSSConfigPath = new Uri(rootDir + "rss-config.xml");
            _Logger = Logger.GetInstance(rootDir + @"MurongLie.log");
            report.Trace($"Config(), rootDir: {rootDir}, cfgPath:{RSSConfigPath}, downloadDir:{DownloadDirectory}");
            Subscribe(report);
        }
        public static Config GetInstance()
        {
            s_Config ??= new Config();
            return s_Config;
        }
        //--------
        public static Reporter OneReporterPlease(string ModuleName, LogType type = LogType.NONE, LogMod mod = LogMod.NONE)
        {
            if (type == LogType.NONE)
                type = logType;
            if (mod == LogMod.NONE)
                mod = logMod;

            Reporter _r = new(type, mod, ModuleName);
            report.Trace($"Reporter {report.ReportIdentifier} created");
            Subscribe(_r);
            return _r;
        }
        private static void Subscribe(Reporter reporter)
        {
            lock (Reporters)
            {
                report.Trace($"Reporter {report.ReportIdentifier} subscribed");
                Reporters.Add(reporter);
            }
        }
        protected static void NotifySubscribers()
        {
            report.Trace("Propagate loglevel to subscribers");
            lock (Reporters)
            {
                foreach (Reporter r in Reporters)
                {
                    r.SetLogLevel(logType, logMod);
                }
            }
        }
        //----------
        public static void SetLogLevel(LogType type, LogMod mod)
        {
            logType = type;
            logMod = mod;
            report.DebugVal($"New LogLevel: {type} | {mod}");
            NotifySubscribers();
        }
        public void SetRSSPath(string path)
        {
            RSSConfigPath = new Uri(path);
            report.DebugVal($"RSS Path changed: {RSSConfigPath}");
        }
        public Uri GetRSSPath()
        {
            return RSSConfigPath;
        }
        public void SetDownloadPath(string path)
        {
            DownloadDirectory = path;
            report.DebugVal($"Download Path changed: {DownloadDirectory}");
        }
        public string GetDownloadPath()
        {
            return DownloadDirectory;
        }
        //----------
        public void Dispose()
        {
            Reporters.Clear();
            s_Config = null;
            _Logger.Quit();
        }
    }
}
