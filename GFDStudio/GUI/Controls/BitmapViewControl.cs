using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace GFDStudio.GUI.Controls
{
    public partial class BitmapViewControl : UserControl
    {
        private static BitmapViewControl sInstance;
        private Bitmap Image;
        private Point LastMouseLocation;
        public static BitmapViewControl Instance => sInstance ?? ( sInstance = new BitmapViewControl() );
        public float TextureScale = 1;
        public Vector2 TextureOffset = Vector2.Zero;
        public bool Nearest = false;
        private BitmapViewControl()
        {
            InitializeComponent();
            this.SetStyle( ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true );
        }

        public void LoadBitmap( Bitmap bitmap )
        {
            Image = bitmap;
        }
        protected override void OnPaint( PaintEventArgs e )
        {
            if ( Image != null )
            {
                e.Graphics.InterpolationMode = Nearest ? InterpolationMode.NearestNeighbor : InterpolationMode.Default;
                e.Graphics.DrawImage( Image, TextureOffset.X + this.Width / 2 - Image.Width * TextureScale / 2,
                    TextureOffset.Y + this.Height / 2 - Image.Height * TextureScale / 2,
                    Image.Width * TextureScale, Image.Height * TextureScale );
            }
        }
        protected override void OnMouseMove( MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left || e.Button == MouseButtons.Right )
            {
                TextureOffset.X += e.Location.X - LastMouseLocation.X;
                TextureOffset.Y += e.Location.Y - LastMouseLocation.Y;
                Invalidate();
            }

            LastMouseLocation = e.Location;
        }
        protected override void OnMouseWheel( MouseEventArgs e )
        {
            TextureScale += TextureScale * ( e.Delta * 0.001f );
            TextureOffset.X += TextureOffset.X * ( e.Delta * 0.001f );
            TextureOffset.Y += TextureOffset.Y * ( e.Delta * 0.001f );
            Invalidate();
        }
        protected override void OnKeyDown( KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Space )
            {
                TextureOffset.X = 0;
                TextureOffset.Y = 0;
                TextureScale = 1;
                Invalidate();
            }
            else if ( e.KeyCode == Keys.F )
            {
                Nearest = !Nearest;
                Invalidate();
            }
        }
    }
}
