using GFDLibrary.IO;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplHelper : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplHelper;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field08" )]
        public float AdjustSpeed { get; set; }
        [FormerName( "Field0C" )]
        public float Scale { get; set; }
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
            Flags = reader.ReadUInt32();
            AdjustScale = reader.ReadSingle();
            AdjustSpeed = reader.ReadSingle();
            Scale = reader.ReadSingle();
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
            writer.WriteUInt32( Flags );
            writer.WriteSingle( AdjustScale );
            writer.WriteSingle( AdjustSpeed );
            writer.WriteSingle( Scale );
            writer.WriteBoolean( Field84 );
            if ( Field84 )
                writer.WriteResource( EmbeddedFile1 );
            writer.WriteBoolean( Field88 );
            if ( Field88 )
                writer.WriteResource( EmbeddedFile2 );
        }
    }
}
