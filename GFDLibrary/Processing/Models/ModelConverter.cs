using System;
using System.Drawing;
using System.IO;
using GFDLibrary.Assimp;
using Ai = Assimp;

namespace GFDLibrary
{
    public static class ModelConverter
    {
        public static Model ConvertFromAssimpScene( string filePath, ModelConverterOptions options )
        {
            // For importing textures
            string baseDirectoryPath = Path.GetDirectoryName( filePath );

            // Import scene
            var aiScene = AssimpSceneImporter.ImportFile( filePath );

            // Build textures & Materials
            var model = new Model( options.Version );
            model.TextureDictionary = new TextureDictionary( options.Version );
            model.MaterialDictionary = new MaterialDictionary( options.Version );

            foreach ( var aiSceneMaterial in aiScene.Materials )
            {
                var material = ConvertMaterialAndTextures( aiSceneMaterial, options, baseDirectoryPath, model.TextureDictionary );
                model.MaterialDictionary.Add( material );
            }

            // Create scene
            var sceneConverterOptions = new SceneConverterOptions()
            {
                Version = options.Version,
                ConvertSkinToZUp = options.ConvertSkinToZUp,
                GenerateVertexColors = options.GenerateVertexColors,
            };

            model.Scene = SceneConverter.ConvertFromAssimpScene( aiScene, sceneConverterOptions );

            return model;
        }

        private static string UnescapeName( string name )
        {
            return name.Replace( "___", " " );
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
            Material material = null;
            string materialName = UnescapeName( aiMaterial.Name );

            switch ( options.MaterialPreset )
            {
                case MaterialPreset.FieldTerrain:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateFieldTerrainMaterial( materialName, diffuseTexture.Name, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
                case MaterialPreset.FieldTerrainCastShadow:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateFieldTerrainCastShadowMaterial( materialName, diffuseTexture.Name, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
                case MaterialPreset.CharacterSkinP5:
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
                            material = MaterialFactory.CreateCharacterSkinP5Material( materialName, diffuseTexture.Name, shadowTextureName, HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;

                case MaterialPreset.CharacterClothP4D:
                    {
                        if ( diffuseTexture != null )
                        {
                            textureDictionary.Add( diffuseTexture.Texture );
                            material = MaterialFactory.CreateCharacterClothP4DMaterial( materialName, diffuseTexture.Name,
                                                                                       HasAlpha( diffuseTexture.PixelFormat ) );
                        }
                    }
                    break;
            }

            // Create dummy material if none was created
            if ( material == null )
                material = new Material( materialName );

            return material;
        }

        private static TextureInfo ConvertTexture( Ai.TextureSlot aiTextureSlot, string baseDirectoryPath )
        {
            var relativeFilePath = aiTextureSlot.FilePath;
            var fullFilePath = Path.GetFullPath( Path.Combine( baseDirectoryPath, relativeFilePath ) );
            var textureName = UnescapeName( Path.GetFileNameWithoutExtension( relativeFilePath ) + ".dds" );

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
            return pixelFormat == TexturePixelFormat.DXT3 || pixelFormat == TexturePixelFormat.DXT5;
        }
    }

    public class ModelConverterOptions
    {
        /// <summary>
        /// Gets or sets the material preset that should be used while converting the materials.
        /// </summary>
        public MaterialPreset MaterialPreset { get; set; }

        /// <summary>
        /// Gets or sets the version to use for the converted resources.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets whether to convert the up axis of the inverse bind pose matrices to Z-up. This is used by Persona 5's battle models for example.
        /// </summary>
        public bool ConvertSkinToZUp { get; set; }

        /// <summary>
        /// Gets or sets whether to generate dummy (white) vertex colors if they're not already present. Some material shaders rely on vertex colors being present, and the lack of them will cause graphics corruption.
        /// </summary>
        public bool GenerateVertexColors { get; set; }

        public ModelConverterOptions()
        {
            MaterialPreset = MaterialPreset.CharacterSkinP5;
            Version = Resource.PERSONA5_RESOURCE_VERSION;
            ConvertSkinToZUp = false;
            GenerateVertexColors = false;
        }
    }
}
