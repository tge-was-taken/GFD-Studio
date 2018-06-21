using System;

namespace GFDStudio.GUI.DataViewNodes
{
    public class DataViewNodePropertyChangedEventArgs : EventArgs
    {
        public DataViewNode Node { get; }

        public string PropertyName { get; }

        public DataViewNodePropertyChangedEventArgs( DataViewNode node, string propertyName )
        {
            Node = node;
            PropertyName = propertyName;
        }
    }
}