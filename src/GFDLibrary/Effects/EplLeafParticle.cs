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
        public float Field12C { get; set; }
        public float Field130 { get; set; }
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
                Field12C = reader.ReadSingle();
                Field130 = reader.ReadSingle();
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
                case 2: Params = reader.ReadResource<EplExplosionEffectParams>( Version ); break;
                case 3: Params = reader.ReadResource<EplSpiralEffectParams>( Version ); break;
                case 4: Params = reader.ReadResource<EplBallEffectParams>( Version ); break;
                case 5: Params = reader.ReadResource<EplCircleEffectParams>( Version ); break;
                case 6: Params = reader.ReadResource<EplStraightLineEffectParams>( Version ); break;
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
                writer.WriteSingle( Field12C );
                writer.WriteSingle( Field130 );
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

        public float Field170 { get; set; }
        public Vector2 Field174 { get; set; }
        public Vector2 Field17C { get; set; }
        public Vector2 Field184 { get; set; }
        public Vector2 Field18C { get; set; }
        public Vector2 Field194 { get; set; }

        public EplSmokeEffectParams() { }
        public EplSmokeEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadSingle();
            Field174 = reader.ReadVector2();
            Field17C = reader.ReadVector2();
            Field184 = reader.ReadVector2();
            Field18C = reader.ReadVector2();
            Field194 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Field170 );
            writer.WriteVector2( Field174 );
            writer.WriteVector2( Field17C );
            writer.WriteVector2( Field184 );
            writer.WriteVector2( Field18C );
            writer.WriteVector2( Field194 );
        }
    }

    public sealed class EplExplosionEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplExplosionEffectParams;

        public Vector2 Field170 { get; set; }
        public Vector2 Field188 { get; set; }
        public Vector2 Field178 { get; set; }
        public Vector2 Field180 { get; set; }
        public Vector2 Field190 { get; set; }
        public float Field198 { get; set; }

        public EplExplosionEffectParams() { }
        public EplExplosionEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadVector2();
            Field188 = reader.ReadVector2();
            Field178 = reader.ReadVector2();
            Field180 = reader.ReadVector2();
            Field190 = reader.ReadVector2();
            Field198 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( Field170 );
            writer.WriteVector2( Field188 );
            writer.WriteVector2( Field178 );
            writer.WriteVector2( Field180 );
            writer.WriteVector2( Field190 );
            writer.WriteSingle( Field198 );
        }
    }

    public sealed class EplSpiralEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSpiralEffectParams;

        public float Field170 { get; set; }
        public Vector2 Field174 { get; set; }
        public Vector2 Field17C { get; set; }
        public Vector2 Field184 { get; set; }
        public float Field18C { get; set; }
        public Vector2 Field190 { get; set; }
        public Vector2 Field198 { get; set; }

        public EplSpiralEffectParams() { }
        public EplSpiralEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadSingle();
            Field174 = reader.ReadVector2();
            Field17C = reader.ReadVector2();
            Field184 = reader.ReadVector2();
            Field18C = reader.ReadSingle();
            Field190 = reader.ReadVector2();
            Field198 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Field170 );
            writer.WriteVector2( Field174 );
            writer.WriteVector2( Field17C );
            writer.WriteVector2( Field184 );
            writer.WriteSingle( Field18C );
            writer.WriteVector2( Field190 );
            writer.WriteVector2( Field198 );
        }
    }

    public sealed class EplBallEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplBallEffectParams;

        public Vector2 Field170 { get; set; }
        public Vector2 Field178 { get; set; }
        public Vector2 Field180 { get; set; }
        public float Field188 { get; set; }
        public Vector2 Field18C { get; set; }

        public EplBallEffectParams() { }
        public EplBallEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadVector2();
            Field178 = reader.ReadVector2();
            Field180 = reader.ReadVector2();
            Field188 = reader.ReadSingle();
            Field18C = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( Field170 );
            writer.WriteVector2( Field178 );
            writer.WriteVector2( Field180 );
            writer.WriteSingle( Field188 );
            writer.WriteVector2( Field18C );
        }
    }

    public sealed class EplCircleEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCircleEffectParams;

        public Vector2 Field170 { get; set; }
        public Vector2 Field180 { get; set; }
        public float Field188 { get; set; }
        public Vector2 Field18C { get; set; }
        public float Field194 { get; set; }
        public Vector2 Field178 { get; set; }
        public Vector2 Field198 { get; set; }

        public EplCircleEffectParams() { }
        public EplCircleEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadVector2();
            Field180 = reader.ReadVector2();
            Field188 = reader.ReadSingle();
            Field18C = reader.ReadVector2();
            Field194 = reader.ReadSingle();
            Field178 = reader.ReadVector2();
            Field198 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteVector2( Field170 );
            writer.WriteVector2( Field180 );
            writer.WriteSingle( Field188 );
            writer.WriteVector2( Field18C );
            writer.WriteSingle( Field194 );
            writer.WriteVector2( Field178 );
            writer.WriteVector2( Field198 );
        }
    }

    public sealed class EplStraightLineEffectParams : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplStraightLineEffectParams;

        public float Field170 { get; set; }
        public Vector2 Field174 { get; set; }
        public Vector2 Field17C { get; set; }
        public Vector2 Field184 { get; set; }
        public Vector2 Field18C { get; set; }
        public Vector2 Field194 { get; set; }

        public EplStraightLineEffectParams() { }
        public EplStraightLineEffectParams( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Field170 = reader.ReadSingle();
            Field174 = reader.ReadVector2();
            Field17C = reader.ReadVector2();
            Field184 = reader.ReadVector2();
            Field18C = reader.ReadVector2();
            Field194 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Field170 );
            writer.WriteVector2( Field174 );
            writer.WriteVector2( Field17C );
            writer.WriteVector2( Field184 );
            writer.WriteVector2( Field18C );
            writer.WriteVector2( Field194 );
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
