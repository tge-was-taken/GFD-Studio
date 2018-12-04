using System.Diagnostics;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.DataManagement;
using OpenTK;

namespace GFDStudio.GUI.Controls.ModelView
{
    public class DefaultShaderProgram : GLShaderProgram
    {
        public static bool TryCreate( out DefaultShaderProgram shaderProgram )
        {
            if ( !TryCreate( DataStore.GetPath( "shaders/default.glsl.vs" ),
                                             DataStore.GetPath( "shaders/default.glsl.fs" ),
                                             out int id ) )
            {
                Trace.TraceWarning( "Failed to compile shaders. Trying to use basic shaders.." );

                if ( !TryCreate( DataStore.GetPath( "shaders/basic.glsl.vs" ),
                                                 DataStore.GetPath( "shaders/basic.glsl.fs" ),
                                                 out id ) )
                {
                    Trace.TraceError( "Failed to compile basic shaders. Disabling GL rendering." );
                    shaderProgram = null;
                    return false;
                }
            }

            shaderProgram = new DefaultShaderProgram( id );
            return true;
        }

        protected DefaultShaderProgram( int id ) : base( id )
        {
            RegisterUniform<Matrix4>( "model" );
            RegisterUniform<Matrix4>( "view" );
            RegisterUniform<Matrix4>( "projection" );
            RegisterUniform<bool>( "matHasDiffuse" );
            RegisterUniform<Vector4>( "matAmbient" );
            RegisterUniform<Vector4>( "matDiffuse" );
            RegisterUniform<Vector4>( "matSpecular" );
            RegisterUniform<Vector4>( "matEmissive" );
            RegisterUniform<bool>( "matHasAlphaTransparency" );
        }
    }
}
