using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplCamera : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCamera;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public uint Field0C { get; set; }
        public Resource Params { get; set; }
        public bool HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplCamera() { }
        public EplCamera( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadUInt32();
            switch ( Type )
            {

                case 0: break;
                case 1: Params = reader.ReadResource<EplCameraMeshParams>( Version ); break;
                case 2: Params = reader.ReadResource<EplCameraQuakeParams>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            HasEmbeddedFile = reader.ReadBoolean();
            if ( HasEmbeddedFile )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteUInt32( Field0C );
            if ( Params != null ) writer.WriteResource( Params );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplCameraMeshParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCameraMeshParams;


        public EplCameraMeshParams() { }
        public EplCameraMeshParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     // empty
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     // empty
        }
    }

    public sealed class EplCameraQuakeParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCameraQuakeParams;

        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public float Field20 { get; set; }

        public EplCameraQuakeParams() { }
        public EplCameraQuakeParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteSingle( Field1C );
            writer.WriteSingle( Field20 );
        }
    }
}
