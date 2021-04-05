using System;
using System.Globalization;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Materials;

namespace GFDStudio.GUI.Forms
{
    public partial class ModelConverterOptionsDialog : Form
    {
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
                MaterialPresetComboBox.DataSource = Enum.GetNames( typeof( MaterialPreset ) );
                MaterialPresetComboBox.SelectedIndex = ( int )MaterialPreset.CharacterSkinP5;
            }
        }
    }
}
