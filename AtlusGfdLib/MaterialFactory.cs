using System.Collections.Generic;
using System.Numerics;
using Ai = Assimp;

namespace AtlusGfdLib
{
    public static class MaterialFactory
    {
        public static Material CreateFieldTerrainMaterial( string name, string diffuseMapName, bool hasTransparency = false )
        {
            var material =  new Material( name )
            {
                Ambient = new Vector4( 0.549019635f, 0.549019635f, 0.549019635f, 1f ),
                Diffuse = new Vector4( 0.0980392247f, 0.0980392247f, 0.0980392247f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                Emissive = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0.1f,
                DrawOrder = MaterialDrawOrder.Front,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field4D = 2,
                Field50 = 0,
                Field5C = 0,
                Field6C = 0xfffffff8,
                Field70 = 0xfffffff8,
                Field90 = 0,
                Field92 = 4,
                Field94 = 1,
                Field96 = 0,
                Field98 = 0xffffffff,
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight2 | MaterialFlags.ReceiveShadow | MaterialFlags.HasDiffuseMap,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                Attributes = null,
                ReflectionMap = null,
                ShadowMap = null,
                Specular = new Vector4( 0, 0, 0, 0 ),
                SpecularMap = null,
            };

            if ( hasTransparency )
            {
                material.DrawOrder = MaterialDrawOrder.Back;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            return material;
        }

        public static Material CreateFieldTerrainCastShadowMaterial( string name, string diffuseMapName, bool hasTransparency = false )
        {
            var material = CreateFieldTerrainMaterial( name, diffuseMapName, hasTransparency );
            material.Flags |= MaterialFlags.CastShadow;

            return material;
        }

        public static Material CreateCharacterSkinP5Material( string name, string diffuseMapName, string shadowMapName, bool hasTransparency = false )
        {
            // based off c0001_body_1_skin_kubi_low
            var material =  new Material( name )
            {
                Ambient = new Vector4( 0.7254902f, 0.6f, 0.7843138f, 1f ),
                Diffuse = new Vector4( 0.0784313753f, 0.0784313753f, 0.0784313753f, 1 ),
                Specular = new Vector4( 0.117647067f, 0.117647067f, 0.117647067f, 1 ),
                Emissive = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0,
                DrawOrder = MaterialDrawOrder.Front,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field4D = 1,
                Field50 = 0,
                Field5C = 2,
                Field6C = 0xf8fffff8,
                Field70 = 0xf8fffff8,
                Field90 = 0,
                Field92 = 4,
                Field94 = 4,
                Field96 = 0,
                DiffuseMap = new TextureMap( diffuseMapName ),
                ShadowMap = new TextureMap( shadowMapName ),
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag100 | MaterialFlags.EnableLight2 | 
                        MaterialFlags.ReceiveShadow | MaterialFlags.CastShadow | MaterialFlags.HasAttributes | MaterialFlags.Flag20000Crash | MaterialFlags.HasDiffuseMap | MaterialFlags.HasShadowMap,
                Attributes = new List< MaterialAttribute >
                {
                    new MaterialAttributeType0
                    {
                        Field0C = new Vector4( 0.980392337f, 0.980392337f, 0.980392337f, 0.392156869f ),
                        Field1C = 0.6f,
                        Field20 = 14,
                        Field24 = 0.6f,
                        Field28 = 0.4f,
                        Field2C = 1.5f,
                        Flags = MaterialAttributeFlags.Flag1,
                        Type0Flags = ( MaterialAttributeType0Flags ) 0x00000045
                    },
                    new MaterialAttributeType2
                    {
                        Field0C = 0,
                        Field10 = 100,
                        Flags = MaterialAttributeFlags.Flag1
                    }
                }
            };

            if ( hasTransparency )
            {
                material.DrawOrder = MaterialDrawOrder.Back;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            return material;
        }

        public static Material CreateCharacterClothP4DMaterial( string name, string diffuseMapName, bool hasTransparency = false )
        {
            var material = new Material( name )
            {
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag10Crash | MaterialFlags.Flag20 | MaterialFlags.Flag100 |
                        MaterialFlags.EnableLight2 | MaterialFlags.CastShadow,
                Ambient = new Vector4( 0.6f, 0.6f, 0.6f, 0 ),
                Attributes = new List< MaterialAttribute >
                {
                    new MaterialAttributeType0
                    {
                        Field0C = new Vector4( 0.9799954f, 0.9799954f, 0.9799954f, 0.5882353f ),
                        Field1C = 0.8f,
                        Field20 = 30,
                        Field24 = 1,
                        Field28 = 0,
                        Field2C = 0,
                        Flags = MaterialAttributeFlags.Flag1,
                        Type = MaterialAttributeType.Type0,
                        Type0Flags = 0,
                    }
                },
                Diffuse = new Vector4( 0.3000076f, 0.3000076f, 0.3000076f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                DrawOrder = MaterialDrawOrder.Front,
                Emissive = new Vector4( 0f, 0f, 0f, 0f ),
                Field40 = 1,
                Field44 = 0,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field4D = 1,
                Field50 = 0,
                Field5C = 2,
                Field6C = 0xfffffff8,
                Field70 = 0xfffffff8,
                Field90 = 0,
                Field92 = 4,
                Field94 = -32768,
                Field96 = 0,
                Field98 = 0xFFFFFFFF,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                NormalMap = null,
                ReflectionMap = null,
                ShadowMap = null,
                Specular = new Vector4( 0f, 0f, 0f, 1f ),
                SpecularMap = null
            };

            if ( hasTransparency )
            {
                material.DrawOrder = MaterialDrawOrder.Back;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            return material;
        }
    }

    public enum MaterialPreset
    {
        None,
        FieldTerrain,
        FieldTerrainCastShadow,
        CharacterSkinP5,
        CharacterClothP4D,
    }
}
