using System.Drawing;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;
using GFDLibrary.Textures;

namespace GFDStudio.FormatModules
{
    public class FieldTexturePS3FormatModule : FormatModule<FieldTexturePS3>
    {
        public override string Name
            => "Field Texture (PS3)";

        public override string[] Extensions
            => new[] { "dds" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Bitmap | FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, true, Endianness.BigEndian ) )
            {
                bool isValid = reader.ReadInt32() == 0x020200FF;
                reader.BaseStream.Position = 0;
                return isValid;
            }
        }

        protected override void ExportCore( FieldTexturePS3 obj, Stream stream, string filename = null )
        {
            obj.Save( stream );
        }

        protected override FieldTexturePS3 ImportCore( Stream stream, string filename = null )
        {
            return new FieldTexturePS3( stream );
        }

        protected override Bitmap GetBitmapCore( FieldTexturePS3 obj )
        {
            return TextureDecoder.Decode( obj );
        }
    }
}