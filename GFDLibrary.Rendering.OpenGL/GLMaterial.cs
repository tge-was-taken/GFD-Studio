using System;
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

        public GLTexture DiffuseTexture { get; set; }
        public GLTexture SpecularTexture { get; set; }
        public GLTexture ShadowTexture { get; set; }


        public int DrawMethod { get; set; }

        public bool HasType0 { get; set; }


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

            //HasType0 = ( material.Flags.HasFlag(MaterialFlags.HasAttributes) & materialAttribute.AttributeType == 0 );

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