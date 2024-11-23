using GFDLibrary.IO;

namespace GFDLibrary.Effects
{
    public sealed class EplHelper : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplHelper;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public bool Field84 { get; set; }
        public EplEmbeddedFile EmbeddedFile1 { get; set; }
        public bool Field88 { get; set; }
        public EplEmbeddedFile EmbeddedFile2 { get; set; }

        public EplHelper() { }
        public EplHelper( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field84 = reader.ReadBoolean();
            if ( Field84 )
                EmbeddedFile1 = reader.ReadResource<EplEmbeddedFile>( Version );
            Field88 = reader.ReadBoolean();
            if ( Field88 )
                EmbeddedFile2 = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteBoolean( Field84 );
            if ( Field84 )
                writer.WriteResource( EmbeddedFile1 );
            writer.WriteBoolean( Field88 );
            if ( Field88 )
                writer.WriteResource( EmbeddedFile2 );
        }
    }
}
