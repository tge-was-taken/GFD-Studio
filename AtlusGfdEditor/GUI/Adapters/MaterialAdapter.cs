using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using AtlusGfdLib;
using AtlusGfdLib.IO;
using AtlusGfdLib.IO.Resource;

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
        public MaterialDrawOrder DrawOrder
        {
            get => Resource.DrawOrder;
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
                textureMapItemNames.Add( nameof(Resource.DiffuseMap) );
            }

            if ( Resource.NormalMap != null )
            {
                textureMapList.Add( Resource.NormalMap );
                textureMapItemNames.Add( nameof( Resource.NormalMap ) );
            }

            if ( Resource.SpecularMap != null )
            {
                textureMapList.Add( Resource.SpecularMap );
                textureMapItemNames.Add( nameof( Resource.SpecularMap ) );
            }

            if ( Resource.ReflectionMap != null )
            {
                textureMapList.Add( Resource.ReflectionMap );
                textureMapItemNames.Add( nameof( Resource.ReflectionMap ) );
            }

            if ( Resource.HighlightMap != null )
            {
                textureMapList.Add( Resource.HighlightMap );
                textureMapItemNames.Add( nameof( Resource.HighlightMap ) );
            }

            if ( Resource.GlowMap != null )
            {
                textureMapList.Add( Resource.GlowMap );
                textureMapItemNames.Add( nameof( Resource.GlowMap ) );
            }

            if ( Resource.NightMap != null )
            {
                textureMapList.Add( Resource.NightMap );
                textureMapItemNames.Add( nameof( Resource.NightMap ) );
            }

            if ( Resource.DetailMap != null )
            {
                textureMapList.Add( Resource.DetailMap );
                textureMapItemNames.Add( nameof( Resource.DetailMap ) );
            }

            if ( Resource.ShadowMap != null )
            {
                textureMapList.Add( Resource.ShadowMap );
                textureMapItemNames.Add( nameof( Resource.ShadowMap ) );
            }

            return (textureMapList, textureMapItemNames);
        }

        protected override void InitializeCore()
        {
            RegisterExportAction< Material >( path => AtlusGfdLib.Resource.Save( Resource, path ) );
            RegisterReplaceAction< Material >( AtlusGfdLib.Resource.Load< Material >);
            RegisterRebuildAction( () =>
            {
                var material = Resource;

                material.DetailMap = null;
                material.DiffuseMap = null;
                material.GlowMap = null;
                material.HighlightMap = null;
                material.NightMap = null;
                material.NormalMap = null;
                material.ReflectionMap = null;
                material.ShadowMap = null;
                material.SpecularMap = null;

                foreach ( TextureMapAdapter adapter in TextureMaps.Nodes )
                {
                    switch ( adapter.Name )
                    {
                        case nameof( Material.DiffuseMap ):
                            material.DiffuseMap = adapter.Resource;
                            break;
                        case nameof( Material.DetailMap ):
                            material.DetailMap = adapter.Resource;
                            break;
                        case nameof( Material.GlowMap ):
                            material.GlowMap = adapter.Resource;
                            break;
                        case nameof( Material.HighlightMap ):
                            material.HighlightMap = adapter.Resource;
                            break;
                        case nameof( Material.NightMap ):
                            material.NightMap = adapter.Resource;
                            break;
                        case nameof( Material.NormalMap ):
                            material.NormalMap = adapter.Resource;
                            break;
                        case nameof( Material.ReflectionMap ):
                            material.ReflectionMap = adapter.Resource;
                            break;
                        case nameof( Material.ShadowMap ):
                            material.ShadowMap = adapter.Resource;
                            break;
                        case nameof( Material.SpecularMap ):
                            material.SpecularMap = adapter.Resource;
                            break;
                    }
                }

                /*
                material.Attributes.Clear();
                foreach ( MaterialAttributeAdapter materialAttributeAdapter in Attributes.Nodes )
                {
                    
                }
                */
                return material;
            } );

            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            var textureMapInfo = CreateTextureMapInfo();
            TextureMaps = ( ListAdapter<TextureMap> )TreeNodeAdapterFactory.Create( "TextureMaps", textureMapInfo.TextureMapList, new object[] { textureMapInfo.TextureMapItemNames } );

            Nodes.Add( TextureMaps );
        }
    }
}
