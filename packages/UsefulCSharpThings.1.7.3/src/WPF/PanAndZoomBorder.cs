using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Border that allows panning and zooming of content. Predominantly got from: http://stackoverflow.com/questions/741956/pan-zoom-image
    /// </summary>
    public class PanAndZoomBorder : Border
    {
        /// <summary>
        /// Indicates whether element is zoomed in or not.
        /// </summary>
        public bool IsZoomed
        {
            get
            {
                var scaler = GetScaleTransform();
                return scaler.ScaleX != 1 || scaler.ScaleY != 1;
            }
        }
        private UIElement child = null;
        private Point origin;
        private Point start;
        private static Point DefaultRTO = new Point(0.5, 0.5);

        private TranslateTransform GetTranslateTransform()
        {
            return (TranslateTransform)((TransformGroup)child.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);

        }

        private ScaleTransform GetScaleTransform()
        {
            return (ScaleTransform)((TransformGroup)child.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            this.ClipToBounds = true;

            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = DefaultRTO;
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonDown += LeftMouseDownLinked;  // For linked boxes
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.MouseMove += MouseMoveLinked;  // For linked boxes
                this.MouseRightButtonDown += child_MouseRightButtonDown;
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform();
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;
                child.RenderTransformOrigin = DefaultRTO;

                // reset pan
                var tt = GetTranslateTransform();
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }


        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null) 
            {
                var st = GetScaleTransform();
                
                double zoom = e.Delta > 0 ? 0.2 : -0.2;
                double xScale = st.ScaleX + zoom;
                double YScale = st.ScaleY + zoom;


                bool isMouseZoom = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                if (sender == this)
                {
                    if (!isMouseZoom)
                        child.RenderTransformOrigin = DefaultRTO;
                    else
                    {
                        // Move transform origin to mouse cursor - percentage thing.
                        var mouse = e.GetPosition(child);
                        child.RenderTransformOrigin = new Point(mouse.X / child.RenderSize.Width, mouse.Y / child.RenderSize.Height);
                    }
                }

                // Update links
                if (Links.Count > 0)
                {
                    foreach (var link in Links)
                    {
                        if (link.child != null)
                            link.child.RenderTransformOrigin = child.RenderTransformOrigin;
                    }
                }

                // Prevent zooming out too far.
                if (xScale < 1)
                    xScale = 1;

                if (YScale < 1)
                    YScale = 1;

                st.ScaleX = xScale;
                st.ScaleY = YScale;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                // TODO When zooming out, make sure it fits back on the screen from wherever it was.

                // Don't allow moving if not zoomed at all.
                if (!CanPan())
                    return;

                var tt = GetTranslateTransform();
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.ScrollAll;
                bool captured = child.CaptureMouse();

                // Handled goes to linked mouse down
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        List<PanAndZoomBorder> Links = new List<PanAndZoomBorder>();

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured || (sender != this)) // Sender != this, means linked box is trying to move it.
                {
                    Vector v = start - e.GetPosition(this);
                    child_MouseMoveLinked(v);
                }
            }
        }

        private void child_MouseMoveLinked(Vector relative)
        {
            var tt = GetTranslateTransform();
            tt.X = origin.X - relative.X;
            tt.Y = origin.Y - relative.Y;
        }

        /// <summary>
        /// Links the transforms of a border to this one so they zoom and pan in tandem. 
        /// </summary>
        /// <param name="borderToLink">Border to link to this one.</param>
        public void Link(PanAndZoomBorder borderToLink)
        {
            if (Links.Contains(borderToLink) || borderToLink.Links.Contains(this))
                return;

            Links.Add(borderToLink);
            borderToLink.Links.Add(this);

            // Set borderToLink to same transform as this one
            var thisScale = GetScaleTransform();
            var thisTranslate = GetTranslateTransform();

            borderToLink.GetTranslateTransform().X = thisTranslate.X;
            borderToLink.GetTranslateTransform().Y = thisTranslate.Y;

            borderToLink.GetScaleTransform().ScaleX = thisScale.ScaleX;
            borderToLink.GetScaleTransform().ScaleY = thisScale.ScaleY;


            // Link changes on both borders to each other.
            this.MouseWheel += borderToLink.child_MouseWheel;
            this.MouseLeftButtonDown += borderToLink.child_MouseLeftButtonDown;
            this.MouseLeftButtonUp += borderToLink.child_MouseLeftButtonUp;
            this.MouseRightButtonDown += borderToLink.child_MouseRightButtonDown;


            borderToLink.MouseWheel += this.child_MouseWheel;
            borderToLink.MouseLeftButtonDown += this.child_MouseLeftButtonDown;
            borderToLink.MouseLeftButtonUp += this.child_MouseLeftButtonUp;
            borderToLink.MouseRightButtonDown += this.child_MouseRightButtonDown;
        }


        /// <summary>
        /// Unlinks a border from this one to move independently.
        /// </summary>
        /// <param name="linkedBorder">Border to remove link from.</param>
        public void Unlink(PanAndZoomBorder linkedBorder)
        {
            Links.Remove(linkedBorder);
            linkedBorder.Links.Remove(this);

            // Link changes on both borders to each other.
            this.MouseWheel -= linkedBorder.child_MouseWheel;
            this.MouseLeftButtonDown -= linkedBorder.child_MouseLeftButtonDown;
            this.MouseLeftButtonUp -= linkedBorder.child_MouseLeftButtonUp;
            this.MouseRightButtonDown -= linkedBorder.child_MouseRightButtonDown;

            linkedBorder.MouseWheel -= this.child_MouseWheel;
            linkedBorder.MouseLeftButtonDown -= this.child_MouseLeftButtonDown;
            linkedBorder.MouseLeftButtonUp -=  this.child_MouseLeftButtonUp;
            linkedBorder.MouseRightButtonDown -= this.child_MouseRightButtonDown;
        }

        private void MouseMoveLinked(object sender, MouseEventArgs e) 
        {
            if (Links.Count == 0)
                return;

            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    Vector v = start - e.GetPosition(this);

                    foreach (var link in Links)
                        link.child_MouseMoveLinked(v);
                }
            }
        }

        private void LeftMouseDownLinked(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (CanPan())
                    e.Handled = true;

                if (Links.Count == 0)
                    return;

                foreach (var link in Links)
                {
                    var tt = link.GetTranslateTransform();
                    link.origin = new Point(tt.X, tt.Y);   
                }
            }
        }

        bool CanPan()
        {
            var st = GetScaleTransform();
            return !(st.ScaleX == 1 && st.ScaleY == 1);
        }
    }
}
