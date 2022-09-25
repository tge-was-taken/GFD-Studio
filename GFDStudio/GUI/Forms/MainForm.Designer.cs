using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.GUI.Forms
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeRelativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertAnimationsToP5InDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertMaterialInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retainTexNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thereIsNoHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mContentPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mModelEditorTreeView = new GFDStudio.GUI.DataViewNodes.DataTreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mAnimationListTreeView = new GFDStudio.GUI.DataViewNodes.DataTreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mAnimationStopButton = new System.Windows.Forms.Button();
            this.mAnimationPlaybackButton = new System.Windows.Forms.Button();
            this.mAnimationTrackBar = new System.Windows.Forms.TrackBar();
            this.mMainMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mAnimationTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // mMainMenuStrip
            // 
            this.mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.animationToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mMainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMainMenuStrip.Name = "mMainMenuStrip";
            this.mMainMenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.mMainMenuStrip.Size = new System.Drawing.Size(1461, 24);
            this.mMainMenuStrip.TabIndex = 0;
            this.mMainMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.mOpenToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.mFileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // modelToolStripMenuItem
            // 
            this.modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            this.modelToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.modelToolStripMenuItem.Text = "Model";
            this.modelToolStripMenuItem.Click += new System.EventHandler(this.HandleNewModelToolStripMenuItemClick);
            // 
            // mOpenToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            this.mOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler(this.HandleOpenToolStripMenuItemClick);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.HandleSaveToolStripMenuItemClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.HandleSaveAsToolStripMenuItemClick);
            // 
            // animationToolStripMenuItem
            // 
            this.animationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadExternalToolStripMenuItem});
            this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            this.animationToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.animationToolStripMenuItem.Text = "Animation";
            // 
            // loadExternalToolStripMenuItem
            // 
            this.loadExternalToolStripMenuItem.Name = "loadExternalToolStripMenuItem";
            this.loadExternalToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadExternalToolStripMenuItem.Text = "Load";
            this.loadExternalToolStripMenuItem.Click += new System.EventHandler(this.HandleAnimationLoadExternalToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeRelativeToolStripMenuItem,
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem,
            this.convertAnimationsToP5InDirectoryToolStripMenuItem,
            this.convertMaterialInDirectoryToolStripMenuItem,
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // makeRelativeToolStripMenuItem
            // 
            this.makeRelativeToolStripMenuItem.Name = "makeRelativeToolStripMenuItem";
            this.makeRelativeToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.makeRelativeToolStripMenuItem.Text = "Retarget animation packs in directory";
            this.makeRelativeToolStripMenuItem.Click += new System.EventHandler(this.HandleRetargetAnimationsToolStripMenuItemClick);
            // 
            // rescaleAnimationPacksInDirectoryToolStripMenuItem
            // 
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem.Name = "rescaleAnimationPacksInDirectoryToolStripMenuItem";
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem.Text = "Rescale/Reposition animation packs in directory";
            this.rescaleAnimationPacksInDirectoryToolStripMenuItem.Click += new System.EventHandler(this.HandleRescaleAnimationsToolStripMenuItemClick);
            // 
            // convertAnimationsToP5InDirectoryToolStripMenuItem
            // 
            this.convertAnimationsToP5InDirectoryToolStripMenuItem.Name = "convertAnimationsToP5InDirectoryToolStripMenuItem";
            this.convertAnimationsToP5InDirectoryToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.convertAnimationsToP5InDirectoryToolStripMenuItem.Text = "Convert P5R animations to P5 in directory";
            this.convertAnimationsToP5InDirectoryToolStripMenuItem.Click += new System.EventHandler(this.HandleConvertAnimationsToolStripMenuItemClick);
            // 
            // convertMaterialInDirectoryToolStripMenuItem
            // 
            this.convertMaterialInDirectoryToolStripMenuItem.Name = "convertMaterialInDirectoryToolStripMenuItem";
            this.convertMaterialInDirectoryToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.convertMaterialInDirectoryToolStripMenuItem.Text = "Convert model materials in directory";
            this.convertMaterialInDirectoryToolStripMenuItem.Click += new System.EventHandler(this.HandleConvertMaterialsToolStripMenuItemClick);
            // 
            // copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem
            // 
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Name = "copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem";
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Text = "Copy P5 Split GAP to multiple models in directory";
            this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Click += new System.EventHandler(this.copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.retainTexNameToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // retainTexNameToolStripMenuItem
            // 
            this.retainTexNameToolStripMenuItem.Checked = true;
            this.retainTexNameToolStripMenuItem.CheckOnClick = true;
            this.retainTexNameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.retainTexNameToolStripMenuItem.Name = "retainTexNameToolStripMenuItem";
            this.retainTexNameToolStripMenuItem.Size = new System.Drawing.Size(366, 22);
            this.retainTexNameToolStripMenuItem.Text = "Retain original material\'s texture names when replacing";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thereIsNoHelpToolStripMenuItem,
            this.perishToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // thereIsNoHelpToolStripMenuItem
            // 
            this.thereIsNoHelpToolStripMenuItem.Enabled = false;
            this.thereIsNoHelpToolStripMenuItem.Name = "thereIsNoHelpToolStripMenuItem";
            this.thereIsNoHelpToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.thereIsNoHelpToolStripMenuItem.Text = "There is no help.";
            // 
            // perishToolStripMenuItem
            // 
            this.perishToolStripMenuItem.Enabled = false;
            this.perishToolStripMenuItem.Name = "perishToolStripMenuItem";
            this.perishToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.perishToolStripMenuItem.Text = "Perish.";
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.mPropertyGrid.Location = new System.Drawing.Point(867, 513);
            this.mPropertyGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.mPropertyGrid.Size = new System.Drawing.Size(580, 337);
            this.mPropertyGrid.TabIndex = 2;
            // 
            // mContentPanel
            // 
            this.mContentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentPanel.Location = new System.Drawing.Point(14, 32);
            this.mContentPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mContentPanel.Name = "mContentPanel";
            this.mContentPanel.Size = new System.Drawing.Size(846, 770);
            this.mContentPanel.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(867, 32);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(580, 474);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.mModelEditorTreeView);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(572, 446);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Model Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mModelEditorTreeView
            // 
            this.mModelEditorTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mModelEditorTreeView.ImageIndex = 0;
            this.mModelEditorTreeView.Location = new System.Drawing.Point(7, 7);
            this.mModelEditorTreeView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mModelEditorTreeView.Name = "mModelEditorTreeView";
            this.mModelEditorTreeView.SelectedImageIndex = 0;
            this.mModelEditorTreeView.Size = new System.Drawing.Size(556, 430);
            this.mModelEditorTreeView.TabIndex = 1;
            this.mModelEditorTreeView.TopNode = null;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mAnimationListTreeView);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(572, 446);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Animation List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // mAnimationListTreeView
            // 
            this.mAnimationListTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAnimationListTreeView.ImageIndex = 0;
            this.mAnimationListTreeView.Location = new System.Drawing.Point(7, 7);
            this.mAnimationListTreeView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mAnimationListTreeView.Name = "mAnimationListTreeView";
            this.mAnimationListTreeView.SelectedImageIndex = 0;
            this.mAnimationListTreeView.Size = new System.Drawing.Size(556, 430);
            this.mAnimationListTreeView.TabIndex = 2;
            this.mAnimationListTreeView.TopNode = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 164F));
            this.tableLayoutPanel1.Controls.Add(this.mAnimationStopButton, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.mAnimationPlaybackButton, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.mAnimationTrackBar, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 810);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(846, 40);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // mAnimationStopButton
            // 
            this.mAnimationStopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mAnimationStopButton.Location = new System.Drawing.Point(686, 3);
            this.mAnimationStopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mAnimationStopButton.Name = "mAnimationStopButton";
            this.mAnimationStopButton.Size = new System.Drawing.Size(156, 34);
            this.mAnimationStopButton.TabIndex = 2;
            this.mAnimationStopButton.Text = "Stop";
            this.mAnimationStopButton.UseVisualStyleBackColor = true;
            this.mAnimationStopButton.Click += new System.EventHandler(this.HandleAnimationStopButtonClick);
            // 
            // mAnimationPlaybackButton
            // 
            this.mAnimationPlaybackButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mAnimationPlaybackButton.Location = new System.Drawing.Point(604, 3);
            this.mAnimationPlaybackButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mAnimationPlaybackButton.Name = "mAnimationPlaybackButton";
            this.mAnimationPlaybackButton.Size = new System.Drawing.Size(74, 34);
            this.mAnimationPlaybackButton.TabIndex = 0;
            this.mAnimationPlaybackButton.Text = "Play ";
            this.mAnimationPlaybackButton.UseVisualStyleBackColor = true;
            // 
            // mAnimationTrackBar
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.mAnimationTrackBar, 6);
            this.mAnimationTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mAnimationTrackBar.Location = new System.Drawing.Point(4, 3);
            this.mAnimationTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mAnimationTrackBar.Name = "mAnimationTrackBar";
            this.mAnimationTrackBar.Size = new System.Drawing.Size(592, 34);
            this.mAnimationTrackBar.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1461, 864);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mPropertyGrid);
            this.Controls.Add(this.mMainMenuStrip);
            this.Controls.Add(this.mContentPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mMainMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "GFD Studio";
            this.mMainMenuStrip.ResumeLayout(false);
            this.mMainMenuStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mAnimationTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GFDStudio.GUI.DataViewNodes.DataTreeView mModelEditorTreeView;
        private System.Windows.Forms.MenuStrip mMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mFileToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem mOpenToolStripMenuItem;
        private System.Windows.Forms.Panel mContentPanel;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeRelativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadExternalToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button mAnimationPlaybackButton;
        private System.Windows.Forms.TrackBar mAnimationTrackBar;
        private DataTreeView mAnimationListTreeView;
        private System.Windows.Forms.Button mAnimationStopButton;
        private System.Windows.Forms.ToolStripMenuItem rescaleAnimationPacksInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertAnimationsToP5InDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertMaterialInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thereIsNoHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perishToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem retainTexNameToolStripMenuItem;
    }
}