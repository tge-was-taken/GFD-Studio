using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using AtlusGfdLibrary.Cameras;
using AtlusGfdLibrary.IO.Common;
using AtlusGfdLibrary.Lights;
using AtlusGfdLibrary.Shaders;

namespace AtlusGfdLibrary.IO.Resource
{
    internal class ResourceWriter : IDisposable
    {
        private static readonly Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
        private Stack<EndianBinaryWriter> mWriterStack;
        private EndianBinaryWriter mCurWriter;
        private Endianness mEndianness;

        public ResourceWriter( Stream stream, Endianness endianness )
        {
            InitWriterStack( stream, endianness );
        }

        public static void WriteToStream( AtlusGfdLibrary.Resource resource, Stream stream, Endianness endianness )
        {
            using ( var writer = new ResourceWriter( stream, endianness ) )
                writer.WriteResourceFile( resource );
        }

        public void Dispose()
        {
            while (mWriterStack.Count != 0 )
            {
                mWriterStack.Pop().Dispose();
            }
        }

        #region Write primitive methods

        private void WriteByte( byte value ) => mCurWriter.Write( value );

        private void WriteBool( bool value ) => mCurWriter.Write( ( byte )( value ? 1 : 0 ) );

        private void WriteBytes( byte[] value ) => mCurWriter.Write( value );

        private void WriteShort( short value ) => mCurWriter.Write( value );

        private void WriteUShort( ushort value ) => mCurWriter.Write( value );

        private void WriteInt( int value ) => mCurWriter.Write( value );

        private void WriteUInt( uint value ) => mCurWriter.Write( value );

        private void WriteFloat( float value ) => mCurWriter.Write( value );

        private void WriteString( string value, int length )
        {
            WriteBytes( sSJISEncoding.GetBytes( value ) );
        }

        private void WriteString( string value )
        {
            WriteUShort( (ushort)sSJISEncoding.GetByteCount( value ) );
            WriteString( value, value.Length );
        }

        private void WriteStringWithHash( uint version, string value )
        {
            WriteString( value );

            if ( version > 0x1080000 )
            {
                WriteInt( StringHasher.GenerateStringHash( value ) );

                // if ( version <= 0x1104990 )
                // {
                //    hash = StringHasher.GenerateStringHash(str); // TODO: unsupported hash algo?
                // }
            }         
        }

        private void WriteVector2( Vector2 value )
        {
            WriteFloat( value.X );
            WriteFloat( value.Y );
        }

        private void WriteVector3( Vector3 value )
        {
            WriteFloat( value.X );
            WriteFloat( value.Y );
            WriteFloat( value.Z );
        }

        private void WriteByteVector3( ByteVector3 value )
        {
            WriteByte( value.X );
            WriteByte( value.Y );
            WriteByte( value.Z );
        }

        private void WriteVector4( Vector4 value )
        {
            WriteFloat( value.X );
            WriteFloat( value.Y );
            WriteFloat( value.Z );
            WriteFloat( value.W );
        }

        private void WriteByteVector4( ByteVector4 value )
        {
            WriteByte( value.X );
            WriteByte( value.Y );
            WriteByte( value.Z );
            WriteByte( value.W );
        }

        private void WriteQuaternion( Quaternion value )
        {
            WriteFloat( value.X );
            WriteFloat( value.Y );
            WriteFloat( value.Z );
            WriteFloat( value.W );
        }

        private void WriteMatrix4x4( Matrix4x4 matrix4x4 )
        {
            WriteFloat( matrix4x4.M11 );
            WriteFloat( matrix4x4.M12 );
            WriteFloat( matrix4x4.M13 );
            WriteFloat( matrix4x4.M14 );
            WriteFloat( matrix4x4.M21 );
            WriteFloat( matrix4x4.M22 );
            WriteFloat( matrix4x4.M23 );
            WriteFloat( matrix4x4.M24 );
            WriteFloat( matrix4x4.M31 );
            WriteFloat( matrix4x4.M32 );
            WriteFloat( matrix4x4.M33 );
            WriteFloat( matrix4x4.M34 );
            WriteFloat( matrix4x4.M41 );
            WriteFloat( matrix4x4.M42 );
            WriteFloat( matrix4x4.M43 );
            WriteFloat( matrix4x4.M44 );
        }

        private void WriteBoundingBox( BoundingBox boundingBox )
        {
            WriteVector3( boundingBox.Max );
            WriteVector3( boundingBox.Min );           
        }

        private void WriteBoundingSphere( BoundingSphere boundingSphere )
        {
            WriteVector3( boundingSphere.Center );
            WriteFloat( boundingSphere.Radius );
        }

        #endregion

        // Writer stack methods
        private void InitWriterStack( Stream stream, Endianness endianness )
        {
            mWriterStack = new Stack<EndianBinaryWriter>();
            PushWriter( new EndianBinaryWriter( stream, endianness ) );
            mEndianness = endianness;
        }

