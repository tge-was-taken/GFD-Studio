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
        public List<AnimationJoint> Controllers { get; }
        public List<AnimationJoint> BlendControllers { get; }
        public Matrix4x4 CurrentTransform { get; set; }
        public Matrix4x4 WorldTransform { get; set; }
        public List<GLMesh> Geometries { get; }
        public bool IsVisible { get; set; } = true;

        public GLNode( Node node )
        {
            Node = node;
            WorldTransform = node.WorldTransform;
            Geometries = new List<GLMesh>();
            Controllers = new List<AnimationJoint>();
            BlendControllers = new List<AnimationJoint>();
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

                    foreach ( var glMesh in Geometries )
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