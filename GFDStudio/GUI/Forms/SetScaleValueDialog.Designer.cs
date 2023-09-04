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
            num_ScaleX = new MetroSetNumeric();
            lbl_ScaleY = new MetroSetLabel();
            num_ScaleY = new MetroSetNumeric();
            num_ScaleZ = new MetroSetNumeric();
            lbl_Scale = new MetroSetLabel();
            CancelButton = new System.Windows.Forms.Button();
            OKButton = new System.Windows.Forms.Button();
            tableLayoutPanel_Pos = new System.Windows.Forms.TableLayoutPanel();
            lbl_PosZ = new MetroSetLabel();
            lbl_PosX = new MetroSetLabel();
            num_PosX = new MetroSetNumeric();
            lbl_PosY = new MetroSetLabel();
            num_PosY = new MetroSetNumeric();
            num_PosZ = new MetroSetNumeric();
            lbl_Position = new MetroSetLabel();
            tableLayoutPanel_Rot = new System.Windows.Forms.TableLayoutPanel();
            lbl_RotW = new MetroSetLabel();
            lbl_RotZ = new MetroSetLabel();
            lbl_RotX = new MetroSetLabel();
            num_RotX = new MetroSetNumeric();
            lbl_RotY = new MetroSetLabel();
            num_RotY = new MetroSetNumeric();
            num_RotZ = new MetroSetNumeric();
            lbl_Rotation = new MetroSetLabel();
            num_RotW = new MetroSetNumeric();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel_Scale.SuspendLayout();
            tableLayoutPanel_Pos.SuspendLayout();
            tableLayoutPanel_Rot.SuspendLayout();
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
            num_ScaleX.BackColor = System.Drawing.Color.Transparent;
            num_ScaleX.BackgroundColor = System.Drawing.Color.Empty;
            num_ScaleX.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleX.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_ScaleX.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleX.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_ScaleX.IsDerivedStyle = true;
            num_ScaleX.Location = new System.Drawing.Point( 47, 75 );
            num_ScaleX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleX.Maximum = 100;
            num_ScaleX.Minimum = 0;
            num_ScaleX.Name = "num_ScaleX";
            num_ScaleX.Size = new System.Drawing.Size( 173, 26 );
            num_ScaleX.Style = MetroSet_UI.Enums.Style.Dark;
            num_ScaleX.StyleManager = null;
            num_ScaleX.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleX.TabIndex = 4;
            num_ScaleX.ThemeAuthor = "Narwin";
            num_ScaleX.ThemeName = "MetroDark";
            num_ScaleX.Value = 1;
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
            num_ScaleY.BackColor = System.Drawing.Color.Transparent;
            num_ScaleY.BackgroundColor = System.Drawing.Color.Empty;
            num_ScaleY.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleY.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_ScaleY.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleY.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_ScaleY.IsDerivedStyle = true;
            num_ScaleY.Location = new System.Drawing.Point( 47, 134 );
            num_ScaleY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleY.Maximum = 100;
            num_ScaleY.Minimum = 0;
            num_ScaleY.Name = "num_ScaleY";
            num_ScaleY.Size = new System.Drawing.Size( 173, 26 );
            num_ScaleY.Style = MetroSet_UI.Enums.Style.Dark;
            num_ScaleY.StyleManager = null;
            num_ScaleY.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleY.TabIndex = 7;
            num_ScaleY.ThemeAuthor = "Narwin";
            num_ScaleY.ThemeName = "MetroDark";
            num_ScaleY.Value = 1;
            // 
            // num_ScaleZ
            // 
            num_ScaleZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_ScaleZ.BackColor = System.Drawing.Color.Transparent;
            num_ScaleZ.BackgroundColor = System.Drawing.Color.Empty;
            num_ScaleZ.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleZ.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_ScaleZ.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleZ.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_ScaleZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_ScaleZ.IsDerivedStyle = true;
            num_ScaleZ.Location = new System.Drawing.Point( 47, 193 );
            num_ScaleZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_ScaleZ.Maximum = 100;
            num_ScaleZ.Minimum = 0;
            num_ScaleZ.Name = "num_ScaleZ";
            num_ScaleZ.Size = new System.Drawing.Size( 173, 26 );
            num_ScaleZ.Style = MetroSet_UI.Enums.Style.Dark;
            num_ScaleZ.StyleManager = null;
            num_ScaleZ.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_ScaleZ.TabIndex = 9;
            num_ScaleZ.ThemeAuthor = "Narwin";
            num_ScaleZ.ThemeName = "MetroDark";
            num_ScaleZ.Value = 1;
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
            num_PosX.BackColor = System.Drawing.Color.Transparent;
            num_PosX.BackgroundColor = System.Drawing.Color.Empty;
            num_PosX.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosX.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_PosX.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosX.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_PosX.IsDerivedStyle = true;
            num_PosX.Location = new System.Drawing.Point( 64, 75 );
            num_PosX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosX.Maximum = 999999;
            num_PosX.Minimum = -999999;
            num_PosX.Name = "num_PosX";
            num_PosX.Size = new System.Drawing.Size( 156, 26 );
            num_PosX.Style = MetroSet_UI.Enums.Style.Dark;
            num_PosX.StyleManager = null;
            num_PosX.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosX.TabIndex = 4;
            num_PosX.ThemeAuthor = "Narwin";
            num_PosX.ThemeName = "MetroDark";
            num_PosX.Value = 0;
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
            num_PosY.BackColor = System.Drawing.Color.Transparent;
            num_PosY.BackgroundColor = System.Drawing.Color.Empty;
            num_PosY.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosY.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_PosY.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosY.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_PosY.IsDerivedStyle = true;
            num_PosY.Location = new System.Drawing.Point( 64, 134 );
            num_PosY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosY.Maximum = 999999;
            num_PosY.Minimum = -999999;
            num_PosY.Name = "num_PosY";
            num_PosY.Size = new System.Drawing.Size( 156, 26 );
            num_PosY.Style = MetroSet_UI.Enums.Style.Dark;
            num_PosY.StyleManager = null;
            num_PosY.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosY.TabIndex = 7;
            num_PosY.ThemeAuthor = "Narwin";
            num_PosY.ThemeName = "MetroDark";
            num_PosY.Value = 0;
            // 
            // num_PosZ
            // 
            num_PosZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_PosZ.BackColor = System.Drawing.Color.Transparent;
            num_PosZ.BackgroundColor = System.Drawing.Color.Empty;
            num_PosZ.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosZ.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_PosZ.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosZ.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_PosZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_PosZ.IsDerivedStyle = true;
            num_PosZ.Location = new System.Drawing.Point( 64, 193 );
            num_PosZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_PosZ.Maximum = 999999;
            num_PosZ.Minimum = -999999;
            num_PosZ.Name = "num_PosZ";
            num_PosZ.Size = new System.Drawing.Size( 156, 26 );
            num_PosZ.Style = MetroSet_UI.Enums.Style.Dark;
            num_PosZ.StyleManager = null;
            num_PosZ.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_PosZ.TabIndex = 9;
            num_PosZ.ThemeAuthor = "Narwin";
            num_PosZ.ThemeName = "MetroDark";
            num_PosZ.Value = 0;
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
            num_RotX.BackColor = System.Drawing.Color.Transparent;
            num_RotX.BackgroundColor = System.Drawing.Color.Empty;
            num_RotX.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotX.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_RotX.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotX.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotX.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_RotX.IsDerivedStyle = true;
            num_RotX.Location = new System.Drawing.Point( 61, 75 );
            num_RotX.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotX.Maximum = 999;
            num_RotX.Minimum = -999;
            num_RotX.Name = "num_RotX";
            num_RotX.Size = new System.Drawing.Size( 159, 26 );
            num_RotX.Style = MetroSet_UI.Enums.Style.Dark;
            num_RotX.StyleManager = null;
            num_RotX.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotX.TabIndex = 4;
            num_RotX.ThemeAuthor = "Narwin";
            num_RotX.ThemeName = "MetroDark";
            num_RotX.Value = 0;
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
            num_RotY.BackColor = System.Drawing.Color.Transparent;
            num_RotY.BackgroundColor = System.Drawing.Color.Empty;
            num_RotY.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotY.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_RotY.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotY.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotY.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_RotY.IsDerivedStyle = true;
            num_RotY.Location = new System.Drawing.Point( 61, 134 );
            num_RotY.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotY.Maximum = 999;
            num_RotY.Minimum = -999;
            num_RotY.Name = "num_RotY";
            num_RotY.Size = new System.Drawing.Size( 159, 26 );
            num_RotY.Style = MetroSet_UI.Enums.Style.Dark;
            num_RotY.StyleManager = null;
            num_RotY.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotY.TabIndex = 7;
            num_RotY.ThemeAuthor = "Narwin";
            num_RotY.ThemeName = "MetroDark";
            num_RotY.Value = 0;
            // 
            // num_RotZ
            // 
            num_RotZ.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            num_RotZ.BackColor = System.Drawing.Color.Transparent;
            num_RotZ.BackgroundColor = System.Drawing.Color.Empty;
            num_RotZ.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotZ.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_RotZ.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotZ.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotZ.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_RotZ.IsDerivedStyle = true;
            num_RotZ.Location = new System.Drawing.Point( 61, 193 );
            num_RotZ.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotZ.Maximum = 999;
            num_RotZ.Minimum = -999;
            num_RotZ.Name = "num_RotZ";
            num_RotZ.Size = new System.Drawing.Size( 159, 26 );
            num_RotZ.Style = MetroSet_UI.Enums.Style.Dark;
            num_RotZ.StyleManager = null;
            num_RotZ.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotZ.TabIndex = 9;
            num_RotZ.ThemeAuthor = "Narwin";
            num_RotZ.ThemeName = "MetroDark";
            num_RotZ.Value = 0;
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
            num_RotW.BackColor = System.Drawing.Color.Transparent;
            num_RotW.BackgroundColor = System.Drawing.Color.Empty;
            num_RotW.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotW.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            num_RotW.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotW.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            num_RotW.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            num_RotW.IsDerivedStyle = true;
            num_RotW.Location = new System.Drawing.Point( 61, 254 );
            num_RotW.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
            num_RotW.Maximum = 999;
            num_RotW.Minimum = 0;
            num_RotW.Name = "num_RotW";
            num_RotW.Size = new System.Drawing.Size( 159, 26 );
            num_RotW.Style = MetroSet_UI.Enums.Style.Dark;
            num_RotW.StyleManager = null;
            num_RotW.SymbolsColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            num_RotW.TabIndex = 11;
            num_RotW.ThemeAuthor = "Narwin";
            num_RotW.ThemeName = "MetroDark";
            num_RotW.Value = 1;
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
            AutoScaleDimensions = new System.Drawing.SizeF( 120F, 120F );
            
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
            tableLayoutPanel_Pos.ResumeLayout( false );
            tableLayoutPanel_Pos.PerformLayout();
            tableLayoutPanel_Rot.ResumeLayout( false );
            tableLayoutPanel_Rot.PerformLayout();
            tableLayoutPanel1.ResumeLayout( false );
            ResumeLayout( false );
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Scale;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private MetroSetLabel lbl_scaleX;
        private MetroSetNumeric num_ScaleX;
        private MetroSetLabel lbl_ScaleZ;
        private MetroSetLabel lbl_Scale;
        private MetroSetLabel lbl_ScaleY;
        private MetroSetNumeric num_ScaleY;
        private MetroSetNumeric num_ScaleZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Pos;
        private MetroSetLabel lbl_PosZ;
        private MetroSetLabel lbl_PosX;
        private MetroSetNumeric num_PosX;
        private MetroSetLabel lbl_Position;
        private MetroSetLabel lbl_PosY;
        private MetroSetNumeric num_PosY;
        private MetroSetNumeric num_PosZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Rot;
        private MetroSetLabel lbl_RotW;
        private MetroSetLabel lbl_RotZ;
        private MetroSetLabel lbl_RotX;
        private MetroSetNumeric num_RotX;
        private MetroSetLabel lbl_RotY;
        private MetroSetNumeric num_RotY;
        private MetroSetNumeric num_RotZ;
        private MetroSetLabel lbl_Rotation;
        private MetroSetNumeric num_RotW;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}