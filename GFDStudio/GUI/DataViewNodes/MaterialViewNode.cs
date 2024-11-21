using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDStudio.GUI.Forms;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialLegacyParametersViewNode : DataViewNode<MaterialLegacyParameters>
    {

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Ambient color (float)" )]
        public Vector4 AmbientColor
        {
            get => Data.AmbientColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Ambient color (RGBA)" )]
        public System.Drawing.Color AmbientColorRGBA
        {
            get => Data.AmbientColor.ToByte();
            set => Data.AmbientColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 DiffuseColor
        {
            get => Data.DiffuseColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Diffuse color (RGBA)" )]
        public System.Drawing.Color DiffuseColorRGBA
        {
            get => Data.DiffuseColor.ToByte();
            set => Data.DiffuseColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive color (float)" )]
        public Vector4 EmissiveColor
        {
            get => Data.EmissiveColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Emissive color (RGBA)" )]
        public System.Drawing.Color EmissiveColorRGBA
        {
            get => Data.EmissiveColor.ToByte();
            set => Data.EmissiveColor = value.ToFloat();
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        [DisplayName( "Specular color (float)" )]
        public Vector4 SpecularColor
        {
            get => Data.SpecularColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Specular color (RGBA)" )]
        public System.Drawing.Color SpecularColorRGBA
        {
            get => Data.SpecularColor.ToByte();
            set => Data.SpecularColor = value.ToFloat();
        }

        [Browsable( true )]
        [DisplayName( "Reflectivity" )]
        public float Reflectivity
        {
            get => Data.Reflectivity;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Diffusivity" )]
        public float Diffusivity
        {
            get => Data.Diffusivity;
            set => SetDataProperty( value );
        }

        public MaterialLegacyParametersViewNode( string text, MaterialLegacyParameters data ) : base( text, data )
        {
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
    public class MaterialViewNode : DataViewNode<Material>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Convert;

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
        public MaterialFlags Flags
        {
            get => GetDataProperty< MaterialFlags >();
            set => SetDataProperty( value );
        }
        

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialDrawMethod> ) )]
        [DisplayName( "Draw method" )]
        public MaterialDrawMethod DrawMethod
        {
            get => GetDataProperty<MaterialDrawMethod>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Blend Source Color" )]
        public byte BlendSourceColor
        {
            get => Data.BlendSourceColor;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Blend Destination Color" )]
        public byte BlendDestinationColor
        {
            get => Data.BlendDestinationColor;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Source Alpha" )]
        public byte SourceAlpha
        {
            get => Data.SourceAlpha;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Destination Alpha" )]
        public byte DestinationAlpha
        {
            get => Data.DestinationAlpha;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<HighlightMapMode> ) )]
        [DisplayName( "Highlight Map Blend Mode" )]
        public HighlightMapMode HighlightMapBlendMode
        {
            get => Data.HighlightMapBlendMode;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Alpha Clip" )]
        public short AlphaClip
        {
            get => Data.AlphaClip;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<AlphaClipMode> ) )]
        [DisplayName( "Alpha Clip Mode" )]
        public AlphaClipMode AlphaClipMode
        {
            get => Data.AlphaClipMode;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Flags 2" )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialFlags2> ) )]
        public MaterialFlags2 Flags2
        {
            get => Data.Flags2;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Sort Priority" )]
        public short SortPriority
        {
            get => Data.SortPriority;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Shader ID" )]
        public short ShaderId
        {
            get => Data.ShaderId;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        [DisplayName( "Texcoord flags [0]" )]
        public uint TexCoordFlags0
        {
            get => Data.TexCoordFlags0;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        [DisplayName( "Texcoord flags [1]" )]
        public uint TexCoordFlags1
        {
            get => Data.TexCoordFlags1;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName("Disable Backface Culling")]
        public short DisableBackfaceCulling
        {
            get => Data.DisableBackfaceCulling;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field98
        {
            get => Data.Field98;
            set => SetDataProperty( value );
        }
        public float Field6C_2
        {
            get => Data.Field6C_2;
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        //public DataViewNode<MaterialParameterSetBase> MaterialParameterSetViewNode { get; set; }
        public DataViewNode MaterialParameterSetViewNode { get; set; }

        [Browsable( false )]
        public TextureMapListViewNode TextureMapsViewNode { get; set; }

        [Browsable( false )]
        public ListViewNode<MaterialAttribute> AttributesViewNode { get; set; }

        public MaterialViewNode( string text, Material data ) : base( text, data )
        {
        }

        private List<TextureMap> CreateTextureMapInfo()
        {
            var textureMapList = new List<TextureMap>()
            {
                Data.DiffuseMap,
                Data.NormalMap,
                Data.SpecularMap,
                Data.ReflectionMap,
                Data.HighlightMap,
                Data.GlowMap,
                Data.NightMap,
                Data.DetailMap,
                Data.ShadowMap,
                Data.TextureMap10,
            };
            return textureMapList;
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Material>( path => Data.Save(  path ) );
            RegisterReplaceHandler<Material>( path =>
            {
                var temp = Resource.Load<Material>( path );
                temp.Name = Data.Name;
                return temp;
            } );
            RegisterCustomHandler("Convert to", "Material preset", () => { ConvertToMaterialPreset(); });
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
                material.TextureMap10 = null;

                //if ( material.METAPHOR_UseMaterialParameterSet )
                //    material.METAPHOR_MaterialParameterSet = MaterialParameterSetViewNode.Data;

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
                        case 9:
                            material.TextureMap10 = textureMap;
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
            if ( Data.METAPHOR_UseMaterialParameterSet )
            {
                MaterialParameterSetViewNode = DataViewNodeFactory.Create( Data.METAPHOR_MaterialParameterSet.GetParameterName(), Data.METAPHOR_MaterialParameterSet );
                AddChildNode( MaterialParameterSetViewNode );
            } else
            {
                AddChildNode( DataViewNodeFactory.Create( "Parameters" , Data.LegacyParameters ) );
            }
            TextureMapsViewNode = ( TextureMapListViewNode)DataViewNodeFactory.Create( "Texture Maps", CreateTextureMapInfo() );
            AddChildNode( TextureMapsViewNode );

            AttributesViewNode =
                ( ListViewNode< MaterialAttribute > ) DataViewNodeFactory.Create(
                    "Attributes", 
                    Data.Attributes == null ? new List< MaterialAttribute >() : Data.Attributes,
                    new object[] { new ListItemNameProvider< MaterialAttribute >( ( value, index ) => value.AttributeType.ToString() ) } );
            AddChildNode( AttributesViewNode );
        }

        private void ConvertToMaterialPreset()
        {
            using (var dialog = new ModelConverterOptionsDialog(false))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                ModelConverterOptions options = new ModelConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version
                };
                Replace(Material.ConvertToMaterialPreset(Data, options));
            }
        }
    }
}
