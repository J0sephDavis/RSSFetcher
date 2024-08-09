using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Murong_Xue
{
    internal class ConfigData
    {
        private Uri path;
        private List<FeedData> data;

        public ConfigData(Uri filePath)
        {
            this.path = filePath;
            data = new List<FeedData>();
            Console.WriteLine(
                $"filePath: IsFile?{filePath.IsFile}" +
                $"LocalPath:{filePath.LocalPath}"
            );
        }
        public void Process()
        {
            DoConfig();
        }
        public async void DoConfig()
        {
            Console.WriteLine("Opening file");
            FileStream xStream = System.IO.File.Open(path.LocalPath, FileMode.Open);
            await ReadConfig(xStream);
        }
        async Task ReadConfig(System.IO.Stream stream)
        {
            XmlReaderSettings xSettings = new XmlReaderSettings();
            xSettings.Async = true;

            using (XmlReader reader = XmlReader.Create(stream, xSettings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine("Start Element {0}", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine("Text Node {0}", await reader.GetValueAsync());
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine("End Element {0}", reader.Name);
                            break;
                        default:
                            Console.WriteLine("Other Node {0} with value {1}", reader.NodeType, reader.Value);
                            break;
                    }
                }
            }
        }
    }
}
