using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class ChunkType000100F9FormatModule : FormatModule<ChunkType000100F9>
    {
        public override string Name =>
            "Chunk Type 000100F9";

        public override string[] Extensions =>
            new[] { "00100F9" };

        public override FormatModuleUsageFlags UsageFlags =>
            FormatModuleUsageFlags.Export | FormatModuleUsageFlags.ImportForEditing;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.ChunkType000100F9;
        }

        protected override void ExportCore( ChunkType000100F9 obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override ChunkType000100F9 ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<ChunkType000100F9>( stream );
        }
    }
}