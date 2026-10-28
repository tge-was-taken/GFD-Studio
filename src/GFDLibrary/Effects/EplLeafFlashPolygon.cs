using GFDLibrary.IO;
using System;
using System.Numerics;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplFlashPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
        [FormerName( "Field08" )]
        public uint Number { get; set; }
        [FormerName( "Field14" )]
        public float Alpha { get; set; }
        [FormerName( "Field24" )]
        public uint Blend { get; set; }
        [FormerName( "Field84" )]
        public uint Seed { get; set; }
        public Vector2 Field08_ { get; set; }
        public Vector2 Field18 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        [FormerName( "Field20" )]
        public float Random { get; set; }
        [FormerName( "Field7C" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field80" )]
        public float AdjustSpeed { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplFlashPolygon() { }
        public EplFlashPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Life = reader.ReadSingle();
            Number = reader.ReadUInt32();
            Alpha = reader.ReadSingle();
            Blend = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            Field08_ = reader.ReadVector2();
            Field18 = reader.ReadVector2();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            if ( Version > 0x1104700 )
                Random = reader.ReadSingle();
            if ( Version > 0x1104050 )
            {
                AdjustScale = reader.ReadSingle();
                AdjustSpeed = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplFlashPolygonRadiation>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplFlashPolygonExplosion>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplFlashPolygonRing>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplFlashPolygonSplash>( Version ); break;
                case 5: Polygon = reader.ReadResource<EplFlashPolygonCylinder>( Version ); break;
                default: throw new NotImplementedException( $"Epl flash polygon type {Type} not implemented" );
            }
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteUInt32( Number );
            writer.WriteSingle( Alpha );
            writer.WriteUInt32( Blend );
            writer.WriteUInt32( Seed );
            writer.WriteVector2( Field08_ );
            writer.WriteVector2( Field18 );
            writer.WriteResource( Field28 );
            if ( Version > 0x1104700 )
                writer.WriteSingle( Random );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( AdjustScale );
                writer.WriteSingle( AdjustSpeed );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplFlashPolygonRadiation : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonRadiation;

        public uint Field88 { get; set; }
        public uint Field8C { get; set; }
        public Vector2 Field90 { get; set; }
        public Vector2 Field98 { get; set; }
        public Vector2 FieldA0 { get; set; }
        public Vector2 FieldA8 { get; set; }
        public float FieldB0 { get; set; }
        public uint FieldB4 { get; set; }

        public EplFlashPolygonRadiation() { }
        public EplFlashPolygonRadiation( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadUInt32();
            Field8C = reader.ReadUInt32();
            Field90 = reader.ReadVector2();
            Field98 = reader.ReadVector2();
            FieldA0 = reader.ReadVector2();
            FieldA8 = reader.ReadVector2();
            FieldB0 = reader.ReadSingle();
            FieldB4 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Field88 );
            writer.WriteUInt32( Field8C );
            writer.WriteVector2( Field90 );
            writer.WriteVector2( Field98 );
            writer.WriteVector2( FieldA0 );
            writer.WriteVector2( FieldA8 );
            writer.WriteSingle( FieldB0 );
            writer.WriteUInt32( FieldB4 );
        }
    }

    public sealed class EplFlashPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonExplosion;

        public Vector2 Field88 { get; set; }
        public uint Field90 { get; set; }
        public uint Feild94 { get; set; }
        public Vector2 Field94 { get; set; }
        public Vector2 FieldA0 { get; set; }
        public Vector2 FieldA8 { get; set; }
        public float FieldB0 { get; set; }

        public EplFlashPolygonExplosion() { }
        public EplFlashPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadVector2();
            Field90 = reader.ReadUInt32();
            Feild94 = reader.ReadUInt32();
            Field94 = reader.ReadVector2();
            FieldA0 = reader.ReadVector2();
            FieldA8 = reader.ReadVector2();
            FieldB0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field88 );
            writer.WriteUInt32( Field90 );
            writer.WriteUInt32( Feild94 );
            writer.WriteVector2( Field94 );
            writer.WriteVector2( FieldA0 );
            writer.WriteVector2( FieldA8 );
            writer.WriteSingle( FieldB0 );
        }
    }

    public sealed class EplFlashPolygonRing : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonRing;

        public EplLeafCommonData Field88 { get; set; }
        public EplLeafCommonData FieldDC { get; set; }
        public uint Field130 { get; set; }
        public uint Field134 { get; set; }
        public Vector2 Field138 { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }
        public uint Field15C { get; set; }

        public EplFlashPolygonRing() { }
        public EplFlashPolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            Field130 = reader.ReadUInt32();
            Field134 = reader.ReadUInt32();
            Field138 = reader.ReadVector2();
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
            Field15C = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            writer.WriteResource( FieldDC );
            writer.WriteUInt32( Field130 );
            writer.WriteUInt32( Field134 );
            writer.WriteVector2( Field138 );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
            writer.WriteUInt32( Field15C );
        }
    }

    public sealed class EplFlashPolygonSplash : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonSplash;

        public EplLeafCommonData Field88 { get; set; }
        public EplLeafCommonData FieldDC { get; set; }
        public uint Field130 { get; set; }
        public uint Field134 { get; set; }
        public Vector2 Field138 { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }

        public EplFlashPolygonSplash() { }
        public EplFlashPolygonSplash( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            Field130 = reader.ReadUInt32();
            Field134 = reader.ReadUInt32();
            Field138 = reader.ReadVector2();
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            writer.WriteResource( FieldDC );
            writer.WriteUInt32( Field130 );
            writer.WriteUInt32( Field134 );
            writer.WriteVector2( Field138 );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
        }
    }

    public sealed class EplFlashPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonCylinder;

        public EplLeafCommonData Field88 { get; set; }
        public Resource FieldDC { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }
        public Vector2 Field15C { get; set; }
        public float Field164 { get; set; }
        public uint Field168 { get; set; }

        public EplFlashPolygonCylinder() { }
        public EplFlashPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            if ( Version <= 0x1104040 )
                FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            else
                FieldDC = reader.ReadResource<EplLeafCommonData2>( Version );
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadSingle();
            Field168 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            if ( Version <= 0x1104040 )
                writer.WriteResource( FieldDC );
            else
                writer.WriteResource( FieldDC );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
            writer.WriteVector2( Field15C );
            writer.WriteSingle( Field164 );
            writer.WriteUInt32( Field168 );
        }
    }
}
