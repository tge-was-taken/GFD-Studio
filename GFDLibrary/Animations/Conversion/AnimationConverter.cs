using System.Linq;
using System.Numerics;
using GFDLibrary.Models.Conversion;
using Ai = Assimp;

namespace GFDLibrary.Animations.Conversion
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

                var layer = new AnimationLayer( options.Version );

                // NodePRS only for now
                layer.KeyType = KeyType.NodePRS;

                // Fetch the unique key frame timings from all position, rotation and scale keys.
                var aiKeyTimings = aiChannel.PositionKeys
                                               .Select( x => x.Time )
                                               .Concat( aiChannel.RotationKeys.Select( x => x.Time ) )
                                               .Concat( aiChannel.ScalingKeys.Select( x => x.Time ) )
                                               .Distinct()
                                               .OrderBy( x => x )
                                               .ToList();

                // Decompose the local transform of the affected node so we can use them as the base values for our keyframes
                aiScene.RootNode.FindNode( nodeName ).Transform
                       .Decompose( out var nodeBaseScale, out var nodeBaseRotation, out var nodeBaseTranslation );

                // Keep track of the last position, rotation and scale used to ensure that interpolation works properly
                var lastPosition = nodeBaseTranslation;
                var lastRotation = nodeBaseRotation;
                var lastScale = nodeBaseScale;

                for ( var i = 0; i < aiKeyTimings.Count; i++ )
                {
                    var aiTime = aiKeyTimings[ i ];

                    // Start building the keyframe
                    var key = new PRSKey( layer.KeyType )
                    {
                        Position = new Vector3( lastPosition.X, lastPosition.Y, lastPosition.Z ),
                        Rotation = new Quaternion( lastRotation.X, lastRotation.Y, lastRotation.Z, lastRotation.W ),
                        Scale    = new Vector3( lastScale.X, lastScale.Y, lastScale.Z )
                    };

                    // Fetch the Assimp keys for this time
                    var aiPositionKey = aiChannel.PositionKeys.SingleOrDefault( x => x.Time == aiTime );
                    var aiRotationKey = aiChannel.RotationKeys.SingleOrDefault( x => x.Time == aiTime );
                    var aiScaleKey    = aiChannel.ScalingKeys.SingleOrDefault( x => x.Time == aiTime );

                    if ( aiPositionKey != default )
                    {
                        key.Position = new Vector3( aiPositionKey.Value.X, aiPositionKey.Value.Y, aiPositionKey.Value.Z );
                        lastPosition = aiPositionKey.Value;
                    }

                    if ( aiRotationKey != default )
                    {
                        key.Rotation = new Quaternion( aiRotationKey.Value.X, aiRotationKey.Value.Y, aiRotationKey.Value.Z,
                                                       aiRotationKey.Value.W );
                        lastRotation = aiRotationKey.Value;
                    }

                    if ( aiScaleKey != default )
                    {
                        key.Scale = new Vector3( aiScaleKey.Value.X, aiScaleKey.Value.Y, aiScaleKey.Value.Z );
                        lastScale = aiScaleKey.Value;
                    }

                    key.Time = ConvertTime( aiTime, aiAnimation.TicksPerSecond );
                    layer.Keys.Add( key );
                }

                controller.Layers.Add( layer );
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
