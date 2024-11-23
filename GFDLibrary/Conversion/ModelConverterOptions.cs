using GFDLibrary.Graphics;
using GFDLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace GFDLibrary.Conversion;

public class ModelConverterOptions
{
    public uint Version { get; set; } = ResourceVersion.Persona5;

    public object MaterialPreset { get; set; }

    public bool ConvertSkinToZUp { get; set; }

    public bool SetFullBodyNodeProperties { get; set; }

    public bool AutoAddGFDHelperIDs { get; set; } = true;

    public ModelConverterGeometryOptions DefaultGeometryOptions { get; init; } = new()
    {
        GeometryFlags = GeometryFlags.Bit7,
        VertexAttributeFlags = VertexAttributeFlags.Position | VertexAttributeFlags.Normal | VertexAttributeFlags.TexCoord0
    };

    public Dictionary<string, ModelConverterGeometryOptions> GeometryOptionsOverrideByMeshName { get; set; } = new();

    public Dictionary<string, ModelConverterGeometryOptions> GeometryOptionsOverrideByMaterialName { get; set; } = new();

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

    public ModelConverterOptions() { }

    public ModelConverterOptions( ModelPack originalModel )
    {
        if ( originalModel?.Model is not null )
        {
            foreach ( var node in originalModel.Model.Nodes )
            {
                for ( int i = 0; i < node.Attachments.Count; i++ )
                {
                    if ( node.Attachments[i].Type == NodeAttachmentType.Mesh )
                    {
                        var meshName = ModelConversionHelpers.GetMeshAttachmentName( node.Name, i );
                        var mesh = node.Attachments[i].GetValue<Mesh>();
                        GeometryOptionsOverrideByMeshName[meshName] = new()
                        {
                            GeometryFlags = mesh.Flags,
                            VertexAttributeFlags = mesh.VertexAttributeFlags,
                        };
                    }
                }
            }

            if ( originalModel.Materials?.Count > 0 )
            {
                foreach ( var material in originalModel.Materials )
                {
                    var meshesWithMaterial = originalModel.Model.Meshes.Where( m => m.MaterialName == material.Key );
                    var firstMesh = meshesWithMaterial.FirstOrDefault();
                    if ( firstMesh is not null )
                    {
                        GeometryOptionsOverrideByMaterialName[material.Key] = new()
                        {
                            GeometryFlags = firstMesh.Flags,
                            VertexAttributeFlags = firstMesh.VertexAttributeFlags,
                        };
                    }
                }
            }
        }
    }
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

public class ModelConverterGeometryOptions
{
    public GeometryFlags GeometryFlags { get; set; }

    public VertexAttributeFlags VertexAttributeFlags { get; set; }
}