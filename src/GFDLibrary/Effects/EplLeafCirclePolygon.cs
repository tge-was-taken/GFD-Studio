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
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field10 { get; set; }
        public uint Field1C { get; set; }
        public uint Field20 { get; set; }
        public Vector2 Field08 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplCirclePolygon() { }
        public EplCirclePolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field10 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            Field08 = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                Field14 = reader.ReadSingle();
                Field18 = reader.ReadSingle();
            }
            switch ( Type )
            {

                case 0: break;
                case 1: Polygon = reader.ReadResource<EplCirclePolygonRing>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplCirclePolygonTrajectory>( Version ); break;
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
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field1C );
            writer.WriteUInt32( Field20 );
            writer.WriteVector2( Field08 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field14 );
                writer.WriteSingle( Field18 );
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

        public float Field24 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        public EplLeafCommonData Field7C { get; set; }
        public Vector2 FieldD0 { get; set; }
        public uint FieldD8 { get; set; }
        public uint FieldDC { get; set; }
        public uint FieldE0 { get; set; }

        public EplCirclePolygonRing() { }
        public EplCirclePolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            Field7C = reader.ReadResource<EplLeafCommonData>( Version );
            FieldD0 = reader.ReadVector2();
            FieldD8 = reader.ReadUInt32();
            FieldDC = reader.ReadUInt32();
            FieldE0 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteResource( Field28 );
            writer.WriteResource( Field7C );
            writer.WriteVector2( FieldD0 );
            writer.WriteUInt32( FieldD8 );
            writer.WriteUInt32( FieldDC );
            writer.WriteUInt32( FieldE0 );
        }
    }

    public sealed class EplCirclePolygonTrajectory : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonTrajectory;

        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public EplLeafCommonData Field2C { get; set; }
        public EplLeafCommonData Field90 { get; set; }
        public float FieldF4 { get; set; }
        public float FieldF8 { get; set; }
        public float FieldFC { get; set; }
        public float Field100 { get; set; }

        public EplCirclePolygonTrajectory() { }
        public EplCirclePolygonTrajectory( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadResource<EplLeafCommonData>( Version );
            Field90 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldF4 = reader.ReadSingle();
            FieldF8 = reader.ReadSingle();
            FieldFC = reader.ReadSingle();
            Field100 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteSingle( Field28 );
            writer.WriteResource( Field2C );
            writer.WriteResource( Field90 );
            writer.WriteSingle( FieldF4 );
            writer.WriteSingle( FieldF8 );
            writer.WriteSingle( FieldFC );
            writer.WriteSingle( Field100 );
        }
    }

    public sealed class EplCirclePolygonFill : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonFill;

        public float Field24 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        public EplLeafCommonData2 Field7C { get; set; }
        public EplLeafCommonData2 FieldE0 { get; set; }

        public EplCirclePolygonFill() { }
        public EplCirclePolygonFill( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            Field7C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldE0 = reader.ReadResource<EplLeafCommonData2>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteResource( Field28 );
            writer.WriteResource( Field7C );
            writer.WriteResource( FieldE0 );
        }
    }

    public sealed class EplCirclePolygonHoop : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonHoop;

        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public EplLeafCommonData Field34 { get; set; }
        public Vector2 Field88 { get; set; }
        public EplLeafCommonData2 Field90 { get; set; }
        public EplLeafCommonData2 FieldF4 { get; set; }
        public EplLeafCommonData2 Field158 { get; set; }
        public float Field1BC { get; set; }
        public float Field1C0 { get; set; }

        public EplCirclePolygonHoop() { }
        public EplCirclePolygonHoop( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadResource<EplLeafCommonData>( Version );
            Field88 = reader.ReadVector2();
            Field90 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldF4 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field158 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field1BC = reader.ReadSingle();
            Field1C0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteSingle( Field30 );
            writer.WriteResource( Field34 );
            writer.WriteVector2( Field88 );
            writer.WriteResource( Field90 );
            writer.WriteResource( FieldF4 );
            writer.WriteResource( Field158 );
            writer.WriteSingle( Field1BC );
            writer.WriteSingle( Field1C0 );
        }
    }
}
