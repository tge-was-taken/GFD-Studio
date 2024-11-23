using GFDLibrary.Conversion;
using GFDStudio.Properties;
using MetroSet_UI.Forms;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    partial class ModelConversionOptionsDialog : Form
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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            button2 = new Button();
            button1 = new Button();
            splitContainer1 = new SplitContainer();
            groupBox2 = new GroupBox();
            generalPropertyGrid = new PropertyGrid();
            groupBox1 = new GroupBox();
            splitContainer2 = new SplitContainer();
            sceneTreeView = new TreeView();
            scenePropertyGrid = new PropertyGrid();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)splitContainer1 ).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)splitContainer2 ).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
            tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel2, 0, 1 );
            tableLayoutPanel1.Controls.Add( splitContainer1, 0, 0 );
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point( 2, 0 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 95.71106F ) );
            tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 4.288939F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 1100, 886 );
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
            tableLayoutPanel2.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
            tableLayoutPanel2.Controls.Add( button2, 1, 0 );
            tableLayoutPanel2.Controls.Add( button1, 0, 0 );
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point( 3, 851 );
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add( new RowStyle( SizeType.Percent, 50F ) );
            tableLayoutPanel2.Size = new System.Drawing.Size( 1094, 32 );
            tableLayoutPanel2.TabIndex = 0;
            // 
            // button2
            // 
            button2.Anchor =   AnchorStyles.Top  |  AnchorStyles.Bottom   |  AnchorStyles.Left ;
            button2.DialogResult = DialogResult.Cancel;
            button2.Location = new System.Drawing.Point( 550, 3 );
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size( 75, 26 );
            button2.TabIndex = 1;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor =   AnchorStyles.Top  |  AnchorStyles.Bottom   |  AnchorStyles.Right ;
            button1.BackColor = System.Drawing.SystemColors.Control;
            button1.DialogResult = DialogResult.OK;
            button1.Location = new System.Drawing.Point( 469, 3 );
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size( 75, 26 );
            button1.TabIndex = 0;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point( 3, 3 );
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add( groupBox2 );
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add( groupBox1 );
            splitContainer1.Size = new System.Drawing.Size( 1094, 842 );
            splitContainer1.SplitterDistance = 386;
            splitContainer1.TabIndex = 1;
            splitContainer1.SplitterMoved +=  splitContainer1_SplitterMoved ;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add( generalPropertyGrid );
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point( 0, 0 );
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size( 386, 842 );
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "General settings";
            // 
            // generalPropertyGrid
            // 
            generalPropertyGrid.Dock = DockStyle.Fill;
            generalPropertyGrid.Location = new System.Drawing.Point( 3, 19 );
            generalPropertyGrid.Name = "generalPropertyGrid";
            generalPropertyGrid.PropertySort = PropertySort.Categorized;
            generalPropertyGrid.Size = new System.Drawing.Size( 380, 820 );
            generalPropertyGrid.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add( splitContainer2 );
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point( 0, 0 );
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size( 704, 842 );
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Scene";
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point( 3, 19 );
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add( sceneTreeView );
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add( scenePropertyGrid );
            splitContainer2.Size = new System.Drawing.Size( 698, 820 );
            splitContainer2.SplitterDistance = 354;
            splitContainer2.TabIndex = 1;
            // 
            // sceneTreeView
            // 
            sceneTreeView.Dock = DockStyle.Fill;
            sceneTreeView.Location = new System.Drawing.Point( 0, 0 );
            sceneTreeView.Name = "sceneTreeView";
            sceneTreeView.Size = new System.Drawing.Size( 354, 820 );
            sceneTreeView.TabIndex = 0;
            // 
            // scenePropertyGrid
            // 
            scenePropertyGrid.Dock = DockStyle.Fill;
            scenePropertyGrid.Location = new System.Drawing.Point( 0, 0 );
            scenePropertyGrid.Name = "scenePropertyGrid";
            scenePropertyGrid.PropertySort = PropertySort.Categorized;
            scenePropertyGrid.Size = new System.Drawing.Size( 340, 820 );
            scenePropertyGrid.TabIndex = 0;
            // 
            // ModelConversionOptionsDialog
            // 
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size( 1104, 888 );
            Controls.Add( tableLayoutPanel1 );
            Margin = new Padding( 4, 3, 4, 3 );
            Name = "ModelConversionOptionsDialog";
            Opacity = 0.99D;
            Padding = new Padding( 2, 0, 2, 2 );
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "MODEL CONVERTER OPTIONS";
            tableLayoutPanel1.ResumeLayout( false );
            tableLayoutPanel2.ResumeLayout( false );
            splitContainer1.Panel1.ResumeLayout( false );
            splitContainer1.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)splitContainer1 ).EndInit();
            splitContainer1.ResumeLayout( false );
            groupBox2.ResumeLayout( false );
            groupBox1.ResumeLayout( false );
            splitContainer2.Panel1.ResumeLayout( false );
            splitContainer2.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)splitContainer2 ).EndInit();
            splitContainer2.ResumeLayout( false );
            ResumeLayout( false );
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private SplitContainer splitContainer1;
        private PropertyGrid generalPropertyGrid;
        private Button button2;
        private Button button1;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private SplitContainer splitContainer2;
        private TreeView sceneTreeView;
        private PropertyGrid scenePropertyGrid;
    }
}