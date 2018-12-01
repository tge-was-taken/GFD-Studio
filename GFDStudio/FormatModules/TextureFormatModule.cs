using System;
using System.Drawing;
using System.IO;
using GFDLibrary;
using GFDLibrary.Textures;

namespace GFDStudio.FormatModules
{
    public class TextureFormatModule : FormatModule<Texture>
    {
        public override string Name
            => "Texture";

        public override string[] Extensions
            => new[] { "gtx" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Bitmap;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return false;
        }

        protected override void ExportCore( Texture obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Texture ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Texture>( stream, true );
        }

        protected override Bitmap GetBitmapCore( Texture obj )
        {
            return TextureDecoder.Decode( obj );
        }
    }
}
