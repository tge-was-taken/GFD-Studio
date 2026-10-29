using GFDLibrary.IO;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public class EplGlitterPolygon2 : EplGlitterPolygon
    {

    }

    public class EplGlitterPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
        [FormerName( "Field08" )]
        public uint Number { get; set; }
        [FormerName( "Field20" )]
        public uint Blend { get; set; }
        [FormerName( "Field150" )]
        public uint Seed { get; set; }
        [FormerName( "Field08_" )]
        public Vector2 Count { get; set; }
        [FormerName( "Field14" )]
        public Vector2 Fade { get; set; }
        [FormerName( "FieldEC" )]
        public EplLeafCommonData Scale { get; set; }
        [FormerName( "Field1C" )]
        public float Random { get; set; }
        [FormerName( "Field24" )]
        public EplLeafCommonData2 ColorIn { get; set; }
        [FormerName( "Field88" )]
        public EplLeafCommonData2 ColorOut { get; set; }
        [FormerName( "Field140" )]
        public float RepeatTexV { get; set; }
        [FormerName( "Field144" )]
        public float MoveTexV { get; set; }
        [FormerName( "Field148" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field14C" )]
        public float AdjustSpeed { get; set; }
        public Resource Polygon { get; set; }
        [FormerName( "Field70" )]
        public bool HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplGlitterPolygon() { }
        public EplGlitterPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Life = reader.ReadSingle();
            Number = reader.ReadUInt32();
            Blend = reader.ReadUInt32();
            Seed = reader.ReadUInt32();
            Count = reader.ReadVector2();
            Fade = reader.ReadVector2();
            Scale = reader.ReadResource<EplLeafCommonData>( Version );
            Random = reader.ReadSingle();
            ColorIn = reader.ReadResource<EplLeafCommonData2>( Version );
            ColorOut = reader.ReadResource<EplLeafCommonData2>( Version );
            RepeatTexV = reader.ReadSingle();
            MoveTexV = reader.ReadSingle();
            AdjustScale = reader.ReadSingle();
            AdjustSpeed = reader.ReadSingle();
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplGlitterPolygonSpark>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplGlitterPolygonScatter>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplGlitterPolygonCylinder>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplGlitterPolygonWall>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            HasEmbeddedFile = reader.ReadBoolean();
            if ( HasEmbeddedFile )
            {
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
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
            writer.WriteUInt32( Seed );
            writer.WriteVector2( Count );
            writer.WriteVector2( Fade );
            writer.WriteResource( Scale );
            writer.WriteSingle( Random );
            writer.WriteResource( ColorIn );
            writer.WriteResource( ColorOut );
            writer.WriteSingle( RepeatTexV );
            writer.WriteSingle( MoveTexV );
            writer.WriteSingle( AdjustScale );
            writer.WriteSingle( AdjustSpeed );
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
            {
                writer.WriteResource( EmbeddedFile );
            }
        }
    }

    [FormerName( "EplGlitterPolygonExplosion" )]
    public sealed class EplGlitterPolygonSpark : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonSpark;

        [FormerName( "Field154" )]
        public Vector2 Radius { get; set; }
        [FormerName( "Field15C" )]
        public Vector2 Width { get; set; }
        [FormerName( "Field164" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field16C" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field174" )]
        public float Accele { get; set; }

        public EplGlitterPolygonSpark() { }
        public EplGlitterPolygonSpark( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadVector2();
            Width = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Radius );
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
        }
    }

    [FormerName( "EplGlitterPolygonSplash" )]
    public sealed class EplGlitterPolygonScatter : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonScatter;

        [FormerName( "Field154" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field1A8" )]
        public EplLeafCommonData Spread { get; set; }
        [FormerName( "Field1FC" )]
        public Vector2 Width { get; set; }
        [FormerName( "Field204" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field20C" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field214" )]
        public float Accele { get; set; }

        public EplGlitterPolygonScatter() { }
        public EplGlitterPolygonScatter( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
            Spread = reader.ReadResource<EplLeafCommonData>( Version );
            Width = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Radius );
            writer.WriteResource( Spread );
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
        }
    }

    public sealed class EplGlitterPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonCylinder;

        [FormerName( "Field154" )]
        public EplLeafCommonData Radius { get; set; }
        [FormerName( "Field1A8" )]
        public Vector2 Width { get; set; }
        [FormerName( "Field1B0" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field1B8" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field1C0" )]
        public float Accele { get; set; }
        [FormerName( "Field1C4" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field1CC" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field1D0" )]
        public uint RotateType { get; set; }

        public EplGlitterPolygonCylinder() { }
        public EplGlitterPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Radius = reader.ReadResource<EplLeafCommonData>( Version );
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
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
        }
    }

    public sealed class EplGlitterPolygonWall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonWall;

        [FormerName( "Field154" )]
        public EplLeafCommonData Distance { get; set; }
        [FormerName( "Field1A8" )]
        public Vector2 Width { get; set; }
        [FormerName( "Field1B0" )]
        public Vector2 Length { get; set; }
        [FormerName( "Field1B8" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field1C0" )]
        public float Accele { get; set; }

        public EplGlitterPolygonWall() { }
        public EplGlitterPolygonWall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Distance = reader.ReadResource<EplLeafCommonData>( Version );
            Width = reader.ReadVector2();
            Length = reader.ReadVector2();
            Speed = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Distance );
            writer.WriteVector2( Width );
            writer.WriteVector2( Length );
            writer.WriteVector2( Speed );
            writer.WriteSingle( Accele );
        }
    }
}
