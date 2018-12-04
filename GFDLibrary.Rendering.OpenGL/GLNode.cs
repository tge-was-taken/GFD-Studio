using System;
using System.Collections.Generic;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Models;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLNode : IDisposable
    {
        public GLNode Parent { get; set; }
        public Node Node { get; set; }
        public List<AnimationController> Controllers { get; }
        public Matrix4x4 CurrentTransform { get; set; }
        public Matrix4x4 WorldTransform { get; set; }
        public List<GLMesh> Meshes { get; }

        public GLNode( Node node )
        {
            Node = node;
            WorldTransform = node.WorldTransform;
            Meshes = new List<GLMesh>();
            Controllers = new List<AnimationController>();
        }

        #region IDisposable Support
        private bool mDisposed = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    Parent?.Dispose();

                    foreach ( var glMesh in Meshes )
                    {
                        glMesh.Dispose();
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