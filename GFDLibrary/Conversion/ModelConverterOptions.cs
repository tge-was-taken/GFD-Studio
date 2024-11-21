using GFDLibrary.Models;
using System.Collections.Generic;

namespace GFDLibrary.Conversion;

public class ModelConverterOptions
{
    /// <summary>
    /// Gets or sets the version to use for the converted resources.
    /// </summary>
    public uint Version { get; set; }

    public object MaterialPreset { get; set; }

    /// <summary>
    /// Gets or sets whether to convert the up axis of the inverse bind pose matrices to Z-up. This is used by Persona 5's battle models for example.
    /// </summary>
    public bool ConvertSkinToZUp { get; set; }

    /// <summary>
    /// Gets or sets whether to generate dummy (white) vertex colors if they're not already present. Some material shaders rely on vertex colors being present, and the lack of them will cause graphics corruption.
    /// </summary>
    public bool GenerateVertexColors { get; set; }

    /// <summary>
    /// Gets or sets whether to generate dummy (white) vertex colors if they're not already present. Some material shaders rely on vertex colors being present, and the lack of them will cause graphics corruption.
    /// </summary>
    public bool MinimalVertexAttributes { get; set; }

    public bool SetFullBodyNodeProperties { get; set; }

    public bool AutoAddGFDHelperIDs { get; set; }

    public Dictionary<string, ModelConverterGeometryOptions> GeometryOptionsByMaterial { get; set; }

    public ModelConverterOptions()
    {
        Version = ResourceVersion.Persona5;
        ConvertSkinToZUp = false;
        GenerateVertexColors = false;
        MinimalVertexAttributes = true;
        SetFullBodyNodeProperties = false;
        GeometryOptionsByMaterial = new();
    }
}

public class ModelConverterGeometryOptions
{
    public GeometryFlags GeometryFlags { get; set; }
    public VertexAttributeFlags VertexAttributeFlags { get; set; }
}