using System;
using System.Collections;
using System.Linq;

namespace AtlusGfdLib
{
    public struct Triangle : IEquatable<Triangle>
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

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( Triangle ) )
                return false;

            return Equals( ( Triangle)obj );
        }

        public bool Equals( Triangle other )
        {
            if ( Indices != null && other.Indices == null || Indices == null && other.Indices != null )
                return false;

            if ( Indices.Length != other.Indices.Length )
                return false;

            for ( int i = 0; i < Indices.Length; i++ )
            {
                if ( Indices[i] != other.Indices[i] )
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                if ( Indices != null )
                    hash = hash * 33 + Indices.GetHashCode();

                return hash;
            }
        }
    }
}