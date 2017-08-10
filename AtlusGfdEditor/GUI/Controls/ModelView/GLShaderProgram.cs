using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AtlusGfdEditor.GUI.Controls.ModelView
{
    public class GLShaderProgram : IDisposable
    {
        class Uniform
        {
            public string Name;
            public Type Type;
            public int Location;
            public bool IsAssigned;
        }

        private bool mDisposed;
        private Dictionary<string, Uniform> mUniforms;

        public int ID { get; }

        public GLShaderProgram( int glShaderProgram )
        {
            ID = glShaderProgram;
            mUniforms = new Dictionary<string, Uniform>();
        }

        ~GLShaderProgram()
        {
            Dispose( false );
        }

        public static bool TryCreate( string vertexShaderFilepath, string fragmentShaderFilepath, out GLShaderProgram program )
        {
            var vertexShaderSource = File.ReadAllText( vertexShaderFilepath );
            var fragmentShaderSource = File.ReadAllText( fragmentShaderFilepath );

            using ( var builder = new GLShaderProgramBuilder() )
            {
                if ( !builder.AttachShader( ShaderType.VertexShader, vertexShaderSource ) )
                {
                    program = null;
                    return false;
                }

                if ( !builder.AttachShader( ShaderType.FragmentShader, fragmentShaderSource ))
                {
                    program = null;
                    return false;
                }

                if ( !builder.Build( out program ))
                {
                    return false;
                }

                return true;
            }
        }

        public static GLShaderProgram Create( string vertexShaderFilepath, string fragmentShaderFilepath )
        {
            if ( !TryCreate(vertexShaderFilepath, fragmentShaderFilepath, out var program))
            {
                throw new Exception( "Failed to create shader program" );
            }
            else
            {
                return program;
            }
        }

        public void RegisterUniform<T>( string name )
        {
            // check if uniform exists
            if ( mUniforms.ContainsKey(name) )
            {
                throw new Exception( $"Uniform \"{name}\" already registered" );
            }

            // get the location of the uniform isn't invalid
            int location = GL.GetUniformLocation( ID, name );
            if ( location == -1 )
            {
                //throw new Exception( $"Uniform \"{name}\" does not exist in shader program" );
            }

            // register uniform in dict
            mUniforms[name] = new Uniform()
            {
                Name = name,
                Type = typeof( T ),
                Location = location,
            };
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

        public void SetUniform( string name, bool value )
        {
            var uniform = GetUniform( name );
            DebugSetUniformAssignedFlag( uniform, value );

            GL.Uniform1( uniform.Location, (int)(value ? 1 : 0) );
        }

        public void Use()
        {
            DebugCheckAllUniformsAssigned();

            GL.UseProgram( ID );
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
                    GL.DeleteProgram( ID );
                }

                mDisposed = true;
            }
        }

        private Uniform GetUniform( string name )
        {
            Uniform uniform;

            if ( !mUniforms.ContainsKey(name) )
            {
                throw new Exception( "Uniform \"{name}\" is not registered" );
            }
            else
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
            if ( uniformType != type )
            {
                throw new Exception( $"Attempted to assign value of type {type} to uniform \"{uniform.Name}\" of type {uniformType}" );
            }

            uniform.IsAssigned = true;
        }

        [Conditional("DEBUG")]
        private void DebugCheckAllUniformsAssigned()
        {
            foreach ( var uniform in mUniforms.Values )
            {
                if ( !uniform.IsAssigned )
                    throw new Exception( $"Uniform \"{uniform.Name}\" has not been assigned before use." );
            }
        }
    }
}
