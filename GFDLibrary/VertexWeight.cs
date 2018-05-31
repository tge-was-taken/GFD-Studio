using System;

namespace GFDLibrary
{
    public struct VertexWeight : IEquatable<VertexWeight>
    {
        public float[] Weights;
        public byte[] Indices;

        public VertexWeight( float[] weight, byte[] indices )
        {
            Weights = weight;
            Indices = indices;
        }

        public VertexWeight( float weight1, float weight2, float weight3, float weight4, byte index1, byte index2, byte index3, byte index4 )
        {
            Weights = new float[4];
            Weights[0] = weight1;
            Weights[1] = weight2;
            Weights[2] = weight3;
            Weights[3] = weight4;

            Indices = new byte[4];
            Indices[0] = index1;
            Indices[1] = index2;
            Indices[2] = index3;
            Indices[3] = index4;
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( VertexWeight ) )
                return false;

            return Equals( (VertexWeight)obj );
        }

        public bool Equals( VertexWeight other )
        {
            if ( Weights != null && other.Weights == null || Weights == null && other.Weights != null )
                return false;

            if ( Weights.Length != other.Weights.Length )
                return false;

            for ( int i = 0; i < Weights.Length; i++ )
            {
                if ( Weights[i] != other.Weights[i] )
                    return false;
            }

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
                if ( Weights != null )
                    hash = hash * 33 + Weights.GetHashCode();
                if ( Indices != null )
                    hash = hash * 33 + Indices.GetHashCode();
                return hash;
            }
        }
    }
}