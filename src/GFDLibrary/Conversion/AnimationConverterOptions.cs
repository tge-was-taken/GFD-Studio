namespace GFDLibrary.Conversion;

public class AnimationConverterOptions
{
    public static bool CompressKeyframesDefault { get; set; } = false;

    /// <summary>
    /// Gets or sets the version to use for the converted resources.
    /// </summary>
    public uint Version { get; set; }

    public bool UseCompressedKeyframes { get; set; }

    public AnimationConverterOptions()
    {
        Version = ResourceVersion.Persona5;
        UseCompressedKeyframes = CompressKeyframesDefault;
    }
}
