using System;

namespace GFDLibrary.Textures.Swizzle
{
    // From RawTex by daemon1
    public class SwitchSwizzleAlgorithm : ISwizzleAlgorithm
    {
        public SwizzleType Type => SwizzleType.PS4;


        public byte[] Swizzle( byte[] data, int width, int height, int blockSize )
        {
            throw new NotImplementedException();
        }

        public byte[] UnSwizzle( byte[] data, int width, int height, int blockSize )
        {
            var buffer1 = new byte[data.Length * 4L];
            var dataIndex = 0;
            var numArray = new int[height * 2, width * 2];
            int num7 = width / 8;
            if ( num7 > 16 )
                num7 = 16;
            var num8 = 0;
            var num9 = 1;

            switch ( blockSize )
            {
                case 16:
                    num9 = 1;
                    break;
                case 8:
                    num9 = 2;
                    break;
                case 4:
                    num9 = 4;
                    break;
            }

            for ( var index1 = 0; index1 < width / 8 / num7; ++index1 )
            {
                for ( var index2 = 0; index2 < height / 4 / num9; ++index2 )
                {
                    for ( var index3 = 0; index3 < num7; ++index3 )
                    {
                        for ( var index4 = 0; index4 < 32; ++index4 )
                        {
                            for ( var index5 = 0; index5 < num9; ++index5 )
                            {
                                int num10 = sSwizzleTable[index4];
                                int num11 = num10 / 4;
                                int num12 = num10 % 4;
 
                                int index6 = ( index1 * num7 + index3 ) * 8 + num11;
                                int index7 = ( index2 * 4 + num12 ) * num9 + index5;
                                int destinationIndex = blockSize * ( index6 * height + index7 );

                                Array.Copy( data, dataIndex, buffer1, destinationIndex, blockSize );

                                dataIndex += blockSize;
                                numArray[index7, index6] = num8;

                                ++num8;
                            }
                        }
                    }
                }
            }

            Array.Resize( ref buffer1, data.Length );
            return buffer1;
        }

        private static readonly int[] sSwizzleTable = 
        {
            0,
            4,
            1,
            5,
            8,
            12,
            9,
            13,
            16,
            20,
            17,
            21,
            24,
            28,
            25,
            29,
            2,
            6,
            3,
            7,
            10,
            14,
            11,
            15,
            18,
            22,
            19,
            23,
            26,
            30,
            27,
            31
        };
    }
}
