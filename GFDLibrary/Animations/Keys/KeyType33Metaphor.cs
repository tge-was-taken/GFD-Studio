using GFDLibrary.IO;

namespace GFDLibrary.Animations.Keys
{
    public class KeyType33Metaphor : Key
    {
        public float Field00 {get; set; }
        public float Field04 {get; set; }
        public float Field08 {get; set; }
        public float Field0c {get; set; }
        public float Field10 { get; set; }

        public KeyType33Metaphor() : base( KeyType.NodeSHalf )
        {
        }

        internal override void Read( ResourceReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0c = reader.ReadSingle();
            Field10 = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle(Field00);
            writer.WriteSingle(Field04);
            writer.WriteSingle(Field08);
            writer.WriteSingle(Field0c);
            writer.WriteSingle(Field10);
        }
    }
}
