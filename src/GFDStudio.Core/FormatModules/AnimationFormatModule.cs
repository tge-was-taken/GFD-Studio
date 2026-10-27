using System.IO;
using GFDLibrary;
using GFDLibrary.Animations;

namespace GFDStudio.FormatModules
{
    public class AnimationFormatModule : FormatModule<Animation>
    {
        public override string Name =>
            "Animation";

        public override string[] Extensions => 
            new[] { "ganm" };

        public override FormatModuleUsageFlags UsageFlags =>
            FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Animation;
        }

        protected override void ExportCore( Animation obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Animation ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Animation>( stream );
        }
    }
}