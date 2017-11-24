using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

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
        public ListViewModel<TextureMap> TextureMapsViewModel { get; set; }

        [Browsable( false )]
        public ListViewModel<MaterialAttribute> AttributesViewModel { get; set; }

        public MaterialViewModel( string text, Material resource ) : base( text, resource )
        {
        }

        private (List<TextureMap> TextureMapList, List<string> TextureMapItemNames) CreateTextureMapInfo()
        {
            var textureMapItemNames = new List<string>();
            var textureMapList = new List<TextureMap>();

            if ( Model.DiffuseMap != null )
            {
                textureMapList.Add( Model.DiffuseMap );
                textureMapItemNames.Add( nameof( Model.DiffuseMap ) );
            }

            if ( Model.NormalMap != null )
            {
                textureMapList.Add( Model.NormalMap );
                textureMapItemNames.Add( nameof( Model.NormalMap ) );
            }

            if ( Model.SpecularMap != null )
            {
                textureMapList.Add( Model.SpecularMap );
                textureMapItemNames.Add( nameof( Model.SpecularMap ) );
            }

            if ( Model.ReflectionMap != null )
            {
                textureMapList.Add( Model.ReflectionMap );
                textureMapItemNames.Add( nameof( Model.ReflectionMap ) );
            }

            if ( Model.HighlightMap != null )
            {
                textureMapList.Add( Model.HighlightMap );
                textureMapItemNames.Add( nameof( Model.HighlightMap ) );
            }

            if ( Model.GlowMap != null )
            {
                textureMapList.Add( Model.GlowMap );
                textureMapItemNames.Add( nameof( Model.GlowMap ) );
            }

            if ( Model.NightMap != null )
            {
                textureMapList.Add( Model.NightMap );
                textureMapItemNames.Add( nameof( Model.NightMap ) );
            }

            if ( Model.DetailMap != null )
            {
                textureMapList.Add( Model.DetailMap );
                textureMapItemNames.Add( nameof( Model.DetailMap ) );
            }

            if ( Model.ShadowMap != null )
            {
                textureMapList.Add( Model.ShadowMap );
                textureMapItemNames.Add( nameof( Model.ShadowMap ) );
            }

            return (textureMapList, textureMapItemNames);
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

                foreach ( TextureMapViewModel viewModel in TextureMapsViewModel.Nodes )
                {
                    switch ( viewModel.Text )
                    {
                        case nameof( Material.DiffuseMap ):
                            material.DiffuseMap = viewModel.Model;
                            break;
                        case nameof( Material.DetailMap ):
                            material.DetailMap = viewModel.Model;
                            break;
                        case nameof( Material.GlowMap ):
                            material.GlowMap = viewModel.Model;
                            break;
                        case nameof( Material.HighlightMap ):
                            material.HighlightMap = viewModel.Model;
                            break;
                        case nameof( Material.NightMap ):
                            material.NightMap = viewModel.Model;
                            break;
                        case nameof( Material.NormalMap ):
                            material.NormalMap = viewModel.Model;
                            break;
                        case nameof( Material.ReflectionMap ):
                            material.ReflectionMap = viewModel.Model;
                            break;
                        case nameof( Material.ShadowMap ):
                            material.ShadowMap = viewModel.Model;
                            break;
                        case nameof( Material.SpecularMap ):
                            material.SpecularMap = viewModel.Model;
                            break;
                    }
                }

                material.Attributes.Clear();
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
            var textureMapInfo = CreateTextureMapInfo();
            TextureMapsViewModel = ( ListViewModel<TextureMap> )TreeNodeViewModelFactory.Create( "TextureMaps", textureMapInfo.TextureMapList, new object[] { textureMapInfo.TextureMapItemNames } );
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
