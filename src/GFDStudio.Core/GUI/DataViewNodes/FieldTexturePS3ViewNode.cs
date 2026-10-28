using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDStudio.FormatModules;

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
            get => Data.Field00;
            set => Set( x => x.Field00, value );
        }

        [DisplayName( "Data length" )]
        public int DataLength => Data.DataLength;

        public int Field08
        {
            get => Data.Field08;
            set => Set( x => x.Field08, value );
        }

        public int Field0C
        {
            get => Data.Field0C;
            set => Set( x => x.Field0C, value );
        }

        public FieldTextureFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [DisplayName( "Mip map count" )]
        public byte MipMapCount
        {
            get => Data.MipMapCount;
            set => Set( x => x.MipMapCount, value );
        }

        public byte Field1A
        {
            get => Data.Field1A;
            set => Set( x => x.Field1A, value );
        }

        public byte Field1B
        {
            get => Data.Field1B;
            set => Set( x => x.Field1B, value );
        }

        public int Field1C
        {
            get => Data.Field1C;
            set => Set( x => x.Field1C, value );
        }

        public short Width
        {
            get => Data.Width;
            set => Set( x => x.Width, value );
        }

        public short Height
        {
            get => Data.Height;
            set => Set( x => x.Height, value );
        }

        public short Field24
        {
            get => Data.Field24;
            set => Set( x => x.Field24, value );
        }

        public FieldTexturePS3ViewNode( string text, FieldTexturePS3 data ) : base( text, data )
        {
        }
        protected override void InitializeCore()
        {
            RegisterExportHandler<FieldTexturePS3>( filePath => Data.Save( filePath ) );
            RegisterExportHandler<GFDLibrary.Graphics.Image>( path => PngCodec.Save( TextureDecoder.Decode( Data ), path ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, TextureDecoder.DecodeToDDS( Data ) ) );

            RegisterReplaceHandler<GFDLibrary.Graphics.Image>( path => new FieldTexturePS3( GFDLibrary.Textures.PngCodec.Load( path ) ) );
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
