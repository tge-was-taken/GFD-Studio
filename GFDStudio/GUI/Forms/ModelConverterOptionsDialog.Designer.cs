using MetroSet_UI.Controls;
using MetroSet_UI.Forms;

namespace GFDStudio.GUI.Forms
{
    partial class ModelConverterOptionsDialog : MetroSetForm
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
            this.VersionLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.MaterialPresetLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.MaterialPresetComboBox = new System.Windows.Forms.ComboBox();
            this.VersionTextBox = new MetroSet_UI.Controls.MetroSetTextBox();
            this.ConvertSkinToZUpLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.ConvertSkinToZUpCheckBox = new MetroSet_UI.Controls.MetroSetCheckBox();
            this.GenerateVertexColorsCheckBox = new MetroSet_UI.Controls.MetroSetCheckBox();
            this.GenerateVertexColorsLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.minVertexAttributesLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.MinimalVertexAttributesCheckBox = new MetroSet_UI.Controls.MetroSetCheckBox();
            this.AutoAddGFDHelperIDsLabel = new MetroSet_UI.Controls.MetroSetLabel();
            this.AutoAddGFDHelperIDsCheckBox = new MetroSet_UI.Controls.MetroSetCheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.descriptionTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.OpenPresetButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
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
            this.tableLayoutPanel1.Controls.Add(this.AutoAddGFDHelperIDsLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.AutoAddGFDHelperIDsCheckBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(426, 323);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.VersionLabel.IsDerivedStyle = true;
            this.VersionLabel.Location = new System.Drawing.Point(153, 18);
            this.VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(56, 17);
            this.VersionLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.VersionLabel.StyleManager = null;
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "Version";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.VersionLabel.ThemeAuthor = "Narwin";
            this.VersionLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip(this.VersionLabel, "Resource version to use on model/animation pack.\r\n\r\nKnown resource versions:\r\n0x0" +
        "1105030 - P4D\r\n0x01105070 - P5\r\n0x01105090 - P3D & P5D\r\n0x01105100 - P5R & CFB");
            // 
            // MaterialPresetLabel
            // 
            this.MaterialPresetLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MaterialPresetLabel.AutoSize = true;
            this.MaterialPresetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MaterialPresetLabel.IsDerivedStyle = true;
            this.MaterialPresetLabel.Location = new System.Drawing.Point(107, 71);
            this.MaterialPresetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MaterialPresetLabel.Name = "MaterialPresetLabel";
            this.MaterialPresetLabel.Size = new System.Drawing.Size(102, 17);
            this.MaterialPresetLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.MaterialPresetLabel.StyleManager = null;
            this.MaterialPresetLabel.TabIndex = 0;
            this.MaterialPresetLabel.Text = "Material preset";
            this.MaterialPresetLabel.ThemeAuthor = "Narwin";
            this.MaterialPresetLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip(this.MaterialPresetLabel, "Material Preset to apply to converted model.");
            // 
            // MaterialPresetComboBox
            // 
            this.MaterialPresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MaterialPresetComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MaterialPresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MaterialPresetComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MaterialPresetComboBox.ForeColor = System.Drawing.Color.Silver;
            this.MaterialPresetComboBox.Location = new System.Drawing.Point(217, 68);
            this.MaterialPresetComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            this.MaterialPresetComboBox.Size = new System.Drawing.Size(205, 28);
            this.MaterialPresetComboBox.TabIndex = 1;
            // 
            // VersionTextBox
            // 
            this.VersionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.VersionTextBox.AutoCompleteCustomSource = null;
            this.VersionTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.VersionTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.VersionTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.VersionTextBox.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.VersionTextBox.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.VersionTextBox.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.VersionTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.VersionTextBox.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.VersionTextBox.Image = null;
            this.VersionTextBox.IsDerivedStyle = true;
            this.VersionTextBox.Lines = null;
            this.VersionTextBox.Location = new System.Drawing.Point(217, 11);
            this.VersionTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.VersionTextBox.MaxLength = 32767;
            this.VersionTextBox.Multiline = false;
            this.VersionTextBox.Name = "VersionTextBox";
            this.VersionTextBox.ReadOnly = false;
            this.VersionTextBox.Size = new System.Drawing.Size(205, 30);
            this.VersionTextBox.Style = MetroSet_UI.Enums.Style.Dark;
            this.VersionTextBox.StyleManager = null;
            this.VersionTextBox.TabIndex = 3;
            this.VersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.VersionTextBox.ThemeAuthor = "Narwin";
            this.VersionTextBox.ThemeName = "MetroDark";
            this.VersionTextBox.UseSystemPasswordChar = false;
            this.VersionTextBox.WatermarkText = "";
            // 
            // ConvertSkinToZUpLabel
            // 
            this.ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConvertSkinToZUpLabel.AutoSize = true;
            this.ConvertSkinToZUpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConvertSkinToZUpLabel.IsDerivedStyle = true;
            this.ConvertSkinToZUpLabel.Location = new System.Drawing.Point(74, 115);
            this.ConvertSkinToZUpLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ConvertSkinToZUpLabel.Name = "ConvertSkinToZUpLabel";
            this.ConvertSkinToZUpLabel.Size = new System.Drawing.Size(135, 17);
            this.ConvertSkinToZUpLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.ConvertSkinToZUpLabel.StyleManager = null;
            this.ConvertSkinToZUpLabel.TabIndex = 4;
            this.ConvertSkinToZUpLabel.Text = "Convert skin to Z up";
            this.ConvertSkinToZUpLabel.ThemeAuthor = "Narwin";
            this.ConvertSkinToZUpLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip(this.ConvertSkinToZUpLabel, "Convert the up axis of the inverse bind pose matrices to Z-up.");
            // 
            // ConvertSkinToZUpCheckBox
            // 
            this.ConvertSkinToZUpCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConvertSkinToZUpCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.ConvertSkinToZUpCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ConvertSkinToZUpCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.ConvertSkinToZUpCheckBox.Checked = false;
            this.ConvertSkinToZUpCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.ConvertSkinToZUpCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            this.ConvertSkinToZUpCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ConvertSkinToZUpCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.ConvertSkinToZUpCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConvertSkinToZUpCheckBox.IsDerivedStyle = true;
            this.ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point(217, 115);
            this.ConvertSkinToZUpCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConvertSkinToZUpCheckBox.Name = "ConvertSkinToZUpCheckBox";
            this.ConvertSkinToZUpCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            this.ConvertSkinToZUpCheckBox.Size = new System.Drawing.Size(25, 16);
            this.ConvertSkinToZUpCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            this.ConvertSkinToZUpCheckBox.StyleManager = null;
            this.ConvertSkinToZUpCheckBox.TabIndex = 5;
            this.ConvertSkinToZUpCheckBox.ThemeAuthor = "Narwin";
            this.ConvertSkinToZUpCheckBox.ThemeName = "MetroDark";
            // 
            // GenerateVertexColorsCheckBox
            // 
            this.GenerateVertexColorsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GenerateVertexColorsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.GenerateVertexColorsCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GenerateVertexColorsCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.GenerateVertexColorsCheckBox.Checked = false;
            this.GenerateVertexColorsCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.GenerateVertexColorsCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            this.GenerateVertexColorsCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GenerateVertexColorsCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.GenerateVertexColorsCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GenerateVertexColorsCheckBox.IsDerivedStyle = true;
            this.GenerateVertexColorsCheckBox.Location = new System.Drawing.Point(217, 150);
            this.GenerateVertexColorsCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GenerateVertexColorsCheckBox.Name = "GenerateVertexColorsCheckBox";
            this.GenerateVertexColorsCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            this.GenerateVertexColorsCheckBox.Size = new System.Drawing.Size(25, 16);
            this.GenerateVertexColorsCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            this.GenerateVertexColorsCheckBox.StyleManager = null;
            this.GenerateVertexColorsCheckBox.TabIndex = 6;
            this.GenerateVertexColorsCheckBox.ThemeAuthor = "Narwin";
            this.GenerateVertexColorsCheckBox.ThemeName = "MetroDark";
            // 
            // GenerateVertexColorsLabel
            // 
            this.GenerateVertexColorsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GenerateVertexColorsLabel.AutoSize = true;
            this.GenerateVertexColorsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GenerateVertexColorsLabel.IsDerivedStyle = true;
            this.GenerateVertexColorsLabel.Location = new System.Drawing.Point(57, 150);
            this.GenerateVertexColorsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GenerateVertexColorsLabel.Name = "GenerateVertexColorsLabel";
            this.GenerateVertexColorsLabel.Size = new System.Drawing.Size(152, 17);
            this.GenerateVertexColorsLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.GenerateVertexColorsLabel.StyleManager = null;
            this.GenerateVertexColorsLabel.TabIndex = 7;
            this.GenerateVertexColorsLabel.Text = "Generate vertex colors";
            this.GenerateVertexColorsLabel.ThemeAuthor = "Narwin";
            this.GenerateVertexColorsLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip(this.GenerateVertexColorsLabel, "Generates dummy white vertex colors for models with none.\r\nFixes vertex explosion" +
        " bug in P4D when vertex colors are missing.");
            // 
            // minVertexAttributesLabel
            // 
            this.minVertexAttributesLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.minVertexAttributesLabel.AutoSize = true;
            this.minVertexAttributesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.minVertexAttributesLabel.IsDerivedStyle = true;
            this.minVertexAttributesLabel.Location = new System.Drawing.Point(47, 185);
            this.minVertexAttributesLabel.Name = "minVertexAttributesLabel";
            this.minVertexAttributesLabel.Size = new System.Drawing.Size(163, 17);
            this.minVertexAttributesLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.minVertexAttributesLabel.StyleManager = null;
            this.minVertexAttributesLabel.TabIndex = 8;
            this.minVertexAttributesLabel.Text = "Minimal Vertex Attributes";
            this.minVertexAttributesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.minVertexAttributesLabel.ThemeAuthor = "Narwin";
            this.minVertexAttributesLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip(this.minVertexAttributesLabel, resources.GetString("minVertexAttributesLabel.ToolTip"));
            // 
            // MinimalVertexAttributesCheckBox
            // 
            this.MinimalVertexAttributesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MinimalVertexAttributesCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.MinimalVertexAttributesCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.MinimalVertexAttributesCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.MinimalVertexAttributesCheckBox.Checked = true;
            this.MinimalVertexAttributesCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.MinimalVertexAttributesCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Checked;
            this.MinimalVertexAttributesCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MinimalVertexAttributesCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.MinimalVertexAttributesCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinimalVertexAttributesCheckBox.IsDerivedStyle = true;
            this.MinimalVertexAttributesCheckBox.Location = new System.Drawing.Point(217, 185);
            this.MinimalVertexAttributesCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimalVertexAttributesCheckBox.Name = "MinimalVertexAttributesCheckBox";
            this.MinimalVertexAttributesCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            this.MinimalVertexAttributesCheckBox.Size = new System.Drawing.Size(25, 16);
            this.MinimalVertexAttributesCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            this.MinimalVertexAttributesCheckBox.StyleManager = null;
            this.MinimalVertexAttributesCheckBox.TabIndex = 9;
            this.MinimalVertexAttributesCheckBox.ThemeAuthor = "Narwin";
            this.MinimalVertexAttributesCheckBox.ThemeName = "MetroDark";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.ConvertButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.CancelButton, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(216, 249);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(207, 71);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConvertButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConvertButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ConvertButton.Location = new System.Drawing.Point(107, 20);
            this.ConvertButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(96, 48);
            this.ConvertButton.TabIndex = 3;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CancelButton.Location = new System.Drawing.Point(4, 20);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(95, 48);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // AutoAddGFDHelperIDsLabel
            // 
            this.AutoAddGFDHelperIDsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AutoAddGFDHelperIDsLabel.AutoSize = true;
            this.AutoAddGFDHelperIDsLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            this.AutoAddGFDHelperIDsLabel.IsDerivedStyle = true;
            this.AutoAddGFDHelperIDsLabel.Location = new System.Drawing.Point( 47, 185 );
            this.AutoAddGFDHelperIDsLabel.Name = "AutoAddGFDHelperIDsLabel";
            this.AutoAddGFDHelperIDsLabel.Size = new System.Drawing.Size( 163, 17 );
            this.AutoAddGFDHelperIDsLabel.Style = MetroSet_UI.Enums.Style.Dark;
            this.AutoAddGFDHelperIDsLabel.StyleManager = null;
            this.AutoAddGFDHelperIDsLabel.TabIndex = 8;
            this.AutoAddGFDHelperIDsLabel.Text = "Auto add GFD Helper IDs";
            this.AutoAddGFDHelperIDsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoAddGFDHelperIDsLabel.ThemeAuthor = "Narwin";
            this.AutoAddGFDHelperIDsLabel.ThemeName = "MetroDark";
            this.descriptionTooltip.SetToolTip( this.AutoAddGFDHelperIDsLabel, resources.GetString( "AutoAddGFDHelperIDsLabel.ToolTip" ) );
            // 
            // AutoAddGFDHelperIDsCheckBox
            // 
            this.AutoAddGFDHelperIDsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.AutoAddGFDHelperIDsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.AutoAddGFDHelperIDsCheckBox.BackgroundColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 30 ) ) ) ), ( (int)( ( (byte)( 30 ) ) ) ), ( (int)( ( (byte)( 30 ) ) ) ) );
            this.AutoAddGFDHelperIDsCheckBox.BorderColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 155 ) ) ) ), ( (int)( ( (byte)( 155 ) ) ) ), ( (int)( ( (byte)( 155 ) ) ) ) );
            this.AutoAddGFDHelperIDsCheckBox.Checked = true;
            this.AutoAddGFDHelperIDsCheckBox.CheckSignColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 65 ) ) ) ), ( (int)( ( (byte)( 177 ) ) ) ), ( (int)( ( (byte)( 225 ) ) ) ) );
            this.AutoAddGFDHelperIDsCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Checked;
            this.AutoAddGFDHelperIDsCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AutoAddGFDHelperIDsCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 85 ) ) ) ), ( (int)( ( (byte)( 85 ) ) ) ), ( (int)( ( (byte)( 85 ) ) ) ) );
            this.AutoAddGFDHelperIDsCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            this.AutoAddGFDHelperIDsCheckBox.IsDerivedStyle = true;
            this.AutoAddGFDHelperIDsCheckBox.Location = new System.Drawing.Point( 217, 185 );
            this.AutoAddGFDHelperIDsCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            this.AutoAddGFDHelperIDsCheckBox.Name = "AutoAddGFDHelperIDsCheckBox";
            this.AutoAddGFDHelperIDsCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            this.AutoAddGFDHelperIDsCheckBox.Size = new System.Drawing.Size( 25, 16 );
            this.AutoAddGFDHelperIDsCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            this.AutoAddGFDHelperIDsCheckBox.StyleManager = null;
            this.AutoAddGFDHelperIDsCheckBox.TabIndex = 9;
            this.AutoAddGFDHelperIDsCheckBox.ThemeAuthor = "Narwin";
            this.AutoAddGFDHelperIDsCheckBox.ThemeName = "MetroDark";
            // 
            // descriptionTooltip
            // 
            this.descriptionTooltip.AutoPopDelay = 10000;
            this.descriptionTooltip.InitialDelay = 500;
            this.descriptionTooltip.ReshowDelay = 100;
            // 
            // OpenPresetButton
            // 
            this.OpenPresetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.OpenPresetButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OpenPresetButton.Location = new System.Drawing.Point(32, 351);
            this.OpenPresetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.OpenPresetButton.Name = "OpenPresetButton";
            this.OpenPresetButton.Size = new System.Drawing.Size(120, 27);
            this.OpenPresetButton.TabIndex = 3;
            this.OpenPresetButton.Text = "Open preset folder";
            this.OpenPresetButton.UseVisualStyleBackColor = true;
            // 
            // ModelConverterOptionsDialog
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(430, 325);
            this.Controls.Add(this.OpenPresetButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DropShadowEffect = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HeaderHeight = -40;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ModelConverterOptionsDialog";
            this.Opacity = 0.99D;
            this.Padding = new System.Windows.Forms.Padding(2, 0, 2, 2);
            this.ShowHeader = true;
            this.ShowLeftRect = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroSet_UI.Enums.Style.Dark;
            this.Text = "MODEL CONVERTER OPTIONS";
            this.ThemeName = "MetroDark";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroSetLabel VersionLabel;
        private MetroSetLabel MaterialPresetLabel;
        private System.Windows.Forms.ComboBox MaterialPresetComboBox;
        private MetroSetTextBox VersionTextBox;
        private MetroSetLabel ConvertSkinToZUpLabel;
        private MetroSetCheckBox ConvertSkinToZUpCheckBox;
        private MetroSetCheckBox GenerateVertexColorsCheckBox;
        private MetroSetLabel GenerateVertexColorsLabel;
        private System.Windows.Forms.ToolTip descriptionTooltip;
        private System.Windows.Forms.Button OpenPresetButton;
        private MetroSetLabel minVertexAttributesLabel;
        private MetroSetLabel AutoAddGFDHelperIDsLabel;
        private MetroSetCheckBox MinimalVertexAttributesCheckBox;
        private MetroSetCheckBox AutoAddGFDHelperIDsCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button ConvertButton;
    }
}