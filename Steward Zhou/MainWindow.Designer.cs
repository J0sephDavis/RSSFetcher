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
            FeedTree = new TreeView();
            LogListBox = new ListBox();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // FeedTree
            // 
            FeedTree.Dock = DockStyle.Fill;
            FeedTree.Location = new Point(0, 0);
            FeedTree.Name = "FeedTree";
            FeedTree.Size = new Size(800, 225);
            FeedTree.TabIndex = 0;
            // 
            // LogListBox
            // 
            LogListBox.Dock = DockStyle.Fill;
            LogListBox.FormattingEnabled = true;
            LogListBox.HorizontalScrollbar = true;
            LogListBox.ItemHeight = 15;
            LogListBox.Location = new Point(0, 0);
            LogListBox.Name = "LogListBox";
            LogListBox.Size = new Size(800, 221);
            LogListBox.TabIndex = 1;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(FeedTree);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(LogListBox);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 225;
            splitContainer1.TabIndex = 2;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "MainWindow";
            Text = "MainWindow";
            Load += MainWindow_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TreeView FeedTree;
        private ListBox LogListBox;
        private SplitContainer splitContainer1;
    }
}
