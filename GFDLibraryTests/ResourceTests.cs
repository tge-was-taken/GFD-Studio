using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Effects;
using GFDLibrary.Lights;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Shaders;
using GFDLibrary.Textures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    [TestClass()]
    public class ResourceTests
    {
        public const string Model_P5PlayerModel_Path          = @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD";
        public const string Model_P4DFaceModel_Path           = @"D:\Modding\Persona 4 Dancing CPK RIP\data\dance\player\pc001_f1.GMD";
        public const string Model_P5FieldLevelModel_Path      = @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\field_tex\f013_014_0.GFS";
        public const string ShaderCache_GFDPS3PRESET_Path     = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3PRESET.GSC";
        public const string ShaderCache_GFDPS3_Path           = @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\GFDPS3.GSC";
        public const string ShaderCache_GFDPSP2PRESET_Path    = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2PRESET.GSC";
        public const string ShaderCache_GFDPSP2_Path          = @"D:\Modding\Persona 4 Dancing CPK RIP\data\GFDPSP2.GSC";

        private string GetNewPath( string path )
        {
            return Path.Combine( Path.GetDirectoryName( path ), Path.GetFileNameWithoutExtension( path ) + "_new" + Path.GetExtension( path ) );
        }

        // Model tests ( Persona 5 protagonist battle model )

        [TestMethod()]
        public void LoadFromStream_Model_P5PlayerModel_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( Model_P5PlayerModel_Path ) )
            {
                var res = Resource.Load<ModelPack>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_Model_P5PlayerModel_ShouldNotThrow()
        {
            var res = Resource.Load<ModelPack>( Model_P5PlayerModel_Path );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P5PlayerModel_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( Model_P5PlayerModel_Path ).Length;
            var model = Resource.Load<ModelPack>( Model_P5PlayerModel_Path );
            model.Save( GetNewPath( Model_P5PlayerModel_Path ) );
            var newSize = new FileInfo( GetNewPath( Model_P5PlayerModel_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P5PlayerModel_OutputShouldBeEqualToInput()
        {
            var originalModel = Resource.Load<ModelPack>( Model_P5PlayerModel_Path );
            originalModel.Save( GetNewPath( Model_P5PlayerModel_Path ) );
            var newModel = Resource.Load<ModelPack>( GetNewPath( Model_P5PlayerModel_Path ) );

            CompareModels( originalModel, newModel );
        }

        // Model tests ( Persona 4 DAN Face model w/ morphers )

        [TestMethod()]
        public void LoadFromStream_Model_P4DMorphFaceModel_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( Model_P4DFaceModel_Path ) )
            {
                var res = Resource.Load<ModelPack>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_Model_P4DMorphFaceModel_ShouldNotThrow()
        {
            var res = Resource.Load<ModelPack>( Model_P4DFaceModel_Path );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P4DMorphFaceModel_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( Model_P4DFaceModel_Path ).Length;
            var model = Resource.Load<ModelPack>( Model_P4DFaceModel_Path );
            model.Save( GetNewPath( Model_P4DFaceModel_Path ) );
            var newSize = new FileInfo( GetNewPath( Model_P4DFaceModel_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P4DMorphFaceModel_OutputShouldBeEqualToInput()
        {
            var originalModel = Resource.Load<ModelPack>( Model_P4DFaceModel_Path );
            originalModel.Save( GetNewPath( Model_P4DFaceModel_Path ) );
            var newModel = Resource.Load<ModelPack>( GetNewPath( Model_P4DFaceModel_Path ) );

            CompareModels( originalModel, newModel );
        }

        // Model tests ( Persona 5 field level model )

        [TestMethod()]
        public void LoadFromStream_Model_P5FieldLevelModel_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( Model_P5FieldLevelModel_Path ) )
            {
                var res = Resource.Load<ModelPack>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_Model_P5FieldLevelModel_ShouldNotThrow()
        {
            var res = Resource.Load<ModelPack>( Model_P5FieldLevelModel_Path );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P5FieldLevelModel_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( Model_P5FieldLevelModel_Path ).Length;
            var model = Resource.Load<ModelPack>( Model_P5FieldLevelModel_Path );
            model.Save( GetNewPath( Model_P5FieldLevelModel_Path ) );
            var newSize = new FileInfo( GetNewPath( Model_P5FieldLevelModel_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_Model_P5FieldLevelModel_OutputShouldBeEqualToInput()
        {
            var originalModel = Resource.Load<ModelPack>( Model_P5FieldLevelModel_Path );
            originalModel.Save( GetNewPath( Model_P5FieldLevelModel_Path ) );
            var newModel = Resource.Load<ModelPack>( GetNewPath( Model_P5FieldLevelModel_Path ) );

            CompareModels( originalModel, newModel );
        }


        // Shader cache tests ( Persona 5 PS3 )

        [TestMethod()]
        public void LoadFromStream_ShaderCache_GFDPS3PRESET_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( ShaderCache_GFDPS3PRESET_Path ) )
            {
                var res = Resource.Load<ShaderCachePS3>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_ShaderCache_GFDPS3PRESET_ShouldNotThrow()
        {
            var res = Resource.Load<ShaderCachePS3>( ShaderCache_GFDPS3PRESET_Path );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_GFDPS3PRESET_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache_GFDPS3PRESET_Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePS3>( ShaderCache_GFDPS3PRESET_Path );
            shaderCache.Save( GetNewPath( ShaderCache_GFDPS3PRESET_Path ) );
            var newSize = new FileInfo( GetNewPath( ShaderCache_GFDPS3PRESET_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        // Shader cache ( empty ) tests ( Persona 5 PS3 )

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_GFDPS3_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache_GFDPS3_Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePS3>( ShaderCache_GFDPS3_Path );
            shaderCache.Save( GetNewPath( ShaderCache_GFDPS3_Path ) );
            var newSize = new FileInfo( GetNewPath( ShaderCache_GFDPS3_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        // Shader cache test ( Persona 4 DAN )


        [TestMethod()]
        public void LoadFromStream_ShaderCache_GFDPSP2PRESET_ShouldNotThrow()
        {
            using ( var fileStream = File.OpenRead( ShaderCache_GFDPSP2PRESET_Path ) )
            {
                var res = Resource.Load<ShaderCachePSP2>( fileStream );
            }
        }

        [TestMethod()]
        public void LoadFromFile_ShaderCache_GFDPSP2PRESET_ShouldNotThrow()
        {
            var res = Resource.Load<ShaderCachePSP2>( ShaderCache_GFDPSP2PRESET_Path );
        }

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_GFDPSP2PRESET_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache_GFDPSP2PRESET_Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePSP2>( ShaderCache_GFDPSP2PRESET_Path );
            shaderCache.Save( GetNewPath( ShaderCache_GFDPSP2PRESET_Path ) );
            var newSize = new FileInfo( GetNewPath( ShaderCache_GFDPSP2PRESET_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        // Shader cache ( empty ) tests ( Persona 4 DAN ) 

        [TestMethod()]
        public void LoadAndSaveToFile_ShaderCache_GFDPSP2_OutputSizeShouldBeEqualToInputSize()
        {
            var originalSize = new FileInfo( ShaderCache_GFDPSP2_Path ).Length;
            var shaderCache = Resource.Load<ShaderCachePSP2>( ShaderCache_GFDPSP2_Path );
            shaderCache.Save( GetNewPath( ShaderCache_GFDPSP2_Path ) );
            var newSize = new FileInfo( GetNewPath( ShaderCache_GFDPSP2_Path ) ).Length;

            Assert.AreEqual( originalSize, newSize );
        }

        // Comparison methods

        private void CompareModels(ModelPack a, ModelPack b)
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.ResourceType, b.ResourceType );
            Assert.AreEqual( a.Version, b.Version );

            CompareTextureDictionaries( a.Textures, a.Textures );
            CompareMaterialDictionaries( a.Materials, b.Materials );
            CompareScenes( a.Model, b.Model );
            CompareAnimationPackage( a.AnimationPack, b.AnimationPack );
        }

        private void CompareTextureDictionaries( TextureDictionary a, TextureDictionary b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.ResourceType, b.ResourceType );
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
            Assert.AreEqual( a.ResourceType, b.ResourceType );
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
            Assert.AreEqual( a.AmbientColor, b.AmbientColor );
            Assert.AreEqual( a.DiffuseColor, b.DiffuseColor );
            Assert.AreEqual( a.SpecularColor, b.SpecularColor );
            Assert.AreEqual( a.EmissiveColor, b.EmissiveColor );
            Assert.AreEqual( a.Field40, b.Field40 );
            Assert.AreEqual( a.Field44, b.Field44 );
            Assert.AreEqual( a.DrawOrder, b.DrawOrder );
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
            CompareMaterialProperties( a.Attributes, b.Attributes );
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

        private void CompareMaterialProperties( List<MaterialAttribute> a, List<MaterialAttribute> b )
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

                switch ( a[i].AttributeType )
                {
                    case MaterialAttributeType.Type0:
                        {
                            var a2 = ( MaterialAttributeType0 )a[i];
                            var b2 = ( MaterialAttributeType0 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field28, b2.Field28 );
                            Assert.AreEqual( a2.Field2C, b2.Field2C );
                            Assert.AreEqual( a2.Type0Flags, b2.Type0Flags );
                        }
                        break;
                    case MaterialAttributeType.Type1:
                        {
                            var a2 = ( MaterialAttributeType1 )a[i];
                            var b2 = ( MaterialAttributeType1 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field1C, b2.Field1C );
                            Assert.AreEqual( a2.Field20, b2.Field20 );
                            Assert.AreEqual( a2.Field24, b2.Field24 );
                            Assert.AreEqual( a2.Field34, b2.Field34 );
                            Assert.AreEqual( a2.Field38, b2.Field38 );
                            Assert.AreEqual( a2.Type1Flags, b2.Type1Flags );
                        }
                        break;
                    case MaterialAttributeType.Type2:
                        {
                            var a2 = ( MaterialAttributeType2 )a[i];
                            var b2 = ( MaterialAttributeType2 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                        }
                        break;
                    case MaterialAttributeType.Type3:
                        {
                            var a2 = ( MaterialAttributeType3 )a[i];
                            var b2 = ( MaterialAttributeType3 )b[i];

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
                    case MaterialAttributeType.Type4:
                        {
                            var a2 = ( MaterialAttributeType4 )a[i];
                            var b2 = ( MaterialAttributeType4 )b[i];

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
                    case MaterialAttributeType.Type5:
                        {
                            var a2 = ( MaterialAttributeType5 )a[i];
                            var b2 = ( MaterialAttributeType5 )b[i];

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
                    case MaterialAttributeType.Type6:
                        {
                            var a2 = ( MaterialAttributeType6 )a[i];
                            var b2 = ( MaterialAttributeType6 )b[i];

                            Assert.AreEqual( a2.Field0C, b2.Field0C );
                            Assert.AreEqual( a2.Field10, b2.Field10 );
                            Assert.AreEqual( a2.Field14, b2.Field14 );
                        }
                        break;
                    case MaterialAttributeType.Type7:
                        // no fields
                        break;
                    default:
                        break;
                }
            }
        }

        private void CompareScenes( Model a, Model b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.ResourceType, b.ResourceType );
            Assert.AreEqual( a.Version, b.Version );

            Assert.AreEqual( a.Flags, b.Flags );
            CollectionAssert.AreEqual( a.Bones, b.Bones );
            Assert.AreEqual( a.BoundingBox, b.BoundingBox );
            Assert.AreEqual( a.BoundingSphere, b.BoundingSphere );
            CompareNodes( a.RootNode, b.RootNode );
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
                    CompareUserProperty( sortedProperties[i], otherSortedProperties[i] );
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

        private void CompareUserProperty( UserProperty a, UserProperty b )
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
                case NodeAttachmentType.Model:
                case NodeAttachmentType.Unknown:
                    throw new InvalidDataException();

                case NodeAttachmentType.Node:
                    CompareNodes( a.GetValue<Node>(), b.GetValue<Node>() );
                    break;
                case NodeAttachmentType.Mesh:
                    CompareGeometries( a.GetValue<Mesh>(), b.GetValue<Mesh>() );
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

            Assert.AreEqual( a.TargetCount, b.TargetCount );
            CollectionAssert.AreEqual( a.TargetInts, b.TargetInts );
            Assert.AreEqual( a.NodeName, b.NodeName );
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

            Assert.AreEqual( a.Type, b.Type );
            Assert.AreEqual( a.AmbientColor, b.AmbientColor );
            Assert.AreEqual( a.DiffuseColor, b.DiffuseColor );
            Assert.AreEqual( a.SpecularColor, b.SpecularColor );

            switch ( a.Type )
            {
                case LightType.Type1:
                    Assert.AreEqual( a.Field20, b.Field20 );
                    Assert.AreEqual( a.Field04, b.Field04 );
                    Assert.AreEqual( a.Field08, b.Field08 );
                    break;
                case LightType.Point:
                    Assert.AreEqual( a.Field10, b.Field10 );
                    Assert.AreEqual( a.Field04, b.Field04 );
                    Assert.AreEqual( a.Field08, b.Field08 );

                    if ( a.Flags.HasFlag( LightFlags.Bit2 ) )
                    {
                        Assert.AreEqual( a.AttenuationStart, b.AttenuationStart );
                        Assert.AreEqual( a.AttenuationEnd, b.AttenuationEnd );
                    }
                    else
                    {
                        Assert.AreEqual( a.Field60, b.Field60 );
                        Assert.AreEqual( a.Field64, b.Field64 );
                        Assert.AreEqual( a.Field68, b.Field68 );
                    }
                    break;
                case LightType.Spot:
                    Assert.AreEqual( a.Field20, b.Field20 );
                    Assert.AreEqual( a.Field08, b.Field08 );
                    Assert.AreEqual( a.Field04, b.Field04 );
                    Assert.AreEqual( a.AngleInnerCone, b.AngleInnerCone );
                    Assert.AreEqual( a.AngleOuterCone, b.AngleOuterCone );
                    goto case LightType.Point;
            }
        }

        private void CompareCameras( Camera a, Camera b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.ViewMatrix, b.ViewMatrix );
            Assert.AreEqual( a.ClipPlaneNear, b.ClipPlaneNear );
            Assert.AreEqual( a.ClipPlaneFar, b.ClipPlaneFar );
            Assert.AreEqual( a.FieldOfView, b.FieldOfView );
            Assert.AreEqual( a.AspectRatio, b.AspectRatio );
            Assert.AreEqual( a.Field190, b.Field190 );
        }

        private void CompareGeometries( Mesh a, Mesh b )
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

            CompareMorphTargetLists( a.MorphTargets, b.MorphTargets );

            Assert.AreEqual( a.MaterialName, b.MaterialName );
            Assert.AreEqual( a.BoundingBox, b.BoundingBox );
            Assert.AreEqual( a.BoundingSphere, b.BoundingSphere );
            Assert.AreEqual( a.FieldD4, b.FieldD4 );
            Assert.AreEqual( a.FieldD8, b.FieldD8 );
        }

        private void CompareMorphTargetLists( MorphTargetList a, MorphTargetList b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }

            Assert.AreEqual( a.Flags, b.Flags );

            for ( int i = 0; i < a.Count; i++ )
            {
                Assert.AreEqual( a[i].Flags, b[i].Flags );
                Assert.AreEqual( a[i].VertexCount, b[i].VertexCount );
                CollectionAssert.AreEqual( a[i].Vertices, b[i].Vertices );
            }
        }

        private void CompareAnimationPackage( AnimationPack a, AnimationPack b )
        {
            if ( a == null || b == null )
            {
                Assert.IsTrue( a == null ? ( b == null ) : ( b != null ) );
                return;
            }
            Assert.AreEqual( a.ResourceType, b.ResourceType );
            Assert.AreEqual( a.Version, b.Version );

            throw new NotImplementedException();
        }
    }
}