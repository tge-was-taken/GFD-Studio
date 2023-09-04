using DarkUI.Controls;
using DarkUI.Forms;
using GFDStudio.GUI.DataViewNodes;
using MetroSet_UI.Child;
using MetroSet_UI.Controls;
using MetroSet_UI.Forms;

namespace GFDStudio.GUI.Forms
{
    partial class MainForm : MetroSetForm
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
            retainColorValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            thereIsNoHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            perishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mContentPanel = new System.Windows.Forms.Panel();
            tabControl1 = new MetroSetTabControl();
            tabPage1 = new MetroSetSetTabPage();
            mModelEditorTreeView = new DataTreeView();
            tabPage2 = new MetroSetSetTabPage();
            mAnimationListTreeView = new DataTreeView();
            tableLayoutPanel_AnimationControls = new System.Windows.Forms.TableLayoutPanel();
            mAnimationTrackBar = new System.Windows.Forms.TrackBar();
            mAnimationPlaybackButton = new MetroSetButton();
            mAnimationStopButton = new MetroSetButton();
            splitContainer_Main = new System.Windows.Forms.SplitContainer();
            splitContainer_LeftSide = new System.Windows.Forms.SplitContainer();
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
            ( (System.ComponentModel.ISupportInitialize) splitContainer_LeftSide  ).BeginInit();
            splitContainer_LeftSide.Panel1.SuspendLayout();
            splitContainer_LeftSide.Panel2.SuspendLayout();
            splitContainer_LeftSide.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) splitContainer_RightSide  ).BeginInit();
            splitContainer_RightSide.Panel1.SuspendLayout();
            splitContainer_RightSide.Panel2.SuspendLayout();
            splitContainer_RightSide.SuspendLayout();
            panel_PropertyGridContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mMainMenuStrip
            // 
            mMainMenuStrip.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mMainMenuStrip.ForeColor = System.Drawing.Color.FromArgb(   220  ,   220  ,   220   );
            mMainMenuStrip.ImageScalingSize = new System.Drawing.Size( 20, 20 );
            mMainMenuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] { mFileToolStripMenuItem, animationToolStripMenuItem, toolsToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem } );
            mMainMenuStrip.Location = new System.Drawing.Point( 2, 0 );
            mMainMenuStrip.Name = "mMainMenuStrip";
            mMainMenuStrip.Padding = new System.Windows.Forms.Padding( 8, 3, 0, 3 );
            mMainMenuStrip.Size = new System.Drawing.Size( 878, 30 );
            mMainMenuStrip.TabIndex = 0;
            mMainMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            mFileToolStripMenuItem.AutoSize = false;
            mFileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mFileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, mOpenToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem } );
            mFileToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            mFileToolStripMenuItem.Size = new System.Drawing.Size( 46, 24 );
            mFileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.AutoSize = false;
            newToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            newToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { modelToolStripMenuItem } );
            newToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            newToolStripMenuItem.Text = "New";
            // 
            // modelToolStripMenuItem
            // 
            modelToolStripMenuItem.AutoSize = false;
            modelToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            modelToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            modelToolStripMenuItem.Size = new System.Drawing.Size( 224, 26 );
            modelToolStripMenuItem.Text = "Model";
            modelToolStripMenuItem.Click += HandleNewModelToolStripMenuItemClick;
            // 
            // mOpenToolStripMenuItem
            // 
            mOpenToolStripMenuItem.AutoSize = false;
            mOpenToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mOpenToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            mOpenToolStripMenuItem.ShortcutKeys =   System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.O  ;
            mOpenToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            mOpenToolStripMenuItem.Text = "Open";
            mOpenToolStripMenuItem.Click += HandleOpenToolStripMenuItemClick;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.AutoSize = false;
            saveToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            saveToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys =   System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.S  ;
            saveToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += HandleSaveToolStripMenuItemClick;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.AutoSize = false;
            saveAsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            saveAsToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys =    System.Windows.Forms.Keys.Control  |  System.Windows.Forms.Keys.Shift   |  System.Windows.Forms.Keys.S  ;
            saveAsToolStripMenuItem.Size = new System.Drawing.Size( 240, 26 );
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += HandleSaveAsToolStripMenuItemClick;
            // 
            // animationToolStripMenuItem
            // 
            animationToolStripMenuItem.AutoSize = false;
            animationToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            animationToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { loadExternalToolStripMenuItem } );
            animationToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            animationToolStripMenuItem.Size = new System.Drawing.Size( 92, 24 );
            animationToolStripMenuItem.Text = "Animation";
            // 
            // loadExternalToolStripMenuItem
            // 
            loadExternalToolStripMenuItem.AutoSize = false;
            loadExternalToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            loadExternalToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            loadExternalToolStripMenuItem.Name = "loadExternalToolStripMenuItem";
            loadExternalToolStripMenuItem.Size = new System.Drawing.Size( 224, 26 );
            loadExternalToolStripMenuItem.Text = "Load";
            loadExternalToolStripMenuItem.Click += HandleAnimationLoadExternalToolStripMenuItemClick;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.AutoSize = false;
            toolsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            toolsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { makeRelativeToolStripMenuItem, rescaleAnimationPacksInDirectoryToolStripMenuItem, convertAnimationsToP5InDirectoryToolStripMenuItem, convertMaterialInDirectoryToolStripMenuItem, copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem, MassExportTexturesToolStripMenuItem, MassReplaceTexturesToolStripMenuItem } );
            toolsToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size( 58, 24 );
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // makeRelativeToolStripMenuItem
            // 
            makeRelativeToolStripMenuItem.AutoSize = false;
            makeRelativeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            makeRelativeToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            makeRelativeToolStripMenuItem.Name = "makeRelativeToolStripMenuItem";
            makeRelativeToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            makeRelativeToolStripMenuItem.Text = "Retarget animation packs in directory";
            makeRelativeToolStripMenuItem.Click += HandleRetargetAnimationsToolStripMenuItemClick;
            // 
            // rescaleAnimationPacksInDirectoryToolStripMenuItem
            // 
            rescaleAnimationPacksInDirectoryToolStripMenuItem.AutoSize = false;
            rescaleAnimationPacksInDirectoryToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            rescaleAnimationPacksInDirectoryToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Name = "rescaleAnimationPacksInDirectoryToolStripMenuItem";
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Text = "Rescale/Reposition animation packs in directory";
            rescaleAnimationPacksInDirectoryToolStripMenuItem.Click += HandleRescaleAnimationsToolStripMenuItemClick;
            // 
            // convertAnimationsToP5InDirectoryToolStripMenuItem
            // 
            convertAnimationsToP5InDirectoryToolStripMenuItem.AutoSize = false;
            convertAnimationsToP5InDirectoryToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            convertAnimationsToP5InDirectoryToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            convertAnimationsToP5InDirectoryToolStripMenuItem.Name = "convertAnimationsToP5InDirectoryToolStripMenuItem";
            convertAnimationsToP5InDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            convertAnimationsToP5InDirectoryToolStripMenuItem.Text = "Convert P5R animations to P5 in directory";
            convertAnimationsToP5InDirectoryToolStripMenuItem.Click += HandleConvertAnimationsToolStripMenuItemClick;
            // 
            // convertMaterialInDirectoryToolStripMenuItem
            // 
            convertMaterialInDirectoryToolStripMenuItem.AutoSize = false;
            convertMaterialInDirectoryToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            convertMaterialInDirectoryToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            convertMaterialInDirectoryToolStripMenuItem.Name = "convertMaterialInDirectoryToolStripMenuItem";
            convertMaterialInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            convertMaterialInDirectoryToolStripMenuItem.Text = "Convert model materials in directory";
            convertMaterialInDirectoryToolStripMenuItem.Click += HandleConvertMaterialsToolStripMenuItemClick;
            // 
            // copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem
            // 
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.AutoSize = false;
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Name = "copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem";
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Text = "Copy P5 Split GAP to multiple models in directory";
            copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem.Click += copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem_Click;
            // 
            // MassExportTexturesToolStripMenuItem
            // 
            MassExportTexturesToolStripMenuItem.AutoSize = false;
            MassExportTexturesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            MassExportTexturesToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            MassExportTexturesToolStripMenuItem.Name = "MassExportTexturesToolStripMenuItem";
            MassExportTexturesToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            MassExportTexturesToolStripMenuItem.Text = "Mass Export Textures from Models";
            MassExportTexturesToolStripMenuItem.Click += MassExportTexturesToolStripMenuItem_Click;
            // 
            // MassReplaceTexturesToolStripMenuItem
            // 
            MassReplaceTexturesToolStripMenuItem.AutoSize = false;
            MassReplaceTexturesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            MassReplaceTexturesToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            MassReplaceTexturesToolStripMenuItem.Name = "MassReplaceTexturesToolStripMenuItem";
            MassReplaceTexturesToolStripMenuItem.Size = new System.Drawing.Size( 421, 26 );
            MassReplaceTexturesToolStripMenuItem.Text = "Mass Replace Textures in Models";
            MassReplaceTexturesToolStripMenuItem.Click += MassReplaceTexturesToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.AutoSize = false;
            optionsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            optionsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { retainTexNameToolStripMenuItem, retainColorValuesToolStripMenuItem } );
            optionsToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size( 75, 24 );
            optionsToolStripMenuItem.Text = "Options";
            // 
            // retainTexNameToolStripMenuItem
            // 
            retainTexNameToolStripMenuItem.AutoSize = false;
            retainTexNameToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   20  ,   20  ,   20   );
            retainTexNameToolStripMenuItem.Checked = true;
            retainTexNameToolStripMenuItem.CheckOnClick = true;
            retainTexNameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            retainTexNameToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            retainTexNameToolStripMenuItem.Name = "retainTexNameToolStripMenuItem";
            retainTexNameToolStripMenuItem.Size = new System.Drawing.Size( 459, 26 );
            retainTexNameToolStripMenuItem.Text = "Retain original material's texture names when replacing";
            // 
            // retainColorValuesToolStripMenuItem
            // 
            retainColorValuesToolStripMenuItem.AutoSize = false;
            retainColorValuesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb( 20, 20, 20 );
            retainColorValuesToolStripMenuItem.Checked = false;
            retainColorValuesToolStripMenuItem.CheckOnClick = true;
            retainColorValuesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Unchecked;
            retainColorValuesToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            retainColorValuesToolStripMenuItem.Name = "retainColorValuesToolStripMenuItem";
            retainColorValuesToolStripMenuItem.Size = new System.Drawing.Size( 459, 26 );
            retainColorValuesToolStripMenuItem.Text = "Retain original material's color values when replacing";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.AutoSize = false;
            helpToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            helpToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] { thereIsNoHelpToolStripMenuItem, perishToolStripMenuItem } );
            helpToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size( 55, 24 );
            helpToolStripMenuItem.Text = "Help";
            // 
            // thereIsNoHelpToolStripMenuItem
            // 
            thereIsNoHelpToolStripMenuItem.AutoSize = false;
            thereIsNoHelpToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            thereIsNoHelpToolStripMenuItem.Enabled = false;
            thereIsNoHelpToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            thereIsNoHelpToolStripMenuItem.Name = "thereIsNoHelpToolStripMenuItem";
            thereIsNoHelpToolStripMenuItem.Size = new System.Drawing.Size( 200, 26 );
            thereIsNoHelpToolStripMenuItem.Text = "There is no help.";
            // 
            // perishToolStripMenuItem
            // 
            perishToolStripMenuItem.AutoSize = false;
            perishToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            perishToolStripMenuItem.Enabled = false;
            perishToolStripMenuItem.ForeColor = System.Drawing.Color.Silver;
            perishToolStripMenuItem.Name = "perishToolStripMenuItem";
            perishToolStripMenuItem.Size = new System.Drawing.Size( 200, 26 );
            perishToolStripMenuItem.Text = "Perish.";
            // 
            // mContentPanel
            // 
            mContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mContentPanel.Location = new System.Drawing.Point( 0, 0 );
            mContentPanel.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mContentPanel.Name = "mContentPanel";
            mContentPanel.Size = new System.Drawing.Size( 451, 492 );
            mContentPanel.TabIndex = 3;
            // 
            // tabControl1
            // 
            tabControl1.AnimateEasingType = MetroSet_UI.Enums.EasingType.CubeOut;
            tabControl1.AnimateTime = 200;
            tabControl1.BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            tabControl1.Controls.Add( tabPage1 );
            tabControl1.Controls.Add( tabPage2 );
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.ForeColor = System.Drawing.Color.Silver;
            tabControl1.IsDerivedStyle = true;
            tabControl1.ItemSize = new System.Drawing.Size( 100, 38 );
            tabControl1.Location = new System.Drawing.Point( 0, 0 );
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.SelectedTextColor = System.Drawing.Color.White;
            tabControl1.Size = new System.Drawing.Size( 423, 225 );
            tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            tabControl1.Speed = 100;
            tabControl1.Style = MetroSet_UI.Enums.Style.Dark;
            tabControl1.StyleManager = null;
            tabControl1.TabIndex = 4;
            tabControl1.TabStyle = MetroSet_UI.Enums.TabStyle.Style2;
            tabControl1.ThemeAuthor = "Narwin";
            tabControl1.ThemeName = "MetroDark";
            tabControl1.UnselectedTextColor = System.Drawing.Color.Gray;
            tabControl1.UseAnimation = false;
            // 
            // tabPage1
            // 
            tabPage1.BaseColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            tabPage1.Controls.Add( mModelEditorTreeView );
            tabPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPage1.Font = null;
            tabPage1.ForeColor = System.Drawing.Color.Silver;
            tabPage1.ImageIndex = 0;
            tabPage1.ImageKey = null;
            tabPage1.IsDerivedStyle = true;
            tabPage1.Location = new System.Drawing.Point( 4, 42 );
            tabPage1.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage1.Size = new System.Drawing.Size( 415, 179 );
            tabPage1.Style = MetroSet_UI.Enums.Style.Dark;
            tabPage1.StyleManager = null;
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Model Editor";
            tabPage1.ThemeAuthor = "Narwin";
            tabPage1.ThemeName = "MetroDark";
            tabPage1.ToolTipText = null;
            // 
            // mModelEditorTreeView
            // 
            mModelEditorTreeView.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mModelEditorTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            mModelEditorTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            mModelEditorTreeView.ForeColor = System.Drawing.Color.Silver;
            mModelEditorTreeView.ImageIndex = 0;
            mModelEditorTreeView.Location = new System.Drawing.Point( 5, 4 );
            mModelEditorTreeView.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mModelEditorTreeView.Name = "mModelEditorTreeView";
            mModelEditorTreeView.SelectedImageIndex = 0;
            mModelEditorTreeView.Size = new System.Drawing.Size( 405, 171 );
            mModelEditorTreeView.TabIndex = 1;
            mModelEditorTreeView.TopNode = null;
            // 
            // tabPage2
            // 
            tabPage2.BaseColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            tabPage2.Controls.Add( mAnimationListTreeView );
            tabPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPage2.Font = null;
            tabPage2.ForeColor = System.Drawing.Color.Silver;
            tabPage2.ImageIndex = 0;
            tabPage2.ImageKey = null;
            tabPage2.IsDerivedStyle = true;
            tabPage2.Location = new System.Drawing.Point( 4, 42 );
            tabPage2.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tabPage2.Size = new System.Drawing.Size( 415, 179 );
            tabPage2.Style = MetroSet_UI.Enums.Style.Dark;
            tabPage2.StyleManager = null;
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Animation List";
            tabPage2.ThemeAuthor = "Narwin";
            tabPage2.ThemeName = "MetroDark";
            tabPage2.ToolTipText = null;
            // 
            // mAnimationListTreeView
            // 
            mAnimationListTreeView.BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mAnimationListTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            mAnimationListTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationListTreeView.ForeColor = System.Drawing.Color.Silver;
            mAnimationListTreeView.ImageIndex = 0;
            mAnimationListTreeView.Location = new System.Drawing.Point( 5, 4 );
            mAnimationListTreeView.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationListTreeView.Name = "mAnimationListTreeView";
            mAnimationListTreeView.SelectedImageIndex = 0;
            mAnimationListTreeView.Size = new System.Drawing.Size( 405, 171 );
            mAnimationListTreeView.TabIndex = 2;
            mAnimationListTreeView.TopNode = null;
            // 
            // tableLayoutPanel_AnimationControls
            // 
            tableLayoutPanel_AnimationControls.ColumnCount = 3;
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 70F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 15F ) );
            tableLayoutPanel_AnimationControls.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 15F ) );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationTrackBar, 0, 0 );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationPlaybackButton, 1, 0 );
            tableLayoutPanel_AnimationControls.Controls.Add( mAnimationStopButton, 2, 0 );
            tableLayoutPanel_AnimationControls.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_AnimationControls.Location = new System.Drawing.Point( 0, 0 );
            tableLayoutPanel_AnimationControls.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            tableLayoutPanel_AnimationControls.Name = "tableLayoutPanel_AnimationControls";
            tableLayoutPanel_AnimationControls.RowCount = 1;
            tableLayoutPanel_AnimationControls.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            tableLayoutPanel_AnimationControls.Size = new System.Drawing.Size( 451, 25 );
            tableLayoutPanel_AnimationControls.TabIndex = 0;
            // 
            // mAnimationTrackBar
            // 
            mAnimationTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationTrackBar.Location = new System.Drawing.Point( 5, 4 );
            mAnimationTrackBar.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationTrackBar.Name = "mAnimationTrackBar";
            mAnimationTrackBar.Size = new System.Drawing.Size( 305, 17 );
            mAnimationTrackBar.TabIndex = 1;
            // 
            // mAnimationPlaybackButton
            // 
            mAnimationPlaybackButton.DisabledBackColor = System.Drawing.Color.FromArgb(   120  ,   65  ,   177  ,   225   );
            mAnimationPlaybackButton.DisabledBorderColor = System.Drawing.Color.FromArgb(   120  ,   65  ,   177  ,   225   );
            mAnimationPlaybackButton.DisabledForeColor = System.Drawing.Color.Gray;
            mAnimationPlaybackButton.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationPlaybackButton.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            mAnimationPlaybackButton.HoverBorderColor = System.Drawing.Color.FromArgb(   95  ,   207  ,   255   );
            mAnimationPlaybackButton.HoverColor = System.Drawing.Color.FromArgb(   95  ,   207  ,   255   );
            mAnimationPlaybackButton.HoverTextColor = System.Drawing.Color.White;
            mAnimationPlaybackButton.IsDerivedStyle = true;
            mAnimationPlaybackButton.Location = new System.Drawing.Point( 320, 4 );
            mAnimationPlaybackButton.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationPlaybackButton.MaximumSize = new System.Drawing.Size( 100, 30 );
            mAnimationPlaybackButton.Name = "mAnimationPlaybackButton";
            mAnimationPlaybackButton.NormalBorderColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            mAnimationPlaybackButton.NormalColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            mAnimationPlaybackButton.NormalTextColor = System.Drawing.Color.White;
            mAnimationPlaybackButton.Padding = new System.Windows.Forms.Padding( 5 );
            mAnimationPlaybackButton.PressBorderColor = System.Drawing.Color.FromArgb(   35  ,   147  ,   195   );
            mAnimationPlaybackButton.PressColor = System.Drawing.Color.FromArgb(   35  ,   147  ,   195   );
            mAnimationPlaybackButton.PressTextColor = System.Drawing.Color.White;
            mAnimationPlaybackButton.Size = new System.Drawing.Size( 57, 17 );
            mAnimationPlaybackButton.Style = MetroSet_UI.Enums.Style.Dark;
            mAnimationPlaybackButton.StyleManager = null;
            mAnimationPlaybackButton.TabIndex = 0;
            mAnimationPlaybackButton.Text = "▶";
            mAnimationPlaybackButton.ThemeAuthor = "Narwin";
            mAnimationPlaybackButton.ThemeName = "MetroDark";
            // 
            // mAnimationStopButton
            // 
            mAnimationStopButton.DisabledBackColor = System.Drawing.Color.FromArgb(   120  ,   65  ,   177  ,   225   );
            mAnimationStopButton.DisabledBorderColor = System.Drawing.Color.FromArgb(   120  ,   65  ,   177  ,   225   );
            mAnimationStopButton.DisabledForeColor = System.Drawing.Color.Gray;
            mAnimationStopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            mAnimationStopButton.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            mAnimationStopButton.HoverBorderColor = System.Drawing.Color.FromArgb(   95  ,   207  ,   255   );
            mAnimationStopButton.HoverColor = System.Drawing.Color.FromArgb(   95  ,   207  ,   255   );
            mAnimationStopButton.HoverTextColor = System.Drawing.Color.White;
            mAnimationStopButton.IsDerivedStyle = true;
            mAnimationStopButton.Location = new System.Drawing.Point( 387, 4 );
            mAnimationStopButton.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mAnimationStopButton.MaximumSize = new System.Drawing.Size( 100, 30 );
            mAnimationStopButton.Name = "mAnimationStopButton";
            mAnimationStopButton.NormalBorderColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            mAnimationStopButton.NormalColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            mAnimationStopButton.NormalTextColor = System.Drawing.Color.White;
            mAnimationStopButton.Padding = new System.Windows.Forms.Padding( 5 );
            mAnimationStopButton.PressBorderColor = System.Drawing.Color.FromArgb(   35  ,   147  ,   195   );
            mAnimationStopButton.PressColor = System.Drawing.Color.FromArgb(   35  ,   147  ,   195   );
            mAnimationStopButton.PressTextColor = System.Drawing.Color.White;
            mAnimationStopButton.Size = new System.Drawing.Size( 59, 17 );
            mAnimationStopButton.Style = MetroSet_UI.Enums.Style.Light;
            mAnimationStopButton.StyleManager = null;
            mAnimationStopButton.TabIndex = 2;
            mAnimationStopButton.Text = "■";
            mAnimationStopButton.ThemeAuthor = "Narwin";
            mAnimationStopButton.ThemeName = "MetroLite";
            mAnimationStopButton.Click += HandleAnimationStopButtonClick;
            // 
            // splitContainer_Main
            // 
            splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer_Main.Location = new System.Drawing.Point( 2, 30 );
            splitContainer_Main.Name = "splitContainer_Main";
            // 
            // splitContainer_Main.Panel1
            // 
            splitContainer_Main.Panel1.Controls.Add( splitContainer_LeftSide );
            // 
            // splitContainer_Main.Panel2
            // 
            splitContainer_Main.Panel2.Controls.Add( splitContainer_RightSide );
            splitContainer_Main.Size = new System.Drawing.Size( 878, 521 );
            splitContainer_Main.SplitterDistance = 451;
            splitContainer_Main.TabIndex = 5;
            // 
            // splitContainer_LeftSide
            // 
            splitContainer_LeftSide.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer_LeftSide.Location = new System.Drawing.Point( 0, 0 );
            splitContainer_LeftSide.Name = "splitContainer_LeftSide";
            splitContainer_LeftSide.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_LeftSide.Panel1
            // 
            splitContainer_LeftSide.Panel1.Controls.Add( mContentPanel );
            // 
            // splitContainer_LeftSide.Panel2
            // 
            splitContainer_LeftSide.Panel2.Controls.Add( tableLayoutPanel_AnimationControls );
            splitContainer_LeftSide.Size = new System.Drawing.Size( 451, 521 );
            splitContainer_LeftSide.SplitterDistance = 492;
            splitContainer_LeftSide.TabIndex = 7;
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
            splitContainer_RightSide.Size = new System.Drawing.Size( 423, 521 );
            splitContainer_RightSide.SplitterDistance = 225;
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
            panel_PropertyGridContainer.Size = new System.Drawing.Size( 423, 292 );
            panel_PropertyGridContainer.TabIndex = 0;
            // 
            // mPropertyGrid
            // 
            mPropertyGrid.CategoryForeColor = System.Drawing.Color.Silver;
            mPropertyGrid.CommandsBorderColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mPropertyGrid.CommandsForeColor = System.Drawing.Color.Silver;
            mPropertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(   127  ,   192  ,   192  ,   192   );
            mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            mPropertyGrid.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            mPropertyGrid.HelpBackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mPropertyGrid.HelpBorderColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mPropertyGrid.HelpForeColor = System.Drawing.Color.Silver;
            mPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDarkDark;
            mPropertyGrid.Location = new System.Drawing.Point( 0, 0 );
            mPropertyGrid.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            mPropertyGrid.Name = "mPropertyGrid";
            mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            mPropertyGrid.Size = new System.Drawing.Size( 418, 287 );
            mPropertyGrid.TabIndex = 5;
            mPropertyGrid.ToolbarVisible = false;
            mPropertyGrid.ViewBackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mPropertyGrid.ViewBorderColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            mPropertyGrid.ViewForeColor = System.Drawing.Color.Silver;
            // 
            // MainForm
            // 
            BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            ClientSize = new System.Drawing.Size( 882, 553 );
            Controls.Add( splitContainer_Main );
            Controls.Add( mMainMenuStrip );
            DropShadowEffect = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HeaderHeight = -40;
            Icon = (System.Drawing.Icon) resources.GetObject( "$this.Icon" ) ;
            MainMenuStrip = mMainMenuStrip;
            Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
            Name = "MainForm";
            Opacity = 0.99D;
            Padding = new System.Windows.Forms.Padding( 2, 0, 2, 2 );
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            Style = MetroSet_UI.Enums.Style.Dark;
            Text = "GFD STUDIO";
            TextColor = System.Drawing.Color.White;
            ThemeName = "MetroDark";
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
            splitContainer_LeftSide.Panel1.ResumeLayout( false );
            splitContainer_LeftSide.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) splitContainer_LeftSide  ).EndInit();
            splitContainer_LeftSide.ResumeLayout( false );
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
        private MetroSetTabControl tabControl1;
        private MetroSetSetTabPage tabPage1;
        private MetroSetSetTabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_AnimationControls;
        private MetroSetButton mAnimationPlaybackButton;
        private System.Windows.Forms.TrackBar mAnimationTrackBar;
        private DataTreeView mAnimationListTreeView;
        private MetroSetButton mAnimationStopButton;
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
        public System.Windows.Forms.ToolStripMenuItem retainColorValuesToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Leftside;
        private System.Windows.Forms.SplitContainer splitContainer_RightSide;
        private System.Windows.Forms.Panel panel_PropertyGridContainer;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.SplitContainer splitContainer_LeftSide;
    }
}