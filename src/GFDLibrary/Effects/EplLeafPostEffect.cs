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
        [FormerName( "Field00" )]
        public uint Flags { get; set; }
        [FormerName( "Field04" )]
        public float Life { get; set; }
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
            Flags = reader.ReadUInt32();
            Life = reader.ReadSingle();
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
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteResource( Data );
            writer.WriteBoolean( HasEmbeddedFile );
            if ( HasEmbeddedFile )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplPostEffectRadiationBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectRadiationBlurData;

        [FormerName( "Field10" )]

        public EplLeafCommonData2 Color { get; set; }
        [FormerName( "Field74" )]
        public uint Blend { get; set; }
        [FormerName( "Field78" )]
        public EplLeafCommonData2 Power { get; set; }
        [FormerName( "FieldDC" )]
        public float Falloff { get; set; }
        [FormerName( "FieldE0" )]
        public float CenterX { get; set; }
        [FormerName( "FieldE4" )]
        public float CenterY { get; set; }
        [FormerName( "FieldE8" )]
        public byte SsaoMask { get; set; }

        public EplPostEffectRadiationBlurData() { }
        public EplPostEffectRadiationBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Blend = reader.ReadUInt32();
            Power = reader.ReadResource<EplLeafCommonData2>( Version );
            Falloff = reader.ReadSingle();
            CenterX = reader.ReadSingle();
            CenterY = reader.ReadSingle();
            if ( Version > 0x1104930 )
            {
                SsaoMask = reader.ReadByte();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Color );
            writer.WriteUInt32( Blend );
            writer.WriteResource( Power );
            writer.WriteSingle( Falloff );
            writer.WriteSingle( CenterX );
            writer.WriteSingle( CenterY );
            if ( Version > 0x1104930 )
            {
                writer.WriteByte( SsaoMask );
            }
        }
    }

    public sealed class EplPostEffectStraightBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectStraightBlurData;

        [FormerName( "Field10" )]

        public EplLeafCommonData2 Color { get; set; }
        [FormerName( "Field74" )]
        public uint Blend { get; set; }
        [FormerName( "Field78" )]
        public EplLeafCommonData2 Power { get; set; }
        [FormerName( "FieldDC" )]
        public float Direction { get; set; }

        public EplPostEffectStraightBlurData() { }
        public EplPostEffectStraightBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Blend = reader.ReadUInt32();
            Power = reader.ReadResource<EplLeafCommonData2>( Version );
            Direction = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Color );
            writer.WriteUInt32( Blend );
            writer.WriteResource( Power );
            writer.WriteSingle( Direction );
        }
    }

    public sealed class EplPostEffectNoiseBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectNoiseBlurData;

        [FormerName( "Field10" )]

        public EplLeafCommonData2 Color { get; set; }
        [FormerName( "Field74" )]
        public uint Blend { get; set; }
        [FormerName( "Field78" )]
        public EplLeafCommonData2 Power { get; set; }
        [FormerName( "FieldDC" )]
        public EplLeafCommonData2 Scale { get; set; }
        [FormerName( "Field140" )]
        public byte SsaoMask { get; set; }

        public EplPostEffectNoiseBlurData() { }
        public EplPostEffectNoiseBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Blend = reader.ReadUInt32();
            Power = reader.ReadResource<EplLeafCommonData2>( Version );
            Scale = reader.ReadResource<EplLeafCommonData2>( Version );
            if ( Version > 0x1104920 )
            {
                SsaoMask = reader.ReadByte();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Color );
            writer.WriteUInt32( Blend );
            writer.WriteResource( Power );
            writer.WriteResource( Scale );
            if ( Version > 0x1104920 )
            {
                writer.WriteByte( SsaoMask );
            }
        }
    }

    public sealed class EplPostEffectDistortionBlurData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectDistortionBlurData;

        [FormerName( "Field10" )]

        public EplLeafCommonData2 Color { get; set; }
        [FormerName( "Field74" )]
        public uint Blend { get; set; }
        [FormerName( "Field78" )]
        public EplLeafCommonData2 Power0 { get; set; }
        [FormerName( "FieldDC" )]
        public EplLeafCommonData2 Power1 { get; set; }
        [FormerName( "Field140" )]
        public float Uv0X { get; set; }
        [FormerName( "Field144" )]
        public float Uv0Y { get; set; }
        [FormerName( "Field148" )]
        public float Uv1X { get; set; }
        [FormerName( "Field14C" )]
        public float Uv1Y { get; set; }

        public EplPostEffectDistortionBlurData() { }
        public EplPostEffectDistortionBlurData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Blend = reader.ReadUInt32();
            Power0 = reader.ReadResource<EplLeafCommonData2>( Version );
            Power1 = reader.ReadResource<EplLeafCommonData2>( Version );
            Uv0X = reader.ReadSingle();
            Uv0Y = reader.ReadSingle();
            Uv1X = reader.ReadSingle();
            Uv1Y = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Color );
            writer.WriteUInt32( Blend );
            writer.WriteResource( Power0 );
            writer.WriteResource( Power1 );
            writer.WriteSingle( Uv0X );
            writer.WriteSingle( Uv0Y );
            writer.WriteSingle( Uv1X );
            writer.WriteSingle( Uv1Y );
        }
    }

    public sealed class EplPostEffectFillData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectFillData;

        [FormerName( "Field10" )]

        public EplLeafCommonData2 Color0 { get; set; }
        [FormerName( "Field74" )]
        public EplLeafCommonData2 Color1 { get; set; }
        [FormerName( "FieldD8" )]
        public EplLeafCommonData2 Color2 { get; set; }
        [FormerName( "Field13C" )]
        public EplLeafCommonData2 Color3 { get; set; }
        [FormerName( "FieldlA0" )]
        public uint Blend { get; set; }

        public EplPostEffectFillData() { }
        public EplPostEffectFillData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Color0 = reader.ReadResource<EplLeafCommonData2>( Version );
            Color1 = reader.ReadResource<EplLeafCommonData2>( Version );
            Color2 = reader.ReadResource<EplLeafCommonData2>( Version );
            Color3 = reader.ReadResource<EplLeafCommonData2>( Version );
            Blend = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Color0 );
            writer.WriteResource( Color1 );
            writer.WriteResource( Color2 );
            writer.WriteResource( Color3 );
            writer.WriteUInt32( Blend );
        }
    }

    public sealed class EplPostEffectLensFlareData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectLensFlareData;

        [FormerName( "Field10" )]

        public uint Flags { get; set; }
        [FormerName( "Field14" )]
        public uint Template { get; set; }
        [FormerName( "Field18" )]
        public uint Filter { get; set; }
        [FormerName( "Field1C" )]
        public EplLeafCommonData2 Color { get; set; }
        [FormerName( "Field80" )]
        public float Brightness { get; set; }

        public EplPostEffectLensFlareData() { }
        public EplPostEffectLensFlareData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Flags = reader.ReadUInt32();
            Template = reader.ReadUInt32();
            Filter = reader.ReadUInt32();
            Color = reader.ReadResource<EplLeafCommonData2>( Version );
            Brightness = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Flags );
            writer.WriteUInt32( Template );
            writer.WriteUInt32( Filter );
            writer.WriteResource( Color );
            writer.WriteSingle( Brightness );
        }
    }

    public sealed class EplPostEffectColorCorrectionData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectColorCorrectionData;

        [FormerName( "Field10" )]

        public float Cyan { get; set; }
        [FormerName( "Field14" )]
        public float Magenta { get; set; }
        [FormerName( "Field18" )]
        public float Yellow { get; set; }
        [FormerName( "Field1C" )]
        public float Dodge { get; set; }
        [FormerName( "Field20" )]
        public float Burn { get; set; }
        [FormerName( "Field24" )]
        public float Alpha { get; set; }
        [FormerName( "Field28" )]
        public Vector2 Fade { get; set; }

        public EplPostEffectColorCorrectionData() { }
        public EplPostEffectColorCorrectionData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Cyan = reader.ReadSingle();
            Magenta = reader.ReadSingle();
            Yellow = reader.ReadSingle();
            Dodge = reader.ReadSingle();
            Burn = reader.ReadSingle();
            Alpha = reader.ReadSingle();
            Fade = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Cyan );
            writer.WriteSingle( Magenta );
            writer.WriteSingle( Yellow );
            writer.WriteSingle( Dodge );
            writer.WriteSingle( Burn );
            writer.WriteSingle( Alpha );
            writer.WriteVector2( Fade );
        }
    }

    public sealed class EplPostEffectMonotoneData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplPostEffectMonotoneData;

        [FormerName( "Field10" )]

        public float Alpha { get; set; }
        [FormerName( "Field14" )]
        public Vector2 Fade { get; set; }

        public EplPostEffectMonotoneData() { }
        public EplPostEffectMonotoneData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Alpha = reader.ReadSingle();
            Fade = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Alpha );
            writer.WriteVector2( Fade );
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
