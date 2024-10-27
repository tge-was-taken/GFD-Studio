using System;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Materials;
using MetroSet_UI.Forms;
using System.Reflection;
using GFDLibrary.Effects;

namespace GFDStudio.GUI.Forms
{
    public partial class ModelConverterOptionsDialog : MetroSetForm
    {

        private static string BasePresetLibraryPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath ) + @"\Presets\";
        private string PresetPath => Version >= 0x02000000 ? BasePresetLibraryPath + @"Metaphor\" : BasePresetLibraryPath;
        private string[] Presets;

        public object MaterialPreset
            => YamlSerializer.LoadYamlFile<Material>( PresetPath + Presets[MaterialPresetComboBox.SelectedIndex] );


        string CurrentPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath );

        private string[] VersionPresets;

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

        public bool AutoAddGFDHelperIDs
        {
            get => AutoAddGFDHelperIDsCheckBox.Checked;
        }

        public string CombinedMeshNodeName
        {
            get => CombinedMeshNodeNameTextbox.Text;
        }

        public int VertexColorStartingSlot
        {
            get => VertexColorSlotNumber.Value;
        }

        private string[] CreateVersionPresets()
        {
            var PropVersions = typeof( ResourceVersion )
                .GetFields( BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy )
                .Where( x => !x.Name.EndsWith( "ShaderCache" ) )
                .Select( x => x.Name );
            PropVersions = PropVersions.Append( "Custom Version" );
            return PropVersions.ToArray();
        }
        private void RefreshPresetList()
        {
            Presets = Directory.GetFiles( PresetPath, "*.yml" ).Select( file => Path.GetFileName( file ) ).ToArray();
            MaterialPresetComboBox.DataSource = Presets;
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

        private void VersionTextUserUpdated( object? sender, EventArgs e )
        {
            if ( VersionComboBox.SelectedIndex == VersionPresets.Length - 1 ) // Custom version only
                RefreshPresetList();
        }

        private void RefreshVersionID( object sender, EventArgs e )
        {
            // Last index is Custom Version, so enable the option to enter a custom model version
            if ( VersionComboBox.SelectedIndex == VersionPresets.Length - 1 ) VersionTextBox.Enabled = true;
            else SetResourceVersion( VersionPresets[VersionComboBox.SelectedIndex], false );
        }

        private void SetResourceVersion( string ResourceFieldName, bool bEnableCustomVerNumber = false )
        {
            uint NewResourceVersion = (uint)typeof( ResourceVersion ).GetField( ResourceFieldName ).GetValue( null );
            VersionTextBox.Text = $"0x{NewResourceVersion:X8}";
            VersionTextBox.Enabled = bEnableCustomVerNumber;
            RefreshPresetList();
        }

        public ModelConverterOptionsDialog( bool showSceneOptionsOnly )
        {
            InitializeComponent();
            Text = "Model Converter Options";
            Theme.Apply( this );
            SetResourceVersion( "MetaphorRefantazio" );
            //SetResourceVersion( "Persona5Royal" );
            RefreshPresetList();
            VersionPresets = CreateVersionPresets();
            VersionComboBox.DataSource = VersionPresets;
            VersionComboBox.SelectedIndexChanged += RefreshVersionID;

            if ( showSceneOptionsOnly )
            {
                MaterialPresetComboBox.Enabled = false;
            }
            else
            {
                MaterialPresetComboBox.DataSource = Presets;
                VersionTextBox.TextChanged += VersionTextUserUpdated;
            }
        }

        private void OpenPresetButton_Click( object sender, EventArgs e )
        {
            Process OpenPreset = new Process();
            OpenPreset.StartInfo = new ProcessStartInfo( PresetPath )
            {
                UseShellExecute = true
            };
            OpenPreset.Start();
        }
    }
}
