using System;
using System.Diagnostics;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.DataManagement;
using OpenTK;

namespace GFDStudio.GUI.Controls.ModelView
{
    public class LineShaderProgram : GLShaderProgram
    {
        public static bool TryCreate( out LineShaderProgram lineShaderProgram )
        {
            if ( !TryCreate( DataStore.GetPath( "shaders/line.glsl.vs" ),
                             DataStore.GetPath( "shaders/line.glsl.fs" ),
                             out var id ) )
            {
                Trace.TraceError( "Failed to compile line shader" );
                lineShaderProgram = null;
                return false;
            }

            lineShaderProgram = new LineShaderProgram( id );
            return true;
        }

        private LineShaderProgram( int id ) : base( id )
        {
            RegisterUniform<Matrix4>( "view" );
            RegisterUniform<Matrix4>( "projection" );
            RegisterUniform<Vector4>( "color" );
            RegisterUniform<float>( "minZ" );
        }
    }
}
