using System.Collections.Generic;
using System.Numerics;
using GFDLibrary.Models;
using GFDLibrary.Models.Conversion;

namespace GFDLibrary.Materials
{
    public static class MaterialFactory
    {
        public static Material CreateMaterial( string name, string diffuseMapName, string lightmapName, string displacementMapName, string opacityMapName, string normalMapName, 
            string heightMapName, string emissiveMapName, string ambientMapName, string specularMapName, string reflectionMapName, ModelPackConverterOptions options )
        {
            var MaterialPreset = (Material)options.MaterialPreset;
            var material = new Material( name )
            {
                Version = MaterialPreset.Version,

                DrawMethod = MaterialPreset.DrawMethod,
                Field49 = MaterialPreset.Field49,
                Field4A = MaterialPreset.Field4A,
                Field4B = MaterialPreset.Field4B,
                Field4C = MaterialPreset.Field4C,
                Field4D = MaterialPreset.Field4D,
                DisableBackfaceCulling = MaterialPreset.DisableBackfaceCulling,
                Field5C = MaterialPreset.Field5C,
                Field6C = MaterialPreset.Field6C,
                Field70 = MaterialPreset.Field70,
                Field90 = MaterialPreset.Field90,
                Field92 = MaterialPreset.Field92,
                Flags2 = MaterialPreset.Flags2,
                Field96 = MaterialPreset.Field96,
                Field98 = MaterialPreset.Field98,
                DiffuseMap = null,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                NormalMap = null,
                ReflectionMap = null,
                ShadowMap = null,
                SpecularMap = null,
                Flags = MaterialPreset.Flags,
                Attributes = MaterialPreset.Attributes,
            };
            if ( MaterialPreset.METAPHOR_MaterialParameterSet != null )
            {
                material.METAPHOR_UseMaterialParameterSet = MaterialPreset.METAPHOR_UseMaterialParameterSet;
                material.METAPHOR_MaterialParameterSet = MaterialPreset.METAPHOR_MaterialParameterSet;
            }
            else
            {
                material.LegacyParameters = MaterialPreset.LegacyParameters;
            }

            TextureMap NewTextureMapResource(string name)
            {
                TextureMap newMap = new( name );
                if (material.METAPHOR_UseMaterialParameterSet)
                    newMap.METAPHOR_ParentMaterialParameterSet = material.METAPHOR_MaterialParameterSet;
                return newMap;
            }

            // TODO: which one is which
            if ( MaterialPreset.DiffuseMap != null ) material.DiffuseMap = NewTextureMapResource( diffuseMapName );
            // if ( MaterialPreset.GlowMap != null ) material.GlowMap = new TextureMap( diffuseMapName );
            // if ( MaterialPreset.HighlightMap != null ) material.HighlightMap = new TextureMap( diffuseMapName );
            // if ( MaterialPreset.NightMap != null ) material.NightMap = new TextureMap( diffuseMapName );
            //if ( MaterialPreset.NormalMap != null ) material.NormalMap = NewTextureMapResource( normalMapName );
            //if ( MaterialPreset.ReflectionMap != null ) material.ReflectionMap = NewTextureMapResource( reflectionMapName );
            // if ( MaterialPreset.ShadowMap != null ) material.ShadowMap = new TextureMap( diffuseMapName );
            if (options.Version >= 0x2000000)
            {
                // force adding toon shadow map to character models so that they don't crash by default
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType2_3_13)
                    material.SpecularMap = NewTextureMapResource( diffuseMapName );
                // distortion multiply map
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType4 )
                    material.HighlightMap = NewTextureMapResource( diffuseMapName );
            } else
            {
                if ( MaterialPreset.NormalMap != null ) material.NormalMap = NewTextureMapResource( normalMapName );
                if ( MaterialPreset.ReflectionMap != null ) material.ReflectionMap = NewTextureMapResource( reflectionMapName );
                if ( MaterialPreset.SpecularMap != null ) material.SpecularMap = NewTextureMapResource( specularMapName );
            }

            material.IsPresetMaterial = false;

            return material;
        }

        public static Material CreateMaterial( string name, string diffuseMapName, ModelPackConverterOptions options )
        {
            return CreateMaterial(name, diffuseMapName, null, null, null, null, null, null, null, null, null, options);
        }
    }
}
