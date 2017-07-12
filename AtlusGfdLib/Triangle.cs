using System;
using System.Collections;
using System.Linq;

namespace AtlusGfdLib
{
    public struct Triangle
    {
        public uint[] Indices;

        public Triangle( uint[] indices )
        {
            CheckIndices( indices );
            Indices = indices;
        }

        public Triangle( uint a, uint b, uint c )
        {
            Indices = new uint[3];
            Indices[0] = a;
            Indices[1] = b;
            Indices[2] = c;
        }

        public Triangle( IConvertible[] indices )
        {
            CheckIndices( indices );

            Indices = new uint[indices.Length];
            for ( int i = 0; i < Indices.Length; i++ )
            {
                Indices[i] = ( uint )indices[i];
            }
        }

        public Triangle( IConvertible a, IConvertible b, IConvertible c )
        {
            Indices = new uint[3];
            Indices[0] = ( uint )a;
            Indices[1] = ( uint )b;
            Indices[2] = ( uint )c;
        }

        private static void CheckIndices( ICollection indices )
        {
            if ( indices.Count != 3 )
                throw new ArgumentException( "Invalid number of indices for a triangle" );
        }
    }
}