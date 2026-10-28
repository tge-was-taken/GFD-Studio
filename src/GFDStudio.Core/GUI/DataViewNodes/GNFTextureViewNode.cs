using System;
using System.ComponentModel;
using System.IO;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.GNF;
using GFDStudio.FormatModules;

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
            get => Data.MinLodClamp;
            set => Set( x => x.MinLodClamp, value );
        }

        [DisplayName( "Surface format" )]
        public SurfaceFormat SurfaceFormat
        {
            get => Data.SurfaceFormat;
            set => Set( x => x.SurfaceFormat, value );
        }

        [DisplayName( "Channel type" )]
        public ChannelType ChannelType
        {
            get => Data.ChannelType;
            set => Set( x => x.ChannelType, value );
        }

        public int Width
        {
            get => Data.Width;
            set => Set( x => x.Width, value );
        }

        public int Height
        {
            get => Data.Height;
            set => Set( x => x.Height, value );
        }

        [DisplayName( "Sampler modulation factor" )]
        public SamplerModulationFactor SamplerModulationFactor
        {
            get => Data.SamplerModulationFactor;
            set => Set( x => x.SamplerModulationFactor, value );
        }

        [DisplayName( "Channel order X" )]
        public TextureChannel ChannelOrderX
        {
            get => Data.ChannelOrderX;
            set => Set( x => x.ChannelOrderX, value );
        }

        [DisplayName( "Channel order Y" )]
        public TextureChannel ChannelOrderY
        {
            get => Data.ChannelOrderY;
            set => Set( x => x.ChannelOrderY, value );
        }

        [DisplayName( "Channel order Z" )]
        public TextureChannel ChannelOrderZ
        {
            get => Data.ChannelOrderZ;
            set => Set( x => x.ChannelOrderZ, value );
        }

        [DisplayName( "Channel order W" )]
        public TextureChannel ChannelOrderW
        {
            get => Data.ChannelOrderW;
            set => Set( x => x.ChannelOrderW, value );
        }

        [DisplayName( "Base mip level" )]
        public int BaseMipLevel
        {
            get => Data.BaseMipLevel;
            set => Set( x => x.BaseMipLevel, value );
        }

        [DisplayName( "Last mip level" )]
        public int LastMipLevel
        {
            get => Data.LastMipLevel;
            set => Set( x => x.LastMipLevel, value );
        }

        [DisplayName( "Tile mode" )]
        public TileMode TileMode
        {
            get => Data.TileMode;
            set => Set( x => x.TileMode, value );
        }

        [DisplayName( "Is padded to pow 2" )]
        public bool IsPaddedToPow2
        {
            get => Data.IsPaddedToPow2;
            set => Set( x => x.IsPaddedToPow2, value );
        }

        [DisplayName( "Texture type" )]
        public TextureType TextureType
        {
            get => Data.TextureType;
            set => Set( x => x.TextureType, value );
        }

        public int Depth
        {
            get => Data.Depth;
            set => Set( x => x.Depth, value );
        }

        public int Pitch
        {
            get => Data.Pitch;
            set => Set( x => x.Pitch, value );
        }

        [DisplayName( "Base array slice index" )]
        public int BaseArraySliceIndex
        {
            get => Data.BaseArraySliceIndex;
            set => Set( x => x.BaseArraySliceIndex, value );
        }

        [DisplayName( "Last array slice index" )]
        public int LastArraySliceIndex
        {
            get => Data.LastArraySliceIndex;
            set => Set( x => x.LastArraySliceIndex, value );
        }

        [DisplayName( "Min LOD warning" )]
        public int MinLodWarning
        {
            get => Data.MinLodWarning;
            set => Set( x => x.MinLodWarning, value );
        }

        [DisplayName( "Mip stats counter index" )]
        public int MipStatsCounterIndex
        {
            get => Data.MipStatsCounterIndex;
            set => Set( x => x.MipStatsCounterIndex, value );
        }

        [DisplayName( "Mip stats enabled" )]
        public bool MipStatsEnabled
        {
            get => Data.MipStatsEnabled;
            set => Set( x => x.MipStatsEnabled, value );
        }

        [DisplayName( "Metadata compression enabled" )]
        public bool MetadataCompressionEnabled
        {
            get => Data.MetadataCompressionEnabled;
            set => Set( x => x.MetadataCompressionEnabled, value );
        }

        [DisplayName( "DCC alpha on MSB" )]
        public bool DccAlphaOnMsb
        {
            get => Data.DccAlphaOnMsb;
            set => Set( x => x.DccAlphaOnMsb, value );
        }

        [DisplayName( "DCC color transform" )]
        public int DccColorTransform
        {
            get => Data.DccColorTransform;
            set => Set( x => x.DccColorTransform, value );
        }

        [DisplayName( "Use alt tile mode" )]
        public bool UseAltTileMode
        {
            get => Data.UseAltTileMode;
            set => Set( x => x.UseAltTileMode, value );
        }

        public GNFTextureViewNode( string text, GNFTexture data ) : base( text, data )
        {
        }
        protected override void InitializeCore()
        {
            RegisterExportHandler<GNFTexture>( filePath => Data.Save( filePath ) );
            RegisterExportHandler<GFDLibrary.Graphics.Image>( path => PngCodec.Save( TextureDecoder.Decode( Data ), path ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, TextureDecoder.DecodeToDDS( Data ) ) );

            RegisterReplaceHandler<GFDLibrary.Graphics.Image>( path => new GNFTexture( GFDLibrary.Textures.PngCodec.Load( path ) ) );
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