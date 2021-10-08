namespace GFDStudio.GUI.Forms
{
    partial class SetScaleValueDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetScaleValueDialog));
            this.tableLayoutPanel_Scale = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_ScaleZ = new System.Windows.Forms.Label();
            this.lbl_scaleX = new System.Windows.Forms.Label();
            this.num_ScaleX = new System.Windows.Forms.NumericUpDown();
            this.lbl_ScaleY = new System.Windows.Forms.Label();
            this.num_ScaleY = new System.Windows.Forms.NumericUpDown();
            this.num_ScaleZ = new System.Windows.Forms.NumericUpDown();
            this.lbl_Scale = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Pos = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_PosZ = new System.Windows.Forms.Label();
            this.lbl_PosX = new System.Windows.Forms.Label();
            this.num_PosX = new System.Windows.Forms.NumericUpDown();
            this.lbl_PosY = new System.Windows.Forms.Label();
            this.num_PosY = new System.Windows.Forms.NumericUpDown();
            this.num_PosZ = new System.Windows.Forms.NumericUpDown();
            this.lbl_Position = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Rot = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_RotW = new System.Windows.Forms.Label();
            this.lbl_RotZ = new System.Windows.Forms.Label();
            this.lbl_RotX = new System.Windows.Forms.Label();
            this.num_RotX = new System.Windows.Forms.NumericUpDown();
            this.lbl_RotY = new System.Windows.Forms.Label();
            this.num_RotY = new System.Windows.Forms.NumericUpDown();
            this.num_RotZ = new System.Windows.Forms.NumericUpDown();
            this.lbl_Rotation = new System.Windows.Forms.Label();
            this.num_RotW = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel_Scale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleZ)).BeginInit();
            this.tableLayoutPanel_Pos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosZ)).BeginInit();
            this.tableLayoutPanel_Rot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotW)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Scale
            // 
            this.tableLayoutPanel_Scale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Scale.ColumnCount = 2;
            this.tableLayoutPanel_Scale.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.tableLayoutPanel_Scale.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.tableLayoutPanel_Scale.Controls.Add(this.lbl_ScaleZ, 0, 3);
            this.tableLayoutPanel_Scale.Controls.Add(this.lbl_scaleX, 0, 1);
            this.tableLayoutPanel_Scale.Controls.Add(this.num_ScaleX, 1, 1);
            this.tableLayoutPanel_Scale.Controls.Add(this.lbl_ScaleY, 0, 2);
            this.tableLayoutPanel_Scale.Controls.Add(this.num_ScaleY, 1, 2);
            this.tableLayoutPanel_Scale.Controls.Add(this.num_ScaleZ, 1, 3);
            this.tableLayoutPanel_Scale.Controls.Add(this.lbl_Scale, 1, 0);
            this.tableLayoutPanel_Scale.Location = new System.Drawing.Point(16, 18);
            this.tableLayoutPanel_Scale.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel_Scale.Name = "tableLayoutPanel_Scale";
            this.tableLayoutPanel_Scale.RowCount = 4;
            this.tableLayoutPanel_Scale.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Scale.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Scale.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel_Scale.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel_Scale.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel_Scale.Size = new System.Drawing.Size(111, 149);
            this.tableLayoutPanel_Scale.TabIndex = 0;
            // 
            // lbl_ScaleZ
            // 
            this.lbl_ScaleZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_ScaleZ.AutoSize = true;
            this.lbl_ScaleZ.Location = new System.Drawing.Point(4, 120);
            this.lbl_ScaleZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_ScaleZ.Name = "lbl_ScaleZ";
            this.lbl_ScaleZ.Size = new System.Drawing.Size(13, 20);
            this.lbl_ScaleZ.TabIndex = 8;
            this.lbl_ScaleZ.Text = "Z";
            // 
            // lbl_scaleX
            // 
            this.lbl_scaleX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_scaleX.AutoSize = true;
            this.lbl_scaleX.Location = new System.Drawing.Point(4, 41);
            this.lbl_scaleX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_scaleX.Name = "lbl_scaleX";
            this.lbl_scaleX.Size = new System.Drawing.Size(13, 20);
            this.lbl_scaleX.TabIndex = 3;
            this.lbl_scaleX.Text = "X";
            // 
            // num_ScaleX
            // 
            this.num_ScaleX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_ScaleX.DecimalPlaces = 2;
            this.num_ScaleX.Location = new System.Drawing.Point(25, 39);
            this.num_ScaleX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_ScaleX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleX.Name = "num_ScaleX";
            this.num_ScaleX.Size = new System.Drawing.Size(70, 27);
            this.num_ScaleX.TabIndex = 4;
            this.num_ScaleX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbl_ScaleY
            // 
            this.lbl_ScaleY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_ScaleY.AutoSize = true;
            this.lbl_ScaleY.Location = new System.Drawing.Point(4, 79);
            this.lbl_ScaleY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_ScaleY.Name = "lbl_ScaleY";
            this.lbl_ScaleY.Size = new System.Drawing.Size(13, 20);
            this.lbl_ScaleY.TabIndex = 6;
            this.lbl_ScaleY.Text = "Y";
            // 
            // num_ScaleY
            // 
            this.num_ScaleY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_ScaleY.DecimalPlaces = 2;
            this.num_ScaleY.Location = new System.Drawing.Point(25, 76);
            this.num_ScaleY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_ScaleY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleY.Name = "num_ScaleY";
            this.num_ScaleY.Size = new System.Drawing.Size(70, 27);
            this.num_ScaleY.TabIndex = 7;
            this.num_ScaleY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // num_ScaleZ
            // 
            this.num_ScaleZ.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_ScaleZ.DecimalPlaces = 2;
            this.num_ScaleZ.Location = new System.Drawing.Point(25, 116);
            this.num_ScaleZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_ScaleZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleZ.Name = "num_ScaleZ";
            this.num_ScaleZ.Size = new System.Drawing.Size(70, 27);
            this.num_ScaleZ.TabIndex = 9;
            this.num_ScaleZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbl_Scale
            // 
            this.lbl_Scale.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Scale.AutoSize = true;
            this.lbl_Scale.Location = new System.Drawing.Point(34, 0);
            this.lbl_Scale.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Scale.Name = "lbl_Scale";
            this.lbl_Scale.Size = new System.Drawing.Size(73, 34);
            this.lbl_Scale.TabIndex = 5;
            this.lbl_Scale.Text = "Scale Multiplier";
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(20, 183);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(13, 15, 13, 15);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 35);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(139, 183);
            this.OKButton.Margin = new System.Windows.Forms.Padding(13, 15, 13, 15);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(100, 35);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // tableLayoutPanel_Pos
            // 
            this.tableLayoutPanel_Pos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Pos.ColumnCount = 2;
            this.tableLayoutPanel_Pos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.02703F));
            this.tableLayoutPanel_Pos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.97298F));
            this.tableLayoutPanel_Pos.Controls.Add(this.lbl_PosZ, 0, 3);
            this.tableLayoutPanel_Pos.Controls.Add(this.lbl_PosX, 0, 1);
            this.tableLayoutPanel_Pos.Controls.Add(this.num_PosX, 1, 1);
            this.tableLayoutPanel_Pos.Controls.Add(this.lbl_PosY, 0, 2);
            this.tableLayoutPanel_Pos.Controls.Add(this.num_PosY, 1, 2);
            this.tableLayoutPanel_Pos.Controls.Add(this.num_PosZ, 1, 3);
            this.tableLayoutPanel_Pos.Controls.Add(this.lbl_Position, 1, 0);
            this.tableLayoutPanel_Pos.Location = new System.Drawing.Point(135, 20);
            this.tableLayoutPanel_Pos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel_Pos.Name = "tableLayoutPanel_Pos";
            this.tableLayoutPanel_Pos.RowCount = 4;
            this.tableLayoutPanel_Pos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.05882F));
            this.tableLayoutPanel_Pos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.94118F));
            this.tableLayoutPanel_Pos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel_Pos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel_Pos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel_Pos.Size = new System.Drawing.Size(111, 149);
            this.tableLayoutPanel_Pos.TabIndex = 3;
            // 
            // lbl_PosZ
            // 
            this.lbl_PosZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_PosZ.AutoSize = true;
            this.lbl_PosZ.Location = new System.Drawing.Point(8, 120);
            this.lbl_PosZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_PosZ.Name = "lbl_PosZ";
            this.lbl_PosZ.Size = new System.Drawing.Size(18, 20);
            this.lbl_PosZ.TabIndex = 8;
            this.lbl_PosZ.Text = "Z";
            // 
            // lbl_PosX
            // 
            this.lbl_PosX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_PosX.AutoSize = true;
            this.lbl_PosX.Location = new System.Drawing.Point(8, 40);
            this.lbl_PosX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_PosX.Name = "lbl_PosX";
            this.lbl_PosX.Size = new System.Drawing.Size(18, 20);
            this.lbl_PosX.TabIndex = 3;
            this.lbl_PosX.Text = "X";
            // 
            // num_PosX
            // 
            this.num_PosX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosX.DecimalPlaces = 2;
            this.num_PosX.Location = new System.Drawing.Point(34, 37);
            this.num_PosX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_PosX.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_PosX.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_PosX.Name = "num_PosX";
            this.num_PosX.Size = new System.Drawing.Size(70, 27);
            this.num_PosX.TabIndex = 4;
            // 
            // lbl_PosY
            // 
            this.lbl_PosY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_PosY.AutoSize = true;
            this.lbl_PosY.Location = new System.Drawing.Point(9, 79);
            this.lbl_PosY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_PosY.Name = "lbl_PosY";
            this.lbl_PosY.Size = new System.Drawing.Size(17, 20);
            this.lbl_PosY.TabIndex = 6;
            this.lbl_PosY.Text = "Y";
            // 
            // num_PosY
            // 
            this.num_PosY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosY.DecimalPlaces = 2;
            this.num_PosY.Location = new System.Drawing.Point(34, 76);
            this.num_PosY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_PosY.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_PosY.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_PosY.Name = "num_PosY";
            this.num_PosY.Size = new System.Drawing.Size(70, 27);
            this.num_PosY.TabIndex = 7;
            // 
            // num_PosZ
            // 
            this.num_PosZ.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosZ.DecimalPlaces = 2;
            this.num_PosZ.Location = new System.Drawing.Point(34, 116);
            this.num_PosZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_PosZ.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_PosZ.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_PosZ.Name = "num_PosZ";
            this.num_PosZ.Size = new System.Drawing.Size(70, 27);
            this.num_PosZ.TabIndex = 9;
            // 
            // lbl_Position
            // 
            this.lbl_Position.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Position.AutoSize = true;
            this.lbl_Position.Location = new System.Drawing.Point(42, 0);
            this.lbl_Position.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Position.Name = "lbl_Position";
            this.lbl_Position.Size = new System.Drawing.Size(65, 32);
            this.lbl_Position.TabIndex = 5;
            this.lbl_Position.Text = "Position Offset";
            // 
            // tableLayoutPanel_Rot
            // 
            this.tableLayoutPanel_Rot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Rot.ColumnCount = 2;
            this.tableLayoutPanel_Rot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.71428F));
            this.tableLayoutPanel_Rot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.28571F));
            this.tableLayoutPanel_Rot.Controls.Add(this.lbl_RotW, 0, 4);
            this.tableLayoutPanel_Rot.Controls.Add(this.lbl_RotZ, 0, 3);
            this.tableLayoutPanel_Rot.Controls.Add(this.lbl_RotX, 0, 1);
            this.tableLayoutPanel_Rot.Controls.Add(this.num_RotX, 1, 1);
            this.tableLayoutPanel_Rot.Controls.Add(this.lbl_RotY, 0, 2);
            this.tableLayoutPanel_Rot.Controls.Add(this.num_RotY, 1, 2);
            this.tableLayoutPanel_Rot.Controls.Add(this.num_RotZ, 1, 3);
            this.tableLayoutPanel_Rot.Controls.Add(this.lbl_Rotation, 1, 0);
            this.tableLayoutPanel_Rot.Controls.Add(this.num_RotW, 1, 4);
            this.tableLayoutPanel_Rot.Location = new System.Drawing.Point(254, 20);
            this.tableLayoutPanel_Rot.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel_Rot.Name = "tableLayoutPanel_Rot";
            this.tableLayoutPanel_Rot.RowCount = 5;
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.71429F));
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.28571F));
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel_Rot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel_Rot.Size = new System.Drawing.Size(131, 186);
            this.tableLayoutPanel_Rot.TabIndex = 10;
            // 
            // lbl_RotW
            // 
            this.lbl_RotW.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_RotW.AutoSize = true;
            this.lbl_RotW.Location = new System.Drawing.Point(6, 156);
            this.lbl_RotW.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_RotW.Name = "lbl_RotW";
            this.lbl_RotW.Size = new System.Drawing.Size(23, 20);
            this.lbl_RotW.TabIndex = 10;
            this.lbl_RotW.Text = "W";
            // 
            // lbl_RotZ
            // 
            this.lbl_RotZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_RotZ.AutoSize = true;
            this.lbl_RotZ.Location = new System.Drawing.Point(11, 119);
            this.lbl_RotZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_RotZ.Name = "lbl_RotZ";
            this.lbl_RotZ.Size = new System.Drawing.Size(18, 20);
            this.lbl_RotZ.TabIndex = 8;
            this.lbl_RotZ.Text = "Z";
            // 
            // lbl_RotX
            // 
            this.lbl_RotX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_RotX.AutoSize = true;
            this.lbl_RotX.Location = new System.Drawing.Point(11, 39);
            this.lbl_RotX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_RotX.Name = "lbl_RotX";
            this.lbl_RotX.Size = new System.Drawing.Size(18, 20);
            this.lbl_RotX.TabIndex = 3;
            this.lbl_RotX.Text = "X";
            // 
            // num_RotX
            // 
            this.num_RotX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_RotX.DecimalPlaces = 7;
            this.num_RotX.Location = new System.Drawing.Point(37, 36);
            this.num_RotX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_RotX.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_RotX.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_RotX.Name = "num_RotX";
            this.num_RotX.Size = new System.Drawing.Size(90, 27);
            this.num_RotX.TabIndex = 4;
            // 
            // lbl_RotY
            // 
            this.lbl_RotY.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_RotY.AutoSize = true;
            this.lbl_RotY.Location = new System.Drawing.Point(12, 80);
            this.lbl_RotY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_RotY.Name = "lbl_RotY";
            this.lbl_RotY.Size = new System.Drawing.Size(17, 20);
            this.lbl_RotY.TabIndex = 6;
            this.lbl_RotY.Text = "Y";
            // 
            // num_RotY
            // 
            this.num_RotY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_RotY.DecimalPlaces = 7;
            this.num_RotY.Location = new System.Drawing.Point(37, 76);
            this.num_RotY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_RotY.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_RotY.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_RotY.Name = "num_RotY";
            this.num_RotY.Size = new System.Drawing.Size(90, 27);
            this.num_RotY.TabIndex = 7;
            // 
            // num_RotZ
            // 
            this.num_RotZ.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_RotZ.DecimalPlaces = 7;
            this.num_RotZ.Location = new System.Drawing.Point(37, 117);
            this.num_RotZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_RotZ.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_RotZ.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_RotZ.Name = "num_RotZ";
            this.num_RotZ.Size = new System.Drawing.Size(90, 27);
            this.num_RotZ.TabIndex = 9;
            // 
            // lbl_Rotation
            // 
            this.lbl_Rotation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Rotation.AutoSize = true;
            this.lbl_Rotation.Location = new System.Drawing.Point(61, 5);
            this.lbl_Rotation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Rotation.Name = "lbl_Rotation";
            this.lbl_Rotation.Size = new System.Drawing.Size(66, 20);
            this.lbl_Rotation.TabIndex = 5;
            this.lbl_Rotation.Text = "Rotation";
            // 
            // num_RotW
            // 
            this.num_RotW.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_RotW.DecimalPlaces = 7;
            this.num_RotW.Location = new System.Drawing.Point(37, 153);
            this.num_RotW.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.num_RotW.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.num_RotW.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.num_RotW.Name = "num_RotW";
            this.num_RotW.Size = new System.Drawing.Size(90, 27);
            this.num_RotW.TabIndex = 11;
            this.num_RotW.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SetScaleValueDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 242);
            this.Controls.Add(this.tableLayoutPanel_Rot);
            this.Controls.Add(this.tableLayoutPanel_Pos);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.tableLayoutPanel_Scale);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "SetScaleValueDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scale/Reposition";
            this.tableLayoutPanel_Scale.ResumeLayout(false);
            this.tableLayoutPanel_Scale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleZ)).EndInit();
            this.tableLayoutPanel_Pos.ResumeLayout(false);
            this.tableLayoutPanel_Pos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosZ)).EndInit();
            this.tableLayoutPanel_Rot.ResumeLayout(false);
            this.tableLayoutPanel_Rot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_RotW)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Scale;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label lbl_scaleX;
        private System.Windows.Forms.NumericUpDown num_ScaleX;
        private System.Windows.Forms.Label lbl_ScaleZ;
        private System.Windows.Forms.Label lbl_Scale;
        private System.Windows.Forms.Label lbl_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Pos;
        private System.Windows.Forms.Label lbl_PosZ;
        private System.Windows.Forms.Label lbl_PosX;
        private System.Windows.Forms.NumericUpDown num_PosX;
        private System.Windows.Forms.Label lbl_Position;
        private System.Windows.Forms.Label lbl_PosY;
        private System.Windows.Forms.NumericUpDown num_PosY;
        private System.Windows.Forms.NumericUpDown num_PosZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Rot;
        private System.Windows.Forms.Label lbl_RotW;
        private System.Windows.Forms.Label lbl_RotZ;
        private System.Windows.Forms.Label lbl_RotX;
        private System.Windows.Forms.NumericUpDown num_RotX;
        private System.Windows.Forms.Label lbl_RotY;
        private System.Windows.Forms.NumericUpDown num_RotY;
        private System.Windows.Forms.NumericUpDown num_RotZ;
        private System.Windows.Forms.Label lbl_Rotation;
        private System.Windows.Forms.NumericUpDown num_RotW;
    }
}