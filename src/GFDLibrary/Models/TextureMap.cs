using GFDLibrary.IO;
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
        [FormerName( "Field4C" )]
        public float TransformM11 { get; set; }

        // 0x50
        [FormerName( "Field50" )]
        public float TransformM12 { get; set; }

        // 0x54
        [FormerName( "Field54" )]
        public float TransformM13 { get; set; }

        // 0x58
        [FormerName( "Field58" )]
        public float TransformM14 { get; set; }

        // 0x5C
        [FormerName( "Field5C" )]
        public float TransformM21 { get; set; }

        // 0x60
        [FormerName( "Field60" )]
        public float TransformM22 { get; set; }

        // 0x64
        [FormerName( "Field64" )]
        public float TransformM23 { get; set; }

        // 0x68
        [FormerName( "Field68" )]
        public float TransformM24 { get; set; }

        // 0x6C
        [FormerName( "Field6C" )]
        public float TransformM31 { get; set; }

        // 0x70
        [FormerName( "Field70" )]
        public float TransformM32 { get; set; }

        // 0x74
        [FormerName( "Field74" )]
        public float TransformM33 { get; set; }

        // 0x78
        [FormerName( "Field78" )]
        public float TransformM34 { get; set; }

        // 0x7C
        [FormerName( "Field7C" )]
        public float TransformM41 { get; set; }

        // 0x80
        [FormerName( "Field80" )]
        public float TransformM42 { get; set; }

        // 0x84
        [FormerName( "Field84" )]
        public float TransformM43 { get; set; }

        // 0x88
        [FormerName( "Field88" )]
        public float TransformM44 { get; set; }

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
            TransformM11 = 1;
            TransformM12 = 0;
            TransformM13 = 0;
            TransformM14 = 0;
            TransformM21 = 0;
            TransformM22 = 1;
            TransformM23 = 0;
            TransformM24 = 0;
            TransformM31 = 0;
            TransformM32 = 0;
            TransformM33 = 0;
            TransformM34 = 0;
            TransformM41 = 0;
            TransformM42 = 0;
            TransformM43 = 0;
            TransformM44 = 0;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Name = reader.ReadStringWithHash( Version );
            Flags = reader.ReadInt32();
            MinificationFilter = (TextureFilteringMethod)reader.ReadByte();
            MagnificationFilter = (TextureFilteringMethod)reader.ReadByte();
            WrapModeU = (TextureWrapMethod)reader.ReadByte();
            WrapModeV = (TextureWrapMethod)reader.ReadByte();
            TransformM11 = reader.ReadSingle();
            TransformM12 = reader.ReadSingle();
            TransformM13 = reader.ReadSingle();
            TransformM14 = reader.ReadSingle();
            TransformM21 = reader.ReadSingle();
            TransformM22 = reader.ReadSingle();
            TransformM23 = reader.ReadSingle();
            TransformM24 = reader.ReadSingle();
            TransformM31 = reader.ReadSingle();
            TransformM32 = reader.ReadSingle();
            TransformM33 = reader.ReadSingle();
            TransformM34 = reader.ReadSingle();
            TransformM41 = reader.ReadSingle();
            TransformM42 = reader.ReadSingle();
            TransformM43 = reader.ReadSingle();
            TransformM44 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, Name );
            writer.WriteInt32( Flags );
            writer.WriteByte( (byte)MinificationFilter );
            writer.WriteByte( (byte)MagnificationFilter );
            writer.WriteByte( (byte)WrapModeU );
            writer.WriteByte( (byte)WrapModeV );
            writer.WriteSingle( TransformM11 );
            writer.WriteSingle( TransformM12 );
            writer.WriteSingle( TransformM13 );
            writer.WriteSingle( TransformM14 );
            writer.WriteSingle( TransformM21 );
            writer.WriteSingle( TransformM22 );
            writer.WriteSingle( TransformM23 );
            writer.WriteSingle( TransformM24 );
            writer.WriteSingle( TransformM31 );
            writer.WriteSingle( TransformM32 );
            writer.WriteSingle( TransformM33 );
            writer.WriteSingle( TransformM34 );
            writer.WriteSingle( TransformM41 );
            writer.WriteSingle( TransformM42 );
            writer.WriteSingle( TransformM43 );
            writer.WriteSingle( TransformM44 );
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
