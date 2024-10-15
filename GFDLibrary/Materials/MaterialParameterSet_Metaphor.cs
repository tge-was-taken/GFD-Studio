using Assimp;
using GFDLibrary.IO;
using GFDLibrary.Models.Conversion.Utilities;
using System;
using System.Numerics;

using Ai = Assimp;

namespace GFDLibrary.Materials
{
    [AttributeUsage( AttributeTargets.Property, Inherited = false )]
    public class ShaderUniformAttribute : Attribute
    {
        public string Uniform { get; protected set; }
        public ShaderUniformAttribute( string _Uniform )
        {
            Uniform = _Uniform;
        }
    }
    public abstract class MaterialParameterSetBase : Resource
    {
        public abstract string GetParameterName();
        public void Write( ResourceWriter writer ) => WriteCore( writer );

        public virtual string GetTextureMap1Name() => "Base Map";
        public virtual string GetTextureMap2Name() => "Map 2";
        public virtual string GetTextureMap3Name() => "Map 3";
        public virtual string GetTextureMap4Name() => "Map 4";
        public virtual string GetTextureMap5Name() => "Map 5";
        public virtual string GetTextureMap6Name() => "Map 6";
        public virtual string GetTextureMap7Name() => "Map 7";
        public virtual string GetTextureMap8Name() => "Map 8";
        public virtual string GetTextureMap9Name() => "Map 9";
        public virtual string GetTextureMap10Name() => "Map 10";
        public virtual void ConvertToAssimp( ref Ai.Material material ) { }
        public abstract void SetShaderFlags( Material mat );
        public virtual bool IsMaterialTransparent() => false;
    }

    // 7.HLSL or 9.HLSL
    public sealed class MaterialParameterSetType0 : MaterialParameterSetBase
    {
        [ShaderUniform( "baseColor" )]
        public Vector4 P0_0 { get; set; } // 0x90
        [ShaderUniform( "emissiveStrength" )]
        public float P0_1 { get; set; } // 0xa0
        [ShaderUniform( "roughness" )]
        public float P0_2 { get; set; } // 0xa4
        [ShaderUniform( "metallic" )]
        public float P0_3 { get; set; } // 0xa8
        [ShaderUniform( "multiAlpha" )]
        public float P0_4 { get; set; } // 0xac
        [ShaderUniform( "bloomIntensity" )]
        public float P0_5 { get; set; } // 0xb0
        public Type0Flags Flags { get; set; } // 0xb4

        [Flags]
        public enum Type0Flags : uint
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
            FLAG2 = 0x00000004,
            FLAG3 = 0x00000008,
            FLAG4 = 0x00000010,
            InfluencedBySky = 0x00000020,
            Transparency = 0x00000040,
            MultiTextureMask = 0x00000080,
            RemoveDiffuseShadow = 0x00000100,
            BillboardShadowMap = 0x00000200,
            FLAG10 = 0x00000400,
            FLAG11 = 0x00000800,
        }

        public MaterialParameterSetType0()
        {
            Flags = 0;
        }

        public override string GetParameterName() => "Parameter Type 0";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap8Name() => "PBR Params Map"; // Roughness, Metallic, Emissive, Intensity

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType0;

