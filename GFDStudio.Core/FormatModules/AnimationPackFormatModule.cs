using System.IO;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDStudio.GUI;

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
            return Resource.GetResourceType( stream ) == ResourceType.ModelPack
                || Resource.GetResourceType( stream ) == ResourceType.ModelPack_Metaphor
                || Resource.GetResourceType( stream ) == ResourceType.ModelPack_Metaphor_BIG_ENDIAN;
        }

        protected override void ExportCore( AnimationPack obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override AnimationPack ImportCore( Stream stream, string filename = null )
        {
            var pack = Resource.Load<AnimationPack>( stream );
            if ( pack != null && pack.ErrorsOccuredDuringLoad )
                UserDialogs.Current.ShowMessage( "One or more error(s) occured while loading the animation pack. The data may be truncated, or otherwise corrupted",
                                                 "Warning", UserMessageKind.Warning );

            return pack;
        }
    }
}