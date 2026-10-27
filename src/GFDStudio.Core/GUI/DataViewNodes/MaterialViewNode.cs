using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDLibrary.Models;
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

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 DiffuseColor
        {
            get => Data.DiffuseColor;
            set => SetDataProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive color (float)" )]
        public Vector4 EmissiveColor
        {
            get => Data.EmissiveColor;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4ColorTypeConverter ) )]
        [DisplayName( "Specular color (float)" )]
        public Vector4 SpecularColor
        {
            get => Data.SpecularColor;
            set => SetDataProperty( value );
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
        [TypeConverter( typeof( EnumTypeConverter<MultiBlendType> ) )]
        [DisplayName( "Highlight Map Blend Mode" )]
        public MultiBlendType MultiBlendMode
        {
            get => Data.MultiBlendMode;
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
        public short Culling
        {
            get => Data.Culling;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint ConstantColor
        {
            get => Data.ConstantColor;
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
                Data.GlossMap,
                Data.ReflectionMap,
                Data.MultiMap,
                Data.GlowMap,
                Data.DarkMap,
                Data.DetailMap,
                Data.LightMap,
                Data.ReserveMap,
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

            RegisterCustomHandler( "Meshes using this material", "Set binormals to a direction", () =>
            {
                var direction = UserDialogs.Current.AskDirection(
                    "Set binormals",
                    "Every binormal of every mesh using this material is set to this direction. A shader " +
                    "that reads the binormal as a forward vector, rather than as part of a tangent basis, " +
                    "wants this.",
                    Vector3.UnitZ );

                if ( direction != null )
                    ApplyToMeshesUsingThisMaterial( mesh => MeshVertexGeometry.SetBinormals( mesh, direction.Value ) );
            } );

            RegisterCustomHandler( "Meshes using this material", "Regenerate normals", () =>
                ApplyToMeshesUsingThisMaterial( MeshVertexGeometry.RegenerateNormals ) );

            RegisterCustomHandler( "Meshes using this material", "Regenerate tangents and binormals", () =>
                ApplyToMeshesUsingThisMaterial( MeshVertexGeometry.RegenerateTangentSpace ) );
            RegisterModelUpdateHandler( () =>
            {
                var material = Data;

                material.DetailMap = null;
                material.DiffuseMap = null;
                material.GlowMap = null;
                material.MultiMap = null;
                material.DarkMap = null;
                material.NormalMap = null;
                material.ReflectionMap = null;
                material.LightMap = null;
                material.GlossMap = null;
                material.ReserveMap = null;

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
                            material.GlossMap = textureMap;
                            break;
                        case 3:
                            material.ReflectionMap = textureMap;
                            break;
                        case 4:
                            material.MultiMap = textureMap;
                            break;
                        case 5:
                            material.GlowMap = textureMap;
                            break;
                        case 6:
                            material.DarkMap = textureMap;
                            break;
                        case 7:
                            material.DetailMap = textureMap;
                            break;
                        case 8:
                            material.LightMap = textureMap;
                            break;
                        case 9:
                            material.ReserveMap = textureMap;
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

        /// <summary>
        /// Runs something over every mesh in the model that names this material. The meshes live
        /// under the model rather than under the material, so the pack has to be found first.
        /// </summary>
        private void ApplyToMeshesUsingThisMaterial( Action<Mesh> apply )
        {
            var model = FindModel();
            if ( model == null )
            {
                UserDialogs.Current.ShowMessage(
                    "This material is not part of a model, so there are no meshes to change.",
                    "Nothing to do", UserMessageKind.Information );
                return;
            }

            var changed = 0;
            foreach ( var mesh in model.Meshes.Where( x => x.MaterialName == Data.Name ) )
            {
                apply( mesh );
                changed++;
            }

            UserDialogs.Current.ShowMessage(
                changed == 0
                    ? $"No mesh uses \"{Data.Name}\"."
                    : $"Changed {changed} mesh{( changed == 1 ? string.Empty : "es" )} using \"{Data.Name}\".",
                "Done", UserMessageKind.Information );
        }

        /// <summary>
        /// The model this material belongs to, found by walking up past the material dictionary.
        /// </summary>
        private Model FindModel()
        {
            for ( var node = Parent; node != null; node = node.Parent )
            {
                if ( node.Data is ModelPack pack )
                    return pack.Model;

                if ( node.Data is Model model )
                    return model;
            }

            return null;
        }

        private void ConvertToMaterialPreset()
        {
            var options = UserDialogs.Current.AskModelConversionOptions( null, null );
            if ( options == null )
                return;

            Replace( Material.ConvertToMaterialPreset( Data, options.DefaultMaterial.Preset ) );
        }
    }
}
