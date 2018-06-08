using GFDLibrary.IO;

namespace GFDLibrary
{
    public abstract class Keyframe
    {
        public abstract KeyframeType Type { get; internal set; }

        internal abstract void Read( ResourceReader reader );
        internal abstract void Write( ResourceWriter writer );
    }
}