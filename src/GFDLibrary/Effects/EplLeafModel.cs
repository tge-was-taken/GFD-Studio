using GFDLibrary.IO;
using System.Numerics;
using System.Diagnostics;

namespace GFDLibrary.Effects
{
    public sealed class EplModel : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public Resource Data { get; set; }
        public byte HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public Vector2 Field0C { get; set; }
        public Vector2 Field14 { get; set; }
        public float Field24 { get; set; }
        public uint Field28 { get; set; }
        public float Field1C { get; set; }
        public uint Field20 { get; set; }

        public float Field0C_P5R { get; set; }
        public float Field10_P5R { get; set; }
        public float Field14_P5R { get; set; }
        public float Field18_P5R { get; set; }
        public float Field1C_P5R { get; set; }
        public float Field20_P5R { get; set; }
        public int Field24_P5R { get; set; }
        public float Field28_P5R { get; set; }
        public float Field2C_P5R { get; set; }


        public EplModel() { }
        public EplModel( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            if ( Version > 0x1104050 )
            {
                Field04 = reader.ReadSingle();
                Field08 = reader.ReadSingle();
                if ( (Field00 & 0x10000000) != 0 && Version > ResourceVersion.Persona5 && Version < 0x2000000 )
                {
                    Field0C_P5R = reader.ReadSingle();
                    Field10_P5R = reader.ReadSingle();
                    Field14_P5R = reader.ReadSingle();
                    Field18_P5R = reader.ReadSingle();
                    Field1C_P5R = reader.ReadSingle();
                    Field20_P5R = reader.ReadSingle();
                    Field24_P5R = reader.ReadInt32();
                    Field28_P5R = reader.ReadSingle();
                    Field2C_P5R = reader.ReadSingle();
                }
                if ( Version > 0x2110031 )
                {
                    Field24 = reader.ReadSingle();
                    Field28 = reader.ReadUInt32();
                    Field0C = reader.ReadVector2();
                    Field14 = reader.ReadVector2();
                    Field1C = reader.ReadSingle();
                    Field20 = reader.ReadUInt32();
                }
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Data = reader.ReadResource<EplModel3DData>( Version ); break;
                case 2: Data = reader.ReadResource<EplModel2DData>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            HasEmbeddedFile = reader.ReadByte();
            if ( HasEmbeddedFile == 1 )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field04 );
                writer.WriteSingle( Field08 );
                if ( (Field00 & 0x10000000) != 0 && Version > ResourceVersion.Persona5 && Version < 0x2000000 )
                {
                    writer.WriteSingle( Field0C_P5R );
                    writer.WriteSingle( Field10_P5R );
                    writer.WriteSingle( Field14_P5R );
                    writer.WriteSingle( Field18_P5R );
                    writer.WriteSingle( Field1C_P5R );
                    writer.WriteSingle( Field20_P5R );
                    writer.WriteInt32( Field24_P5R );
                    writer.WriteSingle( Field28_P5R );
                    writer.WriteSingle( Field2C_P5R );
                }
                if ( Version > 0x2110031 )
                {
                    writer.WriteSingle( Field24 );
                    writer.WriteUInt32( Field28 );
                    writer.WriteVector2( Field0C );
                    writer.WriteVector2( Field14 );
                    writer.WriteSingle( Field1C );
                    writer.WriteUInt32( Field20 );
                }
            }
            if ( Data != null ) writer.WriteResource( Data );
            writer.WriteByte( HasEmbeddedFile );
            if ( HasEmbeddedFile == 1 )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplModel3DData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel3DData;


        public EplModel3DData() { }
        public EplModel3DData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
        }

        protected override void WriteCore( ResourceWriter writer )
        {
        }
    }

    public sealed class EplModel2DData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel2DData;

        public float Field0C { get; set; }

        public EplModel2DData() { }
        public EplModel2DData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field0C );
        }
    }
}
