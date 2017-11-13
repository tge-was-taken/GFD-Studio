using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using AtlusGfdLib.Assimp;
using Ai = Assimp;

namespace AtlusGfdLib
{
    public static class ModelFactory
    {
        public static Model CreateFromAssimpScene( string filePath, uint version, Material baseMaterial = null )
        {
            // Import scene
            var aiScene = AssimpImporter.ImportFile( filePath );

            // Build textures & Materials
            var model = new Model( version );
            model.TextureDictionary = new TextureDictionary( version );
            model.MaterialDictionary = new MaterialDictionary( version );

            foreach ( var aiSceneMaterial in aiScene.Materials )
            {
                var material = baseMaterial != null ? baseMaterial.ShallowCopy() : new Material();
                material.Name = aiSceneMaterial.Name;

                if ( aiSceneMaterial.HasTextureDiffuse )
                {
                    var relativeFilePath = aiSceneMaterial.TextureDiffuse.FilePath;
                    var fullFilePath = Path.GetFullPath( Path.Combine( Path.GetDirectoryName( filePath ), relativeFilePath ) );
                    Texture texture;

                    if ( relativeFilePath.EndsWith( ".dds" ) )
                    {
                        var textureName = Path.GetFileName( relativeFilePath );
                        texture = new Texture( textureName, TextureFormat.DDS, File.ReadAllBytes( fullFilePath ) );
                    }
                    else
                    {
                        var bitmap = new Bitmap( fullFilePath );
                        var textureName = Path.GetFileNameWithoutExtension( relativeFilePath ) + ".dds";
                        texture = TextureEncoder.Encode( textureName, TextureFormat.DDS, bitmap );
                    }

                    model.TextureDictionary.Add( texture );


                    material.DiffuseMap = new TextureMap( texture.Name );

                    // todo: map this to one of the entries in the assimp material
                    if ( material.ShadowMap != null )
                        material.ShadowMap = new TextureMap( texture.Name );
                }

                model.MaterialDictionary.Add( material );
            }

            // Create scene
            model.Scene = SceneFactory.CreateFromAssimpScene( aiScene, version );

            return model;
        }
    }
}
