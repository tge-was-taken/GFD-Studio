namespace GFDLibrary.Textures.GNF
{
    public enum TextureChannel
    {
        /// <summary> Destination channel will contain a constant 0.0.</summary>
        Constant0 = 0x00000000,
        /// <summary> Destination channel will contain a constant 1.0.</summary>
        Constant1 = 0x00000001,
        /// <summary> Destination channel will contain the source's X channel.</summary>
        X = 0x00000004,
        /// <summary> Destination channel will contain the source's Y channel.</summary>
        Y = 0x00000005,
        /// <summary> Destination channel will contain the source's Z channel.</summary>
        Z = 0x00000006,
        /// <summary> Destination channel will contain the source's W channel.</summary>
        W = 0x00000007,
    }
}