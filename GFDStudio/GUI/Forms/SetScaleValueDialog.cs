using System;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class SetScaleValueDialog : Form
    {
        public ResultValue Result { get; private set; }

        public SetScaleValueDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click( object sender, EventArgs e )
        {
            Result = new ResultValue( this );
        }

        public class ResultValue
        {
            private readonly SetScaleValueDialog mParent;

            internal ResultValue(SetScaleValueDialog parent )
            {
                mParent = parent;
            }

            public float Scale => Convert.ToSingle(mParent.numUpDwn.Value);
        }
    }
}
