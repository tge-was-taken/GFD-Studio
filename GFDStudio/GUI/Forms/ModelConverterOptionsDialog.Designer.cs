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
            GenerateVertexColorsCheckBox = new MetroSetCheckBox();
            GenerateVertexColorsLabel = new MetroSetLabel();
            minVertexAttributesLabel = new MetroSetLabel();
            MinimalVertexAttributesCheckBox = new MetroSetCheckBox();
            AutoAddGFDHelperIDsLabel = new MetroSetLabel();
            AutoAddGFDHelperIDsCheckBox = new MetroSetCheckBox();
            VertexColorSlotLabel = new MetroSetLabel();
            VertexColorSlotNumber = new GUI.Controls.MetroSetNumericSingleClick();
            CombinedMeshNodeNameLabel = new MetroSetLabel();
            CombinedMeshNodeNameTextbox = new MetroSetTextBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ConvertButton = new System.Windows.Forms.Button();
            CancelButton = new System.Windows.Forms.Button();
            VersionComboBox = new System.Windows.Forms.ComboBox();
            VersionTextBox = new MetroSetTextBox();
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
            tableLayoutPanel1.Controls.Add( GenerateVertexColorsCheckBox, 1, 4 );
            tableLayoutPanel1.Controls.Add( GenerateVertexColorsLabel, 0, 4 );
            tableLayoutPanel1.Controls.Add( minVertexAttributesLabel, 0, 5 );
            tableLayoutPanel1.Controls.Add( MinimalVertexAttributesCheckBox, 1, 5 );
            tableLayoutPanel1.Controls.Add( AutoAddGFDHelperIDsLabel, 0, 6 );
            tableLayoutPanel1.Controls.Add( AutoAddGFDHelperIDsCheckBox, 1, 6 );
            tableLayoutPanel1.Controls.Add( VertexColorSlotLabel, 0, 7 );
            tableLayoutPanel1.Controls.Add( VertexColorSlotNumber, 1, 7 );
            tableLayoutPanel1.Controls.Add( CombinedMeshNodeNameLabel, 0, 8 );
            tableLayoutPanel1.Controls.Add( CombinedMeshNodeNameTextbox, 1, 8 );
            tableLayoutPanel1.Controls.Add( tableLayoutPanel2, 1, 9 );
            tableLayoutPanel1.Controls.Add( VersionComboBox, 1, 0 );
            tableLayoutPanel1.Controls.Add( VersionTextBox, 1, 1 );
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point( 2, 0 );
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 12F ) );
            tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Absolute, 47F ) );
            tableLayoutPanel1.Size = new System.Drawing.Size( 426, 351 );
            tableLayoutPanel1.TabIndex = 0;
            // 
            // metroSetLabel1
            // 
            metroSetLabel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            metroSetLabel1.AutoSize = true;
            metroSetLabel1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            metroSetLabel1.IsDerivedStyle = true;
            metroSetLabel1.Location = new System.Drawing.Point( 136, 41 );
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
            VersionLabel.Location = new System.Drawing.Point( 153, 8 );
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
            MaterialPresetLabel.Location = new System.Drawing.Point( 107, 74 );
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
            MaterialPresetComboBox.Location = new System.Drawing.Point( 217, 69 );
            MaterialPresetComboBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            MaterialPresetComboBox.Name = "MaterialPresetComboBox";
            MaterialPresetComboBox.Size = new System.Drawing.Size( 205, 28 );
            MaterialPresetComboBox.TabIndex = 1;
            // 
            // ConvertSkinToZUpLabel
            // 
            ConvertSkinToZUpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            ConvertSkinToZUpLabel.AutoSize = true;
            ConvertSkinToZUpLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            ConvertSkinToZUpLabel.IsDerivedStyle = true;
            ConvertSkinToZUpLabel.Location = new System.Drawing.Point( 74, 107 );
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
            ConvertSkinToZUpCheckBox.Location = new System.Drawing.Point( 217, 107 );
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
            // GenerateVertexColorsCheckBox
            // 
            GenerateVertexColorsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            GenerateVertexColorsCheckBox.BackColor = System.Drawing.Color.Transparent;
            GenerateVertexColorsCheckBox.BackgroundColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            GenerateVertexColorsCheckBox.BorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            GenerateVertexColorsCheckBox.Checked = false;
            GenerateVertexColorsCheckBox.CheckSignColor = System.Drawing.Color.FromArgb( 65, 177, 225 );
            GenerateVertexColorsCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            GenerateVertexColorsCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            GenerateVertexColorsCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb( 85, 85, 85 );
            GenerateVertexColorsCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            GenerateVertexColorsCheckBox.IsDerivedStyle = true;
            GenerateVertexColorsCheckBox.Location = new System.Drawing.Point( 217, 140 );
            GenerateVertexColorsCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            GenerateVertexColorsCheckBox.Name = "GenerateVertexColorsCheckBox";
            GenerateVertexColorsCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            GenerateVertexColorsCheckBox.Size = new System.Drawing.Size( 25, 16 );
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
            GenerateVertexColorsLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            GenerateVertexColorsLabel.IsDerivedStyle = true;
            GenerateVertexColorsLabel.Location = new System.Drawing.Point( 57, 140 );
            GenerateVertexColorsLabel.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
            GenerateVertexColorsLabel.Name = "GenerateVertexColorsLabel";
            GenerateVertexColorsLabel.Size = new System.Drawing.Size( 152, 17 );
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
            minVertexAttributesLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            minVertexAttributesLabel.IsDerivedStyle = true;
            minVertexAttributesLabel.Location = new System.Drawing.Point( 47, 173 );
            minVertexAttributesLabel.Name = "minVertexAttributesLabel";
            minVertexAttributesLabel.Size = new System.Drawing.Size( 163, 17 );
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
            MinimalVertexAttributesCheckBox.BackgroundColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            MinimalVertexAttributesCheckBox.BorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            MinimalVertexAttributesCheckBox.Checked = true;
            MinimalVertexAttributesCheckBox.CheckSignColor = System.Drawing.Color.FromArgb( 65, 177, 225 );
            MinimalVertexAttributesCheckBox.CheckState = MetroSet_UI.Enums.CheckState.Checked;
            MinimalVertexAttributesCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            MinimalVertexAttributesCheckBox.DisabledBorderColor = System.Drawing.Color.FromArgb( 85, 85, 85 );
            MinimalVertexAttributesCheckBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            MinimalVertexAttributesCheckBox.IsDerivedStyle = true;
            MinimalVertexAttributesCheckBox.Location = new System.Drawing.Point( 217, 173 );
            MinimalVertexAttributesCheckBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            MinimalVertexAttributesCheckBox.Name = "MinimalVertexAttributesCheckBox";
            MinimalVertexAttributesCheckBox.SignStyle = MetroSet_UI.Enums.SignStyle.Sign;
            MinimalVertexAttributesCheckBox.Size = new System.Drawing.Size( 25, 16 );
            MinimalVertexAttributesCheckBox.Style = MetroSet_UI.Enums.Style.Dark;
            MinimalVertexAttributesCheckBox.StyleManager = null;
            MinimalVertexAttributesCheckBox.TabIndex = 9;
            MinimalVertexAttributesCheckBox.ThemeAuthor = "Narwin";
            MinimalVertexAttributesCheckBox.ThemeName = "MetroDark";
            // 
            // AutoAddGFDHelperIDsLabel
            // 
            AutoAddGFDHelperIDsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            AutoAddGFDHelperIDsLabel.AutoSize = true;
            AutoAddGFDHelperIDsLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            AutoAddGFDHelperIDsLabel.IsDerivedStyle = true;
            AutoAddGFDHelperIDsLabel.Location = new System.Drawing.Point( 42, 206 );
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
            AutoAddGFDHelperIDsCheckBox.Location = new System.Drawing.Point( 217, 206 );
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
            // VertexColorSlotLabel
            // 
            VertexColorSlotLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            VertexColorSlotLabel.AutoSize = true;
            VertexColorSlotLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            VertexColorSlotLabel.IsDerivedStyle = true;
            VertexColorSlotLabel.Location = new System.Drawing.Point( 101, 239 );
            VertexColorSlotLabel.Name = "VertexColorSlotLabel";
            VertexColorSlotLabel.Size = new System.Drawing.Size( 109, 17 );
            VertexColorSlotLabel.Style = MetroSet_UI.Enums.Style.Dark;
            VertexColorSlotLabel.StyleManager = null;
            VertexColorSlotLabel.TabIndex = 0;
            VertexColorSlotLabel.Text = "Vertex color starting slot";
            VertexColorSlotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            VertexColorSlotLabel.ThemeAuthor = "Narwin";
            VertexColorSlotLabel.ThemeName = "MetroLite";
            // 
            // VertexColorSlotNumber
            // 
            VertexColorSlotNumber.BackColor = System.Drawing.Color.Transparent;
            VertexColorSlotNumber.BackgroundColor = System.Drawing.Color.Empty;
            VertexColorSlotNumber.BorderColor = System.Drawing.Color.FromArgb( 150, 150, 150 );
            VertexColorSlotNumber.DisabledBackColor = System.Drawing.Color.FromArgb( 204, 204, 204 );
            VertexColorSlotNumber.DisabledBorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            VertexColorSlotNumber.DisabledForeColor = System.Drawing.Color.FromArgb( 136, 136, 136 );
            VertexColorSlotNumber.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            VertexColorSlotNumber.IsDerivedStyle = true;
            VertexColorSlotNumber.Location = new System.Drawing.Point( 216, 234 );
            VertexColorSlotNumber.Maximum = 2;
            VertexColorSlotNumber.Minimum = 0;
            VertexColorSlotNumber.Name = "VertexColorSlotNumber";
            VertexColorSlotNumber.Size = new System.Drawing.Size( 207, 26 );
            VertexColorSlotNumber.Style = MetroSet_UI.Enums.Style.Light;
            VertexColorSlotNumber.StyleManager = null;
            VertexColorSlotNumber.SymbolsColor = System.Drawing.Color.FromArgb( 128, 128, 128 );
            VertexColorSlotNumber.TabIndex = 0;
            VertexColorSlotNumber.ThemeAuthor = "Narwin";
            VertexColorSlotNumber.ThemeName = "MetroLite";
            VertexColorSlotNumber.Value = 0;
            // 
            // CombinedMeshNodeNameLabel
            // 
            CombinedMeshNodeNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            CombinedMeshNodeNameLabel.AutoSize = true;
            CombinedMeshNodeNameLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            CombinedMeshNodeNameLabel.IsDerivedStyle = true;
            CombinedMeshNodeNameLabel.Location = new System.Drawing.Point( 26, 272 );
            CombinedMeshNodeNameLabel.Name = "CombinedMeshNodeNameLabel";
            CombinedMeshNodeNameLabel.Size = new System.Drawing.Size( 184, 17 );
            CombinedMeshNodeNameLabel.Style = MetroSet_UI.Enums.Style.Dark;
            CombinedMeshNodeNameLabel.StyleManager = null;
            CombinedMeshNodeNameLabel.TabIndex = 13;
            CombinedMeshNodeNameLabel.Text = "Combined mesh node name";
            CombinedMeshNodeNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            CombinedMeshNodeNameLabel.ThemeAuthor = "Narwin";
            CombinedMeshNodeNameLabel.ThemeName = "MetroDark";
            // 
            // CombinedMeshNodeNameTextbox
            // 
            CombinedMeshNodeNameTextbox.Anchor =  System.Windows.Forms.AnchorStyles.Left  |  System.Windows.Forms.AnchorStyles.Right ;
            CombinedMeshNodeNameTextbox.AutoCompleteCustomSource = null;
            CombinedMeshNodeNameTextbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            CombinedMeshNodeNameTextbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            CombinedMeshNodeNameTextbox.BorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            CombinedMeshNodeNameTextbox.DisabledBackColor = System.Drawing.Color.FromArgb( 204, 204, 204 );
            CombinedMeshNodeNameTextbox.DisabledBorderColor = System.Drawing.Color.FromArgb( 155, 155, 155 );
            CombinedMeshNodeNameTextbox.DisabledForeColor = System.Drawing.Color.FromArgb( 136, 136, 136 );
            CombinedMeshNodeNameTextbox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10F );
            CombinedMeshNodeNameTextbox.HoverColor = System.Drawing.Color.FromArgb( 102, 102, 102 );
            CombinedMeshNodeNameTextbox.Image = null;
            CombinedMeshNodeNameTextbox.IsDerivedStyle = true;
            CombinedMeshNodeNameTextbox.Lines = null;
            CombinedMeshNodeNameTextbox.Location = new System.Drawing.Point( 216, 267 );
            CombinedMeshNodeNameTextbox.MaxLength = 32767;
            CombinedMeshNodeNameTextbox.Multiline = false;
            CombinedMeshNodeNameTextbox.Name = "CombinedMeshNodeNameTextbox";
            CombinedMeshNodeNameTextbox.ReadOnly = false;
            CombinedMeshNodeNameTextbox.Size = new System.Drawing.Size( 207, 26 );
            CombinedMeshNodeNameTextbox.Style = MetroSet_UI.Enums.Style.Light;
            CombinedMeshNodeNameTextbox.StyleManager = null;
            CombinedMeshNodeNameTextbox.TabIndex = 14;
            CombinedMeshNodeNameTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CombinedMeshNodeNameTextbox.ThemeAuthor = "Narwin";
            CombinedMeshNodeNameTextbox.ThemeName = "MetroLite";
            CombinedMeshNodeNameTextbox.UseSystemPasswordChar = false;
            CombinedMeshNodeNameTextbox.WatermarkText = "";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel2.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            tableLayoutPanel2.Controls.Add( ConvertButton, 1, 0 );
            tableLayoutPanel2.Controls.Add( CancelButton, 0, 0 );
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point( 216, 300 );
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            tableLayoutPanel2.Size = new System.Drawing.Size( 207, 48 );
            tableLayoutPanel2.TabIndex = 10;
            // 
            // ConvertButton
            // 
            ConvertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            ConvertButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            ConvertButton.Font = new System.Drawing.Font( "Microsoft Sans Serif", 13F );
            ConvertButton.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            ConvertButton.Location = new System.Drawing.Point( 107, 8 );
            ConvertButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            ConvertButton.Name = "ConvertButton";
            ConvertButton.Size = new System.Drawing.Size( 96, 37 );
            ConvertButton.TabIndex = 3;
            ConvertButton.Text = "Convert";
            ConvertButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
            CancelButton.Location = new System.Drawing.Point( 4, 8 );
            CancelButton.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size( 95, 37 );
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
            VersionComboBox.Location = new System.Drawing.Point( 217, 5 );
            VersionComboBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            VersionComboBox.Name = "VersionComboBox";
            VersionComboBox.Size = new System.Drawing.Size( 205, 28 );
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
            VersionTextBox.Location = new System.Drawing.Point( 217, 36 );
            VersionTextBox.Margin = new System.Windows.Forms.Padding( 4, 3, 4, 3 );
            VersionTextBox.MaxLength = 32767;
            VersionTextBox.Multiline = false;
            VersionTextBox.Name = "VersionTextBox";
            VersionTextBox.ReadOnly = false;
            VersionTextBox.Size = new System.Drawing.Size( 205, 27 );
            VersionTextBox.Style = MetroSet_UI.Enums.Style.Dark;
            VersionTextBox.StyleManager = null;
            VersionTextBox.TabIndex = 3;
            VersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            VersionTextBox.ThemeAuthor = "Narwin";
            VersionTextBox.ThemeName = "MetroDark";
            VersionTextBox.UseSystemPasswordChar = false;
            VersionTextBox.WatermarkText = "";
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
            OpenPresetButton.Location = new System.Drawing.Point( 32, 379 );
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
            ClientSize = new System.Drawing.Size( 430, 353 );
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
        private MetroSetCheckBox GenerateVertexColorsCheckBox;
        private MetroSetLabel GenerateVertexColorsLabel;
        private System.Windows.Forms.ToolTip descriptionTooltip;
        private System.Windows.Forms.Button OpenPresetButton;
        private MetroSetLabel minVertexAttributesLabel;
        private MetroSetLabel AutoAddGFDHelperIDsLabel;
        private MetroSetCheckBox MinimalVertexAttributesCheckBox;
        private MetroSetCheckBox AutoAddGFDHelperIDsCheckBox;
        private System.Windows.Forms.ComboBox VersionComboBox;
        private MetroSetLabel metroSetLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.Button CancelButton;
        private MetroSetLabel CombinedMeshNodeNameLabel;
        private MetroSetTextBox CombinedMeshNodeNameTextbox;
        private MetroSetLabel VertexColorSlotLabel;
        private MetroSetNumeric VertexColorSlotNumber;
    }
}