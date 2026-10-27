namespace GFDLibrary.Conversion;

public class AnimationConverterOptions
{
    /// <summary>
    /// Gets or sets the version to use for the converted resources.
    /// </summary>
    public uint Version { get; set; }

    public AnimationConverterOptions()
    {
        Version = ResourceVersion.Persona5;
    }
}
