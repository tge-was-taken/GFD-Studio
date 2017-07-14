using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides easier access to Dragging in and Out of Windows.
    /// Requires the following Events to be definied: DragOver, Drop, and additionally for dragging out: MouseMove.
    /// </summary>
    /// <typeparam name="DroppedDataContext">Type of object that will perform operations on any dropped items. Often the ViewModel or Context of UIElement (e.g. context of ListBoxItem).</typeparam>
    public class DragDropHandler<DroppedDataContext> where DroppedDataContext : class
    {
        Window BaseWindow = null;
        Window subWindow = null;

        /// <summary>
        /// Action to be performed when items are dropped.
        /// Parameters: DroppedDataContext = Item being dropped on. Does something with the dropped items.
        ///    string[] = List of dropped items (usually file paths). Allows DroppedDataContext to perform some action on them.
        /// </summary>
        public Action<DroppedDataContext, string[]> DropAction { get; set; } = null;

        /// <summary>
        /// Provides a "Gate Keeper" to the DropAction. Validates the dropped files as they're dragged over the target element.
        /// Parameters: string[] = List of files being dragged over.
        /// Returns: True if dragged items are suitable for DropAction - Could provide count restrictions, file extension restrictions, etc.
        /// </summary>
        public Predicate<string[]> DropValidator { get; set; } = null;

        /// <summary>
        /// Optional: Specifies how program item data is retrieved when dragging out to Windows Explorer or another program.
        /// Parameters: DroppedDataContext = Type of item being dragged out - item to get data out of to be dropped outside program.
        ///    Dictionary: Keys = Filenames to save as, Function = Function to get data from DroppedDataContext. Done as a function to improve performance; Lazy evaluation.
        /// </summary>
        public Func<DroppedDataContext, Dictionary<string, Func<byte[]>>> DragOutDataGetter { get; set; } = null;

        /// <summary>
        /// Creates handler for easily dealing with Drop/Drag operations.
        /// </summary>
        /// <param name="baseWindow">Original window to base DPI calculations on.</param>
        public DragDropHandler(Window baseWindow)
        {
            BaseWindow = baseWindow;
        }

        /// <summary>
        /// Provides visual feedback when dragging and dropping.
        /// </summary>
        /// <param name="relative">Window to provide DPI measurement base.</param>
        public void GiveFeedback(Window relative)
        {
            // update the position of the visual feedback item
            var w32Mouse = UsefulThings.General.GetDPIAwareMouseLocation(relative);

            subWindow.Left = w32Mouse.X;
            subWindow.Top = w32Mouse.Y;
        }


        /// <summary>
        /// Performs the Drop action.
        /// </summary>
        /// <param name="sender">UI Container receiving data.</param>
        /// <param name="e">Data container.</param>
        public void Drop(object sender, DragEventArgs e)
        {
            string[] files = ((string[])e.Data.GetData(DataFormats.FileDrop));  // Can't be more than one due to DragEnter and DragOver events
            DroppedDataContext context = null;

            if (sender != null)
                context = (DroppedDataContext)(((FrameworkElement)sender).DataContext);

            DropAction(context, files);

            e.Handled = true;
        }

        /// <summary>
        /// Performs the given action when mouse is moving with a the left button pressed.
        /// </summary>
        /// <param name="sender">UI container.</param>
        /// <param name="e">Mouse event captured</param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var context = item.DataContext as DroppedDataContext;
                if (context == null)
                    return;

                CreateDragDropWindow(item);

                var saveInfo = DragOutDataGetter(context);
                VirtualFileDataObject.FileDescriptor[] files = new VirtualFileDataObject.FileDescriptor[saveInfo.Keys.Count];
                int count = 0;
                foreach (var info in saveInfo)
                    files[count++] = new VirtualFileDataObject.FileDescriptor { Name = info.Key, StreamContents = stream =>
                    {
                        byte[] data = info.Value();
                        stream.Write(data, 0, data.Length);
                    }};


                VirtualFileDataObject obj = new VirtualFileDataObject(() => GiveFeedback(BaseWindow), files);
                VirtualFileDataObject.DoDragDrop(item, obj, DragDropEffects.Copy);
                subWindow.Close();
            }
        }

        /// <summary>
        /// Performs the DragEnter/Over checking of whether the dragged data is supported.
        /// </summary>
        /// <param name="e">Dragged data container.</param>
        public void DragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (DropValidator(files))
                    e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }


        private void CreateDragDropWindow(Visual dragElement)
        {
            subWindow = new Window();
            subWindow.WindowStyle = WindowStyle.None;
            subWindow.AllowsTransparency = true;
            subWindow.AllowDrop = false;
            subWindow.Background = null;
            subWindow.IsHitTestVisible = false;
            subWindow.SizeToContent = SizeToContent.WidthAndHeight;
            subWindow.Topmost = true;
            subWindow.ShowInTaskbar = false;
            subWindow.Opacity = 0.5;

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            subWindow.Content = r;

            var w32Mouse = UsefulThings.General.GetDPIAwareMouseLocation(BaseWindow);

            subWindow.Left = w32Mouse.X;
            subWindow.Top = w32Mouse.Y;
            subWindow.Show();
        }
    }
}
