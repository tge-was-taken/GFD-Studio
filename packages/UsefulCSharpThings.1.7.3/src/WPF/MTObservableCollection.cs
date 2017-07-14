using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Multithreaded version of ObservableCollection. Not mine.
    /// </summary>
    /// <typeparam name="T">Type of content.</typeparam>
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Lock/sync object used when doing multi-threaded things.
        /// </summary>
        protected readonly object locker = new object();

        /// <summary>
        /// Event handling things - not mine.
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Event handling things - not mine.
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }

        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        /// <param name="collection">Enumerable to initialise with.</param>
        public MTObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }


        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        /// <param name="list">List to initialise with.</param>
        public MTObservableCollection(List<T> list)
            : base(list)
        {

        }


        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        public MTObservableCollection()
            : base()
        {

        }

        /// <summary>
        /// Multi-threaded Add.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public new void Add(T item)
        {
            lock (locker)
                base.Add(item);
        }

        /// <summary>
        /// Multi-threaded Clear.
        /// </summary>
        public new void Clear()
        {
            lock (locker)
                base.Clear();
        }
    }
}
