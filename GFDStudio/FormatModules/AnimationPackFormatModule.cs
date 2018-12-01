using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;

namespace GFDStudio.FormatModules
{
    public class AnimationPackFormatModule : FormatModule<AnimationPack>
    {
        public override string Name =>
            "Animation Pack";

        public override string[] Extensions =>
            new[] { "gap" };

        public override FormatModuleUsageFlags UsageFlags =>
            FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.ModelPack;
        }

        protected override void ExportCore( AnimationPack obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override AnimationPack ImportCore( Stream stream, string filename = null )
        {
            var pack = Resource.Load<AnimationPack>( stream );
            if ( pack != null && pack.ErrorsOccuredDuringLoad )
                MessageBox.Show( "One or more error(s) occured while loading the animation pack. The data may be truncated, or otherwise corrupted",
                                 "Warning", MessageBoxButtons.OK );

            return pack;
        }
    }
}