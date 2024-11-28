using GFDLibrary.Conversion;
using GFDLibrary.Models;

namespace GFDLibrary.Materials
{
    public static class MaterialFactory
    {
        public static Material CreateMaterial( string name, string diffuseMapName, string lightmapName, string displacementMapName, string opacityMapName, string normalMapName, 
            string heightMapName, string emissiveMapName, string ambientMapName, string specularMapName, string reflectionMapName, Material materialPreset )
        {
            var material = new Material( name )
            {
                Version = materialPreset.Version,

                DrawMethod = materialPreset.DrawMethod,
                BlendSourceColor = materialPreset.BlendSourceColor,
                BlendDestinationColor = materialPreset.BlendDestinationColor,
                SourceAlpha = materialPreset.SourceAlpha,
                DestinationAlpha = materialPreset.DestinationAlpha,
                HighlightMapBlendMode = materialPreset.HighlightMapBlendMode,
                DisableBackfaceCulling = materialPreset.DisableBackfaceCulling,
                ShaderId = materialPreset.ShaderId,
                TexCoordFlags0 = materialPreset.TexCoordFlags0,
                TexCoordFlags1 = materialPreset.TexCoordFlags1,
                AlphaClip = materialPreset.AlphaClip,
                AlphaClipMode = materialPreset.AlphaClipMode,
                Flags2 = materialPreset.Flags2,
                SortPriority = materialPreset.SortPriority,
                Field98 = materialPreset.Field98,
                DiffuseMap = null,
                GlowMap = null,
                HighlightMap = null,
                NightMap = null,
                NormalMap = null,
                ReflectionMap = null,
                ShadowMap = null,
                SpecularMap = null,
                Flags = materialPreset.Flags,
                Attributes = materialPreset.Attributes,
                RuntimeMetadata = materialPreset.RuntimeMetadata,
            };
            if ( materialPreset.METAPHOR_MaterialParameterSet != null )
            {
                material.METAPHOR_UseMaterialParameterSet = materialPreset.METAPHOR_UseMaterialParameterSet;
                material.METAPHOR_MaterialParameterSet = materialPreset.METAPHOR_MaterialParameterSet;
            }
            else
            {
                material.LegacyParameters = materialPreset.LegacyParameters;
            }

            TextureMap NewTextureMapResource(string name)
            {
                TextureMap newMap = new( name );
                if (material.METAPHOR_UseMaterialParameterSet)
                    newMap.METAPHOR_ParentMaterialParameterSet = material.METAPHOR_MaterialParameterSet;
                return newMap;
            }

            if ( materialPreset.DiffuseMap != null ) material.DiffuseMap = NewTextureMapResource( diffuseMapName );
            if ( materialPreset.Version >= 0x2000000)
            {
                // force adding toon shadow map to character models so that they don't crash by default
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType2_3_13)
                    material.SpecularMap = NewTextureMapResource( diffuseMapName );
                // distortion multiply map
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType4 )
                    material.HighlightMap = NewTextureMapResource( diffuseMapName );
            } else
            {
                if ( materialPreset.NormalMap != null ) material.NormalMap = NewTextureMapResource( normalMapName );
                if ( materialPreset.ReflectionMap != null ) material.ReflectionMap = NewTextureMapResource( reflectionMapName );
                if ( materialPreset.SpecularMap != null ) material.SpecularMap = NewTextureMapResource( specularMapName );
            }

            material.RuntimeMetadata.IsCustomMaterial = false;

            return material;
        }

        public static Material CreateMaterial( string name, string diffuseMapName, Material materialPreset )
        {
            return CreateMaterial(name, diffuseMapName, null, null, null, null, null, null, null, null, null, materialPreset );
        }
    }
}
