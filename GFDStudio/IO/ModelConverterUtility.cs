using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Forms;

namespace GFDStudio.IO
{
    public static class ModelConverterUtility
    {
        public static Model ConvertAssimpModel()
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.ImportForEditing, typeof( Assimp.Scene ) );
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

        public static Model ConvertAssimpModel( string path )
        {
            using ( var dialog = new ModelConverterOptionsDialog( false ) )
            {
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                ModelConverterOptions options = new ModelConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version,
                    ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                    GenerateVertexColors = dialog.GenerateVertexColors
                };

                return ModelConverter.ConvertFromAssimpScene( path, options );
            }
        }
    }
}
