using RSSFetcher.Logging;

namespace Murong_Xue.Logging.Output.Modules
{
    public interface IOutputModule
    {
        public void WriteMsg(LogMsg msg); //accept a copy of the buffer?
        public void Dispose();
    }
}
