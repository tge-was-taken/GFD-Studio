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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ModelConverterOptionsDialog ) );
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            metroSetLabel1 = new MetroSetLabel();
            VersionLabel = new MetroSetLabel();
            MaterialPresetLabel = new MetroSetLabel();
            MaterialPresetComboBox = new System.Windows.Forms.ComboBox();
            ConvertSkinToZUpLabel = new MetroSetLabel();
            ConvertSkinToZUpCheckBox = new MetroSetCheckBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ConvertButton = new System.Windows.Forms.Button();
            CancelButton = new System.Windows.Forms.Button();
            VersionComboBox = new System.Windows.Forms.ComboBox();
            VersionTextBox = new MetroSetTextBox();
            AutoAddGFDHelperIDsLabel = new MetroSetLabel();
            AutoAddGFDHelperIDsCheckBox = new MetroSetCheckBox();
            descriptionTooltip = new System.Windows.Forms.ToolTip( components );
            OpenPresetButton = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel1.Controls.Add( metroSetLabel1, 0, 1 );
            tableLayoutPanel1.Controls.Add( VersionLabel, 0, 0 );
            tableLayoutPanel1.Controls.Add( MaterialPresetLabel, 0, 2 );
            tableLayoutPanel1.Controls.Add( MaterialPresetComboBox, 1, 2 );
            tableLayoutPanel1.Controls.Add( ConvertSkinToZUpLabel, 0, 3 );
            tableLayoutPanel1.Controls.Add( ConvertSkinToZUpCheckBox, 1, 3 );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel2, 1, 7 );
            tableLayoutPanel1.Controls.Add( VersionComboBox, 1, 0 );
            tableLayoutPanel1.Controls.Add( VersionTextBox, 1, 1 );
            tableLayoutPanel1.Controls.Add( AutoAddGFDHelperIDsLabel, 0, 4 );
            tableLayoutPanel1.Controls.Add( AutoAddGFDHelperIDsCheckBox, 1, 4 );
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point( 2, 0 );
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 13.6222906F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Absolute, 38F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 19.1950474F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 11.11111F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 11.11111F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 11.11111F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 11.11111F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 22.22222F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 610, 323 );
            tableLayoutPanel1.TabIndex = 0;
            // 
            // metroSetLabel1
            // 
            metroSetLabel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            metroSetLabel1.AutoSize = true;
            metroSetLabel1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            metroSetLabel1.IsDerivedStyle = true;
            metroSetLabel1.Location = new System.Drawing.Point( 228, 49 );
            metroSetLabel1.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            metroSetLabel1.Name = "metroSetLabel1";
            metroSetLabel1.Size = new System.Drawing.Size( 73, 17 );
            metroSetLabel1.Style = MetroSet_UI.Enums.Style.Dark;
            metroSetLabel1.StyleManager = null;
            metroSetLabel1.TabIndex = 12;
            metroSetLabel1.Text = "Version ID";
            metroSetLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            metroSetLabel1.ThemeAuthor = "Narwin";
            metroSetLabel1.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( metroSetLabel1, "Resource version to use on model/animation pack.\r\n\r\nKnown resource versions:\r\n0x01105030 - P4D\r\n0x01105070 - P5\r\n0x01105090 - P3D & P5D\r\n0x01105100 - P5R & CFB" );
            // 
            // VersionLabel
            // 
            VersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            VersionLabel.AutoSize = true;
            VersionLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            VersionLabel.IsDerivedStyle = true;
            VersionLabel.Location = new System.Drawing.Point( 245, 11 );
            VersionLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new System.Drawing.Size( 56, 17 );
            VersionLabel.Style = MetroSet_UI.Enums.Style.Dark;
            VersionLabel.StyleManager = null;
            VersionLabel.TabIndex = 2;
            VersionLabel.Text = "Version";
            VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            VersionLabel.ThemeAuthor = "Narwin";
            VersionLabel.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( VersionLabel, "Resource version to use on model/animation pack.\r\n\r\nKnown resource versions:\r\n0x01105030 - P4D\r\n0x01105070 - P5\r\n0x01105090 - P3D & P5D\r\n0x01105100 - P5R & CFB" );
            // 
            // MaterialPresetLabel
            // 
            MaterialPresetLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            MaterialPresetLabel.AutoSize = true;
            MaterialPresetLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            MaterialPresetLabel.IsDerivedStyle = true;
            MaterialPresetLabel.Location = new System.Drawing.Point( 199, 95 );
            MaterialPresetLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            MaterialPresetLabel.Name = "MaterialPresetLabel";
            MaterialPresetLabel.Size = new System.Drawing.Size( 102, 17 );
            MaterialPresetLabel.Style = MetroSet_UI.Enums.Style.Dark;
            MaterialPresetLabel.StyleManager = null;
            MaterialPresetLabel.TabIndex = 0;
            MaterialPresetLabel.Text = "Material preset";
            MaterialPresetLabel.ThemeAuthor = "Narwin";
            MaterialPresetLabel.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( MaterialPresetLabel, "Material Preset to apply to converted model." );
            // 
            // MaterialPresetComboBox
            // 
            MaterialPresetComboBox.Anchor =  System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right ;
            MaterialPresetComboBox.BackColor = System.Drawing.Color.FromArgb( 20, 20, 20 );
            MaterialPresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            MaterialPresetComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            MaterialPresetComboBox.ForeColor = System.Drawing.Color.Silver;
            MaterialPresetComboBox.Location = new System.Drawing.Point( 309, 92 );
            MaterialPresetComboBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            MaterialPresetComboBox.Size = new System.Drawing.Size( 297, 28 );
            MaterialPresetComboBox.TabIndex = 1;
            // 
            // ConvertSkinToZUpLabel
            // 
            ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            ConvertSkinToZUpLabel.AutoSize = true;
            ConvertSkinToZUpLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            ConvertSkinToZUpLabel.IsDerivedStyle = true;
            ConvertSkinToZUpLabel.Location = new System.Drawing.Point( 166, 138 );
            ConvertSkinToZUpLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            ConvertSkinToZUpLabel.Name = "ConvertSkinToZUpLabel";
            ConvertSkinToZUpLabel.Size = new System.Drawing.Size( 135, 17 );
            ConvertSkinToZUpLabel.Style = MetroSet_UI.Enums.Style.Dark;
            ConvertSkinToZUpLabel.StyleManager = null;
            ConvertSkinToZUpLabel.TabIndex = 4;
            ConvertSkinToZUpLabel.Text = "Convert skin to Z up";
            ConvertSkinToZUpLabel.ThemeAuthor = "Narwin";
            ConvertSkinToZUpLabel.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( ConvertSkinToZUpLabel, "Convert the up axis of the inverse bind pose matrices to Z-up." );
            // 
            // ConvertSkinToZUpCheckBox
            // 
            ConvertSkinToZUpCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            ConvertSkinToZUpCheckBox.BackColor = System.Drawing.Color.Transparent;
            ConvertSkinToZUpCheckBox.BackgroundColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            ConvertSkinToZUpCheckBox.BorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            ConvertSkinToZUpCheckBox.Checked = false;
            ConvertSkinToZUpCheckBox.CheckSignColor = System.Drawing.Color.FromArgb( 65, 177, 225 );
            ConvertSkinToZUpCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            ConvertSkinToZUpCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            ConvertSkinToZUpCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb( 85, 85, 85 );
            ConvertSkinToZUpCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            ConvertSkinToZUpCheckBox.IsDerivedStyle = true;
            ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point( 309, 138 );
            ConvertSkinToZUpCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            ConvertSkinToZUpCheckBox.Name = "ConvertSkinToZUpCheckBox";
            ConvertSkinToZUpCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            ConvertSkinToZUpCheckBox.Size = new System.Drawing.Size( 25, 16 );
            ConvertSkinToZUpCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            ConvertSkinToZUpCheckBox.StyleManager = null;
            ConvertSkinToZUpCheckBox.TabIndex = 5;
            ConvertSkinToZUpCheckBox.ThemeAuthor = "Narwin";
            ConvertSkinToZUpCheckBox.ThemeName = "MetroDark";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel2.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel2.Controls.Add( ConvertButton, 1, 0 );
            tableLayoutPanel2.Controls.Add( CancelButton, 0, 0 );
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point( 308, 258 );
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            tableLayoutPanel2.Size = new System.Drawing.Size( 299, 62 );
            tableLayoutPanel2.TabIndex = 10;
            // 
            // ConvertButton
            // 
            ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ConvertButton.Font = new System.Drawing.Font( "Microsoft Sans Serif", 13F );
            ConvertButton.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            ConvertButton.Location = new System.Drawing.Point( 176, 11 );
            ConvertButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            ConvertButton.Name = "ConvertButton";
            ConvertButton.Size = new System.Drawing.Size( 96, 48 );
            ConvertButton.TabIndex = 3;
            ConvertButton.Text = "Convert";
            ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            CancelButton.Location = new System.Drawing.Point( 27, 11 );
            CancelButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size( 95, 48 );
            CancelButton.TabIndex = 4;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // VersionComboBox
            // 
            VersionComboBox.Anchor =  System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right ;
            VersionComboBox.BackColor = System.Drawing.Color.FromArgb( 20, 20, 20 );
            VersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            VersionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            VersionComboBox.ForeColor = System.Drawing.Color.Silver;
            VersionComboBox.Location = new System.Drawing.Point( 309, 8 );
            VersionComboBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            VersionComboBox.Name = "VersionComboBox";
            VersionComboBox.Size = new System.Drawing.Size( 297, 28 );
            VersionComboBox.TabIndex = 11;
            // 
            // VersionTextBox
            // 
            VersionTextBox.Anchor =  System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right ;
            VersionTextBox.AutoCompleteCustomSource = null;
            VersionTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            VersionTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            VersionTextBox.BorderColor = System.Drawing.Color.FromArgb( 110, 110, 110 );
            VersionTextBox.DisabledBackColor = System.Drawing.Color.FromArgb( 80, 80, 80 );
            VersionTextBox.DisabledBorderColor = System.Drawing.Color.FromArgb( 109, 109, 109 );
            VersionTextBox.DisabledForeColor = System.Drawing.Color.FromArgb( 109, 109, 109 );
            VersionTextBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            VersionTextBox.HoverColor = System.Drawing.Color.FromArgb( 65, 177, 225 );
            VersionTextBox.Image = null;
            VersionTextBox.IsDerivedStyle = true;
            VersionTextBox.Lines = null;
            VersionTextBox.Location = new System.Drawing.Point( 309, 43 );
            VersionTextBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            VersionTextBox.MaxLength = 32767;
            VersionTextBox.Multiline = false;
            VersionTextBox.Name = "VersionTextBox";
            VersionTextBox.ReadOnly = false;
            VersionTextBox.Size = new System.Drawing.Size( 297, 30 );
            VersionTextBox.Style = MetroSet_UI.Enums.Style.Dark;
            VersionTextBox.StyleManager = null;
            VersionTextBox.TabIndex = 3;
            VersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            VersionTextBox.ThemeAuthor = "Narwin";
            VersionTextBox.ThemeName = "MetroDark";
            VersionTextBox.UseSystemPasswordChar = false;
            VersionTextBox.WatermarkText = "";
            // 
            // AutoAddGFDHelperIDsLabel
            // 
            AutoAddGFDHelperIDsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            AutoAddGFDHelperIDsLabel.AutoSize = true;
            AutoAddGFDHelperIDsLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            AutoAddGFDHelperIDsLabel.IsDerivedStyle = true;
            AutoAddGFDHelperIDsLabel.Location = new System.Drawing.Point( 134, 169 );
            AutoAddGFDHelperIDsLabel.Name = "AutoAddGFDHelperIDsLabel";
            AutoAddGFDHelperIDsLabel.Size = new System.Drawing.Size( 168, 17 );
            AutoAddGFDHelperIDsLabel.Style = MetroSet_UI.Enums.Style.Dark;
            AutoAddGFDHelperIDsLabel.StyleManager = null;
            AutoAddGFDHelperIDsLabel.TabIndex = 8;
            AutoAddGFDHelperIDsLabel.Text = "Auto add GFD Helper IDs";
            AutoAddGFDHelperIDsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            AutoAddGFDHelperIDsLabel.ThemeAuthor = "Narwin";
            AutoAddGFDHelperIDsLabel.ThemeName = "MetroDark";
            // 
            // AutoAddGFDHelperIDsCheckBox
            // 
            AutoAddGFDHelperIDsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            AutoAddGFDHelperIDsCheckBox.BackColor = System.Drawing.Color.Transparent;
            AutoAddGFDHelperIDsCheckBox.BackgroundColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            AutoAddGFDHelperIDsCheckBox.BorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            AutoAddGFDHelperIDsCheckBox.Checked = true;
            AutoAddGFDHelperIDsCheckBox.CheckSignColor = System.Drawing.Color.FromArgb( 65, 177, 225 );
            AutoAddGFDHelperIDsCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Checked;
            AutoAddGFDHelperIDsCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            AutoAddGFDHelperIDsCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb( 85, 85, 85 );
            AutoAddGFDHelperIDsCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            AutoAddGFDHelperIDsCheckBox.IsDerivedStyle = true;
            AutoAddGFDHelperIDsCheckBox.Location = new System.Drawing.Point( 309, 169 );
            AutoAddGFDHelperIDsCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            AutoAddGFDHelperIDsCheckBox.Name = "AutoAddGFDHelperIDsCheckBox";
            AutoAddGFDHelperIDsCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            AutoAddGFDHelperIDsCheckBox.Size = new System.Drawing.Size( 25, 16 );
            AutoAddGFDHelperIDsCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            AutoAddGFDHelperIDsCheckBox.StyleManager = null;
            AutoAddGFDHelperIDsCheckBox.TabIndex = 9;
            AutoAddGFDHelperIDsCheckBox.ThemeAuthor = "Narwin";
            AutoAddGFDHelperIDsCheckBox.ThemeName = "MetroDark";
            // 
            // descriptionTooltip
            // 
            descriptionTooltip.AutoPopDelay = 10000;
            descriptionTooltip.InitialDelay = 500;
            descriptionTooltip.ReshowDelay = 100;
            // 
            // OpenPresetButton
            // 
            OpenPresetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            OpenPresetButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            OpenPresetButton.Location = new System.Drawing.Point( 124, 351 );
            OpenPresetButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            OpenPresetButton.Name = "OpenPresetButton";
            OpenPresetButton.Size = new System.Drawing.Size( 120, 27 );
            OpenPresetButton.TabIndex = 3;
            OpenPresetButton.Text = "Open preset folder";
            OpenPresetButton.UseVisualStyleBackColor = true;
            // 
            // ModelConverterOptionsDialog
            // 
            BackColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            BackgroundColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            ClientSize = new System.Drawing.Size( 614, 325 );
            Controls.Add( OpenPresetButton );
            Controls.Add( tableLayoutPanel1 );
            DropShadowEffect = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HeaderHeight = -40;
            Icon = (System.Drawing.Icon)resources.GetObject( "$this.Icon" );
            Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            Name = "ModelConverterOptionsDialog";
            Opacity = 0.99D;
            Padding = new System.Windows.Forms.Padding( 2, 0, 2, 2 );
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Style = MetroSet_UI.Enums.Style.Dark;
            Text = "MODEL CONVERTER OPTIONS";
            ThemeName = "MetroDark";
            tableLayoutPanel1.ResumeLayout( false );
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout( false );
            ResumeLayout( false );
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroSetLabel VersionLabel;
        private MetroSetLabel MaterialPresetLabel;
        private System.Windows.Forms.ComboBox MaterialPresetComboBox;
        private MetroSetTextBox VersionTextBox;
        private MetroSetLabel ConvertSkinToZUpLabel;
        private MetroSetCheckBox ConvertSkinToZUpCheckBox;
        private System.Windows.Forms.ToolTip descriptionTooltip;
        private System.Windows.Forms.Button OpenPresetButton;
        private MetroSetLabel AutoAddGFDHelperIDsLabel;
        private MetroSetCheckBox AutoAddGFDHelperIDsCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.ComboBox VersionComboBox;
        private MetroSetLabel metroSetLabel1;
    }
}