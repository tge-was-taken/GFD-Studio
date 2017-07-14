using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides AddRange, InsertRange, and Reset functionality for collections.
    /// Mostly used for ObservableCollections, which don't implement these by default.
    /// </summary>
    /// <typeparam name="T">Type of collection content.</typeparam>
    public interface IRangedCollection<T>
    {
        /// <summary>
        /// Adds a range of elements to a list.
        /// </summary>
        /// <param name="enumerable">Contents to add.</param>
        void AddRange(IEnumerable<T> enumerable);


        /// <summary>
        /// Removes a range of elements from a list.
        /// </summary>
        /// <param name="enumerable">Elements to remove.</param>
        void RemoveRange(IEnumerable<T> enumerable);

        /// <summary>
        /// Inserts a range of elements into a list at the specified index.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="enumerable">Elements to insert.</param>
        void InsertRange(int index, IEnumerable<T> enumerable);

        /// <summary>
        /// Clears list and adds elements.
        /// </summary>
        /// <param name="enumerable">Elements to add.</param>
        void Reset(IEnumerable<T> enumerable);
    }
}
