using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType3ViewNode : MaterialAttributeViewNode<MaterialAttributeType3>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        public float WaveLengthX
        {
            get => Data.WaveLengthX;
            set => Set( x => x.WaveLengthX, value );
        }

        // 10
        [Browsable( true )]
        public float WaveLengthY
        {
            get => Data.WaveLengthY;
            set => Set( x => x.WaveLengthY, value );
        }

        // 14
        [Browsable( true )]
        public float WaveLengthZ
        {
            get => Data.WaveLengthZ;
            set => Set( x => x.WaveLengthZ, value );
        }

        // 18
        [Browsable( true )]
        public float AmplitudeX
        {
            get => Data.AmplitudeX;
            set => Set( x => x.AmplitudeX, value );
        }

        // 1C
        [Browsable( true )]
        public float AmplitudeY
        {
            get => Data.AmplitudeY;
            set => Set( x => x.AmplitudeY, value );
        }

        // 20
        [Browsable( true )]
        public float AmplitudeZ
        {
            get => Data.AmplitudeZ;
            set => Set( x => x.AmplitudeZ, value );
        }

        // 24
        [Browsable( true )]
        public float SpeedX
        {
            get => Data.SpeedX;
            set => Set( x => x.SpeedX, value );
        }

        // 28
        [Browsable( true )]
        public float SpeedY
        {
            get => Data.SpeedY;
            set => Set( x => x.SpeedY, value );
        }

        // 2C
        [Browsable( true )]
        public float SpeedZ
        {
            get => Data.SpeedZ;
            set => Set( x => x.SpeedZ, value );
        }

        // 30
        [Browsable( true )]
        public float ReflectAlpha
        {
            get => Data.ReflectAlpha;
            set => Set( x => x.ReflectAlpha, value );
        }

        // 34
        [Browsable( true )]
        public float RefractAlpha
        {
            get => Data.RefractAlpha;
            set => Set( x => x.RefractAlpha, value );
        }

        // 38
        [Browsable( true )]
        public float RefractRatio
        {
            get => Data.RefractRatio;
            set => Set( x => x.RefractRatio, value );
        }

        // 3C
        [Browsable( true )]
        public int Type3Flags
        {
            get => Data.Type3Flags;
            set => Set( x => x.Type3Flags, value );
        }


        public MaterialAttributeType3ViewNode( string text, MaterialAttributeType3 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType3> );
        }
    }
}