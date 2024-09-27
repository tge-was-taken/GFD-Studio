using GFDLibrary;
using GFDLibrary.Rendering.OpenGL;
using System;
using System.Collections.Generic;

namespace GFDLibrary.Rendering.OpenGL
{
    public class ShaderRegistry
    {
        public GLShaderProgram mDefaultShader;
        public GLShaderProgram mLineShader;
        //public Dictionary<int, GLShaderProgram> mPersona5Shaders;
        public Dictionary<ResourceType, GLShaderProgram> mMetaphorShaders;
        public ShaderRegistry() {}
        public bool InitializeShaders(Func<string, string> GetAppData)
        {
            if ( !GLShaderProgram.TryCreate( GetAppData( "shaders/default.glsl.vs" ),
                                             GetAppData( "shaders/default.glsl.fs" ),
                                             out mDefaultShader ) )
            {
                if ( !GLShaderProgram.TryCreate( GetAppData( "shaders/basic.glsl.vs" ),
                                                 GetAppData( "shaders/basic.glsl.fs" ),
                                                 out mDefaultShader ) )
                {
                    return false;
                }
            }
            if ( !GLShaderProgram.TryCreate( GetAppData( "shaders/line.glsl.vs" ), GetAppData( "shaders/line.glsl.fs" ), out mLineShader ) )
                return false;
            try
            {
                mMetaphorShaders = new()
                {
                    { ResourceType.MaterialParameterSetType0, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType1, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType2_3_13, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType4, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType5, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType6, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType7, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType8, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType9, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType10, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType11, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType12, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType14, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                    { ResourceType.MaterialParameterSetType15, GLShaderProgram.TryThrowOnFail( GetAppData( "shaders/basic.glsl.vs" ), GetAppData( "shaders/basic.glsl.fs" )) },
                };
            }
            catch ( Exception ex )
            {
                Logger.Info( $"Exception occurred when building shaders for Metaphor: {ex.Message}" );
                Logger.Info( $"{ex.StackTrace}" );
                return false;
            }
            return true;
        }
    }
}
