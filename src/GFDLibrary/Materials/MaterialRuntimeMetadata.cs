using GFDLibrary.Models;

namespace GFDLibrary.Materials;

/// <summary>
/// Describes additional metadata about a live instance of a material, how it's used, etc.
/// </summary>
public class MaterialRuntimeMetadata
{
    public GeometryFlags? GeometryFlags { get; set; }
    public VertexAttributeFlags? VertexAttributeFlags { get; set; }
    public bool IsCustomMaterial { get; set; }
}
