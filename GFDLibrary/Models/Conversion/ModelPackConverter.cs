using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GFDLibrary.Materials;
using GFDLibrary.Textures;
using Ai = Assimp;

namespace GFDLibrary.Models.Conversion
{
    public static class ModelPackConverter
    {
        public static ModelPack ConvertFromAssimpScene( string filePath, ModelPackConverterOptions options )
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
            var sceneConverterOptions = new ModelConverterOptions()
            {
                Version = options.Version,
                ConvertSkinToZUp = options.ConvertSkinToZUp,
                GenerateVertexColors = options.GenerateVertexColors,
                MinimalVertexAttributes = options.MinimalVertexAttributes,
            };

            model.Model = ModelConverter.ConvertFromAssimpScene( aiScene, sceneConverterOptions );

            return model;
        }

        private static Material ConvertMaterialAndTextures( Ai.Material aiMaterial, ModelPackConverterOptions options, string baseDirectoryPath, TextureDictionary textureDictionary )
        {
            // Convert all textures
            TextureInfo diffuseTexture = null;
            if ( aiMaterial.HasTextureDiffuse )
                diffuseTexture = ConvertTexture( aiMaterial.TextureDiffuse, baseDirectoryPath );


            TextureInfo lightmapTexture = null;
            if ( aiMaterial.HasTextureLightMap )
            {
                lightmapTexture = ConvertTexture( aiMaterial.TextureLightMap, baseDirectoryPath );
                textureDictionary.Add( lightmapTexture.Texture );
            }

            TextureInfo displacementTexture = null;
            if ( aiMaterial.HasTextureDisplacement )
            {
                displacementTexture = ConvertTexture( aiMaterial.TextureDisplacement, baseDirectoryPath );
                textureDictionary.Add( displacementTexture.Texture );
            }

            TextureInfo opacityTexture = null;
            if ( aiMaterial.HasTextureOpacity )
            {
                opacityTexture = ConvertTexture( aiMaterial.TextureOpacity, baseDirectoryPath );
                textureDictionary.Add( opacityTexture.Texture );
            }

            TextureInfo normalTexture = null;
            if ( aiMaterial.HasTextureNormal )
            {
                normalTexture = ConvertTexture( aiMaterial.TextureNormal, baseDirectoryPath );
                textureDictionary.Add( normalTexture.Texture );
            }

            TextureInfo heightTexture = null;
            if ( aiMaterial.HasTextureHeight )
            {
                heightTexture = ConvertTexture( aiMaterial.TextureHeight, baseDirectoryPath );
                textureDictionary.Add( heightTexture.Texture );
            }


            TextureInfo emissiveTexture = null;
            if ( aiMaterial.HasTextureEmissive )
            {
                emissiveTexture = ConvertTexture( aiMaterial.TextureEmissive, baseDirectoryPath );
                textureDictionary.Add( emissiveTexture.Texture );
            }


            TextureInfo ambientTexture = null;
            if ( aiMaterial.HasTextureAmbient )
            {
                ambientTexture = ConvertTexture( aiMaterial.TextureAmbient, baseDirectoryPath );
                textureDictionary.Add(ambientTexture.Texture);
            }
                

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
                material = MaterialFactory.CreateMaterial( materialName, diffuseTexture.Name, CheckIfTextureNull( lightmapTexture ), CheckIfTextureNull( displacementTexture ),
                    CheckIfTextureNull( opacityTexture ), CheckIfTextureNull( normalTexture ), CheckIfTextureNull( heightTexture ), CheckIfTextureNull( emissiveTexture ),
                    CheckIfTextureNull( ambientTexture ), CheckIfTextureNull( specularTexture ), CheckIfTextureNull( reflectionTexture ), options ); 
            }

            return material;
        }

        private static string CheckIfTextureNull( TextureInfo p )
        {
            if (p == null) return null;
            else
            {
                return p.Name;
            }
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

    public class ModelPackConverterOptions
    {
        /// <summary>
        /// Gets or sets the material preset that should be used while converting the materials.
        /// </summary>
        public object MaterialPreset { get; set; }

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

        /// <summary>
        /// Sometimes extra vertex attributes are generated for one reason or another, on games using the GFD formats after P5 this causes problems if the data for the attributes does not exist, this sets the minimal amount of attributes to circumvent this issue.
        /// </summary>
        public bool MinimalVertexAttributes { get; set; }

        public ModelPackConverterOptions()
        {
            // MaterialPreset = MaterialPreset.CharacterSkinP5;
            Version = ResourceVersion.Persona5;
            ConvertSkinToZUp = false;
            GenerateVertexColors = false;
            MinimalVertexAttributes = true;
        }
    }
}
