using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class KeyType31Dancing : PRSKey, IKeyType31
    {
        public KeyType31Type SubType => KeyType31Type.Dancing;

        public KeyType31Dancing() : base( KeyType.Type31 )
        {
            
        }

        internal override void Read( ResourceReader reader )
        {
            Position = reader.ReadVector3Half();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteVector3Half( Position );
        }
    }
}