using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class Morph : Resource
    {
        public override ResourceType ResourceType => ResourceType.Morph;

        public int TargetCount => TargetInts.Length;

        public int[] TargetInts { get; set; }

        public string NodeName { get; set; }

        public Morph()
        {
            
        }

        public Morph(uint version) :base(version)
        {
                
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            int morphTargetCount = reader.ReadInt32();

            TargetInts = new int[morphTargetCount];
            for ( int i = 0; i < TargetInts.Length; i++ )
                TargetInts[i] = reader.ReadInt32();

            NodeName = reader.ReadStringWithHash( Version );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( TargetCount );

            foreach ( int t in TargetInts )
                writer.WriteInt32( t );

            writer.WriteStringWithHash( Version, NodeName );
        }
    }
}