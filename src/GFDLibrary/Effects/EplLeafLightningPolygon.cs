using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplLightningPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field08 { get; set; }
        public uint Field1C { get; set; }
        public uint Feild20 { get; set; }
        public float Field44 { get; set; }
        public float Field48 { get; set; }
        public uint Field4C { get; set; }
        public uint Field50 { get; set; }
        public uint Field54 { get; set; }
        public uint Field60 { get; set; }
        public Vector2 Field0C { get; set; }
        public Vector2 Field24 { get; set; }
        public Vector2 Field2C { get; set; }
        public Vector2 Field34 { get; set; }
        public Vector2 Field14 { get; set; }
        public Vector2 Field3C { get; set; }
        public float Field58 { get; set; }
        public float Field5C { get; set; }
        public Resource Polygon { get; set; }

        public EplLightningPolygon() { }
        public EplLightningPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Feild20 = reader.ReadUInt32();
            Field44 = reader.ReadSingle();
            Field48 = reader.ReadSingle();
            Field4C = reader.ReadUInt32();
            Field50 = reader.ReadUInt32();
            Field54 = reader.ReadUInt32();
            Field60 = reader.ReadUInt32();
            Field0C = reader.ReadVector2();
            Field24 = reader.ReadVector2();
            Field2C = reader.ReadVector2();
            Field34 = reader.ReadVector2();
            Field14 = reader.ReadVector2();
            Field3C = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                Field58 = reader.ReadSingle();
                Field5C = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplLightningPolygonRod>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplLightningPolygonBall>( Version ); break;
                    //         default: Assert(false, "Not implemented"); break;
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field08 );
            writer.WriteUInt32( Field1C );
            writer.WriteUInt32( Feild20 );
            writer.WriteSingle( Field44 );
            writer.WriteSingle( Field48 );
            writer.WriteUInt32( Field4C );
            writer.WriteUInt32( Field50 );
            writer.WriteUInt32( Field54 );
            writer.WriteUInt32( Field60 );
            writer.WriteVector2( Field0C );
            writer.WriteVector2( Field24 );
            writer.WriteVector2( Field2C );
            writer.WriteVector2( Field34 );
            writer.WriteVector2( Field14 );
            writer.WriteVector2( Field3C );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field58 );
                writer.WriteSingle( Field5C );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
        }
    }

    public sealed class EplLightningPolygonRod : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonRod;

        public Vector2 Field64 { get; set; }

        public EplLightningPolygonRod() { }
        public EplLightningPolygonRod( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field64 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field64 );
        }
    }

    public sealed class EplLightningPolygonBall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonBall;

        public EplLeafCommonData Field64 { get; set; }
        public Vector2 FieldB8 { get; set; }
        public Vector2 FieldC0 { get; set; }
        public float FieldC8 { get; set; }
        public uint FieldCC { get; set; }
        public Vector2 FieldD0 { get; set; }
        public float FieldD8 { get; set; }
        public uint FieldDC { get; set; }

        public EplLightningPolygonBall() { }
        public EplLightningPolygonBall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field64 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldB8 = reader.ReadVector2();
            FieldC0 = reader.ReadVector2();
            FieldC8 = reader.ReadSingle();
            FieldCC = reader.ReadUInt32();
            FieldD0 = reader.ReadVector2();
            FieldD8 = reader.ReadSingle();
            FieldDC = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field64 );
            writer.WriteVector2( FieldB8 );
            writer.WriteVector2( FieldC0 );
            writer.WriteSingle( FieldC8 );
            writer.WriteUInt32( FieldCC );
            writer.WriteVector2( FieldD0 );
            writer.WriteSingle( FieldD8 );
            writer.WriteUInt32( FieldDC );
        }
    }
}
