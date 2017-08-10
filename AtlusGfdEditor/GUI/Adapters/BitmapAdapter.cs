using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class BitmapAdapter : TreeNodeAdapter<Bitmap>
    {
        public override MenuFlags ContextMenuFlags =>
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Leaf;

        protected internal BitmapAdapter( string text, Bitmap resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Bitmap>( ( path ) => Resource.Save( path ) );
            RegisterReplaceAction<Bitmap>( ( path ) => new Bitmap( path ) );
        }
    }
}
