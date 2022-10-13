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
                AmbientColor = MaterialPreset.AmbientColor,
                DiffuseColor = MaterialPreset.DiffuseColor,
                SpecularColor = MaterialPreset.SpecularColor,
                EmissiveColor = MaterialPreset.EmissiveColor,
                Field40 = MaterialPreset.Field40,
                Field44 = MaterialPreset.Field44,
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
                Field94 = MaterialPreset.Field94,
                Field96 = MaterialPreset.Field96,
                Field98 = MaterialPreset.Field98,
                DiffuseMap = MaterialPreset.DiffuseMap,
                GlowMap = MaterialPreset.GlowMap,
                HighlightMap = MaterialPreset.HighlightMap,
                NightMap = MaterialPreset.NightMap,
                NormalMap = MaterialPreset.NormalMap,
                ReflectionMap = MaterialPreset.ReflectionMap,
                ShadowMap = MaterialPreset.ShadowMap,
                SpecularMap = MaterialPreset.SpecularMap,
                Flags = MaterialPreset.Flags,
                Attributes = MaterialPreset.Attributes
            };

            // TODO: which one is which
            if ( material.DiffuseMap != null ) material.DiffuseMap.Name = diffuseMapName;
            // if ( material.GlowMap != null ) material.GlowMap.Name = diffuseMapName;
            // if ( material.HighlightMap != null ) material.HighlightMap.Name = diffuseMapName;
            // if ( material.NightMap != null ) material.NightMap.Name = diffuseMapName;
            if ( material.NormalMap != null ) material.NormalMap.Name = normalMapName;
            if ( material.ReflectionMap != null ) material.ReflectionMap.Name = reflectionMapName;
            // if ( material.ShadowMap != null ) material.ShadowMap.Name = diffuseMapName;
            if ( material.SpecularMap != null ) material.SpecularMap.Name = specularMapName;

            material.IsPresetMaterial = false;

            return material;
        }

        public static Material CreateMaterial( string name, string diffuseMapName, ModelPackConverterOptions options )
        {
            return CreateMaterial(name, diffuseMapName, null, null, null, null, null, null, null, null, null, options);
        }
    }
}
