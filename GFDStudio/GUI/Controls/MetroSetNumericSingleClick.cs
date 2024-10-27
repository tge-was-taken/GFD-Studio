using MetroSet_UI.Controls;
using System;
using System.Windows.Forms;

namespace GFDStudio.GUI.Controls
{
    // The up/down controls used in MetroSetUI's numeric control immediately starts repeated input
    // on hold (it's far too imprecise), so I'm ignoring those events
    internal class MetroSetNumericSingleClick : MetroSetNumeric
    {
        protected override void OnMouseDown( MouseEventArgs e ) {}
        protected override void OnMouseUp( MouseEventArgs e ) {}
        protected override void OnDoubleClick( EventArgs e )
        {
            base.OnDoubleClick( e );
            OnClick( e );
        }
    }
}
