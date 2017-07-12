using System;
using System.Linq;

namespace AtlusGfdLib
{
    public struct Triangle
    {
        public uint[] Indices;

        public Triangle( uint[] indices )
        {
            Indices = indices;
        }

        public Triangle( int[] indices )
        {
            Indices = new uint[indices.Length];
            for ( int i = 0; i < Indices.Length; i++ )
            {
                Indices[i] = (uint)indices[i];
            }
        }

        public Triangle( ushort[] indices )
        {
            Indices = new uint[indices.Length];
            for ( int i = 0; i < Indices.Length; i++ )
            {
                Indices[i] = ( uint )indices[i];
            }
        }

        public Triangle( IConvertible[] indices )
        {
            Indices = new uint[indices.Length];
            for ( int i = 0; i < Indices.Length; i++ )
            {
                Indices[i] = ( uint )indices[i];
            }
        }
    }
}