using Murong_Xue.Logging;
using Murong_Xue.Logging.OutputHandling;

namespace Steward_Zhou
{
    internal class LogOutputListView : IOutputModule
    {
        ListBox lb;
        bool Disposed = false;
        public LogOutputListView(ListBox ListBox)
        {
            lb = ListBox;
            lb.Disposed += OnDisposed;
        }

        private void OnDisposed(object? sender, EventArgs e)
        {
            Disposed = true;
        }
        public void WriteMsg(LogMsg msg, bool InteractiveMode)
        {
            if (Disposed) return;

            if (lb.InvokeRequired)
                lb.Invoke((MethodInvoker)delegate
                {
                    lb.Items.Add(msg.ToString());
                });
            else lb.Items.Add(msg.ToString()); //doubt this ever happenss
        }
        public void Dispose() { Disposed = true; }
    }
}
