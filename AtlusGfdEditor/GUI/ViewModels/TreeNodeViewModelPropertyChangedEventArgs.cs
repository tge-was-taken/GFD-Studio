using System;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class TreeNodeViewModelPropertyChangedEventArgs : EventArgs
    {
        public TreeNodeViewModel TreeNodeViewModel { get; }

        public string PropertyName { get; }

        public TreeNodeViewModelPropertyChangedEventArgs( TreeNodeViewModel treeNodeViewModel, string propertyName )
        {
            TreeNodeViewModel = treeNodeViewModel;
            PropertyName = propertyName;
        }
    }
}