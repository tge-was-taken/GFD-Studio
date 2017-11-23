using System;
using System.Drawing;
using System.IO;
using AtlusGfdLib.Assimp;
using Ai = Assimp;

namespace AtlusGfdLib
{
    public static class ModelConverter
    {
        public static Model CreateFromAssimpScene( string filePath, MaterialPreset preset, SceneConverterOptions options )
        {
            // For importing textures
            string baseDirectoryPath = Path.GetDirectoryName( filePath );

            // Import scene
            var aiScene = AssimpImporter.ImportFile( filePath );

            // Build textures & Materials
            var model = new Model( options.Version );
            model.TextureDictionary = new TextureDictionary( options.Version );
            model.MaterialDictionary = new MaterialDictionary( options.Version );

            foreach ( var aiSceneMaterial in aiScene.Materials )
            {
                var material = ConvertMaterialAndTextures( aiSceneMaterial, preset, baseDirectoryPath, model.TextureDictionary );
                model.MaterialDictionary.Add( material );
            }

            // Create scene
            model.Scene = SceneConverter.ConvertFromAssimpScene( aiScene, options );

            return model;
        }

        private static Material ConvertMaterialAndTextures( Ai.Material aiMaterial, MaterialPreset preset, string baseDirectoryPath, TextureDictionary textureDictionary )
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
            Material material = null;

            switch ( preset )
            {
                case MaterialPreset.FieldDiffuse:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateFieldDiffuseMaterial( aiMaterial.Name, diffuseTexture.Name, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
                case MaterialPreset.FieldDiffuseCastShadow:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateFieldDiffuseCastShadowMaterial( aiMaterial.Name, diffuseTexture.Name, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
                case MaterialPreset.CharacterSkin:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            string shadowTextureName = diffuseTexture.Name;

                            if ( ambientTexture != null )
                            {
                                textureDictionary.Add( ambientTexture.Texture );
                                shadowTextureName = ambientTexture.Name;
                            }

                            // TODO: transparency
                            material = MaterialFactory.CreateCharacterSkinMaterial( aiMaterial.Name, diffuseTexture.Name, shadowTextureName, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;

                case MaterialPreset.CharacterSkinP4D:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateCharacterSkinP4DMaterial( aiMaterial.Name, diffuseTexture.Name,
                                                                                       HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
            }

            // Create dummy material if none was created
            if ( material == null )
                material = new Material( aiMaterial.Name );

            return material;
        }

        private static TextureInfo ConvertTexture( Ai.TextureSlot aiTextureSlot, string baseDirectoryPath )
        {
            var relativeFilePath = aiTextureSlot.FilePath;
            var fullFilePath = Path.GetFullPath( Path.Combine( baseDirectoryPath, relativeFilePath ) );
            var textureName = Path.GetFileNameWithoutExtension( relativeFilePath ) + ".dds";;

            Texture texture;
            if ( !File.Exists( fullFilePath ) )
            {
                texture = new Texture( textureName, TextureFormat.DDS, Texture.DummyTextureData );
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

            return TextureUtillities.GetTextureInfo( texture );
        }

        private static bool HasAlpha( TexturePixelFormat pixelFormat )
        {
            return pixelFormat == TexturePixelFormat.DXT3 || pixelFormat == TexturePixelFormat.DXT5;
        }
    }
}
