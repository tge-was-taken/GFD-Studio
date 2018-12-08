using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Models.Conversion;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Forms;

namespace GFDStudio.IO
{
    public static class ModelConverterUtility
    {
        public static ModelPack ConvertAssimpModel()
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.ImportForEditing, typeof( Assimp.Scene ) ).Filter;
                dialog.Multiselect = false;
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Select an Assimp model.";
                dialog.ValidateNames = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                {
                    return null;
                }

                return ConvertAssimpModel( dialog.FileName );
            }
        }

        public static ModelPack ConvertAssimpModel( string path )
        {
            using ( var dialog = new ModelConverterOptionsDialog( false ) )
            {
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                ModelPackConverterOptions options = new ModelPackConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version,
                    ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                    GenerateVertexColors = dialog.GenerateVertexColors
                };

                return ModelPackConverter.ConvertFromAssimpScene( path, options );
            }
        }
    }
}
