using System;
using GFDLibrary.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

        public bool HasAlphaTransparency { get; set; }

        public bool HasDiffuseTexture => DiffuseTexture != null;

        public GLMaterial( Material material, MaterialTextureCreator textureCreator )
        {
            // color parameters
            Ambient = material.Ambient.Convert();
            Diffuse = material.Diffuse.Convert();
            Specular = material.Specular.Convert();
            Emissive = material.Emissive.Convert();

            // texture
            if ( material.DiffuseMap != null )
            {
                DiffuseTexture = textureCreator( material, material.DiffuseMap.Name );
                HasAlphaTransparency = material.DrawOrder != MaterialDrawOrder.Front;
            }
        }

        public GLMaterial()
        {       
        }

        public void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "matHasDiffuse", HasDiffuseTexture );

            if ( HasDiffuseTexture )
                DiffuseTexture.Bind();

            shaderProgram.SetUniform( "matAmbient",              Ambient );
            shaderProgram.SetUniform( "matDiffuse",              Diffuse );
            shaderProgram.SetUniform( "matSpecular",             Specular );
            shaderProgram.SetUniform( "matEmissive",             Emissive );
            shaderProgram.SetUniform( "matHasAlphaTransparency", HasAlphaTransparency );
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