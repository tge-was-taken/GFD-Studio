using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public static class MaterialFactory
    {
        public static Material CreatePresetMaterial( MaterialPreset preset )
        {
            switch ( preset )
            {
                case MaterialPreset.FieldDiffuse:
                    return CreateFieldDiffuseMaterial( null, null );
                case MaterialPreset.CharacterSkin:
                    return CreateCharacterSkinMaterial( null, null, null );
                default:
                    throw new ArgumentOutOfRangeException( nameof( preset ), preset, null );
            }
        }

        public static Material CreateFieldDiffuseMaterial( string name, string diffuseMapName )
        {
            // based off ground_dummy2
            return new Material( name )
            {
                Ambient = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f ),
                Diffuse = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f ),
                DiffuseMap = new TextureMap( diffuseMapName ),
                Emissive = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0.1f,
                Field48 = 0,
                Field49 = 1,
                Field4A = 0,
                Field4B = 1,
                Field4C = 0,
                Field50 = 0,
                Field5C = 0,
                Field6C = 0xfffffff8,
                Field70 = 0xfffffff8,
                Field90 = 0,
                Field92 = 4,
                Field94 = 1,
                Field96 = 0,
                Field98 = 0xffffffff,
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight | MaterialFlags.EnableLight2 | MaterialFlags.ReceiveShadow | MaterialFlags.HasDiffuseMap,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                Attributes = null,
                ReflectionMap = null,
                ShadowMap = null,
                Specular = new Vector4( 0, 0, 0, 0 ),
                SpecularMap = null,
            };
        }

        public static Material CreateCharacterSkinMaterial( string name, string diffuseMapName, string shadowMapName )
        {
            // based off c0001_body_1_skin_kubi_low
            return new Material( name )
            {
                Ambient = new Vector4( 0.7254902f, 0.6f, 0.7843138f, 1f ),
                Diffuse = new Vector4( 0.0784313753f, 0.0784313753f, 0.0784313753f, 1 ),
                Specular = new Vector4( 0.117647067f, 0.117647067f, 0.117647067f, 1 ),
                Emissive = new Vector4( 0, 0, 0, 0 ),
                Field40 = 1,
                Field44 = 0,
                Field48 = 0,
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
                Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag100 | MaterialFlags.EnableLight2 | MaterialFlags.ReceiveShadow | MaterialFlags.CastShadow | MaterialFlags.HasAttributes | MaterialFlags.Flag20000Crash | MaterialFlags.HasDiffuseMap | MaterialFlags.HasShadowMap,
                Attributes = new List< MaterialAttribute >()
                {
                    new MaterialAttributeType0()
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
                    new MaterialAttributeType2()
                    {
                        Field0C = 0,
                        Field10 = 100,
                        Flags = MaterialAttributeFlags.Flag1
                    }
                }
            };
        }
    }

    public enum MaterialPreset
    {
        None,
        FieldDiffuse,
        CharacterSkin,
    }
}
