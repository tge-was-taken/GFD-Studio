using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplCirclePolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
        [FormerName( "Field10" )]
        public uint Blend { get; set; }
        [FormerName( "Field1C" )]
        public uint Seed { get; set; }
        [FormerName( "Field20" )]
        public uint Split { get; set; }
        [FormerName( "Field08" )]
        public Vector2 Fade { get; set; }
        [FormerName( "Field14" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field18" )]
        public float AdjustSpeed { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplCirclePolygon() { }
        public EplCirclePolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Life = reader.ReadSingle();
            Blend = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            Split = reader.ReadUInt32();
            Fade = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                AdjustScale = reader.ReadSingle();
                AdjustSpeed = reader.ReadSingle();
            }
            switch ( Type )
            {

                case 0: break;
                case 1: Polygon = reader.ReadResource<EplCirclePolygonRing>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplCirclePolygonTrack>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplCirclePolygonFill>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplCirclePolygonHoop>( Version ); break;
                default: throw new NotImplementedException( $"Epl circle polygon type {Type} not implemented" );
            }

            if ( Type != 1 && Type != 3 )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteUInt32( Blend );
            writer.WriteUInt32( Seed );
            writer.WriteUInt32( Split );
            writer.WriteVector2( Fade );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( AdjustScale );
                writer.WriteSingle( AdjustSpeed );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );

            if ( Type != 1 && Type != 3 )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplCirclePolygonRing : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonRing;

        [FormerName( "Field24" )]
        public float Occurrence { get; set; }
        [FormerName( "Field28" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field7C" )]
        public EplLeafCommonData Width { get; set; }
        [FormerName( "FieldD0" )]
        public Vector2 WidthRatio { get; set; }
        [FormerName( "FieldD8" )]
        public uint ColorIn { get; set; }
        [FormerName( "FieldDC" )]
        public uint ColorCenter { get; set; }
        [FormerName( "FieldE0" )]
        public uint ColorOut { get; set; }

        public EplCirclePolygonRing() { }
        public EplCirclePolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Occurrence = reader.ReadSingle();
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Width = reader.ReadResource<EplLeafCommonData>( Version );
            WidthRatio = reader.ReadVector2();
            ColorIn = reader.ReadUInt32();
            ColorCenter = reader.ReadUInt32();
            ColorOut = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Occurrence );
            writer.WriteResource( Radius );
            writer.WriteResource( Width );
            writer.WriteVector2( WidthRatio );
            writer.WriteUInt32( ColorIn );
            writer.WriteUInt32( ColorCenter );
            writer.WriteUInt32( ColorOut );
        }
    }

    [FormerName( "EplCirclePolygonTrajectory" )]
    public sealed class EplCirclePolygonTrack : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonTrack;

        [FormerName( "Field24" )]
        public float Radius { get; set; }
        [FormerName( "Field28" )]
        public float WidthRatio { get; set; }
        [FormerName( "Field2C" )]
        public EplLeafCommonData ColorIn { get; set; }
        [FormerName( "Field90" )]
        public EplLeafCommonData ColorOut { get; set; }
        [FormerName( "FieldF4" )]
        public float Speed { get; set; }
        [FormerName( "FieldF8" )]
        public float Accele { get; set; }
        [FormerName( "FieldFC" )]
        public float RepeatTexU { get; set; }
        [FormerName( "Field100" )]
        public float MoveTexV { get; set; }

        public EplCirclePolygonTrack() { }
        public EplCirclePolygonTrack( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadSingle();
            WidthRatio = reader.ReadSingle();
            ColorIn = reader.ReadResource<EplLeafCommonData>( Version );
            ColorOut = reader.ReadResource<EplLeafCommonData>( Version );
            Speed = reader.ReadSingle();
            Accele = reader.ReadSingle();
            RepeatTexU = reader.ReadSingle();
            MoveTexV = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Radius );
            writer.WriteSingle( WidthRatio );
            writer.WriteResource( ColorIn );
            writer.WriteResource( ColorOut );
            writer.WriteSingle( Speed );
            writer.WriteSingle( Accele );
            writer.WriteSingle( RepeatTexU );
            writer.WriteSingle( MoveTexV );
        }
    }

    public sealed class EplCirclePolygonFill : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonFill;

        [FormerName( "Field24" )]
        public float Occurrence { get; set; }
        [FormerName( "Field28" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field7C" )]
        public EplLeafCommonData2 ColorIn { get; set; }
        [FormerName( "FieldE0" )]
        public EplLeafCommonData2 ColorOut { get; set; }

        public EplCirclePolygonFill() { }
        public EplCirclePolygonFill( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Occurrence = reader.ReadSingle();
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            ColorIn = reader.ReadResource<EplLeafCommonData2>( Version );
            ColorOut = reader.ReadResource<EplLeafCommonData2>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Occurrence );
            writer.WriteResource( Radius );
            writer.WriteResource( ColorIn );
            writer.WriteResource( ColorOut );
        }
    }

    public sealed class EplCirclePolygonHoop : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonHoop;

        [FormerName( "Field28" )]
        public float Radius { get; set; }
        [FormerName( "Field2C" )]
        public float Speed { get; set; }
        [FormerName( "Field30" )]
        public float Accele { get; set; }
        [FormerName( "Field34" )]
        public EplLeafCommonData Width { get; set; }
        [FormerName( "Field88" )]
        public Vector2 WidthRatio { get; set; }
        [FormerName( "Field90" )]
        public EplLeafCommonData2 ColorIn { get; set; }
        [FormerName( "FieldF4" )]
        public EplLeafCommonData2 ColorCenter { get; set; }
        [FormerName( "Field158" )]
        public EplLeafCommonData2 ColorOut { get; set; }
        [FormerName( "Field1BC" )]
        public float RepeatTexU { get; set; }
        [FormerName( "Field1C0" )]
        public float MoveTexV { get; set; }

        public EplCirclePolygonHoop() { }
        public EplCirclePolygonHoop( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadSingle();
            Speed = reader.ReadSingle();
            Accele = reader.ReadSingle();
            Width = reader.ReadResource<EplLeafCommonData>( Version );
            WidthRatio = reader.ReadVector2();
            ColorIn = reader.ReadResource<EplLeafCommonData2>( Version );
            ColorCenter = reader.ReadResource<EplLeafCommonData2>( Version );
            ColorOut = reader.ReadResource<EplLeafCommonData2>( Version );
            RepeatTexU = reader.ReadSingle();
            MoveTexV = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Radius );
            writer.WriteSingle( Speed );
            writer.WriteSingle( Accele );
            writer.WriteResource( Width );
            writer.WriteVector2( WidthRatio );
            writer.WriteResource( ColorIn );
            writer.WriteResource( ColorCenter );
            writer.WriteResource( ColorOut );
            writer.WriteSingle( RepeatTexU );
            writer.WriteSingle( MoveTexV );
        }
    }
}
