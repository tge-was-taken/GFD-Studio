using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLibrary;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialViewModel : TreeNodeViewModel<Material>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Branch;

        [Browsable( true )]
        public new string Name
        {
            get => GetModelProperty< string >();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialFlags> ) )]
        public new MaterialFlags Flags
        {
            get => GetModelProperty< MaterialFlags >();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Ambient
        {
            get => GetModelProperty< Vector4 >();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Diffuse
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Specular
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Emissive
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field44
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialDrawOrder> ) )]
        public MaterialDrawOrder DrawOrder
        {
            get => GetModelProperty<MaterialDrawOrder>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field49
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4A
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4B
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4C
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4D
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field90
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field92
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Int16HexTypeConverter ) )]
        public short Field94
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field96
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field5C
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field6C
        {
            get => GetModelProperty<uint>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field70
        {
            get => GetModelProperty<uint>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field50
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field98
        {
            get => GetModelProperty<uint>();
            set => SetModelProperty( value );
        }

        [Browsable( false )]
        public TextureMapListViewModel TextureMapsViewModel { get; set; }

        [Browsable( false )]
        public ListViewModel<MaterialAttribute> AttributesViewModel { get; set; }

        public MaterialViewModel( string text, Material resource ) : base( text, resource )
        {
        }

        private List<TextureMap> CreateTextureMapInfo()
        {
            var textureMapList = new List<TextureMap>();
             textureMapList.Add( Model.DiffuseMap );
            textureMapList.Add( Model.NormalMap );
            textureMapList.Add( Model.SpecularMap );
            textureMapList.Add( Model.ReflectionMap );
            textureMapList.Add( Model.HighlightMap );
            textureMapList.Add( Model.GlowMap );
            textureMapList.Add( Model.NightMap );

            textureMapList.Add( Model.DetailMap );

            textureMapList.Add( Model.ShadowMap );

            return textureMapList;
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Material>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Material>( Resource.Load<Material> );
            RegisterModelUpdateHandler( () =>
            {
                var material = Model;

                material.DetailMap = null;
                material.DiffuseMap = null;
                material.GlowMap = null;
                material.HighlightMap = null;
                material.NightMap = null;
                material.NormalMap = null;
                material.ReflectionMap = null;
                material.ShadowMap = null;
                material.SpecularMap = null;

                if ( !TextureMapsViewModel.IsExpanded )
                {
                    // Fixes accidentally accessing dummy node
                    TextureMapsViewModel.InitializeView();
                }

                for ( var i = 0; i < TextureMapsViewModel.Model.Count; i++ )
                {
                    var textureMap = TextureMapsViewModel.Model[ i ];
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

                material.Attributes = new List< MaterialAttribute >();
                foreach ( var attribute in AttributesViewModel.Model )
                {
                    material.Attributes.Add( attribute );
                }

                return material;
            } );

            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            TextureMapsViewModel = ( TextureMapListViewModel)TreeNodeViewModelFactory.Create( "Texture Maps", CreateTextureMapInfo() );
            Nodes.Add( TextureMapsViewModel );

            AttributesViewModel =
                ( ListViewModel< MaterialAttribute > ) TreeNodeViewModelFactory.Create(
                    "Attributes", 
                    Model.Attributes == null ? new List< MaterialAttribute >() : Model.Attributes,
                    new object[] { new ListItemNameProvider< MaterialAttribute >( ( value, index ) => value.Type.ToString() ) } );
            Nodes.Add( AttributesViewModel );
        }
    }
}
