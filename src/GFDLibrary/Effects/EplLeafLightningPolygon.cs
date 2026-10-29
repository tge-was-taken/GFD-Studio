using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplLightningPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
        [FormerName( "Field08" )]
        public uint Number { get; set; }
        [FormerName( "Field1C" )]
        public uint Blend { get; set; }
        [FormerName( "Feild20" )]
        public uint Split { get; set; }
        [FormerName( "Field44" )]
        public float WidthCore { get; set; }
        [FormerName( "Field48" )]
        public float WidthEdge { get; set; }
        [FormerName( "Field4C" )]
        public uint RgbaCore { get; set; }
        [FormerName( "Field50" )]
        public uint RgbaBorder { get; set; }
        [FormerName( "Field54" )]
        public uint RgbaEdge { get; set; }
        [FormerName( "Field60" )]
        public uint Seed { get; set; }
        [FormerName( "Field0C" )]
        public Vector2 Count { get; set; }
        [FormerName( "Field24" )]
        public Vector2 Wave { get; set; }
        [FormerName( "Field2C" )]
        public Vector2 WaveHeight { get; set; }
        [FormerName( "Field34" )]
        public Vector2 Uneven { get; set; }
        [FormerName( "Field14" )]
        public Vector2 Fade { get; set; }
        [FormerName( "Field3C" )]
        public Vector2 BothEnds { get; set; }
        [FormerName( "Field58" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field5C" )]
        public float AdjustSpeed { get; set; }
        public Resource Polygon { get; set; }

        public EplLightningPolygon() { }
        public EplLightningPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Life = reader.ReadSingle();
            Number = reader.ReadUInt32();
            Blend = reader.ReadUInt32();
            Split = reader.ReadUInt32();
            WidthCore = reader.ReadSingle();
            WidthEdge = reader.ReadSingle();
            RgbaCore = reader.ReadUInt32();
            RgbaBorder = reader.ReadUInt32();
            RgbaEdge = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            Count = reader.ReadVector2();
            Wave = reader.ReadVector2();
            WaveHeight = reader.ReadVector2();
            Uneven = reader.ReadVector2();
            Fade = reader.ReadVector2();
            BothEnds = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                AdjustScale = reader.ReadSingle();
                AdjustSpeed = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplLightningPolygonBar>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplLightningPolygonSphere>( Version ); break;
                    //         default: Assert(false, "Not implemented"); break;
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteUInt32( Number );
            writer.WriteUInt32( Blend );
            writer.WriteUInt32( Split );
            writer.WriteSingle( WidthCore );
            writer.WriteSingle( WidthEdge );
            writer.WriteUInt32( RgbaCore );
            writer.WriteUInt32( RgbaBorder );
            writer.WriteUInt32( RgbaEdge );
            writer.WriteUInt32( Seed );
            writer.WriteVector2( Count );
            writer.WriteVector2( Wave );
            writer.WriteVector2( WaveHeight );
            writer.WriteVector2( Uneven );
            writer.WriteVector2( Fade );
            writer.WriteVector2( BothEnds );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( AdjustScale );
                writer.WriteSingle( AdjustSpeed );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
        }
    }

    [FormerName( "EplLightningPolygonRod" )]
    public sealed class EplLightningPolygonBar : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonBar;

        [FormerName( "Field64" )]
        public Vector2 Length { get; set; }

        public EplLightningPolygonBar() { }
        public EplLightningPolygonBar( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Length = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Length );
        }
    }

    [FormerName( "EplLightningPolygonBall" )]
    public sealed class EplLightningPolygonSphere : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonSphere;

        [FormerName( "Field64" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "FieldB8" )]
        public Vector2 Round { get; set; }
        [FormerName( "FieldC0" )]
        public Vector2 RotateYSpeed { get; set; }
        [FormerName( "FieldC8" )]
        public float RotateYAccele { get; set; }
        [FormerName( "FieldCC" )]
        public uint RotateYType { get; set; }
        [FormerName( "FieldD0" )]
        public Vector2 RotateZSpeed { get; set; }
        [FormerName( "FieldD8" )]
        public float RotateZAccele { get; set; }
        [FormerName( "FieldDC" )]
        public uint RotateZType { get; set; }

        public EplLightningPolygonSphere() { }
        public EplLightningPolygonSphere( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Round = reader.ReadVector2();
            RotateYSpeed = reader.ReadVector2();
            RotateYAccele = reader.ReadSingle();
            RotateYType = reader.ReadUInt32();
            RotateZSpeed = reader.ReadVector2();
            RotateZAccele = reader.ReadSingle();
            RotateZType = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteVector2( Round );
            writer.WriteVector2( RotateYSpeed );
            writer.WriteSingle( RotateYAccele );
            writer.WriteUInt32( RotateYType );
            writer.WriteVector2( RotateZSpeed );
            writer.WriteSingle( RotateZAccele );
            writer.WriteUInt32( RotateZType );
        }
    }
}
