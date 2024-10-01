namespace GFDLibrary.Conversion
{
    public abstract class ModelImporter
    {
        public abstract ModelPack Import( string filePath, ModelConverterOptions config );
    }
}
