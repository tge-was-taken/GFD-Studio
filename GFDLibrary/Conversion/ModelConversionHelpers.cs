namespace GFDLibrary.Conversion;

public static class ModelConversionHelpers
{
    public static readonly string MeshAttachmentNameSuffix = $"_gfdMesh_";
    public static string GetMeshAttachmentName( string nodeName, int meshIndex )
        => $"{nodeName}{MeshAttachmentNameSuffix}{meshIndex}";
}
