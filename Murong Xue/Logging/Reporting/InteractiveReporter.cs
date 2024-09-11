using RSSFetcher.Logging.Output.Modules;

namespace RSSFetcher.Logging.Reporting
{
    public class InteractiveReporter(LogLevel level, string identifier)
        : Reporter(level,identifier)
    {
        public void Interactive(string msg)
        {
            Log(LogType.OUTPUT, LogMod.INTERACTIVE, msg);
        }

        readonly IOutputConsole? console = Logger.GetInstance().GetConsole();
        public void PauseOutput()
        {
            console?.Pause();    
        }
        public void UnpauseOutput()
        {
            console?.Unpause();
        }
        public bool IsPaused()
        {
            if (console != null) return console.IsPaused();
            else return false;
        }
        public bool IsInteractive()
        {
            if (console != null) return console.IsInteractive();
            else return false;
        }
    }
}
