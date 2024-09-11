using RSSFetcher.Logging.Output.Modules;
using RSSFetcher.Logging;
namespace Steward_Zhou
{
    internal class LogOutputListView : IOutputModule
    {
        public OutputModuleClassification Type { get => OutputModuleClassification.UI; }
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
        public void WriteMsg(LogMsg msg)
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
