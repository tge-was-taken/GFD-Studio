using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class SingleKey : Key
    {
        public float Value { get; set; }

        public SingleKey() : this( KeyType.Single ) { }

        public SingleKey( KeyType type ) : base( type )
        {
        }

        internal override void Read( ResourceReader reader )
        {
            Value = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Value );
        }
    }
}