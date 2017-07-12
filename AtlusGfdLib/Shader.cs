using System.Collections.ObjectModel;

namespace AtlusGfdLib
{
    public sealed class Shader
    {
        public ushort Type { get; }

        public ushort Field06 { get; }

        public uint Field08 { get; }

        public uint Field0C { get; }

        public uint Field10 { get; }

        public float Field14 { get; }

        public float Field18 { get; }

        public byte[] Data { get; }

        public Shader(ushort type, ushort field06, uint field08, uint field0C, uint field10, float field14, float field18, byte[] data)
        {
            Type = type;
            Field06 = field06;
            Field08 = field08;
            Field0C = field0C;
            Field10 = field10;
            Field14 = field14;
            Field18 = field18;
            Data = data;
        }

        internal Shader()
        {
        }
    }
}