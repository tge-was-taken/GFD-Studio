using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Lights;
using GFDLibrary.Materials;
using GFDLibrary.Models.Conversion.Utilities;
using GFDLibrary.Textures;
using GFDLibrary.Utilities;
using Ai = Assimp;

namespace GFDLibrary.Models.Conversion
{
    public class ModelPackExporter
    {
        private Ai.Scene mAiScene;
        private string mPath;
        private string mFileName;
        private string mBaseDirectoryPath;
        private string mTextureBaseDirectoryPath;
        private string mTextureBaseRelativeDirectoryPath;

        public static void ExportFile( ModelPack modelPack, string path )
        {
            var exporter = new ModelPackExporter();
            var scene = exporter.ConvertModelPack( modelPack, path );
            ExportFile( scene, path );
        }

        public static void ExportFile( Ai.Scene scene, string path )
        {
            var aiContext = new Ai.AssimpContext();
            aiContext.XAxisRotation = 90;
            aiContext.ExportFile( scene, path, "collada", Ai.PostProcessSteps.FlipUVs );
        }

        private void Init( string path )
        {
            mAiScene = new Ai.Scene();
            mPath = Path.GetFullPath( path );
            mFileName = Path.GetFileNameWithoutExtension( mPath );
            mBaseDirectoryPath = Path.GetDirectoryName( mPath );
            mTextureBaseDirectoryPath = Path.Combine( mBaseDirectoryPath, $"{mFileName}_Textures" );
            mTextureBaseRelativeDirectoryPath = mTextureBaseDirectoryPath.Substring( mBaseDirectoryPath.Length );
        }

        private Ai.Scene ConvertModelPack( ModelPack modelPack, string path )
        {
            Init( path );

            if ( modelPack.Textures != null )
                ConvertTextures( modelPack.Textures );

            if ( modelPack.Materials != null )
                ConvertMaterials( modelPack.Materials );

            if ( modelPack.Model != null )
                ConvertModel( modelPack.Model );

            return mAiScene;
        }

        private void ConvertTextures( TextureDictionary textureDictionary )
        {
            Directory.CreateDirectory( mTextureBaseDirectoryPath );

            foreach ( var texture in textureDictionary.Textures )
            {
                var texturePath = Path.Combine( mTextureBaseDirectoryPath, AssimpConverterCommon.EscapeName(texture.Name) );

                File.WriteAllBytes( texturePath, texture.Data );
            }
        }

        private void ConvertMaterials( MaterialDictionary materialDictionary )
        {
            foreach ( var material in materialDictionary.Materials )
            {
                mAiScene.Materials.Add( ConvertMaterial( material ) );
            }
        }

        private Ai.Material InitializeAssimpMaterial( Material material )
        {
            var aiMaterial = new Ai.Material { Name = AssimpConverterCommon.EscapeName( material.Name ) };
            if ( material.METAPHOR_MaterialParameterSet != null )
                material.METAPHOR_MaterialParameterSet.ConvertToAssimp(ref aiMaterial);
            else
            {
                aiMaterial.ColorAmbient = material.LegacyParameters.AmbientColor.ToAssimp();
                aiMaterial.ColorDiffuse = material.LegacyParameters.DiffuseColor.ToAssimp();
                aiMaterial.ColorSpecular = material.LegacyParameters.SpecularColor.ToAssimp();
                aiMaterial.ColorEmissive = material.LegacyParameters.EmissiveColor.ToAssimp();
            }
            return aiMaterial;
        }

