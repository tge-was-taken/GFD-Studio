using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using GFDLibrary.Common;
using GFDLibrary.IO.Common;

namespace GFDLibrary.IO
{
    public class ResourceWriter : EndianBinaryWriter
    {
#if NETCOREAPP1_0_OR_GREATER
        private static readonly Encoding sSJISEncoding = CodePagesEncodingProvider.Instance.GetEncoding( 932 );
#else
        private static readonly Encoding sSJISEncoding = Encoding.GetEncoding( 932 );
#endif

        public ResourceWriter( Stream stream, bool leaveOpen ) : base( stream, Encoding.Default, leaveOpen, Endianness.BigEndian )
        {
        }

        public void WriteByte( byte value ) => Write( value );

        public void WriteBoolean( bool value ) => Write( ( byte )( value ? 1 : 0 ) );

        public void WriteBytes( byte[] value ) => Write( value );

        public void WriteInt16( short value ) => Write( value );

        public void WriteUInt16( ushort value ) => Write( value );

        public void WriteInt32( int value ) => Write( value );

        public void WriteUInt32( uint value ) => Write( value );

        public void WriteSingle( float value ) => Write( value );

        public void WriteHalf( float value ) => Write( FloatToHalf( value ) );

        // https://blog.fpmurphy.com/2008/12/half-precision-floating-point-format_14.html
        private static ushort FloatToHalf( float value )
        {
            int i = UnsafeUtilities.ReinterpretCast<float, int>( value );
            int s = ( i >> 16 ) & 0x00008000;                       // sign
            int e = ( ( i >> 23 ) & 0x000000ff ) - ( 127 - 15 );    // exponent
            int f = i & 0x007fffff;                                 // fraction

            // need to handle NaNs and Inf?
            if ( e <= 0 )
            {
                if ( e < -10 )
                {
                    if ( s > 0 ) // handle -0.0
                        return 0x8000;
                    else
                        return 0;
                }
                f = ( f | 0x00800000 ) >> ( 1 - e );
                return ( ushort ) ( s | ( f >> 13 ) );
            }
            else if ( e == 0xff - ( 127 - 15 ) )
            {
                if ( f == 0 ) // Inf
                    return ( ushort ) ( s | 0x7c00 );
                else
                {
                    // NAN
                    f >>= 13;
                    return ( ushort ) ( s | 0x7c00 | f | ( f == 0 ? 1 : 0 ) );
                }
            }
            else
            {
                if ( e > 30 ) // Overflow
                    return ( ushort ) ( s | 0x7c00 );
                return ( ushort ) ( s | ( e << 10 ) | ( f >> 13 ) );
            }
        }

        public void WriteStringRaw( string value ) => WriteBytes( sSJISEncoding.GetBytes( value ) );

        public void WriteString( string value )
        {
            WriteUInt16( ( ushort )sSJISEncoding.GetByteCount( value ) );
            WriteStringRaw( value );
        }

        public void WriteStringWithHash( uint version, string value, bool withPadding = false, bool isCatherineFullBodyData = false )
        {
            WriteString( value );

            if ( isCatherineFullBodyData && withPadding )
                WriteByte( 0 ); // padding byte

            if ( version > 0x1080000 )
                WriteInt32( StringHasher.GenerateStringHash( value ) );
        }

        public void WriteVector2( Vector2 value )
        {
            WriteSingle( value.X );
            WriteSingle( value.Y );
        }

        public void WriteVector3( Vector3 value )
        {
            WriteSingle( value.X );
            WriteSingle( value.Y );
            WriteSingle( value.Z );
        }


        public void WriteVector3Half( Vector3 value )
        {
            WriteHalf( value.X );
            WriteHalf( value.Y );
            WriteHalf( value.Z );
        }

        public void WriteByteVector3( ByteVector3 value )
        {
            WriteByte( value.X );
            WriteByte( value.Y );
            WriteByte( value.Z );
        }

        public void WriteVector4( Vector4 value )
        {
            WriteSingle( value.X );
            WriteSingle( value.Y );
            WriteSingle( value.Z );
            WriteSingle( value.W );
        }

