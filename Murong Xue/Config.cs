using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class Config
    {
        private string DownloadDirectory;
        private Uri RSSConfigPath;
        private LogFlag LogLevel = LogFlag.ALL;
        //---
        private static Config? s_Config = null;
        private static readonly Reporter report = new Reporter(LogFlag.DEFAULT, "CONFIG");

        private Config()
        {

            string rootDir = Path.GetDirectoryName(System.AppContext.BaseDirectory) + @"\";
            DownloadDirectory = rootDir + @"Downloads\";
            RSSConfigPath = new Uri(rootDir + "rss-config.xml");
            report.Log(LogFlag.DEBUG, $"Config(), rootDir: {rootDir}, cfgPath:{RSSConfigPath}, downloadDir:{DownloadDirectory}");
        }
        public static Config GetInstance()
        {
            report.Log(LogFlag.DEBUG, "GetInstance");
            if (s_Config == null)
                s_Config = new Config();
            return s_Config;
        }
        public void SetLogLevel(LogFlag level)
        {
            this.LogLevel = level;
            report.Log(LogFlag.DEBUG_SPAM, $"New LogLevel {this.LogLevel}");
        }
        public LogFlag GetLogLevel()
        {
            report.Log(LogFlag.DEBUG_SPAM, $"Get LogLevel {this.LogLevel}");
            return this.LogLevel;
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
