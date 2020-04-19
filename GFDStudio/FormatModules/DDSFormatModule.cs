using System.Drawing;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;
using GFDLibrary.Textures.DDS;

namespace GFDStudio.FormatModules
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class DDSFormatModule : FormatModule<DDSStream>
#pragma warning restore S101 // Types should be named in PascalCase
    {
        public override string Name
            => "DDS texture";

        public override string[] Extensions
            => new[] { "dds" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Bitmap | FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, true, Endianness.LittleEndian ) )
            {
                bool isValid = reader.ReadInt32() == DDSHeader.MAGIC;
                reader.BaseStream.Position = 0;
                return isValid;
            }
        }

        protected override void ExportCore( DDSStream obj, Stream stream, string filename = null )
        {
            obj.Save( stream );
        }

        protected override DDSStream ImportCore( Stream stream, string filename = null )
        {
            return new DDSStream( stream );
        }

        protected override Bitmap GetBitmapCore( DDSStream obj )
        {
            return DDSCodec.DecompressImage( obj );
        }
    }
}