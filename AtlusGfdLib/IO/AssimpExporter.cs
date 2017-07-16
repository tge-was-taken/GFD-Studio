using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib.IO
{
    public class AssimpExporter
    {
        private Assimp.Scene mAiScene;

        public static void ExportToFile( Model model, string path )
        {
            var exporter = new AssimpExporter();
            var scene = exporter.ConvertModel( model );
            var aiContext = new Assimp.AssimpContext();
            aiContext.ExportFile( scene, path, "collada" );
        }

        private Assimp.Scene ConvertModel( Model model )
        {
            mAiScene = new Assimp.Scene();

            //if ( model.TextureDictionary != null )
            //    ConvertTextures( model.TextureDictionary );

            if ( model.MaterialDictionary != null )
                ConvertMaterials( model.MaterialDictionary );

            if ( model.Scene != null )
                ConvertScene( model.Scene );

            return mAiScene;
        }

        private void ConvertTextures( TextureDictionary textureDictionary )
        {
            foreach ( var texture in textureDictionary.Textures )
            {
                var aiTexture = new Assimp.EmbeddedTexture( "dds", texture.Data );
                mAiScene.Textures.Add( aiTexture );
            }
        }

        private void ConvertMaterials( MaterialDictionary materialDictionary )
        {
            foreach ( var material in materialDictionary.Materials )
            {
                mAiScene.Materials.Add( ConvertMaterial( material ) );
            }
        }

        private Assimp.Material ConvertMaterial( Material material )
        {
            var aiMaterial = new Assimp.Material();
            aiMaterial.Name = material.Name;
            aiMaterial.ColorAmbient = new Assimp.Color4D( material.Ambient.X, material.Ambient.Y, material.Ambient.Z, material.Ambient.W );
            aiMaterial.ColorDiffuse = new Assimp.Color4D( material.Diffuse.X, material.Diffuse.Y, material.Diffuse.Z, material.Diffuse.W );
            aiMaterial.ColorSpecular = new Assimp.Color4D( material.Specular.X, material.Specular.Y, material.Specular.Z, material.Specular.W );
            aiMaterial.ColorEmissive = new Assimp.Color4D( material.Emissive.X, material.Emissive.Y, material.Emissive.Z, material.Emissive.W );
            
            if ( material.Flags.HasFlag( MaterialFlags.HasDiffuseMap ))
            {
                aiMaterial.TextureDiffuse = new Assimp.TextureSlot( material.DiffuseMap.Name, Assimp.TextureType.Diffuse, 0, Assimp.TextureMapping.FromUV, 0, 0, Assimp.TextureOperation.Add, Assimp.TextureWrapMode.Wrap, Assimp.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                aiMaterial.TextureNormal = new Assimp.TextureSlot( material.NormalMap.Name, Assimp.TextureType.Normals, 1, Assimp.TextureMapping.FromUV, 0, 0, Assimp.TextureOperation.Add, Assimp.TextureWrapMode.Wrap, Assimp.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                aiMaterial.TextureSpecular = new Assimp.TextureSlot( material.SpecularMap.Name, Assimp.TextureType.Specular, 2, Assimp.TextureMapping.FromUV, 0, 0, Assimp.TextureOperation.Add, Assimp.TextureWrapMode.Wrap, Assimp.TextureWrapMode.Wrap, 0 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                aiMaterial.TextureReflection = new Assimp.TextureSlot( material.ReflectionMap.Name, Assimp.TextureType.Reflection, 3, Assimp.TextureMapping.FromUV, 0, 0, Assimp.TextureOperation.Add, Assimp.TextureWrapMode.Wrap, Assimp.TextureWrapMode.Wrap, 0 );
            }

            // todo: add more textures

            return aiMaterial;
        }

        private void ConvertScene( Scene scene )
        {
            mAiScene.RootNode = ConvertNode( scene, scene.RootNode, null );
        }

        private Assimp.Node ConvertNode( Scene scene, Node node, Assimp.Node aiParent )
        {
            var aiNode = new Assimp.Node( node.Name, aiParent );
            aiNode.Transform = new Assimp.Matrix4x4( node.LocalTransform.M11, node.LocalTransform.M21, node.LocalTransform.M31, node.LocalTransform.M41,
                                                     node.LocalTransform.M12, node.LocalTransform.M22, node.LocalTransform.M32, node.LocalTransform.M42,
                                                     node.LocalTransform.M13, node.LocalTransform.M23, node.LocalTransform.M33, node.LocalTransform.M43,
                                                     node.LocalTransform.M14, node.LocalTransform.M24, node.LocalTransform.M34, node.LocalTransform.M44 );
            
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

        private static void ConvertNodeProperties( Dictionary<string, NodeProperty> properties, Assimp.Node aiNode )
        {
            foreach ( var property in properties.Values )
            {
                Assimp.MetaDataType dataType;
                object data = property.GetValue();

                switch ( property.ValueType )
                {
                    case PropertyValueType.Int:
                        dataType = Assimp.MetaDataType.Int;
                        break;
                    case PropertyValueType.Float:
                        dataType = Assimp.MetaDataType.Float;
                        break;
                    case PropertyValueType.Bool:
                        dataType = Assimp.MetaDataType.Bool;
                        break;
                    case PropertyValueType.String:
                        dataType = Assimp.MetaDataType.String;
                        break;
                    case PropertyValueType.ByteVector3:
                        dataType = Assimp.MetaDataType.Vector3D;
                        break;
                    case PropertyValueType.ByteVector4:
                        dataType = Assimp.MetaDataType.String;
                        data = property.GetValue().ToString();
                        break;
                    case PropertyValueType.Vector3:
                        dataType = Assimp.MetaDataType.Vector3D;
                        break;
                    case PropertyValueType.Vector4:
                        dataType = Assimp.MetaDataType.String;
                        data = property.GetValue().ToString();
                        break;
                    case PropertyValueType.ByteArray:
                        dataType = Assimp.MetaDataType.String;
                        data = property.GetValue().ToString(); // todo: fix
                        break;
                    default:
                        throw new Exception( "Unknown property value type" );
                }

                aiNode.Metadata[property.Name] = new Assimp.Metadata.Entry( dataType, data );
            }
        }

        private void ConvertNodeAttachments( Scene scene, Node node, Assimp.Node aiNode )
        {
            for (int i = 0; i < node.Attachments.Count; i++)        
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

        private Assimp.Mesh ConvertGeometry( Scene scene, Node geometryNode, Geometry geometry)
        {
            var aiMesh = new Assimp.Mesh( Assimp.PrimitiveType.Triangle );

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMaterial ))
                aiMesh.MaterialIndex = mAiScene.Materials.FindIndex( x => x.Name == geometry.MaterialName );

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) ) 
            {
                foreach ( var triangle in geometry.Triangles )
                {
                    var aiFace = new Assimp.Face( triangle.Indices );
                    aiMesh.Faces.Add( aiFace );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ))
            {
                foreach ( var vertex in geometry.Vertices )
                {
                    aiMesh.Vertices.Add( new Assimp.Vector3D( vertex.X, vertex.Y, vertex.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
            {
                foreach ( var normal in geometry.Normals )
                {
                    aiMesh.Normals.Add( new Assimp.Vector3D( normal.X, normal.Y, normal.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
            {
                foreach ( var tangent in geometry.Tangents )
                {
                    aiMesh.Tangents.Add( new Assimp.Vector3D( tangent.X, tangent.Y, tangent.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
            {
                foreach ( var binormal in geometry.Binormals )
                {
                    aiMesh.BiTangents.Add( new Assimp.Vector3D( binormal.X, binormal.Y, binormal.Z ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel0 )
                {
                    aiMesh.TextureCoordinateChannels[0].Add( new Assimp.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel1 )
                {
                    aiMesh.TextureCoordinateChannels[1].Add( new Assimp.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
            {
                foreach ( var vertex in geometry.TexCoordsChannel2 )
                {
                    aiMesh.TextureCoordinateChannels[2].Add( new Assimp.Vector3D( vertex.X, vertex.Y, 0 ) );
                }
            }

            /* todo: colors
            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
            {
                foreach ( var color in geometry.ColorChannel0 )
                {
                    aiMesh.VertexColorChannels[0].Add( new Assimp.Color4D( color. ))
                }
            }
            */

            //if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ))
            //{
            //    var aiBoneMap = new Dictionary<int, Assimp.Bone>();

            //    for ( int i = 0; i < geometry.VertexWeights.Length; i++ )
            //    {
            //        var vertexWeight = geometry.VertexWeights[i];

            //        for ( int j = 0; j < 4; j++ )
            //        {
            //            var boneWeight = vertexWeight.Weights[j];

            //            if ( boneWeight == 0.0f )
            //                continue;

            //            var nodeIndex = scene.MatrixMap.BoneToNodeIndices[vertexWeight.Indices[j]];

            //            if ( !aiBoneMap.ContainsKey( nodeIndex ) )
            //            {
            //                var aiBone = new Assimp.Bone();
            //                var boneNode = scene.Nodes[nodeIndex];

            //                aiBone.Name = boneNode.Name;
            //                aiBone.VertexWeights.Add( new Assimp.VertexWeight( i, boneWeight ) );

            //                Matrix4x4.Invert( geometryNode.WorldTransform, out Matrix4x4 invGeometryNodeWorldTransform );
            //                Matrix4x4.Invert( boneNode.WorldTransform * invGeometryNodeWorldTransform, out Matrix4x4 offsetMatrix );
            //                aiBone.OffsetMatrix = new Assimp.Matrix4x4( offsetMatrix.M11, offsetMatrix.M21, offsetMatrix.M31, offsetMatrix.M41,
            //                                                            offsetMatrix.M12, offsetMatrix.M22, offsetMatrix.M32, offsetMatrix.M42,
            //                                                            offsetMatrix.M13, offsetMatrix.M23, offsetMatrix.M33, offsetMatrix.M43,
            //                                                            offsetMatrix.M14, offsetMatrix.M24, offsetMatrix.M34, offsetMatrix.M44 );
            //                aiBoneMap[nodeIndex] = aiBone;
            //            }
            //            else
            //            {
            //                aiBoneMap[nodeIndex].VertexWeights.Add( new Assimp.VertexWeight( i, boneWeight ) );
            //            }
            //        }
            //    }

            //    aiMesh.Bones.AddRange( aiBoneMap.Values );
            //}

            return aiMesh;
        }

        private static Assimp.Matrix4x4 GetWorldTransform( Assimp.Node node )
        {
            var matrix = node.Transform;
            if ( node.Parent != null )
                matrix *= GetWorldTransform( node.Parent );

            return matrix;
        }
    }
}
