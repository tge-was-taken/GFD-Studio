using System.Drawing;
using System.Windows.Forms;

namespace GFDStudio.GUI.Controls
{
    public partial class BitmapViewControl : UserControl
    {
        private static BitmapViewControl sInstance;

        public static BitmapViewControl Instance => sInstance ?? ( sInstance = new BitmapViewControl() );

        private BitmapViewControl()
        {
            InitializeComponent();
        }

        public void LoadBitmap( Bitmap bitmap )
        {
            BackgroundImage = bitmap;
            Width           = bitmap.Width;
            Height          = bitmap.Height;
        }
    }
}
