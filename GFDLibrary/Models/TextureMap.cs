using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class TextureMap : Resource
    {
        public override ResourceType ResourceType => ResourceType.TextureMap;

        public string Name { get; set; }

        // 0x44
        public int Field44 { get; set; }

        // 0x48
        public byte Field48 { get; set; }

        // 0x49
        public byte Field49 { get; set; }

        // 0x4A
        public byte Field4A { get; set; }

        // 0x4B
        public byte Field4B { get; set; }
        
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
            Field44 = 0;
            Field48 = 1;
            Field49 = 1;
            Field4A = 0;
            Field4B = 0;
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
            Field44 = reader.ReadInt32();
            Field48 = reader.ReadByte();
            Field49 = reader.ReadByte();
            Field4A = reader.ReadByte();
            Field4B = reader.ReadByte();
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
            writer.WriteInt32( Field44 );
            writer.WriteByte( Field48 );
            writer.WriteByte( Field49 );
            writer.WriteByte( Field4A );
            writer.WriteByte( Field4B );
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
}
