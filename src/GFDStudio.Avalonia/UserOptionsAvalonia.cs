using GFDLibrary;
using GFDLibrary.Conversion;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The toggles the user sets from the Options menu, kept in the same config file the WinForms
    /// front end used.
    /// </summary>
    public sealed class UserOptionsAvalonia : IUserOptions
    {
        public static Config Settings { get; } = Load();

        /// <summary>
        /// Whether a file is Catherine Full Body cannot be read from the file, so the saved
        /// answer has to reach the library before anything is opened.
        /// </summary>
        private static Config Load()
        {
            var settings = new Config().LoadJson();
            ResourceVersion.TreatAsCatherineFullBody = settings.TreatAsCatherineFullBody;
            AnimationConverterOptions.CompressKeyframesDefault = settings.CompressKeyframes;

            return settings;
        }

        public bool RetainMaterialTextureNames => Settings.RetainTextureNames;

        public bool RetainMaterialColorValues => Settings.RetainMaterialColors;

        public bool SaveReplacedTexturesExternally => Settings.SaveReplacedTexturesExternally;
    }
}
