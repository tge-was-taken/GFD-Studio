using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class KeyType31FullBody : PRSKey, IKeyType31
    {
        public KeyType31Type SubType => KeyType31Type.FullBody;

        public byte[] Data { get; set; }

        public KeyType31FullBody() : base( KeyType.Type31 )
        {
        }
        /*
        internal override void Read( ResourceReader reader )
        {
            Data = reader.ReadBytes( 0x1C );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteBytes( Data );
        }
        */
    }
}