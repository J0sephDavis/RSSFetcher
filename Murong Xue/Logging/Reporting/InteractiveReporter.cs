using RSSFetcher.Logging.OutputHandling;

namespace RSSFetcher.Logging.Reporting
{
    public class InteractiveReporter(LogConsole _console, LogLevel level, string identifier)
        : Reporter(level,identifier)
    {
        public void Interactive(string msg)
        {
            Log(LogType.OUTPUT, LogMod.INTERACTIVE, msg);
        }

        LogConsole console = _console;
        public void PauseOutput()
        {
            console.Pause();    
        }
        public void UnpauseOutput()
        {
            console.Unpause();
        }
        public void StartMsg()
        {
            console.StartMessage();
        }
        public void EndMsg()
        {
            console.EndMessage();
        }
        public bool InMessage()
        {
            return console.GetInMessage();
        }
    }
}
