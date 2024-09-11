using RSSFetcher.Logging.Output.Modules;

namespace RSSFetcher.Logging.OutputHandling
{
    internal class LogOutputManager
    {
        // ---
        private readonly List<LogMsg> Buffer = []; // lock needed
        private List<LogMsg> buffer_copy = []; // no lock needed
        private readonly object BufferLock = new();
        // ---
        private readonly Thread BufferThread;
        private readonly AutoResetEvent BufferEvent = new(false);
        // ---
        private bool StopThread = false;
        // ---
        const int BUFFER_THRESHOLD = 10;
        const int BUFFER_TIMEOUT = 500;
        // ---
        private readonly List<IOutputModule> modules = [];
        public IOutputConsole? GetConsole()
        {
            foreach (var mod in modules)
                if (mod.Type == OutputModuleClassification.CONSOLE)
                    //might seem redundant, but i'm unsure the cost of type checking compared to our enum type checking
                    //+ make sure we actually returned a proper cast
                    if (mod is IOutputConsole) return (IOutputConsole?)mod;
            return null;
        }
        //
        public LogOutputManager()
        {
            BufferThread = new(Main);
        }
        // ---
        public void SetPath(Uri path)
        {
            AddModule(new LogFile(path));
        }
        public void AddModule(IOutputModule mod)
        {
            modules.Add(mod);
        }
        // ---
        public void Start()
        {
            BufferThread.Start();
        }
        public void Stop()
        {
            if (BufferThread.IsAlive)
            {
                StopThread = true;
                BufferEvent.Set();
                BufferThread.Join();
            }
            foreach (var mod in modules)
                mod.Dispose();
        }
        // ---
        public void Log(LogMsg msg)
        {
            lock (BufferLock)
            {
                Buffer.Add(msg);
                if (Buffer.Count > BUFFER_THRESHOLD)
                    BufferEvent.Set();
            }
        }
        private void WriteMsg()
        {
            lock (BufferLock)
            {
                buffer_copy = new(Buffer);
                Buffer.Clear();
            }
            if (buffer_copy.Count == 0) return;
            foreach (LogMsg msg in buffer_copy)
            {
                foreach (var module in modules)
                    module.WriteMsg(msg);
            }
        }
        // ---
        private void Main()
        {
            while (StopThread == false)
            {
                BufferEvent.WaitOne(BUFFER_TIMEOUT);
                WriteMsg();
            }
            WriteMsg(); //stragglers
        }
    }
}
