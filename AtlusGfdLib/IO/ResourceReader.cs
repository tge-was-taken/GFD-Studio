using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using AtlusGfdLib.IO;
using AtlusGfdLib.Loaders;

namespace AtlusGfdLib
{
    internal class ResourceReader : IDisposable
    {
        private EndianBinaryReader mReader;

        public ResourceReader( Stream stream, Endianness endianness )
        {
            mReader = new EndianBinaryReader( stream, endianness );
        }

        public static Resource ReadFromStream( Stream stream, Endianness endianness )
        {
            using ( var reader = new ResourceReader( stream, endianness ) )
                return reader.ReadResourceFile();
        }

        public static TResource ReadFromStream<TResource>( Stream stream, Endianness endianness )
            where TResource : Resource
        {
            return ( TResource )ReadFromStream( stream, endianness );
        }

        // IDispose implementation
        public void Dispose()
        {
            ( ( IDisposable )mReader ).Dispose();
        }

        // Resource read methods
        private Resource ReadResourceFile()
        {
            if ( !ReadFileHeader( out FileHeader header ) || header.Magic != FileHeader.CMAGIC_FS )
                return null;

            // Read resource depending on type
            Resource resource;

            switch ( header.Type )
            {
                case FileType.Model:
                    resource = ReadModel( header.Version );
                    break;

                case FileType.ShaderCache:
                    resource = ReadShaderCache( header.Version );
                    break;

                default:
                    throw new NotSupportedException();
            }

            return resource;
        }

        // Resource read methods
        private Model ReadModel( uint version )
        {
            var model = new Model( version );

            while ( ReadChunkHeader( out ChunkHeader header ) && header.Type != 0 )
            {
                switch ( header.Type )
                {
                    case ChunkType.TextureDictionary:
                        model.TextureDictionary = ReadTextureDictionary( header.Version );
                        break;
                    case ChunkType.MaterialDictionary:
                        model.MaterialDictionary = ReadMaterialDictionary( header.Version );
                        break;
                    case ChunkType.Scene:
                        model.Scene = ReadScene( header.Version );
                        break;
                    case ChunkType.AnimationPackage:
                        model.AnimationPackage = ReadAnimationPackage( header.Version );
                        break;

                    default:
                        Debug.WriteLine( $"{GetMethodName()}: Unknown chunk type '{header.Type}' at offset 0x{mReader.Position.ToString( "X" )}" );
                        mReader.SeekCurrent( header.Size - 12 );
                        continue;
                }
            }

            return model;
        }

        // Shader read methods
        private ShaderCache ReadShaderCache( uint version )
        {
            if ( !ReadFileHeader( out FileHeader header ) || header.Magic != FileHeader.CMAGIC_SHADERCACHE )
                return null;

            // Read shaders into the shader cache
            ShaderCache cache = new ShaderCache( header.Version );
            while ( mReader.Position != mReader.BaseStreamLength )
            {
                ushort type = mReader.ReadUInt16();
                uint size = mReader.ReadUInt32();
                ushort field06 = mReader.ReadUInt16();
                uint field08 = mReader.ReadUInt32();
                uint field0C = mReader.ReadUInt32();
                uint field10 = mReader.ReadUInt32();
                float field14 = mReader.ReadSingle();
                float field18 = mReader.ReadSingle();
                byte[] data = mReader.ReadBytes( ( int )size );

                cache.Add( new Shader( type, field06, field08, field0C, field10, field14, field18, data ) );
            }

            return cache;
        }

        // Debug methods
        private string GetMethodName([CallerMemberName]string name = null)
        {
            return $"{nameof(ResourceReader)}.{name}";
        }

        // Primitive read methods
        private byte ReadByte()
        {
            return mReader.ReadByte();
        }

        private byte[] ReadBytes( int count )
        {
            return mReader.ReadBytes( count );
        }

        private short ReadShort()
        {
            return mReader.ReadInt16();
        }

        private ushort ReadUShort()
        {
            return mReader.ReadUInt16();
        }

        private int ReadInt()
        {
            return mReader.ReadInt32();
        }

