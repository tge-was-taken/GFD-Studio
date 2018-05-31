using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class CreateTextureMapDialog : Form
    {
        public ResultValue Result { get; private set; }

        public CreateTextureMapDialog()
        {
            InitializeComponent();
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
