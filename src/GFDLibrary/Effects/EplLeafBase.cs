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

        /// <summary>Which kind of leaf this is.</summary>
        public uint Category { get; set; }

        /// <summary>Which variant within the category. Read again as the body's first field.</summary>
        public uint Type { get; set; }
        //public ushort Field06 { get; set; }

        public EplLeafDataHeader() { }
        public EplLeafDataHeader( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            Category = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            /*
            if ( Version < 0x2000002 )
                Type = reader.ReadUInt32();
            else
            {
                Type = reader.ReadUInt16();
                Field06 = reader.ReadUInt16();
            }
            */
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteUInt32( Category );
            writer.WriteUInt32( Type );
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
                Data = reader.ReadBytes( reader.ValidateCount( dataLength ) );
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
        [FormerName( "Field04" )]
        public object StartValue { get; set; }
        [FormerName( "Field08" )]
        public object StartRange { get; set; }
        [FormerName( "Field0C" )]
        public object EndValue { get; set; }
        [FormerName( "Field10" )]
        public object EndRange { get; set; }
        [FormerName( "Field14" )]
        public Vector4 Points { get; set; }
        [FormerName( "Field24" )]
        public byte[] Table { get; set; }

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
                        StartValue = reader.ReadInt32();
                        EndValue = reader.ReadInt32();
                        break;
                    }
                case 1:
                    {
                        StartValue = reader.ReadSingle();
                        EndValue = reader.ReadSingle();
                        break;
                    }
                case 2:
                    {
                        StartValue = reader.ReadUInt32();
                        EndValue = reader.ReadUInt32();
                        break;
                    }
                case 3:
                    {
                        StartValue = reader.ReadSingle();
                        StartRange = reader.ReadSingle();
                        EndValue = reader.ReadSingle();
                        EndRange = reader.ReadSingle();
                        break;
                    }
            }
            Points = reader.ReadVector4();
            Table = reader.ReadBytes( 46 );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteUInt16( Type );
            switch ( Type )
            {
                case 0:
                    {
                        writer.WriteInt32( (int)StartValue );
                        writer.WriteInt32( (int)EndValue );
                        break;
                    }
                case 1:
                    {
                        writer.WriteSingle( (float)StartValue );
                        writer.WriteSingle( (float)EndValue );
                        break;
                    }
                case 2:
                    {
                        writer.WriteUInt32( (uint)StartValue );
                        writer.WriteUInt32( (uint)EndValue );
                        break;
                    }
                case 3:
                    {
                        writer.WriteSingle( (float)StartValue );
                        writer.WriteSingle( (float)StartRange );
                        writer.WriteSingle( (float)EndValue );
                        writer.WriteSingle( (float)EndRange );
                        break;
                    }
            }
            writer.WriteVector4( Points );
            writer.WriteBytes( Table );
        }
    }

    public sealed class EplLeafCommonData2 : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplLeafCommonData2;

        public ushort Type { get; set; }
        [FormerName( "Field04" )]
        public object Target0 { get; set; }
        [FormerName( "Field08" )]
        public object Target0Range { get; set; }
        [FormerName( "Field0C" )]
        public object Target1 { get; set; }
        [FormerName( "Field10" )]
        public object Target1Range { get; set; }
        [FormerName( "Field14" )]
        public object Target2 { get; set; }
        [FormerName( "Field18" )]
        public object Target2Range { get; set; }
        [FormerName( "Field1C" )]
        public object Target3 { get; set; }
        [FormerName( "Field20" )]
        public object Target3Range { get; set; }
        [FormerName( "Field14_" )]
        public Vector4 Points { get; set; }
        [FormerName( "Field24" )]
        public byte[] Table { get; set; }
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
                        Target0 = reader.ReadInt32();
                        Target1 = reader.ReadInt32();
                        Target2 = reader.ReadInt32();
                        Target3 = reader.ReadInt32();
                        break;
                    }
                case 1:
                    {
                        Target0 = reader.ReadSingle();
                        Target1 = reader.ReadSingle();
                        Target2 = reader.ReadSingle();
                        Target3 = reader.ReadSingle();
                        break;
                    }
                case 2:
                    {
                        Target0 = reader.ReadUInt32();
                        Target1 = reader.ReadUInt32();
                        Target2 = reader.ReadUInt32();
                        Target3 = reader.ReadUInt32();
                        break;
                    }
                case 3:
                    {
                        Target0 = reader.ReadSingle();
                        Target0Range = reader.ReadSingle();
                        Target1 = reader.ReadSingle();
                        Target1Range = reader.ReadSingle();
                        Target2 = reader.ReadSingle();
                        Target2Range = reader.ReadSingle();
                        Target3 = reader.ReadSingle();
                        Target3Range = reader.ReadSingle();
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            Points = reader.ReadVector4();
            Table = reader.ReadBytes( 46 );
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
                        writer.WriteInt32( (int)Target0 );
                        writer.WriteInt32( (int)Target1 );
                        writer.WriteInt32( (int)Target2 );
                        writer.WriteInt32( (int)Target3 );
                        break;
                    }
                case 1:
                    {
                        writer.WriteSingle( (float)Target0 );
                        writer.WriteSingle( (float)Target1 );
                        writer.WriteSingle( (float)Target2 );
                        writer.WriteSingle( (float)Target3 );
                        break;
                    }
                case 2:
                    {
                        writer.WriteUInt32( (uint)Target0 );
                        writer.WriteUInt32( (uint)Target1 );
                        writer.WriteUInt32( (uint)Target2 );
                        writer.WriteUInt32( (uint)Target3 );
                        break;
                    }
                case 3:
                    {
                        writer.WriteSingle( (float)Target0 );
                        writer.WriteSingle( (float)Target0Range );
                        writer.WriteSingle( (float)Target1 );
                        writer.WriteSingle( (float)Target1Range );
                        writer.WriteSingle( (float)Target2 );
                        writer.WriteSingle( (float)Target2Range );
                        writer.WriteSingle( (float)Target3 );
                        writer.WriteSingle( (float)Target3Range );
                        break;
                    }
            }
            writer.WriteVector4( Points );
            writer.WriteBytes( Table );
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
