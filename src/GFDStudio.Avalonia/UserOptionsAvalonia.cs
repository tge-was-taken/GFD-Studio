using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The toggles the user sets from the Options menu, kept in the same config file the WinForms
    /// front end used.
    /// </summary>
    public sealed class UserOptionsAvalonia : IUserOptions
    {
        public static Config Settings { get; } = new Config().LoadJson();

        public bool RetainMaterialTextureNames => Settings.RetainTextureNames;

        public bool RetainMaterialColorValues => Settings.RetainMaterialColors;

        public bool SaveReplacedTexturesExternally => Settings.SaveReplacedTexturesExternally;
    }
}
