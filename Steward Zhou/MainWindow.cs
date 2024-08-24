using Murong_Xue.Logging;

namespace Steward_Zhou
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void MainWindow_Load(object sender, EventArgs e)
        {
            //UpdateFeedTree();
            Logger.GetInstance().AddModule(new LogOutputListView(LogListBox));
        }
        /*
        public void UpdateFeedTree()
        {
            FeedTree.BeginUpdate();
            //---------------------
            for (int i = 0; i < 5; i++)
            {
                TreeNode parent =
                    FeedTree.Nodes.Add("Parent " + i.ToString());
                for (int j = 0; j < 5 - i; j++)
                {
                    var child = parent.Nodes.Add("Child " + j.ToString());
                    if (j == i)
                        child.Nodes.Add("grand child");
                }
            }
            //---------------------
            FeedTree.EndUpdate();
        }*/
    }
}
