using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Lights;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class LightViewNode : DataViewNode<Light>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public LightFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Ambient color (float)" )]
        public Vector4 AmbientColor
        {
            get => Data.AmbientColor;
            set => Set( x => x.AmbientColor, value );
        }

        [DisplayName( "Ambient color (RGBA)" )]
        public Color AmbientColorRGBA
        {
            get => Data.AmbientColor.ToByte();
            set => Data.AmbientColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 DiffuseColor
        {
            get => Data.DiffuseColor;
            set => Set( x => x.DiffuseColor, value );
        }

        [DisplayName( "Diffuse color (RGBA)" )]
        public Color DiffuseColorRGBA
        {
            get => Data.DiffuseColor.ToByte();
            set => Data.DiffuseColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 SpecularColor
        {
            get => Data.SpecularColor;
            set => Set( x => x.SpecularColor, value );
        }

        [DisplayName( "Specular color (RGBA)" )]
        public Color SpecularColorRGBA
        {
            get => Data.SpecularColor.ToByte();
            set => Data.SpecularColor = value.ToFloat();
        }

        public LightType Type
        {
            get => Data.Type;
            set => Set( x => x.Type, value );
        }

        public float Field20
        {
            get => Data.Field20;
            set => Set( x => x.Field20, value );
        }

        public float Field14
        {
            get => Data.Field14;
            set => Set( x => x.Field14, value );
        }

        public float Field18
        {
            get => Data.Field18;
            set => Set( x => x.Field18, value );
        }

        public float Field10
        {
            get => Data.Field10;
            set => Set( x => x.Field10, value );
        }

        [DisplayName( "Attenuation start" )]
        public float AttenuationStart
        {
            get => Data.AttenuationStart;
            set => Set( x => x.AttenuationStart, value );
        }

        [DisplayName( "Attenuation end" )]
        public float AttenuationEnd
        {
            get => Data.AttenuationEnd;
            set => Set( x => x.AttenuationEnd, value );
        }

        public float Field60
        {
            get => Data.Field60;
            set => Set( x => x.Field60, value );
        }

        public float Field64
        {
            get => Data.Field64;
            set => Set( x => x.Field64, value );
        }

        public float Field68
        {
            get => Data.Field68;
            set => Set( x => x.Field68, value );
        }

        [DisplayName( "Angle inner cone" )]
        public float AngleInnerCone
        {
            get => Data.AngleInnerCone;
            set => Set( x => x.AngleInnerCone, value );
        }

        [DisplayName( "Angle outer cone" )]
        public float AngleOuterCone
        {
            get => Data.AngleOuterCone;
            set => Set( x => x.AngleOuterCone, value );
        }

        public LightViewNode( string text, Light data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Light>( path => Data.Save( path ) );
            RegisterReplaceHandler<Light>( Resource.Load<Light> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}