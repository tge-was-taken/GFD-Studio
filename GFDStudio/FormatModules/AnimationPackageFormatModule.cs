using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class AnimationPackageFormatModule : FormatModule<AnimationPackage>
    {
        public override string Name =>
            "Animation Package";

        public override string[] Extensions =>
            new[] { "gap" };

        public override FormatModuleUsageFlags UsageFlags =>
            FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Model;
        }

        protected override void ExportCore( AnimationPackage obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override AnimationPackage ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<AnimationPackage>( stream );
        }
    }
}