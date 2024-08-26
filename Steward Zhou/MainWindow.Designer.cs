namespace Steward_Zhou
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BasePanel = new Panel();
            splitContainer1 = new SplitContainer();
            FeedListView = new ListView();
            columnHeaderTitle = new ColumnHeader();
            columnHeaderDate = new ColumnHeader();
            RightMainSplit = new SplitContainer();
            tableLayoutPanel1 = new TableLayoutPanel();
            txtBoxID = new TextBox();
            lblTitle = new Label();
            txtBoxTitle = new TextBox();
            txtBoxURL = new TextBox();
            lblDate = new Label();
            txtBoxDate = new TextBox();
            LblId = new Label();
            lblURL = new Label();
            lblHistory = new Label();
            lblExpr = new Label();
            txtBoxRegex = new TextBox();
            txtBoxHistory = new TextBox();
            btnGoToURL = new Button();
            ButtonPanelFlowLayout = new FlowLayoutPanel();
            btnGetFeeds = new Button();
            btnProcess = new Button();
            btnEdit = new Button();
            btnCreate = new Button();
            btnSaveQuit = new Button();
            LogListBox = new ListBox();
            FeedStatusStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            BasePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RightMainSplit).BeginInit();
            RightMainSplit.Panel1.SuspendLayout();
            RightMainSplit.Panel2.SuspendLayout();
            RightMainSplit.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ButtonPanelFlowLayout.SuspendLayout();
            FeedStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // BasePanel
            // 
            BasePanel.Controls.Add(splitContainer1);
            BasePanel.Dock = DockStyle.Fill;
            BasePanel.Location = new Point(0, 0);
            BasePanel.Name = "BasePanel";
            BasePanel.Size = new Size(800, 428);
            BasePanel.TabIndex = 0;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(FeedListView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(RightMainSplit);
            splitContainer1.Size = new Size(800, 428);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 1;
            // 
            // FeedListView
            // 
            FeedListView.AllowColumnReorder = true;
            FeedListView.Columns.AddRange(new ColumnHeader[] { columnHeaderTitle, columnHeaderDate });
            FeedListView.Dock = DockStyle.Fill;
            FeedListView.GridLines = true;
            FeedListView.Location = new Point(0, 0);
            FeedListView.Name = "FeedListView";
            FeedListView.Size = new Size(266, 428);
            FeedListView.TabIndex = 1;
            FeedListView.UseCompatibleStateImageBehavior = false;
            FeedListView.View = View.Details;
            FeedListView.SelectedIndexChanged += FeedListView_SelectedIndexChanged;
            // 
            // columnHeaderTitle
            // 
            columnHeaderTitle.Text = "Title";
            // 
            // columnHeaderDate
            // 
            columnHeaderDate.Text = "Last Update";
            // 
            // RightMainSplit
            // 
            RightMainSplit.Dock = DockStyle.Fill;
            RightMainSplit.Location = new Point(0, 0);
            RightMainSplit.Name = "RightMainSplit";
            RightMainSplit.Orientation = Orientation.Horizontal;
            // 
            // RightMainSplit.Panel1
            // 
            RightMainSplit.Panel1.Controls.Add(tableLayoutPanel1);
            RightMainSplit.Panel1.Controls.Add(ButtonPanelFlowLayout);
            // 
            // RightMainSplit.Panel2
            // 
            RightMainSplit.Panel2.Controls.Add(LogListBox);
            RightMainSplit.Size = new Size(530, 428);
            RightMainSplit.SplitterDistance = 252;
            RightMainSplit.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(txtBoxID, 1, 0);
            tableLayoutPanel1.Controls.Add(lblTitle, 0, 1);
            tableLayoutPanel1.Controls.Add(txtBoxTitle, 1, 1);
            tableLayoutPanel1.Controls.Add(txtBoxURL, 1, 2);
            tableLayoutPanel1.Controls.Add(lblDate, 0, 4);
            tableLayoutPanel1.Controls.Add(txtBoxDate, 1, 4);
            tableLayoutPanel1.Controls.Add(LblId, 0, 0);
            tableLayoutPanel1.Controls.Add(lblURL, 0, 2);
            tableLayoutPanel1.Controls.Add(lblHistory, 0, 5);
            tableLayoutPanel1.Controls.Add(lblExpr, 0, 3);
            tableLayoutPanel1.Controls.Add(txtBoxRegex, 1, 3);
            tableLayoutPanel1.Controls.Add(txtBoxHistory, 1, 5);
            tableLayoutPanel1.Controls.Add(btnGoToURL, 2, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(118, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(412, 252);
            tableLayoutPanel1.TabIndex = 21;
            // 
            // txtBoxID
            // 
            txtBoxID.Dock = DockStyle.Fill;
            txtBoxID.Enabled = false;
            txtBoxID.Location = new Point(54, 3);
            txtBoxID.Name = "txtBoxID";
            txtBoxID.Size = new Size(275, 23);
            txtBoxID.TabIndex = 10;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Location = new Point(3, 29);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(45, 29);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "Title";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtBoxTitle
            // 
            txtBoxTitle.Dock = DockStyle.Fill;
            txtBoxTitle.Location = new Point(54, 32);
            txtBoxTitle.Name = "txtBoxTitle";
            txtBoxTitle.Size = new Size(275, 23);
            txtBoxTitle.TabIndex = 12;
            // 
            // txtBoxURL
            // 
            txtBoxURL.Dock = DockStyle.Fill;
            txtBoxURL.Location = new Point(54, 61);
            txtBoxURL.Name = "txtBoxURL";
            txtBoxURL.PlaceholderText = "URL to rss feed";
            txtBoxURL.Size = new Size(275, 23);
            txtBoxURL.TabIndex = 14;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Dock = DockStyle.Fill;
            lblDate.Location = new Point(3, 116);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(45, 29);
            lblDate.TabIndex = 17;
            lblDate.Text = "Date";
            lblDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtBoxDate
            // 
            txtBoxDate.Dock = DockStyle.Fill;
            txtBoxDate.Location = new Point(54, 119);
            txtBoxDate.Name = "txtBoxDate";
            txtBoxDate.Size = new Size(275, 23);
            txtBoxDate.TabIndex = 18;
            // 
            // LblId
            // 
            LblId.AutoSize = true;
            LblId.Dock = DockStyle.Fill;
            LblId.Location = new Point(3, 0);
            LblId.Name = "LblId";
            LblId.Size = new Size(45, 29);
            LblId.TabIndex = 6;
            LblId.Text = "ID";
            LblId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblURL
            // 
            lblURL.AutoSize = true;
            lblURL.Dock = DockStyle.Fill;
            lblURL.Location = new Point(3, 58);
            lblURL.Name = "lblURL";
            lblURL.Size = new Size(45, 29);
            lblURL.TabIndex = 13;
            lblURL.Text = "URL";
            lblURL.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblHistory
            // 
            lblHistory.AutoSize = true;
            lblHistory.Dock = DockStyle.Fill;
            lblHistory.Location = new Point(3, 145);
            lblHistory.Name = "lblHistory";
            lblHistory.Size = new Size(45, 29);
            lblHistory.TabIndex = 19;
            lblHistory.Text = "History";
            lblHistory.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblExpr
            // 
            lblExpr.AutoSize = true;
            lblExpr.Dock = DockStyle.Fill;
            lblExpr.Location = new Point(3, 87);
            lblExpr.Name = "lblExpr";
            lblExpr.Size = new Size(45, 29);
            lblExpr.TabIndex = 15;
            lblExpr.Text = "Regex";
            lblExpr.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtBoxRegex
            // 
            txtBoxRegex.Dock = DockStyle.Fill;
            txtBoxRegex.Location = new Point(54, 90);
            txtBoxRegex.Name = "txtBoxRegex";
            txtBoxRegex.Size = new Size(275, 23);
            txtBoxRegex.TabIndex = 16;
            // 
            // txtBoxHistory
            // 
            txtBoxHistory.Dock = DockStyle.Fill;
            txtBoxHistory.Location = new Point(54, 148);
            txtBoxHistory.Name = "txtBoxHistory";
            txtBoxHistory.Size = new Size(275, 23);
            txtBoxHistory.TabIndex = 20;
            // 
            // btnGoToURL
            // 
            btnGoToURL.Dock = DockStyle.Right;
            btnGoToURL.Location = new Point(335, 61);
            btnGoToURL.Name = "btnGoToURL";
            btnGoToURL.Size = new Size(74, 23);
            btnGoToURL.TabIndex = 21;
            btnGoToURL.Text = "Open";
            btnGoToURL.UseVisualStyleBackColor = true;
            // 
            // ButtonPanelFlowLayout
            // 
            ButtonPanelFlowLayout.AutoSize = true;
            ButtonPanelFlowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ButtonPanelFlowLayout.Controls.Add(btnGetFeeds);
            ButtonPanelFlowLayout.Controls.Add(btnProcess);
            ButtonPanelFlowLayout.Controls.Add(btnEdit);
            ButtonPanelFlowLayout.Controls.Add(btnCreate);
            ButtonPanelFlowLayout.Controls.Add(btnSaveQuit);
            ButtonPanelFlowLayout.Dock = DockStyle.Left;
            ButtonPanelFlowLayout.Location = new Point(0, 0);
            ButtonPanelFlowLayout.Name = "ButtonPanelFlowLayout";
            ButtonPanelFlowLayout.Size = new Size(118, 252);
            ButtonPanelFlowLayout.TabIndex = 5;
            // 
            // btnGetFeeds
            // 
            btnGetFeeds.AutoSize = true;
            btnGetFeeds.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGetFeeds.Location = new Point(3, 3);
            btnGetFeeds.Name = "btnGetFeeds";
            btnGetFeeds.Size = new Size(68, 25);
            btnGetFeeds.TabIndex = 4;
            btnGetFeeds.Text = "Get Feeds";
            btnGetFeeds.UseVisualStyleBackColor = true;
            btnGetFeeds.Click += btnGetFeeds_Click;
            // 
            // btnProcess
            // 
            btnProcess.AutoSize = true;
            btnProcess.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnProcess.Location = new Point(3, 34);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(90, 25);
            btnProcess.TabIndex = 0;
            btnProcess.Text = "Process Feeds";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // btnEdit
            // 
            btnEdit.AutoSize = true;
            btnEdit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnEdit.Location = new Point(3, 65);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(112, 25);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Edit Selected Feed";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnCreate
            // 
            btnCreate.AutoSize = true;
            btnCreate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCreate.Location = new Point(3, 96);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(79, 25);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "Create Feed";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnSaveQuit
            // 
            btnSaveQuit.AutoSize = true;
            btnSaveQuit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSaveQuit.Location = new Point(3, 127);
            btnSaveQuit.Name = "btnSaveQuit";
            btnSaveQuit.Size = new Size(80, 25);
            btnSaveQuit.TabIndex = 3;
            btnSaveQuit.Text = "Save && Quit";
            btnSaveQuit.UseVisualStyleBackColor = true;
            btnSaveQuit.Click += btnSaveQuit_Click;
            // 
            // LogListBox
            // 
            LogListBox.Dock = DockStyle.Fill;
            LogListBox.FormattingEnabled = true;
            LogListBox.ItemHeight = 15;
            LogListBox.Location = new Point(0, 0);
            LogListBox.Name = "LogListBox";
            LogListBox.Size = new Size(530, 172);
            LogListBox.TabIndex = 0;
            // 
            // FeedStatusStrip
            // 
            FeedStatusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3, toolStripStatusLabel4 });
            FeedStatusStrip.Location = new Point(0, 428);
            FeedStatusStrip.Name = "FeedStatusStrip";
            FeedStatusStrip.Size = new Size(800, 22);
            FeedStatusStrip.Stretch = false;
            FeedStatusStrip.TabIndex = 1;
            FeedStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(118, 17);
            toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(118, 17);
            toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(118, 17);
            toolStripStatusLabel4.Text = "toolStripStatusLabel4";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(BasePanel);
            Controls.Add(FeedStatusStrip);
            Name = "MainWindow";
            Text = "MainWindow";
            Load += MainWindow_Load;
            BasePanel.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            RightMainSplit.Panel1.ResumeLayout(false);
            RightMainSplit.Panel1.PerformLayout();
            RightMainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)RightMainSplit).EndInit();
            RightMainSplit.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ButtonPanelFlowLayout.ResumeLayout(false);
            ButtonPanelFlowLayout.PerformLayout();
            FeedStatusStrip.ResumeLayout(false);
            FeedStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel BasePanel;
        private StatusStrip FeedStatusStrip;
        private SplitContainer RightMainSplit;
        private SplitContainer splitContainer1;
        private ListBox LogListBox;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private Button btnEdit;
        private Button btnProcess;
        private Button btnSaveQuit;
        private Button btnCreate;
        private Button btnGetFeeds;
        private FlowLayoutPanel ButtonPanelFlowLayout;
        private ListView FeedListView;
        private ColumnHeader columnHeaderTitle;
        private ColumnHeader columnHeaderDate;
        private TextBox txtBoxDate;
        private Label lblDate;
        private TextBox txtBoxRegex;
        private Label lblExpr;
        private TextBox txtBoxURL;
        private Label lblURL;
        private TextBox txtBoxTitle;
        private Label lblTitle;
        private Label LblId;
        private TextBox txtBoxHistory;
        private Label lblHistory;
        private TextBox txtBoxID;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnGoToURL;
    }
}
