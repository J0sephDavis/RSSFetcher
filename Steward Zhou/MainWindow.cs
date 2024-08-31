using Murong_Xue;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Security.Cryptography;
using System.Text;

namespace Steward_Zhou
{
    public partial class MainWindow : Form
    {
        private Reporter report = Logger.RequestReporter("W-MAIN");
        private Controller controller = new();
        private Feed? EditingFeed = null;
        public MainWindow()
        {
            Logger.SetInteractiveMode(true);
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Logger.GetInstance().AddModule(new LogOutputListView(LogListBox));
        }
        private void UpdateFeedList()
        {
            List<Feed> feeds = controller.GetFeeds();
            FeedListView.BeginUpdate();
            FeedListView.Items.Clear();
            foreach (var feed in feeds)
            {
                FeedListViewItem item = new(feed, FeedFields.TITLE | FeedFields.DATE | FeedFields.STATUS);
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
            UpdateFeedList();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            report.Trace("btnProcess_Click");
            //queue feeds & process
            controller.DownloadFeeds();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            report.Trace("btnEdit_Click");
            if (EditingFeed == null) return;
            //----
            EditingFeed.Title = txtBoxTitle.Text;

            bool create = Uri.TryCreate(txtBoxURL.Text, UriKind.Absolute, out EditingFeed.URL);
            report.DebugVal($"TryCreate of URL: txt:{txtBoxURL.Text} success? {create}");

            EditingFeed.Expression = txtBoxRegex.Text;
            EditingFeed.Date =
                (txtBoxDate.Text == string.Empty)
                    ? DateTime.UnixEpoch
                    : DateTime.Parse(txtBoxDate.Text);
            EditingFeed.History = txtBoxHistory.Text;
            //----
            if ((EditingFeed.Status & FeedStatus.LINKED) == 0)
            {
                if (controller.CreateFeed(EditingFeed))
                    report.Debug("Failed to create feed");
                else
                    report.Debug("Created feed!");
            }
            UpdateAllPanels();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            report.Trace("btnDelete clicked");
            if (EditingFeed == null)
            {
                report.Trace("editing feed == null");
                return;
            }
            if (controller.DeleteFeed(EditingFeed))
                UpdateAllPanels();
        }
        /// <summary>
        /// Update everything we can feasibly update. probably a better sol'n in the future,
        /// but this should be good for now
        /// </summary>
        private void UpdateAllPanels() // aka Redraw?
        {
            UpdateFeedList();
            UpdateEditorFields();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            report.Trace("btnCreate_Click");
            EditingFeed = controller.CreateNewFeedRecord();
            UpdateAllPanels();
        }

        private void btnSaveQuit_Click(object sender, EventArgs e)
        {
            report.Trace("btnSaveQuit_Click");
            controller.UpdateEntries();
        }

        private void FeedListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder builder = new();
            builder.AppendLine("Selected Index Changed");
            //---
            if (FeedListView.SelectedItems.Count == 0)
            {
                report.Trace("Selection count == 0");
                EditingFeed = null;
            }
            else if (FeedListView.SelectedItems[0] is FeedListViewItem feed_payload)
            {
                report.Trace("first entry in selection: " + FeedListView.SelectedItems[0]);
                EditingFeed = feed_payload.feed;
            }
            else
            {
                report.Error("Invalid selection");
                EditingFeed = null;
            }
            UpdateEditorFields();
        }

        public void UpdateEditorFields()
        {
            //---
            if (EditingFeed == null)
            {
                txtBoxID.Clear();
                txtBoxTitle.Clear();
                txtBoxURL.Clear();
                txtBoxRegex.Clear();
                txtBoxDate.Clear();
                txtBoxHistory.Clear();
                return;
            }
            //---
            txtBoxID.Text = EditingFeed.ID.ToString();
            txtBoxTitle.Text = EditingFeed.Title;
            txtBoxURL.Text = EditingFeed.URL != null ? EditingFeed.URL.ToString() : string.Empty;
            txtBoxRegex.Text = EditingFeed.Expression;
            txtBoxDate.Text = EditingFeed.Date.ToString();
            txtBoxHistory.Text = EditingFeed.History;
            txtBoxStatus.Text = EditingFeed.Status.ToString();
        }
    }
    [Flags]
    enum FeedFields
    {
        ID          = 1 << 0,
        TITLE       = 1 << 1,
        URL         = 1 << 2,
        EXPRESSION  = 1 << 3,
        DATE        = 1 << 4,
        HISTORY     = 1 << 5,
        STATUS      = 1 << 6,
        
        ALL = ID | TITLE | URL | EXPRESSION | DATE | HISTORY | STATUS,
    };
    internal class FeedListViewItem : ListViewItem
    {
        public Feed feed;
        //TODO instead of use FeedFields as a flag, it should be an ordered list/array
        //So we can genereate diff orders
        public FeedListViewItem(Feed _feed, FeedFields fields = FeedFields.ALL) : base()
        {
            //reduce data dup by only storing the ID instaed of the Feed itself?, call GetFeed(ID) to get it later
            base.SubItems.Clear();
            feed = _feed;

            if ((fields & FeedFields.ID)>0)
                base.SubItems.Add(feed.ID.ToString());

            if ((fields & FeedFields.TITLE) > 0)
                base.SubItems.Add(feed.Title);

            if ((fields & FeedFields.URL) > 0)
                base.SubItems.Add(feed.URL != null ? feed.URL.ToString() : string.Empty);

            if ((fields & FeedFields.EXPRESSION) > 0)
                base.SubItems.Add(feed.Expression);

            if ((fields & FeedFields.DATE) > 0)
                base.SubItems.Add(feed.Date.ToString());

            if ((fields & FeedFields.HISTORY) > 0)
                base.SubItems.Add(feed.History);

            if ((fields & FeedFields.STATUS) > 0)
                base.SubItems.Add(feed.Status.ToString());

            //I have no clue why there is an empty string added by default
            if (base.SubItems.Count > 0 && base.SubItems[0].Text == string.Empty)
                base.SubItems.RemoveAt(0);
        }
    }
}