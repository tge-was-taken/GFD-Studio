using GFDLibrary.IO;
using System;
using System.Diagnostics;
using System.Numerics;

namespace GFDLibrary.Effects
{
    public sealed class EplLeaf : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeaf;

        public EplLeafFlags Flags { get; set; }
        public string Name { get; set; }
        public Resource Data { get; set; }
        // METAPHOR
        public Vector2 Field34 { get; set; }
        public Vector2 Field3C { get; set; }
        public EplLeaf() { }
        public EplLeaf( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            if ( Version > 0x2110060 )
            {
                Field34 = reader.ReadVector2();
                Field3C = reader.ReadVector2();
            }
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
            if ( Version > 0x2110060 )
            {
                writer.WriteVector2( Field34 );
                writer.WriteVector2( Field3C );
            }
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
        //public ushort Field06 { get; set; }

        public EplLeafDataHeader() { }
        public EplLeafDataHeader( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Type = reader.ReadUInt32();
            Field04 = reader.ReadUInt32();
            /*
            if ( Version < 0x2000002 )
                Field04 = reader.ReadUInt32();
            else
            {
                Field04 = reader.ReadUInt16();
                Field06 = reader.ReadUInt16();
            }
            */
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Type );
            writer.WriteUInt32( Field04 );
            //writer.WriteUInt16( Field06 );
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
        public ushort Field62 { get; set; }

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
            if ( Version > 0x2110186 )
                Field62 = reader.ReadUInt16();

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
            if ( Version > 0x2110186 )
                writer.WriteUInt16( Field62 );
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
}
