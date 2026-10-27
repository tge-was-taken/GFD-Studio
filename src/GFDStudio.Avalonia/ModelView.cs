using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.DataManagement;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Hosts the model scene in an Avalonia control, passing on input and repaint requests.
    /// </summary>
    public sealed class ModelView : OpenGlControlBase
    {
        private ModelScene mScene;
        private DispatcherTimer mUpdateTimer;
        private Point mLastPointerPosition;
        private ModelPack mPendingModel;
        private ModelPack mLoadedModel;

        public ModelView()
        {
            Focusable = true;
            ClipToBounds = true;
        }

        /// <summary>
        /// Queues a model to be shown. Building one creates textures and buffers, which is only
        /// legal while the context is current, and Avalonia makes it current for the render
        /// callbacks alone.
        /// </summary>
        public void LoadModel( ModelPack modelPack )
        {
            if ( ReferenceEquals( modelPack, mLoadedModel ) )
                return;

            mPendingModel = modelPack;
            RequestNextFrameRendering();
        }

        public void LoadAnimation( Animation animation, bool reset = true )
        {
            mScene?.LoadAnimation( animation, reset );
            RequestNextFrameRendering();
        }

        public bool HasModel => mLoadedModel != null || mPendingModel != null;

        public Animation Animation => mScene?.Animation;

        public bool IsAnimationLoaded => mScene?.IsAnimationLoaded ?? false;

        public AnimationPlaybackState AnimationPlayback
        {
            get => mScene?.AnimationPlayback ?? AnimationPlaybackState.Stopped;
            set
            {
                if ( mScene != null )
                    mScene.AnimationPlayback = value;
            }
        }

        public double AnimationTime
        {
            get => mScene?.AnimationTime ?? 0;
            set
            {
                if ( mScene == null )
                    return;

                mScene.AnimationTime = value;
                RequestNextFrameRendering();
            }
        }

        public event EventHandler<Animation> AnimationLoaded;

        public event EventHandler<AnimationPlaybackState> AnimationPlaybackStateChanged;

        public event EventHandler<double> AnimationTimeChanged;

        public void ClearSelection()
        {
            mScene?.ClearSelection();
            RequestNextFrameRendering();
        }

        public void SetSelection( Mesh data )
        {
            mScene?.SetSelection( data );
            RequestNextFrameRendering();
        }

        public void SetSelection( Material data )
        {
            mScene?.SetSelection( data );
            RequestNextFrameRendering();
        }

        protected override void OnOpenGlInit( GlInterface gl )
        {
            // OpenTK resolves its entry points through whatever context the host made current,
            // which here is Avalonia's.
            GL.LoadBindings( new AvaloniaBindings( gl ) );

            mScene = new ModelScene( DataStore.GetPath );
            mScene.Initialize();
            mScene.RedrawRequested += ( s, e ) => Dispatcher.UIThread.Post( RequestNextFrameRendering );
            mScene.Resize( PixelWidth, PixelHeight );
            mScene.AnimationLoaded += ( s, e ) => AnimationLoaded?.Invoke( this, e );
            mScene.AnimationPlaybackStateChanged += ( s, e ) => AnimationPlaybackStateChanged?.Invoke( this, e );
            mScene.AnimationTimeChanged += ( s, e ) => AnimationTimeChanged?.Invoke( this, e );

            mUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds( 1.0 / 60.0 ) };
            mUpdateTimer.Tick += ( s, e ) =>
            {
                if ( mScene is { CanRender: true } && mScene.Advance() )
                    RequestNextFrameRendering();
            };
            mUpdateTimer.Start();

            LoadPendingModel();
        }

        private void LoadPendingModel()
        {
            if ( mPendingModel == null || mScene is not { CanRender: true } )
                return;

            mScene.LoadModel( mPendingModel );
            mLoadedModel = mPendingModel;
            mPendingModel = null;
        }

        protected override void OnOpenGlDeinit( GlInterface gl )
        {
            mUpdateTimer?.Stop();
            mUpdateTimer = null;
            mScene?.Dispose();
            mScene = null;

            // The scene's GL resources went with the context, so the model has to be rebuilt.
            mPendingModel ??= mLoadedModel;
            mLoadedModel = null;
        }

        protected override void OnOpenGlRender( GlInterface gl, int fb )
        {
            if ( mScene is not { CanRender: true } )
                return;

            LoadPendingModel();

            mScene.Resize( PixelWidth, PixelHeight );
            mScene.Render();

            // Avalonia composites its own framebuffer, which the scene unbinds as it draws.
            GL.BindFramebuffer( FramebufferTarget.Framebuffer, fb );
        }

        /// <summary>
        /// The GL surface is composited separately, so without something drawn here the control
        /// is not hit testable and never sees the pointer.
        /// </summary>
        public override void Render( DrawingContext context )
        {
            context.FillRectangle( Brushes.Transparent, new Rect( Bounds.Size ) );
            base.Render( context );
        }

        protected override void OnPointerPressed( PointerPressedEventArgs e )
        {
            base.OnPointerPressed( e );
            mLastPointerPosition = e.GetPosition( this );
            Focus();
        }

        protected override void OnPointerMoved( PointerEventArgs e )
        {
            base.OnPointerMoved( e );

            var position = e.GetPosition( this );
            var deltaX = (float)( position.X - mLastPointerPosition.X );
            var deltaY = (float)( position.Y - mLastPointerPosition.Y );
            mLastPointerPosition = position;

            var point = e.GetCurrentPoint( this );
            if ( point.Properties.IsRightButtonPressed )
                mScene?.Pan( deltaX, deltaY, Multiplier( e.KeyModifiers ) );
            else if ( point.Properties.IsLeftButtonPressed )
                mScene?.Orbit( deltaX, deltaY, Multiplier( e.KeyModifiers ) );
        }

        protected override void OnPointerReleased( PointerReleasedEventArgs e )
        {
            base.OnPointerReleased( e );

            if ( e.InitialPressMouseButton == MouseButton.Left )
            {
                var position = e.GetPosition( this );
                mScene?.Raypick( (int)position.X, (int)position.Y );
            }
        }

        protected override void OnPointerWheelChanged( PointerWheelEventArgs e )
        {
            base.OnPointerWheelChanged( e );

            // A wheel notch is one unit here and 120 in WinForms, which the scene is scaled for.
            mScene?.Zoom( (float)e.Delta.Y * 120f, Multiplier( e.KeyModifiers, 0.25f ) );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            base.OnKeyDown( e );

            if ( e.Key == global::Avalonia.Input.Key.Space )
                mScene?.ResetView();
        }

        private double Scaling => global::Avalonia.Controls.TopLevel.GetTopLevel( this )?.RenderScaling ?? 1.0;

        private int PixelWidth => Math.Max( (int)( Bounds.Width * Scaling ), 1 );

        private int PixelHeight => Math.Max( (int)( Bounds.Height * Scaling ), 1 );

        private static float Multiplier( KeyModifiers modifiers, float baseValue = 0.5f )
        {
            if ( modifiers.HasFlag( KeyModifiers.Shift ) )
                return baseValue * 10f;

            if ( modifiers.HasFlag( KeyModifiers.Control ) )
                return baseValue / 2f;

            return baseValue;
        }

        private sealed class AvaloniaBindings : IBindingsContext
        {
            private readonly GlInterface mGl;

            public AvaloniaBindings( GlInterface gl ) => mGl = gl;

            public IntPtr GetProcAddress( string procName ) => mGl.GetProcAddress( procName );
        }
    }
}
