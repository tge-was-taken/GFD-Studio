using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLShaderProgram : IDisposable
    {
        public static bool TryCreate( string vertexShaderFilepath, string fragmentShaderFilepath, out GLShaderProgram shaderProgram )
        {
            return TryCreate( File.OpenRead( vertexShaderFilepath ), File.OpenRead( fragmentShaderFilepath ), out shaderProgram );
        }

        public static GLShaderProgram TryThrowOnFail( string vertexshaderFilepath, string fragmentShaderFilepath )
        {
            GLShaderProgram ShaderOut;
            if (!TryCreate(vertexshaderFilepath, fragmentShaderFilepath, out ShaderOut))
                throw new Exception( $"Failed to build shader program: VS \"{vertexshaderFilepath}\" PS \"{fragmentShaderFilepath}\"" );
            return ShaderOut;
        }

        public static bool TryCreate( Stream vertexShaderStream, Stream fragmentShaderStream, out GLShaderProgram shaderProgram )
        {
            var vertexShaderSource = new StreamReader( vertexShaderStream ).ReadToEnd();
            var fragmentShaderSource = new StreamReader( fragmentShaderStream ).ReadToEnd();

            using ( var builder = new GLShaderProgramBuilder() )
            {
                if ( !builder.TryAttachShader( ShaderType.VertexShader, vertexShaderSource ) )
                {
                    shaderProgram = null;
                    return false;
                }

                if ( !builder.TryAttachShader( ShaderType.FragmentShader, fragmentShaderSource ) )
                {
                    shaderProgram = null;
                    return false;
                }

                if ( !builder.TryBuild( out var id ) )
                {
                    shaderProgram = null;
                    return false;
                }

                shaderProgram = new GLShaderProgram( id );
                return true;
            }
        }

        private class Uniform
        {
            public string Name;
            public Type Type;
            public int Location;
            public bool IsAssigned;
        }

        private bool mDisposed;
        private readonly Dictionary<string, Uniform> mUniforms;

        public int Id { get; }

        public GLShaderProgram( int id )
        {
            Id = id;
            mUniforms = new Dictionary<string, Uniform>();
        }

        public void RegisterUniform<T>( string name )
        {
            RegisterUniform( name, typeof( T ) );
        }

        public void RegisterUniform( string name, Type type )
        {
            // check if uniform exists
            if ( mUniforms.ContainsKey( name ) )
            {
                throw new Exception( $"Uniform \"{name}\" already registered" );
            }

            // get the location of the uniform isn't invalid
            int location = GL.GetUniformLocation( Id, name );
            if ( location == -1 )
            {
                Trace.TraceWarning( $"Attempted to register uniform \"{name}\" which does not exist in the shader program" );
            }

            // register uniform in dict
            mUniforms[name] = new Uniform()
            {
                Name     = name,
                Type     = type,
                Location = location,
            };
        }

        public void SetUniform( string name, Vector3 value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            GL.Uniform3( uniform.Location, value );
        }

        public void SetUniform( string name, Vector4 value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            GL.Uniform4( uniform.Location, value );
        }

        public void SetUniform( string name, Matrix4 value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            GL.UniformMatrix4( uniform.Location, false, ref value );
        }

        public unsafe void SetUniform( string name, Matrix4[] value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            fixed ( Matrix4* pValue = value )
                GL.UniformMatrix4( uniform.Location, value.Length, false, ( float* ) pValue );
        }

        public void SetUniform( string name, int value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );
            GL.Uniform1( uniform.Location, value );
        }

        public void SetUniform( string name, float value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );
            GL.Uniform1( uniform.Location, value );
        }

        public void SetUniform( string name, bool value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            GL.Uniform1( uniform.Location, (int)(value ? 1 : 0) );
        }

        public void Use()
        {
            GL.UseProgram( Id );
        }

        public void Use( GLCamera camera )
        {
            Use();
            camera.Bind( this );
        }

        [Conditional( "DEBUG" )]
        public void Check( )
        {
            foreach ( var uniform in mUniforms.Values )
            {
                if ( !uniform.IsAssigned )
                    throw new Exception( $"Uniform \"{uniform.Name}\" has not been assigned before use." );
            }
        }

        public void Dispose()
        {
            Dispose( true );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    GL.DeleteProgram( Id );
                }

                mDisposed = true;
            }
        }

        private Uniform GetUniform( string name )
        {
            Uniform uniform;

            if ( !mUniforms.ContainsKey(name) )
            {
                //throw new Exception( $"Uniform \"{name}\" is not registered" );
                RegisterUniform( name, null );
            }
            //else
            {
                uniform = mUniforms[name];
            }

            return uniform;
        }

        [Conditional( "DEBUG" )]
        private void DebugSetUniformAssignedFlag<T>( Uniform uniform, T value )
        {
            var type = typeof( T );
            var uniformType = uniform.Type;
            if ( uniformType != null && uniformType != type )
            {
                throw new Exception( $"Attempted to assign value of type {type} to uniform \"{uniform.Name}\" of type {uniformType}" );
            }

            uniform.IsAssigned = true;
        }
    }
}
