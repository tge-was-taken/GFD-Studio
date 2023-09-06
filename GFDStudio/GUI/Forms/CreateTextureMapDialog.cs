using System;
using System.Windows.Forms;
using MetroSet_UI.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class CreateTextureMapDialog : MetroSetForm
    {
        public ResultValue Result { get; private set; }

        public CreateTextureMapDialog()
        {
            InitializeComponent();
            Theme.Apply( this );
        }

        private void OKButton_Click( object sender, EventArgs e )
        {
            Result = new ResultValue( this );
        }

        public class ResultValue
        {
            private readonly CreateTextureMapDialog mParent;

            internal ResultValue( CreateTextureMapDialog parent )
            {
                mParent = parent;
            }

            public int Type => mParent.comboBox1.SelectedIndex;

            public string Name => mParent.textBox1.Text;
        }
    }
}
