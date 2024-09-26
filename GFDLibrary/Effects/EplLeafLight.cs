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
    public sealed class EplLight : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLight;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public Resource Data { get; set; }
        public bool HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplLight() { }
        public EplLight( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            switch ( Type )
            {
                case 0: break;
                case 1: Data = reader.ReadResource<EplLightMeshData>( Version ); break;
                case 2: Data = reader.ReadResource<EplLightSceneData>( Version ); break;
                case 3: Data = reader.ReadResource<EplLightPointData>( Version ); break;
                case 4: Data = reader.ReadResource<EplLightSpotData>( Version ); break;
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
            if ( Data != null ) writer.WriteResource( Data );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplLightMeshData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightMeshData;


        public EplLightMeshData() { }
        public EplLightMeshData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     // empty
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     // empty
        }
    }

    public sealed class EplLightSceneData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightSceneData;

        public float Field0C { get; set; }
        public Vector2 Field10 { get; set; }
        public uint Field18 { get; set; }
        public uint Field1C { get; set; }
        public uint Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }

        public EplLightSceneData() { }
        public EplLightSceneData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadVector2();
            Field18 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            if ( Version > 0x1104910 )
            {
                Field24 = reader.ReadSingle();
                Field28 = reader.ReadSingle();
                Field2C = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field0C );
            writer.WriteVector2( Field10 );
            writer.WriteUInt32( Field18 );
            writer.WriteUInt32( Field1C );
            writer.WriteUInt32( Field20 );
            if ( Version > 0x1104910 )
            {
                writer.WriteSingle( Field24 );
                writer.WriteSingle( Field28 );
                writer.WriteSingle( Field2C );
            }
        }
    }

    public sealed class EplLightPointData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightPointData;
        public float FieldC { get; set; }
        public Vector2 Field10 { get; set; }
        public EplLeafCommonData2 Field18 { get; set; }
        public float Field7C { get; set; }
        public float Field80 { get; set; }
        public float Field84 { get; set; }

        public EplLightPointData() { }
        public EplLightPointData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldC = reader.ReadSingle();
            Field10 = reader.ReadVector2();
            Field18 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field7C = reader.ReadSingle();
            Field80 = reader.ReadSingle();
            Field84 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( FieldC );
            writer.WriteVector2( Field10 );
            writer.WriteResource( Field18 );
            writer.WriteSingle( Field7C );
            writer.WriteSingle( Field80 );
            writer.WriteSingle( Field84 );
        }
    }
    public sealed class EplLightSpotData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightSpotData;

        public float FieldC { get; set; }
        public Vector2 Field10 { get; set; }
        public EplLeafCommonData2 Field18 { get; set; }
        public float Field7C { get; set; }
        public float Field80 { get; set; }
        public float Field84 { get; set; }
        public float Field88 { get; set; }
        public float Field8C { get; set; }


        public EplLightSpotData() { }
        public EplLightSpotData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldC = reader.ReadSingle();
            Field10 = reader.ReadVector2();
            Field18 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field7C = reader.ReadSingle();
            Field80 = reader.ReadSingle();
            Field84 = reader.ReadSingle();
            Field88 = reader.ReadSingle();
            Field8C = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(FieldC );
            writer.WriteVector2(Field10 );
            writer.WriteResource( Field18 );
            writer.WriteSingle(Field7C );
            writer.WriteSingle(Field80 );
            writer.WriteSingle(Field84 );
            writer.WriteSingle(Field88 );
            writer.WriteSingle( Field8C );
        }
    }
}
