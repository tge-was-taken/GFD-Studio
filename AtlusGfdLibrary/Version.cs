using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public struct Version : IEquatable<Version>, IComparable<Version>
    {
        private const uint MAJOR_MASK = 0xFFu << 24;
        private const uint MINOR_MASK = 0xFFu << 16;
        private const uint REVISION_MASK = 0xFFu << 8;
        private const uint BUILD_MASK = 0xFF;

        public uint Value;     

        public byte Major
        {
            get { return ( byte )( Value & MAJOR_MASK ); }
            set
            {
                Value &= ~MAJOR_MASK;
                Value |= value;
            }
        }

        public byte Minor
        {
            get { return ( byte )( Value & MINOR_MASK ); }
            set
            {
                Value &= ~MINOR_MASK;
                Value |= value;
            }
        }

        public byte Revision
        {
            get { return ( byte )( Value & REVISION_MASK ); }
            set
            {
                Value &= ~REVISION_MASK;
                Value |= value;
            }
        }

        public byte Build
        {
            get { return ( byte )( Value & BUILD_MASK ); }
            set
            {
                Value &= ~BUILD_MASK;
                Value |= value;
            }
        }

        public Version(uint value)
        {
            Value = value;
        }

        public Version(byte major, byte minor, byte revision, byte build)
        {
            Value = 0;
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
        }

        public static Version Persona5 = new Version( 0x01105070 );

        public bool Equals( Version other )
        {
            return Value == other.Value;
        }

        public int CompareTo( Version other )
        {
            if ( Value > other.Value )
                return -1;

            if ( Value == other.Value )
                return 0;

            return 1;
        }
    }
}
