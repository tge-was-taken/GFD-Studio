using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;

namespace AtlusGfdLib.IO
{
    internal class ResourceWriter : IDisposable
    {
        private static Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
        private Stack<EndianBinaryWriter> mWriterStack;
        private EndianBinaryWriter mCurWriter;
        private Endianness mEndianness;

        public ResourceWriter( Stream stream, Endianness endianness )
        {
            InitWriterStack( stream, endianness );
        }

        public static void WriteToStream( Resource resource, Stream stream, Endianness endianness )
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

        // Write primitive methods
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
            WriteVector3( boundingBox.Min );
            WriteVector3( boundingBox.Max );
        }

        private void WriteBoundingSphere( BoundingSphere boundingSphere )
        {
            WriteVector3( boundingSphere.Center );
            WriteFloat( boundingSphere.Radius );
        }

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
        private void StartWritingChunk( uint version, ChunkType type )
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
                writer.Position = ChunkHeader.SIZE_OFFSET;
                writer.Write( (uint)writer.BaseStreamLength );
                writer.Position = 0;

                // copy contents of the previous chunk stream to the underlying stream
                writer.BaseStream.CopyTo( mCurWriter.BaseStream );
            }
        }

        // Header write methods
        private void WriteFileHeader( string magic, uint version, FileType type )
        {
            WriteString( magic, 4 );
            WriteUInt( version );
            WriteUInt( ( uint )type );
            WriteUInt( 0 ); // unknown
        }

        private void WriteResourceFileHeader( uint version, FileType type )
        {
            WriteFileHeader( FileHeader.CMAGIC_FS, version, type );
        }

        // Write resource methods
        private void WriteResourceFile( Resource resource )
        {
            FileType fileType;

            switch ( resource.Type )
            {
                case ResourceType.Model:
                    fileType = FileType.Model;
                    break;
                case ResourceType.ShaderCachePS3:
                    fileType = FileType.ShaderCachePS3;
                    break;
                case ResourceType.ShaderCachePSP2:
                    fileType = FileType.ShaderCachePSP2;
                    break;
                default:
                    throw new Exception( $"Resource type { resource.Type } can not be written to a file directly." );
            }

            WriteResourceFileHeader( resource.Version, fileType );

            switch ( fileType )
            {
                case FileType.Model:
                    WriteModel( ( Model )resource );
                    break;
                case FileType.ShaderCachePS3:
                    WriteShaderCache( ( ShaderCachePS3 )resource, fileType );
                    break;
                case FileType.ShaderCachePSP2:
                    WriteShaderCache( ( ShaderCachePSP2 )resource, fileType );
                    break;
            }
        }

        private void WriteModel( Model model )
        {
            if ( model.TextureDictionary != null )
            {
                WriteTextureDictionary( model.TextureDictionary );
            }

            if ( model.MaterialDictionary != null )
            {
                WriteMaterialDictionary( model.MaterialDictionary );
            }

            if ( model.Scene != null )
            {
                WriteScene( model.Scene );
            }

            if ( model.AnimationPackage != null )
            {
                WriteAnimationPackage( model.AnimationPackage );
            }

            bool animationOnly = model.AnimationPackage != null &&
                model.TextureDictionary == null &&
                model.MaterialDictionary == null &&
                model.Scene == null &&
                model.ChunkType000100F9 == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                WriteEndChunk( model.Version );
        }

        // Write texture dictionary methods
        private void WriteTextureDictionary( TextureDictionary textureDictionary )
        {
            StartWritingChunk( textureDictionary.Version, ChunkType.TextureDictionary );

            WriteInt( textureDictionary.Count );
            foreach ( var texture in textureDictionary.Textures )
            {
                WriteTexture( texture );
            }

            FinishWritingChunk();
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

        // Write material methods
        private void WriteMaterialDictionary( MaterialDictionary materialDictionary )
        {
            StartWritingChunk( materialDictionary.Version, ChunkType.MaterialDictionary );

            WriteInt( materialDictionary.Count );
            foreach ( var material in materialDictionary.Materials )
            {
                WriteMaterial( materialDictionary.Version, material );
            }

            FinishWritingChunk();
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
                WriteShort( material.Field48 );
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
                WriteByte( material.Field48 );
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
            WriteInt( material.Field6C );
            WriteInt( material.Field70 );
            WriteShort( material.Field50 );

            if ( version <= 0x1105070 )
            {
                WriteInt( material.Field98 );
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
                        WriteMaterialAttributeType2( version, ( MaterialAttributeType2 )attribute );
                        break;
                    case MaterialAttributeType.Type3:
                        WriteMaterialAttributeType3( version, ( MaterialAttributeType3 )attribute );
                        break;
                    case MaterialAttributeType.Type4:
                        WriteMaterialAttributeType4( version, ( MaterialAttributeType4 )attribute );
                        break;
                    case MaterialAttributeType.Type5:
                        WriteMaterialAttributeType5( version, ( MaterialAttributeType5 )attribute );
                        break;
                    case MaterialAttributeType.Type6:
                        WriteMaterialAttributeType6( version, ( MaterialAttributeType6 )attribute );
                        break;
                    case MaterialAttributeType.Type7:
                        WriteMaterialAttributeType7( version, ( MaterialAttributeType7 )attribute );
                        break;
                    default:
                        throw new Exception( $"Unknown material attribute type { attribute.Type } " );
                }
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

        private void WriteMaterialAttributeType2( uint version, MaterialAttributeType2 attribute )
        {
            WriteInt( attribute.Field0C );
            WriteInt( attribute.Field10 );
        }

        private void WriteMaterialAttributeType3( uint version, MaterialAttributeType3 attribute )
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

        private void WriteMaterialAttributeType4( uint version, MaterialAttributeType4 attribute )
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

        private void WriteMaterialAttributeType5( uint version, MaterialAttributeType5 attribute )
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

        private void WriteMaterialAttributeType6( uint version, MaterialAttributeType6 attribute )
        {
            WriteInt( attribute.Field0C );
            WriteInt( attribute.Field10 );
            WriteInt( attribute.Field14 );
        }

        private void WriteMaterialAttributeType7( uint version, MaterialAttributeType7 attribute )
        {
        }

        // Write scene methods
        private void WriteScene( Scene scene )
        {
            StartWritingChunk( scene.Version, ChunkType.Scene );

            WriteInt( (int)scene.Flags );

            if ( scene.Flags.HasFlag( SceneFlags.HasSkinning ) )
                WriteMatrixMap( scene.MatrixMap );

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingBox ) )
                WriteBoundingBox( scene.BoundingBox.Value );

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                WriteBoundingSphere( scene.BoundingSphere.Value );

            WriteNodeRecursive( scene.Version, scene.RootNode );

            FinishWritingChunk();
        }

        private void WriteMatrixMap( SkinnedBoneMap matrixMap )
        {
            WriteInt( matrixMap.MatrixCount );

            for ( int i = 0; i < matrixMap.BoneInverseBindMatrices.Length; i++ )
                WriteMatrix4x4( matrixMap.BoneInverseBindMatrices[i] );

            for ( int i = 0; i < matrixMap.BoneToNodeIndices.Length; i++ )
                WriteUShort( matrixMap.BoneToNodeIndices[i] );
        }

        private void WriteNodeRecursive( uint version, Node node )
        {
            Debug.WriteLine( node.Name );
            WriteNode( version, node );
            WriteInt( node.ChildCount );

            foreach ( var childNode in node.Children )
            {
                WriteNodeRecursive( version, childNode );
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

                //case NodeAttachmentType.Mesh:
                //    WriteMesh( version, attachment.GetValue<Mesh>() );
                //    break;
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
                //case NodeAttachmentType.Epl:
                //    WriteEpl( version, attachment.GetValue<Epl>() );
                //    break;
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
            {
                WriteInt( morph.TargetInts[i] );
            }

            WriteStringWithHash( version, morph.MaterialName );
        }

        private void WriteNodeProperties( uint version, Dictionary<string, NodeProperty> properties )
        {
            WriteInt( properties.Count );

            foreach ( var property in properties.Values )
            {
                WriteInt( ( int )property.ValueType );
                WriteStringWithHash( version, property.Name );

                switch ( property.ValueType )
                {
                    case PropertyValueType.Int:
                        WriteInt( sizeof( int ) );
                        WriteInt( property.GetValue<int>() );
                        break;
                    case PropertyValueType.Float:
                        WriteInt( sizeof( float ) );
                        WriteFloat( property.GetValue<float>() );
                        break;
                    case PropertyValueType.Bool:
                        WriteInt( sizeof( bool ) );
                        WriteBool( property.GetValue<bool>() );
                        break;
                    case PropertyValueType.String:
                        {
                            var value = property.GetValue<string>();
                            WriteInt( value.Length + 1 );
                            WriteString( value, value.Length );
                        }
                        break;
                    case PropertyValueType.ByteVector3:
                        WriteInt( 3 );
                        WriteByteVector3( property.GetValue<ByteVector3>() );
                        break;
                    case PropertyValueType.ByteVector4:
                        WriteInt( 4 );
                        WriteByteVector4( property.GetValue<ByteVector4>() );
                        break;
                    case PropertyValueType.Vector3:
                        WriteInt( 12 );
                        WriteVector3( property.GetValue<Vector3>() );
                        break;
                    case PropertyValueType.Vector4:
                        WriteInt( 16 );
                        WriteVector4( property.GetValue<Vector4>() );
                        break;
                    case PropertyValueType.ByteArray:
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

        private void WriteAnimationPackage( AnimationPackage animationPackage )
        {
            throw new NotImplementedException();

            StartWritingChunk( animationPackage.Version, ChunkType.AnimationPackage );
            FinishWritingChunk();
        }

        private void WriteEndChunk( uint version )
        {
            WriteUInt( version );
            WriteUInt( 0 );
            WriteUInt( 0 ); 
            WriteUInt( 0 ); 
        }

        // Write shader cache methods
        private void WriteShaderCacheFileHeader( uint version, FileType type )
        {
            WriteFileHeader( FileHeader.CMAGIC_SHADERCACHE, version, type );
        }

        private void WriteShaderCache<TShader>( ShaderCacheBase<TShader> shaderCache, FileType type ) where TShader : ShaderBase
        {
            WriteShaderCacheFileHeader( shaderCache.Version, type );

            foreach ( var shader in shaderCache )
            {
                WriteShader( shaderCache.Version, type, shader );
            }
        }

        private void WriteShader( uint version, FileType type, ShaderBase shader )
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
