using GFDLibrary.Textures;
using GFDStudio.GUI;

namespace GFDStudio.IO
{
    public static class TextureReplacement
    {
        /// <summary>
        /// Stamps a replacement texture with the version it is going into. From Metaphor onwards
        /// the data can sit in a file beside the model instead, which the user can ask for.
        /// </summary>
        public static Texture Prepare( uint version, Texture texture )
        {
            texture.Version = version;

            if ( UserOptions.Current.SaveReplacedTexturesExternally && texture.Version >= 0x2000000 )
                texture.IsTexSourceExternal = true;

            return texture;
        }
    }
}
