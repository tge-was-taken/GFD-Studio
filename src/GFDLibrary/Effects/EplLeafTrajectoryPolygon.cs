using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplTrajectoryPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplTrajectoryPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public uint Number { get; set; }
        [FormerName( "Field08" )]
        public float ThickCore { get; set; }
        [FormerName( "Field0C" )]
        public float ThickOutside { get; set; }
        [FormerName( "Field12C" )]
        public uint Split { get; set; }
        [FormerName( "Field130" )]
        public float RepeatTexV { get; set; }
        [FormerName( "Field134" )]
        public float MoveTexV { get; set; }
        [FormerName( "Field138" )]
        public float Interval { get; set; }
        public EplLeafCommonData2 Field10 { get; set; }
        public EplLeafCommonData2 Field74 { get; set; }
        public EplLeafCommonData FieldD8 { get; set; }
        public EplParticleEmitter Field140 { get; set; }
        public bool HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplTrajectoryPolygon() { }
        public EplTrajectoryPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            if ( Version <= 0x1104170 )
                reader.SeekCurrent( 4 );
            Flags = reader.ReadUInt32();
            Number = reader.ReadUInt32();
            ThickCore = reader.ReadSingle();
            ThickOutside = reader.ReadSingle();
            Split = reader.ReadUInt32();
            RepeatTexV = reader.ReadSingle();
            MoveTexV = reader.ReadSingle();
            if ( Version > 0x1104170 )
                Interval = reader.ReadSingle();
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldD8 = reader.ReadResource<EplLeafCommonData>( Version );
            Field140 = EplParticleEmitter.Read( reader, Version, Type );
            HasEmbeddedFile = reader.ReadBoolean();
            if ( HasEmbeddedFile )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            if ( Version <= 0x1104170 )
                writer.SeekCurrent( 4 );
            writer.WriteUInt32( Flags );
            writer.WriteUInt32( Number );
            writer.WriteSingle( ThickCore );
            writer.WriteSingle( ThickOutside );
            writer.WriteUInt32( Split );
            writer.WriteSingle( RepeatTexV );
            writer.WriteSingle( MoveTexV );
            if ( Version > 0x1104170 )
                writer.WriteSingle( Interval );
            writer.WriteResource( Field10 );
            writer.WriteResource( Field74 );
            writer.WriteResource( FieldD8 );
            writer.WriteResource( Field140 );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
                writer.WriteResource( EmbeddedFile );
        }
    }
}
