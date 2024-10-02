using GFDLibrary.Animations;

namespace GFDLibrary.Conversion
{
    public abstract class AnimationExporter
    {
        public abstract void Export( Animation animation, string filePath, AnimationConverterOptions config );
    }
}
