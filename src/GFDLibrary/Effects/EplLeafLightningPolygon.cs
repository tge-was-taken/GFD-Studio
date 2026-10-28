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
        public uint Feild20 { get; set; }
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
        public Vector2 Field0C { get; set; }
        public Vector2 Field24 { get; set; }
        public Vector2 Field2C { get; set; }
        public Vector2 Field34 { get; set; }
        public Vector2 Field14 { get; set; }
        public Vector2 Field3C { get; set; }
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
            Feild20 = reader.ReadUInt32();
            WidthCore = reader.ReadSingle();
            WidthEdge = reader.ReadSingle();
            RgbaCore = reader.ReadUInt32();
            RgbaBorder = reader.ReadUInt32();
            RgbaEdge = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            Field0C = reader.ReadVector2();
            Field24 = reader.ReadVector2();
            Field2C = reader.ReadVector2();
            Field34 = reader.ReadVector2();
            Field14 = reader.ReadVector2();
            Field3C = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                AdjustScale = reader.ReadSingle();
                AdjustSpeed = reader.ReadSingle();
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
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteUInt32( Number );
            writer.WriteUInt32( Blend );
            writer.WriteUInt32( Feild20 );
            writer.WriteSingle( WidthCore );
            writer.WriteSingle( WidthEdge );
            writer.WriteUInt32( RgbaCore );
            writer.WriteUInt32( RgbaBorder );
            writer.WriteUInt32( RgbaEdge );
            writer.WriteUInt32( Seed );
            writer.WriteVector2( Field0C );
            writer.WriteVector2( Field24 );
            writer.WriteVector2( Field2C );
            writer.WriteVector2( Field34 );
            writer.WriteVector2( Field14 );
            writer.WriteVector2( Field3C );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( AdjustScale );
                writer.WriteSingle( AdjustSpeed );
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
