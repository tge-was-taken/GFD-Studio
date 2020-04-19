using System.Collections.Generic;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public abstract class Key
    {
        public KeyType Type { get; }

        public float Time { get; set; }

        protected Key( KeyType type )
        {
            Type = type;
        } 

        internal abstract void Read( ResourceReader reader );
        internal abstract void Write( ResourceWriter writer );
    }
}