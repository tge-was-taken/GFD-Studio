using System;
using System.ComponentModel;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.DataManagement;
using OpenTK.GLControl;
using OpenTK.Windowing.Common;

namespace GFDStudio.GUI.Controls
{
    /// <summary>
    /// Hosts the model scene in a WinForms control, passing on input and repaint requests.
    /// </summary>
    public class ModelViewControl : GLControl
    {
        private static ModelViewControl sInstance;

        public static ModelViewControl Instance => sInstance ?? ( sInstance = new ModelViewControl() );

        private readonly ModelScene mScene;
        private readonly Timer mUpdateTimer;
        private System.Drawing.Point mLastMouseLocation;

        private ModelViewControl() : base( new GLControlSettings
        {
            APIVersion = new Version( 3, 3 ),
#if GL_DEBUG
            Flags = ContextFlags.Debug | ContextFlags.ForwardCompatible,
#else
            Flags = ContextFlags.ForwardCompatible,
#endif
            DepthBits = 24,
            StencilBits = 0,
            NumberOfSamples = 4,
        } )
        {
            Dock = DockStyle.Fill;

            // required to use GL in the context of this control
            MakeCurrent();

            mScene = new ModelScene( DataStore.GetPath );
            mScene.Initialize();
            mScene.RedrawRequested += ( s, e ) => Invalidate();

            if ( !mScene.CanRender )
                Visible = false;

            mUpdateTimer = new Timer { Interval = (int)( 1f / 60f * 1000 ) };
            mUpdateTimer.Tick += ( s, e ) =>
            {
                if ( mScene.CanRender && mScene.Advance() )
                    Invalidate();
            };
            mUpdateTimer.Start();
        }

        public Animation Animation => mScene.Animation;

        public bool IsAnimationLoaded => mScene.IsAnimationLoaded;

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public AnimationPlaybackState AnimationPlayback
        {
            get => mScene.AnimationPlayback;
            set => mScene.AnimationPlayback = value;
        }

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public double AnimationTime
        {
            get => mScene.AnimationTime;
            set => mScene.AnimationTime = value;
        }

        public event EventHandler<Animation> AnimationLoaded
        {
            add => mScene.AnimationLoaded += value;
            remove => mScene.AnimationLoaded -= value;
        }

        public event EventHandler<AnimationPlaybackState> AnimationPlaybackStateChanged
        {
            add => mScene.AnimationPlaybackStateChanged += value;
            remove => mScene.AnimationPlaybackStateChanged -= value;
        }

        public event EventHandler<double> AnimationTimeChanged
        {
            add => mScene.AnimationTimeChanged += value;
            remove => mScene.AnimationTimeChanged -= value;
        }

        public void LoadModel( ModelPack modelPack ) => mScene.LoadModel( modelPack );

        public void LoadAnimation( Animation animation, bool reset = true ) => mScene.LoadAnimation( animation, reset );

        public void ClearSelection() => mScene.ClearSelection();

        public void SetSelection( Mesh data ) => mScene.SetSelection( data );

        public void SetSelection( Material data ) => mScene.SetSelection( data );

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mUpdateTimer?.Dispose();
                mScene?.Dispose();
            }

            base.Dispose( disposing );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            if ( !mScene.CanRender )
                return;

            mScene.Render();
            SwapBuffers();
        }

        protected override void OnResize( EventArgs e ) => mScene.Resize( Width, Height );

        protected override void OnMouseUp( MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
                mScene.Raypick( e.X, e.Y );
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            var deltaX = e.Location.X - mLastMouseLocation.X;
            var deltaY = e.Location.Y - mLastMouseLocation.Y;

            if ( e.Button.HasFlag( MouseButtons.Right ) )
                mScene.Pan( deltaX, deltaY, CalculateMultiplier() );
            else if ( e.Button.HasFlag( MouseButtons.Left ) )
                mScene.Orbit( deltaX, deltaY, CalculateMultiplier() );

            mLastMouseLocation = e.Location;
        }

        protected override void OnMouseWheel( MouseEventArgs e )
            => mScene.Zoom( e.Delta, CalculateMultiplier( 0.25f ) );

        protected override void OnKeyDown( KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Space )
                mScene.ResetView();
        }

        private static float CalculateMultiplier( float baseValue = 0.5f )
        {
            if ( ModifierKeys.HasFlag( Keys.Shift ) )
                return baseValue * 10f;

            if ( ModifierKeys.HasFlag( Keys.Control ) )
                return baseValue / 2f;

            return baseValue;
        }
    }
}
