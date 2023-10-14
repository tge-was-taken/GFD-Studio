using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using GFDLibrary.Materials;
using GFDLibrary.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using YamlDotNet.Core.Tokens;

namespace GFDLibrary.Rendering.OpenGL
{
    public interface IGLMaterial : IDisposable
    {
        void Bind( GLShaderProgram shaderProgram );
    }

    public delegate GLTexture MaterialTextureCreator( Material material, string textureName );

    public class GLMaterial : IGLMaterial
    {
        public Vector4 Ambient { get; set; }

        public Vector4 Diffuse { get; set; }

        public Vector4 Specular { get; set; }

        public Vector4 Emissive { get; set; }
        public float Reflectivity { get; set; }
        public Vector4 ToonLightColor { get; set; } = new Vector4(0.98f, 0.98f, 0.98f, 0.36f);
        public float ToonLightThreshold { get; set; } = 0.7f;
        public float ToonLightFactor { get; set; } = 14.0f;
        public float ToonShadowBrightness { get; set; } = 0.5f;
        public float ToonShadowThreshold { get; set; } = 0.5f;
        public float ToonShadowFactor { get; set; } = 20.0f;

        public GLTexture DiffuseTexture { get; set; }
        public GLTexture NormalTexture { get; set; }
        public GLTexture SpecularTexture { get; set; }
        public GLTexture ReflectionTexture { get; set; }
        public GLTexture HighlightTexture { get; set; }
        public GLTexture GlowTexture { get; set; }
        public GLTexture NightTexture { get; set; }
        public GLTexture DetailTexture { get; set; }
        public GLTexture ShadowTexture { get; set; }


        public int DrawMethod { get; set; }
        public int HighlightMapBlendMode { get; set; }
        public int AlphaClip { get; set; }
        public int TexcoordsFlags { get; set; }

        public bool HasType0 { get; set; } = false;
        public bool HasType1 { get; set; } = false;
        public bool HasType4 { get; set; } = false;
        public int MatFlags { get; set; }
        public int Type0Flags { get; set; }

        public bool HasDiffuseTexture => DiffuseTexture != null;
        public bool HasNormalTexture => NormalTexture != null;
        public bool HasSpecularTexture => SpecularTexture != null;
        public bool HasReflectionTexture => ReflectionTexture != null;
        public bool HasHighlightTexture => HighlightTexture != null;
        public bool HasGlowTexture => GlowTexture != null;
        public bool HasNightTexture => NightTexture != null;
        public bool HasDetailTexture => DetailTexture != null;
        public bool HasShadowTexture => ShadowTexture != null;


        public bool RenderWireframe { get; set; }

        public bool EnableBackfaceCulling { get; set; }

        public GLMaterial( Material material, MaterialTextureCreator textureCreator )
        {
            // color parameters
            MatFlags = Convert.ToInt32( material.Flags );
            Ambient = material.AmbientColor.ToOpenTK();
            Diffuse = material.DiffuseColor.ToOpenTK();
            Specular = material.SpecularColor.ToOpenTK();
            Emissive = material.EmissiveColor.ToOpenTK();
            Reflectivity = material.Field40;
            DrawMethod = (int)material.DrawMethod;
            HighlightMapBlendMode = (int)material.Field4D;
            AlphaClip = (int)material.Field90;
            TexcoordsFlags = (int)( material.Field6C );
            EnableBackfaceCulling = !(Convert.ToBoolean( material.DisableBackfaceCulling ));

            if ( material.Attributes != null && material.Flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                HasType0 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type0 );
                HasType1 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type1 );
                HasType4 = material.Attributes.Any( x => x.AttributeType == MaterialAttributeType.Type4 );
            }

