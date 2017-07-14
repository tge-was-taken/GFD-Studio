using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace AtlusGfdLib.IO
{
    internal class ResourceWriter : IDisposable
    {
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

        private void WriteByte( byte value ) => mCurWriter.Write( value );

        private void WriteBool( bool value ) => mCurWriter.Write( ( byte )( value ? 1 : 0 ) );

        private void WriteBytes( byte[] value ) => mCurWriter.Write( value );

        private void WriteShort( short value ) => mCurWriter.Write( value );

        private void WriteUShort( ushort value ) => mCurWriter.Write( value );

        private void WriteInt( int value ) => mCurWriter.Write( value );

        private void WriteUInt( uint value ) => mCurWriter.Write( value );

        private void WriteFloat( float value ) => mCurWriter.Write( value );

        private void WriteString( string value, int length ) => mCurWriter.Write( value, StringBinaryFormat.FixedLength, length );

        private void WriteString( string value )
        {
            WriteUShort( ( ushort )value.Length );
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
                writer.Position = 8;
                writer.Write( (uint)writer.BaseStreamLength );
                writer.Position = 0;

                // copy contents of the previous chunk stream to the underlying stream
                writer.BaseStream.CopyTo( mCurWriter.BaseStream );
            }
        }

        private void WriteFileHeader( uint version, FileType type )
        {
            WriteString( FileHeader.CMAGIC_FS, 4 );
            WriteUInt( version );
            WriteUInt( ( uint )type );
            WriteUInt( 0 ); // unknown
        }

        private void WriteResourceFile( Resource resource )
        {
            FileType fileType;

            switch ( resource.Type )
            {
                case ResourceType.Model:
                    fileType = FileType.Model;
                    break;
                case ResourceType.ShaderCache:
                    fileType = FileType.ShaderCache;
                    break;
                default:
                    throw new Exception( $"Resource type { resource.Type } can not be written to a file directly. Wrap it in a {nameof( Model )} or a {nameof( ShaderCache )}" );
            }

            WriteFileHeader( resource.Version, fileType );

            switch ( fileType )
            {
                case FileType.Model:
                    WriteModel( ( Model )resource );
                    break;
                case FileType.ShaderCache:
                    WriteShaderCache( ( ShaderCache )resource );
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

            bool animationOnly = model.AnimationPackage != null &
                model.TextureDictionary == null &&
                model.MaterialDictionary == null &&
                model.Scene == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                WriteEndChunk( model.Version );
        }

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

            if ( material.Flags.HasFlag( MaterialFlags.HasProperties ) && material.Properties.Count != 0 )
            {
                WriteMaterialProperties( version, material.Properties );
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

        private void WriteMaterialProperties( uint version, List<MaterialProperty> properties )
        {
            WriteInt( properties.Count );

            foreach ( var property in properties )
            {
                WriteUInt( property.RawFlags );

                switch ( property.Type )
                {
                    case MaterialPropertyType.Type0:
                        WriteMaterialPropertyType0( version, ( MaterialPropertyType0 )property );
                        break;
                    case MaterialPropertyType.Type1:
                        WriteMaterialPropertyType1( version, ( MaterialPropertyType1 )property );
                        break;
                    case MaterialPropertyType.Type2:
                        WriteMaterialPropertyType2( version, ( MaterialPropertyType2 )property );
                        break;
                    case MaterialPropertyType.Type3:
                        WriteMaterialPropertyType3( version, ( MaterialPropertyType3 )property );
                        break;
                    case MaterialPropertyType.Type4:
                        WriteMaterialPropertyType4( version, ( MaterialPropertyType4 )property );
                        break;
                    case MaterialPropertyType.Type5:
                        WriteMaterialPropertyType5( version, ( MaterialPropertyType5 )property );
                        break;
                    case MaterialPropertyType.Type6:
                        WriteMaterialPropertyType6( version, ( MaterialPropertyType6 )property );
                        break;
                    case MaterialPropertyType.Type7:
                        WriteMaterialPropertyType7( version, ( MaterialPropertyType7 )property );
                        break;
                    default:
                        throw new Exception( $"Unknown material property type { property.Type } " );
                }
            }
        }

        private void WriteMaterialPropertyType0( uint version, MaterialPropertyType0 property )
        {
            if ( version > 0x1104500 )
            {
                WriteVector4( property.Field0C );
                WriteFloat( property.Field1C );
                WriteFloat( property.Field20 );
                WriteFloat( property.Field24 );
                WriteFloat( property.Field28 );
                WriteFloat( property.Field2C );
                WriteInt( property.Field30 );
            }
            else if ( version > 0x1104220 )
            {
                WriteVector4( property.Field0C );
                WriteFloat( property.Field1C );
                WriteFloat( property.Field20 );
                WriteFloat( property.Field24 );
                WriteFloat( property.Field28 );
                WriteFloat( property.Field2C );

                if ( ( property.Field30 & 1 ) == 1 )
                    WriteByte( 1 );
                else
                    WriteByte( 0 );

                if ( ( property.Field30 & 2 ) == 2 )
                    WriteByte( 1 );
                else
                    WriteByte( 0 );

                if ( ( property.Field30 & 4 ) == 4 )
                    WriteByte( 1 );
                else
                    WriteByte( 0 );

                if ( version > 0x1104260 )
                {
                    if ( ( property.Field30 & 8 ) == 8 )
                        WriteByte( 1 );
                    else
                        WriteByte( 0 );
                }
            }
            else
            {
                WriteVector4( property.Field0C );
                WriteFloat( property.Field1C );
                WriteFloat( property.Field20 );
                WriteFloat( property.Field28 );
                WriteFloat( property.Field2C );
            }
        }

        private void WriteMaterialPropertyType1( uint version, MaterialPropertyType1 property )
        {
            WriteVector4( property.Field0C );
            WriteFloat( property.Field1C );
            WriteFloat( property.Field20 );
            WriteVector4( property.Field24 );
            WriteFloat( property.Field34 );
            WriteFloat( property.Field38 );

            if ( version <= 0x1104500 )
            {
                if ( ( property.Field3C & 1 ) == 1 )
                    WriteByte( 1 );
                else
                    WriteByte( 0 );

                if ( version > 0x1104180 )
                {
                    if ( ( property.Field3C & 2 ) == 2 )
                        WriteByte( 1 );
                    else
                        WriteByte( 0 );
                }

                if ( version > 0x1104210 )
                {
                    if ( ( property.Field3C & 4 ) == 4 )
                        WriteByte( 1 );
                    else
                        WriteByte( 0 );
                }

                if ( version > 0x1104400 )
                {
                    if ( ( property.Field3C & 8 ) == 8 )
                        WriteByte( 1 );
                    else
                        WriteByte( 0 );
                }
            }
            else
            {
                WriteInt( property.Field3C );
            }
        }

        private void WriteMaterialPropertyType2( uint version, MaterialPropertyType2 property )
        {
            WriteInt( property.Field0C );
            WriteInt( property.Field10 );
        }

        private void WriteMaterialPropertyType3( uint version, MaterialPropertyType3 property )
        {
            WriteFloat( property.Field0C );
            WriteFloat( property.Field10 );
            WriteFloat( property.Field14 );
            WriteFloat( property.Field18 );
            WriteFloat( property.Field1C );
            WriteFloat( property.Field20 );
            WriteFloat( property.Field24 );
            WriteFloat( property.Field28 );
            WriteFloat( property.Field2C );
            WriteFloat( property.Field30 );
            WriteFloat( property.Field34 );
            WriteFloat( property.Field38 );
            WriteInt( property.Field3C );
        }

        private void WriteMaterialPropertyType4( uint version, MaterialPropertyType4 property )
        {
            WriteVector4( property.Field0C );
            WriteFloat( property.Field1C );
            WriteFloat( property.Field20 );
            WriteVector4( property.Field24 );
            WriteFloat( property.Field34 );
            WriteFloat( property.Field38 );
            WriteFloat( property.Field3C );
            WriteFloat( property.Field40 );
            WriteFloat( property.Field44 );
            WriteFloat( property.Field48 );
            WriteFloat( property.Field4C );
            WriteByte( property.Field50 );
            WriteFloat( property.Field54 );
            WriteFloat( property.Field58 );
            WriteInt( property.Field5C );
        }

        private void WriteMaterialPropertyType5( uint version, MaterialPropertyType5 property )
        {
            WriteInt( property.Field0C );
            WriteInt( property.Field10 );
            WriteFloat( property.Field14 );
            WriteFloat( property.Field18 );
            WriteVector4( property.Field1C );
            WriteFloat( property.Field2C );
            WriteFloat( property.Field30 );
            WriteFloat( property.Field34 );
            WriteFloat( property.Field38 );
            WriteFloat( property.Field3C );
            WriteVector4( property.Field48 );
        }

        private void WriteMaterialPropertyType6( uint version, MaterialPropertyType6 property )
        {
            WriteInt( property.Field0C );
            WriteInt( property.Field10 );
            WriteInt( property.Field14 );
        }

        private void WriteMaterialPropertyType7( uint version, MaterialPropertyType7 property )
        {
        }

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
                //case NodeAttachmentType.Camera:
                //    WriteCamera( version, attachment.GetValue<Camera>() );
                //    break;
                //case NodeAttachmentType.Light:
                //    WriteLight( version, attachment.GetValue<Light>() );
                //    break;
                //case NodeAttachmentType.Epl:
                //    WriteEpl( version, attachment.GetValue<Epl>() );
                //    break;
                //case NodeAttachmentType.EplLeaf:
                //    WriteEplLeafs version, attachment.GetValue<EplLeaf>() );
                //    break;
                //case NodeAttachmentType.Morph:
                //    WriteMorph( version, attachment.GetValue<Morph>() );
                //    break;
                default:
                    throw new Exception( $"Unknown node attachment: {attachment.Type}" );
            }
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
                            WriteInt( value.Length );
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

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                for ( int i = 0; i < geometry.Triangles.Length; i++ )
                {
                    for ( int j = 0; j < geometry.Triangles[i].Indices.Length; j++ )
                    {
                        switch ( geometry.TriangleIndexType )
                        {
                            case TriangleIndexType.UInt16:
                                WriteUShort( (ushort)geometry.Triangles[i].Indices[j] );
                                break;
                            case TriangleIndexType.UInt32:
                                WriteInt( geometry.Triangles[i].Indices[j] );
                                break;
                            default:
                                throw new Exception( $"Unsupported triangle index type: {geometry.TriangleIndexType}" );
                        }
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

        private void WriteShaderCache( ShaderCache resource )
        {
            throw new NotImplementedException();
        }
    }
}
