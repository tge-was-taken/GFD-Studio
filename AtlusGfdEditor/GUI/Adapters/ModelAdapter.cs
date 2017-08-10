using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class ModelAdapter : ResourceAdapter<Model>
    {
        public override MenuFlags ContextMenuFlags =>
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Add |
            MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Branch;

        [Browsable( false )]
        public TextureDictionaryAdapter TextureDictionary { get; set; }

        protected internal ModelAdapter( string text, Model resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Model>( ( path ) => AtlusGfdLib.Resource.Save(Resource, path) );
            RegisterReplaceAction<Model>( ( path ) => AtlusGfdLib.Resource.Load<Model>( path ) );
            RegisterRebuildAction( () =>
            {
                var model = new Model( Version );

                model.TextureDictionary = TextureDictionary.Resource;
                model.MaterialDictionary = Resource.MaterialDictionary;
                model.Scene = Resource.Scene;
                model.AnimationPackage = Resource.AnimationPackage;
                model.ChunkType000100F9 = Resource.ChunkType000100F9;

                return model;
            });

            TextureDictionary = ( TextureDictionaryAdapter )TreeNodeAdapterFactory.Create( "Textures", Resource.TextureDictionary );
        }

        protected override void InitializeViewCore()
        {
            Nodes.Add( TextureDictionary );
        }
    }
}
