using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ai = Assimp;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace AtlusGfdLib.Assimp
{
    public class AssimpExporter
    {
        private Ai.Scene mAiScene;
        private string mPath;
        private string mFileName;
        private string mBaseDirectoryPath;
        private string mTextureBaseDirectoryPath;
        private string mTextureBaseRelativeDirectoryPath;

        public static void ExportFile( Model model, string path )
        {
            var exporter = new AssimpExporter();
            var scene = exporter.ConvertModel( model, path );
            var aiContext = new Ai.AssimpContext();
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

        private Ai.Scene ConvertModel( Model model, string path )
        {
            Init( path );

            if ( model.TextureDictionary != null )
                ConvertTextures( model.TextureDictionary );

            if ( model.MaterialDictionary != null )
                ConvertMaterials( model.MaterialDictionary );

            if ( model.Scene != null )
                ConvertScene( model.Scene );

            return mAiScene;
        }

        private void ConvertTextures( TextureDictionary textureDictionary )
        {
            Directory.CreateDirectory( mTextureBaseDirectoryPath );

            foreach ( var texture in textureDictionary.Textures )
            {
                var texturePath = Path.Combine( mTextureBaseDirectoryPath, texture.Name );

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

        private Ai.Material ConvertMaterial( Material material )
        {
            var aiMaterial = new Ai.Material
            {
                Name = material.Name,
                ColorAmbient = new Ai.Color4D( material.Ambient.X, material.Ambient.Y, material.Ambient.Z, material.Ambient.W ),
                ColorDiffuse = new Ai.Color4D( material.Diffuse.X, material.Diffuse.Y, material.Diffuse.Z, material.Diffuse.W ),
                ColorSpecular = new Ai.Color4D( material.Specular.X, material.Specular.Y, material.Specular.Z, material.Specular.W ),
                ColorEmissive = new Ai.Color4D( material.Emissive.X, material.Emissive.Y, material.Emissive.Z, material.Emissive.W )
            };

            if ( material.Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                aiMaterial.TextureDiffuse = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, material.DiffuseMap.Name ),
                    Ai.TextureType.Diffuse, 0, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                aiMaterial.TextureNormal = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, material.NormalMap.Name ), 
                    Ai.TextureType.Normals, 1, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                aiMaterial.TextureSpecular = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, material.SpecularMap.Name ), 
                    Ai.TextureType.Specular, 2, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                aiMaterial.TextureReflection = new Ai.TextureSlot( 
                    Path.Combine( mTextureBaseRelativeDirectoryPath, material.ReflectionMap.Name ), 
                    Ai.TextureType.Reflection, 3, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            // todo: add more textures

            return aiMaterial;
        }

        private void ConvertScene( Scene scene )
        {
            mAiScene.RootNode = ConvertNode( scene, scene.RootNode, null );
        }

        private string EscapeNodeName( string nodeName )
        {
            // damn assimp bug
            return nodeName.Replace( " ", "__" );
        }

        private Ai.Node ConvertNode( Scene scene, Node node, Ai.Node aiParent )
        {
            var aiNode = new Ai.Node( EscapeNodeName( node.Name ), aiParent )
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
                ConvertNodeAttachments( scene, node, aiNode );
            }

            if ( node.HasChildren )
            {
                foreach ( var childNode in node.Children )
                {
                    aiNode.Children.Add( ConvertNode( scene, childNode, aiNode ) );
                }
            }

            return aiNode;
        }

        private static void ConvertNodeProperties( Dictionary<string, NodeProperty> properties, Ai.Node aiNode )
        {
            var stringBuilder = new StringBuilder();

            foreach ( var property in properties.Values )
            {
                // Todo: Assign to 3ds max user properties
                stringBuilder.Append( $"{property.ToUserPropertyString()}&cr;&lf;" );
            }

            aiNode.Metadata["UDP3DSMAX"] = new Ai.Metadata.Entry( Ai.MetaDataType.String, stringBuilder.ToString() );
        }

        private void ConvertNodeAttachments( Scene scene, Node node, Ai.Node aiNode )
        {
            for ( int i = 0; i < node.Attachments.Count; i++ )
            {
                var attachment = node.Attachments[i];

                switch ( attachment.Type )
                {
                    case NodeAttachmentType.Geometry:
                        {
                            var mesh = ConvertGeometry( scene, node, attachment.GetValue<Geometry>() );

                            mesh.Name = $"{node.Name}_Attachment{i}_Geometry";
                            aiNode.MeshIndices.Add( mAiScene.Meshes.Count );
                            mAiScene.Meshes.Add( mesh );
                        }
                        break;
                    default:
                        //throw new NotImplementedException();
                        break;
                }
            }
        }

        private Ai.Mesh ConvertGeometry( Scene scene, Node geometryNode, Geometry geometry )
        {
            var aiMesh = new Ai.Mesh( Ai.PrimitiveType.Triangle );

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMaterial ) )
                aiMesh.MaterialIndex = mAiScene.Materials.FindIndex( x => x.Name == geometry.MaterialName );

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                foreach ( var triangle in geometry.Triangles )
                {
                    var aiFace = new Ai.Face();
                    aiFace.Indices.Add( ( int )triangle.A );
                    aiFace.Indices.Add( ( int )triangle.B );
                    aiFace.Indices.Add( ( int )triangle.C );
                    aiMesh.Faces.Add( aiFace );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                foreach ( var vertex in geometry.Vertices )
                {
                    aiMesh.Vertices.Add( new Ai.Vector3D( vertex.X, vertex.Y, vertex.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
            {
                foreach ( var normal in geometry.Normals )
                {
                    aiMesh.Normals.Add( new Ai.Vector3D( normal.X, normal.Y, normal.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
            {
                foreach ( var tangent in geometry.Tangents )
                {
                    aiMesh.Tangents.Add( new Ai.Vector3D( tangent.X, tangent.Y, tangent.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
            {
                foreach ( var binormal in geometry.Binormals )
                {
                    aiMesh.BiTangents.Add( new Ai.Vector3D( binormal.X, binormal.Y, binormal.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel0 )
                {
                    aiMesh.TextureCoordinateChannels[0].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel1 )
                {
                    aiMesh.TextureCoordinateChannels[1].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel2 )
                {
                    aiMesh.TextureCoordinateChannels[2].Add( new Ai.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            /* todo: colors
            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
            {
                foreach ( var color in geometry.ColorChannel0 )
                {
                    aiMesh.VertexColorChannels[0].Add( new Ai.Color4D( color. ))
                }
            }
            */

            if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
            {
                var boneMap = new Dictionary<int, Ai.Bone>();

                for ( int i = 0; i < geometry.VertexWeights.Length; i++ )
                {
                    var vertexWeight = geometry.VertexWeights[i];

                    for ( int j = 0; j < 4; j++ )
                    {
                        var boneWeight = vertexWeight.Weights[j];
                        if ( boneWeight == 0f )
                            continue;

                        var boneIndex = vertexWeight.Indices[j];
                        var nodeIndex = scene.MatrixPalette.BoneToNodeIndices[boneIndex];

                        if ( !boneMap.ContainsKey( nodeIndex ) )
                        {
                            var aiBone = new Ai.Bone();
                            var boneNode = scene.Nodes[nodeIndex];

                            aiBone.Name = EscapeNodeName( boneNode.Name );
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
