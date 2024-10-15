using GFDLibrary.IO;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field08 { get; set; }
        public uint Field20 { get; set; }
        public uint Field150 { get; set; }
        public Vector2 Field08_ { get; set; }
        public Vector2 Field14 { get; set; }
        public EplLeafCommonData FieldEC { get; set; }
        public float Field1C { get; set; }
        public EplLeafCommonData2 Field24 { get; set; }
        public EplLeafCommonData2 Field88 { get; set; }
        public float Field140 { get; set; }
        public float Field144 { get; set; }
        public float Field148 { get; set; }
        public float Field14C { get; set; }
        public Resource Polygon { get; set; }
        public bool Field70 { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplGlitterPolygon() { }
        public EplGlitterPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            Field150 = reader.ReadUInt32();
            Field08_ = reader.ReadVector2();
            Field14 = reader.ReadVector2();
            FieldEC = reader.ReadResource<EplLeafCommonData>( Version );
            Field1C = reader.ReadSingle();
            Field24 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field88 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field140 = reader.ReadSingle();
            Field144 = reader.ReadSingle();
            Field148 = reader.ReadSingle();
            Field14C = reader.ReadSingle();
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplGlitterPolygonExplosion>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplGlitterPolygonSplash>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplGlitterPolygonCylinder>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplGlitterPolygonWall>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            Field70 = reader.ReadBoolean();
            if ( Field70 )
            {
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
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
            writer.WriteUInt32( Field20 );
            writer.WriteUInt32( Field150 );
            writer.WriteVector2( Field08_ );
            writer.WriteVector2( Field14 );
            writer.WriteResource( FieldEC );
            writer.WriteSingle( Field1C );
            writer.WriteResource( Field24 );
            writer.WriteResource( Field88 );
            writer.WriteSingle( Field140 );
            writer.WriteSingle( Field144 );
            writer.WriteSingle( Field148 );
            writer.WriteSingle( Field14C );
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteBoolean( Field70 );
            if ( Field70 )
            {
                writer.WriteResource( EmbeddedFile );
            }
        }
    }

    public sealed class EplGlitterPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonExplosion;

        public Vector2 Field154 { get; set; }
        public Vector2 Field15C { get; set; }
        public Vector2 Field164 { get; set; }
        public Vector2 Field16C { get; set; }
        public float Field174 { get; set; }

        public EplGlitterPolygonExplosion() { }
        public EplGlitterPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadVector2();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadVector2();
            Field16C = reader.ReadVector2();
            Field174 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field154 );
            writer.WriteVector2( Field15C );
            writer.WriteVector2( Field164 );
            writer.WriteVector2( Field16C );
            writer.WriteSingle( Field174 );
        }
    }

    public sealed class EplGlitterPolygonSplash : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonSplash;

        public EplLeafCommonData Field154 { get; set; }
        public EplLeafCommonData Field1A8 { get; set; }
        public Vector2 Field1FC { get; set; }
        public Vector2 Field204 { get; set; }
        public Vector2 Field20C { get; set; }
        public float Field214 { get; set; }

        public EplGlitterPolygonSplash() { }
        public EplGlitterPolygonSplash( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1FC = reader.ReadVector2();
            Field204 = reader.ReadVector2();
            Field20C = reader.ReadVector2();
            Field214 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteResource( Field1A8 );
            writer.WriteVector2( Field1FC );
            writer.WriteVector2( Field204 );
            writer.WriteVector2( Field20C );
            writer.WriteSingle( Field214 );
        }
    }

    public sealed class EplGlitterPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonCylinder;

        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }
        public Vector2 Field1C4 { get; set; }
        public float Field1CC { get; set; }
        public uint Field1D0 { get; set; }

        public EplGlitterPolygonCylinder() { }
        public EplGlitterPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
            Field1C4 = reader.ReadVector2();
            Field1CC = reader.ReadSingle();
            Field1D0 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
            writer.WriteVector2( Field1C4 );
            writer.WriteSingle( Field1CC );
            writer.WriteUInt32( Field1D0 );
        }
    }

    public sealed class EplGlitterPolygonWall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonWall;

        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }

        public EplGlitterPolygonWall() { }
        public EplGlitterPolygonWall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
        }
    }
}
