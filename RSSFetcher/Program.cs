using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher;
public partial class Program
{
    //---
    static Reporter report;
    public static async Task Main(string[] args)
    {
        report = Logger.RequestReporter("PROGRM");
        report.Notice(Controller.versionString);
#if DEBUG
        report.Trace("compilation flag DEBUG was set");
        /*
        Logger.SetLogLevel(new(LogType.ALL, LogMod.ALL));
        args = [
            //"--version"
            //"--help"
            //"--SPAM",
            //"--VERBOSE",
            //"--UNIMPORTANT",
            //"--DEBUG",
            //"--edit",
        ];*/
#endif
        using Controller controller = new(new(args));
        await controller.Run();
        report.Out(controller.GetSummary());
        report.Notice("Program STOP");
    }
}