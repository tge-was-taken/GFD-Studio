using System.Linq;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Conversion.AssimpNet;
using GFDLibrary.Conversion.AssimpNet.Utilities;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDStudio.FormatModules;
using GFDStudio.GUI;

namespace GFDStudio.IO
{
    public static class ModelConverterUtility
    {
        public static ModelPack ConvertAssimpModel(ModelPack originalModel)
        {
            var result = UserDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select an Assimp model.",
                Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.ImportForEditing, typeof( AssimpScene ) ).Filter,
            } );

            return result == null ? null : ConvertAssimpModel( result.FileName, originalModel );
        }

        public static ModelPack ConvertAssimpModel( string path, ModelPack originalModel)
        {
            var scene = AssimpHelper.ImportScene( path );
            var options = UserDialogs.Current.AskModelConversionOptions( scene, originalModel );
            return options == null ? null : AssimpNetModelPackConverter.ConvertFromAssimpScene( path, options );
        }
    }
}
