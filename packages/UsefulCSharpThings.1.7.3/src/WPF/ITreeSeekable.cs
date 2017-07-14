using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides TreeView searching functionality.
    /// </summary>
    public interface ITreeSeekable
    {
        /// <summary>
        /// Indicates whether item is open/expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Indicates whether item is selected or not.
        /// </summary>
        bool IsSelected { get; set; }


        /// <summary>
        /// Parent folder to allow for subfolder selection.
        /// </summary>
        ITreeSeekable Parent { get; set; }
    }
}
