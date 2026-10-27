using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLModel : IDisposable
    {
        public ModelPack ModelPack { get; }

        public List<GLNode> Nodes { get; }

        public Dictionary<string, GLBaseMaterial> Materials { get; }

        public Animation Animation { get; private set; }

        public GLModel( ModelPack modelPack, MaterialTextureCreator textureCreator )
        {
            ModelPack = modelPack;

            var nodes = modelPack.Model.Nodes.ToList();
            Nodes = nodes.Select( x => new GLNode( x ) ).ToList();

            Materials = modelPack.Materials?.ToDictionary( x => x.Key, y => GLBaseMaterial.CreateGLMaterial( y.Value, textureCreator ) ) ??
                        new Dictionary<string, GLBaseMaterial>();

            foreach ( var glNode in Nodes )
            {
                glNode.Parent = Nodes.FirstOrDefault( x => x.Node == glNode.Node.Parent );
                if ( !glNode.Node.HasAttachments )
                    continue;

                foreach ( var attachment in glNode.Node.Attachments )
                {
                    if ( attachment.Type != NodeAttachmentType.Mesh )
                        continue;

                    var glMesh = new GLMesh( attachment.GetValue<Mesh>(), glNode.Node.WorldTransform, modelPack.Model.Bones, Nodes, Materials );
                    glNode.Meshes.Add( glMesh );
                }
            }
        }

        public void LoadAnimation( Animation animation )
        {
            Animation = animation;

            foreach ( var glNode in Nodes )
            {
                glNode.Controllers.Clear();
                glNode.Controllers.AddRange( animation.Controllers.Where( x => x.TargetKind == TargetKind.Node &&
                                                                               x.TargetName == glNode.Node.Name ) );
            }
        }

        public void UnloadAnimation()
        {
            Animation = null;

            foreach ( var glNode in Nodes )
            {
                // Calculate current transform
                var transform = Matrix4x4.CreateFromQuaternion( glNode.Node.Rotation ) * Matrix4x4.CreateScale( glNode.Node.Scale );
                transform.Translation   = glNode.Node.Translation;
                glNode.CurrentTransform = transform;

                // Calculate world transform
                glNode.WorldTransform =
                    glNode.Parent == null ? glNode.CurrentTransform : glNode.CurrentTransform * glNode.Parent.WorldTransform;
            }

            foreach ( var glNode in Nodes )
            {
                // Rebuild meshes
                for ( var i = 0; i < glNode.Meshes.Count; i++ )
                {
                    var oldGlMesh = glNode.Meshes[i];
                    if (oldGlMesh.Mesh != null)
                    {
                        glNode.Meshes[i] = new GLMesh( oldGlMesh.Mesh, glNode.WorldTransform, ModelPack.Model.Bones, Nodes, Materials );
                    }

                    oldGlMesh.Dispose();
                }
            }
        }

        private GLShaderProgram GetTargetShader(ShaderRegistry shaderRegistry, GLMesh glMesh, Matrix4 view, Matrix4 projection, HashSet<ResourceType> shaderPrograms )
        {
            if ( typeof( GLMetaphorMaterial ).IsInstanceOfType( glMesh.Material )
                && shaderRegistry.mMetaphorShaders.TryGetValue( ( (GLMetaphorMaterial)glMesh.Material ).ParameterSet.ResourceType, out var metaphorShader ) )
            {
                if ( !shaderPrograms.Contains( ((GLMetaphorMaterial)glMesh.Material).ParameterSet.ResourceType ) )
                {
                    metaphorShader.Use();
                    metaphorShader.SetUniform( "uView", view );
                    metaphorShader.SetUniform( "uProjection", projection );
                }
                return metaphorShader;
            }
            return shaderRegistry.mDefaultShader;
        }

        /// <summary>
        /// Matches MAX_BONES in the vertex shaders. The widest single mesh seen across the test
        /// corpus uses 83 bones, so this leaves room without overrunning the uniform space.
        /// </summary>
        private const int MaxPaletteBones = 96;

        private float[] mBonePalette = new float[0];
        private readonly HashSet<string> mReportedOversizedMeshes = new();

        /// <summary>
        /// Fills the shader's bone palette for one mesh. Each bone goes up as the top three rows
        /// of its matrix, the fourth being constant for an affine transform, which is a quarter
        /// less to upload and keeps the array inside the smallest guaranteed uniform space.
        /// </summary>
        private void UploadBonePalette( GLMesh glMesh, GLShaderProgram shader )
        {
            if ( !glMesh.IsSkinned )
                return;

            var palette = glMesh.PaletteBones;

            if ( palette.Length > MaxPaletteBones )
            {
                if ( mReportedOversizedMeshes.Add( glMesh.Mesh?.MaterialName ?? "?" ) )
                    Trace.TraceWarning( $"Mesh uses {palette.Length} bones, more than the {MaxPaletteBones} the shader holds" );

                return;
            }

            var needed = palette.Length * 3 * 4;

            if ( mBonePalette.Length < needed )
                mBonePalette = new float[needed];

            for ( var i = 0; i < palette.Length; i++ )
            {
                var bone = ModelPack.Model.Bones[palette[i]];
                var matrix = bone.InverseBindMatrix * Nodes[bone.NodeIndex].WorldTransform;
                var at = i * 12;

                // The shader multiplies column vectors, so each row here is a column of the
                // matrix as System.Numerics lays it out.
                mBonePalette[at + 0] = matrix.M11;
                mBonePalette[at + 1] = matrix.M21;
                mBonePalette[at + 2] = matrix.M31;
                mBonePalette[at + 3] = matrix.M41;

                mBonePalette[at + 4] = matrix.M12;
                mBonePalette[at + 5] = matrix.M22;
                mBonePalette[at + 6] = matrix.M32;
                mBonePalette[at + 7] = matrix.M42;

                mBonePalette[at + 8] = matrix.M13;
                mBonePalette[at + 9] = matrix.M23;
                mBonePalette[at + 10] = matrix.M33;
                mBonePalette[at + 11] = matrix.M43;
            }

            shader.SetUniform( "uBones", mBonePalette, palette.Length * 3 );
        }

        private bool ShouldMeshShowAsSelected( DrawContext context, GLMesh glMesh )
        {
            return context.SelectedMesh == glMesh.Mesh || context.SelectedMaterial?.Name == glMesh.Mesh.MaterialName;
        }

        public void Draw( DrawContext context )
        {
            if ( Animation != null )
                AnimateNodes( context.AnimationTime );
            context.ShaderRegistry.mDefaultShader.Use();
            context.ShaderRegistry.mDefaultShader.SetUniform( "uView", context.Camera.View );
            context.ShaderRegistry.mDefaultShader.SetUniform( "uProjection", context.Camera.Projection );

            HashSet<ResourceType> shaderProgramsInUse = new();

            // List to hold transparent meshes and their world transforms
            List<Tuple<GLMesh, Matrix4, GLShaderProgram>> transparentMeshes = new();

            // Draw opaque objects first
            foreach ( var glNode in Nodes )
            {
                if ( !glNode.IsVisible )
                    continue;

                for ( var i = 0; i < glNode.Meshes.Count; i++ )
                {
                    var glMesh = glNode.Meshes[i];


                    GLShaderProgram targetShader = GetTargetShader( context.ShaderRegistry, glMesh, context.Camera.View, context.Camera.Projection, shaderProgramsInUse );
                    if ( !glMesh.Material.IsMaterialTransparent() ) // If opaque
                    {
                        if ( glMesh.Mesh != null )
                            targetShader.SetUniform( "uIsSelected", ShouldMeshShowAsSelected(context, glMesh ) );
                        UploadBonePalette( glMesh, targetShader );
                        glMesh.Draw( glNode.WorldTransform.ToOpenTK(), targetShader );
                    }
                    else // If transparent
                    {
                        transparentMeshes.Add( Tuple.Create( glMesh, glNode.WorldTransform.ToOpenTK(), targetShader ) );
                    }
                }
            }

            // Enable blending for transparent objects
            GL.Enable( EnableCap.Blend );

            // Sort transparent objects based on their distance from the camera
            transparentMeshes.Sort( ( a, b ) => OpenTK.Mathematics.Vector3.Distance( context.Camera.Translation, b.Item2.ExtractTranslation() ).CompareTo( OpenTK.Mathematics.Vector3.Distance( context.Camera.Translation, a.Item2.ExtractTranslation() ) ) );

            // Disable depth mask
            GL.DepthMask( false );

            // Then draw transparent objects
            foreach ( (var glMesh, var worldTransform, var shaderProgram) in transparentMeshes )
            {
                switch ( glMesh.Material.DrawMethod )
                {
                    case 1:
                        GL.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
                        break;
                    case 2:
                        GL.BlendFuncSeparate( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One, // RGB blending
                                              BlendingFactorSrc.Zero, BlendingFactorDest.One ); // Alpha blending
                        break;
                    case 4:
                        GL.BlendFunc( BlendingFactor.DstColor, BlendingFactor.Zero );
                        break;
                }

                if ( glMesh.Mesh != null )
                    shaderProgram.SetUniform( "uIsSelected", ShouldMeshShowAsSelected( context, glMesh ) );
                UploadBonePalette( glMesh, shaderProgram );
                glMesh.Draw( worldTransform, shaderProgram );
            }

            // Re-enable depth mask
            GL.DepthMask( true );

            // Disable blending after drawing transparent objects
            GL.Disable( EnableCap.Blend );
        }


        private void AnimateNodes( double animationTime )
        {
            foreach ( var glNode in Nodes )
            {
                var rotation    = glNode.Node.Rotation;
                var translation = glNode.Node.Translation;
                var scale       = glNode.Node.Scale;

                foreach ( var controller in glNode.Controllers )
                {
                    foreach ( var layer in controller.Layers )
                    {
                        Key curKey = null;
                        Key nextKey = null;

                        if ( controller != null )
                        {
                            (curKey, nextKey) = GetCurrentAndNextKeys( layer, animationTime );
                        }

                        if ( controller != null && curKey != null )
                        {
                            if ( layer.HasPRSKeyFrames )
                            {
                                var prsKey = ( PRSKey )curKey;
                                var nextPrsKey = ( PRSKey )nextKey;

                                if ( nextPrsKey != null )
                                {
                                    InterpolateKeys( animationTime, layer, ref rotation, ref translation, ref scale, prsKey, nextPrsKey );
                                }
                                else
                                {
                                    if ( prsKey.HasRotation )
                                        rotation = prsKey.Rotation;

                                    if ( prsKey.HasPosition )
                                        translation = prsKey.Position * layer.PositionScale;

                                    if ( prsKey.HasScale )
                                        scale = prsKey.Scale * layer.ScaleScale;
                                }
                            }
                            else
                            {
                                //Debugger.Break();
                            }
                        }
                    }
                }

                // Calculate current transform
                var transform = Matrix4x4.CreateFromQuaternion( rotation ) * Matrix4x4.CreateScale( glNode.Node.Scale );
                transform.Translation   = translation;
                glNode.CurrentTransform = transform;

                // Calculate world transform
                glNode.WorldTransform =
                    glNode.Parent == null ? glNode.CurrentTransform : glNode.CurrentTransform * glNode.Parent.WorldTransform;
            }
        }

        private void InterpolateKeys( double animationTime, AnimationLayer layer, ref Quaternion rotation, ref Vector3 translation, ref Vector3 scale, PRSKey prsKey, PRSKey nextPrsKey )
        {
            var nextTime = ( nextPrsKey.Time < prsKey.Time
                ? ( nextPrsKey.Time + Animation.Duration )
                : nextPrsKey.Time );

            var blend = ( float ) ( animationTime / nextTime );

            if ( prsKey.HasRotation )
                rotation = Quaternion.Slerp( prsKey.Rotation, nextPrsKey.Rotation, blend );

            if ( prsKey.HasPosition )
            {
                translation = Vector3.Lerp( prsKey.Position * layer.PositionScale,
                                            nextPrsKey.Position * layer.PositionScale,
                                            blend );
            }

            if ( prsKey.HasScale )
            {
                scale = Vector3.Lerp( prsKey.Scale * layer.ScaleScale,
                                      nextPrsKey.Scale * layer.ScaleScale,
                                      blend );
            }
        }

        private static (Key curKey, Key nextKey) GetCurrentAndNextKeys( AnimationLayer layer, double animationTime )
        {
            Key curKey = null;
            Key nextKey = null;

            // Find most recent key
            foreach ( var key in layer.Keys )
            {
                if ( key.Time <= animationTime )
                    curKey = key;
            }

            // Find next key
            if ( curKey != null )
            {
                foreach ( var key in layer.Keys )
                {
                    if ( key != curKey && key.Time != curKey.Time && key.Time >= animationTime )
                    {
                        nextKey = key;
                        break;
                    }
                }

                if ( nextKey == null )
                {
                    nextKey = layer.Keys.FirstOrDefault( x => x.Time != curKey.Time );
                }
            }

            return ( curKey, nextKey );
        }

        #region IDisposable Support
        private bool mDisposed; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    GLVertexArray.UnbindAll();
                    GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
                    GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
                    GL.BindTexture( TextureTarget.Texture2D, 0 );

                    foreach ( var glNode in Nodes )
                    {
                        glNode.Dispose();

                        foreach ( var geometry in glNode.Meshes )
                            geometry.Dispose();
                    }
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

    public class DrawContext
    {
        public ShaderRegistry ShaderRegistry { get; init; }
        public GLCamera Camera { get; init; }
        public double AnimationTime { get; init; }
        public Mesh SelectedMesh { get; init; }
        public Material SelectedMaterial { get; init; }
    }
}
