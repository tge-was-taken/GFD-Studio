using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UsefulThings.WPF
{
    #region VirtualizingWrapPanel
    /// <summary>
    /// From: https://uhimaniavwp.codeplex.com/
    /// Seems to work perfectly.
    /// </summary>
    #pragma warning disable CS1591 // Disable warnings about missing XML comments for public things. Don't care about them.
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
    {
        #region ItemSize
        #region ItemWidth
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(
                "ItemWidth",
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                ),
                new ValidateValueCallback(VirtualizingWrapPanel.IsWidthHeightValid)
            );

        public double ItemWidth
        {
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        #endregion

        #region ItemHeight
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                "ItemHeight",
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                ),
                new ValidateValueCallback(VirtualizingWrapPanel.IsWidthHeightValid)
            );

        public double ItemHeight
        {
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        #endregion

        #region IsWidthHeightValid
        private static bool IsWidthHeightValid(object value)
        {
            var d = (double)value;
            return double.IsNaN(d) || ((d >= 0) && !double.IsPositiveInfinity(d));
        }
        #endregion

        #endregion

        #region Orientation
        public static readonly DependencyProperty OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner(
                typeof(VirtualizingWrapPanel),
                new FrameworkPropertyMetadata(
                    Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    new PropertyChangedCallback(VirtualizingWrapPanel.OnOrientationChanged)
                )
            );

        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as VirtualizingWrapPanel;
            panel.offset = default(Point);
            panel.InvalidateMeasure();
        }

        #endregion

        #region MeasureOverride, ArrangeOverride
        private Dictionary<int, Rect> containerLayouts = new Dictionary<int, Rect>();

        protected override Size MeasureOverride(Size availableSize)
        {
            this.containerLayouts.Clear();

            var isAutoWidth = double.IsNaN(this.ItemWidth);
            var isAutoHeight = double.IsNaN(this.ItemHeight);
            var childAvailable = new Size(isAutoWidth ? double.PositiveInfinity : this.ItemWidth, isAutoHeight ? double.PositiveInfinity : this.ItemHeight);
            var isHorizontal = this.Orientation == Orientation.Horizontal;

            var childrenCount = this.InternalChildren.Count;

            var itemsControl = ItemsControl.GetItemsOwner(this);
            if (itemsControl != null)
                childrenCount = itemsControl.Items.Count;

            var generator = new ChildGenerator(this);

            var x = 0.0;
            var y = 0.0;
            var lineSize = default(Size);
            var maxSize = default(Size);

            for (int i = 0; i < childrenCount; i++)
            {
                var childSize = this.ContainerSizeForIndex(i);

                var isWrapped = isHorizontal ?
                    lineSize.Width + childSize.Width > availableSize.Width :
                    lineSize.Height + childSize.Height > availableSize.Height;
                if (isWrapped)
                {
                    x = isHorizontal ? 0 : x + lineSize.Width;
                    y = isHorizontal ? y + lineSize.Height : 0;
                }

                var itemRect = new Rect(x, y, childSize.Width, childSize.Height);
                var viewportRect = new Rect(this.offset, availableSize);
                if (itemRect.IntersectsWith(viewportRect))
                {
                    var child = generator.GetOrCreateChild(i);
                    child.Measure(childAvailable);
                    childSize = this.ContainerSizeForIndex(i);
                }

                this.containerLayouts[i] = new Rect(x, y, childSize.Width, childSize.Height);

                isWrapped = isHorizontal ?
                    lineSize.Width + childSize.Width > availableSize.Width :
                    lineSize.Height + childSize.Height > availableSize.Height;
                if (isWrapped)
                {
                    maxSize.Width = isHorizontal ? Math.Max(lineSize.Width, maxSize.Width) : maxSize.Width + lineSize.Width;
                    maxSize.Height = isHorizontal ? maxSize.Height + lineSize.Height : Math.Max(lineSize.Height, maxSize.Height);
                    lineSize = childSize;

                    isWrapped = isHorizontal ?
                        childSize.Width > availableSize.Width :
                        childSize.Height > availableSize.Height;
                    if (isWrapped)
                    {
                        maxSize.Width = isHorizontal ? Math.Max(childSize.Width, maxSize.Width) : maxSize.Width + childSize.Width;
                        maxSize.Height = isHorizontal ? maxSize.Height + childSize.Height : Math.Max(childSize.Height, maxSize.Height);
                        lineSize = default(Size);
                    }
                }
                else
                {
                    lineSize.Width = isHorizontal ? lineSize.Width + childSize.Width : Math.Max(childSize.Width, lineSize.Width);
                    lineSize.Height = isHorizontal ? Math.Max(childSize.Height, lineSize.Height) : lineSize.Height + childSize.Height;
                }

                x = isHorizontal ? lineSize.Width : maxSize.Width;
                y = isHorizontal ? maxSize.Height : lineSize.Height;
            }

            maxSize.Width = isHorizontal ? Math.Max(lineSize.Width, maxSize.Width) : maxSize.Width + lineSize.Width;
            maxSize.Height = isHorizontal ? maxSize.Height + lineSize.Height : Math.Max(lineSize.Height, maxSize.Height);

            this.extent = maxSize;
            this.viewport = availableSize;

            generator.CleanupChildren();
            generator.Dispose();

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            return maxSize;
        }

        /// <summary>
        /// Brings item into view, works with virtualisation.
        /// </summary>
        /// <param name="item">Item to bring into view.</param>
        public void BringItemIntoView(object item)
        {
            var itemsControl = ItemsControl.GetItemsOwner(this);
            int index = itemsControl.Items.IndexOf(item);

            var testing = InternalChildren;

            if (index < 0)
                return;

            if (!this.containerLayouts.ContainsKey(index))
                return;

            var layout = this.containerLayouts[index];

            if (this.HorizontalOffset + this.ViewportWidth < layout.X + layout.Width)
                this.SetHorizontalOffset(layout.X + layout.Width - this.ViewportWidth);
            if (layout.X < this.HorizontalOffset)
                this.SetHorizontalOffset(layout.X);

            if (this.VerticalOffset + this.ViewportHeight < layout.Y + layout.Height)
                this.SetVerticalOffset(layout.Y + layout.Height - this.ViewportHeight);
            if (layout.Y < this.VerticalOffset)
                this.SetVerticalOffset(layout.Y);

            layout.Width = Math.Min(this.ViewportWidth, layout.Width);
            layout.Height = Math.Min(this.ViewportHeight, layout.Height);
        }

        #region ChildGenerator
        private class ChildGenerator : IDisposable
        {
            #region fields
            private VirtualizingWrapPanel owner;
            private IItemContainerGenerator generator;
            private IDisposable generatorTracker;
            private int firstGeneratedIndex;
            private int lastGeneratedIndex;
            private int currentGenerateIndex;

            #endregion

            #region _ctor

            public ChildGenerator(VirtualizingWrapPanel owner)
            {
                this.owner = owner;

                var childrenCount = owner.InternalChildren.Count;
                this.generator = owner.ItemContainerGenerator;
            }

            ~ChildGenerator()
            {
                this.Dispose();
            }

            public void Dispose()
            {
                if (this.generatorTracker != null)
                    this.generatorTracker.Dispose();
            }

            #endregion

            #region GetOrCreateChild
            private void BeginGenerate(int index)
            {
                this.firstGeneratedIndex = index;
                var startPos = this.generator.GeneratorPositionFromIndex(index);
                this.currentGenerateIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
                this.generatorTracker = this.generator.StartAt(startPos, GeneratorDirection.Forward, true);
            }

            public UIElement GetOrCreateChild(int index)
            {
                if (this.generator == null)
                    return this.owner.InternalChildren[index];

                if (this.generatorTracker == null)
                    this.BeginGenerate(index);

                bool newlyRealized;
                var child = this.generator.GenerateNext(out newlyRealized) as UIElement;
                if (newlyRealized)
                {
                    if (this.currentGenerateIndex >= this.owner.InternalChildren.Count)
                        this.owner.AddInternalChild(child);
                    else
                        this.owner.InsertInternalChild(this.currentGenerateIndex, child);

                    this.generator.PrepareItemContainer(child);
                }

                this.lastGeneratedIndex = index;
                this.currentGenerateIndex++;

                return child;
            }

            #endregion

            #region CleanupChildren
            public void CleanupChildren()
            {
                if (this.generator == null)
                    return;

                var children = this.owner.InternalChildren;

                for (int i = children.Count - 1; i >= 0; i--)
                {
                    var childPos = new GeneratorPosition(i, 0);
                    var index = generator.IndexFromGeneratorPosition(childPos);
                    if (index < this.firstGeneratedIndex || index > this.lastGeneratedIndex)
                    {
                        this.generator.Remove(childPos, 1);
                        this.owner.RemoveInternalChildRange(i, 1);
                    }
                }
            }
            #endregion
        }
        #endregion

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in this.InternalChildren)
            {
                var gen = this.ItemContainerGenerator as ItemContainerGenerator;
                var index = (gen != null) ? gen.IndexFromContainer(child) : this.InternalChildren.IndexOf(child);
                if (this.containerLayouts.ContainsKey(index))
                {
                    var layout = this.containerLayouts[index];
                    layout.Offset(this.offset.X * -1, this.offset.Y * -1);
                    child.Arrange(layout);
                }
            }

            return finalSize;
        }

        #endregion

        #region ContainerSizeForIndex

        private Size prevSize = new Size(16, 16);
        private Size ContainerSizeForIndex(int index)
        {
            var getSize = new Func<int, Size>(idx =>
            {
                UIElement item = null;
                var itemsOwner = ItemsControl.GetItemsOwner(this);
                var generator = this.ItemContainerGenerator as ItemContainerGenerator;

                if (itemsOwner == null || generator == null)
                {
                    if (this.InternalChildren.Count > idx)
                        item = this.InternalChildren[idx];
                }
                else
                {
                    if (generator.ContainerFromIndex(idx) != null)
                        item = generator.ContainerFromIndex(idx) as UIElement;
                    else if (itemsOwner.Items.Count > idx)
                        item = itemsOwner.Items[idx] as UIElement;
                }

                if (item != null)
                {
                    if (item.IsMeasureValid)
                        return item.DesiredSize;

                    var i = item as FrameworkElement;
                    if (i != null)
                        return new Size(i.Width, i.Height);
                }

                if (this.containerLayouts.ContainsKey(idx))
                    return this.containerLayouts[idx].Size;

                return this.prevSize;
            });

            var size = getSize(index);

            if (!double.IsNaN(this.ItemWidth))
                size.Width = this.ItemWidth;
            if (!double.IsNaN(this.ItemHeight))
                size.Height = this.ItemHeight;

            return this.prevSize = size;
        }

        #endregion

        #region OnItemsChanged
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }
        #endregion

        #region IScrollInfo Members

        #region Extent
        private Size extent = default(Size);
        public double ExtentHeight
        {
            get { return this.extent.Height; }
        }
        public double ExtentWidth
        {
            get { return this.extent.Width; }
        }

        #endregion Extent

        #region Viewport
        private Size viewport = default(Size);
        public double ViewportHeight
        {
            get { return this.viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return this.viewport.Width; }
        }

        #endregion

        #region Offset
        private Point offset;
        public double HorizontalOffset
        {
            get { return this.offset.X; }
        }

        public double VerticalOffset
        {
            get { return this.offset.Y; }
        }

        #endregion

        #region ScrollOwner
        public ScrollViewer ScrollOwner { get; set; }
        #endregion

        #region CanHorizontallyScroll
        public bool CanHorizontallyScroll { get; set; }
        #endregion

        #region CanVerticallyScroll
        public bool CanVerticallyScroll { get; set; }
        #endregion

        #region LineUp
        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - SystemParameters.ScrollHeight);
        }
        #endregion

        #region LineDown
        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + SystemParameters.ScrollHeight);
        }
        #endregion

        #region LineLeft
        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - SystemParameters.ScrollWidth);
        }
        #endregion

        #region LineRight
        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + SystemParameters.ScrollWidth);
        }
        #endregion

        #region PageUp
        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.viewport.Height);
        }
        #endregion

        #region PageDown
        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.viewport.Height);
        }
        #endregion

        #region PageLeft
        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.viewport.Width);
        }
        #endregion

        #region PageRight
        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.viewport.Width);
        }
        #endregion

        #region MouseWheelUp
        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - SystemParameters.ScrollHeight * SystemParameters.WheelScrollLines);
        }
        #endregion

        #region MouseWheelDown
        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + SystemParameters.ScrollHeight * SystemParameters.WheelScrollLines);
        }
        #endregion

        #region MouseWheelLeft
        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - SystemParameters.ScrollWidth * SystemParameters.WheelScrollLines);
        }
        #endregion

        #region MouseWheelRight
        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + SystemParameters.ScrollWidth * SystemParameters.WheelScrollLines);
        }
        #endregion

        #region MakeVisible
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            var idx = this.InternalChildren.IndexOf(visual as UIElement);

            var generator = this.ItemContainerGenerator as IItemContainerGenerator;
            if (generator != null)
            {
                var pos = new GeneratorPosition(idx, 0);
                idx = generator.IndexFromGeneratorPosition(pos);
            }

            if (idx < 0)
                return Rect.Empty;

            if (!this.containerLayouts.ContainsKey(idx))
                return Rect.Empty;

            var layout = this.containerLayouts[idx];

            if (this.HorizontalOffset + this.ViewportWidth < layout.X + layout.Width)
                this.SetHorizontalOffset(layout.X + layout.Width - this.ViewportWidth);
            if (layout.X < this.HorizontalOffset)
                this.SetHorizontalOffset(layout.X);

            if (this.VerticalOffset + this.ViewportHeight < layout.Y + layout.Height)
                this.SetVerticalOffset(layout.Y + layout.Height - this.ViewportHeight);
            if (layout.Y < this.VerticalOffset)
                this.SetVerticalOffset(layout.Y);

            layout.Width = Math.Min(this.ViewportWidth, layout.Width);
            layout.Height = Math.Min(this.ViewportHeight, layout.Height);

            return layout;
        }
        #endregion

        #region SetHorizontalOffset
        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || this.ViewportWidth >= this.ExtentWidth)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportWidth >= this.ExtentWidth)
                    offset = this.ExtentWidth - this.ViewportWidth;
            }

            this.offset.X = offset;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.InvalidateMeasure();
        }
        #endregion

        #region SetVerticalOffset
        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || this.ViewportHeight >= this.ExtentHeight)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportHeight >= this.ExtentHeight)
                    offset = this.ExtentHeight - this.ViewportHeight;
            }

            this.offset.Y = offset;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.InvalidateMeasure();
        }
        #endregion

        #endregion
    }
    #endregion
}