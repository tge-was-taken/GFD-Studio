using System.Drawing;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;
using GFDLibrary.Textures;
using GFDLibrary.Textures.GNF;

namespace GFDStudio.FormatModules
{
    public class GNFTextureFormatModule : FormatModule<GNFTexture>
    {
        public override string Name
            => "GNF Texture (PS4)";

        public override string[] Extensions
            => new[] { "dds", "gnf" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Bitmap | FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, true, Endianness.LittleEndian ) )
            {
                bool isValid = reader.ReadInt32() == GNFTexture.MAGIC;
                reader.BaseStream.Position = 0;
                return isValid;
            }
        }

        protected override void ExportCore( GNFTexture obj, Stream stream, string filename = null )
        {
            obj.Save( stream );
        }

        protected override GNFTexture ImportCore( Stream stream, string filename = null )
        {
            return new GNFTexture( stream );
        }

        protected override Bitmap GetBitmapCore( GNFTexture obj )
        {
            return TextureDecoder.Decode( obj );
        }
    }
}