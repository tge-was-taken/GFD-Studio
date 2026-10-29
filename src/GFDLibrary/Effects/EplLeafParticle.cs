using GFDLibrary.IO;
using System.Numerics;
using System;

using GFDLibrary;

namespace GFDLibrary.Effects
{
    public sealed class EplParticleEmitter : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplParticleEmitter;

        internal uint Type { get; set; }
        [FormerName( "Field15C" )]
        public uint Flags { get; set; }
        [FormerName( "Field160" )]
        public float Life { get; set; }
        [FormerName( "Field164" )]
        public uint Seed { get; set; }
        [FormerName( "Field040" )]
        public float EmitterLife { get; set; }
        [FormerName( "Field44" )]
        public Vector2 EmitterCount { get; set; }
        [FormerName( "FieldB4" )]
        public float ColorAlpha { get; set; }
        [FormerName( "FieldC4" )]
        public uint ColorBlend { get; set; }
        [FormerName( "FieldB8" )]
        public Vector2 ColorFade { get; set; }
        [FormerName( "Field50" )]
        public EplLeafCommonData2 ColorCurve4 { get; set; }
        [FormerName( "FieldC0" )]
        public float ColorRandom { get; set; }
        [FormerName( "FieldC8" )]
        public Resource ScaleCurve4 { get; set; }
        [FormerName( "Field12C" )]
        public float ScaleRateX { get; set; }
        [FormerName( "Field130" )]
        public float ScaleRateY { get; set; }
        public EplLeafCommonData Field134_Old { get; set; }
        [FormerName( "Field134" )]
        public Vector2 RotateStart { get; set; }
        [FormerName( "Field13C" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field144" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field148" )]
        public uint RotateType { get; set; }
        [FormerName( "Field14C" )]
        public uint BlurLength { get; set; }
        [FormerName( "Field150" )]
        public float BlurInterval { get; set; }
        [FormerName( "Field154" )]
        public float AdjustScale { get; set; }
        [FormerName( "Field158" )]
        public float AdjustSpeed { get; set; }
        public Resource Params { get; set; }

        // METAPHOR
        public Vector2 Field4C { get; set; }
        public Vector2 Field54 { get; set; }
        public Vector2 Field140 { get; set; }

        public EplParticleEmitter() { }
        public EplParticleEmitter( uint Version ) : base( Version ) { }