        protected override void ReadCore( ResourceReader reader )
        {
            P0_0 = reader.ReadVector4();
            P0_1 = reader.ReadSingle();
            P0_2 = reader.ReadSingle();
            P0_3 = reader.ReadSingle();
            P0_4 = 1;
            if ( Version >= 0x2000004 )
                P0_4 = reader.ReadSingle();
            P0_5 = 1;
            if ( Version >= 0x2030001 )
                P0_5 = reader.ReadSingle();
            if ( Version > 0x2110040 )
                Flags = (Type0Flags)reader.ReadUInt32();
            // Unused parameter - value gets discarded in stream reader
            if ( Version == 0x2110140 )
                reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P0_0 );
            writer.WriteSingle( P0_1 );
            writer.WriteSingle( P0_2 );
            writer.WriteSingle( P0_3 );
            if ( Version >= 0x2000004 ) 
                writer.WriteSingle( P0_4 );
            if ( Version >= 0x2030001 )
                writer.WriteSingle( P0_5 );
            if ( Version > 0x2110040 )
                 writer.WriteUInt32( (uint)Flags );
            if ( Version == 0x2110140 )
                writer.WriteSingle( 0 );
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( Flags.HasFlag( Type0Flags.InfluencedBySky ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SKY;
            if ( Flags.HasFlag( Type0Flags.MultiTextureMask ) && mat.Flags.HasFlag(MaterialFlags.HasHighlightMap) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_MULTITEXTURE_AS_MASK;
            if ( Flags.HasFlag( Type0Flags.RemoveDiffuseShadow ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_STAR; // FLAG2_REMOVAL_DIFFUSESHADOW
            if ( Flags.HasFlag( Type0Flags.BillboardShadowMap ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SPECULAR_NORMALMAPALPHA; // FLAG2_BILLBOARD_SHADOWMAP
            
            if ( Flags.HasFlag( Type0Flags.Transparency ) && mat.Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
                if ( mat.DrawMethod == MaterialDrawMethod.Opaque )
                    mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_PUNCHTHROUGH;
        }

        public override bool IsMaterialTransparent() => Flags.HasFlag( Type0Flags.Transparency );
    }

    // 3.HLSL
    public sealed class MaterialParameterSetType1 : MaterialParameterSetBase
    {
        [ShaderUniform( "matAmbient" )]
        public Vector4 AmbientColor { get; set; } // 0x90
        [ShaderUniform( "matDiffuse" )]
        public Vector4 DiffuseColor { get; set; } // 0xa0
        [ShaderUniform( "matSpecular" )]
        public Vector4 SpecularColor { get; set; } // 0xb0
        [ShaderUniform( "matEmissive" )]
        public Vector4 EmissiveColor { get; set; } // 0xc0
        [ShaderUniform( "matReflectivity" )]
        public float Reflectivity { get; set; } // 0xd0
        public float LerpBlendRate { get; set; } // 0xd4

        public override string GetParameterName() => "Parameter Type 1";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap6Name() => "Emissive Map";
        public override string GetTextureMap8Name() => "Toon Params Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType1;

        protected override void ReadCore( ResourceReader reader )
        {
            AmbientColor = reader.ReadVector4();
            DiffuseColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            Reflectivity = reader.ReadSingle();
            LerpBlendRate = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteSingle( Reflectivity );
            writer.WriteSingle( LerpBlendRate );
        }
        public override void SetShaderFlags( Material mat )
        {
            if (mat.Flags.HasFlag( MaterialFlags.HasReflection ) && mat.Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                mat.MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_REFLECTION | METAPHOR_MaterialFlags1.FLAG1_MATERIAL_REFLECTION_LERP;
                //if (a)
                //    mat.MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_REFLECTION_ADD;

            }
        }
        public override bool IsMaterialTransparent() => DiffuseColor.W < 1.0;
    }

    public abstract class MaterialParameterSetType2Base : MaterialParameterSetBase
    {
        [ShaderUniform( "matBaseColor" )]
        public Vector4 BaseColor { get; set; } // 0x90
        [ShaderUniform( "matShadowColor" )]
        public Vector4 ShadowColor { get; set; } // 0xa0
        [ShaderUniform( "matEdgeColor" )]
        public Vector4 EdgeColor { get; set; } // 0xb0
        [ShaderUniform( "matEmissiveColor" )]
        public Vector4 EmissiveColor { get; set; } // 0xc0
        [ShaderUniform( "matSpecularColor" )]
        public Vector3 SpecularColor { get; set; } // 0xd0
        [ShaderUniform( "matSpecularPower" )]
        public float SpecularPower { get; set; } // 0xe0
        [ShaderUniform( "matMetallic" )]
        public float Metallic { get; set; } // 0xe4
        [ShaderUniform( "edgeThreshold" )]
        public float EdgeThreshold { get; set; } // 0xf0
        [ShaderUniform( "edgeFactor" )]
        public float EdgeFactor { get; set; } // 0xf4
        [ShaderUniform( "shadowThreshold" )]
        public float ShadowThreshold { get; set; } // 0xfc
        [ShaderUniform( "shadowFactor" )]
        public float ShadowFactor { get; set; } // 0x100
        public Type2Flags Flags { get; set; } // 0x130
        public float P2_12 { get; set; } // 0x104
        public Vector3 P2_13 { get; set; } // 0x10c
        public float MatBloomIntensity { get; set; } // 0xec
        [ShaderUniform( "matSpecularThreshold" )]
        public float SpecularThreshold { get; set; } // 0xdc
        [ShaderUniform( "edgeRemoveYAxisFactor" )]
        public float EdgeRemoveYAxisFactor { get; set; } // 0xf8
        public float P2_17 { get; set; } // 0x118
        public float P2_18 { get; set; } // 0x11c
        public float P2_19 { get; set; } // 0x120
        public float P2_20 { get; set; } // 0x108
        [ShaderUniform( "matRoughness" )]
        public float MatRoughness { get; set; } // 0xe8
        [ShaderUniform( "fittingTile" )]
        public float FittingTile { get; set; } // 0x128
        [ShaderUniform( "multiFittingTile" )]
        public float MultiFittingTile { get; set; } // 0x12c

        [Flags]
        public enum Type2Flags : uint
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
            FLAG2 = 0x00000004,
            FLAG3 = 0x00000008,
            FLAG4 = 0x00000010,
            FLAG5 = 0x00000020,
            FLAG6 = 0x00000040,
            FLAG7 = 0x00000080,
            FLAG8 = 0x00000100,
            FLAG9 = 0x00000200,
            FLAG10 = 0x00000400,
            FLAG11 = 0x00000800,
            FLAG12 = 0x00001000,
            FLAG13 = 0x00002000,
            FLAG14 = 0x00004000,
            FLAG15 = 0x00008000,
            FLAG16 = 0x00010000,
            FLAG17 = 0x00020000,
            FLAG18 = 0x00040000,
            FLAG19 = 0x00080000,
        }

        public override string GetParameterName() => "Parameter Type 2";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap3Name() => "Toon Shadow Color Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap6Name() => "Toon Params 2 Map";
        public override string GetTextureMap8Name() => "Toon Params Map";
        public override string GetTextureMap9Name() => "Toon Edge Color Map";
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType2_3_13;
        protected override void ReadCore( ResourceReader reader )
        {
            BaseColor = reader.ReadVector4();
            ShadowColor = reader.ReadVector4();
            EdgeColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector3();
            SpecularPower = reader.ReadSingle();
            Metallic = reader.ReadSingle();
            EdgeThreshold = reader.ReadSingle();
            EdgeFactor = reader.ReadSingle();
            ShadowThreshold = reader.ReadSingle();
            ShadowFactor = reader.ReadSingle();
            Flags = (Type2Flags)reader.ReadUInt32();
            if ( Version > 0x200ffff )
            {
                P2_12 = reader.ReadSingle();
                P2_13 = reader.ReadVector3();
            }
            MatBloomIntensity = 0.5f;
            if ( Version >= 0x2030001 )
                MatBloomIntensity = reader.ReadSingle();
            SpecularThreshold = 0.5f;
            if ( Version > 0x2090000 )
                SpecularThreshold = reader.ReadSingle();
            EdgeRemoveYAxisFactor = 3f;
            if ( Version >= 0x2094001 )
                EdgeRemoveYAxisFactor = reader.ReadSingle();
            P2_17 = 1;
            P2_18 = -1;
            P2_19 = 0;
            if ( Version >= 0x2109501 )
            {
                P2_17 = reader.ReadSingle();
                P2_18 = reader.ReadSingle();
                P2_19 = reader.ReadSingle();
            }
            P2_20 = 0.1f;
            if ( Version >= 0x2109601 )
                P2_20 = reader.ReadSingle();
            if ( Version > 0x2110197 )
                MatRoughness = reader.ReadSingle();
            if ( Version > 0x2110203 )
                FittingTile = reader.ReadSingle();
            if ( Version > 0x2110209 )
                MultiFittingTile = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( BaseColor );
            writer.WriteVector4( ShadowColor );
            writer.WriteVector4( EdgeColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteVector3( SpecularColor );
            writer.WriteSingle( SpecularPower );
            writer.WriteSingle( Metallic );
            writer.WriteSingle( EdgeThreshold );
            writer.WriteSingle( EdgeFactor );
            writer.WriteSingle( ShadowThreshold );
            writer.WriteSingle( ShadowFactor );
            writer.WriteUInt32( (uint)Flags );
            if ( Version > 0x200ffff )
            {
                writer.WriteSingle( P2_12 );
                writer.WriteVector3( P2_13 );
            }
            if ( Version >= 0x2030001 )
                writer.WriteSingle( MatBloomIntensity );
            if ( Version > 0x2090000 )
                writer.WriteSingle( SpecularThreshold );
            if ( Version >= 0x2094001 )
                writer.WriteSingle( EdgeRemoveYAxisFactor );
            if ( Version >= 0x2109501 )
            {
                writer.WriteSingle( P2_17 );
                writer.WriteSingle( P2_18 );
                writer.WriteSingle( P2_19 );
            }
            if ( Version >= 0x2109601 )
                writer.WriteSingle( P2_20 );
            if ( Version > 0x2110197 )
                writer.WriteSingle( MatRoughness );
            if ( Version > 0x2110203 )
                writer.WriteSingle( FittingTile );
            if ( Version > 0x2110209 )
                writer.WriteSingle( MultiFittingTile );
        }
        //public override bool IsMaterialTransparent( ) => Flags.HasFlag( Type2Flags.FLAG10 );
    }
    // 11.HLSL
    public sealed class MaterialParameterSetType2 : MaterialParameterSetType2Base
    {
        public override void SetShaderFlags( Material mat )
        {
            //if ( bVar5 )
            //    mat.MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_REFLECTION;
            if ( Flags.HasFlag( Type2Flags.FLAG0 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type2Flags.FLAG1 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REMOVAL_LIGHT_YAXIS;
            if ( Flags.HasFlag( Type2Flags.FLAG2 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type2Flags.FLAG5 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALALPHA;
            if ( Flags.HasFlag( Type2Flags.FLAG6 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_SEMITRANS;
            if ( Flags.HasFlag( Type2Flags.FLAG7 ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_EDGE_REMOVAL_LIGHT_YAXIS;
            //if ( vertexFlags & 0x800 )
            //    mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_OUTLINE;
            if ( Flags.HasFlag( Type2Flags.FLAG9 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_STAR;
            if ( Flags.HasFlag( Type2Flags.FLAG10 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_PUNCHTHROUGH;
            if ( Flags.HasFlag( Type2Flags.FLAG13 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_APPLY_PBR_LIGHT;
            if ( Flags.HasFlag( Type2Flags.FLAG14 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SPECULAR_NORMALMAPALPHA;
        }
    }
    // 13.HLSL
    public sealed class MaterialParameterSetType3 : MaterialParameterSetType2Base
    {
        public override void SetShaderFlags( Material mat )
        {

        }
    }
    // 15.HLSL
    public sealed class MaterialParameterSetType13 : MaterialParameterSetType2Base
    {
        public override void SetShaderFlags( Material mat )
        {

        }
    }

    // 21.HLSL
    public sealed class MaterialParameterSetType4 : MaterialParameterSetBase
    {
        [ShaderUniform( "baseColor" )]
        public Vector4 BaseColor { get; set; } // 0x90
        [ShaderUniform( "emissiveColor" )]
        public Vector4 EmissiveColor { get; set; } // 0xa0
        [ShaderUniform( "distortionPower" )]
        public float DistortionPower { get; set; } // 0xb0
        [ShaderUniform( "distortionThreshold" )]
        public float DistortionThreshold { get; set; } // 0xb4
        public float P4_4 { get; set; } // 0xb8
        public Type4Flags Flags { get; set; } // 0xcc
        [ShaderUniform( "matBloomIntensity" )]
        public float MatBloomIntensity { get; set; } // 0xbc
        public float P4_7 { get; set; } // 0xc0
        public float P4_8 { get; set; } // 0xc4

        [Flags]
        public enum Type4Flags : uint
        {
            FlowMapMultiAsMap = 0x00000001,
            FlowMapRimTransparency = 0x00000002,
            RimTransNegate = 0x00000004,
            FlowMapBgDistortion = 0x00000008,
            FlowMapMultiAlphaColorBlend = 0x00000010,
            FlowMapAlphaDistortion = 0x00000020,
            FlowMapAlphaMaskDistortion = 0x00000040,
            FlowMapApplyAlphaOnly = 0x00000080,
            SoftParticle = 0x00000100,
            ForceBloomIntensity = 0x00000200,
            Fitting = 0x00000400,
            FlowMapDisablwColorCorrection = 0x00000800,
            MultiFitting = 0x00001000,
            FlowMapMultiRefAlphaBaseColor = 0x00002000,
            FlowMapBloomRefAlphaMultiColor = 0x00004000,
            FLAG15 = 0x00008000,
        }

        public override string GetParameterName() => "Parameter Type 4";
        public override string GetTextureMap3Name() => "Dissolve Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap6Name() => "Emissive Map";
        public override string GetTextureMap7Name() => "Transparency Map";
        public override string GetTextureMap8Name() => "Distortion Map";
        public override string GetTextureMap9Name() => "Alpha Mask Map";
        public override string GetTextureMap10Name() => "Alpha Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType4;

        protected override void ReadCore( ResourceReader reader )
        {
            BaseColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            DistortionPower = reader.ReadSingle();
            DistortionThreshold = reader.ReadSingle();
            P4_4 = reader.ReadSingle();
            Flags = (Type4Flags)reader.ReadUInt32();
            MatBloomIntensity = 0.5f;
            if ( Version >= 0x2110184 )
                MatBloomIntensity = reader.ReadSingle();
            P4_7 = 1;
            if ( Version > 0x2110203 )
                P4_7 = reader.ReadSingle();
            if ( Version > 0x2110217 )
                P4_8 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(BaseColor);
            writer.WriteVector4(EmissiveColor);
            writer.WriteSingle(DistortionPower);
            writer.WriteSingle(DistortionThreshold);
            writer.WriteSingle(P4_4);
            writer.WriteUInt32( (uint)Flags );
            if ( Version >= 0x2110184 )
                writer.WriteSingle( MatBloomIntensity );
            if ( Version > 0x2110203 )
                writer.WriteSingle( P4_7 );
            if ( Version > 0x2110217 )
                writer.WriteSingle( P4_8 );
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( Flags.HasFlag( Type4Flags.FlowMapMultiAsMap ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP; // FLAG2_FLOWMAP_MULTIASMASK
            if ( Flags.HasFlag( Type4Flags.FlowMapRimTransparency ) )
            {
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_STAR; // FLAG2_FLOWMAP_RIMTRANSPARENCY
                if ( Flags.HasFlag( Type4Flags.RimTransNegate ) )
                    mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_CAVERNMAP; // FLAG2_FLOWMAP_RIMTRANSNEG
            }
            if ( Flags.HasFlag( Type4Flags.FlowMapBgDistortion ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALALPHA; // FLAG2_FLOWMAP_BGDISTORTION
            if ( Flags.HasFlag( Type4Flags.FlowMapMultiAlphaColorBlend ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_DIFFUSEALPHA; // FLAG2_FLOWMAP_MULTIALPHA_COLORBLENDONLY
            if ( Flags.HasFlag( Type4Flags.FlowMapAlphaDistortion ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REFERENCE_NORMALMAP; // FLAG2_FLOWMAP_ALPHADISTORTION
            if ( Flags.HasFlag( Type4Flags.FlowMapAlphaMaskDistortion ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REMOVAL_LIGHT_YAXIS; // FLAG2_FLOWMAP_ALPHAMASKDISTORTION
            if ( Flags.HasFlag( Type4Flags.FlowMapApplyAlphaOnly ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_BACKLIGHT; // FLAG2_FLOWMAP_APPLYALPHAONLY
            if ( Flags.HasFlag( Type4Flags.SoftParticle ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SOFT_PARTICLE; 
            if ( Flags.HasFlag( Type4Flags.ForceBloomIntensity ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SPECULAR_NORMALMAPALPHA; // FLAG2_FORCED_BLOOMINTENSITY

            if ( Flags.HasFlag( Type4Flags.Fitting ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_LIGHT1_SPOT; // FLAG0_FITTING
            else if ( Flags.HasFlag( Type4Flags.FlowMapDisablwColorCorrection ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_LIGHT2_DIRECTION; // FLAG0_FLOWMAP_COLORCORRECT_DISABLE
            else if ( Flags.HasFlag( Type4Flags.MultiFitting ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_LIGHT0_SPOT; // FLAG0_MULTI_FITTING

            if ( Flags.HasFlag( Type4Flags.FlowMapMultiRefAlphaBaseColor ) )
                mat.MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_LIGHTMAP_MODULATE2; // FLAG1_FLOWMAP_MULTI_REF_ALPHA_BASECOLOR
            if ( Flags.HasFlag( Type4Flags.FlowMapBloomRefAlphaMultiColor ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_LIGHT2_SPOT; // FLAG0_FLOWMAP_BLOOM_REF_ALPHA_MULTICOLOR
            if ( mat.DrawMethod == MaterialDrawMethod.Opaque )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_PUNCHTHROUGH;
        }
        //public override bool IsMaterialTransparent( ) => Flags.HasFlag( Type4Flags.FLAG15 );
        public override bool IsMaterialTransparent( ) => true;
    }

    // 23.HLSL
    public sealed class MaterialParameterSetType5 : MaterialParameterSetBase
    {
        public float P5_0 { get; set; } // 0x90
        public float P5_1 { get; set; } // 0x94
        [ShaderUniform( "tcScale" )]
        public float P5_2 { get; set; } // 0x98
        public float P5_3 { get; set; } // 0x9c
        [ShaderUniform( "oceanDepthScale" )]
        public float P5_4 { get; set; } // 0xa0
        [ShaderUniform( "disturbanceCameraScale" )]
        public float P5_5 { get; set; } // 0xa4
        [ShaderUniform( "disturbanceDepthScale" )]
        public float P5_6 { get; set; } // 0xa8
        [ShaderUniform( "scatteringCameraScale" )]
        public float P5_7 { get; set; } // 0xac
        [ShaderUniform( "disturbanceTolerance" )]
        public float P5_8 { get; set; } // 0xb0
        [ShaderUniform( "foamDistance" )]
        public float P5_9 { get; set; } // 0xb4
        [ShaderUniform( "causticsTolerance" )]
        public float P5_10 { get; set; } // 0xb8
        public float P5_11 { get; set; } // 0xbc
        [ShaderUniform( "textureAnimationSpeed" )]
        public float P5_12 { get; set; } // 0xc0
        public float P5_13 { get; set; } // 0xc4
        public Type5Flags Flags { get; set; } // 0xc8

        [Flags]
        public enum Type5Flags
        {
            InfluencedBySky = 0x00000001,
            HasWaterReflection = 0x00000002,
            IsInfinite = 0x00000004,
            OutlineAttenuationInvalid = 0x00000008,
        }

        public override string GetParameterName() => "Water Parameters";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType5;

        protected override void ReadCore( ResourceReader reader )
        {
            P5_0 = reader.ReadSingle();
            P5_1 = reader.ReadSingle();
            P5_2 = reader.ReadSingle();
            P5_3 = reader.ReadSingle();
            P5_4 = reader.ReadSingle();
            P5_5 = reader.ReadSingle();
            P5_6 = reader.ReadSingle();
            P5_7 = reader.ReadSingle();
            P5_8 = reader.ReadSingle();
            P5_9 = reader.ReadSingle();
            P5_10 = reader.ReadSingle();
            if ( Version >= 0x2110182 )
            {
                P5_11 = reader.ReadSingle();
                P5_12 = reader.ReadSingle();
            }
            if ( Version >= 0x2110205 )
                P5_13 = reader.ReadSingle();
            if ( Version >= 0x2110188 )
                Flags = (Type5Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( P5_0 );
            writer.WriteSingle( P5_1 );
            writer.WriteSingle( P5_2 );
            writer.WriteSingle( P5_3 );
            writer.WriteSingle( P5_4 );
            writer.WriteSingle( P5_5 );
            writer.WriteSingle( P5_6 );
            writer.WriteSingle( P5_7 );
            writer.WriteSingle( P5_8 );
            writer.WriteSingle( P5_9 );
            writer.WriteSingle( P5_10 );
            if ( Version >= 0x2110182 )
            {
                writer.WriteSingle(P5_11);
                writer.WriteSingle( P5_12 );
            }
            if ( Version >= 0x2110205 )
                writer.WriteSingle( P5_13 );
            if ( Version >= 0x2110188 )
                writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {

            if ( Flags.HasFlag( Type5Flags.InfluencedBySky ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SKY;
            if ( Flags.HasFlag( Type5Flags.HasWaterReflection ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_WATER_REFLECTION;
            if ( Flags.HasFlag( Type5Flags.IsInfinite ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_INFINITE;
            if ( Flags.HasFlag( Type5Flags.OutlineAttenuationInvalid ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_OUTLINE_ATTENUATION_INVALID;
        }
        //public override bool IsMaterialTransparent( ) => false;
    }
    // 27.HLSL or 25.HLSL
    public sealed class MaterialParameterSetType6 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType6_Field0
        {
            public Vector4 Field0 { get; set; } // Base Color
            public float Field1 { get; set; } // Emissive 
            public float Field2 { get; set; } // Roughness
            public float Field3 { get; set; } // Bloom Intensity
            public float Field4 { get; set; }
        }
        private MaterialParameterSetType6_Field0[] mP6_0;
        public MaterialParameterSetType6_Field0[] P6_0 // 0x90
        {
            get => mP6_0;
            set => mP6_0 = value;
        }
        public float P6_1 { get; set; } // 0xd0
        public float P6_2 { get; set; } // 0xd4
        public float P6_3 { get; set; } // 0xd8
        public float P6_4 { get; set; } // 0xdc
        public Type6Flags Flags { get; set; } // 0xe0

        [Flags]
        public enum Type6Flags : uint
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
            FLAG2 = 0x00000004,
            FLAG3 = 0x00000008,
            FLAG4 = 0x00000010,
            FLAG5 = 0x00000020,
            FLAG6 = 0x00000040,
            FLAG7 = 0x00000080,
            FLAG8 = 0x00000100,
            FLAG9 = 0x00000200,
            FLAG10 = 0x00000400,
            FLAG11 = 0x00000800,
            FLAG12 = 0x00001000,
            FLAG13 = 0x00002000,
            FLAG14 = 0x00004000,
            FLAG15 = 0x00008000,
            FLAG16 = 0x00010000,
            FLAG17 = 0x00020000,
            FLAG18 = 0x00040000,
            FLAG19 = 0x00080000,
        }

        public override string GetParameterName() => "Parameter Type 6";

        public override string GetTextureMap1Name() => "Layer 0 Base Map";
        public override string GetTextureMap2Name() => "Layer 0 Normal Map";
        public override string GetTextureMap5Name() => "Blend Map";
        public override string GetTextureMap6Name() => "Layer 0 PBR Params Map";
        public override string GetTextureMap7Name() => "Layer 1 Base Map";
        public override string GetTextureMap8Name() => "Layer 1 Normal Map";
        public override string GetTextureMap9Name() => "Layer 1 PBR Params Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType6;

        protected override void ReadCore( ResourceReader reader )
        {
            P6_0 = new MaterialParameterSetType6_Field0[2];
            for ( int i = 0; i < 2; i++ )
            {
                P6_0[i].Field0 = reader.ReadVector4();
                P6_0[i].Field1 = reader.ReadSingle();
                P6_0[i].Field2 = reader.ReadSingle();
                P6_0[i].Field3 = reader.ReadSingle();
                P6_0[i].Field4 = reader.ReadSingle();
            }
            P6_1 = reader.ReadSingle();
            if ( Version >= 0x2110021 ) {
                P6_2 = reader.ReadSingle();
                P6_3 = reader.ReadSingle();
            }
            P6_4 = reader.ReadSingle();
            Flags = (Type6Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for (int i = 0; i < 2; i++ )
            {
                writer.WriteVector4(P6_0[i].Field0);
                writer.WriteSingle(P6_0[i].Field1);
                writer.WriteSingle(P6_0[i].Field2);
                writer.WriteSingle(P6_0[i].Field3);
                writer.WriteSingle( P6_0[i].Field4 );
            }
            writer.WriteSingle( P6_1 );
            if ( Version >= 0x2110021 )
            {
                writer.WriteSingle(P6_2 );
                writer.WriteSingle( P6_3 );
            }
            writer.WriteSingle(P6_4 );
            writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( !Flags.HasFlag( Type6Flags.FLAG0) )
            {
                if ( Flags.HasFlag( Type6Flags.FLAG2 ) )
                    mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP; // FLAG2_TRIPLANARMAPPING_LAYER0
                if ( Flags.HasFlag( Type6Flags.FLAG3 ) )
                    mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_CAVERNMAP; // FLAG2_TRIPLANARMAPPING_LAYER1
                if ( Flags.HasFlag( Type6Flags.FLAG4 ) )
                    mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALALPHA; // FLAG2_TRIPLANARMAPPING_BLEND
            } else
            {
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP | METAPHOR_MaterialFlags2.FLAG2_EDGE_CAVERNMAP | METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALALPHA;
            }
            if ( Flags.HasFlag( Type6Flags.FLAG1 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_STAR; // FLAG2_USECOLORSET2
            // check outline
            if ( Flags.HasFlag( Type6Flags.FLAG5 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SKY;

        }
        //public override bool IsMaterialTransparent( ) => true;
    }
    // 29.HLSL
    public sealed class MaterialParameterSetType7 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType7_Field0
        {
            public float Field0 { get; set; }
            public float Field1 { get; set; }
            public float Field2 { get; set; }
            public float Field3 { get; set; }
            public float Field4 { get; set; }
            public float Field5 { get; set; }
        }
        private MaterialParameterSetType7_Field0[] mP7_0;
        public MaterialParameterSetType7_Field0[] P7_0 // 0x90
        {
            get => mP7_0;
            set => mP7_0 = value;
        }
        public float P7_1 { get; set; } // 0xf0
        public Type7Flags Flags { get; set; } // 0xf4

        [Flags]
        public enum Type7Flags
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
        }

        public override string GetParameterName() => "Parameter Type 7";
        public override string GetTextureMap1Name() => "Layer 0 Base Map";
        public override string GetTextureMap2Name() => "Layer 0 Normal Map";
        public override string GetTextureMap3Name() => "Layer 1 Normal Map";
        public override string GetTextureMap5Name() => "Blend Map";
        public override string GetTextureMap6Name() => "Layer 1 Base Map";
        public override string GetTextureMap7Name() => "Layer 2 Base Map";
        public override string GetTextureMap8Name() => "Layer 2 Normal Map";
        public override string GetTextureMap9Name() => "Layer 3 Base Map";
        public override string GetTextureMap10Name() => "Layer 3 Normal Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType7;

        protected override void ReadCore( ResourceReader reader )
        {
            P7_0 = new MaterialParameterSetType7_Field0[4];
            for ( int i = 0; i < 4; i++ )
            {
                P7_0[i].Field0 = reader.ReadSingle();
                P7_0[i].Field1 = reader.ReadSingle();
                P7_0[i].Field2 = reader.ReadSingle();
                P7_0[i].Field3 = reader.ReadSingle();
                P7_0[i].Field4 = reader.ReadSingle();
                P7_0[i].Field5 = reader.ReadSingle();
            }
            P7_1 = reader.ReadSingle();
            Flags = (Type7Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for ( int i = 0; i < 4; i++ )
            {
                writer.WriteSingle( P7_0[i].Field0 );
                writer.WriteSingle( P7_0[i].Field1 );
                writer.WriteSingle( P7_0[i].Field2 );
                writer.WriteSingle( P7_0[i].Field3 );
                writer.WriteSingle( P7_0[i].Field4 );
                writer.WriteSingle( P7_0[i].Field5 );
            }
            writer.WriteSingle( P7_1 );
            writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {

            if ( Flags.HasFlag( Type7Flags.FLAG0 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP; // FLAG2_TRIPLANARMAPPING
            if ( Flags.HasFlag( Type7Flags.FLAG1 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SKY;
        }
        //public override bool IsMaterialTransparent( ) => false;
    }
    // 33.HLSL or 31.HLSL
    public sealed class MaterialParameterSetType8 : MaterialParameterSetBase
    {
        public float P8_0 { get; set; } // 0x90
        public float P8_1 {get; set;} // 0x94
        public float P8_2 {get; set;} // 0x98
        public float P8_3 {get; set;} // 0x9c
        public Vector4 P8_4 {get; set;} // 0xa0
        public float P8_5 {get; set;} // 0xb0
        public float P8_6 {get; set;} // 0xb4
        public float P8_7 {get; set;} // 0xb8
        public float P8_8 {get; set;} // 0xbc

        public override string GetParameterName() => "Parameter Type 8";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType8;

        protected override void ReadCore( ResourceReader reader )
        {
            P8_0 = reader.ReadSingle();
            P8_1 = reader.ReadSingle();
            P8_2 = reader.ReadSingle();
            P8_3 = reader.ReadSingle();
            P8_4 = reader.ReadVector4();
            P8_5 = reader.ReadSingle();
            P8_6 = reader.ReadSingle();
            P8_7 = reader.ReadSingle();
            P8_8 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(P8_0 );
            writer.WriteSingle(P8_1 );
            writer.WriteSingle(P8_2 );
            writer.WriteSingle(P8_3 );
            writer.WriteVector4(P8_4 );
            writer.WriteSingle(P8_5 );
            writer.WriteSingle(P8_6 );
            writer.WriteSingle(P8_7 );
            writer.WriteSingle( P8_8 );
        }
        public override void SetShaderFlags( Material mat )
        {

        }
        //public override bool IsMaterialTransparent( ) => true;
    }
    // 35.HLSL
    public sealed class MaterialParameterSetType9 : MaterialParameterSetBase
    {
        public float P9_0 { get; set; } // 0x90
        public float P9_1 { get; set; } // 0x94
        public float P9_2 { get; set; } // 0x98
        public float P9_3 { get; set; } // 0x9c
        public Vector4 P9_4 { get; set; } // 0xa0
        public Vector4 P9_5 { get; set; } // 0xb0
        public Vector4 P9_6 { get; set; } // 0xc0
        public Vector4 P9_7 { get; set; } // 0xd0
        public Vector3 P9_8 { get; set; } // 0xe0
        public float P9_9 { get; set; } // 0xec
        public float P9_10 { get; set; } // 0xf0
        public float P9_11 { get; set; } // 0xf4
        public float P9_12 { get; set; } // 0xf8
        public float P9_13 { get; set; } // 0xfc
        public float P9_14 { get; set; } // 0x100
        public float P9_15 { get; set; } // 0x104
        public float P9_16 { get; set; } // 0x108
        public Type9Flags Flags { get; set; } // 0x10c

        [Flags]
        public enum Type9Flags : uint
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
            FLAG2 = 0x00000004,
            FLAG3 = 0x00000008,
            FLAG4 = 0x00000010,
            FLAG5 = 0x00000020,
            FLAG6 = 0x00000040,
            FLAG7 = 0x00000080,
            FLAG8 = 0x00000100,
            FLAG9 = 0x00000200,
            FLAG10 = 0x00000400,
            FLAG11 = 0x00000800,
            FLAG12 = 0x00001000,
            FLAG13 = 0x00002000,
            FLAG14 = 0x00004000,
            FLAG15 = 0x00008000,
            FLAG16 = 0x00010000,
            FLAG17 = 0x00020000,
            FLAG18 = 0x00040000,
            FLAG19 = 0x00080000,
        }

        public override string GetParameterName() => "Parameter Type 9";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType9;

        protected override void ReadCore( ResourceReader reader )
        {
            P9_0 = reader.ReadSingle();
            P9_1 = reader.ReadSingle();
            P9_2 = reader.ReadSingle();
            P9_3 = reader.ReadSingle();
            P9_4 = reader.ReadVector4();
            P9_5 = reader.ReadVector4();
            P9_6 = reader.ReadVector4();
            P9_7 = reader.ReadVector4();
            P9_8 = reader.ReadVector3();
            P9_9 = reader.ReadSingle();
            P9_10 = reader.ReadSingle();
            P9_11 = reader.ReadSingle();
            P9_12 = reader.ReadSingle();
            P9_13 = reader.ReadSingle();
            P9_14 = reader.ReadSingle();
            P9_15 = reader.ReadSingle();
            P9_16 = reader.ReadSingle();
            Flags = (Type9Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(P9_0 );
            writer.WriteSingle(P9_1 );
            writer.WriteSingle(P9_2 );
            writer.WriteSingle(P9_3 );
            writer.WriteVector4(P9_4 );
            writer.WriteVector4(P9_5 );
            writer.WriteVector4(P9_6 );
            writer.WriteVector4(P9_7 );
            writer.WriteVector3(P9_8 );
            writer.WriteSingle(P9_9 );
            writer.WriteSingle(P9_10 );
            writer.WriteSingle(P9_11 );
            writer.WriteSingle(P9_12 );
            writer.WriteSingle(P9_13 );
            writer.WriteSingle(P9_14 );
            writer.WriteSingle(P9_15 );
            writer.WriteSingle(P9_16 );
            writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {
            // Enable reflection
            if ( Flags.HasFlag( Type9Flags.FLAG0 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type9Flags.FLAG1 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REMOVAL_LIGHT_YAXIS;
            if ( Flags.HasFlag( Type9Flags.FLAG2 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type9Flags.FLAG4 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALALPHA;
            if ( Flags.HasFlag( Type9Flags.FLAG5 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_SEMITRANS;

        }
        //public override bool IsMaterialTransparent( ) => true;
    }
    // 37.HLSL
    public sealed class MaterialParameterSetType10 : MaterialParameterSetBase
    {
        public Vector4 P10_0 { get; set; } // 0x90
        public float P10_1 { get; set; } // 0xa0
        public float P10_2 { get; set; } // 0xa4
        public Type10Flags Flags { get; set; } // 0xa8

        [Flags]
        public enum Type10Flags
        {
            FLAG0 = 0x00000001,
        }

        public override string GetParameterName() => "Parameter Type 10";
        public override string GetTextureMap1Name() => "Base Map";
        public override string GetTextureMap5Name() => "Multiply Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType10;

        protected override void ReadCore( ResourceReader reader )
        {
            P10_0 = reader.ReadVector4();
            if ( Version >= 0x2110091 )
                P10_1 = reader.ReadSingle();
            P10_2 = reader.ReadSingle();
            if ( Version > 0x2110100 )
                Flags = (Type10Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P10_0 );
            if ( Version >= 0x2110091 )
                writer.WriteSingle( P10_1 );
            writer.WriteSingle( P10_2 );
            if ( Version > 0x2110100 )
                writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( Flags.HasFlag( Type10Flags.FLAG0 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_BLEND_CLEARCOLOR;
        }
        //public override bool IsMaterialTransparent( ) => true;
    }
    // 41.HLSL or 39.HLSL
    public sealed class MaterialParameterSetType11 : MaterialParameterSetBase
    {
        public Vector4 P11_0 { get; set; } // 0x90
        public float P11_1 { get; set; } // 0xa0

        public override string GetParameterName() => "Parameter Type 11";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap6Name() => "Emissive Map";
        public override string GetTextureMap8Name() => "Toon Params Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType11;

        protected override void ReadCore( ResourceReader reader )
        {
            P11_0 = reader.ReadVector4();
            if ( Version >= 0x2108001 )
                P11_1 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( P11_0 );
            if ( Version >= 0x2108001 )
                writer.WriteSingle( P11_1 );
        }
        public override void SetShaderFlags( Material mat )
        {

        }
        //public override bool IsMaterialTransparent( ) => true;
    }

    public sealed class MaterialParameterSetType12 : MaterialParameterSetBase
    {
        [ShaderUniform( "baseColor" )]
        public Vector4 P12_0 { get; set; } // 0x90
        [ShaderUniform( "edgeColor" )]
        public Vector4 P12_1 {get; set;} // 0xb0
        [ShaderUniform( "emissiveColor" )]
        public Vector4 P12_2 {get; set;} // 0xc0
        [ShaderUniform( "metallic" )]
        public float P12_3 {get; set;} // 0xe8
        [ShaderUniform( "edgeThreshold" )]
        public float P12_4 {get; set;} // 0xf4
        [ShaderUniform( "edgeFactor" )]
        public float P12_5 {get; set;} // 0xf8
        public Type12Flags Flags {get; set;} // 0x12c
        public float P12_7 {get; set;} // 0x108
        public Vector3 P12_8 {get; set;} // 0x110
        [ShaderUniform( "matBloomIntensity" )]
        public float P12_9 {get; set;} // 0xf0
        [ShaderUniform( "edgeRemoveYAxisFactor" )]
        public float P12_10 {get; set;} // 0xfc
        public float P12_11 { get; set; } // 0x11c
        public float P12_12 { get; set; } // 0x120
        public float P12_13 { get; set; } // 0x124
        [ShaderUniform( "matBloomIntensity" )]
        public float P12_14 { get; set; } // 0x10c
        [ShaderUniform( "specularColor" )]
        public Vector3 P12_15 { get; set; } // 0xd0
        [ShaderUniform( "specularThreshold" )]
        public float P12_16 { get; set; } // 0xdc
        [ShaderUniform( "specularPower" )]
        public float P12_17 { get; set; } // 0xe0
        [ShaderUniform( "matRoughness" )]
        public float P12_18 { get; set; } // 0xec
        [ShaderUniform( "matRampAlpha" )]
        public float P12_19 { get; set; } // 0xe4
        [ShaderUniform( "shadowColor" )]
        public Vector4 P12_20 { get; set; } // 0xa0
        [ShaderUniform( "shadowThreshold" )]
        public float P12_21 { get; set; } // 0x100
        [ShaderUniform( "shadowFactor" )]
        public float P12_22 { get; set; } // 0x104

        [Flags]
        public enum Type12Flags : uint
        {
            FLAG0 = 0x00000001,
            FLAG1 = 0x00000002,
            FLAG2 = 0x00000004,
            FLAG3 = 0x00000008,
            FLAG4 = 0x00000010,
            FLAG5 = 0x00000020,
            FLAG6 = 0x00000040,
            FLAG7 = 0x00000080,
            FLAG8 = 0x00000100,
            FLAG9 = 0x00000200,
            FLAG10 = 0x00000400,
            FLAG11 = 0x00000800,
            FLAG12 = 0x00001000,
            FLAG13 = 0x00002000,
            FLAG14 = 0x00004000,
            FLAG15 = 0x00008000,
            FLAG16 = 0x00010000,
            FLAG17 = 0x00020000,
            FLAG18 = 0x00040000,
            FLAG19 = 0x00080000,
        }

        // 45.HLSL
        public override string GetParameterName() => "Parameter Type 12";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap3Name() => "Ramp Map";
        public override string GetTextureMap5Name() => "Multiply Map";
        public override string GetTextureMap6Name() => "Toon Params 2 Map";
        public override string GetTextureMap9Name() => "Toon Edge Color Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType12;

        protected override void ReadCore( ResourceReader reader )
        {
            P12_0 = reader.ReadVector4();
            P12_1 = reader.ReadVector4();
            P12_2 = reader.ReadVector4();
            P12_3 = reader.ReadSingle();
            P12_4 = reader.ReadSingle();
            P12_5 = reader.ReadSingle();
            Flags = (Type12Flags)reader.ReadUInt32();
            P12_7 = reader.ReadSingle();
            P12_8 = reader.ReadVector3();
            P12_9 = reader.ReadSingle();
            P12_10 = reader.ReadSingle();
            if ( Version >= 0x2109501 )
            {
                P12_11 = reader.ReadSingle();
                P12_12 = reader.ReadSingle();
                P12_13 = reader.ReadSingle();
            }
            if ( Version >= 0x2109601 )
                P12_14 = reader.ReadSingle();
            if ( Version >= 0x2109701 )
            {
                P12_15 = reader.ReadVector3();
                P12_16 = reader.ReadSingle();
                P12_17 = reader.ReadSingle();
                P12_18 = reader.ReadSingle();
                P12_19 = reader.ReadSingle(); 
            }
            if ( Version > 0x2110070 )
            {
                P12_20 = reader.ReadVector4();
                P12_21 = reader.ReadSingle();
                P12_22 = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P12_0);
            writer.WriteVector4(P12_1);
            writer.WriteVector4(P12_2);
            writer.WriteSingle(P12_3);
            writer.WriteSingle(P12_4);
            writer.WriteSingle(P12_5);
            writer.WriteUInt32((uint)Flags);
            writer.WriteSingle(P12_7);
            writer.WriteVector3(P12_8);
            writer.WriteSingle(P12_9);
            writer.WriteSingle( P12_10 );
            if ( Version >= 0x2109501 )
            {
                writer.WriteSingle(P12_11 );
                writer.WriteSingle(P12_12 );
                writer.WriteSingle( P12_13 );
            }
            if ( Version >= 0x2109601 )
                writer.WriteSingle( P12_14 );
            if ( Version >= 0x2109701 )
            {
                writer.WriteVector3(P12_15 );
                writer.WriteSingle(P12_16 );
                writer.WriteSingle(P12_17 );
                writer.WriteSingle(P12_18 );
                writer.WriteSingle( P12_19 );
            }
            if ( Version > 0x2110070 )
            {
                writer.WriteVector4(P12_20 );
                writer.WriteSingle(P12_21 );
                writer.WriteSingle( P12_22 );
            }
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( Flags.HasFlag( Type12Flags.FLAG0 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_TOON_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type12Flags.FLAG2 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_REFERENCE_NORMALMAP;
            if ( Flags.HasFlag( Type12Flags.FLAG6 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_EDGE_SEMITRANS;
            if ( Flags.HasFlag( Type12Flags.FLAG7 ) )
                mat.MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_EDGE_REMOVAL_LIGHT_YAXIS;
            /*
            if ( Flags.HasFlag( Type12Flags.FLAG9 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP; // FLAG2_RAMP_REFLIGHTDIRECTION
            */
            if ( Flags.HasFlag( Type12Flags.FLAG11 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_APPLY_PBR_LIGHT;
            if ( Flags.HasFlag( Type12Flags.FLAG12 ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SPECULAR_NORMALMAPALPHA; // FLAG2_FORCED_BLOOMINTENSITY
        }
        //public override bool IsMaterialTransparent( ) => true;
    }

    // 47.HLSL
    public sealed class MaterialParameterSetType14 : MaterialParameterSetBase
    {
        public Vector4 P14_0 { get; set; } // 0x90
        public uint P14_1 { get; set; } // 0xa0

        public override string GetParameterName() => "Parameter Type 14";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType14;

        protected override void ReadCore( ResourceReader reader )
        {
            P14_0 = reader.ReadVector4();
            P14_1 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4(P14_0);
            writer.WriteUInt32( P14_1 );
        }
        public override void SetShaderFlags( Material mat )
        {

        }
        //public override bool IsMaterialTransparent( ) => false;
    }

    // 49.HLSL
    public sealed class MaterialParameterSetType15 : MaterialParameterSetBase
    {
        public struct MaterialParameterSetType15Layer
        {
            public float Field0 { get; set; } // tileSize
            public float Field1 { get; set; }
            public float Field2 { get; set; } // tileOffset
            public float Field3 { get; set; }
            public float Field4 { get; set; } // roughness
            public float Field5 { get; set; } // metallic
            public Vector3 Field6 { get; set; } // color?
        }
        private MaterialParameterSetType15Layer[] mP15_0;
        public MaterialParameterSetType15Layer[] P15_0 // 0x90
        {
            get => mP15_0;
            set => mP15_0 = value;
        }
        public uint P15_LayerCount { get; set; } // 0x2d0
        public float P15_TriPlanarScale { get; set; } // 0x2d4
        public Type15Flags Flags { get; set; } // 0x2d8
        [Flags]
        public enum Type15Flags
        {
            TriplanarMapping = 0x00000001,
            GBufferSkyFlag = 0x00000002,
        }

        public override string GetParameterName() => "Parameter Layered";
        public override string GetTextureMap2Name() => "Normal Map";
        public override string GetTextureMap3Name() => "Blend Map";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType15;

        protected override void ReadCore( ResourceReader reader )
        {
            P15_0 = new MaterialParameterSetType15Layer[0x10];
            for ( int i = 0; i < 0x10; i++ )
            {
                P15_0[i].Field0 = reader.ReadSingle(); 
                P15_0[i].Field1 = reader.ReadSingle();
                P15_0[i].Field2 = reader.ReadSingle(); 
                P15_0[i].Field3 = reader.ReadSingle();
                P15_0[i].Field4 = reader.ReadSingle(); 
                P15_0[i].Field5 = reader.ReadSingle(); 
                P15_0[i].Field6 = reader.ReadVector3(); 
            }
            P15_LayerCount = reader.ReadUInt32();
            P15_TriPlanarScale = reader.ReadSingle();
            Flags = (Type15Flags)reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            for ( int i = 0; i < 0x10; i++ )
            {
                writer.WriteSingle(P15_0[i].Field0);
                writer.WriteSingle(P15_0[i].Field1);
                writer.WriteSingle(P15_0[i].Field2);
                writer.WriteSingle(P15_0[i].Field3);
                writer.WriteSingle(P15_0[i].Field4);
                writer.WriteSingle(P15_0[i].Field5);
                writer.WriteVector3( P15_0[i].Field6 );
            }
            writer.WriteUInt32(P15_LayerCount);
            writer.WriteSingle(P15_TriPlanarScale);
            writer.WriteUInt32( (uint)Flags );
        }
        public override void SetShaderFlags( Material mat )
        {
            if ( Flags.HasFlag( Type15Flags.TriplanarMapping ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_HDR_TONEMAP; // FLAG2_TRIPLANARMAPPING
            if ( Flags.HasFlag( Type15Flags.GBufferSkyFlag ) )
                mat.MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_SKY;
        }
        //public override bool IsMaterialTransparent( ) => false;
    }

    public sealed class MaterialParameterSetType16 : MaterialParameterSetBase
    {
        public override string GetParameterName() => "Parameter Type 16";

        public override ResourceType ResourceType => ResourceType.MaterialParameterSetType16;

        // Empty
        protected override void ReadCore( ResourceReader reader ) {}

        protected override void WriteCore( ResourceWriter writer ) {}
        public override void SetShaderFlags( Material mat ) {}
    }
}
