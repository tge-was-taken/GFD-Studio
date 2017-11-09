using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class MaterialAdapter : TreeNodeAdapter<Material>
    {
        public override MenuFlags ContextMenuFlags =>
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags
            => TreeNodeAdapter.Flags.Branch;

        [Browsable( true )]
        public new string Name
        {
            get => Resource.Name;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public new MaterialFlags Flags
        {
            get => Resource.Flags;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public Vector4 Ambient
        {
            get => Resource.Ambient;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public Vector4 Diffuse
        {
            get => Resource.Diffuse;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public Vector4 Specular
        {
            get => Resource.Specular;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public Vector4 Emissive
        {
            get => Resource.Emissive;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => Resource.Field40;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public float Field44
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
        public byte Field4C
        {
            get => Resource.Field4C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public byte Field4D
        {
            get => Resource.Field4D;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field90
        {
            get => Resource.Field90;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field92
        {
            get => Resource.Field92;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field94
        {
            get => Resource.Field94;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field96
        {
            get => Resource.Field96;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field5C
        {
            get => Resource.Field5C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        [TypeConverter(typeof( UInt32HexTypeConverter ))]
        public uint Field6C
        {
            get => Resource.Field6C;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field70
        {
            get => Resource.Field70;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        public short Field50
        {
            get => Resource.Field50;
            set => SetResourceProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field98
        {
            get => Resource.Field98;
            set => SetResourceProperty( value );
        }

        [Browsable( false )]
        public ListAdapter<TextureMap> TextureMaps { get; set; }

        [Browsable( false )]
        public ListAdapter<MaterialAttribute> Attributes { get; set; }

        public MaterialAdapter( string text, Material resource ) : base( text, resource )
        {
        }

        private ( List<TextureMap> TextureMapList, List<string> TextureMapItemNames ) CreateTextureMapInfo()
        {
            var textureMapItemNames = new List<string>();
            var textureMapList = new List<TextureMap>();

            if ( Resource.DiffuseMap != null )
            {
                textureMapList.Add( Resource.DiffuseMap );
                textureMapItemNames.Add( "DiffuseMap" );
            }

            if ( Resource.NormalMap != null )
            {
                textureMapList.Add( Resource.NormalMap );
                textureMapItemNames.Add( "NormalMap" );
            }

            if ( Resource.SpecularMap != null )
            {
                textureMapList.Add( Resource.SpecularMap );
                textureMapItemNames.Add( "SpecularMap" );
            }

            if ( Resource.ReflectionMap != null )
            {
                textureMapList.Add( Resource.ReflectionMap );
                textureMapItemNames.Add( "ReflectionMap" );
            }

            if ( Resource.HighlightMap != null )
            {
                textureMapList.Add( Resource.HighlightMap );
                textureMapItemNames.Add( "HighlightMap" );
            }

            if ( Resource.GlowMap != null )
            {
                textureMapList.Add( Resource.GlowMap );
                textureMapItemNames.Add( "GlowMap" );
            }

            if ( Resource.NightMap != null )
            {
                textureMapList.Add( Resource.NightMap );
                textureMapItemNames.Add( "NightMap" );
            }

            if ( Resource.DetailMap != null )
            {
                textureMapList.Add( Resource.DetailMap );
                textureMapItemNames.Add( "DetailMap" );
            }

            if ( Resource.ShadowMap != null )
            {
                textureMapList.Add( Resource.ShadowMap );
                textureMapItemNames.Add( "ShadowMap" );
            }

            return (textureMapList, textureMapItemNames);
        }

        protected override void InitializeCore()
        {
            var textureMapInfo = CreateTextureMapInfo();
            TextureMaps = ( ListAdapter<TextureMap> )TreeNodeAdapterFactory.Create( "TextureMaps", textureMapInfo.TextureMapList, new object[] { textureMapInfo.TextureMapItemNames } );

            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            Nodes.Add( TextureMaps );
        }
    }
}