        public static EplParticleEmitter Read( ResourceReader reader, uint version, uint type )
        {
            var obj = new EplParticleEmitter() { Version = version, Type = type };
            obj.ReadCore( reader );
            return obj;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            //     local u32 type = in_type;

            Logger.Debug( $"EplParticleEmitter: Reading type {Type}" );
            Flags = reader.ReadUInt32(); // randomSpawnDelay
            Life = reader.ReadSingle(); // particleLifetime
            Seed = reader.ReadUInt32(); // angleSeed
            EmitterLife = reader.ReadSingle(); // despawnTimer
            EmitterCount = reader.ReadVector2(); // SpawnChoker
            if ( Version > 0x2107001 )
                Field4C = reader.ReadVector2();
            if ( Version > 0x2110193 )
                Field54 = reader.ReadVector2();
            ColorAlpha = reader.ReadSingle(); // colorOverLifeOffset
            ColorBlend = reader.ReadUInt32(); // drawQueueIndex
            ColorFade = reader.ReadVector2(); // OopacityOverLife
            if ( Version >= 0x1104041 )
                ColorCurve4 = reader.ReadResource<EplLeafCommonData2>( Version );
            if ( Version >= 0x1104701 )
                ColorRandom = reader.ReadSingle();
            if ( Version < 0x1104041 )
                ScaleCurve4 = reader.ReadResource<EplLeafCommonData>( Version );
            else
                ScaleCurve4 = reader.ReadResource<EplLeafCommonData2>( Version );
            if (Version > 0x1104041)
            {
                ScaleRateX = reader.ReadSingle();
                ScaleRateY = reader.ReadSingle();
            }
            if (Version > 0x2107001)
                Field140 = reader.ReadVector2();
            if (Version < 0x1104041)
            {
                Field134_Old = reader.ReadResource<EplLeafCommonData>( Version );
            } else
            {
                RotateStart = reader.ReadVector2(); // spawner angles
                RotateSpeed = reader.ReadVector2(); // cycle rate from birth
            }
            RotateAccele = reader.ReadSingle();
            RotateType = reader.ReadUInt32();
            if ( Version > 0x1104171 )
            {
                BlurLength = reader.ReadUInt32(); // particleMultiplier
                BlurInterval = reader.ReadSingle();
            }
            if ( Version > 0x1104051 )
            {
                AdjustScale = reader.ReadSingle(); // particleScale
                AdjustSpeed = reader.ReadSingle(); // particleSpeed
            }

            switch ( Type )
            {
                case 0: break;
                case 1: Params = reader.ReadResource<EplSmokeEffectParams>( Version ); break;
                case 2: Params = reader.ReadResource<EplSparkEffectParams>( Version ); break;
                case 3: Params = reader.ReadResource<EplSpiralEffectParams>( Version ); break;
                case 4: Params = reader.ReadResource<EplSphereEffectParams>( Version ); break;
                case 5: Params = reader.ReadResource<EplRingEffectParams>( Version ); break;
                case 6: Params = reader.ReadResource<EplLineEffectParams>( Version ); break;
                default:
                    throw new NotImplementedException( $"Epl particle emitter type {Type} not implemented" );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            //     local u32 type = in_type;
            writer.WriteUInt32( Flags );
            writer.WriteSingle( Life );
            writer.WriteUInt32( Seed );
            writer.WriteSingle( EmitterLife );
            writer.WriteVector2( EmitterCount );
            if ( Version > 0x2107001 )
                writer.WriteVector2( Field4C );
            if ( Version > 0x2110193 )
                writer.WriteVector2( Field54 );
            writer.WriteSingle( ColorAlpha );
            writer.WriteUInt32( ColorBlend );
            writer.WriteVector2( ColorFade );
            if ( Version >= 0x1104041 )
                writer.WriteResource( ColorCurve4 );
            if ( Version >= 0x1104701 )
                writer.WriteSingle( ColorRandom );
            writer.WriteResource( ScaleCurve4 );
            if ( Version > 0x1104041 )
            {
                writer.WriteSingle( ScaleRateX );
                writer.WriteSingle( ScaleRateY );
            }
            if ( Version > 0x2107001 )
                writer.WriteVector2( Field140 );
            if (Version < 0x1104041)
            {
                writer.WriteResource( Field134_Old );
            } else
            {
                writer.WriteVector2( RotateStart );
                writer.WriteVector2( RotateSpeed );
            }
            writer.WriteSingle( RotateAccele );
            writer.WriteUInt32( RotateType );
            if ( Version > 0x1104171 )
            {
                writer.WriteUInt32( BlurLength );
                writer.WriteSingle( BlurInterval );
            }
            if ( Version > 0x1104051 )
            {
                writer.WriteSingle( AdjustScale );
                writer.WriteSingle( AdjustSpeed );
            }
            if ( Params != null )
                writer.WriteResource( Params );
        }
    }
    public sealed class EplParticle : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplParticle;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field10 { get; set; }
        public EplParticleEmitter Emitter { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplParticle() { }
        public EplParticle( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            //     // 0(r4)
            Type = reader.ReadUInt32();
            Logger.Debug( $"EplParticle: Reading type {Type}" );
            if ( Version < 0x1104170 )
                reader.SeekCurrent( 4 );
            if ( Version > 0x1104180 )
            {
                Field10 = reader.ReadUInt32();
            }
            //     // 0(r27)
            Emitter = EplParticleEmitter.Read( reader, Version, Type );
            //     // 0x10(r27)
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            //     // 0(r4)
            writer.WriteUInt32( Type );
            if ( Version < 0x1104170 )
                writer.SeekCurrent( 4 );
            if ( Version > 0x1104180 )
            {
                writer.WriteUInt32( Field10 );
            }
            //     // 0(r27)
            writer.WriteResource( Emitter );
            //     // 0x10(r27)
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplSmokeEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSmokeEffectParams;

        [FormerName( "Field170" )]
        public float Radius { get; set; }
        [FormerName( "Field174" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field17C" )]
        public Vector2 Gravity { get; set; }
        [FormerName( "Field184" )]
        public Vector2 AmpliBegin { get; set; }
        [FormerName( "Field18C" )]
        public Vector2 AmpliEnd { get; set; }
        [FormerName( "Field194" )]
        public Vector2 AmpliSpeed { get; set; }

        public EplSmokeEffectParams() { }
        public EplSmokeEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Radius = reader.ReadSingle();
            Speed = reader.ReadVector2();
            Gravity = reader.ReadVector2();
            AmpliBegin = reader.ReadVector2();
            AmpliEnd = reader.ReadVector2();
            AmpliSpeed = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Radius );
            writer.WriteVector2( Speed );
            writer.WriteVector2( Gravity );
            writer.WriteVector2( AmpliBegin );
            writer.WriteVector2( AmpliEnd );
            writer.WriteVector2( AmpliSpeed );
        }
    }

    [FormerName( "EplExplosionEffectParams" )]
    public sealed class EplSparkEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSparkEffectParams;

        [FormerName( "Field170" )]
        public Vector2 Radius { get; set; }
        [FormerName( "Field188" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field178" )]
        public Vector2 SpreadXZ { get; set; }
        [FormerName( "Field180" )]
        public Vector2 SpreadY { get; set; }
        [FormerName( "Field190" )]
        public Vector2 Gravity { get; set; }
        [FormerName( "Field198" )]
        public float Accele { get; set; }

        public EplSparkEffectParams() { }
        public EplSparkEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Radius = reader.ReadVector2();
            Speed = reader.ReadVector2();
            SpreadXZ = reader.ReadVector2();
            SpreadY = reader.ReadVector2();
            Gravity = reader.ReadVector2();
            Accele = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( Radius );
            writer.WriteVector2( Speed );
            writer.WriteVector2( SpreadXZ );
            writer.WriteVector2( SpreadY );
            writer.WriteVector2( Gravity );
            writer.WriteSingle( Accele );
        }
    }

    public sealed class EplSpiralEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSpiralEffectParams;

        [FormerName( "Field170" )]
        public float Height { get; set; }
        [FormerName( "Field174" )]
        public Vector2 RadiusBegin { get; set; }
        [FormerName( "Field17C" )]
        public Vector2 RadiusEnd { get; set; }
        [FormerName( "Field184" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field18C" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field190" )]
        public Vector2 RiseSpeed { get; set; }
        [FormerName( "Field198" )]
        public Vector2 Gravity { get; set; }

        public EplSpiralEffectParams() { }
        public EplSpiralEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Height = reader.ReadSingle();
            RadiusBegin = reader.ReadVector2();
            RadiusEnd = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            RiseSpeed = reader.ReadVector2();
            Gravity = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Height );
            writer.WriteVector2( RadiusBegin );
            writer.WriteVector2( RadiusEnd );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteVector2( RiseSpeed );
            writer.WriteVector2( Gravity );
        }
    }

    [FormerName( "EplBallEffectParams" )]
    public sealed class EplSphereEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSphereEffectParams;

        [FormerName( "Field170" )]
        public Vector2 RadiusBegin { get; set; }
        [FormerName( "Field178" )]
        public Vector2 RadiusEnd { get; set; }
        [FormerName( "Field180" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field188" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field18C" )]
        public Vector2 Gravity { get; set; }

        public EplSphereEffectParams() { }
        public EplSphereEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            RadiusBegin = reader.ReadVector2();
            RadiusEnd = reader.ReadVector2();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            Gravity = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( RadiusBegin );
            writer.WriteVector2( RadiusEnd );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteVector2( Gravity );
        }
    }

    [FormerName( "EplCircleEffectParams" )]
    public sealed class EplRingEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplRingEffectParams;

        [FormerName( "Field170" )]
        public Vector2 Radius { get; set; }
        [FormerName( "Field180" )]
        public Vector2 SpreadSpeed { get; set; }
        [FormerName( "Field188" )]
        public float SpreadAccele { get; set; }
        [FormerName( "Field18C" )]
        public Vector2 RotateSpeed { get; set; }
        [FormerName( "Field194" )]
        public float RotateAccele { get; set; }
        [FormerName( "Field178" )]
        public Vector2 Height { get; set; }
        [FormerName( "Field198" )]
        public Vector2 Gravity { get; set; }

        public EplRingEffectParams() { }
        public EplRingEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Radius = reader.ReadVector2();
            SpreadSpeed = reader.ReadVector2();
            SpreadAccele = reader.ReadSingle();
            RotateSpeed = reader.ReadVector2();
            RotateAccele = reader.ReadSingle();
            Height = reader.ReadVector2();
            Gravity = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( Radius );
            writer.WriteVector2( SpreadSpeed );
            writer.WriteSingle( SpreadAccele );
            writer.WriteVector2( RotateSpeed );
            writer.WriteSingle( RotateAccele );
            writer.WriteVector2( Height );
            writer.WriteVector2( Gravity );
        }
    }

    [FormerName( "EplStraightLineEffectParams" )]
    public sealed class EplLineEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLineEffectParams;

        [FormerName( "Field170" )]
        public float Length { get; set; }
        [FormerName( "Field174" )]
        public Vector2 Speed { get; set; }
        [FormerName( "Field17C" )]
        public Vector2 Gravity { get; set; }
        [FormerName( "Field184" )]
        public Vector2 AmpliBegin { get; set; }
        [FormerName( "Field18C" )]
        public Vector2 AmpliEnd { get; set; }
        [FormerName( "Field194" )]
        public Vector2 AmpliSpeed { get; set; }

        public EplLineEffectParams() { }
        public EplLineEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Length = reader.ReadSingle();
            Speed = reader.ReadVector2();
            Gravity = reader.ReadVector2();
            AmpliBegin = reader.ReadVector2();
            AmpliEnd = reader.ReadVector2();
            AmpliSpeed = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Length );
            writer.WriteVector2( Speed );
            writer.WriteVector2( Gravity );
            writer.WriteVector2( AmpliBegin );
            writer.WriteVector2( AmpliEnd );
            writer.WriteVector2( AmpliSpeed );
        }
    }

    public sealed class EplDirectionalParticles : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplDirectionalParticles;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public EplParticleEmitter ParticleEmittter { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplDirectionalParticles() { }
        public EplDirectionalParticles( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            if ( Version > 0x1104080 )
            {
                Field00 = reader.ReadUInt32();
            }
            ParticleEmittter = EplParticleEmitter.Read( reader, Version, Type );
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            if ( Version > 0x1104080 )
            {
                writer.WriteUInt32( Field00 );
            }
            writer.WriteResource( ParticleEmittter );
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplObjectParticles : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplObjectParticles;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public uint Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public float Field10 { get; set; }
        public EplParticleEmitter EffectGenerator { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplObjectParticles() { }
        public EplObjectParticles( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadUInt32();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            EffectGenerator = EplParticleEmitter.Read( reader, Version, Type );
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
            //     //Assert(false, "Not implemented");
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteUInt32( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteSingle( Field10 );
            writer.WriteResource( EffectGenerator );
            writer.WriteResource( EmbeddedFile );
            //     //Assert(false, "Not implemented");
        }
    }
}
