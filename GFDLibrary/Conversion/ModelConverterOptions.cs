using GFDLibrary.Graphics;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using System.Collections.Generic;

namespace GFDLibrary.Conversion;

public class ModelConverterOptions
{
    public uint Version { get; set; } = ResourceVersion.Persona5;

    public bool ConvertSkinToZUp { get; set; }

    public bool SetFullBodyNodeProperties { get; set; }

    public bool AutoAddGFDHelperIDs { get; set; } = true;

    public ModelConverterMaterialOptions DefaultMaterial { get; init; } = new();

    public Dictionary<string, ModelConverterMeshOptions> Meshes { get; set; } = new();

    public Dictionary<string, ModelConverterMaterialOptions> Materials { get; set; } = new();

    public ModelConverterOptions() { }
}

public class ModelConverterMaterialOptions
{
    public Material Preset { get; set; }
    public ModelConverterMeshOptions Mesh { get; set; } = new();
}

public class ModelConverterTexCoordChannelOptions
{
    public int SourceChannel { get; set; }

    public override string ToString()
    {
        return $"TexCoord Channel {SourceChannel}";
    }
}

public class ModelConverterColorChannelOptions
{
    public int SourceChannel { get; set; }

    public bool UseDefaultColor { get; set; } = true;

    public Graphics.Color DefaultColor { get; set; } = Graphics.Color.White;

    public ColorSwizzle Swizzle { get; set; } = new()
    {
        Red = ColorChannel.Red,
        Green = ColorChannel.Green,
        Blue = ColorChannel.Blue,
        Alpha = ColorChannel.Alpha,
    };

    public override string ToString()
    {
        return $"Color Channel {SourceChannel}";
    }
}

public class ModelConverterMeshOptions
{
    public GeometryFlags GeometryFlags { get; set; } 
        = GeometryFlags.Bit7;
    public VertexAttributeFlags VertexAttributeFlags { get; set; } 
        = VertexAttributeFlags.Position | VertexAttributeFlags.Normal | VertexAttributeFlags.TexCoord0;

    public ModelConverterTexCoordChannelOptions[] TexCoordChannelMap { get; init; } = new ModelConverterTexCoordChannelOptions[]
    {
        new() { SourceChannel = 0 },
        new() { SourceChannel = 1 },
        new() { SourceChannel = 2 },
    };

    public ModelConverterColorChannelOptions[] ColorChannelMap { get; init; } = new ModelConverterColorChannelOptions[]
    {
        new() { SourceChannel = 0 },
        new() { SourceChannel = 1 },
        new() { SourceChannel = 2 },
    };
}