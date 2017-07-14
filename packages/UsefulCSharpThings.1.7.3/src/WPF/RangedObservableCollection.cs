using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings.WPF;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Adaptation of ObservableCollection to allow ranged operations.
    /// </summary>
    /// <typeparam name="T">Type of content.</typeparam>
    public class RangedObservableCollection<T> : ObservableCollection<T>, IRangedCollection<T>
    {
        /// <summary>
        /// Creates an ObservableCollection that exposes AddRange functionality.
        /// </summary>
        public RangedObservableCollection()
            : base()
        {

        }


        /// <summary>
        /// Creates an ObservableCollection that exposes AddRange functionality.
        /// </summary>
        /// <param name="collection">Enumerable to initialise with.</param>
        public RangedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }


        /// <summary>
        /// Creates an ObservableCollection that exposes AddRange functionality.
        /// </summary>
        /// <param name="list">List to initialise with.</param>
        public RangedObservableCollection(List<T> list)
            : base(list)
        {

        }


        /// <summary>
        /// Adds range of elements.
        /// </summary>
        /// <param name="enumerable">Elements to add.</param>
        public void AddRange(IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Add(item);

            NotifyRangeChange();
        }


        /// <summary>
        /// Remove range of elements.
        /// </summary>
        /// <param name="enumerable">Elements to remove.</param>
        public void RemoveRange(IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Remove(item);

            NotifyRangeChange();
        }


        /// <summary>
        /// Inserts elements at given index.
        /// </summary>
        /// <param name="index">Index to add at.</param>
        /// <param name="enumerable">Elements to add.</param>
        public void InsertRange(int index, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Insert(index, item);

            NotifyRangeChange();
        }


        /// <summary>
        /// Notifications of property changes.
        /// </summary>
        private void NotifyRangeChange()
        {
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }


        /// <summary>
        /// Clears collection and adds elements from enumerable.
        /// </summary>
        /// <param name="enumerable"></param>
        public void Reset(IEnumerable<T> enumerable)
        {
            this.Items.Clear();
            AddRange(enumerable);
        }
    }
}
