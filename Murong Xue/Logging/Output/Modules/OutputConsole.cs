using RSSFetcher.Logging;

namespace RSSFetcher.Logging.Output.Modules
{

    public interface IOutputConsole
    {
        void Pause();
        void Unpause();
        bool IsPaused();
        bool IsInteractive();
    }

    public class LogConsole() : IOutputModule, IOutputConsole
    {
        public OutputModuleClassification Type { get => OutputModuleClassification.CONSOLE; }
        public void WriteMsg(LogMsg msg)
        {
            Console.WriteLine(msg.ToString());
        }
        public void Dispose() { }
        public void Pause() { }
        public void Unpause() { }
        public bool IsPaused() { return false; }
        public bool IsInteractive() => false;
    }

    public class InteractiveConsole() : IOutputModule, IOutputConsole
    {
        public bool IsInteractive() => true;
        public OutputModuleClassification Type { get => OutputModuleClassification.CONSOLE; }
        // ---
        private readonly object tv_remote = new();
        private bool Paused = false; //todo, set a timeout?
        public void Pause()
        {
            lock (tv_remote)
            {
                Paused = true;
                PrintBuff();
            }
        }
        public void Unpause()
        {
            lock (tv_remote)
            {
                Paused = false;
                PrintBuff();
            }
        }
        public bool IsPaused()
        {
            lock (tv_remote) return Paused;
        }
        // ---
        private readonly object bufflock = new();
        private readonly List<LogMsg> buffer = [];
        private void PrintBuff()
        {
            lock (bufflock)
            {
                foreach (var msg in buffer)
                {
                    DoMsg(msg);
                }
                buffer.Clear();
            }
        }
        // ---
        private static void DoMsg(LogMsg msg)
        {
            if ((msg & LogType.ERROR) != 0)
                Console.WriteLine(msg);
        }
        public void WriteMsg(LogMsg msg)
        {
            if ((msg & LogMod.INTERACTIVE) != 0) return; //ignore msgs marked interactive, they are output immediately by the InteractiveReporter

            lock(tv_remote)
            {
                lock (bufflock)
                {
                    if (Paused) buffer.Add(msg);
                    else
                    {
                        if (buffer.Count != 0)
                            PrintBuff();
                        DoMsg(msg);
                    }
                }
            }
        }
        public void Dispose() { }
    }
}
