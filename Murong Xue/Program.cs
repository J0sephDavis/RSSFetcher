using Murong_Xue;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace MurongXue;
public class Program
{
    private static EntryData? RSSEntries = null;
    private static readonly Config cfg = Config.GetInstance();
    public static async Task Main(string[] args)
    {
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
        foreach (string arg in args)
        {
            switch (arg.ToLower())
            {
                case "-help":
                case "--help":
                case "-h":
                    Console.WriteLine(
                        "Use -rsscfg PATH\\CONFIGFILE.xml to set the confit path\n" +
                        "Use -downloadPath PATH\\DOWNLOADS_FOLDER to set the downloads folder\n" +
                        "Use -version to get the last commit hash(gotta remember to update this one)");
                    return;
                case "--version":
                case "-version":
                case "--v":
                case "-v":
                    Console.WriteLine("Last commit hash: 584b3e3d");
                    return;
                //-------------------------
                case "--rsscfg":
                case "-rsscfg":
                    NextIsConfig = true;
                    break;
                case "--downloadpath":
                case "-downloadpath":
                    NextIsDownloadDir = true;
                    break;
                default:
                    if (NextIsConfig)
                        cfg.SetRSSPath(Path.GetFullPath(arg));
                    else if (NextIsDownloadDir)
                        cfg.SetDownloadPath(Path.GetFullPath(arg));
                    //-
                    NextIsDownloadDir = false;
                    NextIsConfig = false;
                    break;
            }
        }
        RSSEntries = new EntryData(cfg.GetRSSPath());
        await RSSEntries.Process();
    }
}