using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public abstract class Key
    {
        public abstract KeyType Type { get; internal set; }

        internal abstract void Read( ResourceReader reader );
        internal abstract void Write( ResourceWriter writer );
    }
}