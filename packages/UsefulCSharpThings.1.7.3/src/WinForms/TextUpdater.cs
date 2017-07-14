using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsefulThings.WinForms
{
    /// <summary>
    /// Provides threadsafe methods to update text of a ToolStripItem's Text property. 
    /// </summary>
    public class TextUpdater
    {
        Control control = null;
        ToolStrip strip = null;
        ToolStripItem item = null;

        /// <summary>
        /// Provides multi-threaded access to Text Controls.
        /// Allows changing of the Text property on a given Control.
        /// </summary>
        /// <param name="givenControl">Control to alter.</param>
        public TextUpdater(Control givenControl)
        {
            control = givenControl;
        }


        /// <summary>
        /// Provides multi-threaded access to ToolStrip Text Controls.
        /// Allows changing of the Text property on a given Control.
        /// </summary>
        /// <param name="givenControl">Control to monitor.</param>
        /// <param name="givenStrip">Base strip to correctly invoke with.</param>
        public TextUpdater(ToolStripItem givenControl, ToolStrip givenStrip)
        {
            strip = givenStrip;
            item = givenControl;
        }


        /// <summary>
        /// Updates text of targeted text property.
        /// </summary>
        /// <param name="text">New text to display.</param>
        public void UpdateText(string text)
        {
            // KFreon: Check which control to update
            if (control == null)
            {
                if (strip.InvokeRequired)
                    strip.BeginInvoke(new Action(() => UpdateText(text)));
                else
                    item.Text = text;
            }
            else
            {
                if (control.InvokeRequired)
                    control.BeginInvoke(new Action(() => control.Text = text));
                else
                    control.Text = text;
            }
        }
    }
}