        private uint ReadUInt()
        {
            return mReader.ReadUInt32();
        }

        private float ReadFloat()
        {
            return mReader.ReadSingle();
        }

        private Vector2 ReadVector2()
        {
            Vector2 value;
            value.X = ReadFloat();
            value.Y = ReadFloat();
            return value;
        }

        private Vector3 ReadVector3()
        {
            Vector3 value;
            value.X = ReadFloat();
            value.Y = ReadFloat();
            value.Z = ReadFloat();
            return value;
        }

        private ByteVector3 ReadByteVector3()
        {
            ByteVector3 value;
            value.X = ReadByte();
            value.Y = ReadByte();
            value.Z = ReadByte();
            return value;
        }

        private Vector4 ReadVector4()
        {
            Vector4 value;
            value.X = ReadFloat();
            value.Y = ReadFloat();
            value.Z = ReadFloat();
            value.W = ReadFloat();
            return value;
        }

        private ByteVector4 ReadByteVector4()
        {
            ByteVector4 value;
            value.X = ReadByte();
            value.Y = ReadByte();
            value.Z = ReadByte();
            value.W = ReadByte();
            return value;
        }

        private Quaternion ReadQuaternion()
        {
            Quaternion value;
            value.X = ReadFloat();
            value.Y = ReadFloat();
            value.Z = ReadFloat();
            value.W = ReadFloat();
            return value;
        }

        private Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 value;
            value.M11 = ReadFloat();
            value.M12 = ReadFloat();
            value.M13 = ReadFloat();
            value.M14 = ReadFloat();
            value.M21 = ReadFloat();
            value.M22 = ReadFloat();
            value.M23 = ReadFloat();
            value.M24 = ReadFloat();
            value.M31 = ReadFloat();
            value.M32 = ReadFloat();
            value.M33 = ReadFloat();
            value.M34 = ReadFloat();
            value.M41 = ReadFloat();
            value.M42 = ReadFloat();
            value.M43 = ReadFloat();
            value.M44 = ReadFloat();
            return value;
        }

        private BoundingBox ReadBoundingBox()
        {
            BoundingBox value;
            value.Min = ReadVector3();
            value.Max = ReadVector3();
            return value;
        }

        private BoundingSphere ReadBoundingSphere()
        {
            BoundingSphere value;
            value.Center = ReadVector3();
            value.Radius = ReadFloat();
            return value;
        }

        private string ReadString()
        {
            ushort length = mReader.ReadUInt16();
            return mReader.ReadString( StringBinaryFormat.FixedLength, length );
        }

        private string ReadString( int length )
        {
            return mReader.ReadString( StringBinaryFormat.FixedLength, length );
        }

        private string ReadStringWithHash( uint version )
        {
            var str = ReadString();

            if ( version > 0x1080000 )
            {
                uint hash = mReader.ReadUInt32();
                // if ( version <= 0x1104990 )
                // {
                //    hash = StringHasher.GenerateStringHash(str);
                // }
            }

            return str;
        }

        // Header reading methods
        private bool ReadFileHeader( out FileHeader header )
        {
            if ( mReader.Position >= mReader.BaseStreamLength ||
                mReader.Position + FileHeader.CSIZE > mReader.BaseStreamLength )
            {
                header = new FileHeader();
                return false;
            }

            header = new FileHeader()
            {
                Magic = mReader.ReadString( StringBinaryFormat.FixedLength, 4 ),
                Version = mReader.ReadUInt32(),
                Type = (FileType)mReader.ReadInt32(),
                Unknown = mReader.ReadInt32(),
            };

            Debug.WriteLine( $"{GetMethodName()}: {header.Magic} ver: {header.Version:X4} type: {header.Type} unk: {header.Unknown}" );

            return true;
        }

