using GFDLibrary.Cameras;
using GFDLibrary.Lights;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Scarlet.Platform.Sony;
using GFDLibrary.Materials;
using GFDLibrary.Animations;

namespace GFDLibrary.Api
{
    /// <summary>
    /// Simplified API for integration with limited .NET interop.
    /// </summary>
    public static class FlatApi
    {
        private static Dictionary<Model, Node[]> sCachedModelNodes;
        private static Dictionary<Model, Node[]> sCachedModelBoneNodes;

        static FlatApi()
        {
            sCachedModelNodes = new Dictionary<Model, Node[]>();
            sCachedModelBoneNodes = new Dictionary<Model, Node[]>();
        }

        public static ModelPack LoadModel( string path )
        {
            return Resource.Load<ModelPack>( path );
        }

        public static Node[] GetModelNodes( Model model )
        {
            if ( model == null ) return Array.Empty<Node>();

            if (!sCachedModelNodes.ContainsKey(model))
            {
                return sCachedModelNodes[ model ] = model.Nodes.ToArray();
            }
            else
            {
                return sCachedModelNodes[ model ];
            }
        }

        public static Node[] GetNodeChildren( Node node )
        {
            if ( !node.HasChildren ) return Array.Empty<Node>();
            return node.Children.ToArray();
        }

        public static Mesh[] GetNodeMeshes( Node node )
        {
            if ( !node.HasAttachments ) return Array.Empty<Mesh>();
            return node.Attachments.Where( x => x.Type == NodeAttachmentType.Mesh ).Select( x => x.GetValue<Mesh>() ).ToArray();
        }

        public static Camera[] GetNodeCameras( Node node )
        {
            if ( !node.HasAttachments ) return Array.Empty<Camera>();
            return node.Attachments.Where( x => x.Type == NodeAttachmentType.Camera ).Select( x => x.GetValue<Camera>() ).ToArray();
        }

        public static Light[] GetNodeLights( Node node )
        {
            if ( !node.HasAttachments ) return Array.Empty<Light>();
            return node.Attachments.Where( x => x.Type == NodeAttachmentType.Light ).Select( x => x.GetValue<Light>() ).ToArray();
        }

        public static Morph[] GetNodeMorphs( Node node )
        {
            if ( !node.HasAttachments ) return Array.Empty<Morph>();
            return node.Attachments.Where( x => x.Type == NodeAttachmentType.Morph ).Select( x => x.GetValue<Morph>() ).ToArray();
        }

        public static string[][] GetNodeUserProperties( Node node )
        {
            if ( !node.HasProperties ) return Array.Empty<string[]>();
            return node.Properties.Select( x => new string[] { x.Key, x.Value.GetUserPropertyValueString() } ).ToArray();
        }

