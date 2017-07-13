using System.Collections.ObjectModel;

namespace AtlusGfdLib
{
    public sealed class Shader
    {
        public ushort Type { get; set; }

        public ushort Field06 { get; set; }

        public uint Field08 { get; set; }

        public uint Field0C { get; set; }

        public uint Field10 { get; set; }

        public float Field14 { get; set; }

        public float Field18 { get; set; }

        public byte[] Data { get; set; }

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