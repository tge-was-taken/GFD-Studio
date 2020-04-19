namespace GFDLibrary.Textures.Swizzle
{
    public interface ISwizzleAlgorithm
    {
        SwizzleType Type { get; }

        byte[] Swizzle( byte[] data, int width, int height, int blockSize );

        byte[] UnSwizzle( byte[] data, int width, int height, int blockSize );
    }

    public enum SwizzleType
    {
        None,
        PS3,
        PS4,
        Switch
    }
}
