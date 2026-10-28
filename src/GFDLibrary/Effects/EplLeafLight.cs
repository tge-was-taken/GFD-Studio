using GFDLibrary.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplLight : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLight;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field08" )]
        public float AdjustSpeed { get; set; }
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
            Flags = reader.ReadUInt32();
            AdjustScale = reader.ReadSingle();
            AdjustSpeed = reader.ReadSingle();
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
            writer.WriteUInt32( Flags );
            writer.WriteSingle( AdjustScale );
            writer.WriteSingle( AdjustSpeed );
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

        [FormerName( "Field0C" )]

        public float Life { get; set; }
        public Vector2 Field10 { get; set; }
        [FormerName( "Field18" )]
        public uint Ambient { get; set; }
        [FormerName( "Field1C" )]
        public uint Diffuse { get; set; }
        [FormerName( "Field20" )]
        public uint Specular { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }

        public EplLightSceneData() { }
        public EplLightSceneData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Life = reader.ReadSingle();
            Field10 = reader.ReadVector2();
            Ambient = reader.ReadUInt32();
            Diffuse = reader.ReadUInt32();
            Specular = reader.ReadUInt32();
            if ( Version > 0x1104910 )
            {
                Field24 = reader.ReadSingle();
                Field28 = reader.ReadSingle();
                Field2C = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Life );
            writer.WriteVector2( Field10 );
            writer.WriteUInt32( Ambient );
            writer.WriteUInt32( Diffuse );
            writer.WriteUInt32( Specular );
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
