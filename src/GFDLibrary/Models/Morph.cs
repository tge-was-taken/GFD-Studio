using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class Morph : Resource
    {
        public override ResourceType ResourceType => ResourceType.Morph;

        public int TargetCount => Weights.Length;

        /// <summary>
        /// One weight per morph target. The game reads these with its float array reader, so
        /// they were only ever integers here by mistake.
        /// </summary>
        public float[] Weights { get; set; }

        public string NodeName { get; set; }

        public Morph()
        {
            
        }

        public Morph(uint version) :base(version)
        {
                
        }

        protected override void ReadCore( ResourceReader reader )
        {
            int morphTargetCount = reader.ReadInt32();

            Weights = new float[morphTargetCount];
            for ( int i = 0; i < Weights.Length; i++ )
                Weights[i] = reader.ReadSingle();

            NodeName = reader.ReadStringWithHash( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( TargetCount );

            foreach ( var weight in Weights )
                writer.WriteSingle( weight );

            writer.WriteStringWithHash( Version, NodeName );
        }
    }
}