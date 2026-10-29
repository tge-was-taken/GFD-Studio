namespace GFDLibrary.Shaders
{
    /// <summary>
    /// The compiled shader on its own, without the flags the cache stores alongside it. Editing
    /// a cache usually means swapping one of these out, so it is exported and imported as a file
    /// in its own right.
    /// </summary>
    public sealed class ShaderBinary
    {
        public byte[] Data { get; }

        public ShaderBinary( byte[] data ) => Data = data;
    }
}
