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
                FeedListViewItem item = new(feed, FeedFields.TITLE | FeedFields.DATE);
                FeedListView.Items.Add(item);
            }
            //Autosize the columns to the text.
            foreach (ColumnHeader col in FeedListView.Columns)
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
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
                report.Trace("No items selected");
            else if (FeedListView.SelectedItems.Count == 1)
            {
                report.Trace("One entry: " + FeedListView.SelectedItems[0]);
                FeedListViewItem feed_payload = FeedListView.SelectedItems[0] as FeedListViewItem;
                InfoListView_PrintFeed(feed_payload.feed);
            }
        }

        public void InfoListView_PrintFeed(Feed feed)
        {
            InfoListView.BeginUpdate();
            InfoListView.Items.Clear();
            InfoListView.Items.Add(new FeedListViewItem(feed));
            //Autosize the columns to the text.
            foreach (ColumnHeader col in InfoListView.Columns)
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            InfoListView.EndUpdate();
        }
    }
    [Flags]
    enum FeedFields
    {
        ID = 1 << 0,
        TITLE = 1 << 1,
        URL = 1<<2,
        EXPRESSION =1<<3,
        DATE = 1<<4,
        HISTORY = 1<<5,
        
        ALL = ID | TITLE | URL | EXPRESSION | DATE | HISTORY,
    };
    internal class FeedListViewItem : ListViewItem
    {
        public Feed feed;
        //TODO instead of use FeedFields as a flag, it should be an ordered list/array
        //So we can genereate diff orders
        public FeedListViewItem(Feed _feed, FeedFields fields = FeedFields.ALL) : base()
        {
            base.SubItems.Clear();
            feed = _feed;

            if ((fields & FeedFields.ID)>0)
                base.SubItems.Add(feed.ID.ToString());

            if ((fields & FeedFields.TITLE) > 0)
                base.SubItems.Add(feed.Title);

            if ((fields & FeedFields.URL) > 0)
                base.SubItems.Add(feed.URL.ToString());

            if ((fields & FeedFields.EXPRESSION) > 0)
                base.SubItems.Add(feed.Expression);

            if ((fields & FeedFields.DATE) > 0)
                base.SubItems.Add(feed.Date.ToString());

            if ((fields & FeedFields.HISTORY) > 0)
                base.SubItems.Add(feed.History);

            //I have no clue why there is an empty string added by default
            if (base.SubItems.Count > 0 && base.SubItems[0].Text == string.Empty)
                base.SubItems.RemoveAt(0);
        }
    }
}
