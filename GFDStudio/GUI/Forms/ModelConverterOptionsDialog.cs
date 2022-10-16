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

        private static string PresetLibraryPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath ) + @"\Presets\";
        private string[] Presets = Directory.GetFiles( PresetLibraryPath, "*.yml" ).Select( file => Path.GetFileName( file ) ).ToArray();

        public object MaterialPreset 
            => YamlSerializer.LoadYamlFile<Material>( PresetLibraryPath + Presets[MaterialPresetComboBox.SelectedIndex] );


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

        private void OpenEditPreset( string preset )
        {
            Process PresetEdit = new Process();
            PresetEdit.StartInfo = new ProcessStartInfo( preset )
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
            }
            else
            {
                MaterialPresetComboBox.DataSource = Presets;
            }
        }

        private void OpenPresetButton_Click( object sender, EventArgs e )
        {
            Process OpenPreset = new Process();
            OpenPreset.StartInfo = new ProcessStartInfo( PresetLibraryPath )
            {
                UseShellExecute = true
            };
            OpenPreset.Start();
        }
    }
}
