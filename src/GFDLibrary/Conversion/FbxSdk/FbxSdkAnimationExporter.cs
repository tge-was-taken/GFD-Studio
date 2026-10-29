using System;
using System.Collections.Generic;
using System.Linq;
using GFDLibrary.Animations;
using GFDLibrary.Models;

namespace GFDLibrary.Conversion.FbxSdk
{
    /// <summary>
    /// Writes GFD animations into an FBX scene as a take per animation. The node tree has to be
    /// in the scene already, so this runs off the model exporter's node map.
    /// </summary>
    internal sealed class FbxSdkAnimationExporter
    {
        private readonly IntPtr mScene;
        private readonly IReadOnlyDictionary<string, IntPtr> mNodesByName;
        private readonly bool mAlignEulers;

        public FbxSdkAnimationExporter( IntPtr scene, IReadOnlyDictionary<string, IntPtr> nodesByName, bool alignEulers = true )
        {
            mScene = scene;
            mNodesByName = nodesByName;
            mAlignEulers = alignEulers;
        }

        public void Export( AnimationPack pack )
        {
            if ( pack == null )
                return;

            // A pack can hold either kind, and some hold only blend animations, so both lists
            // are walked rather than just the first.
            Export( pack.Animations, "Take" );
            Export( pack.BlendAnimations, "Blend" );
        }

        private void Export( List<Animation> animations, string prefix )
        {
            if ( animations == null )
                return;

            for ( var i = 0; i < animations.Count; i++ )
                ExportAnimation( animations[i], $"{prefix} {i + 1:D3}" );
        }

        private void ExportAnimation( Animation animation, string name )
        {
            if ( animation?.Joints == null )
                return;

            var stack = FbxSdk.AnimStackCreate( mScene, name );
            var layer = FbxSdk.AnimLayerCreate( mScene, "Base Layer" );
            FbxSdk.AnimStackAddLayer( stack, layer );
            FbxSdk.AnimStackSetLocalTimeSpan( stack, 0, animation.Duration );

            foreach ( var joint in animation.Joints.Where( x => x.TargetKind == TargetKind.Node ) )
            {
                if ( joint.TargetName == null || !mNodesByName.TryGetValue( joint.TargetName, out var node ) )
                    continue;

                foreach ( var animationLayer in joint.Layers ?? Enumerable.Empty<AnimationLayer>() )
                    ExportLayer( animationLayer, node, layer );
            }
        }

        private void ExportLayer( AnimationLayer source, IntPtr node, IntPtr layer )
        {
            if ( source?.Keys == null )
                return;

            var keys = source.Keys.OfType<PRSKey>().ToList();
            if ( keys.Count == 0 )
                return;

            var translation = new Channel( node, layer, FbxTransformChannel.Translation );
            var rotation = new Channel( node, layer, FbxTransformChannel.Rotation );
            var scale = new Channel( node, layer, FbxTransformChannel.Scaling );

            var positionScale = source.PositionScale;
            var scaleScale = source.ScaleScale;
            var euler = new double[3];
            var quaternion = new double[4];
            var previousEuler = new double[3];
            var hasPreviousEuler = false;

            foreach ( var key in keys )
            {
                if ( key.HasPosition )
                    translation.Add( key.Time,
                                     key.Position.X * positionScale.X,
                                     key.Position.Y * positionScale.Y,
                                     key.Position.Z * positionScale.Z );

                if ( key.HasRotation )
                {
                    quaternion[0] = key.Rotation.X;
                    quaternion[1] = key.Rotation.Y;
                    quaternion[2] = key.Rotation.Z;
                    quaternion[3] = key.Rotation.W;

                    // The SDK's own conversion, so the angles land in the order and units
                    // LclRotation expects.
                    FbxSdk.QuaternionToEulerDegrees( quaternion, euler );

                    if ( mAlignEulers )
                        AlignEuler( euler, previousEuler, ref hasPreviousEuler );

                    rotation.Add( key.Time, (float)euler[0], (float)euler[1], (float)euler[2] );
                }

                if ( key.HasScale )
                    scale.Add( key.Time,
                               key.Scale.X * scaleScale.X,
                               key.Scale.Y * scaleScale.Y,
                               key.Scale.Z * scaleScale.Z );
            }

            translation.Close();
            rotation.Close();
            scale.Close();
        }

        /// <summary>
        /// Shifts each angle by whole turns so it sits next to the one before it. Without this a
        /// track crossing 180 degrees reads as a jump to -180, and anything interpolating the
        /// curve, 3ds Max included, spins the bone the long way round.
        /// </summary>
        private static void AlignEuler( double[] euler, double[] previousEuler, ref bool hasPreviousEuler )
        {
            if ( hasPreviousEuler )
            {
                for ( var i = 0; i < 3; i++ )
                    euler[i] += 360.0 * Math.Round( ( previousEuler[i] - euler[i] ) / 360.0 );
            }

            Array.Copy( euler, previousEuler, 3 );
            hasPreviousEuler = true;
        }

        /// <summary>
        /// The three component curves of one transform channel. They are created on first use,
        /// so a layer that never touches a channel leaves no empty curves behind.
        /// </summary>
        private sealed class Channel
        {
            private readonly IntPtr mNode;
            private readonly IntPtr mLayer;
            private readonly FbxTransformChannel mChannel;
            private IntPtr[] mCurves;

            public Channel( IntPtr node, IntPtr layer, FbxTransformChannel channel )
            {
                mNode = node;
                mLayer = layer;
                mChannel = channel;
            }

            public void Add( float time, float x, float y, float z )
            {
                Open();

                FbxSdk.AnimCurveKeyAddLinear( mCurves[0], time, x );
                FbxSdk.AnimCurveKeyAddLinear( mCurves[1], time, y );
                FbxSdk.AnimCurveKeyAddLinear( mCurves[2], time, z );
            }

            public void Close()
            {
                if ( mCurves == null )
                    return;

                foreach ( var curve in mCurves )
                    FbxSdk.AnimCurveKeyModifyEnd( curve );

                mCurves = null;
            }

            private void Open()
            {
                if ( mCurves != null )
                    return;

                mCurves = new IntPtr[3];

                for ( var i = 0; i < 3; i++ )
                {
                    mCurves[i] = FbxSdk.NodeGetAnimCurve( mNode, mLayer, mChannel, i );

                    if ( mCurves[i] == IntPtr.Zero )
                        throw new FbxSdkModelPackExporterException(
                            $"Failed to create the {mChannel} curve: {FbxSdk.GetLastError()}" );

                    FbxSdk.AnimCurveKeyModifyBegin( mCurves[i] );
                }
            }
        }
    }
}
