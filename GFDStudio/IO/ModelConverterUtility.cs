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

                var options = new ModelConverterOptions(originalModel)
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version,
                    ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                    AutoAddGFDHelperIDs = dialog.AutoAddGFDHelperIDs
                };
                return AssimpNetModelPackConverter.ConvertFromAssimpScene( path, options );
            }
        }
    }
}
