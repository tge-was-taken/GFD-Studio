using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Graphics;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GFDStudio.GUI.Forms;

public class ModelConverterOptionsViewModel
{
    [CustomSortedCategory("Global settings",1,2)]
    [DisplayName( "Version" )]
    [TypeConverter( typeof( VersionTypeConverter ) )]
    public uint Version { get; set; } = ResourceVersion.Persona5;

    [CustomSortedCategory( "Global settings", 1, 2 )]
    [DisplayName( "Convert skin to Z-up" )]
    public bool ConvertSkinToZUp { get; set; }

    [CustomSortedCategory( "Global settings", 1, 2 )]
    [DisplayName( "Enable Catherine: Full Body compatibility" )]
    public bool FullBodyCompatibilityMode { get; set; }

    [CustomSortedCategory( "Global settings", 1, 2 )]
    [DisplayName( "Automatically add GFD Helper IDs" )]
    public bool AutoAddGFDHelperIDs { get; set; } = true;

    [CustomSortedCategory( "Default settings", 2, 2 )]
    [DisplayName( "Material settings" )]
    public ModelConverterDefaultMaterialOptionsViewModel DefaultMaterial { get; set; }

    [Browsable( false )]
    public ObservableCollection<ModelConverterMaterialOptionsViewModel> Materials { get; set; } = new();

    [Browsable( false )]
    public ObservableCollection<ModelConverterMeshOptionsViewModel> Meshes { get; set; } = new();

    [Browsable( false )]
    public ObservableCollection<Material> AdditionalMaterialPresets { get; set; } = new();

    public ModelConverterOptionsViewModel() 
    {
        DefaultMaterial = new( this );
    }

    public ModelConverterOptions MapToModelConverterOptions()
    {
        var options = new ModelConverterOptions
        {
            Version = Version,
            DefaultMaterial = new()
            {
                Preset = DefaultMaterial.Preset.Material,
                Mesh = new ModelConverterMeshOptions()
                {
                    GeometryFlags = DefaultMaterial.Mesh.GeometryFlags.GetValueOrDefault(),
                    VertexAttributeFlags = DefaultMaterial.Mesh.VertexAttributeFlags.GetValueOrDefault(),
                    TexCoordChannelMap =
                            DefaultMaterial.Mesh.TexCoordChannelMap
                            .Select( tc => new ModelConverterTexCoordChannelOptions
                            {
                                SourceChannel = tc.SourceChannel
                            } ).ToArray(),
                    ColorChannelMap =
                            DefaultMaterial.Mesh.ColorChannelMap
                            .Select( cc => new ModelConverterColorChannelOptions
                            {
                                SourceChannel = cc.SourceChannel,
                                UseDefaultColor = cc.UseDefaultColor,
                                DefaultColor = cc.DefaultColor,
                                Swizzle = cc.Swizzle
                            } ).ToArray(),
                }
            },
            ConvertSkinToZUp = ConvertSkinToZUp,
            SetFullBodyNodeProperties = FullBodyCompatibilityMode,
            AutoAddGFDHelperIDs = AutoAddGFDHelperIDs,
            Meshes = Meshes
                .Where(m => !m.InheritDefaults || !m.InheritChannelSettingsDefaults)
                .Select(m => 
                new
                {
                    Name = m.Name,
                    Options = new ModelConverterMeshOptions()
                    {
                        GeometryFlags = m.InheritDefaults ? DefaultMaterial.Mesh.GeometryFlags.Value : m.GeometryFlags.GetValueOrDefault(),
                        VertexAttributeFlags = m.InheritDefaults ? DefaultMaterial.Mesh.VertexAttributeFlags.Value : m.VertexAttributeFlags.GetValueOrDefault(),
                        TexCoordChannelMap = 
                            (m.InheritChannelSettingsDefaults ? DefaultMaterial.Mesh.TexCoordChannelMap : m.TexCoordChannelMap)
                            .Select( tc => new ModelConverterTexCoordChannelOptions
                            {
                                SourceChannel = tc.SourceChannel
                            } ).ToArray(),
                        ColorChannelMap =
                            (m.InheritChannelSettingsDefaults ? DefaultMaterial.Mesh.ColorChannelMap : m.ColorChannelMap)
                            .Select( cc => new ModelConverterColorChannelOptions
                            {
                                SourceChannel = cc.SourceChannel,
                                UseDefaultColor = cc.UseDefaultColor,
                                DefaultColor = cc.DefaultColor,
                                Swizzle = cc.Swizzle
                            } ).ToArray(),
                    }
                })
                .ToDictionary(result => result.Name, result => result.Options),
            Materials = Materials
            .Where(m => !m.InheritDefaults)
            .Select(m => new
            {
                Name = m.Name,
                Options = new ModelConverterMaterialOptions()
                {
                    Preset = m.Preset.Material
                }
            } )
            .ToDictionary(result => result.Name, result => result.Options)
        };
        return options;
    }
}

