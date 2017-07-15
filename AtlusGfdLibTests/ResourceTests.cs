using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtlusGfdLib.Tests
{
    [TestClass()]
    public class ResourceTests
    {
        public const string ModelPath = @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD";
        public const string ModelNewPath = @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00_new.GMD";
        public const string ShaderCachePS3Path = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3PRESET.GSC";
        public const string ShaderCachePS3NewPath = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3PRESET_new.GSC";
        public const string ShaderCache2PS3Path = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3.GSC";
        public const string ShaderCache2PS3NewPath = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3_new.GSC";
        public const string ShaderCachePSP2Path = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2PRESET.GSC";
        public const string ShaderCachePSP2NewPath = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2PRESET.GSC";
        public const string ShaderCache2PSP2Path = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2.GSC";
        public const string ShaderCache2PSP2NewPath = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2.GSC";

        [TestMethod()]
        public void LoadFromFile_Model_ShouldNotThrow()
        {
            var res = Resource.Load<Model>( ModelPath );
        }

        [TestMethod()]
        public void LoadFromStream_Model_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( ModelPath ) )
            {
                var res = Resource.Load<Model>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_ShaderCache_PS3_ShouldNotThrow()
        {
            var res = Resource.Load<ShaderCachePS3>( ShaderCachePS3Path );
        }

        [TestMethod()]
        public void LoadFromStream_ShaderCache_PS3_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( ShaderCachePS3Path ) )
            {
                var res = Resource.Load<ShaderCachePS3>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ModelPath ).Length;
            var model = Resource.Load<Model>( ModelPath );
            Resource.Save( model, ModelNewPath );
            var newSize = new FileInfo( ModelNewPath ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_PS3_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCachePS3Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePS3>( ShaderCachePS3Path );
            Resource.Save( shaderCache, ShaderCachePS3NewPath );
            var newSize = new FileInfo( ShaderCachePS3NewPath ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache2_PS3_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache2PS3Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePS3>( ShaderCache2PS3Path );
            Resource.Save( shaderCache, ShaderCache2PS3NewPath );
            var newSize = new FileInfo( ShaderCache2PS3NewPath ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_PSP2_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache2PSP2Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePSP2>( ShaderCache2PSP2Path );
            Resource.Save( shaderCache, ShaderCache2PSP2NewPath );
            var newSize = new FileInfo( ShaderCache2PSP2NewPath ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache2_PSP2_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache2PSP2Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePSP2>( ShaderCache2PSP2Path );
            Resource.Save( shaderCache, ShaderCache2PSP2NewPath );
            var newSize = new FileInfo( ShaderCache2PSP2NewPath ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_OutputShouldBeEqualToInput()
        {
            var originalModel = Resource.Load<Model>( ModelPath );
            Resource.Save( originalModel, ModelNewPath );
            var newModel = Resource.Load<Model>( ModelNewPath );

            CompareModels( originalModel, newModel );
        }

        private void CompareModels(Model a, Model b)
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.Version, b.Version );

            CompareTextureDictionaries( a.TextureDictionary, a.TextureDictionary );
            CompareMaterialDictionaries( a.MaterialDictionary, b.MaterialDictionary );
            CompareScenes( a.Scene, b.Scene );
            CompareAnimationPackage( a.AnimationPackage, b.AnimationPackage );
        }

        private void CompareTextureDictionaries( TextureDictionary a, TextureDictionary b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.Version, b.Version );
            Assert.AreEqual( a.Count, b.Count );

            var texturesSorted = a.Textures.OrderBy( x => x.Name ).ToList();
            var otherTexturesSorted = b.Textures.OrderBy( x => x.Name ).ToList();
            for ( int i = 0; i < a.Textures.Count; i++ )
            {
                CompareTextures( texturesSorted[i], otherTexturesSorted[i] );
            }
        }

        private void CompareTextures( Texture a, Texture b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.Format, b.Format );
            CollectionAssert.AreEqual( a.Data, b.Data );
            Assert.AreEqual( a.Field1C, b.Field1C );
            Assert.AreEqual( a.Field1D, b.Field1D );
            Assert.AreEqual( a.Field1E, b.Field1E);
            Assert.AreEqual( a.Field1F, b.Field1F );
        }

        private void CompareMaterialDictionaries( MaterialDictionary a, MaterialDictionary b)
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.Version, b.Version );
            
            var ordered = a.Materials.OrderBy( x => x.Name ).ToList();
            var otherOrdered = b.Materials.OrderBy( x => x.Name ).ToList();
            for ( int i = 0; i < a.Materials.Count; i++ )
            {
                CompareMaterial( ordered[i], otherOrdered[i] );
            }
        }

        private void CompareMaterial( Material a, Material b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.Flags, b.Flags );
            Assert.AreEqual( a.Ambient, b.Ambient );
            Assert.AreEqual( a.Diffuse, b.Diffuse );
            Assert.AreEqual( a.Specular, b.Specular );
            Assert.AreEqual( a.Emissive, b.Emissive );
            Assert.AreEqual( a.Field40, b.Field40 );
            Assert.AreEqual( a.Field44, b.Field44 );
            Assert.AreEqual( a.Field48, b.Field48 );
            Assert.AreEqual( a.Field49, b.Field49 );
            Assert.AreEqual( a.Field4A, b.Field4A );
            Assert.AreEqual( a.Field4B, b.Field4B );
            Assert.AreEqual( a.Field4C, b.Field4C );
            Assert.AreEqual( a.Field4D, b.Field4D );
            Assert.AreEqual( a.Field90, b.Field90 );
            Assert.AreEqual( a.Field92, b.Field92 );
            Assert.AreEqual( a.Field94, b.Field94 );
            Assert.AreEqual( a.Field96, b.Field96 );
            Assert.AreEqual( a.Field5C, b.Field5C );
            Assert.AreEqual( a.Field6C, b.Field6C );
            Assert.AreEqual( a.Field70, b.Field70 );
            Assert.AreEqual( a.Field50, b.Field50 );
            Assert.AreEqual( a.Field98, b.Field98 );

            CompareTextureMap( a.DiffuseMap, b.DiffuseMap );
            CompareTextureMap( a.NormalMap, b.NormalMap );
            CompareTextureMap( a.SpecularMap, b.SpecularMap );
            CompareTextureMap( a.ReflectionMap, b.ReflectionMap );
            CompareTextureMap( a.HighlightMap, b.HighlightMap );
            CompareTextureMap( a.GlowMap, b.GlowMap );
            CompareTextureMap( a.NightMap, b.NightMap );
            CompareTextureMap( a.DetailMap, b.DetailMap );
            CompareTextureMap( a.ShadowMap, b.ShadowMap );
            CompareMaterialProperties( a.Properties, b.Properties );
        }

        private void CompareTextureMap( TextureMap a, TextureMap b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.Field44, b.Field44 );
            Assert.AreEqual( a.Field48, b.Field48 );
            Assert.AreEqual( a.Field49, b.Field49 );
            Assert.AreEqual( a.Field4A, b.Field4A );
            Assert.AreEqual( a.Field4B, b.Field4B );
            Assert.AreEqual( a.Field4C, b.Field4C );
            Assert.AreEqual( a.Field50, b.Field50 );
            Assert.AreEqual( a.Field54, b.Field54 );
            Assert.AreEqual( a.Field58, b.Field58 );
            Assert.AreEqual( a.Field5C, b.Field5C );
            Assert.AreEqual( a.Field60, b.Field60 );
            Assert.AreEqual( a.Field64, b.Field64 );
            Assert.AreEqual( a.Field68, b.Field68 );
            Assert.AreEqual( a.Field6C, b.Field6C );
            Assert.AreEqual( a.Field70, b.Field70 );
            Assert.AreEqual( a.Field74, b.Field74 );
            Assert.AreEqual( a.Field78, b.Field78 );
            Assert.AreEqual( a.Field7C, b.Field7C );
            Assert.AreEqual( a.Field80, b.Field80 );
            Assert.AreEqual( a.Field84, b.Field84 );
            Assert.AreEqual( a.Field88, b.Field88 );

        }

        private void CompareMaterialProperties( List<MaterialProperty> a, List<MaterialProperty> b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Count, b.Count );
            for ( int i = 0; i < a.Count; i++ )
            {
                Assert.AreEqual( a[i].RawFlags, b[i].RawFlags );

                switch ( a[i].Type )
                {
                    case MaterialPropertyType.Type0:
                        {
                            var a2 = ( MaterialPropertyType0 )a[i];
                            var b2 = ( MaterialPropertyType0 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field28, b2.Field28 );
                            Assert.AreEqual( a2.Field2C, b2.Field2C );
                            Assert.AreEqual( a2.Type0Flags, b2.Type0Flags );
                        }
                        break;
                    case MaterialPropertyType.Type1:
                        {
                            var a2 = ( MaterialPropertyType1 )a[i];
                            var b2 = ( MaterialPropertyType1 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field34, b2.Field34 );
                            Assert.AreEqual( a2.Field38, b2.Field38 );
                            Assert.AreEqual( a2.Type1Flags, b2.Type1Flags );
                        }
                        break;
                    case MaterialPropertyType.Type2:
                        {
                            var a2 = ( MaterialPropertyType2 )a[i];
                            var b2 = ( MaterialPropertyType2 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                        }
                        break;
                    case MaterialPropertyType.Type3:
                        {
                            var a2 = ( MaterialPropertyType3 )a[i];
                            var b2 = ( MaterialPropertyType3 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                            Assert.AreEqual( a2.Field14, b2.Field14 );
                            Assert.AreEqual( a2.Field18, b2.Field18 );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field28, b2.Field28 );
                            Assert.AreEqual( a2.Field2C, b2.Field2C );
                            Assert.AreEqual( a2.Field30, b2.Field30 );
                            Assert.AreEqual( a2.Field34, b2.Field34 );
                            Assert.AreEqual( a2.Field38, b2.Field38 );
                            Assert.AreEqual( a2.Field3C, b2.Field3C );
                        }
                        break;
                    case MaterialPropertyType.Type4:
                        {
                            var a2 = ( MaterialPropertyType4 )a[i];
                            var b2 = ( MaterialPropertyType4 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field34, b2.Field34 );
                            Assert.AreEqual( a2.Field38, b2.Field38 );
                            Assert.AreEqual( a2.Field3C, b2.Field3C );
                            Assert.AreEqual( a2.Field40, b2.Field40 );
                            Assert.AreEqual( a2.Field44, b2.Field44 );
                            Assert.AreEqual( a2.Field48, b2.Field48 );
                            Assert.AreEqual( a2.Field4C, b2.Field4C );
                            Assert.AreEqual( a2.Field50, b2.Field50 );
                            Assert.AreEqual( a2.Field54, b2.Field54 );
                            Assert.AreEqual( a2.Field58, b2.Field58 );
                            Assert.AreEqual( a2.Field5C, b2.Field5C );
                        }
                        break;
                    case MaterialPropertyType.Type5:
                        {
                            var a2 = ( MaterialPropertyType5 )a[i];
                            var b2 = ( MaterialPropertyType5 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                            Assert.AreEqual( a2.Field14, b2.Field14 );
                            Assert.AreEqual( a2.Field18, b2.Field18 );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field2C, b2.Field2C );
                            Assert.AreEqual( a2.Field30, b2.Field30 );
                            Assert.AreEqual( a2.Field34, b2.Field34 );
                            Assert.AreEqual( a2.Field38, b2.Field38 );
                            Assert.AreEqual( a2.Field3C, b2.Field3C );
                            Assert.AreEqual( a2.Field40, b2.Field40 );
                            Assert.AreEqual( a2.Field48, b2.Field48 );
                        }
                        break;
                    case MaterialPropertyType.Type6:
                        {
                            var a2 = ( MaterialPropertyType6 )a[i];
                            var b2 = ( MaterialPropertyType6 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                            Assert.AreEqual( a2.Field14, b2.Field14 );
                        }
                        break;
                    case MaterialPropertyType.Type7:
                        // no fields
                        break;
                    default:
                        break;
                }
            }
        }

        private void CompareScenes( Scene a, Scene b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.Version, b.Version );

            Assert.AreEqual( a.Flags, b.Flags );
            CompareBoneMap( a.MatrixMap, b.MatrixMap );
            Assert.AreEqual( a.BoundingBox, b.BoundingBox );
            Assert.AreEqual( a.BoundingSphere, b.BoundingSphere );
            CompareNodes( a.RootNode, b.RootNode );
        }

        private void CompareBoneMap( SkinnedBoneMap a, SkinnedBoneMap b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.MatrixCount, b.MatrixCount );

            if ( a.BoneInverseBindMatrices == null || b.BoneInverseBindMatrices == null )
            {
                Assert.IsTrue( a.BoneInverseBindMatrices == null ? ( b.BoneInverseBindMatrices == null ) : ( b.BoneInverseBindMatrices != null ) );
                return;
            }

            if( a.BoneToNodeIndices == null || b.BoneToNodeIndices == null )
            {
                Assert.IsTrue( a.BoneToNodeIndices == null ? ( b.BoneToNodeIndices == null ) : ( b.BoneToNodeIndices != null ) );
                return;
            }

            for ( int i = 0; i < a.MatrixCount; i++ )
            {
                Assert.AreEqual( a.BoneInverseBindMatrices[i], b.BoneInverseBindMatrices[i] );
                Assert.AreEqual( a.BoneToNodeIndices[i], b.BoneToNodeIndices[i] );
            }
        }

        private void CompareNodes( Node a, Node b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Translation, b.Translation );
            Assert.AreEqual( a.Rotation, b.Rotation );
            Assert.AreEqual( a.Scale, b.Scale );
            Assert.AreEqual( a.AttachmentCount, b.AttachmentCount );
            Assert.AreEqual( a.HasAttachments, b.HasAttachments );

            if ( a.HasAttachments )
            {
                for ( int i = 0; i < a.AttachmentCount; i++ )
                {
                    CompareAttachment( a.Attachments[i], b.Attachments[i] );
                }
            }

            Assert.AreEqual( a.PropertyCount, b.PropertyCount );
            Assert.AreEqual( a.HasProperties, b.HasProperties );

            if ( a.HasProperties )
            {
                var sortedProperties = a.Properties.Values.OrderBy( x => x.Name ).ToList();
                var otherSortedProperties = b.Properties.Values.OrderBy( x => x.Name ).ToList();
                for ( int i = 0; i < a.PropertyCount; i++ )
                {
                    CompareNodeProperty( sortedProperties[i], otherSortedProperties[i] );
                }
            }

            Assert.AreEqual( a.FieldE0, b.FieldE0 );
            Assert.AreEqual( a.HasParent, b.HasParent );

            if ( a.HasParent )
            {
                Assert.AreEqual( a.Parent.Name, b.Parent.Name );
            }

            Assert.AreEqual( a.ChildCount, b.ChildCount );
            Assert.AreEqual( a.HasChildren, b.HasChildren );

            if ( a.HasChildren )
            {
                for ( int i = 0; i < a.ChildCount; i++ )
                {
                    CompareNodes( a.Children[i], b.Children[i] );
                }
            }
        }

        private void CompareNodeProperty( NodeProperty a, NodeProperty b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.ValueType, b.ValueType );
            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.GetValue(), b.GetValue() );
        }

        private void CompareAttachment( NodeAttachment a, NodeAttachment b )
        {
            Assert.AreEqual( a.Type, b.Type );

            switch ( a.Type )
            {
                case NodeAttachmentType.Invalid:
                case NodeAttachmentType.Scene:
                case NodeAttachmentType.Mesh:
                    throw new InvalidDataException();

                case NodeAttachmentType.Node:
                    CompareNodes( a.GetValue<Node>(), b.GetValue<Node>() );
                    break;
                case NodeAttachmentType.Geometry:
                    CompareGeometries( a.GetValue<Geometry>(), b.GetValue<Geometry>() );
                    break;
                case NodeAttachmentType.Camera:
                    CompareCameras( a.GetValue<Camera>(), b.GetValue<Camera>() );
                    break;
                case NodeAttachmentType.Light:
                    CompareLights( a.GetValue<Light>(), b.GetValue<Light>() );
                    break;
                case NodeAttachmentType.Epl:
                    CompareEpls( a.GetValue<Epl>(), b.GetValue<Epl>() );
                    break;
                case NodeAttachmentType.EplLeaf:
                    CompareEplLeafs( a.GetValue<EplLeaf>(), b.GetValue<EplLeaf>() );
                    break;
                case NodeAttachmentType.Morph:
                    CompareMorphs( a.GetValue<Morph>(), b.GetValue<Morph>() );
                    break;
                default:
                    break;
            }
        }

        private void CompareMorphs( Morph a, Morph b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            throw new NotImplementedException();
        }

        private void CompareEplLeafs( EplLeaf a, EplLeaf b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            throw new NotImplementedException();
        }

        private void CompareEpls( Epl a, Epl b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            throw new NotImplementedException();
        }

        private void CompareLights( Light a, Light b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            throw new NotImplementedException();
        }

        private void CompareCameras( Camera a, Camera b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            throw new NotImplementedException();
        }

        private void CompareGeometries( Geometry a, Geometry b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Flags, b.Flags );
            Assert.AreEqual( a.VertexAttributeFlags, b.VertexAttributeFlags );
            Assert.AreEqual( a.TriangleCount, b.TriangleCount );
            Assert.AreEqual( a.TriangleIndexType, b.TriangleIndexType );
            CollectionAssert.AreEqual( a.Triangles, b.Triangles );

            Assert.AreEqual( a.VertexCount, b.VertexCount );
            Assert.AreEqual( a.Field14, b.Field14 );
            CollectionAssert.AreEqual( a.Vertices, b.Vertices );
            CollectionAssert.AreEqual( a.Normals, b.Normals );
            CollectionAssert.AreEqual( a.Tangents, b.Tangents );
            CollectionAssert.AreEqual( a.Binormals, b.Binormals );
            CollectionAssert.AreEqual( a.ColorChannel0, b.ColorChannel0 );
            CollectionAssert.AreEqual( a.TexCoordsChannel0, b.TexCoordsChannel0 );
            CollectionAssert.AreEqual( a.TexCoordsChannel1, b.TexCoordsChannel1 );
            CollectionAssert.AreEqual( a.TexCoordsChannel2, b.TexCoordsChannel2 );
            CollectionAssert.AreEqual( a.ColorChannel1, b.ColorChannel1 );
            CollectionAssert.AreEqual( a.VertexWeights, b.VertexWeights );

            Assert.AreEqual( a.MaterialName, b.MaterialName );
            Assert.AreEqual( a.BoundingBox, b.BoundingBox );
            Assert.AreEqual( a.BoundingSphere, b.BoundingSphere );
            Assert.AreEqual( a.FieldD4, b.FieldD4 );
            Assert.AreEqual( a.FieldD8, b.FieldD8 );
        }

        private void CompareAnimationPackage( AnimationPackage a, AnimationPackage b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.Version, b.Version );

            throw new NotImplementedException();
        }
    }
}