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
    public sealed class EplBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplBoardPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field10 { get; set; }
        public uint Field1C { get; set; }
        public Vector2 Field08 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplBoardPolygon() { }
        public EplBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field10 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Field08 = reader.ReadVector2();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplSquareBoardPolygon>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplRectangleBoardPolygon>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
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
            writer.WriteVector2( Field08 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            if ( Polygon != null ) writer.WriteResource( Polygon );
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplSquareBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSquareBoardPolygon;

        public EplLeafCommonData2 Field28 { get; set; }
        public EplLeafCommonData2 Field8C { get; set; }
        public Vector2 FieldFC { get; set; }
        public float FieldF0 { get; set; }
        public float Field104 { get; set; }
        public uint Field108 { get; set; }
        public Vector2 FieldF4 { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }

        public EplSquareBoardPolygon() { }
        public EplSquareBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field8C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldFC = reader.ReadVector2();
            FieldF0 = reader.ReadSingle();
            Field104 = reader.ReadSingle();
            Field108 = reader.ReadUInt32();
            if ( Version > 0x1104280 )
            {
                FieldF4 = reader.ReadVector2();
                if ( Version > 0x1104290 )
                {
                    Field20 = reader.ReadSingle();
                    Field24 = reader.ReadSingle();
                }
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field28 );
            writer.WriteResource( Field8C );
            writer.WriteVector2( FieldFC );
            writer.WriteSingle( FieldF0 );
            writer.WriteSingle( Field104 );
            writer.WriteUInt32( Field108 );
            if ( Version > 0x1104280 )
            {
                writer.WriteVector2( FieldF4 );
                if ( Version > 0x1104290 )
                {
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field24 );
                }
            }
        }
    }

    public sealed class EplRectangleBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplRectangleBoardPolygon;

        public EplLeafCommonData2 Field28 { get; set; }
        public EplLeafCommonData2 Field8C { get; set; }
        public EplLeafCommonData2 FieldF0 { get; set; }
        public Vector2 Field160 { get; set; }
        public float Field154 { get; set; }
        public float Field168 { get; set; }
        public uint Field158 { get; set; }
        public Vector2 Field16C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }

        public EplRectangleBoardPolygon() { }
        public EplRectangleBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field8C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldF0 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field160 = reader.ReadVector2();
            Field154 = reader.ReadSingle();
            Field168 = reader.ReadSingle();
            Field158 = reader.ReadUInt32();
            Field16C = reader.ReadVector2();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field28 );
            writer.WriteResource( Field8C );
            writer.WriteResource( FieldF0 );
            writer.WriteVector2( Field160 );
            writer.WriteSingle( Field154 );
            writer.WriteSingle( Field168 );
            writer.WriteUInt32( Field158 );
            writer.WriteVector2( Field16C );
            writer.WriteSingle( Field20 );
            writer.WriteSingle( Field24 );
        }
    }
}
