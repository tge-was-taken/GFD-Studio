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
            this.m_MainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.m_FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTreeView = new System.Windows.Forms.TreeView();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.m_Panel = new System.Windows.Forms.Panel();
            this.m_MainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_MainMenuStrip
            // 
            this.m_MainMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.m_FileToolStripMenuItem,
            this.m_EditToolStripMenuItem} );
            this.m_MainMenuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.m_MainMenuStrip.Name = "m_MainMenuStrip";
            this.m_MainMenuStrip.Size = new System.Drawing.Size( 1222, 24 );
            this.m_MainMenuStrip.TabIndex = 0;
            this.m_MainMenuStrip.Text = "menuStrip1";
            // 
            // m_FileToolStripMenuItem
            // 
            this.m_FileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.m_OpenToolStripMenuItem} );
            this.m_FileToolStripMenuItem.Name = "m_FileToolStripMenuItem";
            this.m_FileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.m_FileToolStripMenuItem.Text = "File";
            // 
            // m_OpenToolStripMenuItem
            // 
            this.m_OpenToolStripMenuItem.Name = "m_OpenToolStripMenuItem";
            this.m_OpenToolStripMenuItem.Size = new System.Drawing.Size( 103, 22 );
            this.m_OpenToolStripMenuItem.Text = "Open";
            this.m_OpenToolStripMenuItem.Click += new System.EventHandler( this.OpenToolStripMenuItemClickEventHandler );
            // 
            // m_EditToolStripMenuItem
            // 
            this.m_EditToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.m_OptionsToolStripMenuItem} );
            this.m_EditToolStripMenuItem.Name = "m_EditToolStripMenuItem";
            this.m_EditToolStripMenuItem.Size = new System.Drawing.Size( 39, 20 );
            this.m_EditToolStripMenuItem.Text = "Edit";
            // 
            // m_OptionsToolStripMenuItem
            // 
            this.m_OptionsToolStripMenuItem.Name = "m_OptionsToolStripMenuItem";
            this.m_OptionsToolStripMenuItem.Size = new System.Drawing.Size( 116, 22 );
            this.m_OptionsToolStripMenuItem.Text = "Options";
            // 
            // m_TreeView
            // 
            this.mTreeView.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mTreeView.Location = new System.Drawing.Point( 898, 27 );
            this.mTreeView.Name = "m_TreeView";
            this.mTreeView.Size = new System.Drawing.Size( 312, 380 );
            this.mTreeView.TabIndex = 1;
            // 
            // m_PropertyGrid
            // 
            this.mPropertyGrid.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mPropertyGrid.Location = new System.Drawing.Point( 898, 413 );
            this.mPropertyGrid.Name = "m_PropertyGrid";
            this.mPropertyGrid.Size = new System.Drawing.Size( 312, 414 );
            this.mPropertyGrid.TabIndex = 2;
            // 
            // m_Panel
            // 
            this.m_Panel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.m_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_Panel.Location = new System.Drawing.Point( 12, 28 );
            this.m_Panel.Name = "m_Panel";
            this.m_Panel.Size = new System.Drawing.Size( 880, 800 );
            this.m_Panel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 1222, 839 );
            this.Controls.Add( this.mPropertyGrid );
            this.Controls.Add( this.mTreeView );
            this.Controls.Add( this.m_MainMenuStrip );
            this.Controls.Add( this.m_Panel );
            this.MainMenuStrip = this.m_MainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Atlus GFD Editor";
            this.m_MainMenuStrip.ResumeLayout( false );
            this.m_MainMenuStrip.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip m_MainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem m_FileToolStripMenuItem;
        private System.Windows.Forms.TreeView mTreeView;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem m_OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_OptionsToolStripMenuItem;
        private System.Windows.Forms.Panel m_Panel;
    }
}