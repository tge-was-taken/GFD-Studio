using GFDLibrary.Conversion;
using GFDLibrary.Models;

namespace GFDLibrary.Materials
{
    public static class MaterialFactory
    {
        public static Material CreateMaterial( string name, string diffuseMapName, string lightmapName, string displacementMapName, string opacityMapName, string normalMapName, 
            string heightMapName, string emissiveMapName, string ambientMapName, string specularMapName, string reflectionMapName, Material materialPreset,
            string highlightMapName = null, string glowMapName = null, string nightMapName = null, string detailMapName = null,
            string shadowMapName = null, string textureMap10Name = null )
        {
            var material = new Material( name )
            {
                Version = materialPreset.Version,

                DrawMethod = materialPreset.DrawMethod,
                BlendSourceColor = materialPreset.BlendSourceColor,
                BlendDestinationColor = materialPreset.BlendDestinationColor,
                SourceAlpha = materialPreset.SourceAlpha,
                DestinationAlpha = materialPreset.DestinationAlpha,
                MultiBlendMode = materialPreset.MultiBlendMode,
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
                MultiMap = null,
                DarkMap = null,
                NormalMap = null,
                ReflectionMap = null,
                LightMap = null,
                GlossMap = null,
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
                if ( name == null )
                    return null;

                TextureMap newMap = new( name );
                if (material.METAPHOR_UseMaterialParameterSet)
                    newMap.METAPHOR_ParentMaterialParameterSet = material.METAPHOR_MaterialParameterSet;
                return newMap;
            }

            if ( materialPreset.DiffuseMap != null ) material.DiffuseMap = NewTextureMapResource( diffuseMapName );
            if ( materialPreset.MultiMap != null ) material.MultiMap = NewTextureMapResource( highlightMapName );
            if ( materialPreset.GlowMap != null ) material.GlowMap = NewTextureMapResource( glowMapName );
            if ( materialPreset.DarkMap != null ) material.DarkMap = NewTextureMapResource( nightMapName );
            if ( materialPreset.DetailMap != null ) material.DetailMap = NewTextureMapResource( detailMapName );
            if ( materialPreset.LightMap != null ) material.LightMap = NewTextureMapResource( shadowMapName );
            if ( materialPreset.ReserveMap != null ) material.ReserveMap = NewTextureMapResource( textureMap10Name );
            if ( materialPreset.Version >= 0x2000000)
            {
                // force adding toon shadow map to character models so that they don't crash by default
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType2_3_13)
                    material.GlossMap = NewTextureMapResource( diffuseMapName );
                // distortion multiply map
                if ( material.METAPHOR_MaterialParameterSet.ResourceType == ResourceType.MaterialParameterSetType4 )
                    material.MultiMap = NewTextureMapResource( diffuseMapName );
            } else
            {
                if ( materialPreset.NormalMap != null ) material.NormalMap = NewTextureMapResource( normalMapName );
                if ( materialPreset.ReflectionMap != null ) material.ReflectionMap = NewTextureMapResource( reflectionMapName );
                if ( materialPreset.GlossMap != null ) material.GlossMap = NewTextureMapResource( specularMapName );
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
