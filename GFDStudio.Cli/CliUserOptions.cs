using GFDStudio.GUI;

namespace GFDStudio.Cli
{
    /// <summary>
    /// The toggles the view nodes consult, taken from the command line rather than from a saved
    /// configuration, so that a run depends on nothing but what it was asked to do.
    /// </summary>
    public sealed class CliUserOptions : IUserOptions
    {
        public bool RetainMaterialTextureNames { get; set; }

        public bool RetainMaterialColorValues { get; set; }

        /// <summary>
        /// Always off: writing a replaced texture beside the model surprises a script that asked
        /// for one output file.
        /// </summary>
        public bool SaveReplacedTexturesExternally => false;
    }
}
