using System.Collections.Generic;

namespace GFDLibrary.Textures.Swizzle
{
    public static class Swizzler
    {
        private static readonly Dictionary<SwizzleType, ISwizzleAlgorithm> sSwizzleAlgorithms = new Dictionary<SwizzleType, ISwizzleAlgorithm>()
        {
            { SwizzleType.None, new NullSwizzleAlgorithm() },
            { SwizzleType.PS3, new PS3SwizzleAlgorithm() },
            { SwizzleType.PS4, new PS4SwizzleAlgorithm() },
            { SwizzleType.Switch, new SwitchSwizzleAlgorithm() }
        };

        public static byte[] Swizzle( byte[] data, int width, int height, int blockSize, SwizzleType type )
        {
            if ( type == SwizzleType.None )
                return data;

            return sSwizzleAlgorithms[type].Swizzle( data, width, height, blockSize );
        }

        public static byte[] UnSwizzle( byte[] data, int width, int height, int blockSize, SwizzleType type )
        {
            if ( type == SwizzleType.None )
                return data;

            return sSwizzleAlgorithms[ type ].UnSwizzle( data, width, height, blockSize );
        }
    }
}
