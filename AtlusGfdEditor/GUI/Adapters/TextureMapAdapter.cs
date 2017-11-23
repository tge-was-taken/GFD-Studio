using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    class TextureMapAdapter : TreeNodeAdapter<TextureMap>
    {
        public override MenuFlags ContextMenuFlags 
            => MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => Resource.Name;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public int Field44
        {
            get => Resource.Field44;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field48
        {
            get => Resource.Field48;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field49
        {
            get => Resource.Field49;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field4A
        {
            get => Resource.Field4A;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field4B
        {
            get => Resource.Field4B;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field4C
        {
            get => Resource.Field4C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field50
        {
            get => Resource.Field50;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field54
        {
            get => Resource.Field54;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field58
        {
            get => Resource.Field58;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field5C
        {
            get => Resource.Field5C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field60
        {
            get => Resource.Field60;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field64
        {
            get => Resource.Field64;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field68
        {
            get => Resource.Field68;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field6C
        {
            get => Resource.Field6C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field70
        {
            get => Resource.Field70;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field74
        {
            get => Resource.Field74;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field78
        {
            get => Resource.Field78;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field7C
        {
            get => Resource.Field7C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field80
        {
            get => Resource.Field80;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => Resource.Field84;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field88
        {
            get => Resource.Field88;
            set => SetResourceProperty( value );
        }

        public TextureMapAdapter( string text, TextureMap resource ) : base( text, resource )
        {
            RegisterExportAction<TextureMap>( path => AtlusGfdLib.Resource.Save( Resource, path ) );
            RegisterReplaceAction<TextureMap>( AtlusGfdLib.Resource.Load<TextureMap> );
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
