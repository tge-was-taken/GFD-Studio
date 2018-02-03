using System;
using System.Numerics;

namespace AtlusGfdLibrary.Lights
{
    public sealed class Light
    {
        public LightFlags Flags { get; set; }

        public Vector4 Field30 { get; set; }

        public Vector4 Field40 { get; set; }

        public Vector4 Field50 { get; set; }

        public LightType Type { get; set; }

        public float Field20 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public float Field10 { get; set; }

        public float Field6C { get; set; }

        public float Field70 { get; set; }

        public float Field60 { get; set; }

        public float Field64 { get; set; }

        public float Field68 { get; set; }

        public float Field74 { get; set; }

        public float Field78 { get; set; }
    }

    [Flags]
    public enum LightFlags
    {
        Flag2 = 1 << 1,
    }

    public enum LightType
    {
        Type1 = 1,
        Sky = 2,
        Type3 = 3,
    }
}