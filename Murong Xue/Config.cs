using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher
{
    public class Config : IDisposable
    {
        private string DownloadDirectory;
        private Uri RSSConfigPath;
        //---
        private static Config? s_Config = null;
        private readonly Reporter report;
        private readonly Logger _Logger;

        private Config()
        {
            string rootDir = Path.GetDirectoryName(System.AppContext.BaseDirectory) + Path.DirectorySeparatorChar;
            DownloadDirectory = rootDir + @"Downloads" + Path.DirectorySeparatorChar;
            RSSConfigPath = new Uri(rootDir + "rss-config.xml");

            _Logger = Logger.GetInstance();
            _Logger.SetPath(new(rootDir + @"MurongLie.log"));
            report = Logger.RequestReporter("CONFIG");
            report.Trace($"Config(), rootDir: {rootDir}, cfgPath:{RSSConfigPath}, downloadDir:{DownloadDirectory}");
        }
        public static Config GetInstance()
        {
            s_Config ??= new Config();
            return s_Config;
        }
        //----------
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
            //clear s_Config?
            _Logger.Dispose();
        }
    }
}
