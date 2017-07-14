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
    /// Adaptation of Multithreaded ObservableCollection to allow range operations.
    /// </summary>
    /// <typeparam name="T">Type of content.</typeparam>
    public class MTRangedObservableCollection<T> : MTObservableCollection<T>, IRangedCollection<T>
    {
        /// <summary>
        /// Creates a multi-threaded ObservableCollection with range modification capabilities.
        /// Allows changes from other threads, and provides AddRange functionality.
        /// </summary>
        public MTRangedObservableCollection()
            : base()
        {
            
        }

        /// <summary>
        /// Creates a multi-threaded ObservableCollection with range modification capabilities.
        /// Allows changes from other threads, and provides AddRange functionality.
        /// </summary>
        /// <param name="collection">Enumerable to initialise with.</param>
        public MTRangedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }


        /// <summary>
        /// Creates a multi-threaded ObservableCollection with range modification capabilities.
        /// Allows changes from other threads, and provides AddRange functionality.
        /// </summary>
        /// <param name="list">List to initialise with.</param>
        public MTRangedObservableCollection(List<T> list)
            : base(list)
        {
            
        }


        /// <summary>
        /// Adds range of elements from IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable of elements to add.</param>
        public void AddRange(IEnumerable<T> enumerable)
        {
            // Adds items to underlying collection.
            lock (locker)
                foreach (T item in enumerable)
                    Items.Add(item);

            NotifyRangeChange();
        }

        /// <summary>
        /// Adds range of elements from List.
        /// </summary>
        /// <param name="list">List of elements to add.</param>
        public void AddRange(IList<T> list)
        {
            // Adds items to underlying collection.
            lock (locker)
                foreach (T item in list)
                    this.Items.Add(item);

            NotifyRangeChange();
        }

        /// <summary>
        /// Inserts elements at given index.
        /// </summary>
        /// <param name="index">Index to add at.</param>
        /// <param name="enumerable">Elements to add.</param>
        public void InsertRange(int index, IEnumerable<T> enumerable)
        {
            lock (locker)
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
            lock (locker)
            {
                this.Items.Clear();
                AddRange(enumerable);
            }
        }


        /// <summary>
        /// Removes range of items.
        /// </summary>
        /// <param name="enumerable">Items to remove.</param>
        public void RemoveRange(IEnumerable<T> enumerable)
        {
            lock (locker)
                foreach (T item in enumerable)
                    this.Items.Remove(item);

            NotifyRangeChange();
        }
    }
}
