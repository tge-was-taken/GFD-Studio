namespace GFDLibrary.Shaders
{
    public abstract class ShaderBase
    {
        public ushort Type { get; set; }

        public int DataLength => Data.Length;

        public ushort Field06 { get; set; }

        public uint Field08 { get; set; }

        public uint Field0C { get; set; }

        public uint Field10 { get; set; }

        public uint Field14 { get; set; }

        public uint Field18 { get; set; }

        public byte[] Data { get; set; }
    }

    public sealed class ShaderPS3 : ShaderBase
    {
    }

    public sealed class ShaderPSP2 : ShaderBase
    {
    }
}