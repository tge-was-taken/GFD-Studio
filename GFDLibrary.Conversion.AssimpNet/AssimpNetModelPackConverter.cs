using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GFDLibrary.Conversion.AssimpNet.Utilities;
using GFDLibrary.Materials;
using GFDLibrary.Textures;
using Ai = Assimp;

namespace GFDLibrary.Conversion.AssimpNet
{
    public static class AssimpNetModelPackConverter
    {
        public static ModelPack ConvertFromAssimpScene( string filePath, ModelConverterOptions options )
        {
            // For importing textures
            string baseDirectoryPath = Path.GetDirectoryName( filePath );

            // Import scene
            var aiScene = AssimpSceneImporter.ImportFile( filePath );

            // Build textures & Materials
            var model = new ModelPack( options.Version );
            model.Textures = new TextureDictionary( options.Version );
            model.Materials = new MaterialDictionary( options.Version );

            foreach ( var aiSceneMaterial in aiScene.Materials )
            {
                var optionsTemp = options;
                Material mat = ConvertMaterialAndTextures( aiSceneMaterial, optionsTemp, baseDirectoryPath, model.Textures );
                model.Materials.Add( mat );

                Trace.TraceInformation( "ModelPackConverter -> Material added: " + mat );
            }

            // Create scene
            model.Model = AssimpNetModelConverter.ConvertFromAssimpScene( aiScene, options );

            return model;
        }

        private static Material ConvertMaterialAndTextures( Ai.Material aiMaterial, ModelConverterOptions options, string baseDirectoryPath, TextureDictionary textureDictionary )
        {
            // Convert all textures
            TextureInfo diffuseTexture = null;
            if ( aiMaterial.HasTextureDiffuse )
                diffuseTexture = ConvertTexture( aiMaterial.TextureDiffuse, baseDirectoryPath );

            TextureInfo lightmapTexture = null;
            if ( aiMaterial.HasTextureLightMap )
                lightmapTexture = ConvertTexture( aiMaterial.TextureLightMap, baseDirectoryPath );

            TextureInfo displacementTexture = null;
            if ( aiMaterial.HasTextureDisplacement )
                displacementTexture = ConvertTexture( aiMaterial.TextureDisplacement, baseDirectoryPath );

            TextureInfo opacityTexture = null;
            if ( aiMaterial.HasTextureOpacity )
                opacityTexture = ConvertTexture( aiMaterial.TextureOpacity, baseDirectoryPath );

            TextureInfo normalTexture = null;
            if ( aiMaterial.HasTextureNormal )
                normalTexture = ConvertTexture( aiMaterial.TextureNormal, baseDirectoryPath );

            TextureInfo heightTexture = null;
            if ( aiMaterial.HasTextureHeight )
                heightTexture = ConvertTexture( aiMaterial.TextureHeight, baseDirectoryPath );

            TextureInfo emissiveTexture = null;
            if ( aiMaterial.HasTextureEmissive )
                emissiveTexture = ConvertTexture( aiMaterial.TextureEmissive, baseDirectoryPath );

            TextureInfo ambientTexture = null;
            if ( aiMaterial.HasTextureAmbient )
                ambientTexture = ConvertTexture( aiMaterial.TextureAmbient, baseDirectoryPath );

            TextureInfo specularTexture = null;
            if ( aiMaterial.HasTextureSpecular )
                specularTexture = ConvertTexture( aiMaterial.TextureSpecular, baseDirectoryPath );

            TextureInfo reflectionTexture = null;
            if ( aiMaterial.HasTextureReflection )
                reflectionTexture = ConvertTexture( aiMaterial.TextureReflection, baseDirectoryPath );

            // Convert material
            string materialName = AssimpConverterCommon.UnescapeName( aiMaterial.Name );
            Material material = new Material( materialName );

            if ( diffuseTexture != null )
            {
                textureDictionary.Add( diffuseTexture.Texture );
                material = MaterialFactory.CreateMaterial( materialName, diffuseTexture.Name, options );
            }

            return material;
        }

        private static TextureInfo ConvertTexture( Ai.TextureSlot aiTextureSlot, string baseDirectoryPath )
        {
            var relativeFilePath = aiTextureSlot.FilePath;
            var fullFilePath = Path.GetFullPath( Path.Combine( baseDirectoryPath, relativeFilePath ) );
            var textureName = AssimpConverterCommon.UnescapeName( Path.GetFileNameWithoutExtension( relativeFilePath ) + ".dds" );

            Texture texture;
            if ( !File.Exists( fullFilePath ) )
            {
                texture = Texture.CreateDefaultTexture( textureName );
            }
            else if ( relativeFilePath.EndsWith( ".dds", StringComparison.InvariantCultureIgnoreCase ) )
            {
                texture = new Texture( textureName, TextureFormat.DDS, File.ReadAllBytes( fullFilePath ) );

            }
            else
            {
                var bitmap = new Bitmap( fullFilePath );
                texture = TextureEncoder.Encode( textureName, TextureFormat.DDS, bitmap );
            }

            return TextureInfo.GetTextureInfo( texture );
        }

        private static bool HasAlpha( TexturePixelFormat pixelFormat )
        {
            return pixelFormat == TexturePixelFormat.BC2 || pixelFormat == TexturePixelFormat.BC3;
        }
    }
}
