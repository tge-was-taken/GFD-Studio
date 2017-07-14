using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace UsefulThings.WPF
{
    /// <summary>
    /// This behaviour ensures that a selected node (from code-behind) is brought into view.
    /// It requires a single property in the back end for the tree's currently selected item. Can't just bind IsSelected to every item it seems.
    /// Beautiful. Sorry I can't remember where it came from.
    /// </summary>
    #pragma warning disable CS1591 // Disable warnings about missing XML comments for public things. Don't care about them.
    public class TreeViewNodeSelectionBehaviour : Behavior<TreeView>
    {
        public ITreeSeekable SelectedItem
        {
            get { return (ITreeSeekable)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ITreeSeekable), typeof(TreeViewNodeSelectionBehaviour),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newNode = e.NewValue as ITreeSeekable;
            if (newNode == null) return;
            var behavior = (TreeViewNodeSelectionBehaviour)d;
            var tree = behavior.AssociatedObject;

            var nodeDynasty = new List<ITreeSeekable> { newNode };
            var parent = newNode.Parent;
            while (parent != null)
            {
                nodeDynasty.Insert(0, parent);
                parent = parent.Parent;
            }

            var currentParent = tree as ItemsControl;
            foreach (var ITreeSeekable in nodeDynasty)
            {
                // first try the easy way
                var newParent = currentParent.ItemContainerGenerator.ContainerFromItem(ITreeSeekable) as TreeViewItem;
                if (newParent == null)
                {
                    // if this failed, it's probably because of virtualization, and we will have to do it the hard way.
                    // this code is influenced by TreeViewItem.ExpandRecursive decompiled code, and the MSDN sample at http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
                    // see also the question at http://stackoverflow.com/q/183636/46635
                    currentParent.ApplyTemplate();
                    var itemsPresenter = (ItemsPresenter)currentParent.Template.FindName("ItemsHost", currentParent);
                    if (itemsPresenter != null)
                    {
                        itemsPresenter.ApplyTemplate();
                    }
                    else
                    {
                        currentParent.UpdateLayout();
                    }

                    var virtualizingPanel = GetItemsHost(currentParent) as VirtualizingPanel;
                    CallEnsureGenerator(virtualizingPanel);
                    var index = currentParent.Items.IndexOf(ITreeSeekable);
                    if (index < 0)
                    {
                        throw new InvalidOperationException("ITreeSeekable '" + ITreeSeekable + "' cannot be fount in container");
                    }
                    virtualizingPanel.BringIndexIntoViewPublic(index);
                    newParent = currentParent.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                }

                if (newParent == null)
                {
                    throw new InvalidOperationException("Tree view item cannot be found or created for ITreeSeekable '" + ITreeSeekable + "'");
                }

                if (ITreeSeekable == newNode)
                {
                    newParent.IsSelected = true;
                    newParent.BringIntoView();
                    break;
                }

                newParent.IsExpanded = true;
                currentParent = newParent;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue as ITreeSeekable;
        }

        #region Functions to get internal members using reflection

        // Some functionality we need is hidden in internal members, so we use reflection to get them

        #region ItemsControl.ItemsHost

        static readonly PropertyInfo ItemsHostPropertyInfo = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            Debug.Assert(itemsControl != null);
            return ItemsHostPropertyInfo.GetValue(itemsControl, null) as Panel;
        }

        #endregion ItemsControl.ItemsHost

        #region Panel.EnsureGenerator

        private static readonly MethodInfo EnsureGeneratorMethodInfo = typeof(Panel).GetMethod("EnsureGenerator", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void CallEnsureGenerator(Panel panel)
        {
            Debug.Assert(panel != null);
            EnsureGeneratorMethodInfo.Invoke(panel, null);
        }

        #endregion Panel.EnsureGenerator
        #endregion Functions to get internal members using reflection
    }
}
