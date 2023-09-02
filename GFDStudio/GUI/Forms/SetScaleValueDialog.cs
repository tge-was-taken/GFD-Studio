using MetroSet_UI.Forms;
using System;
using System.Numerics;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class SetScaleValueDialog : MetroSetForm
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

            internal ResultValue( SetScaleValueDialog parent )
            {
                mParent = parent;
            }

            public Vector3 Scale => new Vector3 { X = Convert.ToSingle( mParent.num_ScaleX.Value ), Y = Convert.ToSingle( mParent.num_ScaleY.Value ), Z = Convert.ToSingle( mParent.num_ScaleZ.Value ) };
            public Vector3 Position => new Vector3 { X = Convert.ToSingle( mParent.num_PosX.Value ), Y = Convert.ToSingle( mParent.num_PosY.Value ), Z = Convert.ToSingle( mParent.num_PosZ.Value ) };
            public Quaternion Rotation => new Quaternion { X = Convert.ToSingle( mParent.num_RotX.Value ), Y = Convert.ToSingle( mParent.num_RotY.Value ), Z = Convert.ToSingle( mParent.num_RotZ.Value ), W = Convert.ToSingle( mParent.num_RotW.Value ) };

        }
    }
}
