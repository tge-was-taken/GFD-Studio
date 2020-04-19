using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.GNF;
using GFDLibrary.Textures.Utilities;

namespace GFDStudio.GUI.DataViewNodes
{
    public class GNFTextureViewNode : DataViewNode<GNFTexture>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export |
                                                                  DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename |
                                                                  DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        public byte Version { get; set; }

        public byte Alignment { get; set; }

        [DisplayName( "Min LOD clamp" )]
        public int MinLodClamp
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Surface format" )]
        public SurfaceFormat SurfaceFormat
        {
            get => GetDataProperty<SurfaceFormat>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Channel type" )]
        public ChannelType ChannelType
        {
            get => GetDataProperty<ChannelType>();
            set => SetDataProperty( value );
        }

        public int Width
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public int Height
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Sampler modulation factor" )]
        public SamplerModulationFactor SamplerModulationFactor
        {
            get => GetDataProperty<SamplerModulationFactor>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Channel order X" )]
        public TextureChannel ChannelOrderX
        {
            get => GetDataProperty<TextureChannel>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Channel order Y" )]
        public TextureChannel ChannelOrderY
        {
            get => GetDataProperty<TextureChannel>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Channel order Z" )]
        public TextureChannel ChannelOrderZ
        {
            get => GetDataProperty<TextureChannel>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Channel order W" )]
        public TextureChannel ChannelOrderW
        {
            get => GetDataProperty<TextureChannel>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Base mip level" )]
        public int BaseMipLevel
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Last mip level" )]
        public int LastMipLevel
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Tile mode" )]
        public TileMode TileMode
        {
            get => GetDataProperty<TileMode>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Is padded to pow 2" )]
        public bool IsPaddedToPow2
        {
            get => GetDataProperty<bool>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Texture type" )]
        public TextureType TextureType
        {
            get => GetDataProperty<TextureType>();
            set => SetDataProperty( value );
        }

        public int Depth
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public int Pitch
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Base array slice index" )]
        public int BaseArraySliceIndex
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Last array slice index" )]
        public int LastArraySliceIndex
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Min LOD warning" )]
        public int MinLodWarning
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Mip stats counter index" )]
        public int MipStatsCounterIndex
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Mip stats enabled" )]
        public bool MipStatsEnabled
        {
            get => GetDataProperty<bool>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Metadata compression enabled" )]
        public bool MetadataCompressionEnabled
        {
            get => GetDataProperty<bool>();
            set => SetDataProperty( value );
        }

        [DisplayName( "DCC alpha on MSB" )]
        public bool DccAlphaOnMsb
        {
            get => GetDataProperty<bool>();
            set => SetDataProperty( value );
        }

        [DisplayName( "DCC color transform" )]
        public int DccColorTransform
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Use alt tile mode" )]
        public bool UseAltTileMode
        {
            get => GetDataProperty<bool>();
            set => SetDataProperty( value );
        }

        public GNFTextureViewNode( string text, GNFTexture data ) : base( text, data )
        {
        }
        protected override void InitializeCore()
        {
            RegisterExportHandler<GNFTexture>( filePath => Data.Save( filePath ) );
            RegisterExportHandler<Bitmap>( path => TextureDecoder.Decode( Data ).Save( path, ImageFormatHelper.GetImageFormatFromPath( path ) ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, TextureDecoder.DecodeToDDS( Data ) ) );

            RegisterReplaceHandler<Bitmap>( path => new GNFTexture( new Bitmap( path ) ) );
            RegisterReplaceHandler<DDSStream>( path =>
            {
                if ( Path.GetExtension( path ).ToLowerInvariant() != ".dds" )
                    throw new InvalidOperationException( "Expected DDS file" );

                return new GNFTexture( new DDSStream( path ) );
            } );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}