using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialViewNode : DataViewNode<Material>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Branch;

        [Browsable( true )]
        public new string Name
        {
            get => GetDataProperty< string >();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialFlags> ) )]
        public new MaterialFlags Flags
        {
            get => GetDataProperty< MaterialFlags >();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        public Vector4 Ambient
        {
            get => GetDataProperty< Vector4 >();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        public Vector4 Diffuse
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        public Vector4 Specular
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        public Vector4 Emissive
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field44
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialDrawOrder> ) )]
        public MaterialDrawOrder DrawOrder
        {
            get => GetDataProperty<MaterialDrawOrder>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field49
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field4A
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field4B
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field4C
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field4D
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field90
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field92
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Int16HexTypeConverter ) )]
        public short Field94
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field96
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field5C
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field6C
        {
            get => GetDataProperty<uint>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field70
        {
            get => GetDataProperty<uint>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field50
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field98
        {
            get => GetDataProperty<uint>();
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        public TextureMapListViewNode TextureMapsViewNode { get; set; }

        [Browsable( false )]
        public ListViewNode<MaterialAttribute> AttributesViewNode { get; set; }

        public MaterialViewNode( string text, Material data ) : base( text, data )
        {
        }

        private List<TextureMap> CreateTextureMapInfo()
        {
            var textureMapList = new List<TextureMap>();
             textureMapList.Add( Data.DiffuseMap );
            textureMapList.Add( Data.NormalMap );
            textureMapList.Add( Data.SpecularMap );
            textureMapList.Add( Data.ReflectionMap );
            textureMapList.Add( Data.HighlightMap );
            textureMapList.Add( Data.GlowMap );
            textureMapList.Add( Data.NightMap );

            textureMapList.Add( Data.DetailMap );

            textureMapList.Add( Data.ShadowMap );

            return textureMapList;
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Material>( path => Data.Save(  path ) );
            RegisterReplaceHandler<Material>( Resource.Load<Material> );
            RegisterModelUpdateHandler( () =>
            {
                var material = Data;

                material.DetailMap = null;
                material.DiffuseMap = null;
                material.GlowMap = null;
                material.HighlightMap = null;
                material.NightMap = null;
                material.NormalMap = null;
                material.ReflectionMap = null;
                material.ShadowMap = null;
                material.SpecularMap = null;

                if ( !TextureMapsViewNode.IsExpanded )
                {
                    // Fixes accidentally accessing dummy node
                    TextureMapsViewNode.InitializeView();
                }

                for ( var i = 0; i < TextureMapsViewNode.Data.Count; i++ )
                {
                    var textureMap = TextureMapsViewNode.Data[ i ];
                    switch ( i )
                    {
                        case 0:
                            material.DiffuseMap = textureMap;
                            break;
                        case 1:
                            material.NormalMap = textureMap;
                            break;
                        case 2:
                            material.SpecularMap = textureMap;
                            break;
                        case 3:
                            material.ReflectionMap = textureMap;
                            break;
                        case 4:
                            material.HighlightMap = textureMap;
                            break;
                        case 5:
                            material.GlowMap = textureMap;
                            break;
                        case 6:
                            material.NightMap = textureMap;
                            break;
                        case 7:
                            material.DetailMap = textureMap;
                            break;
                        case 8:
                            material.ShadowMap = textureMap;
                            break;
                    }
                }

                if ( AttributesViewNode.Data.Count != 0 )
                {
                    material.Attributes = new List<MaterialAttribute>();
                    foreach ( var attribute in AttributesViewNode.Data )
                    {
                        material.Attributes.Add( attribute );
                    }
                }
                else
                {
                    material.Attributes = null;
                }

                return material;
            } );

            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            TextureMapsViewNode = ( TextureMapListViewNode)DataViewNodeFactory.Create( "Texture Maps", CreateTextureMapInfo() );
            Nodes.Add( TextureMapsViewNode );

            AttributesViewNode =
                ( ListViewNode< MaterialAttribute > ) DataViewNodeFactory.Create(
                    "Attributes", 
                    Data.Attributes == null ? new List< MaterialAttribute >() : Data.Attributes,
                    new object[] { new ListItemNameProvider< MaterialAttribute >( ( value, index ) => value.AttributeType.ToString() ) } );
            Nodes.Add( AttributesViewNode );
        }
    }
}
