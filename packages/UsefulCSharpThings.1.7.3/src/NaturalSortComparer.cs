using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// Compares strings using Natural Sort i.e. 1, 5, 12, 15, instead of 12, 15, 1, 2
    /// </summary>
    public class NaturalSortComparer : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int StrCmpLogicalW(string x, string y);
        /// <summary>
        /// Compares strings using Natural Sort i.e. 1, 5, 12, 15, instead of 12, 15, 1, 2
        /// </summary>
        /// <param name="x">First string.</param>
        /// <param name="y">Second string.</param>
        /// <returns>Positive means x is before y, negative is opposite.</returns>
        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
