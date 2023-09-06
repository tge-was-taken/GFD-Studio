using MetroSet_UI.Controls;
using MetroSet_UI.Forms;

namespace GFDStudio.GUI.Forms
{
    partial class SetScaleValueDialog : MetroSetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( SetScaleValueDialog ) );
            tableLayoutPanel_Scale = new System.Windows.Forms.TableLayoutPanel();
            lbl_ScaleZ = new MetroSetLabel();
            lbl_scaleX = new MetroSetLabel();
            num_ScaleX = new System.Windows.Forms.NumericUpDown();
            lbl_ScaleY = new MetroSetLabel();
            num_ScaleY = new System.Windows.Forms.NumericUpDown();
            num_ScaleZ = new System.Windows.Forms.NumericUpDown();
            lbl_Scale = new MetroSetLabel();
            CancelButton = new System.Windows.Forms.Button();
            OKButton = new System.Windows.Forms.Button();
            tableLayoutPanel_Pos = new System.Windows.Forms.TableLayoutPanel();
            lbl_PosZ = new MetroSetLabel();
            lbl_PosX = new MetroSetLabel();
            num_PosX = new System.Windows.Forms.NumericUpDown();
            lbl_PosY = new MetroSetLabel();
            num_PosY = new System.Windows.Forms.NumericUpDown();
            num_PosZ = new System.Windows.Forms.NumericUpDown();
            lbl_Position = new MetroSetLabel();
            tableLayoutPanel_Rot = new System.Windows.Forms.TableLayoutPanel();
            lbl_RotW = new MetroSetLabel();
            lbl_RotZ = new MetroSetLabel();
            lbl_RotX = new MetroSetLabel();
            num_RotX = new System.Windows.Forms.NumericUpDown();
            lbl_RotY = new MetroSetLabel();
            num_RotY = new System.Windows.Forms.NumericUpDown();
            num_RotZ = new System.Windows.Forms.NumericUpDown();
            lbl_Rotation = new MetroSetLabel();
            num_RotW = new System.Windows.Forms.NumericUpDown();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel_Scale.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleX  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleY  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleZ  ).BeginInit();
            tableLayoutPanel_Pos.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) num_PosX  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_PosY  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_PosZ  ).BeginInit();
            tableLayoutPanel_Rot.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) num_RotX  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotY  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotZ  ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotW  ).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel_Scale
            // 
            tableLayoutPanel_Scale.ColumnCount = 2;
            tableLayoutPanel_Scale.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 19.52663F ) );
            tableLayoutPanel_Scale.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 80.47337F ) );
            tableLayoutPanel_Scale.Controls.Add( lbl_ScaleZ, 0, 3 );
            tableLayoutPanel_Scale.Controls.Add( lbl_scaleX, 0, 1 );
            tableLayoutPanel_Scale.Controls.Add( num_ScaleX, 1, 1 );
            tableLayoutPanel_Scale.Controls.Add( lbl_ScaleY, 0, 2 );
            tableLayoutPanel_Scale.Controls.Add( num_ScaleY, 1, 2 );
            tableLayoutPanel_Scale.Controls.Add( num_ScaleZ, 1, 3 );
            tableLayoutPanel_Scale.Controls.Add( lbl_Scale, 1, 0 );
            tableLayoutPanel_Scale.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_Scale.Location = new System.Drawing.Point( 4, 5 );
            tableLayoutPanel_Scale.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            tableLayoutPanel_Scale.Name = "tableLayoutPanel_Scale";
            tableLayoutPanel_Scale.RowCount = 5;
            tableLayoutPanel_Scale.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Scale.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Scale.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Scale.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Scale.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Scale.Size = new System.Drawing.Size( 224, 298 );
            tableLayoutPanel_Scale.TabIndex = 0;
            // 
            // lbl_ScaleZ
            // 
            lbl_ScaleZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_ScaleZ.AutoSize = true;
            lbl_ScaleZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_ScaleZ.IsDerivedStyle = true;
            lbl_ScaleZ.Location = new System.Drawing.Point( 21, 196 );
            lbl_ScaleZ.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_ScaleZ.Name = "lbl_ScaleZ";
            lbl_ScaleZ.Size = new System.Drawing.Size( 18, 20 );
            lbl_ScaleZ.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_ScaleZ.StyleManager = null;
            lbl_ScaleZ.TabIndex = 8;
            lbl_ScaleZ.Text = "Z";
            lbl_ScaleZ.ThemeAuthor = "Narwin";
            lbl_ScaleZ.ThemeName = "MetroDark";
            // 
            // lbl_scaleX
            // 
            lbl_scaleX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_scaleX.AutoSize = true;
            lbl_scaleX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_scaleX.IsDerivedStyle = true;
            lbl_scaleX.Location = new System.Drawing.Point( 19, 78 );
            lbl_scaleX.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_scaleX.Name = "lbl_scaleX";
            lbl_scaleX.Size = new System.Drawing.Size( 20, 20 );
            lbl_scaleX.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_scaleX.StyleManager = null;
            lbl_scaleX.TabIndex = 3;
            lbl_scaleX.Text = "X";
            lbl_scaleX.ThemeAuthor = "Narwin";
            lbl_scaleX.ThemeName = "MetroDark";
            // 
            // num_ScaleX
            // 
            num_ScaleX.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_ScaleX.DecimalPlaces = 6;
            num_ScaleX.Location = new System.Drawing.Point( 47, 72 );
            num_ScaleX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleX.Name = "num_ScaleX";
            num_ScaleX.Size = new System.Drawing.Size( 173, 32 );
            num_ScaleX.TabIndex = 4;
            num_ScaleX.Value = new decimal( new int[] { 1, 0, 0, 0 } );
            // 
            // lbl_ScaleY
            // 
            lbl_ScaleY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_ScaleY.AutoSize = true;
            lbl_ScaleY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_ScaleY.IsDerivedStyle = true;
            lbl_ScaleY.Location = new System.Drawing.Point( 20, 137 );
            lbl_ScaleY.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_ScaleY.Name = "lbl_ScaleY";
            lbl_ScaleY.Size = new System.Drawing.Size( 19, 20 );
            lbl_ScaleY.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_ScaleY.StyleManager = null;
            lbl_ScaleY.TabIndex = 6;
            lbl_ScaleY.Text = "Y";
            lbl_ScaleY.ThemeAuthor = "Narwin";
            lbl_ScaleY.ThemeName = "MetroDark";
            // 
            // num_ScaleY
            // 
            num_ScaleY.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_ScaleY.DecimalPlaces = 6;
            num_ScaleY.Location = new System.Drawing.Point( 47, 131 );
            num_ScaleY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleY.Name = "num_ScaleY";
            num_ScaleY.Size = new System.Drawing.Size( 173, 32 );
            num_ScaleY.TabIndex = 7;
            num_ScaleY.Value = new decimal( new int[] { 1, 0, 0, 0 } );
            // 
            // num_ScaleZ
            // 
            num_ScaleZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_ScaleZ.DecimalPlaces = 6;
            num_ScaleZ.Location = new System.Drawing.Point( 47, 190 );
            num_ScaleZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleZ.Name = "num_ScaleZ";
            num_ScaleZ.Size = new System.Drawing.Size( 173, 32 );
            num_ScaleZ.TabIndex = 9;
            num_ScaleZ.Value = new decimal( new int[] { 1, 0, 0, 0 } );
            // 
            // lbl_Scale
            // 
            lbl_Scale.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_Scale.AutoSize = true;
            lbl_Scale.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_Scale.IsDerivedStyle = true;
            lbl_Scale.Location = new System.Drawing.Point( 169, 19 );
            lbl_Scale.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_Scale.Name = "lbl_Scale";
            lbl_Scale.Size = new System.Drawing.Size( 51, 20 );
            lbl_Scale.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_Scale.StyleManager = null;
            lbl_Scale.TabIndex = 5;
            lbl_Scale.Text = "Scale";
            lbl_Scale.ThemeAuthor = "Narwin";
            lbl_Scale.ThemeName = "MetroDark";
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.ForeColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            CancelButton.Location = new System.Drawing.Point( 275, 323 );
            CancelButton.Margin = new System.Windows.Forms.Padding( 13, 15, 13, 15 );
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size( 146, 47 );
            CancelButton.TabIndex = 1;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKButton.ForeColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            OKButton.Location = new System.Drawing.Point( 523, 323 );
            OKButton.Margin = new System.Windows.Forms.Padding( 13, 15, 13, 15 );
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size( 114, 47 );
            OKButton.TabIndex = 2;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // tableLayoutPanel_Pos
            // 
            tableLayoutPanel_Pos.ColumnCount = 2;
            tableLayoutPanel_Pos.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 27.027029F ) );
            tableLayoutPanel_Pos.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 72.97298F ) );
            tableLayoutPanel_Pos.Controls.Add( lbl_PosZ, 0, 3 );
            tableLayoutPanel_Pos.Controls.Add( lbl_PosX, 0, 1 );
            tableLayoutPanel_Pos.Controls.Add( num_PosX, 1, 1 );
            tableLayoutPanel_Pos.Controls.Add( lbl_PosY, 0, 2 );
            tableLayoutPanel_Pos.Controls.Add( num_PosY, 1, 2 );
            tableLayoutPanel_Pos.Controls.Add( num_PosZ, 1, 3 );
            tableLayoutPanel_Pos.Controls.Add( lbl_Position, 1, 0 );
            tableLayoutPanel_Pos.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_Pos.Location = new System.Drawing.Point( 468, 5 );
            tableLayoutPanel_Pos.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            tableLayoutPanel_Pos.Name = "tableLayoutPanel_Pos";
            tableLayoutPanel_Pos.RowCount = 5;
            tableLayoutPanel_Pos.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Pos.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Pos.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Pos.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Pos.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Pos.Size = new System.Drawing.Size( 224, 298 );
            tableLayoutPanel_Pos.TabIndex = 3;
            // 
            // lbl_PosZ
            // 
            lbl_PosZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_PosZ.AutoSize = true;
            lbl_PosZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_PosZ.IsDerivedStyle = true;
            lbl_PosZ.Location = new System.Drawing.Point( 38, 196 );
            lbl_PosZ.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_PosZ.Name = "lbl_PosZ";
            lbl_PosZ.Size = new System.Drawing.Size( 18, 20 );
            lbl_PosZ.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_PosZ.StyleManager = null;
            lbl_PosZ.TabIndex = 8;
            lbl_PosZ.Text = "Z";
            lbl_PosZ.ThemeAuthor = "Narwin";
            lbl_PosZ.ThemeName = "MetroDark";
            // 
            // lbl_PosX
            // 
            lbl_PosX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_PosX.AutoSize = true;
            lbl_PosX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_PosX.IsDerivedStyle = true;
            lbl_PosX.Location = new System.Drawing.Point( 36, 78 );
            lbl_PosX.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_PosX.Name = "lbl_PosX";
            lbl_PosX.Size = new System.Drawing.Size( 20, 20 );
            lbl_PosX.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_PosX.StyleManager = null;
            lbl_PosX.TabIndex = 3;
            lbl_PosX.Text = "X";
            lbl_PosX.ThemeAuthor = "Narwin";
            lbl_PosX.ThemeName = "MetroDark";
            // 
            // num_PosX
            // 
            num_PosX.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_PosX.DecimalPlaces = 6;
            num_PosX.Location = new System.Drawing.Point( 64, 72 );
            num_PosX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosX.Maximum = new decimal( new int[] { 999999, 0, 0, 0 } );
            num_PosX.Minimum = new decimal( new int[] { 999999, 0, 0, int.MinValue } );
            num_PosX.Name = "num_PosX";
            num_PosX.Size = new System.Drawing.Size( 156, 32 );
            num_PosX.TabIndex = 4;
            // 
            // lbl_PosY
            // 
            lbl_PosY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_PosY.AutoSize = true;
            lbl_PosY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_PosY.IsDerivedStyle = true;
            lbl_PosY.Location = new System.Drawing.Point( 37, 137 );
            lbl_PosY.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_PosY.Name = "lbl_PosY";
            lbl_PosY.Size = new System.Drawing.Size( 19, 20 );
            lbl_PosY.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_PosY.StyleManager = null;
            lbl_PosY.TabIndex = 6;
            lbl_PosY.Text = "Y";
            lbl_PosY.ThemeAuthor = "Narwin";
            lbl_PosY.ThemeName = "MetroDark";
            // 
            // num_PosY
            // 
            num_PosY.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_PosY.DecimalPlaces = 6;
            num_PosY.Location = new System.Drawing.Point( 64, 131 );
            num_PosY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosY.Maximum = new decimal( new int[] { 999999, 0, 0, 0 } );
            num_PosY.Minimum = new decimal( new int[] { 999999, 0, 0, int.MinValue } );
            num_PosY.Name = "num_PosY";
            num_PosY.Size = new System.Drawing.Size( 156, 32 );
            num_PosY.TabIndex = 7;
            // 
            // num_PosZ
            // 
            num_PosZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_PosZ.DecimalPlaces = 6;
            num_PosZ.Location = new System.Drawing.Point( 64, 190 );
            num_PosZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosZ.Maximum = new decimal( new int[] { 999999, 0, 0, 0 } );
            num_PosZ.Minimum = new decimal( new int[] { 999999, 0, 0, int.MinValue } );
            num_PosZ.Name = "num_PosZ";
            num_PosZ.Size = new System.Drawing.Size( 156, 32 );
            num_PosZ.TabIndex = 9;
            // 
            // lbl_Position
            // 
            lbl_Position.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_Position.AutoSize = true;
            lbl_Position.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_Position.IsDerivedStyle = true;
            lbl_Position.Location = new System.Drawing.Point( 151, 19 );
            lbl_Position.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_Position.Name = "lbl_Position";
            lbl_Position.Size = new System.Drawing.Size( 69, 20 );
            lbl_Position.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_Position.StyleManager = null;
            lbl_Position.TabIndex = 5;
            lbl_Position.Text = "Position";
            lbl_Position.ThemeAuthor = "Narwin";
            lbl_Position.ThemeName = "MetroDark";
            // 
            // tableLayoutPanel_Rot
            // 
            tableLayoutPanel_Rot.ColumnCount = 2;
            tableLayoutPanel_Rot.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 25.7142811F ) );
            tableLayoutPanel_Rot.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 74.28572F ) );
            tableLayoutPanel_Rot.Controls.Add( lbl_RotW, 0, 4 );
            tableLayoutPanel_Rot.Controls.Add( lbl_RotZ, 0, 3 );
            tableLayoutPanel_Rot.Controls.Add( lbl_RotX, 0, 1 );
            tableLayoutPanel_Rot.Controls.Add( num_RotX, 1, 1 );
            tableLayoutPanel_Rot.Controls.Add( lbl_RotY, 0, 2 );
            tableLayoutPanel_Rot.Controls.Add( num_RotY, 1, 2 );
            tableLayoutPanel_Rot.Controls.Add( num_RotZ, 1, 3 );
            tableLayoutPanel_Rot.Controls.Add( lbl_Rotation, 1, 0 );
            tableLayoutPanel_Rot.Controls.Add( num_RotW, 1, 4 );
            tableLayoutPanel_Rot.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel_Rot.Location = new System.Drawing.Point( 236, 5 );
            tableLayoutPanel_Rot.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            tableLayoutPanel_Rot.Name = "tableLayoutPanel_Rot";
            tableLayoutPanel_Rot.RowCount = 5;
            tableLayoutPanel_Rot.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Rot.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Rot.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Rot.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Rot.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel_Rot.Size = new System.Drawing.Size( 224, 298 );
            tableLayoutPanel_Rot.TabIndex = 10;
            // 
            // lbl_RotW
            // 
            lbl_RotW.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_RotW.AutoSize = true;
            lbl_RotW.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_RotW.IsDerivedStyle = true;
            lbl_RotW.Location = new System.Drawing.Point( 28, 257 );
            lbl_RotW.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_RotW.Name = "lbl_RotW";
            lbl_RotW.Size = new System.Drawing.Size( 25, 20 );
            lbl_RotW.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_RotW.StyleManager = null;
            lbl_RotW.TabIndex = 10;
            lbl_RotW.Text = "W";
            lbl_RotW.ThemeAuthor = "Narwin";
            lbl_RotW.ThemeName = "MetroDark";
            // 
            // lbl_RotZ
            // 
            lbl_RotZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_RotZ.AutoSize = true;
            lbl_RotZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_RotZ.IsDerivedStyle = true;
            lbl_RotZ.Location = new System.Drawing.Point( 35, 196 );
            lbl_RotZ.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_RotZ.Name = "lbl_RotZ";
            lbl_RotZ.Size = new System.Drawing.Size( 18, 20 );
            lbl_RotZ.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_RotZ.StyleManager = null;
            lbl_RotZ.TabIndex = 8;
            lbl_RotZ.Text = "Z";
            lbl_RotZ.ThemeAuthor = "Narwin";
            lbl_RotZ.ThemeName = "MetroDark";
            // 
            // lbl_RotX
            // 
            lbl_RotX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_RotX.AutoSize = true;
            lbl_RotX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_RotX.IsDerivedStyle = true;
            lbl_RotX.Location = new System.Drawing.Point( 33, 78 );
            lbl_RotX.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_RotX.Name = "lbl_RotX";
            lbl_RotX.Size = new System.Drawing.Size( 20, 20 );
            lbl_RotX.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_RotX.StyleManager = null;
            lbl_RotX.TabIndex = 3;
            lbl_RotX.Text = "X";
            lbl_RotX.ThemeAuthor = "Narwin";
            lbl_RotX.ThemeName = "MetroDark";
            // 
            // num_RotX
            // 
            num_RotX.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_RotX.DecimalPlaces = 6;
            num_RotX.Location = new System.Drawing.Point( 61, 72 );
            num_RotX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotX.Maximum = new decimal( new int[] { 999, 0, 0, 0 } );
            num_RotX.Minimum = new decimal( new int[] { 999, 0, 0, int.MinValue } );
            num_RotX.Name = "num_RotX";
            num_RotX.Size = new System.Drawing.Size( 159, 32 );
            num_RotX.TabIndex = 4;
            // 
            // lbl_RotY
            // 
            lbl_RotY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_RotY.AutoSize = true;
            lbl_RotY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_RotY.IsDerivedStyle = true;
            lbl_RotY.Location = new System.Drawing.Point( 34, 137 );
            lbl_RotY.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_RotY.Name = "lbl_RotY";
            lbl_RotY.Size = new System.Drawing.Size( 19, 20 );
            lbl_RotY.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_RotY.StyleManager = null;
            lbl_RotY.TabIndex = 6;
            lbl_RotY.Text = "Y";
            lbl_RotY.ThemeAuthor = "Narwin";
            lbl_RotY.ThemeName = "MetroDark";
            // 
            // num_RotY
            // 
            num_RotY.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_RotY.DecimalPlaces = 6;
            num_RotY.Location = new System.Drawing.Point( 61, 131 );
            num_RotY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotY.Maximum = new decimal( new int[] { 999, 0, 0, 0 } );
            num_RotY.Minimum = new decimal( new int[] { 999, 0, 0, int.MinValue } );
            num_RotY.Name = "num_RotY";
            num_RotY.Size = new System.Drawing.Size( 159, 32 );
            num_RotY.TabIndex = 7;
            // 
            // num_RotZ
            // 
            num_RotZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_RotZ.DecimalPlaces = 6;
            num_RotZ.Location = new System.Drawing.Point( 61, 190 );
            num_RotZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotZ.Maximum = new decimal( new int[] { 999, 0, 0, 0 } );
            num_RotZ.Minimum = new decimal( new int[] { 999, 0, 0, int.MinValue } );
            num_RotZ.Name = "num_RotZ";
            num_RotZ.Size = new System.Drawing.Size( 159, 32 );
            num_RotZ.TabIndex = 9;
            // 
            // lbl_Rotation
            // 
            lbl_Rotation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_Rotation.AutoSize = true;
            lbl_Rotation.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            lbl_Rotation.IsDerivedStyle = true;
            lbl_Rotation.Location = new System.Drawing.Point( 149, 19 );
            lbl_Rotation.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            lbl_Rotation.Name = "lbl_Rotation";
            lbl_Rotation.Size = new System.Drawing.Size( 71, 20 );
            lbl_Rotation.Style = MetroSet_UI.Enums.Style.Dark;
            lbl_Rotation.StyleManager = null;
            lbl_Rotation.TabIndex = 5;
            lbl_Rotation.Text = "Rotation";
            lbl_Rotation.ThemeAuthor = "Narwin";
            lbl_Rotation.ThemeName = "MetroDark";
            // 
            // num_RotW
            // 
            num_RotW.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_RotW.DecimalPlaces = 6;
            num_RotW.Location = new System.Drawing.Point( 61, 251 );
            num_RotW.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotW.Maximum = new decimal( new int[] { 999, 0, 0, 0 } );
            num_RotW.Name = "num_RotW";
            num_RotW.Size = new System.Drawing.Size( 159, 32 );
            num_RotW.TabIndex = 11;
            num_RotW.Value = new decimal( new int[] { 1, 0, 0, 0 } );
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.3333321F ) );
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.3333321F ) );
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.3333321F ) );
            tableLayoutPanel1.Controls.Add( OKButton, 2, 1 );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel_Pos, 2, 0 );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel_Rot, 1, 0 );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel_Scale, 0, 0 );
            tableLayoutPanel1.Controls.Add( CancelButton, 1, 1 );
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point( 2, 0 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 80F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 696, 385 );
            tableLayoutPanel1.TabIndex = 11;
            // 
            // SetScaleValueDialog
            // 
            AcceptButton = OKButton;
            BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            ClientSize = new System.Drawing.Size( 700, 387 );
            Controls.Add( tableLayoutPanel1 );
            DropShadowEffect = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HeaderHeight = -40;
            Icon = (System.Drawing.Icon) resources.GetObject( "$this.Icon" ) ;
            Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            Name = "SetScaleValueDialog";
            Opacity = 0.99D;
            Padding = new System.Windows.Forms.Padding( 2, 0, 2, 2 );
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Style = MetroSet_UI.Enums.Style.Dark;
            Text = "Scale/Reposition";
            ThemeName = "MetroDark";
            tableLayoutPanel_Scale.ResumeLayout( false );
            tableLayoutPanel_Scale.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleX  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleY  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_ScaleZ  ).EndInit();
            tableLayoutPanel_Pos.ResumeLayout( false );
            tableLayoutPanel_Pos.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize) num_PosX  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_PosY  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_PosZ  ).EndInit();
            tableLayoutPanel_Rot.ResumeLayout( false );
            tableLayoutPanel_Rot.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize) num_RotX  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotY  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotZ  ).EndInit();
            ( (System.ComponentModel.ISupportInitialize) num_RotW  ).EndInit();
            tableLayoutPanel1.ResumeLayout( false );
            ResumeLayout( false );
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Scale;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private MetroSetLabel lbl_scaleX;
        private System.Windows.Forms.NumericUpDown num_ScaleX;
        private MetroSetLabel lbl_ScaleZ;
        private MetroSetLabel lbl_Scale;
        private MetroSetLabel lbl_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Pos;
        private MetroSetLabel lbl_PosZ;
        private MetroSetLabel lbl_PosX;
        private System.Windows.Forms.NumericUpDown num_PosX;
        private MetroSetLabel lbl_Position;
        private MetroSetLabel lbl_PosY;
        private System.Windows.Forms.NumericUpDown num_PosY;
        private System.Windows.Forms.NumericUpDown num_PosZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Rot;
        private MetroSetLabel lbl_RotW;
        private MetroSetLabel lbl_RotZ;
        private MetroSetLabel lbl_RotX;
        private System.Windows.Forms.NumericUpDown num_RotX;
        private MetroSetLabel lbl_RotY;
        private System.Windows.Forms.NumericUpDown num_RotY;
        private System.Windows.Forms.NumericUpDown num_RotZ;
        private MetroSetLabel lbl_Rotation;
        private System.Windows.Forms.NumericUpDown num_RotW;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}