            if (HasType0)
            {
                MaterialAttributeType0 type0 = (MaterialAttributeType0)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type0 );
                ToonLightColor = type0.Color.ToOpenTK();
                ToonLightThreshold = type0.Field1C;
                ToonLightFactor = type0.Field20;
                ToonShadowBrightness = type0.Field24;
                ToonShadowThreshold = type0.Field28;
                ToonShadowFactor = type0.Field2C;
                Type0Flags = ((int)type0.Type0Flags);
            }
            if (HasType1)
            {
                MaterialAttributeType1 type1 = (MaterialAttributeType1)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type1 );
                ToonLightColor = type1.InnerGlow.ToOpenTK();
                ToonLightThreshold = type1.Field1C;
                ToonLightFactor = type1.Field20;
            }
            if (HasType4)
            {
                MaterialAttributeType4 type4 = (MaterialAttributeType4)material.Attributes.Single(
                    x => x.AttributeType == MaterialAttributeType.Type4 );
                ToonLightColor = type4.Field0C.ToOpenTK();
                ToonLightThreshold = type4.Field1C;
                ToonLightFactor = type4.Field20;
            }

            // texture
            if ( material.DiffuseMap != null )
            {
                DiffuseTexture = textureCreator( material, material.DiffuseMap.Name );
            }
            if ( material.NormalMap != null )
            {
                NormalTexture = textureCreator( material, material.NormalMap.Name );
            }
            if ( material.SpecularMap != null )
            {
                SpecularTexture = textureCreator( material, material.SpecularMap.Name );
            }
            if ( material.ReflectionMap != null )
            {
                ReflectionTexture = textureCreator( material, material.ReflectionMap.Name );
            }
            if ( material.HighlightMap != null )
            {
                HighlightTexture = textureCreator( material, material.HighlightMap.Name );
            }
            if ( material.GlowMap != null )
            {
                GlowTexture = textureCreator( material, material.GlowMap.Name );
            }
            if ( material.NightMap != null )
            {
                NightTexture = textureCreator( material, material.NightMap.Name );
            }
            if ( material.DetailMap != null )
            {
                DetailTexture = textureCreator( material, material.DetailMap.Name );
            }
            if ( material.ShadowMap != null )
            {
                ShadowTexture = textureCreator( material, material.ShadowMap.Name );
            }
        }

        public GLMaterial()
        {       
        }

        public void Bind( GLShaderProgram shaderProgram )
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
            shaderProgram.SetUniform( "uMatAmbient",              Ambient );
            shaderProgram.SetUniform( "uMatDiffuse",              Diffuse );
            shaderProgram.SetUniform( "uMatSpecular",             Specular );
            shaderProgram.SetUniform( "uMatEmissive",             Emissive );
            shaderProgram.SetUniform( "uMatReflectivity",         Reflectivity );
            shaderProgram.SetUniform( "DrawMethod",               DrawMethod );
            shaderProgram.SetUniform( "HighlightMapBlendMode",    HighlightMapBlendMode );
            shaderProgram.SetUniform( "alphaClip",                AlphaClip );
            shaderProgram.SetUniform( "TexcoordFlags",            TexcoordsFlags );
            shaderProgram.SetUniform( "uMatHasType0",             HasType0 );
            shaderProgram.SetUniform( "uMatHasType1",             HasType1 );
            shaderProgram.SetUniform( "uMatHasType4",             HasType4 );
            shaderProgram.SetUniform( "uMatFlags",                MatFlags );
            shaderProgram.SetUniform( "uMatType0Flags",           Type0Flags );
            shaderProgram.SetUniform( "uMatToonLightColor",       ToonLightColor );
            shaderProgram.SetUniform( "uMatToonLightFactor",      ToonLightFactor );
            shaderProgram.SetUniform( "uMatToonLightThreshold",   ToonLightThreshold );
            shaderProgram.SetUniform( "uMatToonShadowBrightness", ToonShadowBrightness );
            shaderProgram.SetUniform( "uMatToonShadowThreshold",  ToonShadowThreshold );
            shaderProgram.SetUniform( "uMatToonShadowFactor",     ToonShadowFactor );

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
    }
}