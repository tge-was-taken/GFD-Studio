using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLModel : IDisposable
    {
        public ModelPack ModelPack { get; }

        public List<GLNode> Nodes { get; }

        public Dictionary<string, GLMaterial> Materials { get; }

        public Animation Animation { get; private set; }

        public GLModel( ModelPack modelPack, MaterialTextureCreator textureCreator )
        {
            ModelPack = modelPack;

            var nodes = modelPack.Model.Nodes.ToList();
            Nodes = nodes.Select( x => new GLNode( x ) ).ToList();

            Materials = modelPack.Materials?.ToDictionary( x => x.Key, y => new GLMaterial( y.Value, textureCreator ) ) ??
                        new Dictionary<string, GLMaterial>();

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
                    glNode.Meshes[i] = new GLMesh( oldGlMesh.Mesh, glNode.WorldTransform, ModelPack.Model.Bones, Nodes, Materials );
                    oldGlMesh.Dispose();
                }
            }
        }

        public void Draw( GLShaderProgram shaderProgram, GLCamera camera, double animationTime )
        {
            var view = camera.View;
            var proj = camera.Projection;
            Draw( shaderProgram, ref view, ref proj, animationTime );
        }

        public void Draw( GLShaderProgram shaderProgram, ref Matrix4 view, ref Matrix4 projection, double animationTime )
        {
            if ( Animation != null )
                AnimateNodes( animationTime );

            shaderProgram.Use();
            shaderProgram.SetUniform( "uView", view );
            shaderProgram.SetUniform( "uProjection", projection );

            foreach ( var glNode in Nodes )
            {
                if ( !glNode.IsVisible )
                    continue;

                for ( var i = 0; i < glNode.Meshes.Count; i++ )
                {
                    var glMesh = glNode.Meshes[i];
                    if ( Animation != null && glMesh.Mesh != null )
                    {
                        var oldGlMesh             = glMesh;
                        glMesh = glNode.Meshes[i] = new GLMesh( oldGlMesh.Mesh, glNode.WorldTransform, ModelPack.Model.Bones, Nodes, Materials );
                        oldGlMesh.Dispose();
                    }

                    glMesh.Draw( glNode.WorldTransform.ToOpenTK(), shaderProgram );
                }
            }
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
}
