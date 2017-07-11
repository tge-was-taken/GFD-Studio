using System.Collections.ObjectModel;

namespace AtlusGfdLib
{
    public sealed class Texture 
    {
        public string Name { get; set; }

        public TextureFormat Format { get; set; }

        public byte[] Data { get; set; }

        // 1C
        public byte Field1C { get; set; }

        // 1D
        public byte Field1D { get; set; }

        // 1E
        public byte Field1E { get; set; }

        // 1F
        public byte Field1F { get; set; }

        public Texture(string name, TextureFormat format, byte[] data)
        {
            Name = name;
            Format = format;
            Data = data;
            Field1C = 1;
            Field1D = 1;
            Field1E = 0;
            Field1F = 0;
        }

        internal Texture() { }

        public override string ToString()
        {
            return Name;
        }
    }
}