
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Animations;
using GFDLibrary.Common;
using GFDLibrary.IO;
using GFDLibrary.Models;

namespace GFDLibrary.Effects
{
    public sealed class Epl : Resource
    {
        public override ResourceType ResourceType => ResourceType.Epl;

        public EplFlags Flags { get; set; }
        public Node RootNode { get; set; }
        public EplAnimation Animation { get; set; }
        public ushort Field40 { get; set; }

        public Epl() { }
        public Epl( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "Epl: Reading" );
            Flags = (EplFlags)reader.ReadInt32();
            RootNode = Node.ReadRecursive( reader, Version );
            Animation = reader.ReadResource<EplAnimation>( Version );
            if ( Version > 0x1105060 )
            {
                Field40 = reader.ReadUInt16();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteInt32( (int)Flags );
            Node.WriteRecursive( writer, RootNode );
            writer.WriteResource( Animation );
            if ( Version > 0x1105060 )
            {
                writer.WriteUInt16( Field40 );
            }
            //     //Assert(false, "Not implemented");
        }
    }

    public sealed class EplAnimation : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplAnimation;

        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public Animation Animation { get; set; }
        public List<EplAnimationController> Controllers { get; set; } = new List<EplAnimationController>();

        public EplAnimation() { }
        public EplAnimation( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "EplAnimation: Reading" );
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Animation = reader.ReadResource<Animation>( Version );
            Controllers = reader.ReadResourceList<EplAnimationController>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteResource( Animation );
            //     local u32 i;
            //     local u32 controllerCount = Animation.ControllerCount;
            //     //for ( i = 0; i < controllerCount; ++i )
            //     //    keyCountsBuffer[i] = Animation.Controllers.Controllers[i].Layers[0].KeyCount;
            writer.WriteResourceList( Controllers );
        }
    }

    public class EplAnimationKey { }

    public sealed class EplAnimationController : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplAnimationController;

        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public int ControllerIndex { get; set; }
        public EplAnimationKey Keys { get; set; }
        public int Field0C { get; set; }

        public EplAnimationController() { }
        public EplAnimationController( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "EplAnimationController: Reading" );
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            ControllerIndex = reader.ReadInt32();
            //     if ( ControllerIndex != -1 )
            //     {
            //         //local u32 keyCount = Animation.Controller[ControllerIndex].KeyCount;
            //         local u32 keyCount = keyCounts[ControllerIndex];
            //         if ( keyCount != 0 )
            //         {
            //				Keys = reader.ReadResource<EplAnimationKey>( Version );
            //         }
            //     }
            Field0C = reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteInt32( ControllerIndex );
            //     if ( ControllerIndex != -1 )
            //     {
            //         //local u32 keyCount = Animation.Controller[ControllerIndex].KeyCount;
            //         local u32 keyCount = keyCounts[ControllerIndex];
            //         if ( keyCount != 0 )
            //         {
            //				writer.WriteResource( Keys );
            //         }
            //     }
            writer.WriteInt32( Field0C );
        }
    }

    public sealed class EplLeaf : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeaf;

        public EplLeafFlags Flags { get; set; }
        public string Name { get; set; }
        public Resource Data { get; set; }
        public EplLeaf() { }
        public EplLeaf( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Flags = (EplLeafFlags)reader.ReadInt32();
            Name = reader.ReadStringWithHash( Version );
            var type = reader.ReadInt32();
            reader.SeekCurrent( -4 );
            //     local u32 type = ReadInt( FTell() + 0 );
            //     // 0x40
            Logger.Debug( $"EplLeaf: Reading {Name} type {type}" );
            switch ( type )
            {
                case 0: Data = reader.ReadResource<EplDummy>( Version ); break;
                case 1: Data = reader.ReadResource<EplParticle>( Version ); break;
                case 2: Data = reader.ReadResource<EplFlashPolygon>( Version ); break;
                case 3: Data = reader.ReadResource<EplCirclePolygon>( Version ); break;
                case 4: Data = reader.ReadResource<EplLightningPolygon>( Version ); break;
                case 5: Data = reader.ReadResource<EplTrajectoryPolygon>( Version ); break;
                case 6: Data = reader.ReadResource<EplWindPolygon>( Version ); break;
                case 7: Data = reader.ReadResource<EplModel>( Version ); break;
                case 8: Data = reader.ReadResource<EplTrajectoryPolygon>( Version ); break;
                case 9: Data = reader.ReadResource<EplBoardPolygon>( Version ); break;
                case 10: Data = reader.ReadResource<EplObjectParticles>( Version ); break;
                case 11: Data = reader.ReadResource<EplGlitterPolygon>( Version ); break;
                case 12: Data = reader.ReadResource<EplGlitterPolygon2>( Version ); break;
                case 13: Data = reader.ReadResource<EplDirectionalParticles>( Version ); break;
                case 14: Data = reader.ReadResource<EplCamera>( Version ); break;
                case 15: Data = reader.ReadResource<EplLight>( Version ); break;
                case 16: Data = reader.ReadResource<EplPostEffect>( Version ); break;
                case 17: Data = reader.ReadResource<EplHelper>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteInt32( (int)Flags );
            writer.WriteStringWithHash( Version, Name );
            writer.WriteResource( Data );
        }
    }

    public sealed class EplLeafDataHeader : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeafDataHeader;

        public uint Type { get; set; }
        public uint Field04 { get; set; }

        public EplLeafDataHeader() { }
        public EplLeafDataHeader( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Type = reader.ReadUInt32();
            Field04 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field04 );
        }
    }

    public sealed class EplEmbeddedFile : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplEmbeddedFile;

        public string FileName { get; set; }
        public uint Field18 { get; set; }
        public uint Field00 { get; set; }
        public int DataLength => Data?.Length ?? 0;
        public byte[] Data { get; set; }

        public EplEmbeddedFile() { }
        public EplEmbeddedFile( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FileName = reader.ReadStringWithHash( Version );
            Field18 = reader.ReadUInt32();

            Logger.Debug( $"EplEmbeddedFile: Reading {FileName}" );
            if ( Field18 == 2 )
            {
                Field00 = reader.ReadUInt32();
                var dataLength = reader.ReadInt32();
                Data = reader.ReadBytes( dataLength );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, FileName );
            writer.WriteUInt32( Field18 );
            if ( Field18 == 2 )
            {
                writer.WriteUInt32( Field00 );
                writer.WriteUInt32( (uint)DataLength );
                writer.WriteBytes( Data );
            }
        }
    }

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
        public Vector2 Field134 { get; set; }
        public Vector2 Field13C { get; set; }
        public float Field144 { get; set; }
        public uint Field148 { get; set; }
        public uint Field14C { get; set; }
        public float Field150 { get; set; }
        public float Field154 { get; set; }
        public float Field158 { get; set; }
        public Resource Params { get; set; }

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
            Field15C = reader.ReadUInt32();
            Field160 = reader.ReadSingle();
            Field164 = reader.ReadUInt32();
            Field040 = reader.ReadSingle();
            Field44 = reader.ReadVector2();
            FieldB4 = reader.ReadSingle();
            FieldC4 = reader.ReadUInt32();
            FieldB8 = reader.ReadVector2();
            if ( Version >= 0x1104041 )
                Field50 = reader.ReadResource<EplLeafCommonData2>( Version );
            if ( Version >= 0x1104701 )
                FieldC0 = reader.ReadSingle();
            if ( Version < 0x1104041 )
                FieldC8 = reader.ReadResource<EplLeafCommonData>( Version );
            else
                FieldC8 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field12C = reader.ReadSingle();
            Field130 = reader.ReadSingle();
            Field134 = reader.ReadVector2();
            Field13C = reader.ReadVector2();
            Field144 = reader.ReadSingle();
            Field148 = reader.ReadUInt32();
            Field14C = reader.ReadUInt32();
            Field150 = reader.ReadSingle();
            Field154 = reader.ReadSingle();
            Field158 = reader.ReadSingle();

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
            writer.WriteSingle( FieldB4 );
            writer.WriteUInt32( FieldC4 );
            writer.WriteVector2( FieldB8 );
            if ( Version >= 0x1104041 )
                writer.WriteResource( Field50 );
            if ( Version >= 0x1104701 )
                writer.WriteSingle( FieldC0 );
            writer.WriteResource( FieldC8 );
            writer.WriteSingle( Field12C );
            writer.WriteSingle( Field130 );
            writer.WriteVector2( Field134 );
            writer.WriteVector2( Field13C );
            writer.WriteSingle( Field144 );
            writer.WriteUInt32( Field148 );
            writer.WriteUInt32( Field14C );
            writer.WriteSingle( Field150 );
            writer.WriteSingle( Field154 );
            writer.WriteSingle( Field158 );
            if ( Params != null )
                writer.WriteResource( Params );
        }
    }

    public sealed class EplLeafCommonData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeafCommonData;

        public ushort Type { get; set; }
        public object Field04 { get; set; }
        public object Field08 { get; set; }
        public object Field0C { get; set; }
        public object Field10 { get; set; }
        public Vector4 Field14 { get; set; }
        public byte[] Field24 { get; set; }

        public EplLeafCommonData() { }
        public EplLeafCommonData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Type = reader.ReadUInt16();
            Logger.Debug( $"EplLeafCommonData: Reading type {Type}" );
            switch ( Type )
            {
                case 0:
                    {
                        Field04 = reader.ReadInt32();
                        Field0C = reader.ReadInt32();
                        break;
                    }
                case 1:
                    {
                        Field04 = reader.ReadSingle();
                        Field0C = reader.ReadSingle();
                        break;
                    }
                case 2:
                    {
                        Field04 = reader.ReadUInt32();
                        Field0C = reader.ReadUInt32();
                        break;
                    }
                case 3:
                    {
                        Field04 = reader.ReadSingle();
                        Field08 = reader.ReadSingle();
                        Field0C = reader.ReadSingle();
                        Field10 = reader.ReadSingle();
                        break;
                    }
            }
            Field14 = reader.ReadVector4();
            Field24 = reader.ReadBytes( 46 );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteUInt16( Type );
            switch ( Type )
            {
                case 0:
                    {
                        writer.WriteInt32( (int)Field04 );
                        writer.WriteInt32( (int)Field0C );
                        break;
                    }
                case 1:
                    {
                        writer.WriteSingle( (float)Field04 );
                        writer.WriteSingle( (float)Field0C );
                        break;
                    }
                case 2:
                    {
                        writer.WriteUInt32( (uint)Field04 );
                        writer.WriteUInt32( (uint)Field0C );
                        break;
                    }
                case 3:
                    {
                        writer.WriteSingle( (float)Field04 );
                        writer.WriteSingle( (float)Field08 );
                        writer.WriteSingle( (float)Field0C );
                        writer.WriteSingle( (float)Field10 );
                        break;
                    }
            }
            writer.WriteVector4( Field14 );
            writer.WriteBytes( Field24 );
        }
    }

    public sealed class EplLeafCommonData2 : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeafCommonData2;

        public ushort Type { get; set; }
        public object Field04 { get; set; }
        public object Field08 { get; set; }
        public object Field0C { get; set; }
        public object Field10 { get; set; }
        public object Field14 { get; set; }
        public object Field18 { get; set; }
        public object Field1C { get; set; }
        public object Field20 { get; set; }
        public Vector4 Field14_ { get; set; }
        public byte[] Field24 { get; set; }

        public EplLeafCommonData2() { }
        public EplLeafCommonData2( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Type = reader.ReadUInt16();
            Logger.Debug( $"EplLeafCommonData2: Reading type {Type}" );
            switch ( Type )
            {
                case 0:
                    {
                        Field04 = reader.ReadInt32();
                        Field0C = reader.ReadInt32();
                        Field14 = reader.ReadInt32();
                        Field1C = reader.ReadInt32();
                        break;
                    }
                case 1:
                    {
                        Field04 = reader.ReadSingle();
                        Field0C = reader.ReadSingle();
                        Field14 = reader.ReadSingle();
                        Field1C = reader.ReadSingle();
                        break;
                    }
                case 2:
                    {
                        Field04 = reader.ReadUInt32();
                        Field0C = reader.ReadUInt32();
                        Field14 = reader.ReadUInt32();
                        Field1C = reader.ReadUInt32();
                        break;
                    }
                case 3:
                    {
                        Field04 = reader.ReadSingle();
                        Field08 = reader.ReadSingle();
                        Field0C = reader.ReadSingle();
                        Field10 = reader.ReadSingle();
                        Field14 = reader.ReadSingle();
                        Field18 = reader.ReadSingle();
                        Field1C = reader.ReadSingle();
                        Field20 = reader.ReadSingle();
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            Field14_ = reader.ReadVector4();
            Field24 = reader.ReadBytes( 46 );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteUInt16( Type );
            switch ( Type )
            {
                case 0:
                    {
                        writer.WriteInt32( (int)Field04 );
                        writer.WriteInt32( (int)Field0C );
                        writer.WriteInt32( (int)Field14 );
                        writer.WriteInt32( (int)Field1C );
                        break;
                    }
                case 1:
                    {
                        writer.WriteSingle( (float)Field04 );
                        writer.WriteSingle( (float)Field0C );
                        writer.WriteSingle( (float)Field14 );
                        writer.WriteSingle( (float)Field1C );
                        break;
                    }
                case 2:
                    {
                        writer.WriteUInt32( (uint)Field04 );
                        writer.WriteUInt32( (uint)Field0C );
                        writer.WriteUInt32( (uint)Field14 );
                        writer.WriteUInt32( (uint)Field1C );
                        break;
                    }
                case 3:
                    {
                        writer.WriteSingle( (float)Field04 );
                        writer.WriteSingle( (float)Field08 );
                        writer.WriteSingle( (float)Field0C );
                        writer.WriteSingle( (float)Field10 );
                        writer.WriteSingle( (float)Field14 );
                        writer.WriteSingle( (float)Field18 );
                        writer.WriteSingle( (float)Field1C );
                        writer.WriteSingle( (float)Field20 );
                        break;
                    }
            }
            writer.WriteVector4( Field14_ );
            writer.WriteBytes( Field24 );
        }
    }

    public sealed class EplDummy : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplDummy;

        public EplLeafDataHeader Header { get; set; }

        public EplDummy() { }
        public EplDummy( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
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

    public sealed class EplFlashPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field08 { get; set; }
        public float Field14 { get; set; }
        public uint Field24 { get; set; }
        public uint Field84 { get; set; }
        public Vector2 Field08_ { get; set; }
        public Vector2 Field18 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        public float Field20 { get; set; }
        public float Field7C { get; set; }
        public float Field80 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplFlashPolygon() { }
        public EplFlashPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadUInt32();
            Field14 = reader.ReadSingle();
            Field24 = reader.ReadUInt32();
            Field84 = reader.ReadUInt32();
            Field08_ = reader.ReadVector2();
            Field18 = reader.ReadVector2();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            if ( Version > 0x1104700 )
                Field20 = reader.ReadSingle();
            if ( Version > 0x1104050 )
            {
                Field7C = reader.ReadSingle();
                Field80 = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplFlashPolygonRadiation>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplFlashPolygonExplosion>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplFlashPolygonRing>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplFlashPolygonSplash>( Version ); break;
                case 5: Polygon = reader.ReadResource<EplFlashPolygonCylinder>( Version ); break;
                default: throw new NotImplementedException( $"Epl flash polygon type {Type} not implemented" );
            }
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field08 );
            writer.WriteSingle( Field14 );
            writer.WriteUInt32( Field24 );
            writer.WriteUInt32( Field84 );
            writer.WriteVector2( Field08_ );
            writer.WriteVector2( Field18 );
            writer.WriteResource( Field28 );
            if ( Version > 0x1104700 )
                writer.WriteSingle( Field20 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field7C );
                writer.WriteSingle( Field80 );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplFlashPolygonRadiation : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonRadiation;

        public uint Field88 { get; set; }
        public uint Field8C { get; set; }
        public Vector2 Field90 { get; set; }
        public Vector2 Field98 { get; set; }
        public Vector2 FieldA0 { get; set; }
        public Vector2 FieldA8 { get; set; }
        public float FieldB0 { get; set; }
        public uint FieldB4 { get; set; }

        public EplFlashPolygonRadiation() { }
        public EplFlashPolygonRadiation( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadUInt32();
            Field8C = reader.ReadUInt32();
            Field90 = reader.ReadVector2();
            Field98 = reader.ReadVector2();
            FieldA0 = reader.ReadVector2();
            FieldA8 = reader.ReadVector2();
            FieldB0 = reader.ReadSingle();
            FieldB4 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Field88 );
            writer.WriteUInt32( Field8C );
            writer.WriteVector2( Field90 );
            writer.WriteVector2( Field98 );
            writer.WriteVector2( FieldA0 );
            writer.WriteVector2( FieldA8 );
            writer.WriteSingle( FieldB0 );
            writer.WriteUInt32( FieldB4 );
        }
    }

    public sealed class EplFlashPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonExplosion;

        public Vector2 Field88 { get; set; }
        public uint Field90 { get; set; }
        public uint Feild94 { get; set; }
        public Vector2 Field94 { get; set; }
        public Vector2 FieldA0 { get; set; }
        public Vector2 FieldA8 { get; set; }
        public float FieldB0 { get; set; }

        public EplFlashPolygonExplosion() { }
        public EplFlashPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadVector2();
            Field90 = reader.ReadUInt32();
            Feild94 = reader.ReadUInt32();
            Field94 = reader.ReadVector2();
            FieldA0 = reader.ReadVector2();
            FieldA8 = reader.ReadVector2();
            FieldB0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field88 );
            writer.WriteUInt32( Field90 );
            writer.WriteUInt32( Feild94 );
            writer.WriteVector2( Field94 );
            writer.WriteVector2( FieldA0 );
            writer.WriteVector2( FieldA8 );
            writer.WriteSingle( FieldB0 );
        }
    }

    public sealed class EplFlashPolygonRing : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonRing;

        public EplLeafCommonData Field88 { get; set; }
        public EplLeafCommonData FieldDC { get; set; }
        public uint Field130 { get; set; }
        public uint Field134 { get; set; }
        public Vector2 Field138 { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }
        public uint Field15C { get; set; }

        public EplFlashPolygonRing() { }
        public EplFlashPolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            Field130 = reader.ReadUInt32();
            Field134 = reader.ReadUInt32();
            Field138 = reader.ReadVector2();
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
            Field15C = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            writer.WriteResource( FieldDC );
            writer.WriteUInt32( Field130 );
            writer.WriteUInt32( Field134 );
            writer.WriteVector2( Field138 );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
            writer.WriteUInt32( Field15C );
        }
    }

    public sealed class EplFlashPolygonSplash : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonSplash;

        public EplLeafCommonData Field88 { get; set; }
        public EplLeafCommonData FieldDC { get; set; }
        public uint Field130 { get; set; }
        public uint Field134 { get; set; }
        public Vector2 Field138 { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }

        public EplFlashPolygonSplash() { }
        public EplFlashPolygonSplash( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            Field130 = reader.ReadUInt32();
            Field134 = reader.ReadUInt32();
            Field138 = reader.ReadVector2();
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            writer.WriteResource( FieldDC );
            writer.WriteUInt32( Field130 );
            writer.WriteUInt32( Field134 );
            writer.WriteVector2( Field138 );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
        }
    }

    public sealed class EplFlashPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplFlashPolygonCylinder;

        public EplLeafCommonData Field88 { get; set; }
        public Resource FieldDC { get; set; }
        public Vector2 Field140 { get; set; }
        public Vector2 Field148 { get; set; }
        public Vector2 Field150 { get; set; }
        public float Field158 { get; set; }
        public Vector2 Field15C { get; set; }
        public float Field164 { get; set; }
        public uint Field168 { get; set; }

        public EplFlashPolygonCylinder() { }
        public EplFlashPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            if ( Version <= 0x1104040 )
                FieldDC = reader.ReadResource<EplLeafCommonData>( Version );
            else
                FieldDC = reader.ReadResource<EplLeafCommonData2>( Version );
            Field140 = reader.ReadVector2();
            Field148 = reader.ReadVector2();
            Field150 = reader.ReadVector2();
            Field158 = reader.ReadSingle();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadSingle();
            Field168 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field88 );
            if ( Version <= 0x1104040 )
                writer.WriteResource( FieldDC );
            else
                writer.WriteResource( FieldDC );
            writer.WriteVector2( Field140 );
            writer.WriteVector2( Field148 );
            writer.WriteVector2( Field150 );
            writer.WriteSingle( Field158 );
            writer.WriteVector2( Field15C );
            writer.WriteSingle( Field164 );
            writer.WriteUInt32( Field168 );
        }
    }

    public sealed class EplCirclePolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field10 { get; set; }
        public uint Field1C { get; set; }
        public uint Field20 { get; set; }
        public Vector2 Field08 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplCirclePolygon() { }
        public EplCirclePolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field10 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            Field08 = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                Field14 = reader.ReadSingle();
                Field18 = reader.ReadSingle();
            }
            switch ( Type )
            {

                case 0: break;
                case 1: Polygon = reader.ReadResource<EplCirclePolygonRing>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplCirclePolygonTrajectory>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplCirclePolygonFill>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplCirclePolygonHoop>( Version ); break;
                default: throw new NotImplementedException( $"Epl circle polygon type {Type} not implemented" );
            }

            if ( Type != 1 && Type != 3 )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field1C );
            writer.WriteUInt32( Field20 );
            writer.WriteVector2( Field08 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field14 );
                writer.WriteSingle( Field18 );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );

            if ( Type != 1 && Type != 3 )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplCirclePolygonRing : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonRing;

        public float Field24 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        public EplLeafCommonData Field7C { get; set; }
        public Vector2 FieldD0 { get; set; }
        public uint FieldD8 { get; set; }
        public uint FieldDC { get; set; }
        public uint FieldE0 { get; set; }

        public EplCirclePolygonRing() { }
        public EplCirclePolygonRing( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            Field7C = reader.ReadResource<EplLeafCommonData>( Version );
            FieldD0 = reader.ReadVector2();
            FieldD8 = reader.ReadUInt32();
            FieldDC = reader.ReadUInt32();
            FieldE0 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteResource( Field28 );
            writer.WriteResource( Field7C );
            writer.WriteVector2( FieldD0 );
            writer.WriteUInt32( FieldD8 );
            writer.WriteUInt32( FieldDC );
            writer.WriteUInt32( FieldE0 );
        }
    }

    public sealed class EplCirclePolygonTrajectory : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonTrajectory;

        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public EplLeafCommonData Field2C { get; set; }
        public EplLeafCommonData Field90 { get; set; }
        public float FieldF4 { get; set; }
        public float FieldF8 { get; set; }
        public float FieldFC { get; set; }
        public float Field100 { get; set; }

        public EplCirclePolygonTrajectory() { }
        public EplCirclePolygonTrajectory( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadResource<EplLeafCommonData>( Version );
            Field90 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldF4 = reader.ReadSingle();
            FieldF8 = reader.ReadSingle();
            FieldFC = reader.ReadSingle();
            Field100 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteSingle( Field28 );
            writer.WriteResource( Field2C );
            writer.WriteResource( Field90 );
            writer.WriteSingle( FieldF4 );
            writer.WriteSingle( FieldF8 );
            writer.WriteSingle( FieldFC );
            writer.WriteSingle( Field100 );
        }
    }

    public sealed class EplCirclePolygonFill : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonFill;

        public float Field24 { get; set; }
        public EplLeafCommonData Field28 { get; set; }
        public EplLeafCommonData2 Field7C { get; set; }
        public EplLeafCommonData2 FieldE0 { get; set; }

        public EplCirclePolygonFill() { }
        public EplCirclePolygonFill( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadResource<EplLeafCommonData>( Version );
            Field7C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldE0 = reader.ReadResource<EplLeafCommonData2>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field24 );
            writer.WriteResource( Field28 );
            writer.WriteResource( Field7C );
            writer.WriteResource( FieldE0 );
        }
    }

    public sealed class EplCirclePolygonHoop : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplCirclePolygonHoop;

        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public EplLeafCommonData Field34 { get; set; }
        public Vector2 Field88 { get; set; }
        public EplLeafCommonData2 Field90 { get; set; }
        public EplLeafCommonData2 FieldF4 { get; set; }
        public EplLeafCommonData2 Field158 { get; set; }
        public float Field1BC { get; set; }
        public float Field1C0 { get; set; }

        public EplCirclePolygonHoop() { }
        public EplCirclePolygonHoop( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadResource<EplLeafCommonData>( Version );
            Field88 = reader.ReadVector2();
            Field90 = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldF4 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field158 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field1BC = reader.ReadSingle();
            Field1C0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteSingle( Field30 );
            writer.WriteResource( Field34 );
            writer.WriteVector2( Field88 );
            writer.WriteResource( Field90 );
            writer.WriteResource( FieldF4 );
            writer.WriteResource( Field158 );
            writer.WriteSingle( Field1BC );
            writer.WriteSingle( Field1C0 );
        }
    }

    public sealed class EplLightningPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field08 { get; set; }
        public uint Field1C { get; set; }
        public uint Feild20 { get; set; }
        public float Field44 { get; set; }
        public float Field48 { get; set; }
        public uint Field4C { get; set; }
        public uint Field50 { get; set; }
        public uint Field54 { get; set; }
        public uint Field60 { get; set; }
        public Vector2 Field0C { get; set; }
        public Vector2 Field24 { get; set; }
        public Vector2 Field2C { get; set; }
        public Vector2 Field34 { get; set; }
        public Vector2 Field14 { get; set; }
        public Vector2 Field3C { get; set; }
        public float Field58 { get; set; }
        public float Field5C { get; set; }
        public Resource Polygon { get; set; }

        public EplLightningPolygon() { }
        public EplLightningPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Feild20 = reader.ReadUInt32();
            Field44 = reader.ReadSingle();
            Field48 = reader.ReadSingle();
            Field4C = reader.ReadUInt32();
            Field50 = reader.ReadUInt32();
            Field54 = reader.ReadUInt32();
            Field60 = reader.ReadUInt32();
            Field0C = reader.ReadVector2();
            Field24 = reader.ReadVector2();
            Field2C = reader.ReadVector2();
            Field34 = reader.ReadVector2();
            Field14 = reader.ReadVector2();
            Field3C = reader.ReadVector2();
            if ( Version > 0x1104050 )
            {
                Field58 = reader.ReadSingle();
                Field5C = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplLightningPolygonRod>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplLightningPolygonBall>( Version ); break;
                    //         default: Assert(false, "Not implemented"); break;
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field08 );
            writer.WriteUInt32( Field1C );
            writer.WriteUInt32( Feild20 );
            writer.WriteSingle( Field44 );
            writer.WriteSingle( Field48 );
            writer.WriteUInt32( Field4C );
            writer.WriteUInt32( Field50 );
            writer.WriteUInt32( Field54 );
            writer.WriteUInt32( Field60 );
            writer.WriteVector2( Field0C );
            writer.WriteVector2( Field24 );
            writer.WriteVector2( Field2C );
            writer.WriteVector2( Field34 );
            writer.WriteVector2( Field14 );
            writer.WriteVector2( Field3C );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field58 );
                writer.WriteSingle( Field5C );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
        }
    }

    public sealed class EplLightningPolygonRod : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonRod;

        public Vector2 Field64 { get; set; }

        public EplLightningPolygonRod() { }
        public EplLightningPolygonRod( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field64 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field64 );
        }
    }

    public sealed class EplLightningPolygonBall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLightningPolygonBall;

        public EplLeafCommonData Field64 { get; set; }
        public Vector2 FieldB8 { get; set; }
        public Vector2 FieldC0 { get; set; }
        public float FieldC8 { get; set; }
        public uint FieldCC { get; set; }
        public Vector2 FieldD0 { get; set; }
        public float FieldD8 { get; set; }
        public uint FieldDC { get; set; }

        public EplLightningPolygonBall() { }
        public EplLightningPolygonBall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field64 = reader.ReadResource<EplLeafCommonData>( Version );
            FieldB8 = reader.ReadVector2();
            FieldC0 = reader.ReadVector2();
            FieldC8 = reader.ReadSingle();
            FieldCC = reader.ReadUInt32();
            FieldD0 = reader.ReadVector2();
            FieldD8 = reader.ReadSingle();
            FieldDC = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field64 );
            writer.WriteVector2( FieldB8 );
            writer.WriteVector2( FieldC0 );
            writer.WriteSingle( FieldC8 );
            writer.WriteUInt32( FieldCC );
            writer.WriteVector2( FieldD0 );
            writer.WriteSingle( FieldD8 );
            writer.WriteUInt32( FieldDC );
        }
    }

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
        public EplLeafCommonData2 FieldD8 { get; set; }
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
            FieldD8 = reader.ReadResource<EplLeafCommonData2>( Version );
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

    public sealed class EplWindPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field78 { get; set; }
        public uint Field88 { get; set; }
        public uint Field8C { get; set; }
        public uint FieldA8 { get; set; }
        public Resource Field14 { get; set; }
        public Vector2 Field0C { get; set; }
        public Vector2 Field90 { get; set; }
        public Vector2 Field98 { get; set; }
        public Vector2 Field7C { get; set; }
        public float Field84 { get; set; }
        public float FieldA0 { get; set; }
        public float FieldA4 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile Field70 { get; set; }

        public EplWindPolygon() { }
        public EplWindPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field78 = reader.ReadSingle();
            Field88 = reader.ReadUInt32();
            Field8C = reader.ReadUInt32();
            FieldA8 = reader.ReadUInt32();
            if ( Version <= 0x1104040 )
                Field14 = reader.ReadResource<EplLeafCommonData>( Version );
            else
                Field14 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field0C = reader.ReadVector2();
            Field90 = reader.ReadVector2();
            Field98 = reader.ReadVector2();
            Field7C = reader.ReadVector2();
            if ( Version > 0x1104700 )
                Field84 = reader.ReadSingle();
            if ( Version > 0x1104050 )
            {
                FieldA0 = reader.ReadSingle();
                FieldA4 = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 1: Polygon = reader.ReadResource<EplWindPolygonSpiral>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplWindPolygonExplosion>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplWindPolygonBall>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            Field70 = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field78 );
            writer.WriteUInt32( Field88 );
            writer.WriteUInt32( Field8C );
            writer.WriteUInt32( FieldA8 );
            if ( Version <= 0x1104040 )
                writer.WriteResource( Field14 );
            else
                writer.WriteResource( Field14 );
            writer.WriteVector2( Field0C );
            writer.WriteVector2( Field90 );
            writer.WriteVector2( Field98 );
            writer.WriteVector2( Field7C );
            if ( Version > 0x1104700 )
                writer.WriteSingle( Field84 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( FieldA0 );
                writer.WriteSingle( FieldA4 );
            }
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            writer.WriteResource( Field70 );
        }
    }

    public sealed class EplWindPolygonSpiral : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonSpiral;

        public EplLeafCommonData FieldAC { get; set; }
        public EplLeafCommonData Field100 { get; set; }
        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }
        public uint Field1C4 { get; set; }
        public Vector2 Field1C8 { get; set; }

        public EplWindPolygonSpiral() { }
        public EplWindPolygonSpiral( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadResource<EplLeafCommonData>( Version );
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
            Field1C4 = reader.ReadUInt32();
            Field1C8 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteResource( Field100 );
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
            writer.WriteUInt32( Field1C4 );
            writer.WriteVector2( Field1C8 );
        }
    }

    public sealed class EplWindPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonExplosion;

        public EplLeafCommonData FieldAC { get; set; }
        public Vector2 Field100 { get; set; }
        public Vector2 Field108 { get; set; }
        public float Field110 { get; set; }
        public float Field114 { get; set; }
        public Vector2 Field118 { get; set; }

        public EplWindPolygonExplosion() { }
        public EplWindPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadVector2();
            Field108 = reader.ReadVector2();
            Field110 = reader.ReadSingle();
            Field114 = reader.ReadSingle();
            if ( Version > 0x1104080 )
                Field118 = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteVector2( Field100 );
            writer.WriteVector2( Field108 );
            writer.WriteSingle( Field110 );
            writer.WriteSingle( Field114 );
            if ( Version > 0x1104080 )
                writer.WriteVector2( Field118 );
        }
    }

    public sealed class EplWindPolygonBall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplWindPolygonBall;

        public EplLeafCommonData FieldAC { get; set; }
        public EplLeafCommonData Field100 { get; set; }
        public Vector2 Field154 { get; set; }
        public Vector2 Field15C { get; set; }
        public float Field164 { get; set; }
        public uint Field168 { get; set; }
        public Vector2 Field16C { get; set; }

        public EplWindPolygonBall() { }
        public EplWindPolygonBall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            FieldAC = reader.ReadResource<EplLeafCommonData>( Version );
            Field100 = reader.ReadResource<EplLeafCommonData>( Version );
            Field154 = reader.ReadVector2();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadSingle();
            Field168 = reader.ReadUInt32();
            Field16C = reader.ReadVector2();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( FieldAC );
            writer.WriteResource( Field100 );
            writer.WriteVector2( Field154 );
            writer.WriteVector2( Field15C );
            writer.WriteSingle( Field164 );
            writer.WriteUInt32( Field168 );
            writer.WriteVector2( Field16C );
        }
    }

    public sealed class EplModel : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public Resource Data { get; set; }
        public byte HasEmbeddedFile { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplModel() { }
        public EplModel( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            if ( Version > 0x1104050 )
            {
                Field04 = reader.ReadSingle();
                Field08 = reader.ReadSingle();
            }
            switch ( Type )
            {
                case 0: break;
                case 1: Data = reader.ReadResource<EplModel3DData>( Version ); break;
                case 2: Data = reader.ReadResource<EplModel2DData>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            HasEmbeddedFile = reader.ReadByte();
            if ( HasEmbeddedFile == 1 )
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            if ( Version > 0x1104050 )
            {
                writer.WriteSingle( Field04 );
                writer.WriteSingle( Field08 );
            }
            if ( Data != null ) writer.WriteResource( Data );
            writer.WriteByte( HasEmbeddedFile );
            if ( HasEmbeddedFile == 1 )
                writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplModel3DData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel3DData;


        public EplModel3DData() { }
        public EplModel3DData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
        }

        protected override void WriteCore( ResourceWriter writer )
        {
        }
    }

    public sealed class EplModel2DData : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplModel2DData;

        public float Field0C { get; set; }

        public EplModel2DData() { }
        public EplModel2DData( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field0C );
        }
    }

    public sealed class EplBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplBoardPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field10 { get; set; }
        public uint Field1C { get; set; }
        public Vector2 Field08 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public Resource Polygon { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplBoardPolygon() { }
        public EplBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field10 = reader.ReadUInt32();
            Field1C = reader.ReadUInt32();
            Field08 = reader.ReadVector2();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplSquareBoardPolygon>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplRectangleBoardPolygon>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field10 );
            writer.WriteUInt32( Field1C );
            writer.WriteVector2( Field08 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            if ( Polygon != null ) writer.WriteResource( Polygon );
            writer.WriteResource( EmbeddedFile );
        }
    }

    public sealed class EplSquareBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplSquareBoardPolygon;

        public EplLeafCommonData2 Field28 { get; set; }
        public EplLeafCommonData2 Field8C { get; set; }
        public Vector2 FieldFC { get; set; }
        public float FieldF0 { get; set; }
        public float Field104 { get; set; }
        public uint Field108 { get; set; }
        public Vector2 FieldF4 { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }

        public EplSquareBoardPolygon() { }
        public EplSquareBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field8C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldFC = reader.ReadVector2();
            FieldF0 = reader.ReadSingle();
            Field104 = reader.ReadSingle();
            Field108 = reader.ReadUInt32();
            if ( Version > 0x1104280 )
            {
                FieldF4 = reader.ReadVector2();
                if ( Version > 0x1104290 )
                {
                    Field20 = reader.ReadSingle();
                    Field24 = reader.ReadSingle();
                }
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field28 );
            writer.WriteResource( Field8C );
            writer.WriteVector2( FieldFC );
            writer.WriteSingle( FieldF0 );
            writer.WriteSingle( Field104 );
            writer.WriteUInt32( Field108 );
            if ( Version > 0x1104280 )
            {
                writer.WriteVector2( FieldF4 );
                if ( Version > 0x1104290 )
                {
                    writer.WriteSingle( Field20 );
                    writer.WriteSingle( Field24 );
                }
            }
        }
    }

    public sealed class EplRectangleBoardPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplRectangleBoardPolygon;

        public EplLeafCommonData2 Field28 { get; set; }
        public EplLeafCommonData2 Field8C { get; set; }
        public EplLeafCommonData2 FieldF0 { get; set; }
        public Vector2 Field160 { get; set; }
        public float Field154 { get; set; }
        public float Field168 { get; set; }
        public uint Field158 { get; set; }
        public Vector2 Field16C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }

        public EplRectangleBoardPolygon() { }
        public EplRectangleBoardPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field28 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field8C = reader.ReadResource<EplLeafCommonData2>( Version );
            FieldF0 = reader.ReadResource<EplLeafCommonData2>( Version );
            Field160 = reader.ReadVector2();
            Field154 = reader.ReadSingle();
            Field168 = reader.ReadSingle();
            Field158 = reader.ReadUInt32();
            Field16C = reader.ReadVector2();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field28 );
            writer.WriteResource( Field8C );
            writer.WriteResource( FieldF0 );
            writer.WriteVector2( Field160 );
            writer.WriteSingle( Field154 );
            writer.WriteSingle( Field168 );
            writer.WriteUInt32( Field158 );
            writer.WriteVector2( Field16C );
            writer.WriteSingle( Field20 );
            writer.WriteSingle( Field24 );
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

    public class EplGlitterPolygon2 : EplGlitterPolygon
    {

    }

    public class EplGlitterPolygon : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygon;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public uint Field08 { get; set; }
        public uint Field20 { get; set; }
        public uint Field150 { get; set; }
        public Vector2 Field08_ { get; set; }
        public Vector2 Field14 { get; set; }
        public EplLeafCommonData FieldEC { get; set; }
        public float Field1C { get; set; }
        public EplLeafCommonData Field24 { get; set; }
        public EplLeafCommonData Field88 { get; set; }
        public float Field140 { get; set; }
        public float Field144 { get; set; }
        public float Field148 { get; set; }
        public float Field14C { get; set; }
        public Resource Polygon { get; set; }
        public bool Field70 { get; set; }
        public EplEmbeddedFile EmbeddedFile { get; set; }

        public EplGlitterPolygon() { }
        public EplGlitterPolygon( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadUInt32();
            Field20 = reader.ReadUInt32();
            Field150 = reader.ReadUInt32();
            Field08_ = reader.ReadVector2();
            Field14 = reader.ReadVector2();
            FieldEC = reader.ReadResource<EplLeafCommonData>( Version );
            Field1C = reader.ReadSingle();
            Field24 = reader.ReadResource<EplLeafCommonData>( Version );
            Field88 = reader.ReadResource<EplLeafCommonData>( Version );
            Field140 = reader.ReadSingle();
            Field144 = reader.ReadSingle();
            Field148 = reader.ReadSingle();
            Field14C = reader.ReadSingle();
            Field70 = reader.ReadBoolean();
            switch ( Type )
            {
                case 0: break;
                case 1: Polygon = reader.ReadResource<EplGlitterPolygonExplosion>( Version ); break;
                case 2: Polygon = reader.ReadResource<EplGlitterPolygonSplash>( Version ); break;
                case 3: Polygon = reader.ReadResource<EplGlitterPolygonCylinder>( Version ); break;
                case 4: Polygon = reader.ReadResource<EplGlitterPolygonWall>( Version ); break;
                default: Debug.Assert( false, "Not implemented" ); break;
            }
            if ( Field70 )
            {
                EmbeddedFile = reader.ReadResource<EplEmbeddedFile>( Version );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteUInt32( Field08 );
            writer.WriteUInt32( Field20 );
            writer.WriteUInt32( Field150 );
            writer.WriteVector2( Field08_ );
            writer.WriteVector2( Field14 );
            writer.WriteResource( FieldEC );
            writer.WriteSingle( Field1C );
            writer.WriteResource( Field24 );
            writer.WriteResource( Field88 );
            writer.WriteSingle( Field140 );
            writer.WriteSingle( Field144 );
            writer.WriteSingle( Field148 );
            writer.WriteSingle( Field14C );
            writer.WriteBoolean( Field70 );
            if ( Polygon != null )
                writer.WriteResource( Polygon );
            if ( Field70 )
            {
                writer.WriteResource( EmbeddedFile );
            }
        }
    }

    public sealed class EplGlitterPolygonExplosion : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonExplosion;

        public Vector2 Field154 { get; set; }
        public Vector2 Field15C { get; set; }
        public Vector2 Field164 { get; set; }
        public Vector2 Field16C { get; set; }
        public float Field174 { get; set; }

        public EplGlitterPolygonExplosion() { }
        public EplGlitterPolygonExplosion( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadVector2();
            Field15C = reader.ReadVector2();
            Field164 = reader.ReadVector2();
            Field16C = reader.ReadVector2();
            Field174 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector2( Field154 );
            writer.WriteVector2( Field15C );
            writer.WriteVector2( Field164 );
            writer.WriteVector2( Field16C );
            writer.WriteSingle( Field174 );
        }
    }

    public sealed class EplGlitterPolygonSplash : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonSplash;

        public EplLeafCommonData Field154 { get; set; }
        public EplLeafCommonData Field1A8 { get; set; }
        public Vector2 Field1FC { get; set; }
        public Vector2 Field204 { get; set; }
        public Vector2 Field20C { get; set; }
        public float Field214 { get; set; }

        public EplGlitterPolygonSplash() { }
        public EplGlitterPolygonSplash( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1FC = reader.ReadVector2();
            Field204 = reader.ReadVector2();
            Field20C = reader.ReadVector2();
            Field214 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteResource( Field1A8 );
            writer.WriteVector2( Field1FC );
            writer.WriteVector2( Field204 );
            writer.WriteVector2( Field20C );
            writer.WriteSingle( Field214 );
        }
    }

    public sealed class EplGlitterPolygonCylinder : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonCylinder;

        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }
        public Vector2 Field1C4 { get; set; }
        public float Field1CC { get; set; }
        public uint Field1D0 { get; set; }

        public EplGlitterPolygonCylinder() { }
        public EplGlitterPolygonCylinder( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
            Field1C4 = reader.ReadVector2();
            Field1CC = reader.ReadSingle();
            Field1D0 = reader.ReadUInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
            writer.WriteVector2( Field1C4 );
            writer.WriteSingle( Field1CC );
            writer.WriteUInt32( Field1D0 );
        }
    }

    public sealed class EplGlitterPolygonWall : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplGlitterPolygonWall;

        public EplLeafCommonData Field154 { get; set; }
        public Vector2 Field1A8 { get; set; }
        public Vector2 Field1B0 { get; set; }
        public Vector2 Field1B8 { get; set; }
        public float Field1C0 { get; set; }

        public EplGlitterPolygonWall() { }
        public EplGlitterPolygonWall( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Field154 = reader.ReadResource<EplLeafCommonData>( Version );
            Field1A8 = reader.ReadVector2();
            Field1B0 = reader.ReadVector2();
            Field1B8 = reader.ReadVector2();
            Field1C0 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteResource( Field154 );
            writer.WriteVector2( Field1A8 );
            writer.WriteVector2( Field1B0 );
            writer.WriteVector2( Field1B8 );
            writer.WriteSingle( Field1C0 );
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
            if ( Version > 0x11084180 )
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
            if ( Version > 0x11084180 )
            {
                writer.WriteUInt32( Field00 );
            }
            writer.WriteResource( ParticleEmittter );
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

    public sealed class EplHelper : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplHelper;

        public EplLeafDataHeader Header { get; set; }
        public uint Type { get; set; }
        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public bool Field84 { get; set; }
        public EplEmbeddedFile EmbeddedFile1 { get; set; }
        public bool Field88 { get; set; }
        public EplEmbeddedFile EmbeddedFile2 { get; set; }

        public EplHelper() { }
        public EplHelper( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Header = reader.ReadResource<EplLeafDataHeader>( Version );
            Type = reader.ReadUInt32();
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field84 = reader.ReadBoolean();
            if ( Field84 )
                EmbeddedFile1 = reader.ReadResource<EplEmbeddedFile>( Version );
            Field88 = reader.ReadBoolean();
            if ( Field88 )
                EmbeddedFile2 = reader.ReadResource<EplEmbeddedFile>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteResource( Header );
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteBoolean( Field84 );
            if ( Field84 )
                writer.WriteResource( EmbeddedFile1 );
            writer.WriteBoolean( Field88 );
            if ( Field88 )
                writer.WriteResource( EmbeddedFile2 );
        }
    }

}
