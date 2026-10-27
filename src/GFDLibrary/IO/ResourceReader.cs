using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using GFDLibrary.Common;
using GFDLibrary.IO.Common;

namespace GFDLibrary.IO
{
    public class ResourceReader : EndianBinaryReader
    {
#if NETCOREAPP1_0_OR_GREATER
        private static readonly Encoding sSJISEncoding = CodePagesEncodingProvider.Instance.GetEncoding( 932 );
#else
        private static readonly Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
#endif

        public bool EndOfStream => Position >= BaseStream.Length;

        public ResourceReader( Stream stream, bool leaveOpen ) : base( stream, Encoding.Default, leaveOpen, Endianness.BigEndian )
        //public ResourceReader( Stream stream, bool leaveOpen ) : base( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian )
        {
        }

        public ResourceFileHeader ReadFileHeader()
        {
            var header = new ResourceFileHeader();
            header.Read( this );
            return header;
        }

        public ResourceChunkHeader ReadChunkHeader()
        {
            var header = new ResourceChunkHeader();
            header.Read( this );
            return header;
        }

        public new string ReadString()
        {
            var length = ReadUInt16();
            if ( length == 0 ) return "";
            var bytes = ReadBytes( length );
            return sSJISEncoding.GetString( bytes );
        }

        public string ReadString( int length )
        {
            if ( length == 0 ) return "";
            var bytes = ReadBytes( length );
            return sSJISEncoding.GetString( bytes );
        }

        public string ReadStringWithHash( uint version, bool withPadding = false, bool isCatherineFullBodyData = false )
        {
            var str = ReadString();

            if ( str.Length > 0 && version > 0x1080000 )
            {
                if ( withPadding && ( version > 0x01105100 || isCatherineFullBodyData ) )
                {
                    SeekCurrent( 1 ); // padding byte
                }

                // hash
                ReadUInt32();
            }

            return str;
        }

        public float ReadHalf()
        {
            var encoded = ReadUInt16();

            // Decode half float
            // Based on numpy implementation (https://github.com/numpy/numpy/blob/984bc91367f9b525eadef14c48c759999bc4adfc/numpy/core/src/npymath/halffloat.c#L477)
            uint decoded;
            uint halfExponent = ( encoded & 0x7c00u );
            uint singleSign = ( encoded & 0x8000u ) << 16;
            switch ( halfExponent )
            {
                case 0x0000u: /* 0 or subnormal */
                    uint halfSign = ( encoded & 0x03ffu );

                    if ( halfSign == 0 )
                    {
                        /* Signed zero */
                        decoded = singleSign;
                    }
                    else
                    {
                        /* Subnormal */
                        halfSign <<= 1;

                        while ( ( halfSign & 0x0400u ) == 0 )
                        {
                            halfSign <<= 1;
                            halfExponent++;
                        }

                        uint singleExponent = 127 - 15 - halfExponent << 23;
                        uint singleSig = ( halfSign & 0x03ffu ) << 13;
                        decoded = singleSign + singleExponent + singleSig;
                    }
                    break;

                case 0x7c00u: /* inf or NaN */
                    /* All-ones exponent and a copy of the significand */
                    decoded = singleSign + 0x7f800000u + ( ( encoded & 0x03ffu ) << 13 );
                    break;

                default: /* normalized */
                    /* Just need to adjust the exponent and shift */
                    decoded = singleSign + ( ( ( encoded & 0x7fffu ) + 0x1c000u ) << 13 );
                    break;
            }

            return UnsafeUtilities.ReinterpretCast<uint, float>( decoded );
        }

        public Vector2 ReadVector2()
        {
            Vector2 value;
            value.X = ReadSingle();
            value.Y = ReadSingle();
            return value;
        }

        public Vector3 ReadVector3()
        {
            Vector3 value;
            value.X = ReadSingle();
            value.Y = ReadSingle();
            value.Z = ReadSingle();
            return value;
        }

        public Vector2 ReadVector2Half()
        {
            Vector2 value;
            value.X = ReadHalf();
            value.Y = ReadHalf();
            return value;
        }

        public Vector3 ReadVector3Half()
        {
            Vector3 value;
            value.X = ReadHalf();
            value.Y = ReadHalf();
            value.Z = ReadHalf();
            return value;
        }

        public ByteVector3 ReadByteVector3()
        {
            ByteVector3 value;
            value.X = ReadByte();
            value.Y = ReadByte();
            value.Z = ReadByte();
            return value;
        }

        public Vector4 ReadVector4()
        {
            Vector4 value;
            value.X = ReadSingle();
            value.Y = ReadSingle();
            value.Z = ReadSingle();
            value.W = ReadSingle();
            return value;
        }

        public ByteVector4 ReadByteVector4()
        {
            ByteVector4 value;
            value.X = ReadByte();
            value.Y = ReadByte();
            value.Z = ReadByte();
            value.W = ReadByte();
            return value;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion value;
            value.X = ReadSingle();
            value.Y = ReadSingle();
            value.Z = ReadSingle();
            value.W = ReadSingle();
            return value;
        }

        public Quaternion ReadQuaternionHalf()
        {
            Quaternion value;
            value.X = ReadHalf();
            value.Y = ReadHalf();
            value.Z = ReadHalf();
            value.W = ReadHalf();
            return value;
        }

        public Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 value;
            value.M11 = ReadSingle();
            value.M12 = ReadSingle();
            value.M13 = ReadSingle();
            value.M14 = ReadSingle();
            value.M21 = ReadSingle();
            value.M22 = ReadSingle();
            value.M23 = ReadSingle();
            value.M24 = ReadSingle();
            value.M31 = ReadSingle();
            value.M32 = ReadSingle();
            value.M33 = ReadSingle();
            value.M34 = ReadSingle();
            value.M41 = ReadSingle();
            value.M42 = ReadSingle();
            value.M43 = ReadSingle();
            value.M44 = ReadSingle();
            return value;
        }

        public BoundingBox ReadBoundingBox()
        {
            BoundingBox value;
            value.Max = ReadVector3();
            value.Min = ReadVector3();
            return value;
        }

        public BoundingSphere ReadBoundingSphere()
        {
            BoundingSphere value;
            value.Center = ReadVector3();
            value.Radius = ReadSingle();
            return value;
        }

        public T ReadResource<T>( uint version ) where T : Resource, new()
        {
            var obj = new T();
            obj.Version = version;
            obj.Read( this );
            return obj;
        }

        public List<T> ReadResources<T>( uint version, int count ) where T : Resource, new()
        {
            var list = new List<T>();
            ReadResources<T>( version, list, count );
            return list;
        }

        public void ReadResources<T>( uint version, IList<T> list, int count ) where T : Resource, new()
        {
            for ( int i = 0; i < count; i++ )
            {
                var res = ReadResource<T>( version );
                if ( i + 1 > list.Count ) list.Add( res );
                else list[i] = res; 
            }
        }

        public List<T> ReadResourceList<T>( uint version ) where T : Resource, new()
        {
            var count = ReadInt32();
            var list = new List<T>( count );
            for ( int i = 0; i < count; i++ )
            {
                list.Add( ReadResource<T>( version ) );
            }

            return list;
        }
    }
}