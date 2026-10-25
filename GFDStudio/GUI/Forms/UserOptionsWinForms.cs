namespace GFDStudio.GUI.Forms
{
    /// <summary>
    /// Reads the toggles from the main form's menu, where the user sets them.
    /// </summary>
    public sealed class UserOptionsWinForms : IUserOptions
    {
        public bool RetainMaterialTextureNames => MainForm.Instance.retainTexNameToolStripMenuItem.Checked;

        public bool RetainMaterialColorValues => MainForm.Instance.retainColorValuesToolStripMenuItem.Checked;
    }
}