        private void PushWriter( EndianBinaryWriter writer )
        {
            mWriterStack.Push( writer );
            mCurWriter = writer;
        }

        private void PushWriter()
        {
            var writer = new EndianBinaryWriter( new MemoryStream(), mEndianness );
            PushWriter( writer );
        }

        private EndianBinaryWriter PopWriter()
        {
            var writer = mWriterStack.Pop();
            mCurWriter = mWriterStack.Peek();

            return writer;
        }

        // Chunk write methods
        private void StartWritingChunk( uint version, ResourceChunkType type )
        {
            // push new writer
            PushWriter();

            // write header
            WriteUInt( version );
            WriteUInt( (uint)type );
            WriteUInt( 0 ); // size
            WriteUInt( 0 ); // unknown
        }

        private void FinishWritingChunk()
        {
            // pop writer and dispose of it after we're done -- it won't be used anymore
            using ( var writer = PopWriter() )
            {
                // write actual chunk size         
                writer.Position = ResourceChunkHeader.SIZE_OFFSET;
                writer.Write( (uint)writer.BaseStreamLength );
                writer.Position = 0;

                // copy contents of the previous chunk stream to the underlying stream
                writer.BaseStream.CopyTo( mCurWriter.BaseStream );
            }
        }

        // Header write methods
        private void WriteFileHeader( string magic, uint version, ResourceFileType type )
        {
            WriteString( magic, 4 );
            WriteUInt( version );
            WriteUInt( ( uint )type );
            WriteUInt( 0 ); // unknown
        }

        private void WriteResourceFileHeader( uint version, ResourceFileType type )
        {
            WriteFileHeader( ResourceFileHeader.MAGIC_FS, version, type );
        }

        // Write resource methods
        private void WriteResourceFile( AtlusGfdLibrary.Resource resource )
        {
            WriteResourceFileHeader( resource.Version, GetFileType( resource.Type ) );
            WriteResource( resource );
        }

        private ResourceFileType GetFileType( ResourceType type )
        {
            switch ( type )
            {
                // Front resource types
                case ResourceType.Model:
                case ResourceType.AnimationPackage:
                case ResourceType.ChunkType000100F9:
                case ResourceType.Scene:
                    return ResourceFileType.ModelResourceBundle;

                // Shader caches
                case ResourceType.ShaderCachePS3:
                    return ResourceFileType.ShaderCachePS3;
                case ResourceType.ShaderCachePSP2:
                    return ResourceFileType.ShaderCachePSP2;

                // Custom resource types
                case ResourceType.Bundle:
                    return ResourceFileType.CustomGenericResourceBundle;

                case ResourceType.TextureDictionary:
                    return ResourceFileType.CustomTextureDictionary;

                case ResourceType.MaterialDictionary:
                    return ResourceFileType.CustomMaterialDictionary;

                case ResourceType.Material:
                    return ResourceFileType.CustomMaterial;

                case ResourceType.TextureMap:
                    return ResourceFileType.CustomTextureMap;

                case ResourceType.MaterialAttribute:
                    return ResourceFileType.CustomMaterialAttribute;

                case ResourceType.Invalid:
                default:
                    throw new Exception( $"Resource type { type } can not be written to a file directly." );
            }
        }

        private ResourceChunkType GetChunkType( ResourceType type )
        {
            switch ( type )
            {
                // Front resource types
                case ResourceType.AnimationPackage:
                    return ResourceChunkType.AnimationPackage;
                case ResourceType.TextureDictionary:
                    return ResourceChunkType.TextureDictionary;
                case ResourceType.MaterialDictionary:
                    return ResourceChunkType.MaterialDictionary;
                case ResourceType.Scene:
                    return ResourceChunkType.Scene;
                case ResourceType.ChunkType000100F9:
                    return ResourceChunkType.ChunkType000100F9;

                // Custom resource types
                case ResourceType.Bundle:
                    return ResourceChunkType.CustomGenericResourceBundle;

                case ResourceType.Material:
                    return ResourceChunkType.CustomMaterial;

                case ResourceType.TextureMap:
                    return ResourceChunkType.CustomTextureMap;

                case ResourceType.MaterialAttribute:
                    return ResourceChunkType.CustomMaterialAttribute;
                default:
                    throw new ArgumentOutOfRangeException( nameof( type ), type, null );
            }
        }

