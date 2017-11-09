using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class StreamAdapter : TreeNodeAdapter<Stream>
    {
        public override MenuFlags ContextMenuFlags => 
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | 
            MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Leaf;

        protected internal StreamAdapter( string text, Stream resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Stream>( ( path ) =>
            {
                using ( var fileStream = File.Create( path ) )
                {
                    Resource.Position = 0;
                    Resource.CopyTo( fileStream );
                }
            } );

            RegisterReplaceAction<Stream>( File.OpenRead );
        }
    }
}
