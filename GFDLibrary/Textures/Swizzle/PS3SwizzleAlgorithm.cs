using System;

namespace GFDLibrary.Textures.Swizzle
{
    // From RawTex by daemon1
    public class PS3SwizzleAlgorithm : ISwizzleAlgorithm
    {
        public SwizzleType Type => SwizzleType.PS3;

        public byte[] Swizzle( byte[] data, int width, int height, int blockSize )
        {
            throw new NotImplementedException();
        }

        public byte[] UnSwizzle( byte[] data, int width, int height, int blockSize )
        {
            var unswizzled = new byte[data.Length];
            var dataIndex = 0;
            int heightTexels = height / 4;
            int widthTexels = width / 4;
            var texelCount = widthTexels * heightTexels;

            for ( int texel = 0; texel < texelCount; ++texel )
            {
                int pixelIndex = SwizzleUtilities.Morton( texel, widthTexels, heightTexels );
                int destIndex = blockSize * pixelIndex;
                Array.Copy( data, dataIndex, unswizzled, destIndex, blockSize );
                dataIndex += blockSize;
            }

            return unswizzled;
        }
    }
}
