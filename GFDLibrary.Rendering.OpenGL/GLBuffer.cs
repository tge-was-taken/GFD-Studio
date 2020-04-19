using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLBuffer<T> : IDisposable where T : struct
    {
        public int Id { get; }

        public int Count { get; }

        public int Stride { get; }

        public int TotalSize { get; }

        public GLBuffer(BufferTarget target, T[] data) 
        {
            // generate buffer id
            Id = GL.GenBuffer();

            // mark buffer as active
            GL.BindBuffer( target, Id );

            Count = data.Length;
            Stride = Unsafe.SizeOf<T>();
            TotalSize = Stride * Count;

            // upload data to buffer store
#if GL_DEBUG
            try
            {
#endif
                GL.BufferData( target, TotalSize, data, BufferUsageHint.StreamDraw );
#if GL_DEBUG
            }
            catch ( Exception )
            {
            }
#endif
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposedValue )
            {
                if ( disposing )
                {
                    GL.DeleteBuffer( Id );
                }

                disposedValue = true;
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
