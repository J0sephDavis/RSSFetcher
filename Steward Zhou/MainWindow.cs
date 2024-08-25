using Murong_Xue;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Text;

namespace Steward_Zhou
{
    public partial class MainWindow : Form
    {
        private Reporter report = Logger.RequestReporter("W-MAIN");
        private Controller controller = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Logger.GetInstance().AddModule(new LogOutputListView(LogListBox));
        }
        private void UpdateFeedList(List<Feed> feeds)
        {
            FeedListView.BeginUpdate();
            FeedListView.Items.Clear();
            foreach (var feed in feeds)
            {
                ListViewItem item = new(feed.ToStringList());
                FeedListView.Items.Add(item);
            }
            FeedListView.EndUpdate();
        }
        private void btnGetFeeds_Click(object sender, EventArgs e)
        {
            report.Trace("btnGetFeeds_click");
            UpdateFeedList(controller.GetFeeds());
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

        private void FeedListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder builder = new();
            builder.AppendLine("Selected Index Changed");
            //---
            if (FeedListView.SelectedItems.Count == 0)
                builder.AppendLine("No items selected");
            else
            {
                for (int i = 0; i < FeedListView.SelectedItems.Count; i++)
                {
                    builder.AppendLine(FeedListView.SelectedItems[i].Text + "|");
                }
            }
            //---
            report.Trace(builder.ToString());
        }
    }
}
