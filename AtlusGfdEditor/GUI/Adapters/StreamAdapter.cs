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
        protected internal StreamAdapter( string text, Stream resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            ContextMenuOptions = ContextMenuOptions.Export | ContextMenuOptions.Replace |
                                 ContextMenuOptions.Move | ContextMenuOptions.Rename | ContextMenuOptions.Delete;

            RegisterExportAction<Stream>( ( path ) =>
            {
                using ( var fileStream = File.Create( path ) )
                {
                    Resource.Position = 0;
                    Resource.CopyTo( fileStream );
                }
            } );

            RegisterReplaceAction<Stream>( ( path ) =>
            {
                Resource = File.OpenRead( path );
            } );
        }

        protected override void InitializeViewCore()
        {
        }

        protected override Stream RebuildCore()
        {
            return Resource;
        }
    }
}
