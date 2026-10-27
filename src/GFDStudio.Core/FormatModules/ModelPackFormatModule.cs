using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class ModelPackFormatModule : FormatModule<ModelPack>
    {
        public override string Name =>
            "Model pack";

        public override string[] Extensions =>
            new[] { "gfs", "gmd" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.ModelPack
                || Resource.GetResourceType( stream ) == ResourceType.ModelPack_Metaphor
                || Resource.GetResourceType( stream ) == ResourceType.ModelPack_Metaphor_BIG_ENDIAN;
        }

        protected override void ExportCore( ModelPack obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override ModelPack ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<ModelPack>( stream );
        }
    }
}