        private Ai.Material ConvertMaterial( Material material )
        {
            var aiMaterial = InitializeAssimpMaterial( material );
            if ( material.Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                aiMaterial.TextureDiffuse = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, AssimpConverterCommon.EscapeName(material.DiffuseMap.Name) ),
                    Ai.TextureType.Diffuse, 0, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                aiMaterial.TextureNormal = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, AssimpConverterCommon.EscapeName(material.NormalMap.Name) ), 
                    Ai.TextureType.Normals, 1, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                aiMaterial.TextureSpecular = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, AssimpConverterCommon.EscapeName(material.SpecularMap.Name) ), 
                    Ai.TextureType.Specular, 2, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                aiMaterial.TextureReflection = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, AssimpConverterCommon.EscapeName(material.ReflectionMap.Name) ), 
                    Ai.TextureType.Reflection, 3, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            // todo: add more textures

            return aiMaterial;
        }

        private void ConvertModel( Model model )
        {
            mAiScene.RootNode = ConvertNode( model, model.RootNode, null );
        }

        private Ai.Node ConvertNode( Model model, Node node, Ai.Node aiParent )
        {
            var aiNode = new Ai.Node( AssimpConverterCommon.EscapeName( node.Name ), aiParent )
            {
                Transform = new Ai.Matrix4x4( node.LocalTransform.M11, node.LocalTransform.M21, node.LocalTransform.M31, node.LocalTransform.M41,
                                              node.LocalTransform.M12, node.LocalTransform.M22, node.LocalTransform.M32, node.LocalTransform.M42,
                                              node.LocalTransform.M13, node.LocalTransform.M23, node.LocalTransform.M33, node.LocalTransform.M43,
                                              node.LocalTransform.M14, node.LocalTransform.M24, node.LocalTransform.M34, node.LocalTransform.M44 )
            };

            if ( node.HasProperties )
            {
                ConvertNodeProperties( node.Properties, aiNode );
            }

            if ( node.HasAttachments )
            {
                ConvertNodeAttachments( model, node, aiNode );
            }

            if ( node.HasChildren )
            {
                foreach ( var childNode in node.Children )
                {
                    aiNode.Children.Add( ConvertNode( model, childNode, aiNode ) );
                }
            }

            return aiNode;
        }

        private static void ConvertNodeProperties( UserPropertyDictionary properties, Ai.Node aiNode )
        {
            var stringBuilder = new StringBuilder();

            foreach ( var property in properties.Values )
            {
                // Todo: Assign to 3ds max user properties
                stringBuilder.Append( $"{property.ToUserPropertyString()}&cr;&lf;" );
            }

            aiNode.Metadata["UDP3DSMAX"] = new Ai.Metadata.Entry( Ai.MetaDataType.String, stringBuilder.ToString() );
        }

        private void ConvertNodeAttachments( Model model, Node node, Ai.Node aiNode )
        {
            for ( int i = 0; i < node.Attachments.Count; i++ )
            {
                var attachment = node.Attachments[i];

                switch ( attachment.Type )
                {
                    case NodeAttachmentType.Mesh:
                        {
                            var mesh = ConvertGeometry( model, node, attachment.GetValue<Mesh>() );

                            mesh.Name = $"{AssimpConverterCommon.EscapeName(node.Name)}_Attachment{i}_Mesh";
                            aiNode.MeshIndices.Add( mAiScene.Meshes.Count );
                            mAiScene.Meshes.Add( mesh );
                        }
                        break;

                    case NodeAttachmentType.Camera:
                        {
                            var camera = attachment.GetValue<Camera>();
                            mAiScene.Cameras.Add( new Ai.Camera
                            {
                                Name          = node.Name,
                                Position      = camera.Position.ToAssimp(),
                                Up            = camera.Up.ToAssimp(),
                                Direction     = -camera.Direction.ToAssimp(),
                                FieldOfview   = MathHelper.DegreesToRadians( camera.FieldOfView ),
                                ClipPlaneNear = camera.ClipPlaneNear,
                                ClipPlaneFar  = camera.ClipPlaneFar,
                                AspectRatio   = camera.AspectRatio
                            });
                        }
                        break;

                    case NodeAttachmentType.Light:
                        {
                            var light = attachment.GetValue<Light>();
                            mAiScene.Lights.Add( new Ai.Light
                            {
                                Name = node.Name,
                                AngleInnerCone = light.AngleInnerCone,
                                AngleOuterCone = light.AngleOuterCone,
                                ColorAmbient = light.AmbientColor.ToAssimpAsColor3D(),
                                ColorDiffuse = light.DiffuseColor.ToAssimpAsColor3D(),
                                ColorSpecular = light.SpecularColor.ToAssimpAsColor3D(),
                                LightType = light.Type == LightType.Point ? Ai.LightSourceType.Point : light.Type == LightType.Spot ? Ai.LightSourceType.Spot : Ai.LightSourceType.Directional,
                            });
                        }
                        break;
                    default:
                        //throw new NotImplementedException();
                        break;
                }
            }
        }

        private Ai.Mesh ConvertGeometry( Model model, Node geometryNode, Mesh mesh )
        {
            var aiMesh = new Ai.Mesh( Ai.PrimitiveType.Triangle );

            if ( mesh.Flags.HasFlag( GeometryFlags.HasMaterial ) )
                aiMesh.MaterialIndex = mAiScene.Materials.FindIndex( x => x.Name == AssimpConverterCommon.EscapeName(mesh.MaterialName) );

            if ( mesh.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                foreach ( var triangle in mesh.Triangles )
                {
                    var aiFace = new Ai.Face();
                    aiFace.Indices.Add( ( int )triangle.A );
                    aiFace.Indices.Add( ( int )triangle.B );
                    aiFace.Indices.Add( ( int )triangle.C );
                    aiMesh.Faces.Add( aiFace );
                }
            }

            var vertices = mesh.Vertices;
            var normals = mesh.Normals;

            if ( mesh.VertexWeights != null )
                ( vertices, normals ) = mesh.Transform( geometryNode, model.Nodes.ToList(), model.Bones );

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                foreach ( var vertex in vertices )
                {
                    aiMesh.Vertices.Add( new Ai.Vector3D( vertex.X, vertex.Y, vertex.Z ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
            {
                foreach ( var normal in normals )
                {
                    aiMesh.Normals.Add( new Ai.Vector3D( normal.X, normal.Y, normal.Z ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
            {
                foreach ( var tangent in mesh.Tangents )
                {
                    aiMesh.Tangents.Add( new Ai.Vector3D( tangent.X, tangent.Y, tangent.Z ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
            {
                foreach ( var binormal in mesh.Binormals )
                {
                    aiMesh.BiTangents.Add( new Ai.Vector3D( binormal.X, binormal.Y, binormal.Z ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                foreach ( var vertex in mesh.TexCoordsChannel0 )
                {
                    aiMesh.TextureCoordinateChannels[0].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
            {
                foreach ( var vertex in mesh.TexCoordsChannel1 )
                {
                    aiMesh.TextureCoordinateChannels[1].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
            {
                foreach ( var vertex in mesh.TexCoordsChannel2 )
                {
                    aiMesh.TextureCoordinateChannels[2].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            /* todo: colors
            if ( mesh.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
            {
                foreach ( var color in mesh.ColorChannel0 )
                {
                    aiMesh.VertexColorChannels[0].Add( new Ai.Color4D( color. ))
                }
            }
            */

            if ( mesh.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
            {
                var boneMap = new Dictionary<int, Ai.Bone>();

                for ( int i = 0; i < mesh.VertexWeights.Length; i++ )
                {
                    var vertexWeight = mesh.VertexWeights[i];

                    for ( int j = 0; j < 4; j++ )
                    {
                        var boneWeight = vertexWeight.Weights[j];
                        if ( boneWeight == 0f )
                            continue;

                        var boneIndex = vertexWeight.Indices[j];
                        var nodeIndex = model.Bones[boneIndex].NodeIndex;

                        if ( !boneMap.ContainsKey( nodeIndex ) )
                        {
                            var aiBone = new Ai.Bone();
                            var boneNode = model.GetNode( nodeIndex );

                            aiBone.Name = AssimpConverterCommon.EscapeName( boneNode.Name );
                            aiBone.VertexWeights.Add( new Ai.VertexWeight( i, boneWeight ) );

                            Matrix4x4.Invert( geometryNode.WorldTransform, out Matrix4x4 invGeometryNodeWorldTransform );
                            Matrix4x4.Invert( boneNode.WorldTransform * invGeometryNodeWorldTransform, out Matrix4x4 offsetMatrix );
                            aiBone.OffsetMatrix = new Ai.Matrix4x4( offsetMatrix.M11, offsetMatrix.M21, offsetMatrix.M31, offsetMatrix.M41,
                                                                    offsetMatrix.M12, offsetMatrix.M22, offsetMatrix.M32, offsetMatrix.M42,
                                                                    offsetMatrix.M13, offsetMatrix.M23, offsetMatrix.M33, offsetMatrix.M43,
                                                                    offsetMatrix.M14, offsetMatrix.M24, offsetMatrix.M34, offsetMatrix.M44 );
                            boneMap[nodeIndex] = aiBone;
                        }
                        else
                        {
                            boneMap[nodeIndex].VertexWeights.Add( new Ai.VertexWeight( i, boneWeight ) );
                        }
                    }
                }

                aiMesh.Bones.AddRange( boneMap.Values );
            }

            return aiMesh;
        }
    }
}
