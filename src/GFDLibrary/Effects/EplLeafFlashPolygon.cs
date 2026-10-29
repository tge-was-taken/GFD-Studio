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
        [FormerName( "Field08_" )]
        public Vector2 Count { get; set; }
        [FormerName( "Field18" )]
        public Vector2 Fade { get; set; }
        [FormerName( "Field28" )]
        public EplLeafCommonData Scale { get; set; }
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
            Count = reader.ReadVector2();
            Fade = reader.ReadVector2();
            Scale = reader.ReadResource<EplLeafCommonData>( Version );
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
            writer.WriteVector2( Count );
            writer.WriteVector2( Fade );
            writer.WriteResource( Scale );
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

        [FormerName( "Field88" )]

        public uint ColorIn { get; set; }
        [FormerName( "Field8C" )]
        public uint ColorOut { get; set; }
        [FormerName( "Field90" )]
        public Vector2 WidthIn { get; set; }
        [FormerName( "Field98" )]
        public Vector2 WidthOut { get; set; }
        [FormerName( "FieldA0" )]
        public Vector2 Length { get; set; }
        [FormerName( "FieldA8" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "FieldB0" )]
        public float RotateAccele { get; set; }
        [FormerName( "FieldB4" )]
        public uint RotateType { get; set; }

        public EplFlashPolygonRadiation() { }
        public EplFlashPolygonRadiation( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            ColorIn = reader.ReadUInt32();
            ColorOut = reader.ReadUInt32();
            WidthIn = reader.ReadVector2();
            WidthOut = reader.ReadVector2();
            Length = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( ColorIn );
            writer.WriteUInt32( ColorOut );
            writer.WriteVector2( WidthIn );
            writer.WriteVector2( WidthOut );
            writer.WriteVector2( Length );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
        }
    }

    public sealed class EplFlashPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonExplosion;

        [FormerName( "Field88" )]

        public Vector2 Radius { get; set; }
        [FormerName( "Field90" )]
        public uint ColorIn { get; set; }
        [FormerName( "Feild94" )]
        public uint ColorOut { get; set; }
        [FormerName( "Field94" )]
        public Vector2 Width { get; set; }
        [FormerName( "FieldA0" )]
        public Vector2 Length { get; set; }
        [FormerName( "FieldA8" )]
        public Vector2 Speed { get; set; }
        [FormerName( "FieldB0" )]
        public float Accele { get; set; }

        public EplFlashPolygonExplosion() { }
        public EplFlashPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadVector2();
            ColorIn = reader.ReadUInt32();
            ColorOut = reader.ReadUInt32();
            Width = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Radius );
            writer.WriteUInt32( ColorIn );
            writer.WriteUInt32( ColorOut );
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
        }
    }

    public sealed class EplFlashPolygonRing : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonRing;

        [FormerName( "Field88" )]

        public EplLeafCommonData Radius { get; set; }
        [FormerName( "FieldDC" )]
        public EplLeafCommonData Spread { get; set; }
        [FormerName( "Field130" )]
        public uint ColorIn { get; set; }
        [FormerName( "Field134" )]
        public uint ColorOut { get; set; }
        [FormerName( "Field138" )]
        public Vector2 WidthIn { get; set; }
        [FormerName( "Field140" )]
        public Vector2 WidthOut { get; set; }
        [FormerName( "Field148" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field150" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field158" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field15C" )]
        public uint RotateType { get; set; }

        public EplFlashPolygonRing() { }
        public EplFlashPolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Spread = reader.ReadResource<EplLeafCommonData>( Version );
            ColorIn = reader.ReadUInt32();
            ColorOut = reader.ReadUInt32();
            WidthIn = reader.ReadVector2();
            WidthOut = reader.ReadVector2();
            Length = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteResource( Spread );
            writer.WriteUInt32( ColorIn );
            writer.WriteUInt32( ColorOut );
            writer.WriteVector2( WidthIn );
            writer.WriteVector2( WidthOut );
            writer.WriteVector2( Length );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
        }
    }

    public sealed class EplFlashPolygonSplash : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonSplash;

        [FormerName( "Field88" )]

        public EplLeafCommonData Radius { get; set; }
        [FormerName( "FieldDC" )]
        public EplLeafCommonData Spread { get; set; }
        [FormerName( "Field130" )]
        public uint ColorIn { get; set; }
        [FormerName( "Field134" )]
        public uint ColorOut { get; set; }
        [FormerName( "Field138" )]
        public Vector2 WidthIn { get; set; }
        [FormerName( "Field140" )]
        public Vector2 WidthOut { get; set; }
        [FormerName( "Field148" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field150" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field158" )]
        public float Accele { get; set; }

        public EplFlashPolygonSplash() { }
        public EplFlashPolygonSplash( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Spread = reader.ReadResource<EplLeafCommonData>( Version );
            ColorIn = reader.ReadUInt32();
            ColorOut = reader.ReadUInt32();
            WidthIn = reader.ReadVector2();
            WidthOut = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteResource( Spread );
            writer.WriteUInt32( ColorIn );
            writer.WriteUInt32( ColorOut );
            writer.WriteVector2( WidthIn );
            writer.WriteVector2( WidthOut );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
        }
    }

    public sealed class EplFlashPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonCylinder;

        [FormerName( "Field88" )]

        public EplLeafCommonData Radius { get; set; }
        [FormerName( "FieldDC" )]
        public Resource Color { get; set; }
        [FormerName( "Field140" )]
        public Vector2 Width { get; set; }
        [FormerName( "Field148" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field150" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field158" )]
        public float Accele { get; set; }
        [FormerName( "Field15C" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field164" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field168" )]
        public uint RotateType { get; set; }

        public EplFlashPolygonCylinder() { }
        public EplFlashPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            if ( Version <= 0x1104040 )
                Color = reader.ReadResource<EplLeafCommonData>( Version );
            else
                Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Width = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            if ( Version <= 0x1104040 )
                writer.WriteResource( Color );
            else
                writer.WriteResource( Color );
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
        }
    }
}
