using GFDLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GFDLibrary.Conversion;

public class ModelConverterOptions
{
    /// <summary>
    /// Gets or sets the version to use for the converted resources.
    /// </summary>
    public uint Version { get; set; } = ResourceVersion.Persona5;

    public object MaterialPreset { get; set; }

    /// <summary>
    /// Gets or sets whether to convert the up axis of the inverse bind pose matrices to Z-up. This is used by Persona 5's battle models for example.
    /// </summary>
    public bool ConvertSkinToZUp { get; set; }

    public bool SetFullBodyNodeProperties { get; set; }

    public bool AutoAddGFDHelperIDs { get; set; } = true;

    public ModelConverterGeometryOptions DefaultGeometryOptions { get; } = new()
    {
        GeometryFlags = GeometryFlags.Bit7,
        VertexAttributeFlags = VertexAttributeFlags.Position | VertexAttributeFlags.Normal | VertexAttributeFlags.TexCoord0
    };

    public Dictionary<string, ModelConverterGeometryOptions> GeometryOptionsOverrideByMeshName { get; set; } = new();

    public Dictionary<string, ModelConverterGeometryOptions> GeometryOptionsOverrideByMaterialName { get; set; } = new();

    public ModelConverterTexCoordChannelOptions[] TexCoordChannelMap { get; } = new ModelConverterTexCoordChannelOptions[]
    {
        new() { SourceChannel = 0 },
        new() { SourceChannel = 1 },
        new() { SourceChannel = 2 },
    };
    public ModelConverterColorChannelOptions[] ColorChannelMap { get; } = new ModelConverterColorChannelOptions[]
    {
        new() { SourceChannel = 0 },
        new() { SourceChannel = 1 },
        new() { SourceChannel = 2 },
    };

    public ModelConverterOptions() { }

    public ModelConverterOptions(ModelPack originalModel)
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
}

public class ModelConverterColorChannelOptions
{
    public int SourceChannel { get; set; }
    public bool UseDefaultColor { get; set; } = true;
    public uint DefaultColor { get; set; } = uint.MaxValue;
}

public class ModelConverterGeometryOptions
{
    public GeometryFlags GeometryFlags { get; set; }
    public VertexAttributeFlags VertexAttributeFlags { get; set; }
}