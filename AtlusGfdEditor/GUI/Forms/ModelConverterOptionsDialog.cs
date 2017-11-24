using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Forms
{
    public partial class ModelConverterOptionsDialog : Form
    {
        public MaterialPreset MaterialPreset 
            => ( MaterialPreset )MaterialPresetComboBox.SelectedIndex;

        public uint Version
        {
            get
            {
                uint version = 0;

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

        public ModelConverterOptionsDialog( bool showSceneOptionsOnly )
        {
            InitializeComponent();
            VersionTextBox.Text = $"0x{Resource.PERSONA5_RESOURCE_VERSION:X8}";

            if ( showSceneOptionsOnly )
            {
                MaterialPresetComboBox.Enabled = false;
            }
            else
            {
                MaterialPresetComboBox.DataSource = Enum.GetNames( typeof( MaterialPreset ) );
            }
        }
    }
}
