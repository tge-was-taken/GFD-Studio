using System.Collections.ObjectModel;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdTexture 
    {
        public GfdString Name { get; set; }
        public GfdTextureFormat Format { get; }
        public byte[] Data { get; }

        public GfdTexture(GfdString name, GfdTextureFormat format, byte[] data)
        {
            Name = name;
            Format = format;
            Data = data;
        }
    }
}