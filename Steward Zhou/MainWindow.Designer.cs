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
            SuspendLayout();
            // 
            // FeedTree
            // 
            FeedTree.Location = new Point(12, 12);
            FeedTree.Name = "FeedTree";
            FeedTree.Size = new Size(341, 426);
            FeedTree.TabIndex = 0;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(FeedTree);
            Name = "MainWindow";
            Text = "MainWindow";
            Load += MainWindow_Load;
            ResumeLayout(false);
        }

        #endregion

        private TreeView FeedTree;
    }
}
