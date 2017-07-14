using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsefulThings.WinForms
{
    /// <summary>
    /// Provides threadsafe methods for changing ToolStripProgressBars, incl Incrementing and setting Value and Maximum properties.
    /// </summary>
    public class ProgressBarChanger
    {
        // KFreon: Strip object for invoking on correct thread
        ToolStrip Strip = null;
        ToolStripProgressBar Progbar = null;


        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="strip">Base strip object for correct invoking.</param>
        /// <param name="progbar">ProgressBar to be targeted.</param>
        public ProgressBarChanger(ToolStrip strip, ToolStripProgressBar progbar)
        {
            Strip = strip;
            Progbar = progbar;
        }


        /// <summary>
        /// Increments targeted ProgressBar.
        /// </summary>
        /// <param name="amount">Optional. Amount to increment bar by. Defaults to 1.</param>
        public void IncrementBar(int amount = 1)
        {
            if (Strip.InvokeRequired)
                Strip.BeginInvoke(new Action(() => Progbar.Increment(amount)));
            else
                Progbar.Increment(amount);
        }


        /// <summary>
        /// Sets Value and Maximum properties of targeted ProgressBar.
        /// </summary>
        /// <param name="start">Value to set Value property to. i.e. Current value.</param>
        /// <param name="end">Value to set Maximum property to. i.e. Number of increments in bar.</param>
        public void ChangeProgressBar(int start, int end = -1)
        {
            if (Strip.InvokeRequired)
                Strip.BeginInvoke(new Action(() => ChangeProgressBar(start, end)));
            else
            {
                Progbar.Maximum = (end == -1) ? Progbar.Maximum : end;
                Progbar.Value = start;
            }
        }
    }
}