public class CustomSortedCategoryAttribute : CategoryAttribute
{
    private const char NonPrintableChar = '\t';

    public CustomSortedCategoryAttribute( string category,
                                            ushort categoryPos,
                                            ushort totalCategories )
        : base( category.PadLeft( category.Length + ( totalCategories - categoryPos ),
                    CustomSortedCategoryAttribute.NonPrintableChar ) )
    {
    }
}

public class ModelConverterMaterialPresetViewModel
{
    private readonly string _filePath;
    private readonly Lazy<Material> _materialLazy;

    public string Name { get; }

    public Material Material => _materialLazy.Value;

    public bool SourcedFromOriginalModel { get; } = false;

    public ModelConverterMaterialPresetViewModel(string filePath)
    {
        _filePath = filePath;
        _materialLazy = new( () => YamlSerializer.LoadYamlFile<Material>( filePath ) );
        Name = Path.GetFileNameWithoutExtension( filePath );
    }

    public ModelConverterMaterialPresetViewModel(Material material)
    {
        _materialLazy = new( material );
        SourcedFromOriginalModel = true;
        Name = material.Name;
    }

    public override string ToString()
    {
        return Name;
    }
}

public static class MaterialPresetHelper
{
    private static readonly string _currentPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath );
    public static readonly string BasePresetLibraryPath = System.IO.Path.GetDirectoryName( Application.ExecutablePath ) + @"\Presets\";
    public static readonly string MetaphorPresetLibraryPath = BasePresetLibraryPath + @"Metaphor\";
    public static string GetDefaultMaterialPresetForVersion( uint version )
    {
        var presetLibraryPath = ResourceVersion.IsV2( version ) ? MetaphorPresetLibraryPath : BasePresetLibraryPath;
        var defaultPresetName = ResourceVersion.IsV2( version ) ? $"1_Metaphor_Character_Toon" : "P5_Character";
        var defaultFilePath = Path.Combine( presetLibraryPath, $"{defaultPresetName}.yml" );
        return defaultFilePath;
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ModelConverterDefaultMaterialOptionsViewModel
{
    private readonly ModelConverterOptionsViewModel _parent;
    protected ModelConverterMaterialPresetViewModel _preset;

    [Category( "Material settings" )]
    [DisplayName( "Preset" )]
    [TypeConverter( typeof( ModelConverterMaterialPresetViewModelConverter ) )]
    public ModelConverterMaterialPresetViewModel Preset
    {
        get => _preset;
        set => SetPreset( value );
    }

    [Category( "Material settings" )]
    [DisplayName( "Mesh" )]
    public ModelConverterDefaultMeshOptionsViewModel Mesh { get; set; }

    [Browsable( false )]
    public ModelConverterOptionsViewModel Parent => _parent;

    public ModelConverterDefaultMaterialOptionsViewModel(ModelConverterOptionsViewModel parent)
    {
        _parent = parent;
        Preset = new( MaterialPresetHelper.GetDefaultMaterialPresetForVersion( parent.Version ) );
    }

    protected virtual void SetPreset( ModelConverterMaterialPresetViewModel preset )
    {
        _preset = preset;
        if ( _preset is null )
        {
            Mesh = null;
        }
        else
        {
            Mesh ??= new();
            Mesh.VertexAttributeFlags = _preset.Material.RuntimeMetadata?.VertexAttributeFlags.HasValue ?? false
                ? _preset.Material.RuntimeMetadata.VertexAttributeFlags.Value
                : ModelConverterDefaultMeshOptionsViewModel.DefaultVertexAttributeFlags;
            Mesh.GeometryFlags = _preset.Material.RuntimeMetadata?.GeometryFlags.HasValue ?? false
                ? _preset.Material.RuntimeMetadata.GeometryFlags.Value
                : ModelConverterDefaultMeshOptionsViewModel.DefaultGeometryFlags;
        }
    }

    public override string ToString()
    {
        return string.Empty;
    }
}

[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class ModelConverterMaterialOptionsViewModel : ModelConverterDefaultMaterialOptionsViewModel
{
    [Category( "Material settings" )]
    [DisplayName( "Name" )]
    [Editable( false )]
    [ReadOnly( true )]
    public string Name { get; set; }

    [Category( "Material settings" )]
    [DisplayName( "Inherit from defaults" )]
    public bool InheritDefaults { get; set; } = true;

    public ModelConverterMaterialOptionsViewModel( ModelConverterOptionsViewModel parent ) : base( parent ) { }

    protected override void SetPreset( ModelConverterMaterialPresetViewModel preset )
    {
        InheritDefaults = preset is null;
        base.SetPreset( preset );
    }

    public override string ToString()
    {
        return string.Empty;
    }
}


[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class ModelConverterTexCoordChannelOptionsViewModel
{
    [DisplayName( "Source channel" )]
    public int SourceChannel { get; set; }

    [DisplayName( "Destination channel")]
    public int DestChannel { get; }

    public ModelConverterTexCoordChannelOptionsViewModel(int sourceChannel, int destChannel)
    {
        SourceChannel = sourceChannel;
        DestChannel = destChannel;
    }

    public override string ToString()
    {
        return $"{SourceChannel} -> {DestChannel}";
    }
}

[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class ModelConverterColorChannelOptionsViewModel
{
    [DisplayName( "Source channel" )]
    public int SourceChannel { get; set; }

    [DisplayName( "Destination channel" )]
    public int DestChannel { get; }

    [DisplayName( "Use default color" )]
    public bool UseDefaultColor { get; set; } = true;

    [DisplayName( "Default color" )]
    public System.Drawing.Color DefaultColor { get; set; } = System.Drawing.Color.White;

    [DisplayName( "Swizzle" )]
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public ColorSwizzle Swizzle { get; set; } = new()
    {
        Red = ColorChannel.Red,
        Green = ColorChannel.Green,
        Blue = ColorChannel.Blue,
        Alpha = ColorChannel.Alpha,
    };

    public ModelConverterColorChannelOptionsViewModel( int sourceChannel, int destChannel )
    {
        SourceChannel = sourceChannel;
        DestChannel = destChannel;
    }

    public override string ToString()
    {
        return $"{SourceChannel} -> {DestChannel}";
    }
}

[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class ModelConverterDefaultMeshOptionsViewModel
{
    public static readonly VertexAttributeFlags DefaultVertexAttributeFlags = 
        GFDLibrary.Models.VertexAttributeFlags.Position | GFDLibrary.Models.VertexAttributeFlags.Normal | GFDLibrary.Models.VertexAttributeFlags.TexCoord0;
    public static readonly GeometryFlags DefaultGeometryFlags =
        GFDLibrary.Models.GeometryFlags.Bit7;

    [CustomSortedCategory( "Mesh settings", 1, 2 )]
    [DisplayName( "Geometry flags" )]
    [Editor( typeof( FlagsEnumEditor ), typeof( UITypeEditor ) )]
    public GeometryFlags? GeometryFlags { get; set; }
        = DefaultGeometryFlags;

    [CustomSortedCategory( "Mesh settings", 1, 2 )]
    [DisplayName( "Vertex attribute flags" )]
    [Editor( typeof( FlagsEnumEditor ), typeof( UITypeEditor ) )]
    public VertexAttributeFlags? VertexAttributeFlags { get; set; }
        = DefaultVertexAttributeFlags;

    [CustomSortedCategory("Mesh channel settings", 2, 2)]
    [DisplayName( "UV channel map" )]
    public ModelConverterTexCoordChannelOptionsViewModel[] TexCoordChannelMap { get; set; } = new ModelConverterTexCoordChannelOptionsViewModel[]
    {
        new(0,0),
        new(1,1),
        new(2,2),
    };

    [CustomSortedCategory("Mesh channel settings", 2, 2)]
    [DisplayName( "Color channel map" )]
    public ModelConverterColorChannelOptionsViewModel[] ColorChannelMap { get; set; } = new ModelConverterColorChannelOptionsViewModel[]
    {
        new(0,0),
        new(1,1),
        new(2,2),
    };

    public override string ToString()
    {
        return string.Empty;
    }
}

[TypeConverter( typeof( ExpandableObjectConverter ) )]
public class ModelConverterMeshOptionsViewModel : ModelConverterDefaultMeshOptionsViewModel
{
    [CustomSortedCategory("Mesh settings", 1, 2)]
    [DisplayName( "Name" )]
    [Editable( false )]
    [ReadOnly( true )]
    public string Name { get; set; }

    [CustomSortedCategory("Mesh settings", 1, 2)]
    [DisplayName( "Inherit from material" )]
    public bool InheritDefaults { get; set; } = true;

    [CustomSortedCategory("Mesh channel settings", 2, 2)]
    [DisplayName( "Inherit from material" )]
    public bool InheritChannelSettingsDefaults { get; set; } = true;
}

public class VersionTypeConverter : TypeConverter
{
    private static readonly Dictionary<uint, string> PredefinedVersions = typeof( ResourceVersion )
        .GetFields( BindingFlags.Public | BindingFlags.Static )
        .Where( f => f.FieldType == typeof( uint ) && !f.Name.EndsWith( "ShaderCache" ) )
        .DistinctBy( f => (uint)f.GetValue( null ) )
        .ToDictionary( f => (uint)f.GetValue( null ), f => f.Name );

    public override bool GetStandardValuesSupported( ITypeDescriptorContext context ) => true;

    public override StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
    {
        return new StandardValuesCollection( PredefinedVersions.Keys.OrderBy( k => k ).ToArray() );
    }

    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
    {
        return sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );
    }

    public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
    {
        if ( value is string stringValue )
        {
            stringValue = stringValue.Replace( "0x", "" ).Split( ' ' )[0];
            if ( uint.TryParse( stringValue, NumberStyles.HexNumber, culture, out uint result ) )
            {
                return result;
            }
        }
        return base.ConvertFrom( context, culture, value );
    }

    public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
    {
        if ( destinationType == typeof( string ) && value is uint uintValue )
        {
            string versionName = PredefinedVersions.TryGetValue( uintValue, out string name ) ? name : "Custom";
            return $"0x{uintValue:X8} ({versionName})";
        }
        return base.ConvertTo( context, culture, value, destinationType );
    }
}

public class FlagsEnumEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
    {
        // Indicate this editor uses a dropdown
        return UITypeEditorEditStyle.DropDown;
    }

    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
    {
        if ( provider?.GetService( typeof( IWindowsFormsEditorService ) ) is IWindowsFormsEditorService editorService )
        {
            var enumType = context.PropertyDescriptor.PropertyType;
            if ( enumType.IsConstructedGenericType )
                enumType = enumType.GetGenericArguments()[0];

            var enumValues = Enum.GetValues( enumType );

            CheckedListBox listBox = new CheckedListBox
            {
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };

            // Populate the list with enum flags
            foreach ( var enumValue in enumValues )
            {
                if ( Convert.ToInt64(enumValue) != 0 ) // Exclude "None" or default values if present
                {
                    listBox.Items.Add( enumValue, value is not null && ( (Enum)value ).HasFlag( (Enum)enumValue ) );
                }
            }

            // Display the dropdown
            editorService.DropDownControl( listBox );

            // Combine the selected flags into a single value
            long result = 0;
            foreach ( object item in listBox.CheckedItems )
            {
                result |= Convert.ToInt64(item);
            }

            if ( result == 0 && value is null )
                return null;
            else
                return Enum.ToObject( enumType, result );
        }

        return value;
    }
}

public class ModelConverterMaterialPresetViewModelConverter : TypeConverter
{
    public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
    {
        // Indicate that this converter supports standard values that can be selected from a list.
        return true;
    }

    public override bool GetStandardValuesExclusive( ITypeDescriptorContext context )
    {
        // Indicate that only the standard values are allowed (drop-down list, no free-form editing).
        return true;
    }

    public override StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
    {
        var presets = new List<object>();

        // Add original materials as presets
        if ( context.Instance is ModelConverterDefaultMaterialOptionsViewModel viewModel )
        {
            presets.AddRange( viewModel.Parent.AdditionalMaterialPresets );
        }

        // Fetch all file names from the specified folder as potential options.
        if ( Directory.Exists( MaterialPresetHelper.BasePresetLibraryPath ) )
        {
            var files = Directory.GetFiles( MaterialPresetHelper.BasePresetLibraryPath, "*.yml" ); 
            presets.AddRange(Array.ConvertAll( files, Path.GetFileNameWithoutExtension ));
        }

        if (Directory.Exists( MaterialPresetHelper.MetaphorPresetLibraryPath ) )
        {
            var files = Directory.GetFiles( MaterialPresetHelper.MetaphorPresetLibraryPath, "*.yml" ); 
            presets.AddRange( Array.ConvertAll( files, Path.GetFileNameWithoutExtension ) ); 
        }

        return new StandardValuesCollection( presets );
    }

    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
    {
        // Allow conversion from a string (e.g., user selects a preset name).
        return sourceType == typeof( string ) || sourceType == typeof(Material) || base.CanConvertFrom( context, sourceType );
    }

    public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
    {
        if ( context.Instance is ModelConverterDefaultMaterialOptionsViewModel viewModel )
        {
            var version = viewModel.Parent.Version;
            if ( value is string stringValue )
            {
                var filePath = Path.Combine( MaterialPresetHelper.BasePresetLibraryPath, $"{stringValue}.yml" );
                if ( File.Exists( filePath ) )
                    return new ModelConverterMaterialPresetViewModel( filePath );

                filePath = Path.Combine( MaterialPresetHelper.MetaphorPresetLibraryPath, $"{stringValue}.yml" );
                if ( File.Exists( filePath ) )
                    return new ModelConverterMaterialPresetViewModel( filePath );

                var mat = viewModel.Parent.AdditionalMaterialPresets.FirstOrDefault( m => m.Name == stringValue );
                if ( mat is not null )
                    return new ModelConverterMaterialPresetViewModel( mat );
            }
        }

        return base.ConvertFrom( context, culture, value );
    }
}