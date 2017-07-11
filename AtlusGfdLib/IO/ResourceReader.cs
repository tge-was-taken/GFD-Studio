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

        private Vector4 ReadVector4()
        {
            Vector4 value;
            value.X = ReadFloat();
            value.Y = ReadFloat();
            value.Z = ReadFloat();
            value.W = ReadFloat();
            return value;
        }

        private byte[] ReadBytes(int count)
        {
            return mReader.ReadBytes(count);
        }

        private string ReadString()
        {
            ushort length = mReader.ReadUInt16();
            return mReader.ReadString( StringBinaryFormat.FixedLength, length );
        }

        private string ReadStringWithHash()
        {
            var str = ReadString();
            uint hash = mReader.ReadUInt32();
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

            Debug.WriteLine( $"{GetMethodName()}: {header.Magic} ver: {header.Version} type: {header.Type} unk: {header.Unknown}" );

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

            Debug.WriteLine( $"{GetMethodName()}: {header.Type} ver: {header.Version} size: {header.Size}" );

            return true;
        }

        // Resource read methods
        private Model ReadModel( uint version )
        {
            var model = new Model( version );

            while (ReadChunkHeader(out ChunkHeader header) && header.Type != 0)
            {
                switch ( header.Type )
                {
                    case ChunkType.TextureDictionary:
                        model.TextureDictionary = ReadTextureDictionary( header.Version );
                        break;
                    case ChunkType.MaterialDictionary:
                        model.MaterialDictionary = ReadMaterialDictionary( header.Version );
                        break;

                    default:
                        Debug.WriteLine($"{GetMethodName()}: Unknown chunk type '{header.Type}' at offset 0x{mReader.Position.ToString("X")}");
                        mReader.SeekCurrent( header.Size - 12);
                        continue;
                }
            }

            return model;
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
            material.Name = ReadStringWithHash();
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
                material.DiffuseMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                material.NormalMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                material.SpecularMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                material.ReflectionMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                material.HighlightMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                material.GlowMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                material.NightMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                material.DetailMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasShadowMap ) ) 
            {
                material.ShadowMap = ReadTextureMap();
            }

            if ( material.Flags.HasFlag( MaterialFlags.HasProperties ) ) 
            {
                material.Properties = ReadMaterialProperties( version );
            }

            return material;
        }

        private TextureMap ReadTextureMap()
        {
            return new TextureMap()
            {
                Name = ReadStringWithHash(),
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
        private Scene ReadScene()
        {
            throw new NotImplementedException();
        }

        // Animation read methods
        private AnimationPackage ReadAnimationPackage()
        {
            throw new NotImplementedException();
        }

        // Shader read methods
        private ShaderCache ReadShaderCache()
        {
            if ( !ReadFileHeader( out FileHeader header ) || header.Magic != FileHeader.CMAGIC_SHADERCACHE )
                return null;

            // Read shaders into the shader cache
            ShaderCache cache = new ShaderCache( header.Version );
            while (mReader.Position != mReader.BaseStreamLength)
            {
                ushort type = mReader.ReadUInt16();
                uint size = mReader.ReadUInt32();
                ushort field06 = mReader.ReadUInt16();
                uint field08 = mReader.ReadUInt32();
                uint field0C = mReader.ReadUInt32();
                uint field10 = mReader.ReadUInt32();
                float field14 = mReader.ReadSingle();
                float field18 = mReader.ReadSingle();
                byte[] data = mReader.ReadBytes((int)size);

                cache.Add(new Shader(type, field06, field08, field0C, field10, field14, field18, data));
            }

            return cache;
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
                    resource = ReadShaderCache();
                    break;

                default:
                    throw new NotSupportedException();
            }

            return resource;
        }

        public static Resource ReadFromStream(Stream stream, Endianness endianness )
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
    }
}