        private bool ReadChunkHeader( out ChunkHeader header )
        {
            if ( mReader.Position >= mReader.BaseStreamLength ||
                mReader.Position + ChunkHeader.CSIZE > mReader.BaseStreamLength )
            {
                header = new ChunkHeader();
                return false;
            }

            header = new ChunkHeader()
            {
                Version = mReader.ReadUInt32(),
                Type = ( ChunkType )mReader.ReadInt32(),
                Size = mReader.ReadInt32(),
                Unknown = mReader.ReadInt32()
            };

            Debug.WriteLine( $"{GetMethodName()}: {header.Type} ver: {header.Version:X4} size: {header.Size}" );

            return true;
        }

        // Texture read methods
        private TextureDictionary ReadTextureDictionary( uint version )
        {
            var textureDictionary = new TextureDictionary( version );

            int textureCount = mReader.ReadInt32();
            for (int i = 0; i < textureCount; i++)
            {
                var texture = ReadTexture();
                textureDictionary.Add( texture );
            }

            return textureDictionary;
        }

        private Texture ReadTexture()
        {
            var texture = new Texture();
            texture.Name = ReadString();
            texture.Format = ( TextureFormat )ReadShort();
            if ( !Enum.IsDefined( typeof( TextureFormat ), texture.Format ) )
                Debug.WriteLine( $"{GetMethodName()}: unknown texture format '{( int )texture.Format}'" );

            int size = ReadInt();
            texture.Data = ReadBytes( size );
            texture.Field1C = ReadByte();
            texture.Field1D = ReadByte();
            texture.Field1E = ReadByte();
            texture.Field1F = ReadByte();

            return texture;
        }

        // Material read methods
        private MaterialDictionary ReadMaterialDictionary( uint version )
        {
            MaterialDictionary materialDictionary = new MaterialDictionary( version );

            int materialCount = ReadInt();
            for (int i = 0; i < materialCount; i++)
            {
                var material = ReadMaterial( version );

                materialDictionary.Add( material );
            }

            return materialDictionary;
        }

