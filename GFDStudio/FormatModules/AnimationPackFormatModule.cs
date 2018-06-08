using System.IO;
using System.Windows.Forms;
using GFDLibrary;

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
            return Resource.GetResourceType( stream ) == ResourceType.Model;
        }

        protected override void ExportCore( AnimationPack obj, Stream stream, string filename = null )
        {
            // Wrap it in a model and save it
            var model = new Model( obj.Version ) { AnimationPack = obj };
            model.Save( stream, true );
        }

        protected override AnimationPack ImportCore( Stream stream, string filename = null )
        {
            var pack = Resource.Load<Model>( stream ).AnimationPack;
            if ( pack != null && pack.ErrorsOccuredDuringLoad )
                MessageBox.Show( "One or more error(s) occured while loading the animation pack. The data may be truncated, or otherwise incorrect",
                                 "Warning", MessageBoxButtons.OK );

            return pack;
        }
    }
}