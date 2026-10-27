using GFDLibrary.IO;
using System.Numerics;
using System;

namespace GFDLibrary.Effects
{
    public sealed class EplParticleEmitter : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplParticleEmitter;

        internal uint Type { get; set; }
        public uint Field15C { get; set; }
        public float Field160 { get; set; }
        public uint Field164 { get; set; }
        public float Field040 { get; set; }
        public Vector2 Field44 { get; set; }
        public float FieldB4 { get; set; }
        public uint FieldC4 { get; set; }
        public Vector2 FieldB8 { get; set; }
        public EplLeafCommonData2 Field50 { get; set; }
        public float FieldC0 { get; set; }
        public Resource FieldC8 { get; set; }
        public float Field12C { get; set; }
        public float Field130 { get; set; }
        public EplLeafCommonData Field134_Old { get; set; }
        public Vector2 Field134 { get; set; }
        public Vector2 Field13C { get; set; }
        public float Field144 { get; set; }
        public uint Field148 { get; set; }
        public uint Field14C { get; set; }
        public float Field150 { get; set; }
        public float Field154 { get; set; }
        public float Field158 { get; set; }
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
            Field15C = reader.ReadUInt32(); // randomSpawnDelay
            Field160 = reader.ReadSingle(); // particleLifetime
            Field164 = reader.ReadUInt32(); // angleSeed
            Field040 = reader.ReadSingle(); // despawnTimer
            Field44 = reader.ReadVector2(); // SpawnChoker
            if ( Version > 0x2107001 )
                Field4C = reader.ReadVector2();
            if ( Version > 0x2110193 )
                Field54 = reader.ReadVector2();
            FieldB4 = reader.ReadSingle(); // colorOverLifeOffset
            FieldC4 = reader.ReadUInt32(); // drawQueueIndex
            FieldB8 = reader.ReadVector2(); // OopacityOverLife
            if ( Version >= 0x1104041 )
                Field50 = reader.ReadResource<EplLeafCommonData2>( Version );
            if ( Version >= 0x1104701 )
                FieldC0 = reader.ReadSingle();
            if ( Version < 0x1104041 )
                FieldC8 = reader.ReadResource<EplLeafCommonData>( Version );
            else
                FieldC8 = reader.ReadResource<EplLeafCommonData2>( Version );
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
                Field134 = reader.ReadVector2(); // spawner angles
                Field13C = reader.ReadVector2(); // cycle rate from birth
            }
            Field144 = reader.ReadSingle();
            Field148 = reader.ReadUInt32();
            if ( Version > 0x1104171 )
            {
                Field14C = reader.ReadUInt32(); // particleMultiplier
                Field150 = reader.ReadSingle();
            }
            if ( Version > 0x1104051 )
            {
                Field154 = reader.ReadSingle(); // particleScale
                Field158 = reader.ReadSingle(); // particleSpeed
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
            writer.WriteUInt32( Field15C );
            writer.WriteSingle( Field160 );
            writer.WriteUInt32( Field164 );
            writer.WriteSingle( Field040 );
            writer.WriteVector2( Field44 );
            if ( Version > 0x2107001 )
                writer.WriteVector2( Field4C );
            if ( Version > 0x2110193 )
                writer.WriteVector2( Field54 );
            writer.WriteSingle( FieldB4 );
            writer.WriteUInt32( FieldC4 );
            writer.WriteVector2( FieldB8 );
            if ( Version >= 0x1104041 )
                writer.WriteResource( Field50 );
            if ( Version >= 0x1104701 )
                writer.WriteSingle( FieldC0 );
            writer.WriteResource( FieldC8 );
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
                writer.WriteVector2( Field134 );
                writer.WriteVector2( Field13C );
            }
            writer.WriteSingle( Field144 );
            writer.WriteUInt32( Field148 );
            if ( Version > 0x1104171 )
            {
                writer.WriteUInt32( Field14C );
                writer.WriteSingle( Field150 );
            }
            if ( Version > 0x1104051 )
            {
                writer.WriteSingle( Field154 );
                writer.WriteSingle( Field158 );
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
