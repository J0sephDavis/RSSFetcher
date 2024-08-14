namespace Murong_Xue
{
    internal class Config
    {
        private readonly static LogFlag ConfigDefaultLogLevel = LogFlag.ALL; //So that we can set default in LogLevel & static Reporter
        //---
        private string DownloadDirectory;
        private Uri RSSConfigPath;
        private static LogFlag LogLevel = ConfigDefaultLogLevel;
        private static List<Reporter>? Reporters = null;
        //---
        private static Config? s_Config = null;
        private static readonly Reporter report = new(ConfigDefaultLogLevel, "CONFIG");

        private Config()
        {

            string rootDir = Path.GetDirectoryName(System.AppContext.BaseDirectory) + @"\";
            DownloadDirectory = rootDir + @"Downloads\";
            RSSConfigPath = new Uri(rootDir + "rss-config.xml");
            report.Log(LogFlag.DEBUG, $"Config(), rootDir: {rootDir}, cfgPath:{RSSConfigPath}, downloadDir:{DownloadDirectory}");
            Reporters = [];
            Subscribe(report);
        }
        public static Config GetInstance()
        {
            s_Config ??= new Config();
            report.Log(LogFlag.DEBUG, "GetInstance");
            return s_Config;
        }
        //--------
        public static Reporter OneReporterPlease(string ModuleName, LogFlag flag = LogFlag.NONE)
        {
            if (flag == LogFlag.NONE)
                flag = LogLevel;
            Reporter _r = new(flag, ModuleName);
            Subscribe(_r);
            return _r;
        }
        private static void Subscribe(Reporter reporter)
        {
            lock (Reporters)
            {
                report.Log(LogFlag.DEBUG, $"SUBSCRIBER! {report.ReportIdentifier}");
                Reporters.Add(reporter);
            }
        }
        protected static void NotifySubscribers()
        {
            //update loglevels
            lock (Reporters)
            {
                foreach (Reporter r in Reporters)
                {
                    r.SetLogLevel(LogLevel);
                }
            }
        }
        //----------
        public static void SetLogLevel(LogFlag level)
        {
            LogLevel = level;
            report.Log(LogFlag.DEBUG_SPAM, $"New LogLevel {LogLevel}");
            NotifySubscribers();
        }
        public static LogFlag GetLogLevel()
        {
            report.Log(LogFlag.DEBUG_SPAM, $"Get LogLevel {LogLevel}");
            return LogLevel;
        }
        public void SetRSSPath(string path)
        {
            RSSConfigPath = new Uri(path);
            report.Log(LogFlag.SPAM, $"RSS Path changed: {RSSConfigPath}");
        }
        public Uri GetRSSPath()
        {
            report.Log(LogFlag.SPAM, $"Get RSSPath: {RSSConfigPath}");
            return RSSConfigPath;
        }
        public void SetDownloadPath(string path)
        {
            DownloadDirectory = path;
            report.Log(LogFlag.SPAM, $"Download Path changed: {DownloadDirectory}");
        }
        public string GetDownloadPath()
        {
            report.Log(LogFlag.SPAM, $"Get DownloadPath: {DownloadDirectory}");
            return DownloadDirectory;
        }
    }
}
