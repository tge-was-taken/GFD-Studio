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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            modelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            makeRelativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            rescaleAnimationPacksInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertAnimationsToP5InDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertMaterialInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MassExportTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MassReplaceTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            retainTexNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            thereIsNoHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            perishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mContentPanel = new System.Windows.Forms.Panel();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            mModelEditorTreeView = new DataTreeView();
            tabPage2 = new System.Windows.Forms.TabPage();
            mAnimationListTreeView = new DataTreeView();
            tableLayoutPanel_AnimationControls = new System.Windows.Forms.TableLayoutPanel();
            mAnimationStopButton = new System.Windows.Forms.Button();
            mAnimationPlaybackButton = new System.Windows.Forms.Button();
            mAnimationTrackBar = new System.Windows.Forms.TrackBar();
            splitContainer_Main = new System.Windows.Forms.SplitContainer();
            tableLayoutPanel_Leftside = new System.Windows.Forms.TableLayoutPanel();
            splitContainer_RightSide = new System.Windows.Forms.SplitContainer();
            panel_PropertyGridContainer = new System.Windows.Forms.Panel();
            mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            mMainMenuStrip.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel_AnimationControls.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) mAnimationTrackBar  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) splitContainer_Main  ).BeginInit();
            splitContainer_Main.Panel1.SuspendLayout();
            splitContainer_Main.Panel2.SuspendLayout();
            splitContainer_Main.SuspendLayout();
            tableLayoutPanel_Leftside.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) splitContainer_RightSide  ).BeginInit();
            splitContainer_RightSide.Panel1.SuspendLayout();
            splitContainer_RightSide.Panel2.SuspendLayout();
            splitContainer_RightSide.SuspendLayout();
            panel_PropertyGridContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mMainMenuStrip
            // 
            mMainMenuStrip.ImageScalingSize = new System.Drawing.Size( 20, 20 );
            mMainMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] { mFileToolStripMenuItem, animationToolStripMenuItem, toolsToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem } );
            mMainMenuStrip.Location = new System.Drawing.Point( 0, 0 );
            mMainMenuStrip.Name = "mMainMenuStrip";
            mMainMenuStrip.Padding = new System.Windows.Forms.Padding( 8, 3, 0, 3 );
            mMainMenuStrip.Size = new System.Drawing.Size( 1182, 30 );
            mMainMenuStrip.TabIndex = 0;
            mMainMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            mFileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, mOpenToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem } );
            mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            mFileToolStripMenuItem.Size = new System.Drawing.Size( 46, 24 );
            mFileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { modelToolStripMenuItem } );
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            newToolStripMenuItem.Text = "New";
            // 
            // modelToolStripMenuItem
            // 
            modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            modelToolStripMenuItem.Size = new System.Drawing.Size( 135, 26 );
            modelToolStripMenuItem.Text = "Model";
            modelToolStripMenuItem.Click += HandleNewModelToolStripMenuItemClick;
            // 
            // mOpenToolStripMenuItem
            // 
            mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            mOpenToolStripMenuItem.ShortcutKeys =   System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.O  ;
            mOpenToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            mOpenToolStripMenuItem.Text = "Open";
            mOpenToolStripMenuItem.Click += HandleOpenToolStripMenuItemClick;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys =   System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.S  ;
            saveToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += HandleSaveToolStripMenuItemClick;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys =    System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.Shift   |  System.Windows.Forms.Keys.S  ;
            saveAsToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += HandleSaveAsToolStripMenuItemClick;
            // 
            // animationToolStripMenuItem
            // 
            animationToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { loadExternalToolStripMenuItem } );
            animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            animationToolStripMenuItem.Size = new System.Drawing.Size( 92, 24 );
            animationToolStripMenuItem.Text = "Animation";
            // 
            // loadExternalToolStripMenuItem
            // 
            loadExternalToolStripMenuItem.Name = "loadExternalToolStripMenuItem";
            loadExternalToolStripMenuItem.Size = new System.Drawing.Size( 125, 26 );
            loadExternalToolStripMenuItem.Text = "Load";
            loadExternalToolStripMenuItem.Click += HandleAnimationLoadExternalToolStripMenuItemClick;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { makeRelativeToolStripMenuItem, rescaleAnimationPacksInDirectoryToolStripMenuItem, convertAnimationsToP5InDirectoryToolStripMenuItem, convertMaterialInDirectoryToolStripMenuItem, copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem, MassExportTexturesToolStripMenuItem, MassReplaceTexturesToolStripMenuItem } );
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size( 58, 24 );
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // makeRelativeToolStripMenuItem
            // 
            makeRelativeToolStripMenuItem.Name = "makeRelativeToolStripMenuItem";
            makeRelativeToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            makeRelativeToolStripMenuItem.Text = "Retarget animation packs in directory";
            makeRelativeToolStripMenuItem.Click += HandleRetargetAnimationsToolStripMenuItemClick;
            // 
            // rescaleAnimationPacksInDirectoryToolStripMenuItem
            // 
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Name = "rescaleAnimationPacksInDirectoryToolStripMenuItem";
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Text = "Rescale/Reposition animation packs in directory";
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Click += HandleRescaleAnimationsToolStripMenuItemClick;
            // 
            // convertAnimationsToP5InDirectoryToolStripMenuItem
            // 
            convertAnimationsToP5InDirectoryToolStripMenuItem.Name = "convertAnimationsToP5InDirectoryToolStripMenuItem";
            convertAnimationsToP5InDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            convertAnimationsToP5InDirectoryToolStripMenuItem.Text = "Convert P5R animations to P5 in directory";
            convertAnimationsToP5InDirectoryToolStripMenuItem.Click += HandleConvertAnimationsToolStripMenuItemClick;
            // 
            // convertMaterialInDirectoryToolStripMenuItem
            // 
            convertMaterialInDirectoryToolStripMenuItem.Name = "convertMaterialInDirectoryToolStripMenuItem";
            convertMaterialInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            convertMaterialInDirectoryToolStripMenuItem.Text = "Convert model materials in directory";
            convertMaterialInDirectoryToolStripMenuItem.Click += HandleConvertMaterialsToolStripMenuItemClick;
            // 
            // copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem
            // 
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Name = "copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem";
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Text = "Copy P5 Split GAP to multiple models in directory";
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Click += copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem_Click;
            // 
            // MassExportTexturesToolStripMenuItem
            // 
            MassExportTexturesToolStripMenuItem.Name = "MassExportTexturesToolStripMenuItem";
            MassExportTexturesToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            MassExportTexturesToolStripMenuItem.Text = "Mass Export Textures from Models";
            MassExportTexturesToolStripMenuItem.Click += MassExportTexturesToolStripMenuItem_Click;
            // 
            // MassReplaceTexturesToolStripMenuItem
            // 
            MassReplaceTexturesToolStripMenuItem.Name = "MassReplaceTexturesToolStripMenuItem";
            MassReplaceTexturesToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            MassReplaceTexturesToolStripMenuItem.Text = "Mass Replace Textures in Models";
            MassReplaceTexturesToolStripMenuItem.Click += MassReplaceTexturesToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { retainTexNameToolStripMenuItem } );
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size( 75, 24 );
            optionsToolStripMenuItem.Text = "Options";
            // 
            // retainTexNameToolStripMenuItem
            // 
            retainTexNameToolStripMenuItem.Checked = true;
            retainTexNameToolStripMenuItem.CheckOnClick = true;
            retainTexNameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            retainTexNameToolStripMenuItem.Name = "retainTexNameToolStripMenuItem";
            retainTexNameToolStripMenuItem.Size = new System.Drawing.Size( 459, 26 );
            retainTexNameToolStripMenuItem.Text = "Retain original material's texture names when replacing";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { thereIsNoHelpToolStripMenuItem, perishToolStripMenuItem } );
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size( 55, 24 );
            helpToolStripMenuItem.Text = "Help";
            // 
            // thereIsNoHelpToolStripMenuItem
            // 
            thereIsNoHelpToolStripMenuItem.Enabled = false;
            thereIsNoHelpToolStripMenuItem.Name = "thereIsNoHelpToolStripMenuItem";
            thereIsNoHelpToolStripMenuItem.Size = new System.Drawing.Size( 200, 26 );
            thereIsNoHelpToolStripMenuItem.Text = "There is no help.";
            // 
            // perishToolStripMenuItem
            // 
            perishToolStripMenuItem.Enabled = false;
            perishToolStripMenuItem.Name = "perishToolStripMenuItem";
            perishToolStripMenuItem.Size = new System.Drawing.Size( 200, 26 );
            perishToolStripMenuItem.Text = "Perish.";
            // 
            // mContentPanel
            // 
            mContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mContentPanel.Location = new System.Drawing.Point( 5, 4 );
            mContentPanel.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mContentPanel.Name = "mContentPanel";
            mContentPanel.Size = new System.Drawing.Size( 599, 749 );
            mContentPanel.TabIndex = 3;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add( tabPage1 );
            tabControl1.Controls.Add( tabPage2 );
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point( 0, 0 );
            tabControl1.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size( 569, 360 );
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add( mModelEditorTreeView );
            tabPage1.Location = new System.Drawing.Point( 4, 29 );
            tabPage1.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage1.Size = new System.Drawing.Size( 561, 327 );
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Model Editor";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // mModelEditorTreeView
            // 
            mModelEditorTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            mModelEditorTreeView.ImageIndex = 0;
            mModelEditorTreeView.Location = new System.Drawing.Point( 5, 4 );
            mModelEditorTreeView.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mModelEditorTreeView.Name = "mModelEditorTreeView";
            mModelEditorTreeView.SelectedImageIndex = 0;
            mModelEditorTreeView.Size = new System.Drawing.Size( 551, 319 );
            mModelEditorTreeView.TabIndex = 1;
            mModelEditorTreeView.TopNode = null;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add( mAnimationListTreeView );
            tabPage2.Location = new System.Drawing.Point( 4, 29 );
            tabPage2.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage2.Size = new System.Drawing.Size( 448, 202 );
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Animation List";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // mAnimationListTreeView
            // 
            mAnimationListTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationListTreeView.ImageIndex = 0;
            mAnimationListTreeView.Location = new System.Drawing.Point( 5, 4 );
            mAnimationListTreeView.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationListTreeView.Name = "mAnimationListTreeView";
            mAnimationListTreeView.SelectedImageIndex = 0;
            mAnimationListTreeView.Size = new System.Drawing.Size( 438, 194 );
            mAnimationListTreeView.TabIndex = 2;
            mAnimationListTreeView.TopNode = null;
            // 
            // tableLayoutPanel_AnimationControls
            // 
            tableLayoutPanel_AnimationControls.ColumnCount = 8;
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 16.66667F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Absolute, 94F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Absolute, 187F ) );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationStopButton, 7, 0 );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationPlaybackButton, 6, 0 );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationTrackBar, 0, 0 );
            tableLayoutPanel_AnimationControls.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_AnimationControls.Location = new System.Drawing.Point( 5, 761 );
            tableLayoutPanel_AnimationControls.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tableLayoutPanel_AnimationControls.Name = "tableLayoutPanel_AnimationControls";
            tableLayoutPanel_AnimationControls.RowCount = 1;
            tableLayoutPanel_AnimationControls.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            tableLayoutPanel_AnimationControls.Size = new System.Drawing.Size( 599, 58 );
            tableLayoutPanel_AnimationControls.TabIndex = 0;
            // 
            // mAnimationStopButton
            // 
            mAnimationStopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationStopButton.Location = new System.Drawing.Point( 411, 4 );
            mAnimationStopButton.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationStopButton.Name = "mAnimationStopButton";
            mAnimationStopButton.Size = new System.Drawing.Size( 183, 50 );
            mAnimationStopButton.TabIndex = 2;
            mAnimationStopButton.Text = "Stop";
            mAnimationStopButton.UseVisualStyleBackColor = true;
            mAnimationStopButton.Click += HandleAnimationStopButtonClick;
            // 
            // mAnimationPlaybackButton
            // 
            mAnimationPlaybackButton.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationPlaybackButton.Location = new System.Drawing.Point( 317, 4 );
            mAnimationPlaybackButton.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationPlaybackButton.Name = "mAnimationPlaybackButton";
            mAnimationPlaybackButton.Size = new System.Drawing.Size( 84, 50 );
            mAnimationPlaybackButton.TabIndex = 0;
            mAnimationPlaybackButton.Text = "Play ";
            mAnimationPlaybackButton.UseVisualStyleBackColor = true;
            // 
            // mAnimationTrackBar
            // 
            tableLayoutPanel_AnimationControls.SetColumnSpan( mAnimationTrackBar, 6 );
            mAnimationTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationTrackBar.Location = new System.Drawing.Point( 5, 4 );
            mAnimationTrackBar.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationTrackBar.Name = "mAnimationTrackBar";
            mAnimationTrackBar.Size = new System.Drawing.Size( 302, 50 );
            mAnimationTrackBar.TabIndex = 1;
            // 
            // splitContainer_Main
            // 
            splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer_Main.Location = new System.Drawing.Point( 0, 30 );
            splitContainer_Main.Name = "splitContainer_Main";
            // 
            // splitContainer_Main.Panel1
            // 
            splitContainer_Main.Panel1.Controls.Add( tableLayoutPanel_Leftside );
            // 
            // splitContainer_Main.Panel2
            // 
            splitContainer_Main.Panel2.Controls.Add( splitContainer_RightSide );
            splitContainer_Main.Size = new System.Drawing.Size( 1182, 823 );
            splitContainer_Main.SplitterDistance = 609;
            splitContainer_Main.TabIndex = 5;
            // 
            // tableLayoutPanel_Leftside
            // 
            tableLayoutPanel_Leftside.ColumnCount = 1;
            tableLayoutPanel_Leftside.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            tableLayoutPanel_Leftside.Controls.Add( tableLayoutPanel_AnimationControls, 0, 1 );
            tableLayoutPanel_Leftside.Controls.Add( mContentPanel, 0, 0 );
            tableLayoutPanel_Leftside.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_Leftside.Location = new System.Drawing.Point( 0, 0 );
            tableLayoutPanel_Leftside.Name = "tableLayoutPanel_Leftside";
            tableLayoutPanel_Leftside.RowCount = 2;
            tableLayoutPanel_Leftside.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 92F ) );
            tableLayoutPanel_Leftside.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 8F ) );
            tableLayoutPanel_Leftside.Size = new System.Drawing.Size( 609, 823 );
            tableLayoutPanel_Leftside.TabIndex = 6;
            // 
            // splitContainer_RightSide
            // 
            splitContainer_RightSide.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer_RightSide.Location = new System.Drawing.Point( 0, 0 );
            splitContainer_RightSide.Name = "splitContainer_RightSide";
            splitContainer_RightSide.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_RightSide.Panel1
            // 
            splitContainer_RightSide.Panel1.Controls.Add( tabControl1 );
            // 
            // splitContainer_RightSide.Panel2
            // 
            splitContainer_RightSide.Panel2.Controls.Add( panel_PropertyGridContainer );
            splitContainer_RightSide.Size = new System.Drawing.Size( 569, 823 );
            splitContainer_RightSide.SplitterDistance = 360;
            splitContainer_RightSide.TabIndex = 0;
            // 
            // panel_PropertyGridContainer
            // 
            panel_PropertyGridContainer.Controls.Add( mPropertyGrid );
            panel_PropertyGridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            panel_PropertyGridContainer.Location = new System.Drawing.Point( 0, 0 );
            panel_PropertyGridContainer.Margin = new System.Windows.Forms.Padding( 25 );
            panel_PropertyGridContainer.Name = "panel_PropertyGridContainer";
            panel_PropertyGridContainer.Padding = new System.Windows.Forms.Padding( 0, 0, 5, 5 );
            panel_PropertyGridContainer.Size = new System.Drawing.Size( 569, 459 );
            panel_PropertyGridContainer.TabIndex = 0;
            // 
            // mPropertyGrid
            // 
            mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            mPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            mPropertyGrid.Location = new System.Drawing.Point( 0, 0 );
            mPropertyGrid.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mPropertyGrid.Name = "mPropertyGrid";
            mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            mPropertyGrid.Size = new System.Drawing.Size( 564, 454 );
            mPropertyGrid.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF( 8F, 20F );
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size( 1182, 853 );
            Controls.Add( splitContainer_Main );
            Controls.Add( mMainMenuStrip );
            Icon = (System.Drawing.Icon) resources.GetObject( "$this.Icon" ) ;
            MainMenuStrip = mMainMenuStrip;
            Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            Name = "MainForm";
            Text = "GFD Studio";
            mMainMenuStrip.ResumeLayout( false );
            mMainMenuStrip.PerformLayout();
            tabControl1.ResumeLayout( false );
            tabPage1.ResumeLayout( false );
            tabPage2.ResumeLayout( false );
            tableLayoutPanel_AnimationControls.ResumeLayout( false );
            tableLayoutPanel_AnimationControls.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize) mAnimationTrackBar  ).EndInit();
            splitContainer_Main.Panel1.ResumeLayout( false );
            splitContainer_Main.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) splitContainer_Main  ).EndInit();
            splitContainer_Main.ResumeLayout( false );
            tableLayoutPanel_Leftside.ResumeLayout( false );
            splitContainer_RightSide.Panel1.ResumeLayout( false );
            splitContainer_RightSide.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) splitContainer_RightSide  ).EndInit();
            splitContainer_RightSide.ResumeLayout( false );
            panel_PropertyGridContainer.ResumeLayout( false );
            ResumeLayout( false );
            PerformLayout();
        }

        #endregion

        private DataTreeView mModelEditorTreeView;
        private System.Windows.Forms.MenuStrip mMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mFileToolStripMenuItem;
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_AnimationControls;
        private System.Windows.Forms.Button mAnimationPlaybackButton;
        private System.Windows.Forms.TrackBar mAnimationTrackBar;
        private DataTreeView mAnimationListTreeView;
        private System.Windows.Forms.Button mAnimationStopButton;
        private System.Windows.Forms.ToolStripMenuItem rescaleAnimationPacksInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertAnimationsToP5InDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertMaterialInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MassExportTexturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MassReplaceTexturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thereIsNoHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perishToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem retainTexNameToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Leftside;
        private System.Windows.Forms.SplitContainer splitContainer_RightSide;
        private System.Windows.Forms.Panel panel_PropertyGridContainer;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
    }
}