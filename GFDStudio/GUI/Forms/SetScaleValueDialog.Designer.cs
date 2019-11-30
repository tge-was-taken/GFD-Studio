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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_ScaleZ = new System.Windows.Forms.Label();
            this.lbl_scaleX = new System.Windows.Forms.Label();
            this.num_ScaleX = new System.Windows.Forms.NumericUpDown();
            this.lbl_ScaleY = new System.Windows.Forms.Label();
            this.num_ScaleY = new System.Windows.Forms.NumericUpDown();
            this.num_ScaleZ = new System.Windows.Forms.NumericUpDown();
            this.lbl_Scale = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.num_PosX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.num_PosY = new System.Windows.Forms.NumericUpDown();
            this.num_PosZ = new System.Windows.Forms.NumericUpDown();
            this.lbl_Position = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleZ)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosZ)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.15068F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.84932F));
            this.tableLayoutPanel1.Controls.Add(this.lbl_ScaleZ, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_scaleX, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.num_ScaleX, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_ScaleY, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.num_ScaleY, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.num_ScaleZ, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_Scale, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(146, 97);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbl_ScaleZ
            // 
            this.lbl_ScaleZ.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_ScaleZ.AutoSize = true;
            this.lbl_ScaleZ.Location = new System.Drawing.Point(45, 78);
            this.lbl_ScaleZ.Name = "lbl_ScaleZ";
            this.lbl_ScaleZ.Size = new System.Drawing.Size(14, 13);
            this.lbl_ScaleZ.TabIndex = 8;
            this.lbl_ScaleZ.Text = "Z";
            // 
            // lbl_scaleX
            // 
            this.lbl_scaleX.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_scaleX.AutoSize = true;
            this.lbl_scaleX.Location = new System.Drawing.Point(45, 26);
            this.lbl_scaleX.Name = "lbl_scaleX";
            this.lbl_scaleX.Size = new System.Drawing.Size(14, 13);
            this.lbl_scaleX.TabIndex = 3;
            this.lbl_scaleX.Text = "X";
            // 
            // num_ScaleX
            // 
            this.num_ScaleX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_ScaleX.DecimalPlaces = 2;
            this.num_ScaleX.Location = new System.Drawing.Point(65, 25);
            this.num_ScaleX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleX.Name = "num_ScaleX";
            this.num_ScaleX.Size = new System.Drawing.Size(76, 20);
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
            this.lbl_ScaleY.Location = new System.Drawing.Point(45, 51);
            this.lbl_ScaleY.Name = "lbl_ScaleY";
            this.lbl_ScaleY.Size = new System.Drawing.Size(14, 13);
            this.lbl_ScaleY.TabIndex = 6;
            this.lbl_ScaleY.Text = "Y";
            // 
            // num_ScaleY
            // 
            this.num_ScaleY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_ScaleY.DecimalPlaces = 2;
            this.num_ScaleY.Location = new System.Drawing.Point(65, 48);
            this.num_ScaleY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleY.Name = "num_ScaleY";
            this.num_ScaleY.Size = new System.Drawing.Size(76, 20);
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
            this.num_ScaleZ.Location = new System.Drawing.Point(65, 75);
            this.num_ScaleZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_ScaleZ.Name = "num_ScaleZ";
            this.num_ScaleZ.Size = new System.Drawing.Size(76, 20);
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
            this.lbl_Scale.Location = new System.Drawing.Point(65, 4);
            this.lbl_Scale.Name = "lbl_Scale";
            this.lbl_Scale.Size = new System.Drawing.Size(78, 13);
            this.lbl_Scale.TabIndex = 5;
            this.lbl_Scale.Text = "Scale Multiplier";
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(60, 122);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(10);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(149, 122);
            this.OKButton.Margin = new System.Windows.Forms.Padding(10);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.36364F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.63636F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.num_PosX, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.num_PosY, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.num_PosZ, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.lbl_Position, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(164, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(128, 97);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Z";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "X";
            // 
            // num_PosX
            // 
            this.num_PosX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosX.DecimalPlaces = 2;
            this.num_PosX.Location = new System.Drawing.Point(49, 25);
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
            this.num_PosX.Size = new System.Drawing.Size(76, 20);
            this.num_PosX.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Y";
            // 
            // num_PosY
            // 
            this.num_PosY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosY.DecimalPlaces = 2;
            this.num_PosY.Location = new System.Drawing.Point(49, 48);
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
            this.num_PosY.Size = new System.Drawing.Size(76, 20);
            this.num_PosY.TabIndex = 7;
            // 
            // num_PosZ
            // 
            this.num_PosZ.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_PosZ.DecimalPlaces = 2;
            this.num_PosZ.Location = new System.Drawing.Point(49, 75);
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
            this.num_PosZ.Size = new System.Drawing.Size(76, 20);
            this.num_PosZ.TabIndex = 9;
            // 
            // lbl_Position
            // 
            this.lbl_Position.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Position.AutoSize = true;
            this.lbl_Position.Location = new System.Drawing.Point(50, 4);
            this.lbl_Position.Name = "lbl_Position";
            this.lbl_Position.Size = new System.Drawing.Size(75, 13);
            this.lbl_Position.TabIndex = 5;
            this.lbl_Position.Text = "Position Offset";
            // 
            // SetScaleValueDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 155);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SetScaleValueDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scale/Reposition";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ScaleZ)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_PosZ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label lbl_scaleX;
        private System.Windows.Forms.NumericUpDown num_ScaleX;
        private System.Windows.Forms.Label lbl_ScaleZ;
        private System.Windows.Forms.Label lbl_Scale;
        private System.Windows.Forms.Label lbl_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleY;
        private System.Windows.Forms.NumericUpDown num_ScaleZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_PosX;
        private System.Windows.Forms.Label lbl_Position;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_PosY;
        private System.Windows.Forms.NumericUpDown num_PosZ;
    }
}