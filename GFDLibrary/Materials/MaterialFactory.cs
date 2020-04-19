using System.Collections.Generic;
using System.Numerics;
using GFDLibrary.Models;

namespace GFDLibrary.Materials
{
    public static class MaterialFactory
    {
        public static Material CreateFieldTerrainMaterial( string name, string diffuseMapName, bool hasTransparency = false )
        {
            var material =  new Material( name )
            {
                AmbientColor = new Vector4( 0.549019635f, 0.549019635f, 0.549019635f, 1f ),
                DiffuseColor = new Vector4( 0.0980392247f, 0.0980392247f, 0.0980392247f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                EmissiveColor = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0.1f,
                DrawMethod = MaterialDrawMethod.Opaque,
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
                SpecularColor = new Vector4( 0, 0, 0, 0 ),
                SpecularMap = null,
            };

            if ( hasTransparency )
            {
                material.DrawMethod = MaterialDrawMethod.Translucent;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            material.IsPresetMaterial = true;

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
                AmbientColor = new Vector4( 0.7254902f, 0.6f, 0.7843138f, 1f ),
                DiffuseColor = new Vector4( 0.0784313753f, 0.0784313753f, 0.0784313753f, 1 ),
                SpecularColor = new Vector4( 0.117647067f, 0.117647067f, 0.117647067f, 1 ),
                EmissiveColor = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0,
                DrawMethod = MaterialDrawMethod.Opaque,
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
                        Color = new Vector4( 0.980392337f, 0.980392337f, 0.980392337f, 0.392156869f ),
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
                material.DrawMethod = MaterialDrawMethod.Translucent;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            material.IsPresetMaterial = true;

            return material;
        }

        public static Material CreatePersonaSkinP5Material(string name, string diffuseMapName, string specularMapName, string shadowMapName )
        {
            // based off ps0201, t01-5 material
            var material = new Material(name)
            {
                AmbientColor = new Vector4(0.7058824f, 0.7058824f, 0.7058824f, 1f),
                DiffuseColor = new Vector4(0.3137255f, 0.3137255f, 0.3137255f, 1),
                SpecularColor = new Vector4(0, 0, 0, 0),
                EmissiveColor = new Vector4(0.9f, 0.9f, 0.9f, 10),
                Field40 = 1,
                Field44 = 0.1f,
                DrawMethod = MaterialDrawMethod.Opaque,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field4D = 1,
                Field50 = 0,
                Field5C = 3,
                Field6C = 0xF8FFFE38,
                Field70 = 0xF8FFFE38,
                Field90 = 0,
                Field92 = 4,
                Field94 = 2,
                Field96 = 0,
                DiffuseMap = new TextureMap(diffuseMapName),
                SpecularMap = new TextureMap(specularMapName),
                ShadowMap = new TextureMap(shadowMapName),
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag4 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight2 |
                        MaterialFlags.CastShadow | MaterialFlags.HasAttributes | MaterialFlags.Flag20000Crash | MaterialFlags.HasDiffuseMap | MaterialFlags.HasSpecularMap | MaterialFlags.HasShadowMap,
                Attributes = new List<MaterialAttribute>
                {
                    new MaterialAttributeType1
                    {
                        Field0C = new Vector4(0.2352941f, 0.5960785f, 1, 1),
                        Field1C = 0.85f,
                        Field20 = 2,
                        Field24 = new Vector4(0, 0, 0, 0),
                        Field34 = 0,
                        Field38 = 0,
                        Flags = MaterialAttributeFlags.Flag1,
                        Type1Flags = MaterialAttributeType1Flags.Flag2
                    },
                    new MaterialAttributeType2
                    {
                        Field0C = 1,
                        Field10 = 121,
                        Flags = MaterialAttributeFlags.Flag1
                    }
                }
            };

            material.IsPresetMaterial = true;

            return material;
        }

        public static Material CreateCharacterClothP4DMaterial( string name, string diffuseMapName, bool hasTransparency = false )
        {
            var material = new Material( name )
            {
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag10Crash | MaterialFlags.Flag20 | MaterialFlags.Flag100 |
                        MaterialFlags.EnableLight2 | MaterialFlags.CastShadow,
                AmbientColor = new Vector4( 0.6f, 0.6f, 0.6f, 0 ),
                Attributes = new List< MaterialAttribute >
                {
                    new MaterialAttributeType0
                    {
                        Color = new Vector4( 0.9799954f, 0.9799954f, 0.9799954f, 0.5882353f ),
                        Field1C = 0.8f,
                        Field20 = 30,
                        Field24 = 1,
                        Field28 = 0,
                        Field2C = 0,
                        Flags = MaterialAttributeFlags.Flag1,
                        AttributeType = MaterialAttributeType.Type0,
                        Type0Flags = 0,
                    }
                },
                DiffuseColor = new Vector4( 0.3000076f, 0.3000076f, 0.3000076f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                DrawMethod = MaterialDrawMethod.Opaque,
                EmissiveColor = new Vector4( 0f, 0f, 0f, 0f ),
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
                SpecularColor = new Vector4( 0f, 0f, 0f, 1f ),
                SpecularMap = null
            };

            if ( hasTransparency )
            {
                material.DrawMethod = MaterialDrawMethod.Translucent;
                material.Field4D = 1;
                material.Field90 = 0x0080;
            }

            material.IsPresetMaterial = true;

            return material;
        }

        public static Material CreateCharacterSkinFBMaterial( string name, string diffuseMapName, string shadowMapName, bool hasTransparency = false )
        {
            var material = new Material( name )
            {
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag100 |
                        MaterialFlags.EnableLight2 | MaterialFlags.CastShadow,
                AmbientColor = new Vector4( 0.4901961f, 0.4901961f, 0.4901961f, 1f ),
                Attributes = new List<MaterialAttribute>
                {
                    new MaterialAttributeType8
                    {
                        Field00 = new Vector3(0.9803922f, 0.9803922f, 0.9803922f),
                        Field0C = 0.470588237f,
                        Field10 = 0.65f,
                        Field14 = 30f,
                        Field18 = 100f,
                        Field1C = new Vector3(0, 0, 0),
                        Field28 = 0.35f,
                        Field2C = 0f,
                        Field30 = 0,
                        Field34 = 0,
                        Field38 = 0,
                        Flags = MaterialAttributeFlags.Flag1,
                        Type8Flags = MaterialAttributeType8Flags.Bit0 | MaterialAttributeType8Flags.Bit8,
                        Version = 0x01105090
                    }
                },
                DiffuseColor = new Vector4( 0f, 0f, 0f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                DrawMethod = MaterialDrawMethod.Opaque,
                EmissiveColor = new Vector4( 0f, 0f, 0f, 0f ),
                Field40 = 1,
                Field44 = 0,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field4D = 1,
                Field50 = 0,
                Field5C = 0x68,
                Field6C = 0xf8fffff8,
                Field70 = 0xf8fffff8,
                Field90 = 0,
                Field92 = 4,
                Field94 = 5,
                Field96 = 0,
                Field98 = 0xFFFFFFFF,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                NormalMap = null,
                ReflectionMap = null,
                ShadowMap = new TextureMap( shadowMapName ),
                SpecularColor = new Vector4( 0.4901961f, 0.4901961f, 0.4901961f, 1f ),
                SpecularMap = null,
                Version = 0x01105100,
            };

            //if ( hasTransparency )
            //{
            //    material.DrawMethod = MaterialDrawMethod.Translucent;
            //    material.Field4D = 1;
            //    material.Field90 = 0x0080;
            //}

            material.IsPresetMaterial = true;

            return material;
        }
    }

    public enum MaterialPreset
    {
        None,
        FieldTerrain,
        FieldTerrainCastShadow,
        CharacterSkinP5,
        PersonaSkinP5,
        CharacterClothP4D,
        CharacterSkinFB
    }
}
