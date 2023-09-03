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
        public Vector4 ToonLightColor { get; set; } = new Vector4(0.98f, 0.98f, 0.98f, 0.36f);
        public float ToonLightThreshold { get; set; } = 0.7f;
        public float ToonLightFactor { get; set; } = 14.0f;
        public float ToonShadowBrightness { get; set; } = 0.5f;
        public float ToonShadowThreshold { get; set; } = 0.5f;
        public float ToonShadowFactor { get; set; } = 20.0f;

        public GLTexture DiffuseTexture { get; set; }
        public GLTexture SpecularTexture { get; set; }
        public GLTexture ShadowTexture { get; set; }


        public int DrawMethod { get; set; }

        public bool HasType0 { get; set; } = false;
        public uint Type0Flags { get; set; }

        public bool HasDiffuseTexture => DiffuseTexture != null;
        public bool HasSpecularTexture => SpecularTexture != null;
        public bool HasShadowTexture => ShadowTexture != null;


        public bool RenderWireframe { get; set; }

        public bool EnableBackfaceCulling { get; set; } = true;

        public GLMaterial( Material material, MaterialTextureCreator textureCreator )
        {
            // color parameters
            Ambient = material.AmbientColor.ToOpenTK();
            Diffuse = material.DiffuseColor.ToOpenTK();
            Specular = material.SpecularColor.ToOpenTK();
            Emissive = material.EmissiveColor.ToOpenTK();

            HasType0 = ( material.Attributes != null && material.Flags.HasFlag(MaterialFlags.HasAttributes) 
                & material.Attributes.Any(x => x.AttributeType == MaterialAttributeType.Type0));

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
                Type0Flags = type0.RawFlags;
            }

            // texture
            if ( material.DiffuseMap != null )
            {
                DiffuseTexture = textureCreator( material, material.DiffuseMap.Name );
                DrawMethod = (int)material.DrawMethod;
            }
            if ( material.SpecularMap != null )
            {
                SpecularTexture = textureCreator( material, material.SpecularMap.Name );
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
            shaderProgram.SetUniform( "uMatHasDiffuse",  HasDiffuseTexture );
            shaderProgram.SetUniform( "uMatHasSpecular", HasSpecularTexture );
            shaderProgram.SetUniform( "uMatHasShadow",   HasShadowTexture );
            shaderProgram.SetUniform( "uDiffuse", 0 );
            shaderProgram.SetUniform( "uSpecular", 1 );
            shaderProgram.SetUniform( "uShadow", 2 );
            if ( HasDiffuseTexture )
            //DiffuseTexture.Bind();
            {
                GL.ActiveTexture( TextureUnit.Texture0 );
                DiffuseTexture.Bind();
            }
            if ( HasSpecularTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture1 );
                SpecularTexture.Bind();
            }
            if ( HasShadowTexture )
            {
                GL.ActiveTexture( TextureUnit.Texture2 );
                ShadowTexture.Bind();
            }
            shaderProgram.SetUniform( "uMatAmbient",              Ambient );
            shaderProgram.SetUniform( "uMatDiffuse",              Diffuse );
            shaderProgram.SetUniform( "uMatSpecular",             Specular );
            shaderProgram.SetUniform( "uMatEmissive",             Emissive );
            shaderProgram.SetUniform( "DrawMethod", DrawMethod );
            shaderProgram.SetUniform( "uMatHasType0", HasType0 );
            shaderProgram.SetUniform( "uMatType0Flags", Type0Flags );
            shaderProgram.SetUniform( "uMatToonLightColor", ToonLightColor );
            shaderProgram.SetUniform( "uMatToonLightFactor", ToonLightFactor );
            shaderProgram.SetUniform( "uMatToonLightThreshold", ToonLightThreshold );
            shaderProgram.SetUniform( "uMatToonShadowBrightness", ToonShadowBrightness );
            shaderProgram.SetUniform( "uMatToonShadowThreshold", ToonShadowThreshold );
            shaderProgram.SetUniform( "uMatToonShadowFactor", ToonShadowFactor );

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
                    SpecularTexture?.Dispose();
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