        private void WriteResource( AtlusGfdLibrary.Resource resource )
        {
            switch ( resource.Type )
            {
                case ResourceType.Model:
                    WriteModel( ( Model )resource );
                    break;

                case ResourceType.AnimationPackage:
                    WriteAnimationPackage( ( AnimationPackage )resource );
                    break;

                case ResourceType.TextureDictionary:
                    WriteTextureDictionary( ( TextureDictionary )resource );
                    break;

                case ResourceType.MaterialDictionary:
                    WriteMaterialDictionary( ( MaterialDictionary )resource );
                    break;

                case ResourceType.Scene:
                    WriteScene( ( Scene )resource );
                    break;

                case ResourceType.ChunkType000100F9:
                    WriteChunkType000100F9( ( ChunkType000100F9 ) resource );
                    break;

                case ResourceType.ShaderCachePS3:
                    WriteShaderCache( ( ShaderCachePS3 )resource );
                    break;

                case ResourceType.ShaderCachePSP2:
                    WriteShaderCache( ( ShaderCachePSP2 )resource );
                    break;

                // Custom resource types
                case ResourceType.Bundle:
                    WriteResourceBundle( ( ResourceBundle )resource );
                    break;

                case ResourceType.Material:
                    WriteMaterial( resource.Version, ( Material )resource );
                    break;

                case ResourceType.MaterialAttribute:
                    WriteMaterialAttribute( resource.Version, ( MaterialAttribute )resource );
                    break;

                default:
                    throw new Exception( $"Resource type { resource.Type } can not be written to a file directly." );
            }
        }

        private void WriteResourceBundle( ResourceBundle resource )
        {
            foreach ( var subResource in resource.GetResources() )
            {
                StartWritingChunk( subResource.Version, GetChunkType( subResource.Type ) );
                WriteResource( subResource );
                FinishWritingChunk();
            }
        }

        // Model
        private void WriteModel( Model model )
        {
            if ( model.TextureDictionary != null )
                WriteTextureDictionaryChunk( model.TextureDictionary );

            if ( model.MaterialDictionary != null )
                WriteMaterialDictionaryChunk( model.MaterialDictionary );

            if ( model.Scene != null )
                WriteSceneChunk( model.Scene );

            if ( model.ChunkType000100F9 != null )
                WriteChunkType000100F9Chunk( model.ChunkType000100F9 );

            if ( model.AnimationPackage != null )
                WriteAnimationPackageChunk( model.AnimationPackage );

            bool animationOnly = model.AnimationPackage != null &&
                model.TextureDictionary == null &&
                model.MaterialDictionary == null &&
                model.Scene == null &&
                model.ChunkType000100F9 == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                WriteEndChunk( model.Version );
        }

        // Texture
        private void WriteTextureDictionaryChunk( TextureDictionary textureDictionary )
        {
            StartWritingChunk( textureDictionary.Version, ResourceChunkType.TextureDictionary );

            WriteTextureDictionary( textureDictionary );

            FinishWritingChunk();
        }

        private void WriteTextureDictionary( TextureDictionary textureDictionary )
        {
            WriteInt( textureDictionary.Count );
            foreach ( var texture in textureDictionary.Textures )
            {
                WriteTexture( texture );
            }
        }

        private void WriteTexture( Texture texture )
        {
            WriteString( texture.Name );
            WriteShort( (short)texture.Format );
            WriteInt( texture.Data.Length );
            WriteBytes( texture.Data );
            WriteByte( texture.Field1C );
            WriteByte( texture.Field1D );
            WriteByte( texture.Field1E );
            WriteByte( texture.Field1F );
        }

        // Material
        private void WriteMaterialDictionaryChunk( MaterialDictionary materialDictionary )
        {
            StartWritingChunk( materialDictionary.Version, ResourceChunkType.MaterialDictionary );

            WriteMaterialDictionary( materialDictionary );

            FinishWritingChunk();
        }

        private void WriteMaterialDictionary( MaterialDictionary materialDictionary )
        {
            WriteInt( materialDictionary.Count );
            foreach ( var material in materialDictionary.Materials )
            {
                WriteMaterial( materialDictionary.Version, material );
            }
        }

