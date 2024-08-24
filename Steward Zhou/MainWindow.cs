using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Steward_Zhou
{
    public partial class MainWindow : Form
    {
        Reporter report = Logger.RequestReporter("W-MAIN");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Logger.GetInstance().AddModule(new LogOutputListView(LogListBox));
        }

        private void btnGetFeeds_Click(object sender, EventArgs e)
        {
            report.Trace("btnGetFeeds_click");
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            report.Trace("btnProcess_Click");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            report.Trace("btnEdit_Click");
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            report.Trace("btnCreate_Click");
        }

        private void btnSaveQuit_Click(object sender, EventArgs e)
        {
            report.Trace("btnSaveQuit_Click");
        }
    }
}
