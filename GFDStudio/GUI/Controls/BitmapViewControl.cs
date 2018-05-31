using System.Drawing;
using System.Windows.Forms;

namespace GFDStudio.GUI.Controls
{
    public partial class BitmapViewControl : UserControl
    {
        public BitmapViewControl()
        {
            InitializeComponent();
        }

        public BitmapViewControl( Bitmap bitmap )
        {
            BackgroundImage = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }
    }
}
