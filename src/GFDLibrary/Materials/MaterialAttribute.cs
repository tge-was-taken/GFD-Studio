using System;
using System.Diagnostics;
using System.Numerics;
using GFDLibrary.IO;
using GFDLibrary.IO.Common;

namespace GFDLibrary.Materials
{
    public abstract class MaterialAttribute : Resource
    {
        private const uint FLAG_MASK = 0xFFFF0000;
        private const int FLAG_SHIFT = 16;
        private const uint TYPE_MASK = 0x0000FFFF;

        public override ResourceType ResourceType => ResourceType.MaterialAttribute;

        public uint RawFlags { get; private set; } 

        public MaterialAttributeFlags Flags
        {
            get => ( MaterialAttributeFlags )( (RawFlags & FLAG_MASK) >> FLAG_SHIFT );
            set
            {
                RawFlags &= ~FLAG_MASK;
                RawFlags |= (uint)( ( (ushort)value << FLAG_SHIFT ) & FLAG_MASK );
            }
        }

        public MaterialAttributeType AttributeType
        {
            get => ( MaterialAttributeType )( RawFlags & TYPE_MASK );
            set
            {
                RawFlags &= ~TYPE_MASK;
                RawFlags |= ( (ushort)value & TYPE_MASK );
            }
        }

        protected MaterialAttribute( uint privateFlags, uint version ) : base(version)
            => RawFlags = privateFlags;

        protected MaterialAttribute( MaterialAttributeFlags flags, MaterialAttributeType type )
        {
            Flags = flags;
            AttributeType = type;
        }

        internal static MaterialAttribute Read( ResourceReader reader, uint version )
        {
            MaterialAttribute attribute = null;
            uint flags = reader.ReadUInt32();

            switch ( ( MaterialAttributeType )( flags & 0xFFFF ) )
            {
                case MaterialAttributeType.Type0:
                    attribute = new MaterialAttributeType0( flags, version );
                    break;

                case MaterialAttributeType.Type1:
                    attribute = new MaterialAttributeType1( flags, version );
                    break;

                case MaterialAttributeType.Type2:
                    attribute = new MaterialAttributeType2( flags, version );
                    break;

                case MaterialAttributeType.Type3:
                    attribute = new MaterialAttributeType3( flags, version );
                    break;

                case MaterialAttributeType.Type4:
                    attribute = new MaterialAttributeType4( flags, version );
                    break;

                case MaterialAttributeType.Type5:
                    attribute = new MaterialAttributeType5( flags, version );
                    break;

                case MaterialAttributeType.Type6:
                    attribute = new MaterialAttributeType6( flags, version );
                    break;

                case MaterialAttributeType.Type7:
                    attribute = new MaterialAttributeType7( flags, version );
                    break;

                case MaterialAttributeType.Type8:
                    attribute = new MaterialAttributeType8( flags, version );
                    break;

                default:
                    Trace.Assert( false, $"Unknown material attribute type: {flags:X8}" );
                    break;
            }

            attribute.ReadCore( reader );

            return attribute;
        }

        public static void Save( MaterialAttribute attribute, string path )
        {
            using ( var stream = FileUtils.Create( path ) )
            using ( var writer = new ResourceWriter(stream, false ))
            {
                writer.WriteFileHeader( ResourceFileIdentifier.Model, attribute.Version, ResourceType.MaterialAttribute );
                Write( writer, attribute );
            }
        }

        internal static void Write( ResourceWriter writer, MaterialAttribute attribute )
        {
            writer.WriteUInt32( attribute.RawFlags );
            writer.WriteResource( attribute );
        }
    }

    public enum MaterialAttributeFlags : ushort
    {
        Bit0 = 1,
    }

    public enum MaterialAttributeType : ushort
    {
        Type0 = 0,
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
    }

    public sealed class MaterialAttributeType0 : MaterialAttribute
    {
        // 0C
        public Vector4 Color { get; set; }

        // 1C
        public float LightTreshold { get; set; }

        // 20
        public float LightFactor { get; set; }

        // 24
        public float LightBrightness { get; set; }