        private void WriteMaterial( uint version, Material material )
        {
            WriteStringWithHash( version, material.Name );
            WriteUInt( (uint)material.Flags );
            WriteVector4( material.Ambient );
            WriteVector4( material.Diffuse );
            WriteVector4( material.Specular );
            WriteVector4( material.Emissive );
            WriteFloat( material.Field40 );
            WriteFloat( material.Field44 );

            if ( version <= 0x1103040 )
            {
                WriteShort( (short)material.DrawOrder );
                WriteShort( material.Field49 );
                WriteShort( material.Field4A );
                WriteShort( material.Field4B );
                WriteShort( material.Field4C );

                if ( version > 0x108011b )
                {
                    WriteShort( material.Field4D );
                }
            }
            else
            {
                WriteByte( (byte)material.DrawOrder );
                WriteByte( material.Field49 );
                WriteByte( material.Field4A );
                WriteByte( material.Field4B );
                WriteByte( material.Field4C );
                WriteByte( material.Field4D );
            }

            WriteShort( material.Field90 );
            WriteShort( material.Field92 );

            if ( version <= 0x1104800 )
            {
                WriteInt( material.Field96 );
            }
            else
            {
                WriteShort( material.Field94 );
                WriteShort( material.Field96 );
            }

            WriteShort( material.Field5C );
            WriteUInt( material.Field6C );
            WriteUInt( material.Field70 );
            WriteShort( material.Field50 );

            if ( version <= 0x1105070 || version >= 0x1105090 )
            {
                WriteUInt( material.Field98 );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                WriteTextureMap( version, material.DiffuseMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                WriteTextureMap( version, material.NormalMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                WriteTextureMap( version, material.SpecularMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                WriteTextureMap( version, material.ReflectionMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                WriteTextureMap( version, material.HighlightMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                WriteTextureMap( version, material.GlowMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                WriteTextureMap( version, material.NightMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                WriteTextureMap( version, material.DetailMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                WriteTextureMap( version, material.ShadowMap );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasAttributes ) && material.Attributes.Count != 0 )
            {
                WriteMaterialAttributes( version, material.Attributes );
            }
        }

        private void WriteTextureMap( uint version, TextureMap map )
        {
            WriteStringWithHash( version, map.Name );
            WriteInt( map.Field44 );
            WriteByte( map.Field48 );
            WriteByte( map.Field49 );
            WriteByte( map.Field4A );
            WriteByte( map.Field4B );
            WriteFloat( map.Field4C );
            WriteFloat( map.Field50 );
            WriteFloat( map.Field54 );
            WriteFloat( map.Field58 );
            WriteFloat( map.Field5C );
            WriteFloat( map.Field60 );
            WriteFloat( map.Field64 );
            WriteFloat( map.Field68 );
            WriteFloat( map.Field6C );
            WriteFloat( map.Field70 );
            WriteFloat( map.Field74 );
            WriteFloat( map.Field78 );
            WriteFloat( map.Field7C );
            WriteFloat( map.Field80 );
            WriteFloat( map.Field84 );
            WriteFloat( map.Field88 );
        }

        private void WriteMaterialAttributes( uint version, List<MaterialAttribute> attributes )
        {
            WriteInt( attributes.Count );

            foreach ( var attribute in attributes )
            {
                WriteMaterialAttribute( version, attribute );
            }
        }

        private void WriteMaterialAttribute( uint version, MaterialAttribute attribute )
        {
            WriteUInt( attribute.RawFlags );

            switch ( attribute.Type )
            {
                case MaterialAttributeType.Type0:
                    WriteMaterialAttributeType0( version, ( MaterialAttributeType0 )attribute );
                    break;
                case MaterialAttributeType.Type1:
                    WriteMaterialAttributeType1( version, ( MaterialAttributeType1 )attribute );
                    break;
                case MaterialAttributeType.Type2:
                    WriteMaterialAttributeType2( ( MaterialAttributeType2 )attribute );
                    break;
                case MaterialAttributeType.Type3:
                    WriteMaterialAttributeType3( ( MaterialAttributeType3 )attribute );
                    break;
                case MaterialAttributeType.Type4:
                    WriteMaterialAttributeType4( ( MaterialAttributeType4 )attribute );
                    break;
                case MaterialAttributeType.Type5:
                    WriteMaterialAttributeType5( ( MaterialAttributeType5 )attribute );
                    break;
                case MaterialAttributeType.Type6:
                    WriteMaterialAttributeType6( ( MaterialAttributeType6 )attribute );
                    break;
                case MaterialAttributeType.Type7:
                    // no data
                    break;
                default:
                    throw new Exception( $"Unknown material attribute type { attribute.Type } " );
            }
        }

        private void WriteMaterialAttributeType0( uint version, MaterialAttributeType0 attribute )
        {
            if ( version > 0x1104500 )
            {
                WriteVector4( attribute.Field0C );
                WriteFloat( attribute.Field1C );
                WriteFloat( attribute.Field20 );
                WriteFloat( attribute.Field24 );
                WriteFloat( attribute.Field28 );
                WriteFloat( attribute.Field2C );
                WriteInt( (int)attribute.Type0Flags );
            }
            else if ( version > 0x1104220 )
            {
                WriteVector4( attribute.Field0C );
                WriteFloat( attribute.Field1C );
                WriteFloat( attribute.Field20 );
                WriteFloat( attribute.Field24 );
                WriteFloat( attribute.Field28 );
                WriteFloat( attribute.Field2C );

                WriteBool( attribute.Type0Flags.HasFlag( MaterialAttributeType0Flags.Flag1 ) );
                WriteBool( attribute.Type0Flags.HasFlag( MaterialAttributeType0Flags.Flag2 ) );
                WriteBool( attribute.Type0Flags.HasFlag( MaterialAttributeType0Flags.Flag4 ) );

                if ( version > 0x1104260 )
                {
                    WriteBool( attribute.Type0Flags.HasFlag( MaterialAttributeType0Flags.Flag8 ) );
                }
            }
            else
            {
                WriteVector4( attribute.Field0C );
                WriteFloat( attribute.Field1C );
                WriteFloat( attribute.Field20 );
                WriteFloat( attribute.Field28 );
                WriteFloat( attribute.Field2C );
            }
        }

        private void WriteMaterialAttributeType1( uint version, MaterialAttributeType1 attribute )
        {
            WriteVector4( attribute.Field0C );
            WriteFloat( attribute.Field1C );
            WriteFloat( attribute.Field20 );
            WriteVector4( attribute.Field24 );
            WriteFloat( attribute.Field34 );
            WriteFloat( attribute.Field38 );

            if ( version <= 0x1104500 )
            {
                WriteBool( attribute.Type1Flags.HasFlag( MaterialAttributeType1Flags.Flag1 ) );

                if ( version > 0x1104180 )
                {
                    WriteBool( attribute.Type1Flags.HasFlag( MaterialAttributeType1Flags.Flag2 ) );
                }

                if ( version > 0x1104210 )
                {
                    WriteBool( attribute.Type1Flags.HasFlag( MaterialAttributeType1Flags.Flag4 ) );
                }

                if ( version > 0x1104400 )
                {
                    WriteBool( attribute.Type1Flags.HasFlag( MaterialAttributeType1Flags.Flag8 ) );
                }
            }
            else
            {
                WriteInt( (int)attribute.Type1Flags );
            }
        }

        private void WriteMaterialAttributeType2( MaterialAttributeType2 attribute )
        {
            WriteInt( attribute.Field0C );
            WriteInt( attribute.Field10 );
        }

        private void WriteMaterialAttributeType3( MaterialAttributeType3 attribute )
        {
            WriteFloat( attribute.Field0C );
            WriteFloat( attribute.Field10 );
            WriteFloat( attribute.Field14 );
            WriteFloat( attribute.Field18 );
            WriteFloat( attribute.Field1C );
            WriteFloat( attribute.Field20 );
            WriteFloat( attribute.Field24 );
            WriteFloat( attribute.Field28 );
            WriteFloat( attribute.Field2C );
            WriteFloat( attribute.Field30 );
            WriteFloat( attribute.Field34 );
            WriteFloat( attribute.Field38 );
            WriteInt( attribute.Field3C );
        }

        private void WriteMaterialAttributeType4( MaterialAttributeType4 attribute )
        {
            WriteVector4( attribute.Field0C );
            WriteFloat( attribute.Field1C );
            WriteFloat( attribute.Field20 );
            WriteVector4( attribute.Field24 );
            WriteFloat( attribute.Field34 );
            WriteFloat( attribute.Field38 );
            WriteFloat( attribute.Field3C );
            WriteFloat( attribute.Field40 );
            WriteFloat( attribute.Field44 );
            WriteFloat( attribute.Field48 );
            WriteFloat( attribute.Field4C );
            WriteByte( attribute.Field50 );
            WriteFloat( attribute.Field54 );
            WriteFloat( attribute.Field58 );
            WriteInt( attribute.Field5C );
        }

        private void WriteMaterialAttributeType5( MaterialAttributeType5 attribute )
        {
            WriteInt( attribute.Field0C );
            WriteInt( attribute.Field10 );
            WriteFloat( attribute.Field14 );
            WriteFloat( attribute.Field18 );
            WriteVector4( attribute.Field1C );
            WriteFloat( attribute.Field2C );
            WriteFloat( attribute.Field30 );
            WriteFloat( attribute.Field34 );
            WriteFloat( attribute.Field38 );
            WriteFloat( attribute.Field3C );
            WriteVector4( attribute.Field48 );
        }

        private void WriteMaterialAttributeType6( MaterialAttributeType6 attribute )
        {
            WriteInt( attribute.Field0C );
            WriteInt( attribute.Field10 );
            WriteInt( attribute.Field14 );
        }

        // Scene
        private void WriteSceneChunk( Scene scene )
        {
            StartWritingChunk( scene.Version, ResourceChunkType.Scene );

            WriteScene( scene );

            FinishWritingChunk();
        }

        private void WriteScene( Scene scene )
        {
            WriteInt( ( int )scene.Flags );

            if ( scene.Flags.HasFlag( SceneFlags.HasSkinning ) )
                WriteMatrixMap( scene.MatrixPalette );

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingBox ) )
                WriteBoundingBox( scene.BoundingBox.Value );

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                WriteBoundingSphere( scene.BoundingSphere.Value );

            WriteNodeRecursive( scene.Version, scene.RootNode );
        }

        private void WriteMatrixMap( MatrixPalette matrixMap )
        {
            WriteInt( matrixMap.MatrixCount );

            for ( int i = 0; i < matrixMap.InverseBindMatrices.Length; i++ )
                WriteMatrix4x4( matrixMap.InverseBindMatrices[i] );

            for ( int i = 0; i < matrixMap.BoneToNodeIndices.Length; i++ )
                WriteUShort( matrixMap.BoneToNodeIndices[i] );
        }

        private void WriteNodeRecursive( uint version, Node node )
        {
            WriteNode( version, node );
            WriteInt( node.ChildCount );

            for ( int i = 0; i < node.ChildCount; i++ )
            {
                WriteNodeRecursive( version, node.Children[ (node.ChildCount - 1) - i ] );
            }
        }

        private void WriteNode( uint version, Node node )
        {
            WriteStringWithHash( version, node.Name );
            WriteVector3( node.Translation );
            WriteQuaternion( node.Rotation );
            WriteVector3( node.Scale );

            if ( version <= 0x1090000 )
                WriteByte(0);

            WriteInt( node.AttachmentCount );
            foreach ( var attachment in node.Attachments )
            {
                WriteNodeAttachment( version, attachment );
            }

            if ( version > 0x1060000 )
            {
                WriteBool( node.HasProperties );
                if ( node.HasProperties )
                {
                    WriteNodeProperties( version, node.Properties );
                }
            }

            if ( version > 0x1104230 )
            {
                WriteFloat( node.FieldE0 );
            }
        }

        private void WriteNodeAttachment( uint version, NodeAttachment attachment )
        {
            WriteInt( ( int )attachment.Type );

            switch ( attachment.Type )
            {
                case NodeAttachmentType.Invalid:
                case NodeAttachmentType.Scene:
                    break;
                case NodeAttachmentType.Node:
                    WriteNode( version, attachment.GetValue<Node>() );
                    break;
                case NodeAttachmentType.Geometry:
                    WriteGeometry( version, attachment.GetValue<Geometry>() );
                    break;
                case NodeAttachmentType.Camera:
                    WriteCamera( version, attachment.GetValue<Camera>() );
                    break;
                case NodeAttachmentType.Light:
                    WriteLight( version, attachment.GetValue<Light>() );
                    break;
                case NodeAttachmentType.Epl:
                    WriteEpl( version, attachment.GetValue<Epl>() );
                    break;
                //case NodeAttachmentType.EplLeaf:
                //    WriteEplLeafs version, attachment.GetValue<EplLeaf>() );
                //    break;
                case NodeAttachmentType.Morph:
                    WriteMorph( version, attachment.GetValue<Morph>() );
                    break;
                default:
                    throw new Exception( $"Unknown node attachment: {attachment.Type}" );
            }
        }

        private void WriteEpl( uint version, Epl epl )
        {
            if ( epl.Raw != null )
                WriteBytes( epl.Raw );
        }

        private void WriteLight( uint version, Light light )
        {
            if ( version > 0x1104190 )
            {
                WriteInt( ( int )light.Flags );
            }

            WriteInt( ( int )light.Type );
            WriteVector4( light.Field30 );
            WriteVector4( light.Field40 );
            WriteVector4( light.Field50 );

            switch ( light.Type )
            {
                case LightType.Type1:
                    WriteFloat( light.Field20 );
                    WriteFloat( light.Field04 );
                    WriteFloat( light.Field08 );
                    break;
                case LightType.Sky:
                    WriteFloat( light.Field10 );
                    WriteFloat( light.Field04 );
                    WriteFloat( light.Field08 );

                    if ( light.Flags.HasFlag( LightFlags.Flag2 ) )
                    {
                        WriteFloat( light.Field6C );
                        WriteFloat( light.Field70 );
                    }
                    else
                    {
                        WriteFloat( light.Field60 );
                        WriteFloat( light.Field64 );
                        WriteFloat( light.Field68 );
                    }
                    break;
                case LightType.Type3:
                    WriteFloat( light.Field20 );
                    WriteFloat( light.Field08 );
                    WriteFloat( light.Field04 );
                    WriteFloat( light.Field74 );
                    WriteFloat( light.Field78 );
                    goto case LightType.Sky;
            }
        }

        private void WriteCamera( uint version, Camera camera )
        {
            WriteMatrix4x4( camera.Transform );
            WriteFloat( camera.Field180 );
            WriteFloat( camera.Field184 );
            WriteFloat( camera.Field188 );
            WriteFloat( camera.Field18C );
            
            if ( version > 0x1104060 )
            {
                WriteFloat( camera.Field190 );
            }
        }

        private void WriteMorph( uint version, Morph morph )
        {
            WriteInt( morph.TargetCount );

            for ( int i = 0; i < morph.TargetInts.Length; i++ )
                WriteInt( morph.TargetInts[i] );

            WriteStringWithHash( version, morph.MaterialName );
        }

        private void WriteNodeProperties( uint version, UserPropertyCollection properties )
        {
            WriteInt( properties.Count );

            foreach ( var property in properties.Values )
            {
                WriteInt( ( int )property.ValueType );
                WriteStringWithHash( version, property.Name );

                switch ( property.ValueType )
                {
                    case UserPropertyValueType.Int:
                        WriteInt( 4 );
                        WriteInt( property.GetValue<int>() );
                        break;
                    case UserPropertyValueType.Float:
                        WriteInt( 4 );
                        WriteFloat( property.GetValue<float>() );
                        break;
                    case UserPropertyValueType.Bool:
                        WriteInt( 1 );
                        WriteBool( property.GetValue<bool>() );
                        break;
                    case UserPropertyValueType.String:
                        {
                            var value = property.GetValue<string>();
                            WriteInt( value.Length + 1 );
                            WriteString( value, value.Length );
                        }
                        break;
                    case UserPropertyValueType.ByteVector3:
                        WriteInt( 3 );
                        WriteByteVector3( property.GetValue<ByteVector3>() );
                        break;
                    case UserPropertyValueType.ByteVector4:
                        WriteInt( 4 );
                        WriteByteVector4( property.GetValue<ByteVector4>() );
                        break;
                    case UserPropertyValueType.Vector3:
                        WriteInt( 12 );
                        WriteVector3( property.GetValue<Vector3>() );
                        break;
                    case UserPropertyValueType.Vector4:
                        WriteInt( 16 );
                        WriteVector4( property.GetValue<Vector4>() );
                        break;
                    case UserPropertyValueType.ByteArray:
                        {
                            var value = property.GetValue<byte[]>();
                            WriteInt( value.Length );
                            WriteBytes( value );
                        }
                        break;
                    default:
                        throw new Exception( $"Unknown node property type: {property.ValueType}" );
                }
            }
        }

        private void WriteGeometry( uint version, Geometry geometry )
        {
            WriteInt( ( int )geometry.Flags );
            WriteInt( ( int )geometry.VertexAttributeFlags );

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                WriteInt( geometry.TriangleCount );
                WriteShort( (short)geometry.TriangleIndexType );
            }

            WriteInt( geometry.VertexCount );

            if ( version > 0x1103020 )
            {
                WriteInt( geometry.Field14 );
            }

            for ( int i = 0; i < geometry.VertexCount; i++ )
            {
                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                    WriteVector3( geometry.Vertices[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
                    WriteVector3( geometry.Normals[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
                    WriteVector3( geometry.Tangents[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
                    WriteVector3( geometry.Binormals[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
                    WriteUInt( geometry.ColorChannel0[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
                    WriteVector2( geometry.TexCoordsChannel0[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                    WriteVector2( geometry.TexCoordsChannel1[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                    WriteVector2( geometry.TexCoordsChannel2[i] );

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                    WriteUInt( geometry.ColorChannel1[i] );

                if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                {
                    var vertexWeight = geometry.VertexWeights[i];
                    for ( int j = 0; j < 4; j++ )
                        WriteFloat( vertexWeight.Weights[j] );

                    int indices = vertexWeight.Indices[0] << 00 | ( vertexWeight.Indices[1] << 08 ) |
                                  vertexWeight.Indices[2] << 16 | ( vertexWeight.Indices[3] << 24 );

                    WriteInt( indices );                
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMorphTargets ) )
            {
                WriteMorphTargetList( geometry.MorphTargets );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                for ( int i = 0; i < geometry.Triangles.Length; i++ )
                {
                    switch ( geometry.TriangleIndexType )
                    {
                        case TriangleIndexType.UInt16:
                            WriteUShort( ( ushort )geometry.Triangles[i].A );
                            WriteUShort( ( ushort )geometry.Triangles[i].B );
                            WriteUShort( ( ushort )geometry.Triangles[i].C );
                            break;
                        case TriangleIndexType.UInt32:
                            WriteUInt( geometry.Triangles[i].A );
                            WriteUInt( geometry.Triangles[i].B );
                            WriteUInt( geometry.Triangles[i].C );
                            break;
                        default:
                            throw new Exception( $"Unsupported triangle index type: {geometry.TriangleIndexType}" );
                    }
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMaterial ) )
            {
                WriteStringWithHash( version, geometry.MaterialName );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingBox ) )
            {
                WriteBoundingBox( geometry.BoundingBox.Value );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingSphere ) )
            {
                WriteBoundingSphere( geometry.BoundingSphere.Value );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.Flag1000 ) )
            {
                WriteFloat( geometry.FieldD4 );
                WriteFloat( geometry.FieldD8 );
            }
        }

        private void WriteMorphTargetList( MorphTargetList morphTargets )
        {
            WriteInt( morphTargets.Flags );
            WriteInt( morphTargets.Count );

            foreach ( var morphTarget in morphTargets )
            {
                WriteMorphTarget( morphTarget );
            }
        }

        private void WriteMorphTarget( MorphTarget morphTarget )
        {
            WriteInt( morphTarget.Flags );
            WriteInt( morphTarget.VertexCount );

            foreach ( var vertex in morphTarget.Vertices )
            {
                WriteVector3( vertex );
            }
        }

        // Chunk type 000100F9
        private void WriteChunkType000100F9Chunk( ChunkType000100F9 resource )
        {
            StartWritingChunk( resource.Version, ResourceChunkType.ChunkType000100F9 );

            WriteChunkType000100F9( resource );

            FinishWritingChunk();
        }

        private void WriteChunkType000100F9( ChunkType000100F9 resource )
        {
            WriteInt( resource.Field140 );
            WriteFloat( resource.Field13C );
            WriteFloat( resource.Field138 );
            WriteFloat( resource.Field134 );
            WriteFloat( resource.Field130 );

            WriteInt( resource.Entry1List.Count );
            WriteInt( resource.Entry2List.Count );
            WriteInt( resource.Entry3List.Count );

            foreach ( var entry in resource.Entry1List )
            {
                WriteFloat( entry.Field34 );
                WriteFloat( entry.Field38 );

                if ( resource.Version > 0x1104120 )
                {
                    WriteFloat( entry.Field3C );
                    WriteFloat( entry.Field40 );
                }

                if ( entry.NodeName != null )
                {
                    WriteBool( true );
                    WriteStringWithHash( resource.Version, entry.NodeName );
                }
                else
                {
                    WriteBool( false );
                    WriteFloat( entry.Field10 );
                    WriteFloat( entry.Field08 );
                    WriteFloat( entry.Field04 );
                }
            }

            foreach ( var entry in resource.Entry2List )
            {
                WriteShort( entry.Field94 );

                if ( entry.Field94 == 0 )
                {
                    WriteFloat( entry.Field84 );
                }
                else if ( entry.Field94 == 1 )
                {
                    WriteFloat( entry.Field84 );
                    WriteFloat( entry.Field88 );
                }

                WriteMatrix4x4( entry.Field8C );

                if ( entry.NodeName != null )
                {
                    WriteBool( true );
                    WriteStringWithHash( resource.Version, entry.NodeName );
                }
                else
                {
                    WriteBool( false );
                }
            }

            foreach ( var entry in resource.Entry3List )
            {
                WriteFloat( entry.Field00 );
                WriteFloat( entry.Field04 );

                if ( resource.Version <= 0x1104120 )
                {
                    WriteShort( entry.Field0C );
                    WriteShort( entry.Field0E );
                }
                else
                {
                    WriteFloat( entry.Field08 );
                    WriteShort( entry.Field0C );
                    WriteShort( entry.Field0E );
                }
            }
        }

        // Animation
        private void WriteAnimationPackageChunk( AnimationPackage animationPackage )
        {
            StartWritingChunk( animationPackage.Version, ResourceChunkType.AnimationPackage );
            WriteAnimationPackage( animationPackage );
            FinishWritingChunk();
        }

        private void WriteAnimationPackage( AnimationPackage animationPackage )
        {
            throw new NotImplementedException();
        }

        private void WriteEndChunk( uint version )
        {
            WriteUInt( version );
            WriteUInt( 0 );
            WriteUInt( 0 ); 
            WriteUInt( 0 ); 
        }

        // Shader Cache
        private void WriteShaderCacheFileHeader( uint version, ResourceFileType type )
        {
            WriteFileHeader( ResourceFileHeader.MAGIC_SHADERCACHE, version, type );
        }

        private void WriteShaderCache<TShader>( ShaderCacheBase<TShader> shaderCache ) where TShader : ShaderBase
        {
            WriteShaderCacheFileHeader( shaderCache.Version, shaderCache.GetType() is ShaderCachePS3 ? ResourceFileType.ShaderCachePS3 : ResourceFileType.ShaderCachePSP2 );

            foreach ( var shader in shaderCache )
            {
                WriteShader( shader );
            }
        }

        private void WriteShader( ShaderBase shader )
        {
            WriteUShort( shader.Type );
            WriteInt( shader.DataLength );
            WriteUShort( shader.Field06 );
            WriteUInt( shader.Field08 );
            WriteUInt( shader.Field0C );
            WriteUInt( shader.Field10 );
            WriteUInt( shader.Field14 );
            WriteUInt( shader.Field18 );
            WriteBytes( shader.Data );
        }
    }
}
