using System.Numerics;

namespace AtlusGfdLib
{
    public abstract class MaterialAttribute : Resource
    {
        private const uint FLAG_MASK = 0xFFFF0000;
        private const int FLAG_SHIFT = 16;
        private const uint TYPE_MASK = 0x0000FFFF;

        public uint RawFlags { get; private set; } 

        public MaterialAttributeFlags Flags
        {
            get { return ( MaterialAttributeFlags )( (RawFlags & FLAG_MASK) >> FLAG_SHIFT ); }
            set
            {
                RawFlags &= ~FLAG_MASK;
                RawFlags |= (uint)( ( (ushort)value << FLAG_SHIFT ) & FLAG_MASK );
            }
        }

        public MaterialAttributeType Type
        {
            get { return ( MaterialAttributeType )( RawFlags & TYPE_MASK ); }
            set
            {
                RawFlags &= ~TYPE_MASK;
                RawFlags |= ( (ushort)value & TYPE_MASK );
            }
        }

        protected MaterialAttribute( uint privateFlags ) : base( ResourceType.MaterialAttribute, PERSONA5_RESOURCE_VERSION )
            => RawFlags = privateFlags;

        protected MaterialAttribute( MaterialAttributeFlags flags, MaterialAttributeType type ) : base( ResourceType.MaterialAttribute, PERSONA5_RESOURCE_VERSION )
        {
            Flags = flags;
            Type = type;
        }
    }

    public enum MaterialAttributeFlags : ushort
    {
        Flag1 = 1,
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
    }

    public sealed class MaterialAttributeType0 : MaterialAttribute
    {
        // 0C
        public Vector4 Field0C { get; set; }

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
        public MaterialAttributeType0Flags Type0Flags { get; set; }

        public MaterialAttributeType0() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type0 ) { }

        internal MaterialAttributeType0( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType0( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type0 )
        {
        }
    }

    public enum MaterialAttributeType0Flags
    {
        Flag1 = 0b0001,
        Flag2 = 0b0010,
        Flag4 = 0b0100,
        Flag8 = 0b1000,
    }

    public sealed class MaterialAttributeType1 : MaterialAttribute
    {
        // 10
        public Vector4 Field0C { get; set; }

        // 1C
        public float Field1C { get; set; }

        // 20
        public float Field20 { get; set; }

        // 24
        public Vector4 Field24 { get; set; }

        // 34
        public float Field34 { get; set; }

        // 38
        public float Field38 { get; set; }

        // 3C
        public MaterialAttributeType1Flags Type1Flags { get; set; }

        public MaterialAttributeType1() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type1 ) { }

        internal MaterialAttributeType1( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType1( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type1 )
        {
        }
    }

    public enum MaterialAttributeType1Flags
    {
        Flag1 = 0b0001,
        Flag2 = 0b0010,
        Flag4 = 0b0100,
        Flag8 = 0b1000,
    }

    public sealed class MaterialAttributeType2 : MaterialAttribute
    {
        // 0C
        public int Field0C { get; set; }

        // 10
        public int Field10 { get; set; }

        public MaterialAttributeType2() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type2 ) { }

        internal MaterialAttributeType2( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType2( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type2 )
        {
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

        public MaterialAttributeType3() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type3 ) { }

        internal MaterialAttributeType3( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType3( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type3 )
        {
        }
    }

    public sealed class MaterialAttributeType4 : MaterialAttribute
    {
        // 0C
        public Vector4 Field0C { get; set; }

        // 1C
        public float Field1C { get; set; }

        // 20
        public float Field20 { get; set; }

        // 24
        public Vector4 Field24 { get; set; }

        // 34
        public float Field34 { get; set; }

        // 38
        public float Field38 { get; set; }

        // 3C
        public float Field3C { get; set; }

        // 40
        public float Field40 { get; set; }

        // 44
        public float Field44 { get; set; }

        // 48
        public float Field48 { get; set; }

        // 4C
        public float Field4C { get; set; }

        // 50
        public byte Field50 { get; set; }

        // 54
        public float Field54 { get; set; }

        // 58
        public float Field58 { get; set; }

        // 5C
        public int Field5C { get; set; }

        public MaterialAttributeType4() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type4 ) { }

        internal MaterialAttributeType4( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType4( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type4 )
        {
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

        public MaterialAttributeType5() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type5 ) { }

        internal MaterialAttributeType5( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType5( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type5 )
        {
        }
    }

    public sealed class MaterialAttributeType6 : MaterialAttribute
    {
        public int Field0C { get; set; }

        public int Field10 { get; set; }

        public int Field14 { get; set; }

        public MaterialAttributeType6() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type6 ) { }

        internal MaterialAttributeType6( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType6( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type6 )
        {
        }
    }

    public sealed class MaterialAttributeType7 : MaterialAttribute
    {
        public MaterialAttributeType7() : base( MaterialAttributeFlags.Flag1, MaterialAttributeType.Type7 ) { }

        internal MaterialAttributeType7( uint privateFlags ) : base( privateFlags ) { }

        public MaterialAttributeType7( MaterialAttributeFlags flags ) : base( flags, MaterialAttributeType.Type7 )
        {
        }
    }
}