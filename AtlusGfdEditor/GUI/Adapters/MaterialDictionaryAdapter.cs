using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class MaterialDictionaryAdapter : ResourceAdapter<MaterialDictionary>
    {
        public override MenuFlags ContextMenuFlags =>
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move |
            MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Branch;

        public MaterialDictionaryAdapter( string text, MaterialDictionary resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<MaterialDictionary>( path => AtlusGfdLib.Resource.Save( Resource, path ) );
            RegisterReplaceAction<MaterialDictionary>( AtlusGfdLib.Resource.Load<MaterialDictionary> );

            RegisterRebuildAction( () =>
            {
                var materialDictionary = new MaterialDictionary( Version );
                foreach ( MaterialAdapter adapter in Nodes )
                    materialDictionary[adapter.Name] = adapter.Resource;

                return materialDictionary;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Resource.Materials )
            {
                Nodes.Add( TreeNodeAdapterFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
