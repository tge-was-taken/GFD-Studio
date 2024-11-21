﻿using GFDLibrary.IO;
using GFDLibrary.Materials;
using YamlDotNet.Serialization;

namespace GFDLibrary.Models
{
    public sealed class TextureMap : Resource
    {
        public override ResourceType ResourceType => ResourceType.TextureMap;

        public string Name { get; set; }

        // 0x44
        public int Flags { get; set; }

        // 0x48
        public TextureFilteringMethod MinificationFilter { get; set; }

        // 0x49
        public TextureFilteringMethod MagnificationFilter { get; set; }

        // 0x4A
        public TextureWrapMethod WrapModeU { get; set; }

        // 0x4B
        public TextureWrapMethod WrapModeV { get; set; }
        
        // 0x4C
        public float Field4C { get; set; }

        // 0x50
        public float Field50 { get; set; }

        // 0x54
        public float Field54 { get; set; }

        // 0x58
        public float Field58 { get; set; }

        // 0x5C
        public float Field5C { get; set; }

        // 0x60
        public float Field60 { get; set; }

        // 0x64
        public float Field64 { get; set; }

        // 0x68
        public float Field68 { get; set; }

        // 0x6C
        public float Field6C { get; set; }

        // 0x70
        public float Field70 { get; set; }

        // 0x74
        public float Field74 { get; set; }

        // 0x78
        public float Field78 { get; set; }

        // 0x7C
        public float Field7C { get; set; }

        // 0x80
        public float Field80 { get; set; }

        // 0x84
        public float Field84 { get; set; }

        // 0x88
        public float Field88 { get; set; }

        [YamlIgnore]
        public MaterialParameterSetBase METAPHOR_ParentMaterialParameterSet { get; set; }

        public TextureMap()
        {
            Initialize();
        }

        public TextureMap( uint version ) : base(version)
        {
            Initialize();
        }

        public TextureMap( string name ) : this()
        {
            Name = name;
        }

        private void Initialize()
        {
            Flags = 0;
            MinificationFilter = (TextureFilteringMethod)1;
            MagnificationFilter = (TextureFilteringMethod)1;
            WrapModeU = (TextureWrapMethod)0;
            WrapModeV = (TextureWrapMethod)0;
            Field4C = 1;
            Field50 = 0;
            Field54 = 0;
            Field58 = 0;
            Field5C = 0;
            Field60 = 1;
            Field64 = 0;
            Field68 = 0;
            Field6C = 0;
            Field70 = 0;
            Field74 = 0;
            Field78 = 0;
            Field7C = 0;
            Field80 = 0;
            Field84 = 0;
            Field88 = 0;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Name = reader.ReadStringWithHash( Version );
            Flags = reader.ReadInt32();
            MinificationFilter = (TextureFilteringMethod)reader.ReadByte();
            MagnificationFilter = (TextureFilteringMethod)reader.ReadByte();
            WrapModeU = (TextureWrapMethod)reader.ReadByte();
            WrapModeV = (TextureWrapMethod)reader.ReadByte();
            Field4C = reader.ReadSingle();
            Field50 = reader.ReadSingle();
            Field54 = reader.ReadSingle();
            Field58 = reader.ReadSingle();
            Field5C = reader.ReadSingle();
            Field60 = reader.ReadSingle();
            Field64 = reader.ReadSingle();
            Field68 = reader.ReadSingle();
            Field6C = reader.ReadSingle();
            Field70 = reader.ReadSingle();
            Field74 = reader.ReadSingle();
            Field78 = reader.ReadSingle();
            Field7C = reader.ReadSingle();
            Field80 = reader.ReadSingle();
            Field84 = reader.ReadSingle();
            Field88 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, Name );
            writer.WriteInt32( Flags );
            writer.WriteByte( (byte)MinificationFilter );
            writer.WriteByte( (byte)MagnificationFilter );
            writer.WriteByte( (byte)WrapModeU );
            writer.WriteByte( (byte)WrapModeV );
            writer.WriteSingle( Field4C );
            writer.WriteSingle( Field50 );
            writer.WriteSingle( Field54 );
            writer.WriteSingle( Field58 );
            writer.WriteSingle( Field5C );
            writer.WriteSingle( Field60 );
            writer.WriteSingle( Field64 );
            writer.WriteSingle( Field68 );
            writer.WriteSingle( Field6C );
            writer.WriteSingle( Field70 );
            writer.WriteSingle( Field74 );
            writer.WriteSingle( Field78 );
            writer.WriteSingle( Field7C );
            writer.WriteSingle( Field80 );
            writer.WriteSingle( Field84 );
            writer.WriteSingle( Field88 );
        }
    }
    public enum TextureFilteringMethod : byte
    {
        Nearest,
        Linear,
        Nearest_Nearest,
        Linear_Nearest,
        Nearest_Linear,
        Linear_Linear,
        ConvolutionMin,
        ConvolutionMag
    }
    public enum TextureWrapMethod : byte
    {
        Repeat,
        Mirror,
        Clamp,
        Border
    }
}
