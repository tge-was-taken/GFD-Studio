using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// This Collection wraps its index such that if length = 2, and the call is list[6], the collection would return the first element. 
    /// Negative indicies are also supported (maybe)
    /// </summary>
    /// <typeparam name="T">Type of elements in collection</typeparam>
    public class WrappingCollection<T> : ICollection<T>, IList<T>
    {
        List<T> UnderlyingCollection = null;
        
        /// <summary>
        /// Number of elements in collection.
        /// </summary>
        public int Count
        {
            get
            {
                return UnderlyingCollection != null ? UnderlyingCollection.Count : -1;
            }
        }

        /// <summary>
        /// This is always false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// Creates a collection whose index wraps back to the start when exceeded.
        /// </summary>
        public WrappingCollection()
        {
            UnderlyingCollection = new List<T>();
        }


        /// <summary>
        /// Creates a collection whose index wraps back to the start when exceeded.
        /// </summary>
        /// <param name="enumerable">Enumerable to initialise with.</param>
        public WrappingCollection(IEnumerable<T> enumerable) : this()
        {
            UnderlyingCollection.AddRange(enumerable);
        }

        /// <summary>
        /// Creates a collection whose index wraps back to the start when exceeded.
        /// </summary>
        /// <param name="collection">Collection to initialise with.</param>
        public WrappingCollection(ICollection<T> collection) : this()
        {
            UnderlyingCollection.AddRange(collection);
        }

        /// <summary>
        /// Adds item to list.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item)
        {
            UnderlyingCollection.Add(item);
        }

        /// <summary>
        /// Adds range of items to list.
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection)
        {
            UnderlyingCollection.AddRange(collection);
        }

        /// <summary>
        /// Clears list.
        /// </summary>
        public void Clear()
        {
            UnderlyingCollection.Clear();
        }

        /// <summary>
        /// Checks if item is in list.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>true if item is in list, else false.</returns>
        public bool Contains(T item)
        {
            return UnderlyingCollection.Contains(item);
        }


        /// <summary>
        /// Copies contents of list to array at arrayIndex.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Index in array to start copying at.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            UnderlyingCollection.CopyTo(array, arrayIndex);
        }

        
        /// <summary>
        /// Removes item from list.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if success, false otherwise.</returns>
        public bool Remove(T item)
        {
            return UnderlyingCollection.Remove(item);
        }

        
        /// <summary>
        /// Gets enumerator for list.
        /// </summary>
        /// <returns>Enumerator of list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return UnderlyingCollection.GetEnumerator();
        }


        /// <summary>
        /// Gets enumerator for list.
        /// </summary>
        /// <returns>Enumerator of list.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return UnderlyingCollection.GetEnumerator();
        }


        /// <summary>
        /// Returns index of item in list.
        /// </summary>
        /// <param name="item">Item to get index of.</param>
        /// <returns>Index of item in list.</returns>
        public int IndexOf(T item)
        {
            return UnderlyingCollection.IndexOf(item);
        }


        /// <summary>
        /// Inserts item into list at index. Wraps index if required.
        /// </summary>
        /// <param name="index">Index of item to add.</param>
        /// <param name="item">Item to insert.</param>
        public void Insert(int index, T item)
        {
            if (UnderlyingCollection.Count == 0)
                Add(item);
            else
                UnderlyingCollection.Insert(WrapIndex(index), item);
        }


        /// <summary>
        /// Removes item at given index.
        /// </summary>
        /// <param name="index">Index of item to remove.</param>
        public void RemoveAt(int index)
        {
            UnderlyingCollection.RemoveAt(WrapIndex(index));
        }


        /// <summary>
        /// Indexer. Works on item at wrappable index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Item at index</returns>
        public T this[int index]
        {
            get
            {
                return UnderlyingCollection[WrapIndex(index)];
            }
            set
            {
                UnderlyingCollection[WrapIndex(index)] = value;
            }
        }

        /// <summary>
        /// Wraps the index using the mod operator to determine if the index is larger than the list and alters it so it's in range.
        /// </summary>
        /// <param name="index">Index to wrap to list size.</param>
        /// <returns>Index valid for list size.</returns>
        private int WrapIndex(int index)
        {
            if (UnderlyingCollection.Count != 0)
            {
                if (UnderlyingCollection.Count == 1)
                    return 0;

                int retval = index % UnderlyingCollection.Count;  // For some reason -1%5 for example is -1.
                if (index < 0)
                    retval = UnderlyingCollection.Count + (index % UnderlyingCollection.Count);

                return retval;
            }
            else
                return -1;
        }
    }
}
