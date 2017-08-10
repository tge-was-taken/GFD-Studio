using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace AtlusGfdEditor.GUI.Controls.ModelView
{
    public class GLShaderProgramBuilder : IDisposable
    {
        private int mGLShaderProgram;
        private List<int> mAttachedShaders;
        private bool mDisposed;

        public GLShaderProgramBuilder()
        {
            mGLShaderProgram = GL.CreateProgram();
            mAttachedShaders = new List<int>();
        }

        ~GLShaderProgramBuilder()
        {
            Dispose( false );
        }

        public bool AttachShader( ShaderType shaderType, string shaderSource )
        {
            if ( shaderSource == null )
            {
                throw new ArgumentNullException( nameof( shaderSource ) );
            }

            // create shader and compile it
            int shader = GL.CreateShader( shaderType );
            GL.ShaderSource( shader, shaderSource );
            GL.CompileShader( shader );

            // check for compile errors
            GL.GetShader( shader, ShaderParameter.CompileStatus, out int compileStatus );
            if ( compileStatus == 0 )
            {
                Console.WriteLine( $"{shaderType} compilation failed" );

                GL.GetShader( shader, ShaderParameter.InfoLogLength, out int infoLogLength );

                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( shader, infoLogLength + 1, out int length, out var shaderInfoLog );

                    Console.WriteLine( shaderInfoLog );
                    Console.WriteLine();
                }

                return false;
            }
            else
            {
                GL.GetShader( shader, ShaderParameter.InfoLogLength, out int infoLogLength );

                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( shader, infoLogLength + 1, out int length, out var shaderInfoLog );

                    Console.WriteLine( $"{shaderType} info log:" );
                    Console.WriteLine( shaderInfoLog );
                    Console.WriteLine();
                }
            }

            // attach it to the program
            GL.AttachShader( mGLShaderProgram, shader );

            // keep it here to delete it later
            mAttachedShaders.Add( shader );

            return true;
        }

        public bool Build( out GLShaderProgram program )
        {
            GL.LinkProgram( mGLShaderProgram );

            // check for linking errors
            GL.GetProgram( mGLShaderProgram, GetProgramParameterName.LinkStatus, out int linkStatus );

            if ( linkStatus == 0 )
            {
                Console.WriteLine( "Shader program linking failed" );

                GL.GetProgram( mGLShaderProgram, GetProgramParameterName.InfoLogLength, out int infoLogLength );
                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( mGLShaderProgram, infoLogLength + 1, out int length, out var log );

                    Console.WriteLine( log );
                    Console.WriteLine();
                }

                program = null;
                return false;
            }
            else
            {
                GL.GetProgram( mGLShaderProgram, GetProgramParameterName.InfoLogLength, out int infoLogLength );
                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( mGLShaderProgram, infoLogLength + 1, out int length, out var log );

                    Console.WriteLine( $"Shader program info log:" );
                    Console.WriteLine( log );
                    Console.WriteLine();
                }
            }

            // dispose to delete shaders
            Dispose();

            program = new GLShaderProgram( mGLShaderProgram );
            return true;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                }

                foreach ( var shader in mAttachedShaders )
                {
                    GL.DeleteShader( shader );
                }

                mDisposed = true;
            }
        }
    }
}
