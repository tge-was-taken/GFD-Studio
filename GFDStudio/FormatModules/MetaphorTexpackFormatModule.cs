using GFDLibrary.Textures.Texpack;
using System.IO;

namespace GFDStudio.FormatModules
{
    internal class MetaphorTexpackFormatModule : FormatModule<MetaphorTexpack>
    {
        public override string Name
            => "Texture Pack (Metaphor: Refantazio)";

        public override string[] Extensions
            => new[] { "tex" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Import;

        // There's no real structure to it to efficiently determine validity, it's just several texture files glued together
        protected override bool CanImportCore( Stream stream, string filename = null ) => true;

        protected override void ExportCore( MetaphorTexpack obj, Stream stream, string filename = null )
            => obj.Save( stream );

        protected override MetaphorTexpack ImportCore( Stream stream, string filename = null )
            => new MetaphorTexpack( stream );
    }
}
