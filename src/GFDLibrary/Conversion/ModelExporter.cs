namespace GFDLibrary.Conversion
{
    public abstract class ModelExporter
    {
        public abstract void Export( ModelPack modelPack, string filePath, ModelConverterOptions config );
    }
}
