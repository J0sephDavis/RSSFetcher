﻿namespace Steward_Zhou
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
            txtBoxHistory = new TextBox();
            lblHistory = new Label();
            txtBoxDate = new TextBox();
            lblDate = new Label();
            txtBoxRegex = new TextBox();
            lblExpr = new Label();
            txtBoxURL = new TextBox();
            lblURL = new Label();
            txtBoxTitle = new TextBox();
            lblTitle = new Label();
            txtBoxID = new TextBox();
            LblId = new Label();
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
            RightMainSplit.Panel1.Controls.Add(txtBoxHistory);
            RightMainSplit.Panel1.Controls.Add(lblHistory);
            RightMainSplit.Panel1.Controls.Add(txtBoxDate);
            RightMainSplit.Panel1.Controls.Add(lblDate);
            RightMainSplit.Panel1.Controls.Add(txtBoxRegex);
            RightMainSplit.Panel1.Controls.Add(lblExpr);
            RightMainSplit.Panel1.Controls.Add(txtBoxURL);
            RightMainSplit.Panel1.Controls.Add(lblURL);
            RightMainSplit.Panel1.Controls.Add(txtBoxTitle);
            RightMainSplit.Panel1.Controls.Add(lblTitle);
            RightMainSplit.Panel1.Controls.Add(txtBoxID);
            RightMainSplit.Panel1.Controls.Add(LblId);
            RightMainSplit.Panel1.Controls.Add(ButtonPanelFlowLayout);
            // 
            // RightMainSplit.Panel2
            // 
            RightMainSplit.Panel2.Controls.Add(LogListBox);
            RightMainSplit.Size = new Size(530, 428);
            RightMainSplit.SplitterDistance = 252;
            RightMainSplit.TabIndex = 0;
            // 
            // txtBoxHistory
            // 
            txtBoxHistory.Location = new Point(226, 178);
            txtBoxHistory.Name = "txtBoxHistory";
            txtBoxHistory.Size = new Size(212, 23);
            txtBoxHistory.TabIndex = 20;
            // 
            // lblHistory
            // 
            lblHistory.AutoSize = true;
            lblHistory.Location = new Point(175, 181);
            lblHistory.Name = "lblHistory";
            lblHistory.Size = new Size(45, 15);
            lblHistory.TabIndex = 19;
            lblHistory.Text = "History";
            // 
            // txtBoxDate
            // 
            txtBoxDate.Location = new Point(226, 149);
            txtBoxDate.Name = "txtBoxDate";
            txtBoxDate.Size = new Size(212, 23);
            txtBoxDate.TabIndex = 18;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(189, 152);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(31, 15);
            lblDate.TabIndex = 17;
            lblDate.Text = "Date";
            // 
            // txtBoxRegex
            // 
            txtBoxRegex.Location = new Point(226, 120);
            txtBoxRegex.Name = "txtBoxRegex";
            txtBoxRegex.Size = new Size(212, 23);
            txtBoxRegex.TabIndex = 16;
            // 
            // lblExpr
            // 
            lblExpr.AutoSize = true;
            lblExpr.Location = new Point(182, 125);
            lblExpr.Name = "lblExpr";
            lblExpr.Size = new Size(39, 15);
            lblExpr.TabIndex = 15;
            lblExpr.Text = "Regex";
            // 
            // txtBoxURL
            // 
            txtBoxURL.Location = new Point(226, 91);
            txtBoxURL.Name = "txtBoxURL";
            txtBoxURL.Size = new Size(212, 23);
            txtBoxURL.TabIndex = 14;
            // 
            // lblURL
            // 
            lblURL.AutoSize = true;
            lblURL.Location = new Point(192, 94);
            lblURL.Name = "lblURL";
            lblURL.Size = new Size(28, 15);
            lblURL.TabIndex = 13;
            lblURL.Text = "URL";
            // 
            // txtBoxTitle
            // 
            txtBoxTitle.Location = new Point(226, 62);
            txtBoxTitle.Name = "txtBoxTitle";
            txtBoxTitle.Size = new Size(212, 23);
            txtBoxTitle.TabIndex = 12;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(191, 65);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(29, 15);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "Title";
            // 
            // txtBoxID
            // 
            txtBoxID.Enabled = false;
            txtBoxID.Location = new Point(226, 34);
            txtBoxID.Name = "txtBoxID";
            txtBoxID.Size = new Size(212, 23);
            txtBoxID.TabIndex = 10;
            // 
            // LblId
            // 
            LblId.AutoSize = true;
            LblId.Location = new Point(202, 37);
            LblId.Name = "LblId";
            LblId.Size = new Size(18, 15);
            LblId.TabIndex = 6;
            LblId.Text = "ID";
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
    }
}
