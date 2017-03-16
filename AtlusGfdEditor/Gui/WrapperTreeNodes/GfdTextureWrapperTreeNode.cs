using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.GfdLib;
using System.ComponentModel;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdTextureWrapperTreeNode : BaseWrapperTreeNode
    {
        [Browsable(false)]
        public new string Text
        {
            get { return GetWrappedObject<GfdTexture>().Name; }
            set
            {
                base.Name = value;
                base.Text = value;
                GetWrappedObject<GfdTexture>().Name = new GfdString(value);
            }
        }

        [Category("Texture properties"), Description("The internal format of the texture data")]
        public GfdTextureFormat Format
        {
            get { return GetWrappedObject<GfdTexture>().Format; }
        }

        internal GfdTextureWrapperTreeNode(GfdTexture texture)
            : base(ContextMenuStripFlags.Delete | ContextMenuStripFlags.Export | 
                   ContextMenuStripFlags.Move | ContextMenuStripFlags.Rename | ContextMenuStripFlags.Replace, texture)
        {
            Text = (string)texture.Name;
        }
    }
}