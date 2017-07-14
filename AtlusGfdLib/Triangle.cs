using System;
using System.Collections;
using System.Linq;

namespace AtlusGfdLib
{
    public struct Triangle
    {
        public int[] Indices;

        public Triangle( int[] indices )
        {
            CheckIndices( indices );
            Indices = indices;
        }

        public Triangle( int a, int b, int c )
        {
            Indices = new int[3];
            Indices[0] = a;
            Indices[1] = b;
            Indices[2] = c;
        }

        public Triangle( IConvertible[] indices )
        {
            CheckIndices( indices );

            Indices = new int[indices.Length];
            for ( int i = 0; i < Indices.Length; i++ )
            {
                Indices[i] = ( int )indices[i];
            }
        }

        public Triangle( IConvertible a, IConvertible b, IConvertible c )
        {
            Indices = new int[3];
            Indices[0] = ( int )a;
            Indices[1] = ( int )b;
            Indices[2] = ( int )c;
        }

        private static void CheckIndices( ICollection indices )
        {
            if ( indices.Count != 3 )
                throw new ArgumentException( "Invalid number of indices for a triangle" );
        }
    }
}