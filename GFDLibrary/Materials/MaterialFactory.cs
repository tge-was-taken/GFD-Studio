using GFDLibrary.Conversion;
using GFDLibrary.Models;

namespace GFDLibrary.Materials
{
    public static class MaterialFactory
    {
        public static Material CreateMaterial( string name, string diffuseMapName, string lightmapName, string displacementMapName, string opacityMapName, string normalMapName, 
            string heightMapName, string emissiveMapName, string ambientMapName, string specularMapName, string reflectionMapName, ModelConverterOptions options )
        {
            var MaterialPreset = (Material)options.MaterialPreset;
            var material = new Material( name )
            {
                Version = MaterialPreset.Version,

                DrawMethod = MaterialPreset.DrawMethod,
                BlendSourceColor = MaterialPreset.BlendSourceColor,
                BlendDestinationColor = MaterialPreset.BlendDestinationColor,
                SourceAlpha = MaterialPreset.SourceAlpha,
                DestinationAlpha = MaterialPreset.DestinationAlpha,
                HighlightMapBlendMode = MaterialPreset.HighlightMapBlendMode,
                DisableBackfaceCulling = MaterialPreset.DisableBackfaceCulling,
                ShaderId = MaterialPreset.ShaderId,
                TexCoordFlags0 = MaterialPreset.TexCoordFlags0,
                TexCoordFlags1 = MaterialPreset.TexCoordFlags1,
                AlphaClip = MaterialPreset.AlphaClip,
                AlphaClipMode = MaterialPreset.AlphaClipMode,
                Flags2 = MaterialPreset.Flags2,
                SortPriority = MaterialPreset.SortPriority,
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

            material.RuntimeMetadata.IsCustomMaterial = false;

            return material;
        }

        public static Material CreateMaterial( string name, string diffuseMapName, ModelConverterOptions options )
        {
            return CreateMaterial(name, diffuseMapName, null, null, null, null, null, null, null, null, null, options);
        }
    }
}
