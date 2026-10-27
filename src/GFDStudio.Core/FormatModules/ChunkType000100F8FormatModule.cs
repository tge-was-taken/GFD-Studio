using System.IO;
using GFDLibrary;
using GFDLibrary.Misc;

namespace GFDStudio.FormatModules
{
    public class ChunkType000100F8FormatModule : FormatModule<ChunkType000100F8>
    {
        public override string Name =>
            "Chunk Type 000100F8";

        public override string[] Extensions =>
            new[] { "00100F8" };

        public override FormatModuleUsageFlags UsageFlags =>
            FormatModuleUsageFlags.Export | FormatModuleUsageFlags.ImportForEditing;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.ChunkType000100F8;
        }

        protected override void ExportCore( ChunkType000100F8 obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override ChunkType000100F8 ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<ChunkType000100F8>( stream );
        }
    }
}