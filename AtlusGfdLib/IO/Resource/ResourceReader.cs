using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using AtlusGfdLib.IO.Common;

namespace AtlusGfdLib.IO.Resource
{
    internal class ResourceReader : IDisposable
    {
        private static readonly Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
        private readonly EndianBinaryReader mReader;

        public ResourceReader( Stream stream, Endianness endianness, bool leaveOpen = false )
        {
            mReader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, endianness );
        }

        public static AtlusGfdLib.Resource ReadFromStream( Stream stream, Endianness endianness, bool leaveOpen = false )
        {
            using ( var reader = new ResourceReader( stream, endianness, leaveOpen ) )
                return reader.ReadResourceBundleFile();
        }

        public static TResource ReadFromStream<TResource>( Stream stream, Endianness endianness, bool leaveOpen = false )
            where TResource : AtlusGfdLib.Resource
        {
            return ( TResource )ReadFromStream( stream, endianness, leaveOpen );
        }

        public static bool IsValidResourceStream( Stream stream )
        {
            try
            {
                using ( var reader = new ResourceReader( stream, Endianness.BigEndian, true ) )
                {
                    if ( !reader.ReadFileHeader( out ResourceFileHeader header ) || header.Magic != ResourceFileHeader.MAGIC_FS )
                        return false;

                    return true;
                }
            }
            finally
            {
                stream.Position = 0;
            }
        }

        // IDispose implementation
        public void Dispose()
        {
            mReader.Dispose();
        }

        // Debug methods
        private string GetMethodName( [CallerMemberName]string name = null )
        {
            return $"{nameof( ResourceReader )}.{name}";
        }

        private void DebugLog( string what, [CallerMemberName]string name = null )
        {
            //Trace.TraceInformation( $"{name}: {what}" );
        }

        private void DebugLogPosition( string what, [CallerMemberName]string name = null )
        {
            //Trace.TraceInformation( $"{name}: {what} @ 0x{mReader.Position:X4}" );
        }

        #region Primitive read methods

        private byte ReadByte() => mReader.ReadByte();

        private bool ReadBool() => ReadByte() == 1;

        private byte[] ReadBytes( int count ) => mReader.ReadBytes( count );

        private short ReadShort() => mReader.ReadInt16();

        private ushort ReadUShort() => mReader.ReadUInt16();

        private int ReadInt() => mReader.ReadInt32();

        private uint ReadUInt() => mReader.ReadUInt32();

