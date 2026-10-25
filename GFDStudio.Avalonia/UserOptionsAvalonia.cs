using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The toggles the user sets from the Options menu.
    /// </summary>
    public sealed class UserOptionsAvalonia : IUserOptions
    {
        public bool RetainMaterialTextureNames { get; set; } = true;

        public bool RetainMaterialColorValues { get; set; }

        public bool SaveReplacedTexturesExternally { get; set; } = true;
    }
}
