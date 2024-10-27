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
    /// <summary>
    /// Automatically add appropriate user properties for a given node by name (helper IDs in P5, named user properties in Metaphor)
    /// </summary>
    public bool AutoAddGFDHelperIDs { get; set; }
    /// <summary>
    /// METAPHOR: Import all geometry as an attachment of a matching node. Used in character meshes.
    /// </summary>
    public string CombinedMeshNodeName { get; set; }
    /// <summary>
    /// Define the starting slot that vertex colors should be put in (always starts at 0 for Assimp and FBX SDK)
    /// </summary>
    public int VertexColorStartingSlot { get; set; }

    public ModelConverterOptions()
    {
        Version = ResourceVersion.Persona5;
        ConvertSkinToZUp = false;
        GenerateVertexColors = false;
        MinimalVertexAttributes = true;
        SetFullBodyNodeProperties = false;
        CombinedMeshNodeName = null;
        VertexColorStartingSlot = 0;
    }
}