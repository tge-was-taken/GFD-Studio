namespace AtlusGfdLib
{
    public struct VertexWeight
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
    }
}