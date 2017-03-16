using System.ComponentModel;
using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.Framework.Gui.PropertyGrid;
using AtlusGfdEditor.GfdLib;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    abstract class GfdResourceWrapperTreeNode : BaseWrapperTreeNode
    {
        [Category("Resource properties"), Description("Version of resource"), TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Version
        {
            get { return GetWrappedObject<GfdResource>().Version; }
        }

        internal GfdResourceWrapperTreeNode(ContextMenuStripFlags flags, GfdResource resource)
            : base(flags)
        {
            // Set text & name
            Text = Name = nameof(GfdResource);

            // Set tag
            Tag = resource;
        }
    }
}
