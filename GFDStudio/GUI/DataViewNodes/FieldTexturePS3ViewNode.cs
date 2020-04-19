using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.Utilities;

namespace GFDStudio.GUI.DataViewNodes
{
    public class FieldTexturePS3ViewNode : DataViewNode<FieldTexturePS3>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export |
                                                                  DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename |
                                                                  DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        public int Field00
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Data length" )]
        public int DataLength => Data.DataLength;

        public int Field08
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public int Field0C
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public FieldTextureFlags Flags
        {
            get => GetDataProperty<FieldTextureFlags>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Mip map count" )]
        public byte MipMapCount
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        public byte Field1A
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        public byte Field1B
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        public int Field1C
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public short Width
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        public short Height
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        public short Field24
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        public FieldTexturePS3ViewNode( string text, FieldTexturePS3 data ) : base( text, data )
        {
        }
        protected override void InitializeCore()
        {
            RegisterExportHandler<FieldTexturePS3>( filePath => Data.Save( filePath ) );
            RegisterExportHandler<Bitmap>( path => TextureDecoder.Decode( Data ).Save( path, ImageFormatHelper.GetImageFormatFromPath( path ) ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, TextureDecoder.DecodeToDDS( Data ) ) );

            RegisterReplaceHandler<Bitmap>( path => new FieldTexturePS3( new Bitmap( path ) ) );
            RegisterReplaceHandler<DDSStream>( path =>
            {
                if ( Path.GetExtension( path ).ToLowerInvariant() != ".dds" )
                    throw new InvalidOperationException( "Expected DDS file" );

                return new FieldTexturePS3( File.ReadAllBytes( path ) );
            });

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
