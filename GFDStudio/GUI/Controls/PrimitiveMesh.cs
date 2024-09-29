using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using GFDLibrary.Rendering.OpenGL;
using GFDLibrary.Utilities;
using GFDStudio.DataManagement;
using OpenTK.Graphics.OpenGL;
using Vector4 = OpenTK.Vector4;

namespace GFDStudio.GUI.Controls
{
    internal class PrimitiveMesh
    {
        public static readonly Vector4 DefaultColor = new Vector4( 1.0f, 1.0f, 0.0f, 1.0f );

        private readonly System.Numerics.Vector3[] mVertices;
        private readonly uint[]                    mIndices;
        private readonly PrimitiveType mPrimitiveType;

        public PrimitiveMesh( string filePath )
        {
            var vertices = new List<System.Numerics.Vector3>();
            var indices  = new List<uint>();

            using ( var reader = File.OpenText(DataStore.GetPath( filePath ) ) )
            {
                float ParseFloat( string str ) => float.Parse( str, CultureInfo.InvariantCulture );

                string line;
                while ( ( line = reader.ReadLine() ) != null )
                {
                    var tokens = line.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                    if ( tokens.Length == 0 )
                        continue;

                    switch ( tokens[ 0 ] )
                    {
                        case "v":
                            vertices.Add( new System.Numerics.Vector3( ParseFloat( tokens[ 1 ] ), ParseFloat( tokens[ 2 ] ),
                                                                       ParseFloat( tokens[ 3 ] ) ) );
                            break;
                        case "f":
                            mPrimitiveType = tokens.Length == 4 ? PrimitiveType.Triangles : PrimitiveType.Quads;
                            indices.Add( uint.Parse( tokens[ 1 ] ) - 1 );
                            indices.Add( uint.Parse( tokens[ 2 ] ) - 1 );
                            indices.Add( uint.Parse( tokens[ 3 ] ) - 1 );

                            if ( mPrimitiveType == PrimitiveType.Quads )
                                indices.Add( uint.Parse( tokens[ 4 ] ) - 1 );
                            break;
                    }
                }
            }

            mVertices = vertices.ToArray();
            mIndices  = indices.ToArray();
        }

        public GLMesh Instantiate( bool renderWireframe, bool enableBackfaceCulling, Vector4 color )
        {
            return new GLMesh( new GLVertexArray( mVertices, null, null, null, mIndices, mPrimitiveType ),
                               new GLMaterial { RenderWireframe = renderWireframe, Diffuse = color, EnableBackfaceCulling = enableBackfaceCulling }, true );
        }
    }
}