using System;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Materials;


namespace GFDStudio.GUI.Forms
{
    public partial class ModelConverterOptionsDialog : Form
    {

        static string PresetLibraryPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath ) + @"\Presets";
        private string[] Presets = Directory.GetFiles( PresetLibraryPath, "*.yml" ).Select( file => Path.GetFileName( file ) ).ToArray();


        public MaterialPreset MaterialPreset 
            => ( MaterialPreset )MaterialPresetComboBox.SelectedIndex;


        public uint Version
        {
            get
            {
                uint version;

                if ( VersionTextBox.Text.StartsWith( "0x" ) )
                {
                    uint.TryParse( VersionTextBox.Text.Substring( 2 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out version );
                }
                else
                {
                    uint.TryParse( VersionTextBox.Text, out version );
                }

                return version;
            }
        }

        public bool ConvertSkinToZUp
        {
            get => ConvertSkinToZUpCheckBox.Checked;
        }

        public bool GenerateVertexColors
        {
            get => GenerateVertexColorsCheckBox.Checked;
        }

        public bool MinimalVertexAttributes
        {
            get => MinimalVertexAttributesCheckBox.Checked;
        }

        private void RefreshPresetList()
        {
            Presets = Directory.GetFiles( PresetLibraryPath, "*.yml" ).Select( file => Path.GetFileName( file ) ).ToArray();
        }

        private void OpenEditPreset( string name )
        {
            Process PresetEdit = new Process();
            PresetEdit.StartInfo = new ProcessStartInfo( name )
            {
                UseShellExecute = true
            };
            PresetEdit.Start();
        }

        public ModelConverterOptionsDialog( bool showSceneOptionsOnly )
        {
            InitializeComponent();
            VersionTextBox.Text = $"0x{ResourceVersion.Persona5:X8}";

            if ( showSceneOptionsOnly )
            {
                MaterialPresetComboBox.Enabled = false;
                EditPresetButton.Enabled = false;
                NewPresetButton.Enabled = false;
            }
            else
            {
                MaterialPresetComboBox.DataSource = Presets;
            }
        }

        private void EditPresetButton_Click( object sender, EventArgs e )
        {
            string pre = PresetLibraryPath + @"\" + Presets[MaterialPresetComboBox.SelectedIndex];

            OpenEditPreset( pre );
        }

        private void NewPresetButton_Click( object sender, EventArgs e )
        {
            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Text;
                dialog.Filter = "YAML files (*.yml)|*.yml";
                dialog.OverwritePrompt = true;
                dialog.Title = "Select a file to export to.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return;
                }

                YamlSerializer.SaveYamlFile( Data, dialog.FileName );
                OpenEditPreset( PresetLibraryPath + @"\" + dialog.FileName );
                RefreshPresetList();
            }
        }
    }
}
