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

        [Browsable( false )]
        public MaterialDictionaryAdapter MaterialDictionary { get; set; }

        protected internal ModelAdapter( string text, Model resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Model>( ( path ) => AtlusGfdLib.Resource.Save(Resource, path) );
            RegisterReplaceAction<Model>( AtlusGfdLib.Resource.Load<Model> );
            RegisterRebuildAction( () =>
            {
                var model = new Model( Version )
                {
                    TextureDictionary = TextureDictionary.Resource,
                    MaterialDictionary = Resource.MaterialDictionary,
                    Scene = Resource.Scene,
                    AnimationPackage = Resource.AnimationPackage,
                    ChunkType000100F9 = Resource.ChunkType000100F9
                };


                return model;
            });

            if ( Resource.TextureDictionary != null )
                TextureDictionary = ( TextureDictionaryAdapter )TreeNodeAdapterFactory.Create( "Textures", Resource.TextureDictionary );

            if ( Resource.MaterialDictionary != null )
                MaterialDictionary = ( MaterialDictionaryAdapter )TreeNodeAdapterFactory.Create( "Materials", Resource.MaterialDictionary );
        }

        protected override void InitializeViewCore()
        {
            if ( TextureDictionary != null )
                Nodes.Add( TextureDictionary );

            if ( MaterialDictionary != null )
                Nodes.Add( MaterialDictionary );
        }
    }
}
