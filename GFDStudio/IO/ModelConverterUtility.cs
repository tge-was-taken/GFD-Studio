using System.Linq;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Conversion.AssimpNet;
using GFDLibrary.Models;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Forms;

namespace GFDStudio.IO
{
    public static class ModelConverterUtility
    {
        public static ModelPack ConvertAssimpModel(ModelPack originalModel)
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.ImportForEditing, typeof( AssimpScene ) ).Filter;
                dialog.Multiselect = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Select an Assimp model.";
                dialog.ValidateNames = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return null;
                }

                return ConvertAssimpModel( dialog.FileName, originalModel );
            }
        }

        public static ModelPack ConvertAssimpModel( string path, ModelPack originalModel)
        {
            using ( var dialog = new ModelConverterOptionsDialog( false ) )
            {
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                var options = new ModelConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version,
                    ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                    AutoAddGFDHelperIDs = dialog.AutoAddGFDHelperIDs
                };
                if (originalModel.Materials?.Count > 0)
                {
                    foreach ( var material in originalModel.Materials )
                    {
                        var meshesWithMaterial = originalModel.Model.Nodes
                            .Where( n => n.HasAttachments )
                            .SelectMany( n => n.Attachments )
                            .Where( a => a.Type == GFDLibrary.Models.NodeAttachmentType.Mesh )
                            .Select( a => a.GetValue<Mesh>() )
                            .Where( m => m.MaterialName == material.Key );    
                        var firstMesh = meshesWithMaterial.FirstOrDefault();
                        if (firstMesh is not null)
                        {
                            options.GeometryOptionsByMaterial[material.Key] = new()
                            {
                                GeometryFlags = firstMesh.Flags,
                                VertexAttributeFlags = firstMesh.VertexAttributeFlags,
                            };
                        }
                    }
                }

                return AssimpNetModelPackConverter.ConvertFromAssimpScene( path, options );
            }
        }
    }
}