        private Material ReadMaterial( uint version )
        {
            var material = new Material();

            // Read material header
            material.Name = ReadStringWithHash( version );
            material.Flags = ( MaterialFlags )ReadUInt();

            if ( version < 0x1104000 )
            {
                material.Flags = ( MaterialFlags )( ( uint )material.Flags & 0x7FFFFFFF );
            }

            material.Ambient = ReadVector4();
            material.Diffuse = ReadVector4();
            material.Specular = ReadVector4();
            material.Emissive = ReadVector4();
            material.Field40 = ReadFloat();
            material.Field44 = ReadFloat();

            if ( version <= 0x1103040 )
            {
                material.Field48 = ( byte )ReadShort();
                material.Field49 = ( byte )ReadShort();
                material.Field4A = ( byte )ReadShort();
                material.Field4B = ( byte )ReadShort();
                material.Field4C = ( byte )ReadShort();

                if (version > 0x108011b )
                {
                    material.Field4D = ( byte )ReadShort();
                }
            }
            else
            {
                material.Field48 = ReadByte();
                material.Field49 = ReadByte();
                material.Field4A = ReadByte();
                material.Field4B = ReadByte();
                material.Field4C = ReadByte();
                material.Field4D = ReadByte();
            }

            material.Field90 = ReadShort();
            material.Field92 = ReadShort();

            if (version <= 0x1104800 )
            {
                material.Field94 = 1;
                material.Field96 = ( short )ReadInt();
            }
            else
            {
                material.Field94 = ReadShort();
                material.Field96 = ReadShort();
            }

            material.Field5C = ReadShort();
            material.Field6C = ReadInt();
            material.Field70 = ReadInt();
            material.Field50 = ReadShort();

            if ( version <= 0x1105070 )
            {
                material.Field98 = ReadInt();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                material.DiffuseMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                material.NormalMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                material.SpecularMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                material.ReflectionMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                material.HighlightMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                material.GlowMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                material.NightMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                material.DetailMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasShadowMap ) ) 
            {
                material.ShadowMap = ReadTextureMap( version );
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasProperties ) ) 
            {
                material.Properties = ReadMaterialProperties( version );
            }

            return material;
        }

        private TextureMap ReadTextureMap( uint version )
        {
            return new TextureMap()
            {
                Name = ReadStringWithHash( version ),
                Field44 = ReadInt(),
                Field48 = ReadByte(),
                Field49 = ReadByte(),
                Field4A = ReadByte(),
                Field4B = ReadByte(),
                Field4C = ReadFloat(),
                Field50 = ReadFloat(),
                Field54 = ReadFloat(),
                Field58 = ReadFloat(),
                Field5C = ReadFloat(),
                Field60 = ReadFloat(),
                Field64 = ReadFloat(),
                Field68 = ReadFloat(),
                Field6C = ReadFloat(),
                Field70 = ReadFloat(),
                Field74 = ReadFloat(),
                Field78 = ReadFloat(),
                Field7C = ReadFloat(),
                Field80 = ReadFloat(),
                Field84 = ReadFloat(),
                Field88 = ReadFloat(),
            };
        }

        private List<MaterialProperty> ReadMaterialProperties( uint version )
        {
            var properties = new List<MaterialProperty>();
            int propertyCount = ReadInt();

            for ( int i = 0; i < propertyCount; i++ )
            {
                MaterialProperty property;             
                uint flags = ReadUInt();

                switch ( (MaterialPropertyType)(flags & 0xFFFF) )
                {
                    case MaterialPropertyType.Type0:
                        property = ReadMaterialPropertyType0( flags, version );
                        break;

                    case MaterialPropertyType.Type1:
                        property = ReadMaterialPropertyType1( flags, version );
                        break;

                    case MaterialPropertyType.Type2:
                        property = ReadMaterialPropertyType2( flags, version );
                        break;

                    case MaterialPropertyType.Type3:
                        property = ReadMaterialPropertyType3( flags, version );
                        break;

                    case MaterialPropertyType.Type4:
                        property = ReadMaterialPropertyType4( flags, version );
                        break;

                    case MaterialPropertyType.Type5:
                        property = ReadMaterialPropertyType5( flags, version );
                        break;

                    case MaterialPropertyType.Type6:
                        property = ReadMaterialPropertyType6( flags, version );
                        break;

                    case MaterialPropertyType.Type7:
                        property = ReadMaterialPropertyType7( flags, version );
                        break;

                    default:
                        throw new Exception();
                }

                properties.Add( property );
            }

            return properties;
        }

        private MaterialPropertyType0 ReadMaterialPropertyType0( uint flags, uint version )
        {
            var property = new MaterialPropertyType0( flags );

            if ( version > 0x1104500 )
            {
                property.Field0C = ReadVector4();
                property.Field1C = ReadFloat();
                property.Field20 = ReadFloat();
                property.Field24 = ReadFloat();
                property.Field28 = ReadFloat();
                property.Field2C = ReadFloat();
                property.Field30 = ReadInt();
            }
            else if ( version > 0x1104220 )
            {
                property.Field0C = ReadVector4();
                property.Field1C = ReadFloat();
                property.Field20 = ReadFloat();
                property.Field24 = ReadFloat();
                property.Field28 = ReadFloat();
                property.Field2C = ReadFloat();

                if ( ReadByte() != 0 )
                    property.Field30 |= 1;

                if ( ReadByte() != 0 )
                    property.Field30 |= 2;

                if ( ReadByte() != 0 )
                    property.Field30 |= 4;

                if ( version > 0x1104260 )
                {
                    if ( ReadByte() != 0 )
                        property.Field30 |= 8;
                }
            }
            else
            {
                property.Field0C = ReadVector4();
                property.Field1C = ReadFloat();
                property.Field20 = ReadFloat();
                property.Field24 = 1.0f;
                property.Field28 = ReadFloat();
                property.Field2C = ReadFloat();
            }

            return property;
        }

        private MaterialPropertyType1 ReadMaterialPropertyType1( uint flags, uint version )
        {
            var property = new MaterialPropertyType1( flags );

            property.Field0C = ReadVector4();
            property.Field1C = ReadFloat();
            property.Field20 = ReadFloat();
            property.Field24 = ReadVector4();
            property.Field34 = ReadFloat();
            property.Field38 = ReadFloat();

            if ( version <= 0x1104500 )
            {
                if ( ReadByte() != 0 )
                    property.Field3C |= 1;

                if ( version > 0x1104180 )
                {
                    if ( ReadByte() != 0 )
                        property.Field3C |= 2;
                }

                if ( version > 0x1104210 )
                {
                    if ( ReadByte() != 0 )
                        property.Field3C |= 4;
                }

                if ( version > 0x1104400 )
                {
                    if ( ReadByte() != 0 )
                        property.Field3C |= 8;
                }
            }
            else
            {
                property.Field3C = ReadInt();
            }

            return property;
        }

        private MaterialPropertyType2 ReadMaterialPropertyType2( uint flags, uint version )
        {
            var property = new MaterialPropertyType2( flags );

            property.Field0C = ReadInt();
            property.Field10 = ReadInt();

            return property;
        }

        private MaterialPropertyType3 ReadMaterialPropertyType3( uint flags, uint version )
        {
            var property = new MaterialPropertyType3( flags );

            property.Field0C = ReadFloat();
            property.Field10 = ReadFloat();
            property.Field14 = ReadFloat();
            property.Field18 = ReadFloat();
            property.Field1C = ReadFloat();
            property.Field20 = ReadFloat();
            property.Field24 = ReadFloat();
            property.Field28 = ReadFloat();
            property.Field2C = ReadFloat();
            property.Field30 = ReadFloat();
            property.Field34 = ReadFloat();
            property.Field38 = ReadFloat();
            property.Field3C = ReadInt();

            return property;
        }

        private MaterialPropertyType4 ReadMaterialPropertyType4( uint flags, uint version )
        {
            var property = new MaterialPropertyType4( flags );

            property.Field0C = ReadVector4();
            property.Field1C = ReadFloat();
            property.Field20 = ReadFloat();
            property.Field24 = ReadVector4();
            property.Field34 = ReadFloat();
            property.Field38 = ReadFloat();
            property.Field3C = ReadFloat();
            property.Field40 = ReadFloat();
            property.Field44 = ReadFloat();
            property.Field48 = ReadFloat();
            property.Field4C = ReadFloat();
            property.Field50 = ReadByte();
            property.Field54 = ReadFloat();
            property.Field58 = ReadFloat();
            property.Field5C = ReadInt();

            return property;
        }

        private MaterialPropertyType5 ReadMaterialPropertyType5( uint flags, uint version )
        {
            var property = new MaterialPropertyType5( flags );

            property.Field0C = ReadInt();
            property.Field10 = ReadInt();
            property.Field14 = ReadFloat();
            property.Field18 = ReadFloat();
            property.Field1C = ReadVector4();
            property.Field2C = ReadFloat();
            property.Field30 = ReadFloat();
            property.Field34 = ReadFloat();
            property.Field38 = ReadFloat();
            property.Field3C = ReadFloat();
            property.Field48 = ReadVector4();

            return property;
        }

        private MaterialPropertyType6 ReadMaterialPropertyType6( uint flags, uint version )
        {
            var property = new MaterialPropertyType6( flags );

            property.Field0C = ReadInt();
            property.Field10 = ReadInt();
            property.Field14 = ReadInt();

            return property;
        }

        private MaterialPropertyType7 ReadMaterialPropertyType7( uint flags, uint version )
        {
            var property = new MaterialPropertyType7( flags );

            return property;
        }

        // Scene read methods
        private Scene ReadScene( uint version )
        {
            Scene scene = new Scene( version );
            scene.Flags = ( SceneFlags )ReadInt();

            if ( scene.Flags.HasFlag( SceneFlags.HasSkinning ) ) 
                scene.MatrixMap = ReadInverseBindPoseMatrixMap();

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingBox ) )
                scene.BoundingBox = ReadBoundingBox();

            if ( scene.Flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                scene.BoundingSphere = ReadBoundingSphere();

            scene.RootNode = ReadNodeRecursive( version );

            return scene;
        }

        private MatrixMap ReadInverseBindPoseMatrixMap()
        {
            int matrixCount = ReadInt();
            var map = new MatrixMap( matrixCount );

            for ( int i = 0; i < map.Matrices.Length; i++ )
                map.Matrices[i] = ReadMatrix4x4();

            for ( int i = 0; i < map.RemapIndices.Length; i++ )
                map.RemapIndices[i] = ReadUShort();

            return map;
        }

        private Node ReadNodeRecursive( uint version )
        {
            var node = ReadNode( version );
            int childCount = ReadInt();

            for ( int i = 0; i < childCount; i++ )
            {
                var childNode = ReadNodeRecursive( version );
                node.AddChildNode( childNode );
            }

            return node;
        }

        private Node ReadNode( uint version )
        {
            Node node = new Node();
            node.Name = ReadStringWithHash( version );
            node.Position = ReadVector3();
            node.Rotation = ReadQuaternion();
            node.Scale = ReadVector3();

            if ( version <= 0x1090000 )
                ReadByte();

            int attachmentCount = ReadInt();
            for ( int i = 0; i < attachmentCount; i++ )
            {
                var attachment = ReadNodeAttachment( version );
                node.Attachments.Add( attachment );
            }

            if ( version > 0x1060000 )
            {
                bool hasProperties = ReadByte() == 1;
                if ( hasProperties )
                {
                    node.Properties = ReadNodeProperties( version );
                }
            }

            if ( version > 0x1104230 )
            {
                node.FieldE0 = ReadFloat();
            }

            return node;
        }

        private NodeAttachment ReadNodeAttachment( uint version )
        {
            NodeAttachmentType type = ( NodeAttachmentType )ReadInt();

            switch ( type )
            {
                case NodeAttachmentType.Invalid:
                case NodeAttachmentType.Scene:
                    throw new NotSupportedException();

                //case NodeAttachmentType.Mesh:
                //  return new NodeMeshAttachment( ReadMesh( version ) );
                case NodeAttachmentType.Node:
                    return new NodeNodeAttachment( ReadNode( version ) );
                case NodeAttachmentType.Geometry:
                    return new NodeGeometryAttachment( ReadGeometry( version ) );
                //case NodeAttachmentType.Camera:
                //    return new NodeCameraAttachment( ReadCamera( version ) );
                //case NodeAttachmentType.Light:
                //    return new NodeLightAttachment( ReadLight( version ) );
                //case NodeAttachmentType.Epl:
                //    return new NodeEplAttachment( ReadEpl( version ) );
                //case NodeAttachmentType.EplLeaf:
                //    return new NodeEplLeafAttachment( ReadEplLeaf( version ) );
                //case NodeAttachmentType.Morph:
                //    return new NodeMorphAttachment( ReadMorph( version ) );
                default:
                    throw new Exception( $"Unknown node attachment: {type}" );
            }
        }

        private Dictionary<string, NodeProperty> ReadNodeProperties( uint version )
        {
            int propertyCount = ReadInt();
            var properties = new Dictionary<string, NodeProperty>( propertyCount );

            for ( int i = 0; i < propertyCount; i++ )
            {
                var type = ( PropertyValueType )ReadInt();
                var name = ReadStringWithHash( version );
                var size = ReadInt();
                NodeProperty property;

                switch ( type )
                {
                    case PropertyValueType.Int:
                        property = new NodeIntProperty( name, ReadInt() );
                        break;
                    case PropertyValueType.Float:
                        property = new NodeFloatProperty( name, ReadFloat() );
                        break;
                    case PropertyValueType.Bool:
                        property = new NodeBoolProperty( name, ReadByte() == 1 );
                        break;
                    case PropertyValueType.String:
                        property = new NodeStringProperty( name, ReadString( size ) );
                        break;
                    case PropertyValueType.ByteVector3:
                        property = new NodeByteVector3Property( name, ReadByteVector3() );
                        break;
                    case PropertyValueType.ByteVector4:
                        property = new NodeByteVector4Property( name, ReadByteVector4() );
                        break;
                    case PropertyValueType.Vector3:
                        property = new NodeVector3Property( name, ReadVector3() );
                        break;
                    case PropertyValueType.Vector4:
                        property = new NodeVector4Property( name, ReadVector4() );
                        break;
                    case PropertyValueType.ByteArray:
                        property = new NodeByteArrayProperty( name, ReadBytes( size ) );
                        break;
                    default:
                        throw new Exception( $"Unknown node property type: {type}" );
                }

                properties[property.Name] = property;
            }

            return properties;
        }

        private Geometry ReadGeometry( uint version )
        {
            var geometry = new Geometry();

            geometry.Flags = ( GeometryFlags )ReadInt();
            geometry.VertexAttributeFlags = ( VertexAttributeFlags )ReadInt();

            Debug.WriteLine( geometry.VertexAttributeFlags );
            int triangleCount = 0;

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) ) 
            {
                triangleCount = ReadInt();
                geometry.TriangleIndexType = ( TriangleIndexType )ReadShort();
                geometry.Triangles = new Triangle[triangleCount];
            }

            int vertexCount = ReadInt();

            if ( version > 0x1103020 )
            {
                geometry.Field14 = ReadInt();
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                geometry.Vertices = new Vector3[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ))
                geometry.Normals = new Vector3[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ))
                geometry.Tangents = new Vector3[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ))
                geometry.Binormals = new Vector3[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ))
                geometry.ColorChannel0 = new uint[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ))
                geometry.TexCoordsChannel0 = new Vector2[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                geometry.TexCoordsChannel1 = new Vector2[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                geometry.TexCoordsChannel2 = new Vector2[vertexCount];

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                geometry.ColorChannel1 = new uint[vertexCount];

            if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                geometry.VertexWeights = new VertexWeight[vertexCount];

            for ( int i = 0; i < vertexCount; i++ )
            {
                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                    geometry.Vertices[i] = ReadVector3();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
                    geometry.Normals[i] = ReadVector3();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
                    geometry.Tangents[i] = ReadVector3();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
                    geometry.Binormals[i] = ReadVector3();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ))
                    geometry.ColorChannel0[i] = ReadUInt();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ))
                    geometry.TexCoordsChannel0[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                    geometry.TexCoordsChannel1[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                    geometry.TexCoordsChannel2[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                    geometry.ColorChannel1[i] = ReadUInt();

                if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                {
                    geometry.VertexWeights[i].Weights = new float[4];

                    for ( int j = 0; j < 4; j++ )
                    {
                        geometry.VertexWeights[i].Weights[j] = ReadFloat();
                    }

                    geometry.VertexWeights[i].Indices = new byte[4];

                    for ( int j = 0; j < 4; j++ )
                    {
                        geometry.VertexWeights[i].Indices[j] = ReadByte();
                    }
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                for ( int i = 0; i < geometry.Triangles.Length; i++ )
                {
                    geometry.Triangles[i].Indices = new uint[3];

                    for ( int j = 0; j < geometry.Triangles[i].Indices.Length; j++ )
                    {
                        uint index;
                        switch ( geometry.TriangleIndexType )
                        {
                            case TriangleIndexType.UInt16:
                                index = ReadUShort();
                                break;
                            case TriangleIndexType.UInt32:
                                index = ReadUInt();
                                break;
                            default:
                                throw new Exception( $"Unsupported triangle index type: {geometry.TriangleIndexType}" );
                        }

                        geometry.Triangles[i].Indices[j] = index;
                    }
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMaterial ) ) 
            {
                geometry.MaterialName = ReadStringWithHash( version );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingBox ) ) 
            {
                geometry.BoundingBox = ReadBoundingBox();
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingSphere ) )
            {
                geometry.BoundingSphere = ReadBoundingSphere();
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.Flag1000 ))
            {
                geometry.FieldD4 = ReadFloat();
                geometry.FieldD8 = ReadFloat();
            }

            return geometry;
        }

        // Animation read methods
        private AnimationPackage ReadAnimationPackage( uint version )
        {
            throw new NotImplementedException();
        }
    }
}
