using System;
using System.Linq;
using System.Reflection;
using GFDLibrary.Materials;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace GFDLibrary.Rendering.OpenGL
{
    public interface IGLMaterial : IDisposable
    {
        void Bind( GLShaderProgram shaderProgram );
        void Unbind( GLShaderProgram shaderProgram );
    }

    public delegate GLTexture MaterialTextureCreator( Material material, string textureName );

    public abstract class GLBaseMaterial : IGLMaterial
    {
        public GLTexture DiffuseTexture { get; set; }
        public GLTexture NormalTexture { get; set; }
        public GLTexture SpecularTexture { get; set; }
        public GLTexture ReflectionTexture { get; set; }
        public GLTexture HighlightTexture { get; set; }
        public GLTexture GlowTexture { get; set; }
        public GLTexture NightTexture { get; set; }
        public GLTexture DetailTexture { get; set; }
        public GLTexture ShadowTexture { get; set; }
        public GLTexture ReserveMap { get; set; }
        public int DrawMethod { get; set; }
        public int MultiBlendMode { get; set; }
        public int AlphaClip { get; set; }
        public int AlphaClipMode { get; set; }
        public int TexcoordsFlags { get; set; }
        public int MatFlags { get; set; }
        public int MatFlags2 { get; set; }
        public bool HasDiffuseTexture => DiffuseTexture != null;
        public bool HasNormalTexture => NormalTexture != null;
        public bool HasSpecularTexture => SpecularTexture != null;
        public bool HasReflectionTexture => ReflectionTexture != null;
        public bool HasHighlightTexture => HighlightTexture != null;
        public bool HasGlowTexture => GlowTexture != null;
        public bool HasNightTexture => NightTexture != null;
        public bool HasDetailTexture => DetailTexture != null;
        public bool HasShadowTexture => ShadowTexture != null;
        public bool HasTexture10 => ReserveMap != null;
        public bool RenderWireframe { get; set; }
        public bool EnableBackfaceCulling { get; set; }

        // Set per frame from the material animation currently playing.
        public float AnimatedAlpha { get; set; } = 1.0f;
        public System.Numerics.Vector2 UVOffset { get; set; }
        public System.Numerics.Vector2 UVScale { get; set; } = System.Numerics.Vector2.One;
        public float UVRotation { get; set; }
        public System.Numerics.Vector2 UV1Offset { get; set; }
        public System.Numerics.Vector2 UV1Scale { get; set; } = System.Numerics.Vector2.One;
        public float UV1Rotation { get; set; }

        protected void BindUVTransforms( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "uUVTransform", new Vector4( UVOffset.X, UVOffset.Y, UVScale.X, UVScale.Y ) );
            shaderProgram.SetUniform( "uUVRotation", UVRotation );
            shaderProgram.SetUniform( "uUV1Transform", new Vector4( UV1Offset.X, UV1Offset.Y, UV1Scale.X, UV1Scale.Y ) );
            shaderProgram.SetUniform( "uUV1Rotation", UV1Rotation );
        }

        public GLBaseMaterial()
        {
        }

        public GLBaseMaterial( Material material, MaterialTextureCreator textureCreator )
        {
            MatFlags = Convert.ToInt32( material.Flags );
            DrawMethod = (int)material.DrawMethod;
            MultiBlendMode = (int)material.MultiBlendMode;
            AlphaClip = (int)material.AlphaClip;
            AlphaClipMode = (int)material.AlphaClipMode;
            MatFlags2 = Convert.ToInt32( material.Flags2 );
            TexcoordsFlags = (int)( material.TexCoordFlags0 );
            EnableBackfaceCulling = !( Convert.ToBoolean( material.Culling ) );

            if ( material.DiffuseMap != null )
            {
                DiffuseTexture = textureCreator( material, material.DiffuseMap.Name );
            }
            if ( material.NormalMap != null )
            {
                NormalTexture = textureCreator( material, material.NormalMap.Name );
            }
            if ( material.GlossMap != null )
            {
                SpecularTexture = textureCreator( material, material.GlossMap.Name );
            }
            if ( material.ReflectionMap != null )
            {
                ReflectionTexture = textureCreator( material, material.ReflectionMap.Name );
            }
            if ( material.MultiMap != null )
            {
                HighlightTexture = textureCreator( material, material.MultiMap.Name );
            }
            if ( material.GlowMap != null )
            {
                GlowTexture = textureCreator( material, material.GlowMap.Name );
            }
            if ( material.DarkMap != null )
            {
                NightTexture = textureCreator( material, material.DarkMap.Name );
            }
            if ( material.DetailMap != null )
            {
                DetailTexture = textureCreator( material, material.DetailMap.Name );
            }
            if ( material.LightMap != null )
            {
                ShadowTexture = textureCreator( material, material.LightMap.Name );
            }
            if ( material.ReserveMap != null )
            {
                ReserveMap = textureCreator( material, material.ReserveMap.Name );
            }
        }

        public virtual void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "uDiffuse", 0 );
            shaderProgram.SetUniform( "uNormal", 1 );
            shaderProgram.SetUniform( "uSpecular", 2 );
            shaderProgram.SetUniform( "uReflection", 3 );
            shaderProgram.SetUniform( "uHighlight", 4 );
            shaderProgram.SetUniform( "uGlow", 5 );
            shaderProgram.SetUniform( "uNight", 6 );
            shaderProgram.SetUniform( "uDetail", 7 );
            shaderProgram.SetUniform( "uShadow", 8 );
            shaderProgram.SetUniform( "uTex10", 9 );
            shaderProgram.SetUniform( "uMatHasDiffuse", HasDiffuseTexture );

            shaderProgram.SetUniform( "DrawMethod", DrawMethod );
            shaderProgram.SetUniform( "MultiBlendMode", MultiBlendMode );
            shaderProgram.SetUniform( "alphaClip", AlphaClip );
            shaderProgram.SetUniform( "alphaClipMode", AlphaClipMode );
            shaderProgram.SetUniform( "TexcoordFlags", TexcoordsFlags );
            shaderProgram.SetUniform( "uMatFlags", MatFlags );
            shaderProgram.SetUniform( "uMatFlags2", MatFlags2 );
            if ( HasDiffuseTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture0 );
                DiffuseTexture.Bind();
            }
            if ( HasNormalTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture1 );
                NormalTexture.Bind();
            }
            if ( HasSpecularTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture2 );
                SpecularTexture.Bind();
            }
            if ( HasReflectionTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture3 );
                ReflectionTexture.Bind();
            }
            if ( HasHighlightTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture4 );
                HighlightTexture.Bind();
            }
            if ( HasGlowTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture5 );
                GlowTexture.Bind();
            }
            if ( HasNightTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture6 );
                NightTexture.Bind();
            }
            if ( HasDetailTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture7 );
                DetailTexture.Bind();
            }
            if ( HasShadowTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture8 );
                ShadowTexture.Bind();
            }
            if ( HasTexture10 )
            {
                GL.ActiveTexture( TextureUnit.Texture9 );
                ReserveMap.Bind();
            }
            if ( RenderWireframe )
            {
                GL.PolygonMode( MaterialFace.FrontAndBack, PolygonMode.Line );
            }

            if ( !EnableBackfaceCulling )
            {
                GL.Disable( EnableCap.CullFace );
            }
        }
        public void Unbind( GLShaderProgram shaderProgram )
        {
            if ( !EnableBackfaceCulling )
            {
                GL.Enable( EnableCap.CullFace );
            }

            if ( RenderWireframe )
            {
                GL.PolygonMode( MaterialFace.FrontAndBack, PolygonMode.Fill );
            }
        }

        #region IDisposable Support
        private bool mDisposed; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    DiffuseTexture?.Dispose();
                    NormalTexture?.Dispose();
                    SpecularTexture?.Dispose();
                    ReflectionTexture?.Dispose();
                    HighlightTexture?.Dispose();
                    GlowTexture?.Dispose();
                    NightTexture?.Dispose();
                    DetailTexture?.Dispose();
                    ShadowTexture?.Dispose();
                }

                mDisposed = true;
            }
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose( true );
        }
        #endregion

        public static GLBaseMaterial CreateGLMaterial( Material material, MaterialTextureCreator textureCreator ) =>
            material.METAPHOR_MaterialParameterSet != null ? new GLMetaphorMaterial( material, textureCreator ) : new GLP5Material( material, textureCreator );
        public abstract bool IsMaterialTransparent();
    }

    public class GLP5Material : GLBaseMaterial
    {
        public Vector4 Ambient { get; set; }

        public Vector4 Diffuse { get; set; }

        public Vector4 Specular { get; set; }

        public Vector4 Emissive { get; set; }
        public float Reflectivity { get; set; }
        public Vector4 ToonLightColor { get; set; } = new Vector4( 0.98f, 0.98f, 0.98f, 0.36f );
        public float ToonLightThreshold { get; set; } = 0.7f;
        public float ToonLightFactor { get; set; } = 14.0f;
        public float ToonShadowBrightness { get; set; } = 0.5f;
        public float ToonShadowThreshold { get; set; } = 0.5f;
        public float ToonShadowFactor { get; set; } = 20.0f;
        public bool HasType0 { get; set; } = false;
        public bool HasType1 { get; set; } = false;
        public bool HasType4 { get; set; } = false;
        public int Type0Flags { get; set; }
        public bool HasType8 { get; set; } = false;

        public GLP5Material()
        {
        }

        public GLP5Material( Material material, MaterialTextureCreator textureCreator ) : base (material, textureCreator)
        {
            Ambient = material.LegacyParameters.AmbientColor.ToOpenTK();
            Diffuse = material.LegacyParameters.DiffuseColor.ToOpenTK();
            Specular = material.LegacyParameters.SpecularColor.ToOpenTK();
            Emissive = material.LegacyParameters.EmissiveColor.ToOpenTK();
            Reflectivity = material.LegacyParameters.Reflectivity;


            if ( material.Attributes != null && material.Flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                HasType0 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type0 );
                HasType1 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type1 );
                HasType4 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type4 );
            }

            if ( HasType0 )
            {
                MaterialAttributeType0 type0 = (MaterialAttributeType0)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type0 );
                ToonLightColor = type0.Color.ToOpenTK();
                ToonLightThreshold = type0.LightThreshold;
                ToonLightFactor = type0.LightFactor;
                ToonShadowBrightness = type0.ShadowBrightness;
                ToonShadowThreshold = type0.ShadowThreshold;
                ToonShadowFactor = type0.ShadowFactor;
                Type0Flags = ( (int)type0.Type0Flags );
            }
            if ( HasType1 )
            {
                MaterialAttributeType1 type1 = (MaterialAttributeType1)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type1 );
                ToonLightColor = type1.LightColor.ToOpenTK();
                ToonLightThreshold = type1.LightThreshold;
                ToonLightFactor = type1.LightFactor;
            }
            if ( HasType4 )
            {
                MaterialAttributeType4 type4 = (MaterialAttributeType4)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type4 );
                ToonLightColor = type4.LightColor.ToOpenTK();
                ToonLightThreshold = type4.LightThreshold;
                ToonLightFactor = type4.LightFactor;
            }

            // Catherine Full Body puts its toon lighting in a type 8 attribute, which the
            // Persona path can draw once it is told where to read it from.
            if ( ResourceVersion.TreatAsCatherineFullBody && material.Attributes != null )
            {
                HasType8 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type8 );

                if ( HasType8 )
                {
                    HasType1 = true;
                    MaterialAttributeType8 type8 = (MaterialAttributeType8)material.Attributes.Single(
                        x => x.AttributeType == MaterialAttributeType.Type8 );
                    ToonLightColor = new Vector4( type8.Field00.X, type8.Field00.Y, type8.Field00.Z, type8.Field0C );
                    ToonLightThreshold = type8.Field10;
                    ToonLightFactor = type8.Field14;
                }
            }
        }

        public override void Bind( GLShaderProgram shaderProgram )
        {
            base.Bind( shaderProgram );
            shaderProgram.SetUniform( "uMatAmbient", Ambient );

            var animatedDiffuse = Diffuse;
            animatedDiffuse.W *= AnimatedAlpha;
            shaderProgram.SetUniform( "uMatDiffuse", animatedDiffuse );
            shaderProgram.SetUniform( "uMatSpecular", Specular );
            shaderProgram.SetUniform( "uMatEmissive", Emissive );
            shaderProgram.SetUniform( "uMatReflectivity", Reflectivity );
            shaderProgram.SetUniform( "uMatHasType0", HasType0 );
            shaderProgram.SetUniform( "uMatHasType1", HasType1 );
            shaderProgram.SetUniform( "uMatHasType4", HasType4 );
            shaderProgram.SetUniform( "uMatHasType8", HasType8 );
            shaderProgram.SetUniform( "uMatType0Flags", Type0Flags );
            shaderProgram.SetUniform( "uMatToonLightColor", ToonLightColor );
            shaderProgram.SetUniform( "uMatToonLightFactor", ToonLightFactor );
            shaderProgram.SetUniform( "uMatToonLightThreshold", ToonLightThreshold );
            shaderProgram.SetUniform( "uMatToonShadowBrightness", ToonShadowBrightness );
            shaderProgram.SetUniform( "uMatToonShadowThreshold", ToonShadowThreshold );
            shaderProgram.SetUniform( "uMatToonShadowFactor", ToonShadowFactor );
            BindUVTransforms( shaderProgram );
        }
        public override bool IsMaterialTransparent() => DrawMethod != 0 || ( DrawMethod == 0 && Diffuse.W * AnimatedAlpha < 1.0 );
    }

    public class GLMetaphorMaterial : GLBaseMaterial
    {
        public MaterialParameterSetBase ParameterSet { get; private set; }
        public GLMetaphorMaterial() { }
        public GLMetaphorMaterial( Material material, MaterialTextureCreator textureCreator ) : base( material, textureCreator )
        { ParameterSet = material.METAPHOR_MaterialParameterSet; }

        public override void Bind( GLShaderProgram shaderProgram )
        {
            base.Bind( shaderProgram );
            foreach ( var prop in ParameterSet.GetType().GetProperties().Where( x => x.GetCustomAttribute<ShaderUniformAttribute>() != null ) )
            {
                if ( prop.PropertyType == typeof( System.Numerics.Vector3 ))
                {
                    shaderProgram.SetUniform( prop.GetCustomAttribute<ShaderUniformAttribute>().Uniform, ((System.Numerics.Vector3)prop.GetValue( ParameterSet )).ToOpenTK() );
                } else if ( prop.PropertyType == typeof( System.Numerics.Vector4 ) )
                {
                    shaderProgram.SetUniform( prop.GetCustomAttribute<ShaderUniformAttribute>().Uniform, ( (System.Numerics.Vector4)prop.GetValue( ParameterSet ) ).ToOpenTK() );
                } else if ( prop.PropertyType == typeof( float ) )
                {
                    shaderProgram.SetUniform( prop.GetCustomAttribute<ShaderUniformAttribute>().Uniform, (float)prop.GetValue( ParameterSet ) );
                } else if ( prop.PropertyType == typeof( int) )
                {
                    shaderProgram.SetUniform( prop.GetCustomAttribute<ShaderUniformAttribute>().Uniform, (int)prop.GetValue( ParameterSet ) );
                }
            }

            BindUVTransforms( shaderProgram );
        }
        public override bool IsMaterialTransparent() => ParameterSet.IsMaterialTransparent();
    }
}