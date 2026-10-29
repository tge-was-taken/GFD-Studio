using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplWindPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public float Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
        [FormerName( "Field08" )]
        public float Number { get; set; }
        [FormerName( "Field78" )]
        public float Alpha { get; set; }
        [FormerName( "Field88" )]
        public uint Blend { get; set; }
        [FormerName( "Field8C" )]
        public uint Split { get; set; }
        [FormerName( "FieldA8" )]
        public uint Seed { get; set; }
        [FormerName( "Field14" )]
        public Resource Color { get; set; }
        [FormerName( "Field0C" )]
        public Vector2 Count { get; set; }
        [FormerName( "Field90" )]
        public Vector2 RepeatTexV { get; set; }
        [FormerName( "Field98" )]
        public Vector2 MoveTexV { get; set; }
        [FormerName( "Field7C" )]
        public Vector2 Fade { get; set; }
        [FormerName( "Field84" )]
        public float Random { get; set; }
        [FormerName( "FieldA0" )]
        public float AdjustScale { get; set; }
        [FormerName( "FieldA4" )]
        public float AdjustSpeed { get; set; }
        public Resource Polygon { get; set; }
        [FormerName( "Field70" )]
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplWindPolygon() { }
        public EplWindPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Flags = reader.ReadSingle();
            Life = reader.ReadSingle();
            Number = reader.ReadSingle();
            Alpha = reader.ReadSingle();
            Blend = reader.ReadUInt32();
            Split = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            if ( Version <= 0x1104040 )
                Color = reader.ReadResource<EplLeafCommonData>( Version );
            else
                Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Count = reader.ReadVector2();
            RepeatTexV = reader.ReadVector2();
            MoveTexV = reader.ReadVector2();
            Fade = reader.ReadVector2();
            if ( Version > 0x1104700 )
                Random = reader.ReadSingle();
            if ( Version > 0x1104050 )
            {
                AdjustScale = reader.ReadSingle();
                AdjustSpeed = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 1: Polygon = reader.ReadResource<EplWindPolygonSpiral>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplWindPolygonSpark>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplWindPolygonSphere>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteSingle( Flags );
            writer.WriteSingle( Life );
            writer.WriteSingle( Number );
            writer.WriteSingle( Alpha );
            writer.WriteUInt32( Blend );
            writer.WriteUInt32( Split );
            writer.WriteUInt32( Seed );
            if ( Version <= 0x1104040 )
                writer.WriteResource( Color );
            else
                writer.WriteResource( Color );
            writer.WriteVector2( Count );
            writer.WriteVector2( RepeatTexV );
            writer.WriteVector2( MoveTexV );
            writer.WriteVector2( Fade );
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
    public sealed class EplWindPolygonSpiral : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonSpiral;

        [FormerName( "FieldAC" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field100" )]
        public EplLeafCommonData Height { get; set; }
        [FormerName( "Field154" )]
        public EplLeafCommonData Width { get; set; }
        [FormerName( "Field1A8" )]
        public Vector2 Slant { get; set; }
        [FormerName( "Field1B0" )]
        public Vector2 Round { get; set; }
        [FormerName( "Field1B8" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field1C0" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field1C4" )]
        public uint RotateType { get; set; }
        [FormerName( "Field1C8" )]
        public Vector2 BothEnds { get; set; }

        public EplWindPolygonSpiral() { }
        public EplWindPolygonSpiral( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Height = reader.ReadResource<EplLeafCommonData>( Version );
            Width = reader.ReadResource<EplLeafCommonData>( Version );
            Slant = reader.ReadVector2();
            Round = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
            BothEnds = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteResource( Height );
            writer.WriteResource( Width );
            writer.WriteVector2( Slant );
            writer.WriteVector2( Round );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
            writer.WriteVector2( BothEnds );
        }
    }

    [FormerName( "EplWindPolygonExplosion" )]
    public sealed class EplWindPolygonSpark : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonSpark;

        [FormerName( "FieldAC" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field100" )]
        public Vector2 WidthRatio { get; set; }
        [FormerName( "Field108" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field110" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field114" )]
        public float RotateType { get; set; }
        [FormerName( "Field118" )]
        public Vector2 MoveTexV { get; set; }

        public EplWindPolygonSpark() { }
        public EplWindPolygonSpark( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            WidthRatio = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadSingle();
            if ( Version > 0x1104080 )
                MoveTexV = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteVector2( WidthRatio );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteSingle( RotateType );
            if ( Version > 0x1104080 )
                writer.WriteVector2( MoveTexV );
        }
    }

    [FormerName( "EplWindPolygonBall" )]
    public sealed class EplWindPolygonSphere : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonSphere;

        [FormerName( "FieldAC" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field100" )]
        public EplLeafCommonData Width { get; set; }
        [FormerName( "Field154" )]
        public Vector2 Round { get; set; }
        [FormerName( "Field15C" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field164" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field168" )]
        public uint RotateType { get; set; }
        [FormerName( "Field16C" )]
        public Vector2 BothEnds { get; set; }

        public EplWindPolygonSphere() { }
        public EplWindPolygonSphere( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Width = reader.ReadResource<EplLeafCommonData>( Version );
            Round = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
            BothEnds = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteResource( Width );
            writer.WriteVector2( Round );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
            writer.WriteVector2( BothEnds );
        }
    }
}