        private float ReadFloat() => mReader.ReadSingle();

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
            value.Max = ReadVector3();
            value.Min = ReadVector3();
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
            var bytes = ReadBytes( length );
            return sSJISEncoding.GetString( bytes );
        }

        private string ReadString( int length )
        {
            var bytes = ReadBytes( length );
            return sSJISEncoding.GetString( bytes );
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

        #endregion

        // Header reading methods
        private bool ReadFileHeader( out ResourceFileHeader header )
        {
            if ( mReader.Position >= mReader.BaseStreamLength ||
                mReader.Position + ResourceFileHeader.SIZE > mReader.BaseStreamLength )
            {
                header = new ResourceFileHeader();
                return false;
            }

            header = new ResourceFileHeader()
            {
                Magic = mReader.ReadString( StringBinaryFormat.FixedLength, 4 ),
                Version = mReader.ReadUInt32(),
                Type = ( ResourceFileType )mReader.ReadInt32(),
                Unknown = mReader.ReadInt32(),
            };

            DebugLog( $"{header.Magic} ver: {header.Version:X4} type: {header.Type} unk: {header.Unknown}" );

            return true;
        }

        private bool ReadChunkHeader( out ResourceChunkHeader header )
        {
            if ( mReader.Position >= mReader.BaseStreamLength ||
                mReader.Position + ResourceChunkHeader.SIZE > mReader.BaseStreamLength )
            {
                header = new ResourceChunkHeader();
                return false;
            }

            header = new ResourceChunkHeader()
            {
                Version = mReader.ReadUInt32(),
                Type = ( ResourceChunkType )mReader.ReadInt32(),
                Size = mReader.ReadInt32(),
                Unknown = mReader.ReadInt32()
            };

            DebugLog( $"{header.Type} ver: {header.Version:X4} size: {header.Size} unk: {header.Unknown}" );

            return true;
        }

        // Resource read methods
        private AtlusGfdLib.Resource ReadResourceBundleFile()
        {
            if ( !ReadFileHeader( out ResourceFileHeader header ) || header.Magic != ResourceFileHeader.MAGIC_FS )
                return null;

            // Read resource depending on type
            AtlusGfdLib.Resource resource;

            switch ( header.Type )
            {
                case ResourceFileType.ModelResourceBundle:
                    resource = ReadModel( header.Version );
                    break;

                case ResourceFileType.ShaderCachePS3:
                    resource = ReadShaderCachePS3( header.Version );
                    break;

                case ResourceFileType.ShaderCachePSP2:
                    resource = ReadShaderCachePSP2( header.Version );
                    break;

                // Custom resource types
                case ResourceFileType.CustomGenericResourceBundle:
                    resource = ReadResourceBundle( header.Version );
                    break;

                case ResourceFileType.CustomTextureDictionary:
                    resource = ReadTextureDictionary( header.Version );
                    break;
                case ResourceFileType.CustomTexture:
                    resource = ReadTexture();
                    break;
                case ResourceFileType.CustomMaterialDictionary:
                    resource = ReadMaterialDictionary( header.Version );
                    break;
                case ResourceFileType.CustomMaterial:
                    resource = ReadMaterial( header.Version );
                    break;
                case ResourceFileType.CustomTextureMap:
                    resource = ReadTextureMap( header.Version );
                    break;
                case ResourceFileType.CustomMaterialAttribute:
                    resource = ReadMaterialAttribute( header.Version );
                    break;

                default:
                    throw new NotSupportedException();
            }

            return resource;
        }

        // Model read methods
        private Model ReadModel( uint version )
        {
            var model = new Model( version );

            while ( ReadChunkHeader( out ResourceChunkHeader header ) && header.Type != 0 )
            {
                switch ( header.Type )
                {
                    case ResourceChunkType.TextureDictionary:
                        model.TextureDictionary = ReadTextureDictionary( header.Version );
                        break;
                    case ResourceChunkType.MaterialDictionary:
                        model.MaterialDictionary = ReadMaterialDictionary( header.Version );
                        break;
                    case ResourceChunkType.Scene:
                        model.Scene = ReadScene( header.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F9:
                        model.ChunkType000100F9 = ReadChunkType000100F9( header.Version );
                        break;
                    //case ChunkType.AnimationPackage:
                    //    model.AnimationPackage = ReadAnimationPackage( header.Version );
                    //    break;
                    default:
                        DebugLogPosition( $"Unknown chunk type '{header.Type}'" );
                        mReader.SeekCurrent( header.Size - 12 );
                        continue;
                }
            }

            return model;
        }

        // Texture read methods
        private TextureDictionary ReadTextureDictionary( uint version )
        {
            var textureDictionary = new TextureDictionary( version );

            int textureCount = ReadInt();
            for ( int i = 0; i < textureCount; i++ )
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
                DebugLogPosition( $"unknown texture format '{( int )texture.Format}'" );

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
            for ( int i = 0; i < materialCount; i++ )
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
            var flags = ( MaterialFlags )ReadUInt();

            if ( version < 0x1104000 )
            {
                flags = ( MaterialFlags )( ( uint )material.Flags & 0x7FFFFFFF );
            }

            material.Ambient = ReadVector4();
            material.Diffuse = ReadVector4();
            material.Specular = ReadVector4();
            material.Emissive = ReadVector4();
            material.Field40 = ReadFloat();
            material.Field44 = ReadFloat();

            if ( version <= 0x1103040 )
            {
                material.DrawOrder = ( MaterialDrawOrder )ReadShort();
                material.Field49 = ( byte )ReadShort();
                material.Field4A = ( byte )ReadShort();
                material.Field4B = ( byte )ReadShort();
                material.Field4C = ( byte )ReadShort();

                if ( version > 0x108011b )
                {
                    material.Field4D = ( byte )ReadShort();
                }
            }
            else
            {
                material.DrawOrder = ( MaterialDrawOrder )ReadByte();
                material.Field49 = ReadByte();
                material.Field4A = ReadByte();
                material.Field4B = ReadByte();
                material.Field4C = ReadByte();
                material.Field4D = ReadByte();
            }

            material.Field90 = ReadShort();
            material.Field92 = ReadShort();

            if ( version <= 0x1104800 )
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
            material.Field6C = ReadUInt();
            material.Field70 = ReadUInt();
            material.Field50 = ReadShort();

            if ( version <= 0x1105070 )
            {
                material.Field98 = ReadUInt();
            }

            if ( flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                material.DiffuseMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                material.NormalMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                material.SpecularMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                material.ReflectionMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                material.HighlightMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                material.GlowMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                material.NightMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                material.DetailMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                material.ShadowMap = ReadTextureMap( version );
            }

            if ( flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                material.Attributes = ReadMaterialAttributes( version );
            }

            material.Flags = flags;
            if ( material.Flags != flags )
                throw new Exception( "Material flags don't match flags from file" );

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

        #region Material attribute read methods
        private List<MaterialAttribute> ReadMaterialAttributes( uint version )
        {
            var attributes = new List<MaterialAttribute>();
            int attributeCount = ReadInt();

            for ( int i = 0; i < attributeCount; i++ )
            {
                var attribute = ReadMaterialAttribute( version );
                attributes.Add( attribute );
            }

            return attributes;
        }

        private MaterialAttribute ReadMaterialAttribute( uint version )
        {
            MaterialAttribute attribute;
            uint flags = ReadUInt();

            DebugLogPosition( $"Material attribute {( MaterialAttributeType )( flags & 0xFFFF )}" );

            switch ( ( MaterialAttributeType )( flags & 0xFFFF ) )
            {
                case MaterialAttributeType.Type0:
                    attribute = ReadMaterialAttributeType0( flags, version );
                    break;

                case MaterialAttributeType.Type1:
                    attribute = ReadMaterialAttributeType1( flags, version );
                    break;

                case MaterialAttributeType.Type2:
                    attribute = ReadMaterialAttributeType2( flags );
                    break;

                case MaterialAttributeType.Type3:
                    attribute = ReadMaterialAttributeType3( flags );
                    break;

                case MaterialAttributeType.Type4:
                    attribute = ReadMaterialAttributeType4( flags );
                    break;

                case MaterialAttributeType.Type5:
                    attribute = ReadMaterialAttributeType5( flags );
                    break;

                case MaterialAttributeType.Type6:
                    attribute = ReadMaterialAttributeType6( flags );
                    break;

                case MaterialAttributeType.Type7:
                    attribute = ReadMaterialAttributeType7( flags );
                    break;

                default:
                    throw new Exception();
            }

            return attribute;
        }

        private MaterialAttributeType0 ReadMaterialAttributeType0( uint flags, uint version )
        {
            var attribute = new MaterialAttributeType0( flags );

            if ( version > 0x1104500 )
            {
                attribute.Field0C = ReadVector4();
                attribute.Field1C = ReadFloat();
                attribute.Field20 = ReadFloat();
                attribute.Field24 = ReadFloat();
                attribute.Field28 = ReadFloat();
                attribute.Field2C = ReadFloat();
                attribute.Type0Flags = ( MaterialAttributeType0Flags )ReadInt();
            }
            else if ( version > 0x1104220 )
            {
                attribute.Field0C = ReadVector4();
                attribute.Field1C = ReadFloat();
                attribute.Field20 = ReadFloat();
                attribute.Field24 = ReadFloat();
                attribute.Field28 = ReadFloat();
                attribute.Field2C = ReadFloat();

                if ( ReadBool() )
                    attribute.Type0Flags |= MaterialAttributeType0Flags.Flag1;

                if ( ReadBool() )
                    attribute.Type0Flags |= MaterialAttributeType0Flags.Flag2;

                if ( ReadBool() )
                    attribute.Type0Flags |= MaterialAttributeType0Flags.Flag4;

                if ( version > 0x1104260 )
                {
                    if ( ReadBool() )
                        attribute.Type0Flags |= MaterialAttributeType0Flags.Flag8;
                }
            }
            else
            {
                attribute.Field0C = ReadVector4();
                attribute.Field1C = ReadFloat();
                attribute.Field20 = ReadFloat();
                attribute.Field24 = 1.0f;
                attribute.Field28 = ReadFloat();
                attribute.Field2C = ReadFloat();
            }

            return attribute;
        }

        private MaterialAttributeType1 ReadMaterialAttributeType1( uint flags, uint version )
        {
            var attribute = new MaterialAttributeType1( flags );

            attribute.Field0C = ReadVector4();
            attribute.Field1C = ReadFloat();
            attribute.Field20 = ReadFloat();
            attribute.Field24 = ReadVector4();
            attribute.Field34 = ReadFloat();
            attribute.Field38 = ReadFloat();

            if ( version <= 0x1104500 )
            {
                if ( ReadBool() )
                    attribute.Type1Flags |= MaterialAttributeType1Flags.Flag1;

                if ( version > 0x1104180 )
                {
                    if ( ReadBool() )
                        attribute.Type1Flags |= MaterialAttributeType1Flags.Flag2;
                }

                if ( version > 0x1104210 )
                {
                    if ( ReadBool() )
                        attribute.Type1Flags |= MaterialAttributeType1Flags.Flag4;
                }

                if ( version > 0x1104400 )
                {
                    if ( ReadBool() )
                        attribute.Type1Flags |= MaterialAttributeType1Flags.Flag8;
                }
            }
            else
            {
                attribute.Type1Flags = ( MaterialAttributeType1Flags )ReadInt();
            }

            return attribute;
        }

        private MaterialAttributeType2 ReadMaterialAttributeType2( uint flags )
        {
            var attribute = new MaterialAttributeType2( flags );

            attribute.Field0C = ReadInt();
            attribute.Field10 = ReadInt();

            return attribute;
        }

        private MaterialAttributeType3 ReadMaterialAttributeType3( uint flags )
        {
            var attribute = new MaterialAttributeType3( flags );

            attribute.Field0C = ReadFloat();
            attribute.Field10 = ReadFloat();
            attribute.Field14 = ReadFloat();
            attribute.Field18 = ReadFloat();
            attribute.Field1C = ReadFloat();
            attribute.Field20 = ReadFloat();
            attribute.Field24 = ReadFloat();
            attribute.Field28 = ReadFloat();
            attribute.Field2C = ReadFloat();
            attribute.Field30 = ReadFloat();
            attribute.Field34 = ReadFloat();
            attribute.Field38 = ReadFloat();
            attribute.Field3C = ReadInt();

            return attribute;
        }

        private MaterialAttributeType4 ReadMaterialAttributeType4( uint flags )
        {
            var attribute = new MaterialAttributeType4( flags );

            attribute.Field0C = ReadVector4();
            attribute.Field1C = ReadFloat();
            attribute.Field20 = ReadFloat();
            attribute.Field24 = ReadVector4();
            attribute.Field34 = ReadFloat();
            attribute.Field38 = ReadFloat();
            attribute.Field3C = ReadFloat();
            attribute.Field40 = ReadFloat();
            attribute.Field44 = ReadFloat();
            attribute.Field48 = ReadFloat();
            attribute.Field4C = ReadFloat();
            attribute.Field50 = ReadByte();
            attribute.Field54 = ReadFloat();
            attribute.Field58 = ReadFloat();
            attribute.Field5C = ReadInt();

            return attribute;
        }

        private MaterialAttributeType5 ReadMaterialAttributeType5( uint flags )
        {
            var attribute = new MaterialAttributeType5( flags );

            attribute.Field0C = ReadInt();
            attribute.Field10 = ReadInt();
            attribute.Field14 = ReadFloat();
            attribute.Field18 = ReadFloat();
            attribute.Field1C = ReadVector4();
            attribute.Field2C = ReadFloat();
            attribute.Field30 = ReadFloat();
            attribute.Field34 = ReadFloat();
            attribute.Field38 = ReadFloat();
            attribute.Field3C = ReadFloat();
            attribute.Field48 = ReadVector4();

            return attribute;
        }

        private MaterialAttributeType6 ReadMaterialAttributeType6( uint flags )
        {
            var attribute = new MaterialAttributeType6( flags );

            attribute.Field0C = ReadInt();
            attribute.Field10 = ReadInt();
            attribute.Field14 = ReadInt();

            return attribute;
        }

        private MaterialAttributeType7 ReadMaterialAttributeType7( uint flags )
        {
            var attribute = new MaterialAttributeType7( flags );

            return attribute;
        }

        #endregion

        #region Scene read methods
        private Scene ReadScene( uint version )
        {
            DebugLogPosition( $"Scene" );

            Scene scene = new Scene( version );
            var flags = ( SceneFlags )ReadInt();

            if ( flags.HasFlag( SceneFlags.HasSkinning ) )
                scene.MatrixPalette = ReadMatrixPalette();

            if ( flags.HasFlag( SceneFlags.HasBoundingBox ) )
                scene.BoundingBox = ReadBoundingBox();

            if ( flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                scene.BoundingSphere = ReadBoundingSphere();

            scene.RootNode = ReadNodeRecursive( version );
            scene.Flags = flags;

            return scene;
        }

        private MatrixPalette ReadMatrixPalette()
        {
            int matrixCount = ReadInt();
            var map = new MatrixPalette( matrixCount );

            for ( int i = 0; i < map.InverseBindMatrices.Length; i++ )
                map.InverseBindMatrices[i] = ReadMatrix4x4();

            for ( int i = 0; i < map.BoneToNodeIndices.Length; i++ )
                map.BoneToNodeIndices[i] = ReadUShort();

            return map;
        }

        private Node ReadNodeRecursive( uint version )
        {
            DebugLogPosition( "Node" );
            var node = ReadNode( version );

            int childCount = ReadInt();

            var childStack = new Stack<Node>();
            for ( int i = 0; i < childCount; i++ )
            {
                var childNode = ReadNodeRecursive( version );
                childStack.Push( childNode );
            }

            while ( childStack.Count > 0 )
            {
                node.AddChildNode( childStack.Pop() );
            }

            return node;
        }

        private Node ReadNode( uint version )
        {
            Node node = new Node
            {
                Name = ReadStringWithHash( version ),
                Translation = ReadVector3(),
                Rotation = ReadQuaternion(),
                Scale = ReadVector3()
            };

            DebugLog( $"{node.Name}" );

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
                var hasProperties = ReadBool();
                if ( hasProperties )
                {
                    node.Properties = ReadNodeProperties( version );
                }
            }

            if ( version > 0x1104230 )
            {
                node.FieldE0 = ReadFloat();
                DebugLog( $"FieldE0: {node.FieldE0}" );
            }

            return node;
        }

        private NodeAttachment ReadNodeAttachment( uint version )
        {
            NodeAttachmentType type = ( NodeAttachmentType )ReadInt();

            DebugLogPosition( $"{type}" );

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
                case NodeAttachmentType.Camera:
                    return new NodeCameraAttachment( ReadCamera( version ) );
                case NodeAttachmentType.Light:
                    return new NodeLightAttachment( ReadLight( version ) );
                //case NodeAttachmentType.Epl:
                //    return new NodeEplAttachment( ReadEpl( version ) );
                //case NodeAttachmentType.EplLeaf:
                //    return new NodeEplLeafAttachment( ReadEplLeaf( version ) );
                case NodeAttachmentType.Morph:
                    return new NodeMorphAttachment( ReadMorph( version ) );
                default:
                    throw new NotImplementedException( $"Unimplemented node attachment type: {type}" );
            }
        }

        private Morph ReadMorph( uint version )
        {
            Morph morph = new Morph();

            int morphTargetCount = ReadInt();

            morph.TargetInts = new int[morphTargetCount];
            for ( int i = 0; i < morph.TargetInts.Length; i++ )
                morph.TargetInts[i] = ReadInt();

            morph.MaterialName = ReadStringWithHash( version );

            return morph;
        }

        private Dictionary<string, NodeProperty> ReadNodeProperties( uint version )
        {
            int propertyCount = ReadInt();
            var properties = new Dictionary<string, NodeProperty>( propertyCount );

            for ( int i = 0; i < propertyCount; i++ )
            {
                DebugLogPosition( "Property" );
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
                        property = new NodeBoolProperty( name, ReadBool() );
                        break;
                    case PropertyValueType.String:
                        property = new NodeStringProperty( name, ReadString( size - 1 ) );
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

                DebugLog( $"{type} {name} {size} {property.GetValue()}" );

                properties[property.Name] = property;
            }

            return properties;
        }
        #endregion

        #region Geometry read methods

        private Geometry ReadGeometry( uint version )
        {
            var geometry = new Geometry();

            var flags = ( GeometryFlags )ReadInt();
            geometry.Flags = flags;
            geometry.VertexAttributeFlags = ( VertexAttributeFlags )ReadInt();

            DebugLog( $"{nameof( geometry.Flags )}: {flags}" );
            DebugLog( $"{nameof( geometry.VertexAttributeFlags )}: {geometry.VertexAttributeFlags.ToString()}" );

            int triangleCount = 0;

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                triangleCount = ReadInt();
                geometry.TriangleIndexType = ( TriangleIndexType )ReadShort();
                geometry.Triangles = new Triangle[triangleCount];

                DebugLog( $"{nameof( triangleCount )}: {triangleCount.ToString()}" );
                DebugLog( $"{nameof( geometry.TriangleIndexType )}: {geometry.TriangleIndexType}" );
            }

            int vertexCount = ReadInt();
            DebugLog( "vertexCount: " + vertexCount.ToString() );

            if ( version > 0x1103020 )
            {
                geometry.Field14 = ReadInt();
                DebugLog( $"{nameof( geometry.Field14 )}: {geometry.Field14.ToString()}" );
            }

            AllocateVertexBuffers( geometry, vertexCount );

            DebugLogPosition( $"vertexbuffer with {vertexCount} vertices" );
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

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
                    geometry.ColorChannel0[i] = ReadUInt();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
                    geometry.TexCoordsChannel0[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                    geometry.TexCoordsChannel1[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                    geometry.TexCoordsChannel2[i] = ReadVector2();

                if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                    geometry.ColorChannel1[i] = ReadUInt();

                if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                {
                    const int MAX_NUM_WEIGHTS = 4;
                    geometry.VertexWeights[i].Weights = new float[MAX_NUM_WEIGHTS];

                    for ( int j = 0; j < geometry.VertexWeights[i].Weights.Length; j++ )
                    {
                        geometry.VertexWeights[i].Weights[j] = ReadFloat();
                    }

                    geometry.VertexWeights[i].Indices = new byte[MAX_NUM_WEIGHTS];
                    uint indices = ReadUInt();

                    for ( int j = 0; j < geometry.VertexWeights[i].Indices.Length; j++ )
                    {
                        int shift = j * 8;
                        geometry.VertexWeights[i].Indices[j] = ( byte )( ( indices & ( 0xFF << shift ) ) >> shift );
                    }
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMorphTargets ) )
            {
                geometry.MorphTargets = ReadMorphTargetList();
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                DebugLogPosition( $"facebuffer with {triangleCount} vertices" );
                for ( int i = 0; i < geometry.Triangles.Length; i++ )
                {
                    switch ( geometry.TriangleIndexType )
                    {
                        case TriangleIndexType.UInt16:
                            geometry.Triangles[i].A = ReadUShort();
                            geometry.Triangles[i].B = ReadUShort();
                            geometry.Triangles[i].C = ReadUShort();
                            break;
                        case TriangleIndexType.UInt32:
                            geometry.Triangles[i].A = ReadUInt();
                            geometry.Triangles[i].B = ReadUInt();
                            geometry.Triangles[i].C = ReadUInt();
                            break;
                        default:
                            throw new Exception( $"Unsupported triangle index type: {geometry.TriangleIndexType}" );
                    }
                }
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasMaterial ) )
            {
                DebugLogPosition( "material name" );
                geometry.MaterialName = ReadStringWithHash( version );
                DebugLog( geometry.MaterialName );
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingBox ) )
            {
                DebugLogPosition( "bbox" );
                geometry.BoundingBox = ReadBoundingBox();
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasBoundingSphere ) )
            {
                DebugLogPosition( "bsphere" );
                geometry.BoundingSphere = ReadBoundingSphere();
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.Flag1000 ) )
            {
                DebugLogPosition( "flag1000 data" );
                geometry.FieldD4 = ReadFloat();
                geometry.FieldD8 = ReadFloat();
            }

            if ( geometry.Flags != flags )
                throw new Exception( "Geometry flags don't match flags in file" );

            return geometry;
        }

        private void AllocateVertexBuffers( Geometry geometry, int vertexCount )
        {
            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                geometry.Vertices = new Vector3[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
            {
                geometry.Normals = new Vector3[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
            {
                geometry.Tangents = new Vector3[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
            {
                geometry.Binormals = new Vector3[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
            {
                geometry.ColorChannel0 = new uint[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                geometry.TexCoordsChannel0 = new Vector2[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
            {
                geometry.TexCoordsChannel1 = new Vector2[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
            {
                geometry.TexCoordsChannel2 = new Vector2[vertexCount];
            }

            if ( geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
            {
                geometry.ColorChannel1 = new uint[vertexCount];
            }

            if ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
            {
                geometry.VertexWeights = new VertexWeight[vertexCount];
            }
        }

        private MorphTargetList ReadMorphTargetList()
        {
            var morphTargetList = new MorphTargetList();

            morphTargetList.Flags = ReadInt();
            int morphCount = ReadInt();

            DebugLogPosition( $"flags: {morphTargetList.Flags} {morphCount} morphs" );

            for ( int i = 0; i < morphCount; i++ )
            {
                var morphTarget = ReadMorphTarget();
                morphTargetList.Add( morphTarget );
            }

            return morphTargetList;
        }

        private MorphTarget ReadMorphTarget()
        {
            var morphTarget = new MorphTarget();

            morphTarget.Flags = ReadInt();
            int vertexCount = ReadInt();

            DebugLogPosition( $"flags: {morphTarget.Flags} {vertexCount} vertices" );

            for ( int j = 0; j < vertexCount; j++ )
            {
                var vertex = ReadVector3();
                morphTarget.Vertices.Add( vertex );
            }

            return morphTarget;
        }

        #endregion

        // Camera
        private Camera ReadCamera( uint version )
        {
            var camera = new Camera();
            camera.Transform = ReadMatrix4x4();
            camera.Field180 = ReadFloat();
            camera.Field184 = ReadFloat();
            camera.Field188 = ReadFloat();
            camera.Field18C = ReadFloat();

            if ( version > 0x1104060 )
            {
                camera.Field190 = ReadFloat();
            }

            return camera;
        }

        // Light
        private Light ReadLight( uint version )
        {
            var light = new Light();

            if ( version > 0x1104190 )
            {
                light.Flags = ( LightFlags )ReadInt();
            }

            light.Type = ( LightType )ReadInt();
            light.Field30 = ReadVector4();
            light.Field40 = ReadVector4();
            light.Field50 = ReadVector4();

            switch ( light.Type )
            {
                case LightType.Type1:
                    light.Field20 = ReadFloat();
                    light.Field04 = ReadFloat();
                    light.Field08 = ReadFloat();
                    break;

                case LightType.Sky:
                    light.Field10 = ReadFloat();
                    light.Field04 = ReadFloat();
                    light.Field08 = ReadFloat();

                    if ( light.Flags.HasFlag( LightFlags.Flag2 ) )
                    {
                        light.Field6C = ReadFloat();
                        light.Field70 = ReadFloat();
                    }
                    else
                    {
                        light.Field60 = ReadFloat();
                        light.Field64 = ReadFloat();
                        light.Field68 = ReadFloat();
                    }
                    break;

                case LightType.Type3:
                    light.Field20 = ReadFloat();
                    light.Field08 = ReadFloat();
                    light.Field04 = ReadFloat();
                    light.Field74 = ReadFloat();
                    light.Field78 = ReadFloat();
                    goto case LightType.Sky;
            }

            return light;
        }

        private ChunkType000100F9 ReadChunkType000100F9( uint version )
        {
            var chunk = new ChunkType000100F9( version );
            chunk.Field140 = ReadInt();
            chunk.Field13C = ReadFloat();
            chunk.Field138 = ReadFloat();
            chunk.Field134 = ReadFloat();
            chunk.Field130 = ReadFloat();

            int entry1Count = ReadInt(); // r28
            int entry2Count = ReadInt(); // r21
            int entry3Count = ReadInt(); // r20

            for ( int i = 0; i < entry1Count; i++ )
            {
                var entry = new ChunkType000100F9Entry1();
                entry.Field34 = ReadFloat();
                entry.Field38 = ReadFloat();

                if ( version > 0x1104120 )
                {
                    entry.Field3C = ReadFloat();
                    entry.Field40 = ReadFloat();
                }
                else
                {
                    entry.Field38 += 20.0f;
                    entry.Field3C = 0;
                    entry.Field40 = 1.0f;
                }

                if ( ReadBool() )
                {
                    entry.NodeName = ReadStringWithHash( version );
                }
                else
                {
                    entry.Field10 = ReadFloat();
                    entry.Field08 = ReadFloat();
                    entry.Field04 = ReadFloat();
                }

                chunk.Entry1List.Add( entry );
            }

            for ( int i = 0; i < entry2Count; i++ )
            {
                var entry = new ChunkType000100F9Entry2();

                entry.Field94 = ReadShort();

                if ( entry.Field94 == 0 )
                {
                    entry.Field84 = ReadFloat();
                }
                else if ( entry.Field94 == 1 )
                {
                    entry.Field84 = ReadFloat();
                    entry.Field88 = ReadFloat();
                }

                entry.Field8C = ReadMatrix4x4();
                if ( ReadBool() )
                {
                    entry.NodeName = ReadStringWithHash( version );
                }

                chunk.Entry2List.Add( entry );
            }

            // r24 = 0x1104120

            for ( int i = 0; i < entry3Count; i++ )
            {
                var entry = new ChunkType000100F9Entry3();
                entry.Field00 = ReadFloat();
                entry.Field04 = ReadFloat();

                if ( version <= 0x1104120 )
                {
                    entry.Field0C = ReadShort();
                    entry.Field0E = ReadShort();
                }
                else
                {
                    entry.Field08 = ReadFloat();
                    entry.Field0C = ReadShort();
                    entry.Field0E = ReadShort();
                }

                chunk.Entry3List.Add( entry );
            }

            return chunk;
        }

        // Animation read methods
        private AnimationPackage ReadAnimationPackage( uint version )
        {
            throw new NotImplementedException();
        }

        // Shader read methods
        private ShaderCachePS3 ReadShaderCachePS3( uint version )
        {
            if ( !ReadFileHeader( out ResourceFileHeader header ) || header.Magic != ResourceFileHeader.MAGIC_SHADERCACHE )
                return null;

            // Read shaders into the shader cache
            ShaderCachePS3 cache = new ShaderCachePS3( header.Version );
            while ( mReader.Position != mReader.BaseStreamLength )
            {
                var shader = ReadShaderPS3( version );
                cache.Add( shader );
            }

            return cache;
        }

        private ShaderPS3 ReadShaderPS3( uint version )
        {
            var shader = new ShaderPS3();
            shader.Type = ReadUShort();
            int size = ReadInt();
            shader.Field06 = ReadUShort();
            shader.Field08 = ReadUInt();
            shader.Field0C = ReadUInt();
            shader.Field10 = ReadUInt();
            shader.Field14 = ReadUInt();
            shader.Field18 = ReadUInt();
            shader.Data = ReadBytes( size );

            return shader;
        }

        private ShaderCachePSP2 ReadShaderCachePSP2( uint version )
        {
            if ( !ReadFileHeader( out ResourceFileHeader header ) || header.Magic != ResourceFileHeader.MAGIC_SHADERCACHE )
                return null;

            // Read shaders into the shader cache
            ShaderCachePSP2 cache = new ShaderCachePSP2( header.Version );
            while ( mReader.Position != mReader.BaseStreamLength )
            {
                var shader = ReadShaderPSP2( version );
                cache.Add( shader );
            }

            return cache;
        }

        private ShaderPSP2 ReadShaderPSP2( uint version )
        {
            var shader = new ShaderPSP2();
            shader.Type = ReadUShort();
            int size = ReadInt();
            shader.Field06 = ReadUShort();
            shader.Field08 = ReadUInt();
            shader.Field0C = ReadUInt();
            shader.Field10 = ReadUInt();
            shader.Field14 = ReadUInt();
            shader.Field18 = ReadUInt();
            shader.Data = ReadBytes( size );

            return shader;
        }

        private ResourceBundle ReadResourceBundle( uint version )
        {
            var bundle = new ResourceBundle( version );

            while ( ReadChunkHeader( out ResourceChunkHeader header ) && header.Type != 0 )
            {
                switch ( header.Type )
                {
                    case ResourceChunkType.TextureDictionary:
                        bundle.AddResource( ReadTextureDictionary( version ) );
                        break;
                    case ResourceChunkType.MaterialDictionary:
                        bundle.AddResource( ReadMaterialDictionary( version ) );
                        break;
                    case ResourceChunkType.Scene:
                        bundle.AddResource( ReadScene( version ) );
                        break;

                    case ResourceChunkType.AnimationPackage:
                    case ResourceChunkType.ChunkType000100F9:
                        goto default;

                    case ResourceChunkType.CustomGenericResourceBundle:
                        bundle.AddResource( ReadResourceBundle( version ) );
                        break;

                    case ResourceChunkType.CustomTexture:
                        bundle.AddResource( ReadTexture() );
                        break;

                    case ResourceChunkType.CustomMaterial:
                        bundle.AddResource( ReadMaterial( version ) );
                        break;

                    case ResourceChunkType.CustomTextureMap:
                        bundle.AddResource( ReadTextureMap( version ) );
                        break;

                    case ResourceChunkType.CustomMaterialAttribute:
                        bundle.AddResource( ReadMaterialAttribute( version ) );
                        break;

                    default:
                        DebugLogPosition( $"Unknown chunk type '{header.Type}'" );
                        mReader.SeekCurrent( header.Size - 12 );
                        continue;
                }
            }

            return bundle;
        }
    }
}
