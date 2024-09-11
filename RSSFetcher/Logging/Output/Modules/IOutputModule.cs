using RSSFetcher.Logging;

namespace RSSFetcher.Logging.Output.Modules
{
    public enum OutputModuleClassification
    {
        NONE = 0,
        CONSOLE,
        FILE,
        UI,
        UNKNOWN,
    }
    public interface IOutputModule
    {
        public OutputModuleClassification Type { get; }
        public void WriteMsg(LogMsg msg); //accept a copy of the buffer?
        public void Dispose();
    }
}
