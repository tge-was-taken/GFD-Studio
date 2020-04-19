using GFDLibrary.IO;

namespace GFDLibrary.Effects
{
    public sealed class EplLeaf : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeaf;

        public EplLeaf()
        {
        }

        public EplLeaf( uint version ) : base( version )
        {
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            throw new System.NotImplementedException();
        }

        internal override void Write( ResourceWriter writer )
        {
            throw new System.NotImplementedException();
        }
    }
}