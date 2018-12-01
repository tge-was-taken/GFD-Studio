using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class KeyType22 : Key
    {
        public override KeyType Type
        {
            get => KeyType.Type22;
            internal set { }
        }

        public int Field04 { get; set; }

        public int Field08 { get; set; }

        public short Field0C { get; set; }

        public float Field10 { get; set; }

        public float Field14 { get; set; }

        public float Field18 { get; set; }

        public int Field1C { get; set; }

        public short Field20 { get; set; }

        public float Field24 { get; set; }

        public float Field28 { get; set; }

        public float Field2C { get; set; }

        public byte Field00 { get; set; }

        public KeyType22() { }

        internal override void Read( ResourceReader reader )
        {
            Field04 = reader.ReadInt32();
            Field08 = reader.ReadInt32();
            Field0C = reader.ReadInt16();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadInt32();
            Field20 = reader.ReadInt16();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field00 = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( Field04 );
            writer.WriteInt32( Field08 );
            writer.WriteInt16( Field0C );
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteInt32( Field1C );
            writer.WriteInt16( Field20 );
            writer.WriteSingle( Field24 );
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteByte( Field00 );
        }
    }
}
