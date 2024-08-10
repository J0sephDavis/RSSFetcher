using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class Config
    {
        private string DownloadDirectory = ".\\Downloads\\";
        private Uri RSSConfigPath = new Uri(".\\rss-config.xml");
        private static Config s_Config;
        private Config()
        { }
        public static Config GetInstance()
        {
            if (s_Config == null)
                s_Config = new Config();
            return s_Config;
        }
        public Uri GetRSSPath()
        {
            return RSSConfigPath;
        }
    }
}
