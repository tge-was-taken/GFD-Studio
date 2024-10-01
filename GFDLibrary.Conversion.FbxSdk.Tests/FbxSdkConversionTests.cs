using GFDLibrary;

namespace GFDLibrary.Conversion.FbxSdk.Tests;

[TestClass]
public class FbxSdkConversionTests
{
    private const string TestDataRoot = "..\\..\\..\\..\\Resources\\TestData";

    [TestMethod]
    public void CanExportWithoutException()
    {
        var modelPack = Resource.Load<ModelPack>( Path.Combine( TestDataRoot, "C0001_001_00.GMD" ) );
        FbxSdkModelPackExporter.ExportFile( modelPack, "out.fbx", new FbxSdkModelPackExporterConfig() );
    }

    [TestMethod]
    public void CanExportWithoutException2()
    {
        var modelPack = Resource.Load<ModelPack>( Path.Combine( TestDataRoot, "F062_010_0.GFS" ) );
        FbxSdkModelPackExporter.ExportFile( modelPack, "out2.fbx", new FbxSdkModelPackExporterConfig() );
    }
}