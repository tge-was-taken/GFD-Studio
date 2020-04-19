using System.IO;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.FormatModules
{
    public class MorphTargetListFormatModule : FormatModule<MorphTargetList>
    {
        public override string Name
            => "Morph target list";

        public override string[] Extensions
            => new[] { "gmotl" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.MorphTargetList;
        }

        protected override void ExportCore( MorphTargetList obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override MorphTargetList ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<MorphTargetList>( stream );
        }
    }
}