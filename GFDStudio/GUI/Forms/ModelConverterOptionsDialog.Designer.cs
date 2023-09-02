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
            VersionLabel = new MetroSetLabel();
            MaterialPresetLabel = new MetroSetLabel();
            MaterialPresetComboBox = new System.Windows.Forms.ComboBox();
            VersionTextBox = new MetroSetTextBox();
            ConvertSkinToZUpLabel = new MetroSetLabel();
            ConvertSkinToZUpCheckBox = new MetroSetCheckBox();
            GenerateVertexColorsCheckBox = new MetroSetCheckBox();
            GenerateVertexColorsLabel = new MetroSetLabel();
            minVertexAttributesLabel = new MetroSetLabel();
            MinimalVertexAttributesCheckBox = new MetroSetCheckBox();
            ConvertButton = new System.Windows.Forms.Button();
            CancelButton = new System.Windows.Forms.Button();
            descriptionTooltip = new System.Windows.Forms.ToolTip( components );
            OpenPresetButton = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor =    System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Left   |  System.Windows.Forms.AnchorStyles.Right  ;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle() );
            tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle() );
            tableLayoutPanel1.Controls.Add( VersionLabel, 0, 0 );
            tableLayoutPanel1.Controls.Add( MaterialPresetLabel, 0, 1 );
            tableLayoutPanel1.Controls.Add( MaterialPresetComboBox, 1, 1 );
            tableLayoutPanel1.Controls.Add( VersionTextBox, 1, 0 );
            tableLayoutPanel1.Controls.Add( ConvertSkinToZUpLabel, 0, 2 );
            tableLayoutPanel1.Controls.Add( ConvertSkinToZUpCheckBox, 1, 2 );
            tableLayoutPanel1.Controls.Add( GenerateVertexColorsCheckBox, 1, 3 );
            tableLayoutPanel1.Controls.Add( GenerateVertexColorsLabel, 0, 3 );
            tableLayoutPanel1.Controls.Add( minVertexAttributesLabel, 0, 4 );
            tableLayoutPanel1.Controls.Add( MinimalVertexAttributesCheckBox, 1, 4 );
            tableLayoutPanel1.Location = new System.Drawing.Point( 14, 14 );
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 20F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 570, 183 );
            tableLayoutPanel1.TabIndex = 0;
            // 
            // VersionLabel
            // 
            VersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            VersionLabel.AutoSize = true;
            VersionLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            VersionLabel.IsDerivedStyle = true;
            VersionLabel.Location = new System.Drawing.Point( 133, 8 );
            VersionLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new System.Drawing.Size( 66, 20 );
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
            MaterialPresetLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            MaterialPresetLabel.IsDerivedStyle = true;
            MaterialPresetLabel.Location = new System.Drawing.Point( 78, 44 );
            MaterialPresetLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            MaterialPresetLabel.Name = "MaterialPresetLabel";
            MaterialPresetLabel.Size = new System.Drawing.Size( 121, 20 );
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
            MaterialPresetComboBox.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            MaterialPresetComboBox.BackColor = System.Drawing.Color.FromArgb(   20  ,   20  ,   20   );
            MaterialPresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            MaterialPresetComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            MaterialPresetComboBox.ForeColor = System.Drawing.Color.Silver;
            MaterialPresetComboBox.Location = new System.Drawing.Point( 207, 40 );
            MaterialPresetComboBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            MaterialPresetComboBox.Size = new System.Drawing.Size( 359, 34 );
            MaterialPresetComboBox.TabIndex = 1;
            // 
            // VersionTextBox
            // 
            VersionTextBox.Anchor =   System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right  ;
            VersionTextBox.AutoCompleteCustomSource = null;
            VersionTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            VersionTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            VersionTextBox.BorderColor = System.Drawing.Color.FromArgb(   110  ,   110  ,   110   );
            VersionTextBox.DisabledBackColor = System.Drawing.Color.FromArgb(   80  ,   80  ,   80   );
            VersionTextBox.DisabledBorderColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            VersionTextBox.DisabledForeColor = System.Drawing.Color.FromArgb(   109  ,   109  ,   109   );
            VersionTextBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            VersionTextBox.HoverColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            VersionTextBox.Image = null;
            VersionTextBox.IsDerivedStyle = true;
            VersionTextBox.Lines = null;
            VersionTextBox.Location = new System.Drawing.Point( 207, 3 );
            VersionTextBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            VersionTextBox.MaxLength = 32767;
            VersionTextBox.Multiline = false;
            VersionTextBox.Name = "VersionTextBox";
            VersionTextBox.ReadOnly = false;
            VersionTextBox.Size = new System.Drawing.Size( 359, 30 );
            VersionTextBox.Style = MetroSet_UI.Enums.Style.Dark;
            VersionTextBox.StyleManager = null;
            VersionTextBox.TabIndex = 3;
            VersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            VersionTextBox.ThemeAuthor = "Narwin";
            VersionTextBox.ThemeName = "MetroDark";
            VersionTextBox.UseSystemPasswordChar = false;
            VersionTextBox.WatermarkText = "";
            // 
            // ConvertSkinToZUpLabel
            // 
            ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            ConvertSkinToZUpLabel.AutoSize = true;
            ConvertSkinToZUpLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            ConvertSkinToZUpLabel.IsDerivedStyle = true;
            ConvertSkinToZUpLabel.Location = new System.Drawing.Point( 41, 80 );
            ConvertSkinToZUpLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            ConvertSkinToZUpLabel.Name = "ConvertSkinToZUpLabel";
            ConvertSkinToZUpLabel.Size = new System.Drawing.Size( 158, 20 );
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
            ConvertSkinToZUpCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            ConvertSkinToZUpCheckBox.BorderColor = System.Drawing.Color.FromArgb(   155  ,   155  ,   155   );
            ConvertSkinToZUpCheckBox.Checked = false;
            ConvertSkinToZUpCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            ConvertSkinToZUpCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            ConvertSkinToZUpCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(   85  ,   85  ,   85   );
            ConvertSkinToZUpCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            ConvertSkinToZUpCheckBox.IsDerivedStyle = true;
            ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point( 207, 82 );
            ConvertSkinToZUpCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            ConvertSkinToZUpCheckBox.Name = "ConvertSkinToZUpCheckBox";
            ConvertSkinToZUpCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            ConvertSkinToZUpCheckBox.Size = new System.Drawing.Size( 15, 16 );
            ConvertSkinToZUpCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            ConvertSkinToZUpCheckBox.StyleManager = null;
            ConvertSkinToZUpCheckBox.TabIndex = 5;
            ConvertSkinToZUpCheckBox.ThemeAuthor = "Narwin";
            ConvertSkinToZUpCheckBox.ThemeName = "MetroDark";
            // 
            // GenerateVertexColorsCheckBox
            // 
            GenerateVertexColorsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            GenerateVertexColorsCheckBox.BackColor = System.Drawing.Color.Transparent;
            GenerateVertexColorsCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            GenerateVertexColorsCheckBox.BorderColor = System.Drawing.Color.FromArgb(   155  ,   155  ,   155   );
            GenerateVertexColorsCheckBox.Checked = false;
            GenerateVertexColorsCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            GenerateVertexColorsCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            GenerateVertexColorsCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(   85  ,   85  ,   85   );
            GenerateVertexColorsCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            GenerateVertexColorsCheckBox.IsDerivedStyle = true;
            GenerateVertexColorsCheckBox.Location = new System.Drawing.Point( 207, 118 );
            GenerateVertexColorsCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            GenerateVertexColorsCheckBox.Name = "GenerateVertexColorsCheckBox";
            GenerateVertexColorsCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            GenerateVertexColorsCheckBox.Size = new System.Drawing.Size( 15, 16 );
            GenerateVertexColorsCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            GenerateVertexColorsCheckBox.StyleManager = null;
            GenerateVertexColorsCheckBox.TabIndex = 6;
            GenerateVertexColorsCheckBox.ThemeAuthor = "Narwin";
            GenerateVertexColorsCheckBox.ThemeName = "MetroDark";
            // 
            // GenerateVertexColorsLabel
            // 
            GenerateVertexColorsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            GenerateVertexColorsLabel.AutoSize = true;
            GenerateVertexColorsLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            GenerateVertexColorsLabel.IsDerivedStyle = true;
            GenerateVertexColorsLabel.Location = new System.Drawing.Point( 20, 116 );
            GenerateVertexColorsLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            GenerateVertexColorsLabel.Name = "GenerateVertexColorsLabel";
            GenerateVertexColorsLabel.Size = new System.Drawing.Size( 179, 20 );
            GenerateVertexColorsLabel.Style = MetroSet_UI.Enums.Style.Dark;
            GenerateVertexColorsLabel.StyleManager = null;
            GenerateVertexColorsLabel.TabIndex = 7;
            GenerateVertexColorsLabel.Text = "Generate vertex colors";
            GenerateVertexColorsLabel.ThemeAuthor = "Narwin";
            GenerateVertexColorsLabel.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( GenerateVertexColorsLabel, "Generates dummy white vertex colors for models with none.\r\nFixes vertex explosion bug in P4D when vertex colors are missing." );
            // 
            // minVertexAttributesLabel
            // 
            minVertexAttributesLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            minVertexAttributesLabel.AutoSize = true;
            minVertexAttributesLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            minVertexAttributesLabel.IsDerivedStyle = true;
            minVertexAttributesLabel.Location = new System.Drawing.Point( 3, 153 );
            minVertexAttributesLabel.Name = "minVertexAttributesLabel";
            minVertexAttributesLabel.Size = new System.Drawing.Size( 197, 20 );
            minVertexAttributesLabel.Style = MetroSet_UI.Enums.Style.Dark;
            minVertexAttributesLabel.StyleManager = null;
            minVertexAttributesLabel.TabIndex = 8;
            minVertexAttributesLabel.Text = "Minimal Vertex Attributes";
            minVertexAttributesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            minVertexAttributesLabel.ThemeAuthor = "Narwin";
            minVertexAttributesLabel.ThemeName = "MetroDark";
            descriptionTooltip.SetToolTip( minVertexAttributesLabel, resources.GetString( "minVertexAttributesLabel.ToolTip" ) );
            // 
            // MinimalVertexAttributesCheckBox
            // 
            MinimalVertexAttributesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            MinimalVertexAttributesCheckBox.BackColor = System.Drawing.Color.Transparent;
            MinimalVertexAttributesCheckBox.BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            MinimalVertexAttributesCheckBox.BorderColor = System.Drawing.Color.FromArgb(   155  ,   155  ,   155   );
            MinimalVertexAttributesCheckBox.Checked = true;
            MinimalVertexAttributesCheckBox.CheckSignColor = System.Drawing.Color.FromArgb(   65  ,   177  ,   225   );
            MinimalVertexAttributesCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Checked;
            MinimalVertexAttributesCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb(   85  ,   85  ,   85   );
            MinimalVertexAttributesCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point );
            MinimalVertexAttributesCheckBox.IsDerivedStyle = true;
            MinimalVertexAttributesCheckBox.Location = new System.Drawing.Point( 207, 155 );
            MinimalVertexAttributesCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            MinimalVertexAttributesCheckBox.Name = "MinimalVertexAttributesCheckBox";
            MinimalVertexAttributesCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            MinimalVertexAttributesCheckBox.Size = new System.Drawing.Size( 15, 16 );
            MinimalVertexAttributesCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            MinimalVertexAttributesCheckBox.StyleManager = null;
            MinimalVertexAttributesCheckBox.TabIndex = 9;
            MinimalVertexAttributesCheckBox.ThemeAuthor = "Narwin";
            MinimalVertexAttributesCheckBox.ThemeName = "MetroDark";
            // 
            // ConvertButton
            // 
            ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ConvertButton.Font = new System.Drawing.Font( "Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point );
            ConvertButton.Location = new System.Drawing.Point( 396, 238 );
            ConvertButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            ConvertButton.Name = "ConvertButton";
            ConvertButton.Size = new System.Drawing.Size( 88, 27 );
            ConvertButton.TabIndex = 1;
            ConvertButton.Text = "Convert";
            ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.Location = new System.Drawing.Point( 300, 238 );
            CancelButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size( 88, 27 );
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
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
            OpenPresetButton.Location = new System.Drawing.Point( 107, 238 );
            OpenPresetButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            OpenPresetButton.Name = "OpenPresetButton";
            OpenPresetButton.Size = new System.Drawing.Size( 120, 27 );
            OpenPresetButton.TabIndex = 3;
            OpenPresetButton.Text = "Open preset folder";
            OpenPresetButton.UseVisualStyleBackColor = true;
            OpenPresetButton.Click += OpenPresetButton_Click;
            // 
            // ModelConverterOptionsDialog
            // 
            BackColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            BackgroundColor = System.Drawing.Color.FromArgb(   30  ,   30  ,   30   );
            ClientSize = new System.Drawing.Size( 580, 212 );
            Controls.Add( OpenPresetButton );
            Controls.Add( CancelButton );
            Controls.Add( ConvertButton );
            Controls.Add( tableLayoutPanel1 );
            DropShadowEffect = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            HeaderHeight = -40;
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
        private MetroSetCheckBox GenerateVertexColorsCheckBox;
        private MetroSetLabel GenerateVertexColorsLabel;
        private System.Windows.Forms.Button ConvertButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.ToolTip descriptionTooltip;
        private System.Windows.Forms.Button OpenPresetButton;
        private MetroSetLabel minVertexAttributesLabel;
        private MetroSetCheckBox MinimalVertexAttributesCheckBox;
    }
}