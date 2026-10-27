using System.Linq;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDLibrary.Models;

namespace GFDStudio.GUI;

/// <summary>
/// Works out the conversion options to offer for an imported scene, taking what it can from the
/// model being replaced.
/// </summary>
public static class ModelConverterOptionsBuilder
{
    public static ModelConverterOptionsViewModel Build( Assimp.Scene scene, ModelPack originalModel )
    {
        var viewModel = new ModelConverterOptionsViewModel();

        if ( scene is not null )
        {
            foreach ( var item in scene.Materials )
            {
                var options = new ModelConverterMaterialOptionsViewModel( viewModel )
                {
                    Name = item.Name,
                    InheritDefaults = true,
                    Mesh = null,
                    Preset = null,
                };

                if ( ( originalModel?.Materials?.Any() ?? false ) &&
                     originalModel.Materials.TryGetValue( item.Name, out var referenceMaterial ) )
                {
                    options.InheritDefaults = false;
                    options.Preset = new ModelConverterMaterialPresetViewModel( referenceMaterial );
                }

                viewModel.Materials.Add( options );
            }

            foreach ( var item in scene.Meshes )
            {
                var options = new ModelConverterMeshOptionsViewModel
                {
                    Name = item.Name,
                    InheritDefaults = true,
                    InheritChannelSettingsDefaults = true,
                    GeometryFlags = null,
                    VertexAttributeFlags = null,
                };

                if ( originalModel is not null )
                {
                    var referenceMesh = FindReferenceMesh( scene, originalModel, item );
                    if ( referenceMesh is not null )
                    {
                        options.InheritDefaults = false;
                        options.GeometryFlags = referenceMesh.Flags;
                        options.VertexAttributeFlags = referenceMesh.VertexAttributeFlags;
                    }
                }

                viewModel.Meshes.Add( options );
            }
        }

        if ( originalModel is not null )
        {
            viewModel.Version = originalModel.Version;

            foreach ( var material in originalModel.Materials.Values )
                viewModel.AdditionalMaterialPresets.Add( Material.ConvertToMaterialPreset( material, material ) );
        }

        return viewModel;
    }

    /// <summary>
    /// Matches an imported mesh to one in the original model, by the name the exporter would have
    /// given it, and failing that by the material it uses.
    /// </summary>
    private static Mesh FindReferenceMesh( Assimp.Scene scene, ModelPack originalModel, Assimp.Mesh item )
    {
        if ( originalModel?.Model is null )
            return null;

        foreach ( var node in originalModel.Model.Nodes )
        {
            for ( var i = 0; i < node.Attachments.Count; i++ )
            {
                if ( node.Attachments[i].Type != NodeAttachmentType.Mesh )
                    continue;

                if ( item.Name == ModelConversionHelpers.GetMeshExportName( node.Name, i ) )
                    return node.Attachments[i].GetValue<Mesh>();
            }
        }

        return originalModel.Model.Meshes
                            .FirstOrDefault( x => x.MaterialName == scene.Materials[item.MaterialIndex].Name );
    }
}
