using GFDLibrary.IO;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Effects
{
    public sealed class EplPostEffect : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffect;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public Resource Data { get; set; }
        public bool HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplPostEffect() { }
        public EplPostEffect( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            switch ( Type )
            {

                case 0: break;
                case 1: Data = reader.ReadResource<EplPostEffectRadiationBlurData>( Version ); break;
                case 2: Data = reader.ReadResource<EplPostEffectStraightBlurData>( Version ); break;
                case 3: Data = reader.ReadResource<EplPostEffectNoiseBlurData>( Version ); break;
                case 4: Data = reader.ReadResource<EplPostEffectDistortionBlurData>( Version ); break;
                case 5: Data = reader.ReadResource<EplPostEffectFillData>( Version ); break;
                case 6: Data = reader.ReadResource<EplPostEffectLensFlareData>( Version ); break;
                case 7: Data = reader.ReadResource<EplPostEffectColorCorrectionData>( Version ); break;
                case 8: Data = reader.ReadResource<EplPostEffectMonotoneData>( Version ); break;
                case 9:
                    if ( Version >= 0x2000000 ) Data = reader.ReadResource<EplPostEffectChromaticAberration>( Version );
                    else Data = reader.ReadResource<EplPostEffectLensFlareMakeData>( Version );
                    break;
                case 10:
                    if ( Version >= 0x2000000 ) Data = reader.ReadResource<EplPostEffectColorCorrectionExcludeToon>( Version );
                    else Data = reader.ReadResource<EplPostEffectMotionBlurData>( Version );
                    break;
                case 11: Data = reader.ReadResource<EplPostEffectAfterimageBlurData>( Version ); break; // Only in P5R
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
            writer.WriteResource( Data );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplPostEffectRadiationBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectRadiationBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public float FieldDC { get; set; }
        public float FieldE0 { get; set; }
        public float FieldE4 { get; set; }
        public byte FieldE8 { get; set; }

        public EplPostEffectRadiationBlurData() { }
        public EplPostEffectRadiationBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldDC = reader.ReadSingle();
            FieldE0 = reader.ReadSingle();
            FieldE4 = reader.ReadSingle();
            if ( Version > 0x1104930 )
            {
                FieldE8 = reader.ReadByte();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteUInt32( Field74 );
            writer.WriteResource( Field78 );
            writer.WriteSingle( FieldDC );
            writer.WriteSingle( FieldE0 );
            writer.WriteSingle( FieldE4 );
            if ( Version > 0x1104930 )
            {
                writer.WriteByte( FieldE8 );
            }
        }
    }

    public sealed class EplPostEffectStraightBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectStraightBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public float FieldDC { get; set; }

        public EplPostEffectStraightBlurData() { }
        public EplPostEffectStraightBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldDC = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteUInt32( Field74 );
            writer.WriteResource( Field78 );
            writer.WriteSingle( FieldDC );
        }
    }

    public sealed class EplPostEffectNoiseBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectNoiseBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public EplLeafCommonData2 FieldDC { get; set; }
        public byte Field140 { get; set; }

        public EplPostEffectNoiseBlurData() { }
        public EplPostEffectNoiseBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData2>( Version );
            if ( Version > 0x1104920 )
            {
                Field140 = reader.ReadByte();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteUInt32( Field74 );
            writer.WriteResource( Field78 );
            writer.WriteResource( FieldDC );
            if ( Version > 0x1104920 )
            {
                writer.WriteByte( Field140 );
            }
        }
    }

    public sealed class EplPostEffectDistortionBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectDistortionBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public EplLeafCommonData2 FieldDC { get; set; }
        public float Field140 { get; set; }
        public float Field144 { get; set; }
        public float Field148 { get; set; }
        public float Field14C { get; set; }

        public EplPostEffectDistortionBlurData() { }
        public EplPostEffectDistortionBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData2>( Version );
            Field140 = reader.ReadSingle();
            Field144 = reader.ReadSingle();
            Field148 = reader.ReadSingle();
            Field14C = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteUInt32( Field74 );
            writer.WriteResource( Field78 );
            writer.WriteResource( FieldDC );
            writer.WriteSingle( Field140 );
            writer.WriteSingle( Field144 );
            writer.WriteSingle( Field148 );
            writer.WriteSingle( Field14C );
        }
    }

    public sealed class EplPostEffectFillData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectFillData;

        public EplLeafCommonData2 Field10 { get; set; }
        public EplLeafCommonData2 Field74 { get; set; }
        public EplLeafCommonData2 FieldD8 { get; set; }
        public EplLeafCommonData2 Field13C { get; set; }
        public uint FieldlA0 { get; set; }

        public EplPostEffectFillData() { }
        public EplPostEffectFillData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldD8 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field13C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldlA0 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteResource( Field74 );
            writer.WriteResource( FieldD8 );
            writer.WriteResource( Field13C );
            writer.WriteUInt32( FieldlA0 );
        }
    }

    public sealed class EplPostEffectLensFlareData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectLensFlareData;

        public uint Field10 { get; set; }
        public uint Field14 { get; set; }
        public uint Field18 { get; set; }
        public EplLeafCommonData2 Field1C { get; set; }
        public float Field80 { get; set; }

        public EplPostEffectLensFlareData() { }
        public EplPostEffectLensFlareData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadUInt32();
            Field14 = reader.ReadUInt32();
            Field18 = reader.ReadUInt32();
            Field1C = reader.ReadResource<EplLeafCommonData2>( Version );
            Field80 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field14 );
            writer.WriteUInt32( Field18 );
            writer.WriteResource( Field1C );
            writer.WriteSingle( Field80 );
        }
    }

    public sealed class EplPostEffectColorCorrectionData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectColorCorrectionData;

        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public Vector2 Field28 { get; set; }

        public EplPostEffectColorCorrectionData() { }
        public EplPostEffectColorCorrectionData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteSingle( Field1C );
            writer.WriteSingle( Field20 );
            writer.WriteSingle( Field24 );
            writer.WriteVector2( Field28 );
        }
    }

    public sealed class EplPostEffectMonotoneData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectMonotoneData;

        public float Field10 { get; set; }
        public Vector2 Field14 { get; set; }

        public EplPostEffectMonotoneData() { }
        public EplPostEffectMonotoneData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field10 );
            writer.WriteVector2( Field14 );
        }
    }

    public sealed class EplPostEffectChromaticAberration : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectChromaticAberrationData;

        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public Vector2 Field1C { get; set; }
        public Vector2 Field24 { get; set; }

        public EplPostEffectChromaticAberration() { }
        public EplPostEffectChromaticAberration( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadVector2();
            Field24 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle(Field10);
            writer.WriteSingle(Field14);
            writer.WriteSingle(Field18);
            writer.WriteVector2(Field1C);
            writer.WriteVector2(Field24);
        }
    }

    public sealed class EplPostEffectColorCorrectionExcludeToon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectColorCorrectionExcludeToonData;

        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public Vector2 Field34 { get; set; }

        public EplPostEffectColorCorrectionExcludeToon() { }
        public EplPostEffectColorCorrectionExcludeToon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteSingle( Field1C );
            writer.WriteSingle( Field20 );
            writer.WriteSingle( Field24 );
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteSingle( Field30 );
            writer.WriteVector2( Field34 );
        }
    }

    public sealed class EplPostEffectLensFlareMakeData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectLensFlareMakeData;

        public uint Field10 { get; set; }
        public uint Field20 { get; set; }
        public EplLeafCommonData2 FieldA0 { get; set; }
        public float Field60 { get; set; }
        public float Field64 { get; set; }
        public float Field68 { get; set; }
        public float Field6C { get; set; }
        public float Field70 { get; set; }
        public float Field74 { get; set; }
        public float Field78 { get; set; }
        public float Field7C { get; set; }
        public float Field80 { get; set; }
        public float Field84 { get; set; }
        public float Field88 { get; set; }
        public float Field8C { get; set; }
        public float Field90 { get; set; }
        public float Field94 { get; set; }
        public float Field98 { get; set; }
        public float Field9C { get; set; }
        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            FieldA0 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field60 = reader.ReadSingle();
            Field64 = reader.ReadSingle();
            Field68 = reader.ReadSingle();
            Field6C = reader.ReadSingle();
            Field70 = reader.ReadSingle();
            Field74 = reader.ReadSingle();
            Field78 = reader.ReadSingle();
            Field7C = reader.ReadSingle();
            Field80 = reader.ReadSingle();
            Field84 = reader.ReadSingle();
            Field88 = reader.ReadSingle();
            Field8C = reader.ReadSingle();
            Field90 = reader.ReadSingle();
            Field94 = reader.ReadSingle();
            Field98 = reader.ReadSingle();
            Field9C = reader.ReadSingle();
        }
        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field20 );
            writer.WriteResource( FieldA0 );
            writer.WriteSingle( Field60 );
            writer.WriteSingle( Field64 );
            writer.WriteSingle( Field68 );
            writer.WriteSingle( Field6C );
            writer.WriteSingle( Field70 );
            writer.WriteSingle( Field74 );
            writer.WriteSingle( Field78 );
            writer.WriteSingle( Field7C );
            writer.WriteSingle( Field80 );
            writer.WriteSingle( Field84 );
            writer.WriteSingle( Field88 );
            writer.WriteSingle( Field8C );
            writer.WriteSingle( Field90 );
            writer.WriteSingle( Field94 );
            writer.WriteSingle( Field98 );
            writer.WriteSingle( Field9C );
        }
    }
    public sealed class EplPostEffectMotionBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectMotionBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public float FieldDC { get; set; }
        public float FieldE0 { get; set; }
        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>(Version);
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>(Version);
            FieldDC = reader.ReadSingle();
            FieldE0 = reader.ReadSingle();
        }
        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10);
            writer.WriteUInt32(Field74);
            writer.WriteResource( Field78);
            writer.WriteSingle(FieldDC);
            writer.WriteSingle( FieldE0 );
        }
    }

    public sealed class EplPostEffectAfterimageBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectAfterimageBlurData;

        public EplLeafCommonData2 Field10 { get; set; }
        public uint Field74 { get; set; }
        public EplLeafCommonData2 Field78 { get; set; }
        public float FieldDC { get; set; }
        protected override void ReadCore( ResourceReader reader )
        {
            Field10 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field74 = reader.ReadUInt32();
            Field78 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldDC = reader.ReadSingle();
        }
        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field10 );
            writer.WriteUInt32( Field74 );
            writer.WriteResource( Field78 );
            writer.WriteSingle( FieldDC );
        }
    }
}
