using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtlusGfdEditor.GUI
{
    public partial class ImageViewControl : UserControl
    {
        public ImageViewControl()
        {
            InitializeComponent();
        }

        public ImageViewControl( Image image )
        {
            BackgroundImage = image;
            Width = image.Width;
            Height = image.Height;
        }
    }
}
