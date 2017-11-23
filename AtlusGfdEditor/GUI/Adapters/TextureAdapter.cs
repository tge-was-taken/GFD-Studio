using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using AtlusGfdLib;
using AtlusGfdLib.Common.Utillities;
using AtlusGfdLib.IO;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class TextureAdapter : TreeNodeAdapter<Texture>
    {
        public override MenuFlags ContextMenuFlags => 
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => Resource.Name;
            set => SetResourceProperty(value);
        }

        [Browsable( true )]
        public TextureFormat Format
        {
            get => Resource.Format;
        }

        [Browsable( true )]
        [TypeConverter(typeof( Int32HexTypeConverter ) )]
        public int DataLength
        {
            get => Resource.Data.Length;
        }

        [Browsable( true )]
        public byte Field1C
        {
            get => Resource.Field1C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field1D
        {
            get => Resource.Field1D;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field1E
        {
            get => Resource.Field1E;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field1F
        {
            get => Resource.Field1F;
            set => SetResourceProperty( value );
        }

        protected internal TextureAdapter( string text, Texture resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Bitmap>( ( path ) 
                => TextureDecoder.Decode( Resource ).Save( path, ImageFormatHelper.GetImageFormatFromPath( path ) ) );

            RegisterExportAction<Stream>( ( path )
                => File.WriteAllBytes( path, Resource.Data ) );

            RegisterReplaceAction<Bitmap>( ( path ) 
                => TextureEncoder.Encode( Name, Format, Field1C, Field1D, Field1E, Field1F, new Bitmap( path ) ) );

            RegisterReplaceAction< Stream >( path
                                                 => new Texture( Name, Format, File.ReadAllBytes( path ), Field1C, Field1D, Field1E, Field1F ) );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
