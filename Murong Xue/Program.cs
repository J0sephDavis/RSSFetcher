using Murong_Xue;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Numerics;
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
        bool EditConfigs = false;
#if DEBUG
        args = ["--edit"];
#endif
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
                    Console.WriteLine("Last commit hash: 0b8375a8");
                    return;
                //-------------------------
                case "--edit":
                case "-edit":
                    EditConfigs = true;
                    break;
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
        if (EditConfigs)
        {
            InteractiveEditor editor = new InteractiveEditor(RSSEntries.GetFeeds());
            if (editor.MainLoop())
            {
                Console.WriteLine("SAVE!!!");
                await RSSEntries.UpdateEntries();
            }
            else
                Console.WriteLine("DISCARD.");
        }
        else
            await RSSEntries.Process();
    }
}