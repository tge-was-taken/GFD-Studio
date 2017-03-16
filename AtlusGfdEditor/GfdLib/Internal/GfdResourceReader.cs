using System.IO;
using System;
using System.Diagnostics;
using AtlusGfdEditor.Framework.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace AtlusGfdEditor.GfdLib.Internal
{
    class GfdResourceReader
    {
        private static FourCC GfsFourCC = new FourCC("GFS0");
        private static FourCC GscFourCC = new FourCC("GSC0");
        private BinarySourceReader m_Reader;
        private GfdChunkHeader m_CurChunkHeader;

        private string GetFunctionString([CallerMemberName]string name = null)
        {
            return $"{nameof(GfdResourceReader)}.{name}";
        }

        private bool ReadChunkHeader()
        {
            if (m_Reader.Position >= m_Reader.SourceLength ||
                m_Reader.Position + Marshal.SizeOf<GfdChunkHeader>() > m_Reader.SourceLength)
                return false;

            GfdChunkHeader chunk = new GfdChunkHeader();
            chunk.Version = m_Reader.ReadUInt32();
            chunk.Type = (GfdChunkType)m_Reader.ReadUInt32();
            chunk.Size = m_Reader.ReadUInt32();
            m_CurChunkHeader = chunk;

            Debug.WriteLine($"{GetFunctionString()}: Chunk {chunk.Type} ver: {chunk.Version} size: {chunk.Size}");

            return true;
        }

        private GfdString ReadGfdStringWithHash()
        {
            return new GfdString(m_Reader.ReadString(m_Reader.ReadUInt16()), m_Reader.ReadUInt32());
        }

        private GfdString ReadGfdString()
        {
            return new GfdString(m_Reader.ReadString(m_Reader.ReadUInt16()));
        }

        private GfdResourceBundle ReadResourceBundle()
        {
            var bundle = new GfdResourceBundle(m_CurChunkHeader.Version);

            while (ReadChunkHeader())
            {
                // Read resource data
                GfdResource resource = null;

                switch (m_CurChunkHeader.Type)
                {
                    case GfdChunkType.TextureDictionary:
                        resource = ReadTextureDictionary();
                        break;
                    case GfdChunkType.MaterialDictionary:
                        resource = ReadMaterialDictionary();
                        break;
                        /*
                    case GfdChunkType.Scene:
                        resource = ReadScene();
                        break;
                    case GfdChunkType.AnimationList:
                        resource = ReadAnimationList();
                        break;
                        */

                    default:
                        Debug.WriteLine($"{nameof(ReadResourceBundle)}: Unknown chunk type '{m_CurChunkHeader.Type}' at offset 0x{m_Reader.Position.ToString("X")}");
                        m_Reader.SeekCurrent(m_CurChunkHeader.Size - 12);
                        continue;
                }

                bundle.AddResource(resource);

                if (m_CurChunkHeader.Type == GfdChunkType.MaterialDictionary)
                    break;
            }

            return bundle;
        }

        private GfdTextureDictionary ReadTextureDictionary()
        {
            var texDic = new GfdTextureDictionary(m_CurChunkHeader.Version);

            if (m_Reader.ReadUInt32() != 0)
                Debug.WriteLine($"{nameof(ReadTextureDictionary)}: field00 is not 0 at offset {m_Reader.Position}");

            uint numTextures = m_Reader.ReadUInt32();
            for (int i = 0; i < numTextures; i++)
            {
                var name = ReadGfdString();

                ushort fmt = m_Reader.ReadUInt16();
                if (!Enum.IsDefined(typeof(GfdTextureFormat), fmt))
                    Debug.WriteLine($"{nameof(ReadTextureDictionary)}: unknown texture format '{fmt}' at offset {m_Reader.Position}");

                int size = m_Reader.ReadInt32();
                byte[] pixelData = m_Reader.ReadByteArray(size);

                uint footer = m_Reader.ReadUInt32();
                if (footer != GfdConstants.TextureDataFooter)
                    Debug.WriteLine($"{nameof(ReadTextureDictionary)}: texture footer mismatch '{footer}' at offset {m_Reader.Position - 4}");

                texDic.Add((uint)name.GetHashCode(), new GfdTexture(name, (GfdTextureFormat)fmt, pixelData));
            }

            return texDic;
        }

        private GfdAnimationList ReadAnimationList()
        {
            throw new NotImplementedException();
        }

        /*
        private GfdMaterialDictionary ReadMaterialDictionary()
        {
            var matDic = new GfdMaterialDictionary(m_CurChunkHeader.Version);

            if (m_Reader.ReadUInt32() != 0)
                Debug.WriteLine($"{GetFunctionString()}: field00 is not 0 at offset {m_Reader.Position}");

            uint numMaterials = m_Reader.ReadUInt32();
            Debug.WriteLine($"{GetFunctionString()}: {numMaterials} materials");

            for (int i = 0; i < numMaterials; i++)
            {
                var name = ReadGfdStringWithHash();
                uint flags = m_Reader.ReadUInt32();
                var floats = m_Reader.ReadSingleArray(18);
                var shorts = m_Reader.ReadUInt16Array(8);
                var uints = m_Reader.ReadUInt32Array(2);
                var short1 = m_Reader.ReadUInt16();
                var uint1 = m_Reader.ReadUInt32();

                Debug.WriteLine($"{GetFunctionString()}: {name} flags: {flags.ToString("X8")}");

                Debug.Write($"{GetFunctionString()}: ");
                for (int j = 0; j < floats.Length; j++)
                {
                    Debug.Write($"{floats[j].ToString("0.########")} ");
                }
                Debug.Write("\n");

                Debug.Write($"{GetFunctionString()}: ");
                for (int j = 0; j < shorts.Length; j++)
                {
                    Debug.Write($"{shorts[j].ToString("X4")} ");
                }
                Debug.Write("\n");

                Debug.Write($"{GetFunctionString()}: ");
                for (int j = 0; j < uints.Length; j++)
                {
                    Debug.Write($"{uints[j].ToString("X8")} ");
                }
                Debug.Write("\n");

                Debug.WriteLine($"{GetFunctionString()}: {short1.ToString("X4")}");
                Debug.WriteLine($"{GetFunctionString()}: {uint1.ToString("X8")}");

                for (int j = 0; j < 9; j++)
                {
                    var bitMask = (uint)GfdMaterialFlags.Diffuse << j;

                    if ((flags & bitMask) == bitMask)
                    {
                        Debug.Write($"{(GfdMaterialFlags)((uint)GfdMaterialFlags.Diffuse << j)} | ");
                        var texName = ReadGfdStringWithHash();
                        //Debug.WriteLine(texName);
                        m_Reader.SeekCurrent(0x48);
                    }
                }
                Debug.Write("\n");

                bool isLast = (i == numMaterials - 1);

                int endSkip = 0;
                bool foundEndSkip = TestMaterialEndSkip(endSkip, isLast);

                endSkip = 0x10;
                if (!foundEndSkip)
                    foundEndSkip = TestMaterialEndSkip(endSkip, isLast);

                endSkip = 0x3C;
                if (!foundEndSkip)
                    foundEndSkip = TestMaterialEndSkip(endSkip, isLast);

                endSkip = 0x48;
                if (!foundEndSkip)
                    foundEndSkip = TestMaterialEndSkip(endSkip, isLast);

                if (!foundEndSkip)
                {
                    for (endSkip = 1; endSkip < 0x100; endSkip++)
                    {
                        if ((endSkip == 0x10) || (endSkip == 0x3C) || (endSkip == 0x48)) continue;

                        foundEndSkip = TestMaterialEndSkip(endSkip, isLast);

                        if (foundEndSkip)
                            break;

                    }
                }

                Debug.WriteLine($"skip: {endSkip}");
                var bytes = m_Reader.ReadByteArray(endSkip);

                for (int j = 0; j < endSkip; j++)
                {
                    if (j % 16 == 0)
                        Debug.Write('\n');
                    Debug.Write($"{bytes[j].ToString("X2")} ");                  
                }
                Debug.Write("\n");
            }           

            return matDic;
        }

        private bool TestMaterialEndSkip(int endSkip, bool isLast)
        {
            var startPos = m_Reader.Position;

            try
            {
                m_Reader.SeekCurrent(endSkip);

                if (!isLast)
                {
                    var nameLen = m_Reader.ReadUInt16();
                    if (nameLen == 0 || nameLen > 64)
                    {
                        m_Reader.SeekSet(startPos);
                        return false;
                    }

                    for (int i = 0; i < nameLen; i++)
                    {
                        var curByte = m_Reader.ReadByte();
                        if ((curByte == 0) || (curByte > 0x7F))
                        {
                            m_Reader.SeekSet(startPos);
                            return false;
                        }
                    }

                    m_Reader.SeekCurrent(0x58);
                    if ((m_Reader.ReadUInt16()) != 4)
                    {
                        m_Reader.SeekSet(startPos);
                        return false;
                    }
                }
                else
                {
                    var ver = m_Reader.ReadUInt32();
                    if (!((ver >= 0x1100000) || (ver < 0x11FFFFF)))
                    {
                        m_Reader.SeekSet(startPos);
                        return false;
                    }
                }
            }
            catch
            {
                m_Reader.SeekSet(startPos);
                return false;
            }

            m_Reader.SeekSet(startPos);
            //m_Reader.SeekSet(startPos + endSkip);
            return true;
        }
        */

        private GfdMaterialColor ReadGfdMaterialColor()
        {
            float red = m_Reader.ReadSingle();
            float green = m_Reader.ReadSingle();
            float blue = m_Reader.ReadSingle();
            float intensity = m_Reader.ReadSingle();

            return new GfdMaterialColor(red, green, blue, intensity);
        }

        private GfdMaterialDictionary ReadMaterialDictionary()
        {
            GfdMaterialDictionary materialDictionary = new GfdMaterialDictionary(m_CurChunkHeader.Version);

            // Check unused field
            if (m_Reader.ReadUInt32() != 0)
                Debug.WriteLine($"{GetFunctionString()}: field00 is not 0 at offset {m_Reader.Position}");

            uint numMaterials = m_Reader.ReadUInt32();
            for (int i = 0; i < numMaterials; i++)
            {
                var material = new GfdMaterial();

                // Read material header
                material.Name = ReadGfdStringWithHash();
                material.Flags = m_Reader.ReadUInt32();
                material.Ambient = ReadGfdMaterialColor();
                material.Diffuse = ReadGfdMaterialColor();
                material.Specular = ReadGfdMaterialColor();
                material.Field30 = ReadGfdMaterialColor();
                material.Field40 = m_Reader.ReadSingle();
                material.Field44 = m_Reader.ReadSingle();
                material.Field48 = m_Reader.ReadByteArray(16);
                material.Field58 = m_Reader.ReadUInt32();
                material.Field5C = m_Reader.ReadUInt32();
                material.Field60 = m_Reader.ReadUInt16();
                material.Field62 = m_Reader.ReadUInt16();

                // Read texture references
                for (int j = 0; j < 11; j++)
                {
                    uint bitMask = (uint)GfdMaterialFlags.Diffuse << j;
                    if ((material.Flags & bitMask) == bitMask)
                    {
                        var textureReference = new GfdMaterialTextureReference();
                        textureReference.TextureName = ReadGfdStringWithHash();
                        textureReference.Field04 = m_Reader.ReadByteArray(8);
                        textureReference.Field0C = m_Reader.ReadSingleArray(16);

                        material.TextureReferences[j] = textureReference;
                    }
                }

                materialDictionary.Add((uint)material.Name.GetHashCode(), material);
            }

            return materialDictionary;
        }

        private GfdScene ReadScene()
        {
            throw new NotImplementedException();
        }

        private GfdShaderCache ReadShaderCache()
        {
            if (!GscFourCC.Matches(EndiannessHelper.SwapEndianness(m_Reader.ReadUInt32())))
                return null;

            // Read chunk
            ReadChunkHeader();

            // Read shaders into the shader cache
            GfdShaderCache cache = new GfdShaderCache(m_CurChunkHeader.Version);
            while (m_Reader.Position != m_Reader.SourceLength)
            {
                ushort type = m_Reader.ReadUInt16();
                uint size = m_Reader.ReadUInt32();
                ushort field06 = m_Reader.ReadUInt16();
                uint field08 = m_Reader.ReadUInt32();
                uint field0C = m_Reader.ReadUInt32();
                uint field10 = m_Reader.ReadUInt32();
                float field14 = m_Reader.ReadSingle();
                float field18 = m_Reader.ReadSingle();
                byte[] data = m_Reader.ReadByteArray((int)size);

                cache.Add(new GfdShader(type, field06, field08, field0C, field10, field14, field18, data));
            }

            return cache;
        }

        public GfdResource ReadFromStream(Stream stream)
        {
            using (m_Reader = BinarySourceReader.GetReader(stream, Endianness.BigEndian))
            {
                // Check magic
                if (!GfsFourCC.Matches(EndiannessHelper.SwapEndianness(m_Reader.ReadUInt32())))
                    return null;

                // Read chunk
                if (!ReadChunkHeader())
                    return null;

                // Check unused field
                if (m_CurChunkHeader.Size != 0)
                    Debug.WriteLine($"{nameof(ReadFromStream)}: size field isn't 0 at offset {m_Reader.Position.ToString("X")}");

                // Read resource depending on type
                GfdResource resource;

                switch (m_CurChunkHeader.Type)
                {
                    case GfdChunkType.ChunkListStart:
                        resource = ReadResourceBundle();
                        break;

                    case GfdChunkType.ShaderCache:
                        resource = ReadShaderCache();
                        break;

                    default:
                        throw new NotSupportedException();
                }

                return resource;
            }
        }
    }
}
