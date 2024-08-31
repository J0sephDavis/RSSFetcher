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
            Property_History = new Panel();
            txtBoxHistory = new TextBox();
            lblHistory = new Label();
            Property_Date = new Panel();
            txtBoxDate = new TextBox();
            lblDate = new Label();
            Property_Expr = new Panel();
            txtBoxRegex = new TextBox();
            lblExpr = new Label();
            Property_URL = new Panel();
            txtBoxURL = new TextBox();
            lblURL = new Label();
            Property_Title = new Panel();
            txtBoxTitle = new TextBox();
            lblTitle = new Label();
            Property_ID = new Panel();
            txtBoxID = new TextBox();
            LblId = new Label();
            ButtonPanelFlowLayout = new FlowLayoutPanel();
            panel1 = new Panel();
            label1 = new Label();
            btnGetFeeds = new Button();
            btnProcess = new Button();
            btnEdit = new Button();
            btnCreate = new Button();
            btnSaveQuit = new Button();
            btnDelete = new Button();
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
            Property_History.SuspendLayout();
            Property_Date.SuspendLayout();
            Property_Expr.SuspendLayout();
            Property_URL.SuspendLayout();
            Property_Title.SuspendLayout();
            Property_ID.SuspendLayout();
            ButtonPanelFlowLayout.SuspendLayout();
            panel1.SuspendLayout();
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
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(Property_History, 0, 5);
            tableLayoutPanel1.Controls.Add(Property_Date, 0, 4);
            tableLayoutPanel1.Controls.Add(Property_Expr, 0, 3);
            tableLayoutPanel1.Controls.Add(Property_URL, 0, 2);
            tableLayoutPanel1.Controls.Add(Property_Title, 0, 1);
            tableLayoutPanel1.Controls.Add(Property_ID, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(131, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(399, 252);
            tableLayoutPanel1.TabIndex = 21;
            // 
            // Property_History
            // 
            Property_History.AutoSize = true;
            Property_History.Controls.Add(txtBoxHistory);
            Property_History.Controls.Add(lblHistory);
            Property_History.Location = new Point(3, 113);
            Property_History.MinimumSize = new Size(275, 16);
            Property_History.Name = "Property_History";
            Property_History.Size = new Size(275, 16);
            Property_History.TabIndex = 27;
            // 
            // txtBoxHistory
            // 
            txtBoxHistory.Dock = DockStyle.Fill;
            txtBoxHistory.Location = new Point(45, 0);
            txtBoxHistory.Name = "txtBoxHistory";
            txtBoxHistory.ScrollBars = ScrollBars.Horizontal;
            txtBoxHistory.Size = new Size(230, 23);
            txtBoxHistory.TabIndex = 20;
            // 
            // lblHistory
            // 
            lblHistory.AutoSize = true;
            lblHistory.Dock = DockStyle.Left;
            lblHistory.Location = new Point(0, 0);
            lblHistory.Name = "lblHistory";
            lblHistory.Size = new Size(45, 15);
            lblHistory.TabIndex = 19;
            lblHistory.Text = "History";
            lblHistory.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Property_Date
            // 
            Property_Date.AutoSize = true;
            Property_Date.Controls.Add(txtBoxDate);
            Property_Date.Controls.Add(lblDate);
            Property_Date.Location = new Point(3, 91);
            Property_Date.MinimumSize = new Size(275, 16);
            Property_Date.Name = "Property_Date";
            Property_Date.Size = new Size(275, 16);
            Property_Date.TabIndex = 26;
            // 
            // txtBoxDate
            // 
            txtBoxDate.Dock = DockStyle.Fill;
            txtBoxDate.Location = new Point(31, 0);
            txtBoxDate.Name = "txtBoxDate";
            txtBoxDate.ScrollBars = ScrollBars.Horizontal;
            txtBoxDate.Size = new Size(244, 23);
            txtBoxDate.TabIndex = 18;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Dock = DockStyle.Left;
            lblDate.Location = new Point(0, 0);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(31, 15);
            lblDate.TabIndex = 17;
            lblDate.Text = "Date";
            lblDate.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Property_Expr
            // 
            Property_Expr.AutoSize = true;
            Property_Expr.Controls.Add(txtBoxRegex);
            Property_Expr.Controls.Add(lblExpr);
            Property_Expr.Location = new Point(3, 69);
            Property_Expr.MinimumSize = new Size(275, 16);
            Property_Expr.Name = "Property_Expr";
            Property_Expr.Size = new Size(275, 16);
            Property_Expr.TabIndex = 25;
            // 
            // txtBoxRegex
            // 
            txtBoxRegex.Dock = DockStyle.Fill;
            txtBoxRegex.Location = new Point(39, 0);
            txtBoxRegex.Name = "txtBoxRegex";
            txtBoxRegex.ScrollBars = ScrollBars.Horizontal;
            txtBoxRegex.Size = new Size(236, 23);
            txtBoxRegex.TabIndex = 16;
            // 
            // lblExpr
            // 
            lblExpr.AutoSize = true;
            lblExpr.Dock = DockStyle.Left;
            lblExpr.Location = new Point(0, 0);
            lblExpr.Name = "lblExpr";
            lblExpr.Size = new Size(39, 15);
            lblExpr.TabIndex = 15;
            lblExpr.Text = "Regex";
            lblExpr.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Property_URL
            // 
            Property_URL.AutoSize = true;
            Property_URL.Controls.Add(txtBoxURL);
            Property_URL.Controls.Add(lblURL);
            Property_URL.Location = new Point(3, 47);
            Property_URL.MinimumSize = new Size(275, 16);
            Property_URL.Name = "Property_URL";
            Property_URL.Size = new Size(275, 16);
            Property_URL.TabIndex = 24;
            // 
            // txtBoxURL
            // 
            txtBoxURL.Dock = DockStyle.Fill;
            txtBoxURL.Location = new Point(28, 0);
            txtBoxURL.Name = "txtBoxURL";
            txtBoxURL.ScrollBars = ScrollBars.Horizontal;
            txtBoxURL.Size = new Size(247, 23);
            txtBoxURL.TabIndex = 14;
            // 
            // lblURL
            // 
            lblURL.AutoSize = true;
            lblURL.Dock = DockStyle.Left;
            lblURL.Location = new Point(0, 0);
            lblURL.Name = "lblURL";
            lblURL.Size = new Size(28, 15);
            lblURL.TabIndex = 13;
            lblURL.Text = "URL";
            lblURL.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Property_Title
            // 
            Property_Title.AutoSize = true;
            Property_Title.Controls.Add(txtBoxTitle);
            Property_Title.Controls.Add(lblTitle);
            Property_Title.Location = new Point(3, 25);
            Property_Title.MinimumSize = new Size(275, 16);
            Property_Title.Name = "Property_Title";
            Property_Title.Size = new Size(275, 16);
            Property_Title.TabIndex = 23;
            // 
            // txtBoxTitle
            // 
            txtBoxTitle.Dock = DockStyle.Fill;
            txtBoxTitle.Location = new Point(29, 0);
            txtBoxTitle.Name = "txtBoxTitle";
            txtBoxTitle.ScrollBars = ScrollBars.Horizontal;
            txtBoxTitle.Size = new Size(246, 23);
            txtBoxTitle.TabIndex = 12;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(29, 15);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "Title";
            lblTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Property_ID
            // 
            Property_ID.AutoSize = true;
            Property_ID.Controls.Add(txtBoxID);
            Property_ID.Controls.Add(LblId);
            Property_ID.Location = new Point(3, 3);
            Property_ID.MinimumSize = new Size(272, 16);
            Property_ID.Name = "Property_ID";
            Property_ID.Size = new Size(272, 16);
            Property_ID.TabIndex = 22;
            // 
            // txtBoxID
            // 
            txtBoxID.Dock = DockStyle.Left;
            txtBoxID.Enabled = false;
            txtBoxID.Location = new Point(18, 0);
            txtBoxID.MaxLength = 16;
            txtBoxID.Name = "txtBoxID";
            txtBoxID.ScrollBars = ScrollBars.Both;
            txtBoxID.Size = new Size(32, 23);
            txtBoxID.TabIndex = 10;
            // 
            // LblId
            // 
            LblId.AutoSize = true;
            LblId.Dock = DockStyle.Left;
            LblId.Location = new Point(0, 0);
            LblId.Name = "LblId";
            LblId.Size = new Size(18, 15);
            LblId.TabIndex = 6;
            LblId.Text = "ID";
            LblId.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ButtonPanelFlowLayout
            // 
            ButtonPanelFlowLayout.AutoSize = true;
            ButtonPanelFlowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ButtonPanelFlowLayout.Controls.Add(panel1);
            ButtonPanelFlowLayout.Controls.Add(btnProcess);
            ButtonPanelFlowLayout.Controls.Add(btnEdit);
            ButtonPanelFlowLayout.Controls.Add(btnCreate);
            ButtonPanelFlowLayout.Controls.Add(btnSaveQuit);
            ButtonPanelFlowLayout.Controls.Add(btnDelete);
            ButtonPanelFlowLayout.Dock = DockStyle.Left;
            ButtonPanelFlowLayout.Location = new Point(0, 0);
            ButtonPanelFlowLayout.Name = "ButtonPanelFlowLayout";
            ButtonPanelFlowLayout.Size = new Size(131, 252);
            ButtonPanelFlowLayout.TabIndex = 5;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnGetFeeds);
            panel1.Location = new Point(3, 3);
            panel1.MinimumSize = new Size(0, 25);
            panel1.Name = "panel1";
            panel1.Size = new Size(110, 25);
            panel1.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Right;
            label1.Location = new Point(68, 0);
            label1.MinimumSize = new Size(0, 25);
            label1.Name = "label1";
            label1.Size = new Size(42, 25);
            label1.TabIndex = 5;
            label1.Text = "#feeds";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnGetFeeds
            // 
            btnGetFeeds.AutoSize = true;
            btnGetFeeds.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGetFeeds.Dock = DockStyle.Left;
            btnGetFeeds.Location = new Point(0, 0);
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
            // btnDelete
            // 
            btnDelete.AutoSize = true;
            btnDelete.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDelete.Location = new Point(3, 158);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(125, 25);
            btnDelete.TabIndex = 7;
            btnDelete.Text = "Delete Selected Feed";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
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
            Property_History.ResumeLayout(false);
            Property_History.PerformLayout();
            Property_Date.ResumeLayout(false);
            Property_Date.PerformLayout();
            Property_Expr.ResumeLayout(false);
            Property_Expr.PerformLayout();
            Property_URL.ResumeLayout(false);
            Property_URL.PerformLayout();
            Property_Title.ResumeLayout(false);
            Property_Title.PerformLayout();
            Property_ID.ResumeLayout(false);
            Property_ID.PerformLayout();
            ButtonPanelFlowLayout.ResumeLayout(false);
            ButtonPanelFlowLayout.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
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
        private Label lblDate;
        private Label lblExpr;
        private Label lblURL;
        private Label lblTitle;
        private Label LblId;
        private TextBox txtBoxHistory;
        private Label lblHistory;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtBoxID;
        private TextBox txtBoxTitle;
        private TextBox txtBoxURL;
        private TextBox txtBoxDate;
        private TextBox txtBoxRegex;
        private Panel Property_ID;
        private Panel Property_URL;
        private Panel Property_Title;
        private Panel Property_History;
        private Panel Property_Date;
        private Panel Property_Expr;
        private Panel panel1;
        private Label label1;
        private Button btnDelete;
    }
}
