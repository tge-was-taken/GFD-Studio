using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using Ai = Assimp;

namespace GFDLibrary.Models.Conversion
{
    public static class AnimationConverter
    {
        public static Animation ConvertFromAssimpScene( string filePath, AnimationConverterOptions options )
        {
            var aiScene = AssimpSceneImporter.ImportFile( filePath );
            return ConvertFromAssimpScene( aiScene, options );
        }

        public static Animation ConvertFromAssimpScene( Ai.Scene aiScene, AnimationConverterOptions options )
        {
            var aiAnimation = aiScene.Animations.FirstOrDefault();
            return aiAnimation != null ? ConvertFromAssimpScene( aiScene, aiAnimation, options ) : null;
        }

        public static Animation ConvertFromAssimpScene( Ai.Scene aiScene, Ai.Animation aiAnimation, AnimationConverterOptions options )
        {
            var animation = new Animation( options.Version );
            animation.Duration = ConvertTime( aiAnimation.DurationInTicks, aiAnimation.TicksPerSecond );

            foreach ( var aiChannel in aiAnimation.NodeAnimationChannels )
            {
                if ( AssimpConverterCommon.MeshAttachmentNameRegex.IsMatch( aiChannel.NodeName ) )
                    continue;

                var nodeName = AssimpConverterCommon.UnescapeName( aiChannel.NodeName );

                var controller = new AnimationController( options.Version )
                {
                    TargetKind = TargetKind.Node,
                    TargetName = nodeName,
                    TargetId = GetTargetIdForNode( aiScene.RootNode, nodeName )
                };

                var track = new AnimationLayer( options.Version );

                // NodePRS only for now
                track.KeyType = KeyType.NodePRS;

                // Fetch the unique key frame timings from all position, rotation and scale keys.
                var keyframeTimings = aiChannel.PositionKeys
                                               .Select( x => x.Time )
                                               .Concat( aiChannel.RotationKeys.Select( x => x.Time ) )
                                               .Concat( aiChannel.ScalingKeys.Select( x => x.Time ) )
                                               .Distinct()
                                               .OrderBy( x => x );

                // Convert the times to our scale and save them.
                track.KeyTimings = keyframeTimings
                    .Select( x => ConvertTime( x, aiAnimation.TicksPerSecond ) )
                    .ToList();

                // Decompose the local transform of the affected node so we can use them as the base values for our keyframes
                aiScene.RootNode.FindNode( nodeName ).Transform
                       .Decompose( out var nodeBaseScale, out var nodeBaseRotation, out var nodeBaseTranslation );

                // Keep track of the last position, rotation and scale used to ensure that interpolation works properly
                var lastPosition = nodeBaseTranslation;
                var lastRotation = nodeBaseRotation;
                var lastScale = nodeBaseScale;

                foreach ( var time in keyframeTimings )
                {
                    // Start building the keyframe
                    var keyframe = new PRSKey( track.KeyType )
                    {
                        Position = new Vector3( lastPosition.X, lastPosition.Y, lastPosition.Z ),
                        Rotation = new Quaternion( lastRotation.X, lastRotation.Y, lastRotation.Z, lastRotation.W ),
                        Scale = new Vector3( lastScale.X, lastScale.Y, lastScale.Z )
                    };

                    // Fetch the Assimp keys for this time
                    var aiPositionKey = aiChannel.PositionKeys.SingleOrDefault( x => x.Time == time );
                    var aiRotationKey = aiChannel.RotationKeys.SingleOrDefault( x => x.Time == time );
                    var aiScaleKey = aiChannel.ScalingKeys.SingleOrDefault( x => x.Time == time );

                    if ( aiPositionKey != default( Ai.VectorKey ) )
                    {
                        keyframe.Position = new Vector3( aiPositionKey.Value.X, aiPositionKey.Value.Y, aiPositionKey.Value.Z );
                        lastPosition = aiPositionKey.Value;
                    }

                    if ( aiRotationKey != default( Ai.QuaternionKey ) )
                    {
                        keyframe.Rotation = new Quaternion( aiRotationKey.Value.X, aiRotationKey.Value.Y, aiRotationKey.Value.Z,
                                                            aiRotationKey.Value.W );
                        lastRotation = aiRotationKey.Value;
                    }

                    if ( aiScaleKey != default( Ai.VectorKey ) )
                    {
                        keyframe.Scale = new Vector3( aiScaleKey.Value.X, aiScaleKey.Value.Y, aiScaleKey.Value.Z );
                        lastScale = aiScaleKey.Value;
                    }

                    track.Keys.Add( keyframe );
                }

                controller.Layers.Add( track );
                animation.Controllers.Add( controller );
            }

            return animation;
        }

        private static float ConvertTime( double ticks, double ticksPerSecond )
        {
            return ( float )( ticks / ( ( ticksPerSecond / 30f ) * 30f ) );
        }

        private static int GetTargetIdForNode( Ai.Node rootNode, string nodeName )
        {
            int targetId = 0;

            bool GetTargetIdForNodeRecursive( Ai.Node node )
            {
                if ( node.Name == nodeName )
                {
                    return true;
                }

                ++targetId;

                foreach ( var child in node.Children )
                {
                    if ( GetTargetIdForNodeRecursive( child ) )
                        return true;
                }

                return false;
            }

            if ( GetTargetIdForNodeRecursive( rootNode ) )
                return targetId;
            else
                return -1;
        }
    }

    public class AnimationConverterOptions
    {
        /// <summary>
        /// Gets or sets the version to use for the converted resources.
        /// </summary>
        public uint Version { get; set; }

        public AnimationConverterOptions()
        {
            Version = ResourceVersion.Persona5;
        }
    }
}
