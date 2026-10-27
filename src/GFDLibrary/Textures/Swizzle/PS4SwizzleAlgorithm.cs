using System;

namespace GFDLibrary.Textures.Swizzle
{
    // From RawTex by daemon1
    public class PS4SwizzleAlgorithm : ISwizzleAlgorithm
    {
        public SwizzleType Type => SwizzleType.PS4;

        public byte[] Swizzle( byte[] data, int width, int height, int blockSize )
        {
            return DoSwizzle( data, width, height, blockSize, false );
        }

        public byte[] UnSwizzle( byte[] data, int width, int height, int blockSize )
        {
            return DoSwizzle( data, width, height, blockSize, true );
        }

        private byte[] DoSwizzle( byte[] data, int width, int height, int blockSize, bool unswizzle )
        {
            byte[] processed        = new byte[data.Length];
            int heightTexels        = height / 4;
            int heightTexelsAligned = ( heightTexels + 7 ) / 8;
            int widthTexels         = width / 4;
            int widthTexelsAligned  = ( widthTexels + 7 ) / 8;
            int dataIndex           = 0;
            string errorMesage      = string.Empty;

            for ( int y = 0; y < heightTexelsAligned; ++y )
            {
                for ( int x = 0; x < widthTexelsAligned; ++x )
                {
                    for ( int t = 0; t < 64; ++t )
                    {
                        int pixelIndex = SwizzleUtilities.Morton( t, 8, 8 );
                        int num8       = pixelIndex / 8;
                        int num9       = pixelIndex % 8;
                        int yOffset    = ( y * 8 ) + num8;
                        int xOffset    = ( x * 8 ) + num9;

                        if ( xOffset < widthTexels && yOffset < heightTexels )
                        {
                            int destPixelIndex = yOffset * widthTexels + xOffset;
                            int destIndex      = blockSize * destPixelIndex;

                            try
                            {
                                if ( unswizzle )
                                {
                                    if ( processed.Length < destIndex + blockSize )
                                        Array.Resize<byte>( ref processed, destIndex + blockSize ); //blockSize = processed.Length - destIndex;
                                    Array.Copy( data, dataIndex, processed, destIndex, blockSize );
                                }
                                else
                                {
                                    if ( processed.Length < dataIndex + blockSize )
                                        Array.Resize<byte>( ref processed, dataIndex + blockSize ); //blockSize = processed.Length - dataIndex;
                                    Array.Copy( data, destIndex, processed, dataIndex, blockSize );
                                }
                            }
                            catch ( Exception e)
                            {
                                errorMesage = e.Message;
                            }
                        }

                        dataIndex += blockSize;
                    }
                }
            }
            if ( errorMesage != string.Empty ) Console.WriteLine( errorMesage );

            return processed;
        }
    }
}
