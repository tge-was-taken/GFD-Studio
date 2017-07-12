using System;
using System.Numerics;

namespace AtlusGfdLib
{
    public abstract class MaterialProperty
    {
        private const uint FLAG_MASK = 0xFFFF0000;
        private const int FLAG_SHIFT = 16;
        private const uint TYPE_MASK = 0x0000FFFF;

        private uint mPrivateFlags;

        public MaterialPropertyFlags Flags
        {
            get { return ( MaterialPropertyFlags )( (mPrivateFlags & FLAG_MASK) >> FLAG_SHIFT ); }
            set
            {
                mPrivateFlags &= ~FLAG_MASK;
                mPrivateFlags |= (uint)( ( (ushort)value << FLAG_SHIFT ) & FLAG_MASK );
            }
        }

        public MaterialPropertyType Type
        {
            get { return ( MaterialPropertyType )( mPrivateFlags & TYPE_MASK ); }
            set
            {
                mPrivateFlags &= ~TYPE_MASK;
                mPrivateFlags |= ( (ushort)value & TYPE_MASK );
            }
        }

        protected MaterialProperty( uint privateFlags ) => mPrivateFlags = privateFlags;

        protected MaterialProperty( MaterialPropertyFlags flags, MaterialPropertyType type )
        {
            Flags = flags;
            Type = type;
        }
    }

    public enum MaterialPropertyFlags : ushort
    {
        Flag1 = 1,
    }

    public enum MaterialPropertyType : ushort
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

    public sealed class MaterialPropertyType0 : MaterialProperty
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
        public int Field30 { get; set; }

        public MaterialPropertyType0() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type0 ) { }

        internal MaterialPropertyType0( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType0( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type0 )
        {
        }
    }

    public sealed class MaterialPropertyType1 : MaterialProperty
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
        public int Field3C { get; set; }

        public MaterialPropertyType1() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type1 ) { }

        internal MaterialPropertyType1( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType1( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type1 )
        {
        }
    }

    public sealed class MaterialPropertyType2 : MaterialProperty
    {
        // 0C
        public int Field0C { get; set; }

        // 10
        public int Field10 { get; set; }

        public MaterialPropertyType2() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type2 ) { }

        internal MaterialPropertyType2( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType2( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type2 )
        {
        }
    }

    public sealed class MaterialPropertyType3 : MaterialProperty
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

        public MaterialPropertyType3() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type3 ) { }

        internal MaterialPropertyType3( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType3( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type3 )
        {
        }
    }

    public sealed class MaterialPropertyType4 : MaterialProperty
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

        public MaterialPropertyType4() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type4 ) { }

        internal MaterialPropertyType4( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType4( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type4 )
        {
        }
    }

    public sealed class MaterialPropertyType5 : MaterialProperty
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

        public MaterialPropertyType5() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type5 ) { }

        internal MaterialPropertyType5( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType5( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type5 )
        {
        }
    }

    public sealed class MaterialPropertyType6 : MaterialProperty
    {
        public int Field0C { get; set; }

        public int Field10 { get; set; }

        public int Field14 { get; set; }

        public MaterialPropertyType6() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type6 ) { }

        internal MaterialPropertyType6( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType6( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type6 )
        {
        }
    }

    public sealed class MaterialPropertyType7 : MaterialProperty
    {
        public MaterialPropertyType7() : base( MaterialPropertyFlags.Flag1, MaterialPropertyType.Type7 ) { }

        internal MaterialPropertyType7( uint privateFlags ) : base( privateFlags ) { }

        public MaterialPropertyType7( MaterialPropertyFlags flags ) : base( flags, MaterialPropertyType.Type7 )
        {
        }
    }
}