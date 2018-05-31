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
            this.VersionLabel = new System.Windows.Forms.Label();
            this.MaterialPresetLabel = new System.Windows.Forms.Label();
            this.MaterialPresetComboBox = new System.Windows.Forms.ComboBox();
            this.VersionTextBox = new System.Windows.Forms.TextBox();
            this.ConvertSkinToZUpLabel = new System.Windows.Forms.Label();
            this.ConvertSkinToZUpCheckBox = new System.Windows.Forms.CheckBox();
            this.GenerateVertexColorsCheckBox = new System.Windows.Forms.CheckBox();
            this.GenerateVertexColorsLabel = new System.Windows.Forms.Label();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.VersionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MaterialPresetLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MaterialPresetComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.VersionTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ConvertSkinToZUpLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ConvertSkinToZUpCheckBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.GenerateVertexColorsCheckBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.GenerateVertexColorsLabel, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 94);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(75, 6);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(42, 13);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "Version";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VersionLabel, "The default value is the version used by Persona 5.\nPersona 4 Dancing All Night f" +
        "or example uses version 0x01105030.\n");
            // 
            // MaterialPresetLabel
            // 
            this.MaterialPresetLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MaterialPresetLabel.AutoSize = true;
            this.MaterialPresetLabel.Location = new System.Drawing.Point(41, 33);
            this.MaterialPresetLabel.Name = "MaterialPresetLabel";
            this.MaterialPresetLabel.Size = new System.Drawing.Size(76, 13);
            this.MaterialPresetLabel.TabIndex = 0;
            this.MaterialPresetLabel.Text = "Material preset";
            this.ToolTip.SetToolTip(this.MaterialPresetLabel, "The material preset is a set of predefined material values based off of materials" +
        " from models in the games.\nNote that some materials require different textures a" +
        "nd/or settings than others.\n");
            // 
            // MaterialPresetComboBox
            // 
            this.MaterialPresetComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MaterialPresetComboBox.FormattingEnabled = true;
            this.MaterialPresetComboBox.Location = new System.Drawing.Point(123, 29);
            this.MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            this.MaterialPresetComboBox.Size = new System.Drawing.Size(161, 21);
            this.MaterialPresetComboBox.TabIndex = 1;
            this.ToolTip.SetToolTip(this.MaterialPresetComboBox, "The material preset is a set of predefined material values based off of materials" +
        " from models in the games.\nNote that some materials require different textures a" +
        "nd/or settings than others.\n");
            // 
            // VersionTextBox
            // 
            this.VersionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.VersionTextBox.Location = new System.Drawing.Point(123, 3);
            this.VersionTextBox.Name = "VersionTextBox";
            this.VersionTextBox.Size = new System.Drawing.Size(161, 20);
            this.VersionTextBox.TabIndex = 3;
            this.ToolTip.SetToolTip(this.VersionTextBox, "The default value is the version used by Persona 5.\nPersona 4 Dancing All Night f" +
        "or example uses version 0x01105030.\n");
            // 
            // ConvertSkinToZUpLabel
            // 
            this.ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConvertSkinToZUpLabel.AutoSize = true;
            this.ConvertSkinToZUpLabel.Location = new System.Drawing.Point(14, 56);
            this.ConvertSkinToZUpLabel.Name = "ConvertSkinToZUpLabel";
            this.ConvertSkinToZUpLabel.Size = new System.Drawing.Size(103, 13);
            this.ConvertSkinToZUpLabel.TabIndex = 4;
            this.ConvertSkinToZUpLabel.Text = "Convert skin to Z up";
            this.ToolTip.SetToolTip(this.ConvertSkinToZUpLabel, "Enable this if you want to replace a Persona 5 battle model.");
            // 
            // ConvertSkinToZUpCheckBox
            // 
            this.ConvertSkinToZUpCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConvertSkinToZUpCheckBox.AutoSize = true;
            this.ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point(123, 56);
            this.ConvertSkinToZUpCheckBox.Name = "ConvertSkinToZUpCheckBox";
            this.ConvertSkinToZUpCheckBox.Size = new System.Drawing.Size(15, 14);
            this.ConvertSkinToZUpCheckBox.TabIndex = 5;
            this.ToolTip.SetToolTip(this.ConvertSkinToZUpCheckBox, "Enable this if you want to replace a Persona 5 battle model.\n");
            this.ConvertSkinToZUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateVertexColorsCheckBox
            // 
            this.GenerateVertexColorsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GenerateVertexColorsCheckBox.AutoSize = true;
            this.GenerateVertexColorsCheckBox.Location = new System.Drawing.Point(123, 76);
            this.GenerateVertexColorsCheckBox.Name = "GenerateVertexColorsCheckBox";
            this.GenerateVertexColorsCheckBox.Size = new System.Drawing.Size(15, 14);
            this.GenerateVertexColorsCheckBox.TabIndex = 6;
            this.ToolTip.SetToolTip(this.GenerateVertexColorsCheckBox, resources.GetString("GenerateVertexColorsCheckBox.ToolTip"));
            this.GenerateVertexColorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // GenerateVertexColorsLabel
            // 
            this.GenerateVertexColorsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GenerateVertexColorsLabel.AutoSize = true;
            this.GenerateVertexColorsLabel.Location = new System.Drawing.Point(3, 77);
            this.GenerateVertexColorsLabel.Name = "GenerateVertexColorsLabel";
            this.GenerateVertexColorsLabel.Size = new System.Drawing.Size(114, 13);
            this.GenerateVertexColorsLabel.TabIndex = 7;
            this.GenerateVertexColorsLabel.Text = "Generate vertex colors";
            this.ToolTip.SetToolTip(this.GenerateVertexColorsLabel, resources.GetString("GenerateVertexColorsLabel.ToolTip"));
            // 
            // ConvertButton
            // 
            this.ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConvertButton.Location = new System.Drawing.Point(212, 112);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(75, 23);
            this.ConvertButton.TabIndex = 1;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(131, 112);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // ToolTip
            // 
            this.ToolTip.AutoPopDelay = 10000;
            this.ToolTip.InitialDelay = 500;
            this.ToolTip.ReshowDelay = 100;
            this.ToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // ModelConverterOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 147);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.tableLayoutPanel1);
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
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.ToolTip ToolTip;
    }
}