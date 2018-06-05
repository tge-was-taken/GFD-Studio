using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using GFDLibrary.Cameras;
using GFDLibrary.IO.Common;
using GFDLibrary.Keyframes;
using GFDLibrary.Lights;
using GFDLibrary.Shaders;

namespace GFDLibrary.IO.Resource
{
    internal class ResourceReader : IDisposable
    {
        private static readonly Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
        private readonly EndianBinaryReader mReader;

        public ResourceReader( Stream stream, Endianness endianness, bool leaveOpen = false )
        {
            mReader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, endianness );
        }

        public static GFDLibrary.Resource ReadFromStream( Stream stream, Endianness endianness, bool leaveOpen = false )
        {
            using ( var reader = new ResourceReader( stream, endianness, leaveOpen ) )
                return reader.ReadResourceBundleFile();
        }

        public static TResource ReadFromStream<TResource>( Stream stream, Endianness endianness, bool leaveOpen = false )
            where TResource : GFDLibrary.Resource
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

        private float ReadHalfFloat()
        {
            var encoded = ReadUShort();

            // Decode half float
            // Based on numpy implementation (https://github.com/numpy/numpy/blob/984bc91367f9b525eadef14c48c759999bc4adfc/numpy/core/src/npymath/halffloat.c#L477)
            uint decoded;
            uint halfExponent = ( encoded & 0x7c00u );
            uint singleSign = ( encoded & 0x8000u ) << 16;
            switch ( halfExponent )
            {
                case 0x0000u: /* 0 or subnormal */
                    uint halfSign = ( encoded & 0x03ffu );
                
                    if ( halfSign == 0 )
                    {
                        /* Signed zero */
                        decoded = singleSign;
                    }
                    else
                    {
                        /* Subnormal */
                        halfSign <<= 1;

                        while ( ( halfSign & 0x0400u ) == 0 )
                        {
                            halfSign <<= 1;
                            halfExponent++;
                        }

                        uint singleExponent = 127 - 15 - halfExponent << 23;
                        uint singleSig = ( halfSign & 0x03ffu ) << 13;
                        decoded = singleSign + singleExponent + singleSig;
                    }
                    break;

                case 0x7c00u: /* inf or NaN */
                    /* All-ones exponent and a copy of the significand */
                    decoded = singleSign + 0x7f800000u + ( ( encoded & 0x03ffu ) << 13 );
                    break;

                default: /* normalized */
                    /* Just need to adjust the exponent and shift */
                    decoded = singleSign + ( ( ( encoded & 0x7fffu ) + 0x1c000u ) << 13 );
                    break;
            }

            return UnsafeUtilities.ReinterpretCast< uint, float >( decoded );
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
        private GFDLibrary.Resource ReadResourceBundleFile()
        {
            if ( !ReadFileHeader( out ResourceFileHeader header ) || header.Magic != ResourceFileHeader.MAGIC_FS )
                return null;

            // Read resource depending on type
            GFDLibrary.Resource resource;

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
                    case ResourceChunkType.AnimationPackage:
                        model.AnimationPackage = new AnimationPackage( header.Version ) { RawData = ReadBytes( header.Size - 12 ) };
                        //model.AnimationPackage = ReadAnimationPackage( header.Version );
                        break;
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

            if ( version <= 0x1105070 || version >= 0x1105090 )
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
                scene.BonePalette = ReadBonePalette();

            if ( flags.HasFlag( SceneFlags.HasBoundingBox ) )
                scene.BoundingBox = ReadBoundingBox();

            if ( flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                scene.BoundingSphere = ReadBoundingSphere();

            scene.RootNode = ReadNodeRecursive( version );
            scene.Flags = flags;

            return scene;
        }

        private BonePalette ReadBonePalette()
        {
            int matrixCount = ReadInt();
            var map = new BonePalette( matrixCount );

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
            var skipProperties = false;
            for ( int i = 0; i < attachmentCount; i++ )
            {
                var attachment = ReadNodeAttachment( version, out var _skipProperties );
                if ( _skipProperties )
                    skipProperties = true;

                node.Attachments.Add( attachment );
            }

            if ( version > 0x1060000 && !skipProperties )
            {
                var hasProperties = ReadBool();
                if ( hasProperties )
                {
                    node.Properties = ReadUserPropertyCollection( version );
                }
            }

            if ( version > 0x1104230 )
            {
                node.FieldE0 = ReadFloat();
                DebugLog( $"FieldE0: {node.FieldE0}" );
            }

            return node;
        }

        private NodeAttachment ReadNodeAttachment( uint version, out bool skipProperties )
        {
            NodeAttachmentType type = ( NodeAttachmentType )ReadInt();

            DebugLogPosition( $"{type}" );

            skipProperties = false;

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
                case NodeAttachmentType.Epl:
                    return new NodeEplAttachment( ReadEpl( version, out skipProperties ) );
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

        private UserPropertyCollection ReadUserPropertyCollection( uint version )
        {
            int propertyCount = ReadInt();
            var userPropertyCollection = new UserPropertyCollection( propertyCount );

            for ( int i = 0; i < propertyCount; i++ )
            {
                DebugLogPosition( "Property" );
                var type = ( UserPropertyValueType )ReadInt();
                var name = ReadStringWithHash( version );
                var size = ReadInt();

                UserProperty property;
                switch ( type )
                {
                    case UserPropertyValueType.Int:
                        property = new UserIntProperty( name, ReadInt() );
                        break;
                    case UserPropertyValueType.Float:
                        property = new UserFloatProperty( name, ReadFloat() );
                        break;
                    case UserPropertyValueType.Bool:
                        property = new UserBoolProperty( name, ReadBool() );
                        break;
                    case UserPropertyValueType.String:
                        property = new UserStringProperty( name, ReadString( size - 1 ) );
                        break;
                    case UserPropertyValueType.ByteVector3:
                        property = new UserByteVector3Property( name, ReadByteVector3() );
                        break;
                    case UserPropertyValueType.ByteVector4:
                        property = new UserByteVector4Property( name, ReadByteVector4() );
                        break;
                    case UserPropertyValueType.Vector3:
                        property = new UserVector3Property( name, ReadVector3() );
                        break;
                    case UserPropertyValueType.Vector4:
                        property = new UserVector4Property( name, ReadVector4() );
                        break;
                    case UserPropertyValueType.ByteArray:
                        property = new UserByteArrayProperty( name, ReadBytes( size ) );
                        break;
                    default:
                        throw new Exception( $"Unknown node property type: {type}" );
                }

                DebugLog( $"{type} {name} {size} {property.GetValue()}" );

                userPropertyCollection[property.Name] = property;
            }

            return userPropertyCollection;
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
            var animationPackage = new AnimationPackage( version );

            if ( version > 0x1104950 )
            {
                animationPackage.Flags = ( AnimationPackageFlags )ReadInt(); // r26
            }

            int count1 = ReadInt();
            for ( int i = 0; i < count1; i++ )
            {
                var animation = ReadAnimation( version );
                animationPackage.Animations.Add( animation );
            }

            int count2 = ReadInt();
            for ( int i = 0; i < count2; i++ )
            {
                var animation = ReadAnimation( version );
                animationPackage.BlendAnimations.Add( animation );
            }

            if ( animationPackage.Flags.HasFlag( AnimationPackageFlags.Flag4 ) )
            {
                animationPackage.ExtraData = ReadAnimationExtraData( version );
            }

            return animationPackage;
        }

        private AnimationExtraData ReadAnimationExtraData( uint version )
        {
            var animationExtraData = new AnimationExtraData();

            animationExtraData.Field00 = ReadAnimation( version );
            animationExtraData.Field10 = ReadFloat();
            animationExtraData.Field04 = ReadAnimation( version );
            animationExtraData.Field14 = ReadFloat();
            animationExtraData.Field08 = ReadAnimation( version );
            animationExtraData.Field18 = ReadFloat();
            animationExtraData.Field0C = ReadAnimation( version );
            animationExtraData.Field1C = ReadFloat();

            return animationExtraData;
        }

        private Animation ReadAnimation( uint version )
        {
            var animation = new Animation();

            if ( version > 0x1104110 )
            {
                animation.Flags = ( AnimationFlags )ReadInt();
            }

            animation.Duration = ReadFloat();

            var controllerCount = ReadInt();
            for ( var i = 0; i < controllerCount; i++ )
            {
                var controller = ReadAnimationController( version );
                animation.Controllers.Add( controller );
            }

            if ( animation.Flags.HasFlag( AnimationFlags.Flag10000000 ) )
            {
                var count = ReadInt();
                for ( var i = 0; i < count; i++ )
                {
                    animation.Field10.Add( new Flag10000000DataEntry
                    {
                        Field00 = ReadEpl( version, out _ ),
                        Field04 = ReadStringWithHash( version )
                    });
                }
            }

            if ( animation.Flags.HasFlag( AnimationFlags.Flag20000000 ) )
            {
                animation.Field14 = ReadAnimationExtraData( version );
            }

            if ( animation.Flags.HasFlag( AnimationFlags.Flag80000000 ) )
            {
                animation.Field1C = new Flag80000000Data
                {
                    Field00 = ReadInt(),
                    Field04 = ReadStringWithHash( version ),
                    Field20 = ReadAnimationKeyframeTrack( version )
                };
            }

            if ( animation.Flags.HasFlag( AnimationFlags.HasBoundingBox ) )
            {
                animation.BoundingBox = ReadBoundingBox();
            }

            if ( animation.Flags.HasFlag( AnimationFlags.Flag2000000 ) )
            {
                animation.Field24 = ReadFloat();
            }

            if ( animation.Flags.HasFlag( AnimationFlags.HasProperties ) )
            {
                animation.Properties = ReadUserPropertyCollection( version );
            }

            return animation;
        }

        private Controller ReadAnimationController( uint version )
        {
            var controller = new Controller();
            controller.TargetKind = ( TargetKind )ReadShort();
            controller.TargetId = ReadInt();
            controller.TargetName = ReadStringWithHash( version );

            var trackCount = ReadInt();
            for ( int j = 0; j < trackCount; j++ )
            {
                var keyframeTrack = ReadAnimationKeyframeTrack( version );
                controller.Tracks.Add( keyframeTrack );
            }

            return controller;
        }

        private KeyframeTrack ReadAnimationKeyframeTrack( uint version )
        {
            var track = new KeyframeTrack();
            track.KeyframeKind = ( KeyframeKind )ReadInt();

            int keyframeCount = ReadInt();

            for ( int i = 0; i < keyframeCount; i++ )
                track.KeyframeTimings.Add( ReadFloat() );

            for ( int i = 0; i < keyframeCount; i++ )
            {
                var keyframe = ReadAnimationKeyframe( version, track.KeyframeKind );
                track.Keyframes.Add( keyframe );
            }

            if ( track.KeyframeKind == KeyframeKind.Type26 || track.KeyframeKind == KeyframeKind.NodePRSHalf ||
                 track.KeyframeKind == KeyframeKind.NodePRHalf )
            {
                track.BasePosition = ReadVector3();
                track.BaseScale = ReadVector3();
            }

            return track;
        }

        private IKeyframe ReadAnimationKeyframe( uint version, KeyframeKind kind )
        {
            switch ( kind )
            {
                case KeyframeKind.NodePR:
                    return ReadAnimationType01( version );

                case KeyframeKind.NodePRS:
                    return ReadAnimationType02( version );

                case KeyframeKind.Type03:
                    return ReadAnimationType03( version );

                case KeyframeKind.Type04:
                    return ReadAnimationType04( version );

                case KeyframeKind.Type05:
                    return ReadAnimationType05( version );

                case KeyframeKind.Type06:
                    return ReadAnimationType06( version );

                case KeyframeKind.Type07:
                    return ReadAnimationType07( version );

                case KeyframeKind.Type08:
                    return ReadAnimationType08( version );

                case KeyframeKind.Type09:
                    return ReadAnimationType09( version );

                case KeyframeKind.Type10:
                    return ReadAnimationType10( version );

                case KeyframeKind.Type11:
                    return ReadAnimationType11( version );

                case KeyframeKind.MaterialType12:
                    return ReadAnimationType12( version );

                case KeyframeKind.Type13:
                    return ReadAnimationType13( version );

                case KeyframeKind.MaterialType14:
                    return ReadAnimationType14( version );

                case KeyframeKind.Type15:
                    return ReadAnimationType15( version );

                case KeyframeKind.Type16:
                    return ReadAnimationType16( version );

                case KeyframeKind.Type17:
                    return ReadAnimationType17( version );

                case KeyframeKind.Type18:
                    return ReadAnimationType18( version );

                case KeyframeKind.Type19:
                    return ReadAnimationType19( version );

                case KeyframeKind.Type20:
                    return ReadAnimationType20( version );

                case KeyframeKind.Type21:
                    return ReadAnimationType21( version );

                case KeyframeKind.Type22:
                    return ReadAnimationType22( version );

                case KeyframeKind.Type23:
                    return ReadAnimationType23( version );

                case KeyframeKind.Type24:
                    return ReadAnimationType24( version );

                case KeyframeKind.Type25:
                    return ReadAnimationType25( version );

                case KeyframeKind.Type26:
                    return ReadAnimationType26( version );

                case KeyframeKind.NodePRSHalf:
                    return ReadAnimationType27( version );

                case KeyframeKind.NodePRHalf:
                    return ReadAnimationType28( version );

                case KeyframeKind.MaterialType29:
                    return ReadAnimationType29( version );

                case KeyframeKind.Type30:
                    return ReadAnimationType30( version );

                case KeyframeKind.Type31:
                    return ReadAnimationType31( version );

                default:
                    throw new Exception( $"Invalid keyframe type! '{kind}'" );
            }
        }

        private IKeyframe ReadAnimationType01( uint version )
        {
            return new NodePRKeyframe()
            {
                Position = ReadVector3(),
                Rotation = ReadQuaternion(),
            };
        }

        private IKeyframe ReadAnimationType02( uint version )
        {
            return new NodePRSKeyframe
            {
                Position = ReadVector3(),
                Rotation = ReadQuaternion(),
                Scale = ReadVector3()
            };
        }

        private IKeyframe ReadAnimationType03( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType04( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType05( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType06( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType07( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType08( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType09( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType10( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType11( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType12( uint version )
        {
            return new MaterialKeyframeType12 { Field00 = ReadFloat() };
        }

        private IKeyframe ReadAnimationType13( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType14( uint version )
        {
            return new MaterialKeyframeType14()
            {
                Field00 = ReadFloat(),
                Field04 = ReadFloat(),
                Field08 = ReadFloat(),
            };
        }

        private IKeyframe ReadAnimationType15( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType16( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType17( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType18( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType19( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType20( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType21( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType22( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType23( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType24( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType25( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType26( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType27( uint version )
        {
            return new NodePRSHalfKeyframe
            {
                Position = new Vector3( ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat() ),
                Rotation = new Quaternion( ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat() ),
                Scale = new Vector3( ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat() )
            };
        }

        private IKeyframe ReadAnimationType28( uint version )
        {
            return new NodePRHalfKeyframe
            {
                Position = new Vector3( ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat() ),
                Rotation = new Quaternion( ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat(), ReadHalfFloat() )
            };
        }

        private IKeyframe ReadAnimationType29( uint version )
        {
            return new MaterialKeyframeType29 { Field00 = ReadFloat() };
        }

        private IKeyframe ReadAnimationType30( uint version )
        {
            throw new NotImplementedException();
        }

        private IKeyframe ReadAnimationType31( uint version )
        {
            throw new NotImplementedException();
        }

        // Epl

        private Epl ReadEpl( uint version, out bool skipProperties )
        {
            var epl = new Epl();

            var startOffset = mReader.Position;
            var skipEplData = false;
            skipProperties = false;

            // Hacks galore
            if ( mReader.Position == 0x1a67d5 || mReader.Position == 0x1b09ef )
            {
                mReader.SeekCurrent( 0x27C9 );
                skipEplData = true;
            }
            else if ( mReader.Position == 0x1a907c || mReader.Position == 0x1b3296 )
            {
                mReader.SeekCurrent( 0x3568 );
                skipEplData = true;
            }
            else if ( mReader.Position == 0x1ac6c2 || mReader.Position == 0x1b68dc )
            {
                mReader.SeekCurrent( 0x3571 );
                skipEplData = true;
            }
            else
            {
                var foundProperty = false;
                int testCount;
                const int MAX_TEST_COUNT = 1000000;
                for ( testCount = 0; testCount < MAX_TEST_COUNT; testCount++ )
                {
                    var test = ReadUInt();

                    if ( test == 0x42697030 ||// Bip01
                        test == 0x726F6F74 ||// root
                        test == 0x726F74E0 ||// rot
                        test == 0x62206C20 ||// b l
                        test == 0x62206C5F ||// b l_
                        test == 0x62207220 ||// b r
                        test == 0x6220725F ||// b r_
                        test == 0x68656164 ||// head
                        test == 0x685F415F ||// h_A_
                        test == 0x685F425F ||// h_B_
                        test == 0x685F435F ||// h_C_
                        test == 0x625F6D61 ||// b_man
                        test == 0x625F6865 ||// b_he
                        test == 0x62206465 ||// b de
                        test == 0x626F6479 ||// body
                        test == 0x62207461 ||// b ta
                        test == 0x6F626A5F ||// obj_
                        test == 0x6D616E64 ||// mand
                        test == 0x625F6E6F ||// b_no
                        test == 0x68206C20 ||// h l
                        test == 0x68207220 ||// h r
                        test == 0x68206C5F ||// h l_
                        test == 0x6820725F ||// h r_
                        test == 0x685F6C5F ||// h_l_
                        test == 0x685F725F ||// h_r_
                        test == 0x73686164 )  //shad
                    {
                        break;
                    }
                    else if ( test == 0x67666448 ) // gfdH
                    {
                        foundProperty = true;
                        mReader.SeekCurrent( -1 );
                        break;
                    }

                    mReader.SeekCurrent( -3 );
                }

                skipProperties = !foundProperty;
                mReader.SeekCurrent( -0xE );
                if ( testCount == MAX_TEST_COUNT )
                {
                    throw new Exception( "Can't handle particle attachment" );
                }
            }

            if ( !skipEplData )
            {
                var endOffset = mReader.Position;
                var byteCount = ( int ) ( endOffset - startOffset );
                mReader.SeekBegin( startOffset );
                epl.Raw = mReader.ReadBytes( byteCount );
            }

            //epl.Field20 = ReadInt() | 4;
            //epl.Field2C = ReadNodeRecursive( version );
            //epl.Field30 = ReadEplAnimation( version );

            //if ( version > 0x1105060 )
            //{
            //    epl.Field40 = ReadShort();
            //}

            return epl;
        }

        private EplAnimation ReadEplAnimation( uint version )
        {
            var epl = new EplAnimation();

            epl.Field00 = ReadInt();
            epl.Field04 = ReadFloat();
            epl.Field0C = ReadAnimation( version );

            var count = ReadInt();
            for ( int i = 0; i < count; i++ )
            {
                ReadFloat();
                ReadFloat();
                ReadInt();
                ReadInt();
            }

            return epl;
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
            shader.Type = ( ShaderType )ReadUShort();
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
            shader.Type = ( ShaderType )ReadUShort();
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
