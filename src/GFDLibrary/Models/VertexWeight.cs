using System;

namespace GFDLibrary.Models
{
    public struct VertexWeight : IEquatable<VertexWeight>
    {
        public float[] Weights;
        public ushort[] Indices;

        public VertexWeight( float[] weight, byte[] indices )
        {
            Weights = weight;
            Indices = new ushort[indices.Length];
            Buffer.BlockCopy( indices, 0, Indices, 0, indices.Length );
        }
        public VertexWeight( float[] weight, ushort[] indices)
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

            Indices = new ushort[4];
            Indices[0] = index1;
            Indices[1] = index2;
            Indices[2] = index3;
            Indices[3] = index4;
        }
        // METAPHOR
        public VertexWeight(
            float w0, float w1, float w2, float w3, float w4, float w5, float w6, float w7,
            ushort i0, ushort i1, ushort i2, ushort i3, ushort i4, ushort i5, ushort i6, ushort i7 )
        {
            Weights = new float[8];
            Weights[0] = w0;
            Weights[1] = w1;
            Weights[2] = w2;
            Weights[3] = w3;
            Weights[4] = w4;
            Weights[5] = w5;
            Weights[6] = w6;
            Weights[7] = w7;

            Indices = new ushort[8];
            Indices[0] = i0;
            Indices[1] = i1;
            Indices[2] = i2;
            Indices[3] = i3;
            Indices[4] = i4;
            Indices[5] = i5;
            Indices[6] = i6;
            Indices[7] = i7;
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