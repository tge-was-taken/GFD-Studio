using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtlusGfdEditor.GUI.Controls
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