        // 28
        public float ShadowTreshold { get; set; }

        // 2C
        public float ShadowFactor { get; set; }

        // 30
        public MaterialAttributeType0Flags Type0Flags { get; set; }

        public MaterialAttributeType0() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type0 ) { }

        internal MaterialAttributeType0( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType0( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type0 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            if ( Version > 0x1104500 )
            {
                Color = reader.ReadVector4();
                LightTreshold = reader.ReadSingle();
                LightFactor = reader.ReadSingle();
                LightBrightness = reader.ReadSingle();
                ShadowTreshold = reader.ReadSingle();
                ShadowFactor = reader.ReadSingle();
                Type0Flags = ( MaterialAttributeType0Flags )reader.ReadInt32();
            }
            else if ( Version > 0x1104220 )
            {
                Color = reader.ReadVector4();
                LightTreshold = reader.ReadSingle();
                LightFactor = reader.ReadSingle();
                LightBrightness = reader.ReadSingle();
                ShadowTreshold = reader.ReadSingle();
                ShadowFactor = reader.ReadSingle();

                if ( reader.ReadBoolean() )
                    Type0Flags |= MaterialAttributeType0Flags.LightNormalMap;

                if ( reader.ReadBoolean() )
                    Type0Flags |= MaterialAttributeType0Flags.LightAdd;

                if ( reader.ReadBoolean() )
                    Type0Flags |= MaterialAttributeType0Flags.ShadowNormalMap;

                if ( Version > 0x1104260 )
                {
                    if ( reader.ReadBoolean() )
                        Type0Flags |= MaterialAttributeType0Flags.LockYAxis;
                }
            }
            else
            {
                Color = reader.ReadVector4();
                LightTreshold = reader.ReadSingle();
                LightFactor = reader.ReadSingle();
                LightBrightness = 1.0f;
                ShadowTreshold = reader.ReadSingle();
                ShadowFactor = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( Version > 0x1104500 )
            {
                writer.WriteVector4( Color );
                writer.WriteSingle( LightTreshold );
                writer.WriteSingle( LightFactor );
                writer.WriteSingle( LightBrightness );
                writer.WriteSingle( ShadowTreshold );
                writer.WriteSingle( ShadowFactor );
                writer.WriteInt32( ( int )Type0Flags );
            }
            else if ( Version > 0x1104220 )
            {
                writer.WriteVector4( Color );
                writer.WriteSingle( LightTreshold );
                writer.WriteSingle( LightFactor );
                writer.WriteSingle( LightBrightness );
                writer.WriteSingle( ShadowTreshold );
                writer.WriteSingle( ShadowFactor );

                writer.WriteBoolean( Type0Flags.HasFlag( MaterialAttributeType0Flags.LightNormalMap ) );
                writer.WriteBoolean( Type0Flags.HasFlag( MaterialAttributeType0Flags.LightAdd ) );
                writer.WriteBoolean( Type0Flags.HasFlag( MaterialAttributeType0Flags.ShadowNormalMap ) );

                if ( Version > 0x1104260 )
                {
                    writer.WriteBoolean( Type0Flags.HasFlag( MaterialAttributeType0Flags.LockYAxis ) );
                }
            }
            else
            {
                writer.WriteVector4( Color );
                writer.WriteSingle( LightTreshold );
                writer.WriteSingle( LightFactor );
                writer.WriteSingle( ShadowTreshold );
                writer.WriteSingle( ShadowFactor );
            }
        }
    }

    [Flags]
    public enum MaterialAttributeType0Flags
    {
        LightNormalMap = 1 << 0,
        LightAdd = 1 << 1,
        ShadowNormalMap = 1 << 2,
        LockYAxis = 1 << 3,
        LightNormalMapAlphaMask = 1 << 4,
        LightDiffuseMapAlphaMask = 1 << 5,
        LightShadowMapAlphaMask = 1 << 6,
    }

    public sealed class MaterialAttributeType1 : MaterialAttribute
    {
        // 0C
        public float Field0C { get; set; }
        
        // 10
        public Vector4 LightColor { get; set; }

        // 1C
        public float LightTreshold { get; set; }

        // 20
        public float LightFactor { get; set; }

        // 24
        public Vector4 ShadowColor { get; set; }

        // 34
        public float ShadowThreshold { get; set; }

        // 38
        public float ShadowFactor { get; set; }

        // 3C
        public MaterialAttributeType1Flags Type1Flags { get; set; }

        public MaterialAttributeType1() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type1 ) { }

        internal MaterialAttributeType1( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType1( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type1 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            LightColor = reader.ReadVector4();
            LightTreshold = reader.ReadSingle();
            LightFactor = reader.ReadSingle();
            ShadowColor = reader.ReadVector4();
            ShadowThreshold = reader.ReadSingle();
            ShadowFactor = reader.ReadSingle();

            if ( Version <= 0x1104500 )
            {
                if ( reader.ReadBoolean() )
                    Type1Flags |= MaterialAttributeType1Flags.NormalMap;

                if ( Version > 0x1104180 )
                {
                    if ( reader.ReadBoolean() )
                        Type1Flags |= MaterialAttributeType1Flags.Backlight;
                }

                if ( Version > 0x1104210 )
                {
                    if ( reader.ReadBoolean() )
                        Type1Flags |= MaterialAttributeType1Flags.LightAdd;
                }

                if ( Version > 0x1104400 )
                {
                    if ( reader.ReadBoolean() )
                        Type1Flags |= MaterialAttributeType1Flags.Cavernmap;
                }
            }
            else
            {
                Type1Flags = ( MaterialAttributeType1Flags )reader.ReadInt32();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( LightColor );
            writer.WriteSingle( LightTreshold );
            writer.WriteSingle( LightFactor );
            writer.WriteVector4( ShadowColor );
            writer.WriteSingle( ShadowThreshold );
            writer.WriteSingle( ShadowFactor );

            if ( Version <= 0x1104500 )
            {
                writer.WriteBoolean( Type1Flags.HasFlag( MaterialAttributeType1Flags.NormalMap ) );

                if ( Version > 0x1104180 )
                {
                    writer.WriteBoolean( Type1Flags.HasFlag( MaterialAttributeType1Flags.Backlight ) );
                }

                if ( Version > 0x1104210 )
                {
                    writer.WriteBoolean( Type1Flags.HasFlag( MaterialAttributeType1Flags.LightAdd ) );
                }

                if ( Version > 0x1104400 )
                {
                    writer.WriteBoolean( Type1Flags.HasFlag( MaterialAttributeType1Flags.Cavernmap ) );
                }
            }
            else
            {
                writer.WriteInt32( ( int )Type1Flags );
            }
        }
    }

    [Flags]
    public enum MaterialAttributeType1Flags
    {
        NormalMap = 1 << 0,
        Backlight = 1 << 1,
        LightAdd = 1 << 2,
        Cavernmap = 1 << 3,
        LightNormalMapAlphaMask = 1 << 4,
        LockYAxis = 1 << 5,
        LightDiffuseMapAlphaMask = 1 << 6,
        LightShadowMapAlphaMask = 1 << 7,
    }

    public sealed class MaterialAttributeType2 : MaterialAttribute
    {
        // 0C
        public MaterialAttributeType2Flags Flags2 { get; set; }

        // 10
        public int Color { get; set; }

        public MaterialAttributeType2() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type2 ) { }

        internal MaterialAttributeType2( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType2( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type2 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Flags2 = (MaterialAttributeType2Flags)reader.ReadInt32();
            Color = reader.ReadInt32();
        }

        [Flags]
        public enum MaterialAttributeType2Flags
        {
            Normal = 1 << 0,
            Disable = 1 << 1,
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( (int)Flags2 );
            writer.WriteInt32( Color );
        }
    }

    public sealed class MaterialAttributeType3 : MaterialAttribute
    {
        // 0C
        public float Field0C { get; set; }

        // 10
        public float Field10 { get; set; }

        // 14
        public float Field14 { get; set; }

        // 18
        public float Field18 { get; set; }

        // 1C
        public float Field1C { get; set; }

        // 20
        public float Field20 { get; set; }

        // 24
        public float Field24 { get; set; }

        // 28
        public float Field28 { get; set; }

        // 2C
        public float Field2C { get; set; }

        // 30
        public float Field30 { get; set; }

        // 34
        public float Field34 { get; set; }

        // 38
        public float Field38 { get; set; }

        // 3C
        public int Field3C { get; set; }

        public MaterialAttributeType3() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type3 ) { }

        internal MaterialAttributeType3( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType3( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type3 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field3C = reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteSingle( Field0C );
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteSingle( Field1C );
            writer.WriteSingle( Field20 );
            writer.WriteSingle( Field24 );
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteSingle( Field30 );
            writer.WriteSingle( Field34 );
            writer.WriteSingle( Field38 );
            writer.WriteInt32( Field3C );
        }
    }

    public sealed class MaterialAttributeType4 : MaterialAttribute
    {
        // 0C
        public Vector4 LightColor { get; set; }

        // 1C
        public float LightThreshold { get; set; }

        // 20
        public float LightFactor { get; set; }

        // 24
        public Vector4 ShadowColor { get; set; }

        // 34
        public float ShadowThreshold { get; set; }

        // 38
        public float ShadowFactor { get; set; }

        // 3C
        public float NightMapSpeed { get; set; }

        // 40
        public float NightMapPower { get; set; }

        // 44
        public float NightMapScale { get; set; }

        // 48
        public float NightMapHeight { get; set; }

        // 4C
        public float NightMapAlpha { get; set; }

        // 50
        public byte NightMapDirection { get; set; }

        // 54
        public float DarkGradientHeight { get; set; }

        // 58
        public float DarkGradientAlpha { get; set; }

        // 5C
        public MaterialAttributeType1Flags Flags2 { get; set; }

        public MaterialAttributeType4() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type4 ) { }

        internal MaterialAttributeType4( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType4( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type4 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            LightColor = reader.ReadVector4();
            LightThreshold = reader.ReadSingle();
            LightFactor = reader.ReadSingle();
            ShadowColor = reader.ReadVector4();
            ShadowThreshold = reader.ReadSingle();
            ShadowFactor = reader.ReadSingle();
            NightMapSpeed = reader.ReadSingle();
            NightMapPower = reader.ReadSingle();
            NightMapScale = reader.ReadSingle();
            NightMapHeight = reader.ReadSingle();
            NightMapAlpha = reader.ReadSingle();
            NightMapDirection = reader.ReadByte();
            DarkGradientHeight = reader.ReadSingle();
            DarkGradientAlpha = reader.ReadSingle();
            Flags2 = ( MaterialAttributeType1Flags )reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( LightColor );
            writer.WriteSingle( LightThreshold );
            writer.WriteSingle( LightFactor );
            writer.WriteVector4( ShadowColor );
            writer.WriteSingle( ShadowThreshold );
            writer.WriteSingle( ShadowFactor );
            writer.WriteSingle( NightMapSpeed );
            writer.WriteSingle( NightMapPower );
            writer.WriteSingle( NightMapScale );
            writer.WriteSingle( NightMapHeight );
            writer.WriteSingle( NightMapAlpha );
            writer.WriteByte( NightMapDirection );
            writer.WriteSingle( DarkGradientHeight );
            writer.WriteSingle( DarkGradientAlpha );
            writer.WriteInt32( (int)Flags2 );
        }
    }

    public sealed class MaterialAttributeType5 : MaterialAttribute
    {
        public int Field0C { get; set; }

        public int Field10 { get; set; }

        public float Field14 { get; set; }

        public float Field18 { get; set; }

        public Vector4 Field1C { get; set; }

        public float Field2C { get; set; }

        public float Field30 { get; set; }

        public float Field34 { get; set; }

        public float Field38 { get; set; }

        public float Field3C { get; set; }

        public float Field40 { get; set; }

        public Vector4 Field48 { get; set; }

        public MaterialAttributeType5() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type5 ) { }

        internal MaterialAttributeType5( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType5( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type5 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadVector4();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field3C = reader.ReadSingle();
            Field48 = reader.ReadVector4();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( Field0C );
            writer.WriteInt32( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteVector4( Field1C );
            writer.WriteSingle( Field2C );
            writer.WriteSingle( Field30 );
            writer.WriteSingle( Field34 );
            writer.WriteSingle( Field38 );
            writer.WriteSingle( Field3C );
            writer.WriteVector4( Field48 );
        }
    }

    public sealed class MaterialAttributeType6 : MaterialAttribute
    {
        public int Field0C { get; set; }

        public int Field10 { get; set; }

        public int Field14 { get; set; }

        public MaterialAttributeType6() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type6 ) { }

        internal MaterialAttributeType6( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType6( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type6 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Field0C = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field14 = reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( Field0C );
            writer.WriteInt32( Field10 );
            writer.WriteInt32( Field14 );
        }
    }

    public sealed class MaterialAttributeType7 : MaterialAttribute
    {
        public MaterialAttributeType7() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type7 ) { }

        internal MaterialAttributeType7( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType7( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type7 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
        }

        protected override void WriteCore( ResourceWriter writer )
        {
        }
    }

    public sealed class MaterialAttributeType8 : MaterialAttribute
    {
        public Vector3 Field00 { get; set; }

        public float Field0C { get; set; }

        public float Field10 { get; set; }

        public float Field14 { get; set; }

        public float Field18 { get; set; }

        public Vector3 Field1C { get; set; }

        public float Field28 { get; set; }

        public float Field2C { get; set; }

        public int Field30 { get; set; }

        public int Field34 { get; set; }

        public int Field38 { get; set; }

        public MaterialAttributeType8Flags Type8Flags { get; set; }

        public MaterialAttributeType8() : base( MaterialAttributeFlags.Bit0, MaterialAttributeType.Type8 ) { }

        internal MaterialAttributeType8( uint privateFlags, uint version ) : base( privateFlags, version ) { }

        public MaterialAttributeType8( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type8 )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Field00 = reader.ReadVector3();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadVector3();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadInt32();
            Field34 = reader.ReadInt32();
            Field38 = reader.ReadInt32();
            Type8Flags = ( MaterialAttributeType8Flags )reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector3( Field00 );
            writer.WriteSingle( Field0C );
            writer.WriteSingle( Field10 );
            writer.WriteSingle( Field14 );
            writer.WriteSingle( Field18 );
            writer.WriteVector3( Field1C );
            writer.WriteSingle( Field28 );
            writer.WriteSingle( Field2C );
            writer.WriteInt32( Field30 );
            writer.WriteInt32( Field34 );
            writer.WriteInt32( Field38 );
            writer.WriteInt32( ( int )Type8Flags );
        }
    }

    [Flags]
    public enum MaterialAttributeType8Flags
    {
        Bit0 = 1 << 0,
        Bit1 = 1 << 1,
        Bit2 = 1 << 2,
        Bit3 = 1 << 3,
        Bit4 = 1 << 4,
        Bit5 = 1 << 5,
        Bit6 = 1 << 6,
        Bit7 = 1 << 7,
        Bit8 = 1 << 8,
        Bit9 = 1 << 9,
        Bit10 = 1 << 10,
        Bit11 = 1 << 11,
        Bit12 = 1 << 12,
        Bit13 = 1 << 13,
        Bit14 = 1 << 14,
        Bit15 = 1 << 15,
        Bit16 = 1 << 16,
        Bit17 = 1 << 17,
        Bit18 = 1 << 18,
        Bit19 = 1 << 19,
        Bit20 = 1 << 20,
        Bit21 = 1 << 21,
        Bit22 = 1 << 22,
        Bit23 = 1 << 23,
        Bit24 = 1 << 24,
        Bit25 = 1 << 25,
        Bit26 = 1 << 26,
        Bit27 = 1 << 27,
        Bit28 = 1 << 28,
        Bit29 = 1 << 29,
        Bit30 = 1 << 30,
        Bit31 = 1 << 31,
    }
}