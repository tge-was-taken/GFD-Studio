namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// The operations a view node needs from whatever is displaying it. Implemented once per UI
    /// framework, so the nodes themselves hold no reference to one.
    /// </summary>
    public interface IDataViewNodeView
    {
        void Select( DataViewNode node );

        void RefreshSelection();

        void ExpandAll();

        void EnsureVisible( DataViewNode node );

        /// <summary>
        /// Starts editing the node's label, calling <see cref="DataViewNode.CommitRename"/> when
        /// the user is done.
        /// </summary>
        void BeginRename( DataViewNode node );

        void NotifyUserPropertyChanged( DataViewNode node, string propertyName );

        /// <summary>
        /// Called when a node's children changed, so the display can catch up.
        /// </summary>
        void OnChildrenChanged( DataViewNode node );
    }
}
