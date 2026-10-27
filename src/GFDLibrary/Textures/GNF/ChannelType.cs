namespace GFDLibrary.Textures.GNF
{
    public enum ChannelType
    {
        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float X/(N-1)</c>.</summary>
        UNorm = 0x00000000,
        /// <summary>Stored as <c>int -N/2\&lt;=X\N/2, interpreted as <c>float MAX(-1,X/(N/2-1))</c>.</summary>
        SNorm = 0x00000001,
        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float X</c>.</summary>
        UScaled = 0x00000002,
        /// <summary>Stored as <c>int -N/2\&lt=X\N/2, interpreted as <c>float X</c>.</summary>
        SScaled = 0x00000003,
        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>uint X</c>. Not filterable.</summary>
        UInt = 0x00000004,
        /// <summary>Stored as <c>int -N/2\&lt=X\N/2, interpreted as <c>int X</c>. Not filterable.</summary>
        SInt = 0x00000005,
        /// <summary>Stored as <c>int -N/2&lt=X\N/2, interpreted as <c>float ((X+N/2)/(N-1))*2-1</c>.</summary>
        SNormNoZero = 0x00000006,
        /// <summary>Stored as <c>float</c>, interpreted as <c>float</c>.
        /// – 32-bit: SE8M23, bias 127, range <c>(-2^129..2^129)
        /// – 16-bit: SE5M10, bias 15, range <c>(-2^17..2^17
        /// – 11-bit: E5M6 bias 15, range <c>[0..2^17)
        /// – 10-bit: E5M5 bias 15, range <c>[0..2^17)
        /// </summary>
        Float = 0x00000007,

        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float sRGB(X/(N-1))</c>. Srgb only applies to the XYZ channels of the texture; the W channel is always linear.</summary>
        Srgb = 0x00000009,

        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float MAX(-1,(X-N/2)/(N/2-1))</c>.</summary>
        UBNorm = 0x0000000A,

        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float (X/(N-1))*2-1</c>.</summary>
        UBNormNoZero = 0x0000000B,

        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>int X-N/2</c>. Not blendable or filterable.</summary>
        UBInt = 0x0000000C,

        /// <summary>Stored as <c>uint X\<N</c>, interpreted as <c>float X-N/2</c></summary>
        UBScaled = 0x0000000D,
    }
}