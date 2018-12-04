using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLShaderProgramBuilder : IDisposable
    {
        private int mId;
        private readonly List<int> mAttachedShaders;
        private bool mDisposed;
        private bool mHasBuilt;

        public GLShaderProgramBuilder()
        {
            mId = GL.CreateProgram();
            mAttachedShaders = new List<int>();
        }

        ~GLShaderProgramBuilder()
        {
            Dispose( false );
        }

        public bool TryAttachShader( ShaderType shaderType, string shaderSource )
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
                Trace.TraceError( $"{shaderType} compilation failed" );

                GL.GetShader( shader, ShaderParameter.InfoLogLength, out int infoLogLength );

                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( shader, infoLogLength + 1, out int length, out var shaderInfoLog );

                    Trace.TraceError( shaderInfoLog );
                    Trace.TraceError( "" );
                }

                return false;
            }
            else
            {
                GL.GetShader( shader, ShaderParameter.InfoLogLength, out int infoLogLength );

                if ( infoLogLength > 0 )
                {
                    GL.GetShaderInfoLog( shader, infoLogLength + 1, out int length, out var shaderInfoLog );

                    Trace.TraceInformation( $"{shaderType} info log:" );
                    Trace.TraceInformation( shaderInfoLog );
                    Trace.TraceInformation( "" );
                }
            }

            // attach it to the program
            GL.AttachShader( mId, shader );

            // keep it here to delete it later
            mAttachedShaders.Add( shader );

            return true;
        }

        public bool TryBuild( out int id )
        {
            if ( mHasBuilt )
            {
                Trace.TraceError( "Attempted to build shader program twice" );

                id = 0;
                return false;
            }

            // link program
            GL.LinkProgram( mId );

            // check for linking errors
            GL.GetProgram( mId, GetProgramParameterName.LinkStatus, out int linkStatus );

            if ( linkStatus == 0 )
            {
                Trace.TraceError( "Shader program linking failed" );

                // print info log
                GL.GetProgram( mId, GetProgramParameterName.InfoLogLength, out int infoLogLength );
                if ( infoLogLength > 0 )
                {
                    GL.GetProgramInfoLog( mId, infoLogLength + 1, out int length, out var log );

                    Trace.TraceError( log );
                    Trace.TraceError( "" );
                }

                id = 0;
                return false;
            }
            else
            {
                // print info log
                GL.GetProgram( mId, GetProgramParameterName.InfoLogLength, out int infoLogLength );
                if ( infoLogLength > 0 )
                {
                    GL.GetProgramInfoLog( mId, infoLogLength + 1, out int length, out var log );

                    Trace.TraceInformation( $"Shader program info log:" );
                    Trace.TraceInformation( log );
                    Trace.TraceInformation( "" );
                }
            }

            DeleteAttachedShaders();

            id = mId;
            mHasBuilt = true;

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
                if ( !mHasBuilt )
                {
                    DeleteAttachedShaders();
                    DeleteProgram();
                }

                mDisposed = true;
            }
        }

        private void DeleteAttachedShaders()
        {
            foreach ( var shader in mAttachedShaders )
            {
                GL.DeleteShader( shader );
            }

            mAttachedShaders.Clear();
        }

        private void DeleteProgram()
        {
            if ( mId != 0 )
            {
                GL.DeleteProgram( mId );
                mId = 0;
            }
        }
    }
}
