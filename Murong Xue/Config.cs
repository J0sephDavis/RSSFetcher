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
        private static Config? s_Config = null;
        private Config()
        {
            string rootDir = Path.GetDirectoryName(System.AppContext.BaseDirectory) + @"\";
            DownloadDirectory = rootDir + @"Downloads\";
            RSSConfigPath = new Uri(rootDir + "rss-config.xml");
        }
        public static Config GetInstance()
        {
            if (s_Config == null)
                s_Config = new Config();
            return s_Config;
        }
        public void SetRSSPath(string path)
        {
            RSSConfigPath = new Uri(path);
            Console.WriteLine("RSS PATH: {0}", RSSConfigPath);
        }
        public Uri GetRSSPath()
        {
            Console.WriteLine("RSS PATH: {0}", RSSConfigPath);
            return RSSConfigPath;
        }
        public void SetDownloadPath(string path)
        {
            DownloadDirectory = path;
            Console.WriteLine("DOWNLOAD PATH: {0}", DownloadDirectory);
        }
        public string GetDownloadPath()
        {
            Console.WriteLine("DOWNLOAD PATH: {0}", DownloadDirectory);
            return DownloadDirectory;
        }
    }
}
