using GFDLibrary.Animations;
using System.IO;

namespace GFDLibrary.Conversion
{
    public abstract class AnimationImporter
    {
        public abstract Animation Import( string filePath, AnimationConverterOptions config );
    }
}
