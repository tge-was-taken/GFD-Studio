using GFDStudio.GUI.Forms;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// Reads the toggles from the main form's menu, where the user sets them.
    /// </summary>
    public sealed class DataViewNodeOptionsWinForms : IDataViewNodeOptions
    {
        public bool RetainMaterialTextureNames => MainForm.Instance.retainTexNameToolStripMenuItem.Checked;

        public bool RetainMaterialColorValues => MainForm.Instance.retainColorValuesToolStripMenuItem.Checked;
    }
}