        public void WriteByteVector4( ByteVector4 value )
        {
            WriteByte( value.X );
            WriteByte( value.Y );
            WriteByte( value.Z );
            WriteByte( value.W );
        }

        public void WriteQuaternion( Quaternion value )
        {
            WriteSingle( value.X );
            WriteSingle( value.Y );
            WriteSingle( value.Z );
            WriteSingle( value.W );
        }

        public void WriteQuaternionHalf( Quaternion value )
        {
            WriteHalf( value.X );
            WriteHalf( value.Y );
            WriteHalf( value.Z );
            WriteHalf( value.W );
        }

        public void WriteMatrix4x4( Matrix4x4 matrix4x4 )
        {
            WriteSingle( matrix4x4.M11 );
            WriteSingle( matrix4x4.M12 );
            WriteSingle( matrix4x4.M13 );
            WriteSingle( matrix4x4.M14 );
            WriteSingle( matrix4x4.M21 );
            WriteSingle( matrix4x4.M22 );
            WriteSingle( matrix4x4.M23 );
            WriteSingle( matrix4x4.M24 );
            WriteSingle( matrix4x4.M31 );
            WriteSingle( matrix4x4.M32 );
            WriteSingle( matrix4x4.M33 );
            WriteSingle( matrix4x4.M34 );
            WriteSingle( matrix4x4.M41 );
            WriteSingle( matrix4x4.M42 );
            WriteSingle( matrix4x4.M43 );
            WriteSingle( matrix4x4.M44 );
        }

        public void WriteBoundingBox( BoundingBox boundingBox )
        {
            WriteVector3( boundingBox.Max );
            WriteVector3( boundingBox.Min );
        }

        public void WriteBoundingSphere( BoundingSphere boundingSphere )
        {
            WriteVector3( boundingSphere.Center );
            WriteSingle( boundingSphere.Radius );
        }

        public void WriteFileHeader( ResourceFileIdentifier identifier, uint version, ResourceType type )
        {
            var header = new ResourceFileHeader() { Identifier = identifier, Version = version, Type = type };
            header.Write( this );
        }

        public void WriteResourceChunk( Resource resource )
        {
            // Get chunk type
            ResourceChunkType type;
            switch ( resource.ResourceType )
            {
                case ResourceType.TextureDictionary:
                    type = ResourceChunkType.TextureDictionary;
                    break;
                case ResourceType.MaterialDictionary:
                    type = ResourceChunkType.MaterialDictionary;
                    break;
                case ResourceType.Model:
                    type = ResourceChunkType.Model;
                    break;
                case ResourceType.ChunkType000100F9:
                    type = ResourceChunkType.ChunkType000100F9;
                    break;
                case ResourceType.ChunkType000100F8:
                    type = ResourceChunkType.ChunkType000100F8;
                    break;
                case ResourceType.AnimationPack:
                    type = ResourceChunkType.AnimationPack;
                    break;
                default:
                    throw new InvalidOperationException( $"Resource of type {resource.ResourceType} has no associated chunk type." );
            }

            // Skip to-be chunk header
            var startPos = Position;
            SeekCurrent( ResourceChunkHeader.SIZE );

            // Write chunk body
            resource.Write( this );
            var endPos = Position;
            var delta = endPos - startPos;

            // Write chunk header
            SeekBegin( startPos );
            var header = new ResourceChunkHeader( resource.Version, type, (int)delta );
            header.Write( this );

            // Seek to end of the chunk
            SeekBegin( endPos );
        }

        public void WriteEndChunk( uint version )
        {
            var header = new ResourceChunkHeader( version, ResourceChunkType.Invalid, 0 );
            header.Write( this );
        }

        public void WriteResource<T>(T value) where T : Resource
        {
            value.Write( this );
        }

        public void WriteResources<T>( IEnumerable<T> values ) where T : Resource
        {
            foreach ( var value in values )
            {
                value.Write( this );
            }
        }


        internal void WriteResourceList<T>( List<T> list ) where T : Resource
        {
            WriteInt32( list.Count );
            foreach ( var item in list )
            {
                item.Write( this );
            }
        }
    }
}