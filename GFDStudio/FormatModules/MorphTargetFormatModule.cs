using System.IO;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.FormatModules
{
    public class MorphTargetFormatModule : FormatModule<MorphTarget>
    {
        public override string Name
            => "Morph target";

        public override string[] Extensions
            => new[] { "gmot" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.MorphTarget;
        }

        protected override void ExportCore( MorphTarget obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override MorphTarget ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<MorphTarget>( stream );
        }
    }
}