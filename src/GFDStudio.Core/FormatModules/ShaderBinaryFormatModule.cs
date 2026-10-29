using System.IO;
using GFDLibrary.Shaders;

namespace GFDStudio.FormatModules
{
    public class ShaderBinaryFormatModule : FormatModule<ShaderBinary>
    {
        public override string Name
            => "Compiled shader";

        public override string[] Extensions
            => new[] { "sb" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        // Nothing identifies a compiled shader from its contents. It is only reachable by
        // picking it under a shader node, which already knows what it is.
        protected override bool CanImportCore( Stream stream, string filename = null ) => false;

        protected override void ExportCore( ShaderBinary obj, Stream stream, string filename = null )
        {
            stream.Write( obj.Data, 0, obj.Data.Length );
        }

        protected override ShaderBinary ImportCore( Stream stream, string filename = null )
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo( memoryStream );
            return new ShaderBinary( memoryStream.ToArray() );
        }
    }
}
