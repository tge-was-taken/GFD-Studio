using AtlusGfdEditor.GUI.Adapters;

namespace AtlusGfdEditor.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTreeView = new AtlusGfdEditor.GUI.Adapters.TreeNodeAdapterView();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mContentPanel = new System.Windows.Forms.Panel();
            this.mMainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMainMenuStrip
            // 
            this.mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mEditToolStripMenuItem});
            this.mMainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMainMenuStrip.Name = "mMainMenuStrip";
            this.mMainMenuStrip.Size = new System.Drawing.Size(1222, 24);
            this.mMainMenuStrip.TabIndex = 0;
            this.mMainMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOpenToolStripMenuItem});
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.mFileToolStripMenuItem.Text = "File";
            // 
            // mOpenToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClickEventHandler);
            // 
            // mEditToolStripMenuItem
            // 
            this.mEditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mOptionsToolStripMenuItem});
            this.mEditToolStripMenuItem.Name = "mEditToolStripMenuItem";
            this.mEditToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.mEditToolStripMenuItem.Text = "Edit";
            // 
            // mOptionsToolStripMenuItem
            // 
            this.mOptionsToolStripMenuItem.Name = "mOptionsToolStripMenuItem";
            this.mOptionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.mOptionsToolStripMenuItem.Text = "Options";
            // 
            // mTreeView
            // 
            this.mTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mTreeView.Location = new System.Drawing.Point(713, 27);
            this.mTreeView.Name = "mTreeView";
            this.mTreeView.Size = new System.Drawing.Size(497, 380);
            this.mTreeView.TabIndex = 1;
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.mPropertyGrid.Location = new System.Drawing.Point(713, 413);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.Size = new System.Drawing.Size(497, 414);
            this.mPropertyGrid.TabIndex = 2;
            // 
            // mContentPanel
            // 
            this.mContentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentPanel.Location = new System.Drawing.Point(12, 28);
            this.mContentPanel.Name = "mContentPanel";
            this.mContentPanel.Size = new System.Drawing.Size(695, 800);
            this.mContentPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1222, 839);
            this.Controls.Add(this.mPropertyGrid);
            this.Controls.Add(this.mTreeView);
            this.Controls.Add(this.mMainMenuStrip);
            this.Controls.Add(this.mContentPanel);
            this.MainMenuStrip = this.mMainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Atlus Gfd Editor";
            this.mMainMenuStrip.ResumeLayout(false);
            this.mMainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mFileToolStripMenuItem;
        private TreeNodeAdapterView mTreeView;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem mOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mEditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOptionsToolStripMenuItem;
        private System.Windows.Forms.Panel mContentPanel;
    }
}