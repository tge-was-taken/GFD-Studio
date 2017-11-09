using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class TextureDictionaryAdapter : ResourceAdapter<TextureDictionary>
    {
        public override MenuFlags ContextMenuFlags => 
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | 
            MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Branch;

        protected internal TextureDictionaryAdapter( string text, TextureDictionary resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<TextureDictionary>( ( path ) =>
            {
                AtlusGfdLib.Resource.Save( Resource, path );
            });
            RegisterReplaceAction<TextureDictionary>( ( path ) =>
            {
                var res = AtlusGfdLib.Resource.Load<Model>( path );
                return res.TextureDictionary ?? Resource;
            });
            RegisterRebuildAction( () =>
            {
                var textureDictionary = new TextureDictionary( Version );
                foreach ( TextureAdapter textureAdapter in Nodes )
                {
                    textureDictionary[textureAdapter.Name] = textureAdapter.Resource;
                }

                return textureDictionary;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Resource.Textures )
            {
                Nodes.Add( TreeNodeAdapterFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
