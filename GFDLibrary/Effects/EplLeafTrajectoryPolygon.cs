using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplTrajectoryPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplTrajectoryPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public uint Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public uint Field12C { get; set; }
        public float Field130 { get; set; }
        public float Field134 { get; set; }
        public float Field138 { get; set; }
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
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadUInt32();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field12C = reader.ReadUInt32();
            Field130 = reader.ReadSingle();
            Field134 = reader.ReadSingle();
            if ( Version > 0x1104170 )
                Field138 = reader.ReadSingle();
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
            writer.WriteUInt32( Field00 );
            writer.WriteUInt32( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteUInt32( Field12C );
            writer.WriteSingle( Field130 );
            writer.WriteSingle( Field134 );
            if ( Version > 0x1104170 )
                writer.WriteSingle( Field138 );
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
