using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplWindPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field78 { get; set; }
        public uint Field88 { get; set; }
        public uint Field8C { get; set; }
        public uint FieldA8 { get; set; }
        public Resource Field14 { get; set; }
        public Vector2 Field0C { get; set; }
        public Vector2 Field90 { get; set; }
        public Vector2 Field98 { get; set; }
        public Vector2 Field7C { get; set; }
        public float Field84 { get; set; }
        public float FieldA0 { get; set; }
        public float FieldA4 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile Field70 { get; set; }

        public EplWindPolygon() { }
        public EplWindPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field78 = reader.ReadSingle();
            Field88 = reader.ReadUInt32();
            Field8C = reader.ReadUInt32();
            FieldA8 = reader.ReadUInt32();
            if ( Version <= 0x1104040 )
                Field14 = reader.ReadResource<EplLeafCommonData>( Version );
            else
                Field14 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field0C = reader.ReadVector2();
            Field90 = reader.ReadVector2();
            Field98 = reader.ReadVector2();
            Field7C = reader.ReadVector2();
            if ( Version > 0x1104700 )
                Field84 = reader.ReadSingle();
            if ( Version > 0x1104050 )
            {
                FieldA0 = reader.ReadSingle();
                FieldA4 = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 1: Polygon = reader.ReadResource<EplWindPolygonSpiral>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplWindPolygonExplosion>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplWindPolygonBall>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            Field70 = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field78 );
            writer.WriteUInt32( Field88 );
            writer.WriteUInt32( Field8C );
            writer.WriteUInt32( FieldA8 );
            if ( Version <= 0x1104040 )
                writer.WriteResource( Field14 );
            else
                writer.WriteResource( Field14 );
            writer.WriteVector2( Field0C );
            writer.WriteVector2( Field90 );
            writer.WriteVector2( Field98 );
            writer.WriteVector2( Field7C );
            if ( Version > 0x1104700 )
                writer.WriteSingle( Field84 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( FieldA0 );
                writer.WriteSingle( FieldA4 );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteResource( Field70 );
        }
    }
    public sealed class EplWindPolygonSpiral : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonSpiral;

        public EplLeafCommonData FieldAC { get; set; }
        public EplLeafCommonData Field100 { get; set; }
        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }
        public uint Field1C4 { get; set; }
        public Vector2 Field1C8 { get; set; }

        public EplWindPolygonSpiral() { }
        public EplWindPolygonSpiral( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadResource<EplLeafCommonData>( Version );
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
            Field1C4 = reader.ReadUInt32();
            Field1C8 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteResource( Field100 );
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
            writer.WriteUInt32( Field1C4 );
            writer.WriteVector2( Field1C8 );
        }
    }

    public sealed class EplWindPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonExplosion;

        public EplLeafCommonData FieldAC { get; set; }
        public Vector2 Field100 { get; set; }
        public Vector2 Field108 { get; set; }
        public float Field110 { get; set; }
        public float Field114 { get; set; }
        public Vector2 Field118 { get; set; }

        public EplWindPolygonExplosion() { }
        public EplWindPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadVector2();
            Field108 = reader.ReadVector2();
            Field110 = reader.ReadSingle();
            Field114 = reader.ReadSingle();
            if ( Version > 0x1104080 )
                Field118 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteVector2( Field100 );
            writer.WriteVector2( Field108 );
            writer.WriteSingle( Field110 );
            writer.WriteSingle( Field114 );
            if ( Version > 0x1104080 )
                writer.WriteVector2( Field118 );
        }
    }

    public sealed class EplWindPolygonBall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonBall;

        public EplLeafCommonData FieldAC { get; set; }
        public EplLeafCommonData Field100 { get; set; }
        public Vector2 Field154 { get; set; }
        public Vector2 Field15C { get; set; }
        public float Field164 { get; set; }
        public uint Field168 { get; set; }
        public Vector2 Field16C { get; set; }

        public EplWindPolygonBall() { }
        public EplWindPolygonBall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadResource<EplLeafCommonData>( Version );
            Field154 = reader.ReadVector2();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadSingle();
            Field168 = reader.ReadUInt32();
            Field16C = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteResource( Field100 );
            writer.WriteVector2( Field154 );
            writer.WriteVector2( Field15C );
            writer.WriteSingle( Field164 );
            writer.WriteUInt32( Field168 );
            writer.WriteVector2( Field16C );
        }
    }
}
