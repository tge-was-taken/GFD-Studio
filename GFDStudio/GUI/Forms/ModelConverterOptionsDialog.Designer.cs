namespace GFDStudio.GUI.Forms
{
    partial class ModelConverterOptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelConverterOptionsDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EditPresetButton = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.MaterialPresetLabel = new System.Windows.Forms.Label();
            this.MaterialPresetComboBox = new System.Windows.Forms.ComboBox();
            this.VersionTextBox = new System.Windows.Forms.TextBox();
            this.ConvertSkinToZUpLabel = new System.Windows.Forms.Label();
            this.ConvertSkinToZUpCheckBox = new System.Windows.Forms.CheckBox();
            this.GenerateVertexColorsCheckBox = new System.Windows.Forms.CheckBox();
            this.GenerateVertexColorsLabel = new System.Windows.Forms.Label();
            this.minVertexAttributesLabel = new System.Windows.Forms.Label();
            this.MinimalVertexAttributesCheckBox = new System.Windows.Forms.CheckBox();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.descriptionTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.NewPresetButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel1.Controls.Add(this.EditPresetButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.VersionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MaterialPresetLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MaterialPresetComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.VersionTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ConvertSkinToZUpLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ConvertSkinToZUpCheckBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.GenerateVertexColorsCheckBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.GenerateVertexColorsLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.minVertexAttributesLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.MinimalVertexAttributesCheckBox, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 14);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(428, 143);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // EditPresetButton
            // 
            this.EditPresetButton.Location = new System.Drawing.Point(345, 31);
            this.EditPresetButton.Name = "EditPresetButton";
            this.EditPresetButton.Size = new System.Drawing.Size(75, 22);
            this.EditPresetButton.TabIndex = 3;
            this.EditPresetButton.Text = "Edit";
            this.EditPresetButton.UseVisualStyleBackColor = true;
            this.EditPresetButton.Click += new System.EventHandler(this.EditPresetButton_Click);
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(98, 6);
            this.VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(45, 15);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "Version";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.descriptionTooltip.SetToolTip(this.VersionLabel, "Resource version to use on model/animation pack.\r\n\r\nKnown resource versions:\r\n0x0" +
        "1105030 - P4D\r\n0x01105070 - P5\r\n0x01105090 - P3D & P5D\r\n0x01105100 - P5R & CFB");
            // 
            // MaterialPresetLabel
            // 
            this.MaterialPresetLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MaterialPresetLabel.AutoSize = true;
            this.MaterialPresetLabel.Location = new System.Drawing.Point(58, 34);
            this.MaterialPresetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MaterialPresetLabel.Name = "MaterialPresetLabel";
            this.MaterialPresetLabel.Size = new System.Drawing.Size(85, 15);
            this.MaterialPresetLabel.TabIndex = 0;
            this.MaterialPresetLabel.Text = "Material preset";
            this.descriptionTooltip.SetToolTip(this.MaterialPresetLabel, "Material Preset to apply to converted model.");
            // 
            // MaterialPresetComboBox
            // 
            this.MaterialPresetComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MaterialPresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MaterialPresetComboBox.Location = new System.Drawing.Point(151, 31);
            this.MaterialPresetComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            this.MaterialPresetComboBox.Size = new System.Drawing.Size(187, 23);
            this.MaterialPresetComboBox.TabIndex = 1;
            // 
            // VersionTextBox
            // 
            this.VersionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.VersionTextBox.Location = new System.Drawing.Point(151, 3);
            this.VersionTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.VersionTextBox.Name = "VersionTextBox";
            this.VersionTextBox.Size = new System.Drawing.Size(187, 23);
            this.VersionTextBox.TabIndex = 3;
            // 
            // ConvertSkinToZUpLabel
            // 
            this.ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConvertSkinToZUpLabel.AutoSize = true;
            this.ConvertSkinToZUpLabel.Location = new System.Drawing.Point(29, 62);
            this.ConvertSkinToZUpLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ConvertSkinToZUpLabel.Name = "ConvertSkinToZUpLabel";
            this.ConvertSkinToZUpLabel.Size = new System.Drawing.Size(114, 15);
            this.ConvertSkinToZUpLabel.TabIndex = 4;
            this.ConvertSkinToZUpLabel.Text = "Convert skin to Z up";
            this.descriptionTooltip.SetToolTip(this.ConvertSkinToZUpLabel, "Convert the up axis of the inverse bind pose matrices to Z-up.");
            // 
            // ConvertSkinToZUpCheckBox
            // 
            this.ConvertSkinToZUpCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConvertSkinToZUpCheckBox.AutoSize = true;
            this.ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point(151, 63);
            this.ConvertSkinToZUpCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConvertSkinToZUpCheckBox.Name = "ConvertSkinToZUpCheckBox";
            this.ConvertSkinToZUpCheckBox.Size = new System.Drawing.Size(15, 14);
            this.ConvertSkinToZUpCheckBox.TabIndex = 5;
            this.ConvertSkinToZUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateVertexColorsCheckBox
            // 
            this.GenerateVertexColorsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GenerateVertexColorsCheckBox.AutoSize = true;
            this.GenerateVertexColorsCheckBox.Location = new System.Drawing.Point(151, 91);
            this.GenerateVertexColorsCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GenerateVertexColorsCheckBox.Name = "GenerateVertexColorsCheckBox";
            this.GenerateVertexColorsCheckBox.Size = new System.Drawing.Size(15, 14);
            this.GenerateVertexColorsCheckBox.TabIndex = 6;
            this.GenerateVertexColorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateVertexColorsLabel
            // 
            this.GenerateVertexColorsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GenerateVertexColorsLabel.AutoSize = true;
            this.GenerateVertexColorsLabel.Location = new System.Drawing.Point(19, 90);
            this.GenerateVertexColorsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GenerateVertexColorsLabel.Name = "GenerateVertexColorsLabel";
            this.GenerateVertexColorsLabel.Size = new System.Drawing.Size(124, 15);
            this.GenerateVertexColorsLabel.TabIndex = 7;
            this.GenerateVertexColorsLabel.Text = "Generate vertex colors";
            this.descriptionTooltip.SetToolTip(this.GenerateVertexColorsLabel, "Generates dummy white vertex colors for models with none.\r\nFixes vertex explosion" +
        " bug in P4D when vertex colors are missing.");
            // 
            // minVertexAttributesLabel
            // 
            this.minVertexAttributesLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.minVertexAttributesLabel.AutoSize = true;
            this.minVertexAttributesLabel.Location = new System.Drawing.Point(3, 120);
            this.minVertexAttributesLabel.Name = "minVertexAttributesLabel";
            this.minVertexAttributesLabel.Size = new System.Drawing.Size(141, 15);
            this.minVertexAttributesLabel.TabIndex = 8;
            this.minVertexAttributesLabel.Text = "Minimal Vertex Attributes";
            this.minVertexAttributesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.descriptionTooltip.SetToolTip(this.minVertexAttributesLabel, resources.GetString("minVertexAttributesLabel.ToolTip"));
            // 
            // MinimalVertexAttributesCheckBox
            // 
            this.MinimalVertexAttributesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MinimalVertexAttributesCheckBox.AutoSize = true;
            this.MinimalVertexAttributesCheckBox.Checked = true;
            this.MinimalVertexAttributesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MinimalVertexAttributesCheckBox.Location = new System.Drawing.Point(151, 120);
            this.MinimalVertexAttributesCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimalVertexAttributesCheckBox.Name = "MinimalVertexAttributesCheckBox";
            this.MinimalVertexAttributesCheckBox.Size = new System.Drawing.Size(15, 14);
            this.MinimalVertexAttributesCheckBox.TabIndex = 9;
            this.MinimalVertexAttributesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConvertButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ConvertButton.Location = new System.Drawing.Point(346, 177);
            this.ConvertButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(88, 27);
            this.ConvertButton.TabIndex = 1;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(250, 177);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(88, 27);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // descriptionTooltip
            // 
            this.descriptionTooltip.AutoPopDelay = 10000;
            this.descriptionTooltip.InitialDelay = 500;
            this.descriptionTooltip.ReshowDelay = 100;
            // 
            // NewPresetButton
            // 
            this.NewPresetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.NewPresetButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NewPresetButton.Location = new System.Drawing.Point(13, 177);
            this.NewPresetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NewPresetButton.Name = "NewPresetButton";
            this.NewPresetButton.Size = new System.Drawing.Size(88, 27);
            this.NewPresetButton.TabIndex = 3;
            this.NewPresetButton.Text = "New preset";
            this.NewPresetButton.UseVisualStyleBackColor = true;
            this.NewPresetButton.Click += new System.EventHandler(this.NewPresetButton_Click);
            // 
            // ModelConverterOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 216);
            this.Controls.Add(this.NewPresetButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ModelConverterOptionsDialog";
            this.Text = "Model converter options";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label MaterialPresetLabel;
        private System.Windows.Forms.ComboBox MaterialPresetComboBox;
        private System.Windows.Forms.TextBox VersionTextBox;
        private System.Windows.Forms.Label ConvertSkinToZUpLabel;
        private System.Windows.Forms.CheckBox ConvertSkinToZUpCheckBox;
        private System.Windows.Forms.CheckBox GenerateVertexColorsCheckBox;
        private System.Windows.Forms.Label GenerateVertexColorsLabel;
        private System.Windows.Forms.Button ConvertButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.CheckBox MinimalVertexAttributesCheckBox;
        private System.Windows.Forms.Label minVertexAttributesLabel;
        private System.Windows.Forms.ToolTip descriptionTooltip;
        private System.Windows.Forms.Button EditPresetButton;
        private System.Windows.Forms.Button NewPresetButton;
    }
}