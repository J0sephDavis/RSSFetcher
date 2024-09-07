namespace RSSFetcher.Logging.OutputHandling
{
    internal class LogOutputManager
    {
        //---MODULES---
        private readonly List<IOutputModule> modules = [];
        //---
        private readonly List<LogMsg> Buffer = []; // lock needed
        private List<LogMsg> buffer_copy = []; // no lock needed
        private readonly object BufferLock = new();
        //---
        private readonly Thread BufferThread;
        private readonly AutoResetEvent BufferEvent = new(false);
        //
        private bool StopThread = false;
        // When InteractiveMode is enabled, only show the short strings in the viewport & decrease either buffer_timeout or threshold
        // OR whenever a msg if flagged interactive & added to the queue immediately end the wait
        public bool InteractiveMode { get; set; } = false;
        // ---
        const int BUFFER_THRESHOLD = 10;
        const int BUFFER_TIMEOUT = 5000;
        const int INTERACTIVE_TIMEOUT = 250;
        //
        public LogOutputManager()
        {
            BufferThread = new(Main);
        }
        public void SetPath(Uri path)
        {
            AddModule(new LogFile(path));
        }
        public void AddModule(IOutputModule mod)
        {
            modules.Add(mod);
        }
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
        public void Log(LogMsg msg)
        {
            lock (BufferLock)
            {
                Buffer.Add(msg);
                if (Buffer.Count > BUFFER_THRESHOLD || InteractiveMode && (msg & LogMod.INTERACTIVE) != 0)
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
        private void Main()
        {
            while (StopThread == false)
            {
                BufferEvent.WaitOne(InteractiveMode ? INTERACTIVE_TIMEOUT : BUFFER_TIMEOUT);
                WriteMsg();
            }
            WriteMsg(); //stragglers
        }
    }
}