        public static float[][] GetMeshVertexPositions( Mesh mesh )
        {
            if ( !mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                return null;

            return mesh.Vertices.Select( Vector3ToFloatArray ).ToArray();
        }

        public static float[][] GetMeshVertexNormals( Mesh mesh )
        {
            if ( !mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
                return null;

            return mesh.Normals.Select( Vector3ToFloatArray ).ToArray();
        }

        public static float[][] GetMeshVertexTexCoords( Mesh mesh, int index )
        {
            if ( !mesh.VertexAttributeFlags.HasFlag( (VertexAttributeFlags)( (int)VertexAttributeFlags.TexCoord0 << index ) ) )
                return null;

            return mesh.TexCoordChannels[ index ].Select( Vector2ToFloatArray ).ToArray();
        }

        public static float[][][] GetMeshTransformed( Model model, Node node, Mesh mesh, bool convertToYUp )
        {
            Matrix4x4? transform = null;
            if ( convertToYUp )
                transform = new Matrix4x4( 1,  0, 0, 0,
                                           0,  0, 1, 0,
                                           0, -1, 0, 0,
                                           0,  0, 0, 1 );

            ( var vertices, var normals ) = mesh.Transform( node, GetModelNodes( model ), model.Bones, 
                includeWeights: true, includeParentTransform: true, offsetTransform: transform );
            return new float[][][] { 
                vertices.Select( Vector3ToFloatArray ).ToArray(), 
                normals?.Select( Vector3ToFloatArray )?.ToArray() ?? null };
        }

        public static uint[][] GetMeshTriangles( Mesh mesh, int baseIndex )
        {
            if ( !mesh.Flags.HasFlag( GeometryFlags.HasTriangles ) )
                return null;

            return mesh.Triangles.Select( x => new uint[] { (uint)( x.A + baseIndex ), (uint)(x.B + baseIndex), (uint)(x.C + baseIndex) } ).ToArray();
        }

        public static uint[] GetMeshVertexColors( Mesh mesh, int index )
        {
            var flag = VertexAttributeFlags.Color0;
            if ( index == 1 )
                flag = VertexAttributeFlags.Color1;
            if ( index == 2 )
                flag = VertexAttributeFlags.Color2;

            if ( !mesh.VertexAttributeFlags.HasFlag( flag ) )
                return null;

            return mesh.ColorChannels[ index ];
        }

        public static float[][] GetMeshVertexWeights( Mesh mesh )
        {
            if ( mesh.VertexWeights == null ) return null;
            var weights = new float[ mesh.VertexWeights.Length ][];
            for ( int i = 0; i < weights.Length; i++ )
            {
                var numUsed = mesh.VertexWeights[ i ].Weights.Count( x => x != 0 );
                var vweights = new float[ numUsed ];
                for ( int j = 0; j < vweights.Length; j++ )
                {
                    vweights[ j ] = mesh.VertexWeights[ i ].Weights[ j ];
                }
                weights[ i ] = vweights;
            }
            return weights;
        }

        public static int[][] GetMeshVertexWeightIndices( Mesh mesh, int baseIndex = 0 )
        {
            if ( mesh.VertexWeights == null ) return null;
            var weights = new int[ mesh.VertexWeights.Length ][];
            for ( int i = 0; i < weights.Length; i++ )
            {
                var numUsed = mesh.VertexWeights[ i ].Weights.Count( x => x != 0 );
                var vweights = new int[ numUsed ];
                for ( int j = 0; j < vweights.Length; j++ )
                {
                    vweights[ j ] = mesh.VertexWeights[ i ].Indices[ j ] + baseIndex;
                }
                weights[ i ] = vweights;
            }
            return weights;
        }

        public static Node[] GetModelBoneNodes( Model model )
        {
            if ( sCachedModelBoneNodes.TryGetValue( model, out var boneNodes ) )
            {
                return boneNodes;
            }
            else
            {
                if ( model.Bones == null )
                {
                    return sCachedModelBoneNodes[ model ] = null;
                }
                else
                {
                    var nodes = GetModelNodes( model );
                    return sCachedModelBoneNodes[ model ] = model.Bones.Select( x => nodes[ x.NodeIndex ] ).ToArray();
                }
            }
        }

        public static Material GetMaterialByName( MaterialDictionary materialDictionary, string name )
        {
            if ( !materialDictionary.TryGetMaterial( name, out var mat ) )
                return null;

            return mat;
        }

        public static string[] GetMaterialMaps( Material material, Dictionary<string, string> exportedTextures )
        {
            var maps = new List<string>();
            foreach ( var texMap in material.TextureMaps )
            {
                if ( texMap == null )
                {
                    maps.Add( null );
                    continue;
                }

                if ( !exportedTextures.TryGetValue( texMap.Name, out var texPath ))
                {
                    maps.Add( texMap.Name );
                    continue;
                }

                maps.Add( texPath );
            }

            return maps.ToArray();
        }

        public static Dictionary<string, string> ExportTextures( TextureDictionary textureDictionary, string dirPath )
        {
            Directory.CreateDirectory( dirPath );
            var texPaths = new Dictionary<string, string>();

            foreach ( var tex in textureDictionary.Textures )
            {
                string texPath;

                switch ( tex.Format )
                {
                    case TextureFormat.DDS:
                        texPath = Path.Combine( dirPath, Path.ChangeExtension( tex.Name, ".dds" ) );
                        File.WriteAllBytes( texPath, tex.Data );
                        break;
                    default:
                        texPath = Path.Combine( dirPath, Path.ChangeExtension( tex.Name, ".png" ) );
                        TextureDecoder.Decode( tex ).Save( texPath );
                        break;
                }

                texPaths[ tex.Name ] = texPath;
            }

            return texPaths;
        }

        public static float[] Vector3ToFloatArray( Vector3 v )
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        public static float[] Vector2ToFloatArray( Vector2 v )
        {
            return new float[] { v.X, v.Y };
        }

        public static float[] Vector4ToFloatArray( Vector4 v )
        {
            return new float[] { v.X, v.Y, v.Z, v.W };
        }

        public static float[] Matrix4x4ToFloatArray( Matrix4x4 v )
        {
            return new float[] { v.M11, v.M12, v.M13, v.M14,
                                 v.M21, v.M22, v.M23, v.M24,
                                 v.M31, v.M32, v.M33, v.M34,
                                 v.M41, v.M42, v.M43, v.M44 };
        }

        public static float[] QuaternionToFloatArray( Quaternion v )
        {
            return new float[] { v.X, v.Y, v.Z, v.W };
        }

        public static bool IsValidFile( string filePath )
        {
            if ( !File.Exists( filePath ) ) return false;
            using ( var stream = File.OpenRead( filePath ) )
                return Resource.GetResourceType( stream ) == ResourceType.ModelPack;
        }

        public static Animation[] GetAnimations(  AnimationPack ap )
        {
            if ( ap == null ) return Array.Empty<Animation>();
            if ( ap.Animations == null ) return Array.Empty<Animation>();
            return ap.Animations.ToArray();
        }

        public static Animation[] GetBlendAnimations( AnimationPack ap )
        {
            if ( ap == null ) return Array.Empty<Animation>();
            if ( ap.BlendAnimations == null ) return Array.Empty<Animation>();
            return ap.BlendAnimations.ToArray();
        }

        public static Animation GetAnimationByIndex( AnimationPack animationPack, int index )
        {
            if ( animationPack == null || animationPack.Animations == null || index > animationPack.Animations.Count )
                return null;

            return animationPack.Animations[index];
        }

        public static Animation GetBlendAnimationByIndex( AnimationPack animationPack, int index )
        {
            if ( animationPack == null || animationPack.BlendAnimations == null || index > animationPack.BlendAnimations.Count )
                return null;

            return animationPack.BlendAnimations[index];
        }

        public static AnimationController[] GetAnimationControllers( Animation a )
        {
            return a.Controllers.ToArray();
        }

        public static AnimationLayer[] GetAnimationLayers( AnimationController c )
        {
            return c.Layers.ToArray();
        }

        public static int GetAnimationControllerTargetKind( AnimationController c )
        {
            return (int)c.TargetKind;
        }

        public static int GetAnimationLayerKeyType( AnimationLayer l )
        {
            return (int)l.KeyType;
        }

        public static bool GetAnimationLayerHasPRSKeyFrames( AnimationLayer l )
        {
            return l.HasPRSKeyFrames;
        }

        public static float[][] GetAnimationLayerPRSKeyFrameData( AnimationLayer l, bool invertRotation )
        {
            if ( !l.HasPRSKeyFrames ) return Array.Empty<float[]>();
            return l.Keys
                .Select( x => (PRSKey)x )
                .Select( x => new { 
                    Time = x.Time, 
                    Position = x.Position * l.PositionScale, 
                    Rotation = invertRotation ? Quaternion.Inverse( x.Rotation ) : x.Rotation, 
                    Scale = x.Scale * l.ScaleScale })
                .Select( x => new[] { x.Time, 
                    x.Position.X, x.Position.Y, x.Position.Z, 
                    x.Rotation.X, x.Rotation.Y, x.Rotation.Z, x.Rotation.W, 
                    x.Scale.X, x.Scale.Y, x.Scale.Z } )
                .ToArray();
        }

        public static float[][] GetAnimationLayerSingleKeyFrameData( AnimationLayer l )
        {

            if ( l.HasSingleKeyFrames )
                return Array.Empty<float[]>();

            return l.Keys
                .Select( x => (SingleKey)x )
                .Select( x => new[] { x.Time, x.Value } )
                .ToArray();
        }
    }
}
