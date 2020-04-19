namespace GFDLibrary.Textures.Swizzle
{
    public class NullSwizzleAlgorithm : ISwizzleAlgorithm
    {
        public SwizzleType Type => SwizzleType.None;

        public byte[] Swizzle( byte[] data, int width, int height, int blockSize )
        {
            return data;
        }

        public byte[] UnSwizzle( byte[] data, int width, int height, int blockSize )
        {
            return data;
        }
    }
}
