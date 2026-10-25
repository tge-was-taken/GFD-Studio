using System;
using System.Runtime.InteropServices;
using GFDLibrary.Textures.DDS;

namespace GFDLibrary.Textures.Native
{
    /// <summary>
    /// The DirectXTex codecs, reached through the DirectXTexC shared library. Mirrors the C++ API
    /// in the DirectX namespace; see GFDLibrary.Conversion.DirectXTex in the repository root for the
    /// native side and how to build it.
    /// </summary>
    internal static class DirectXTex
    {
        private const string Library = "DirectXTexC";

        [StructLayout( LayoutKind.Sequential )]
        internal struct TexMetadata
        {
            public nuint Width;
            public nuint Height;
            public nuint MipLevels;
            public int Format;
        }

        [DllImport( Library, EntryPoint = "DirectXTex_LoadFromDDSMemory" )]
        private static extern int LoadFromDDSMemory( IntPtr source, nuint size, out IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_LoadFromTGAMemory" )]
        private static extern int LoadFromTGAMemory( IntPtr source, nuint size, out IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_LoadFromBlocks" )]
        private static extern int LoadFromBlocks( IntPtr source, nuint size, nuint width, nuint height,
                                                  int format, out IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_LoadFromPNGFile" )]
        private static extern int LoadFromPNGFile( [MarshalAs( UnmanagedType.LPUTF8Str )] string path, out IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_SaveToPNGFile" )]
        private static extern int SaveToPNGFile( IntPtr image, [MarshalAs( UnmanagedType.LPUTF8Str )] string path );

        [DllImport( Library, EntryPoint = "DirectXTex_Initialize2D" )]
        private static extern int Initialize2D( IntPtr pixels, nuint width, nuint height, int format,
                                                out IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_Decompress" )]
        private static extern int Decompress( IntPtr image, int format, out IntPtr result );

        [DllImport( Library, EntryPoint = "DirectXTex_Convert" )]
        private static extern int Convert( IntPtr image, int format, out IntPtr result );

        [DllImport( Library, EntryPoint = "DirectXTex_Compress" )]
        private static extern int Compress( IntPtr image, int format, out IntPtr result );

        [DllImport( Library, EntryPoint = "DirectXTex_GenerateMipMaps" )]
        private static extern int GenerateMipMaps( IntPtr image, nuint levels, out IntPtr result );

        [DllImport( Library, EntryPoint = "DirectXTex_SaveToDDSMemory" )]
        private static extern int SaveToDDSMemory( IntPtr image, out IntPtr blob );

        [DllImport( Library, EntryPoint = "DirectXTex_OverrideFormat" )]
        private static extern int OverrideFormat( IntPtr image, int format );

        [DllImport( Library, EntryPoint = "DirectXTex_GetMetadata" )]
        private static extern int GetMetadata( IntPtr image, out TexMetadata metadata );

        [DllImport( Library, EntryPoint = "DirectXTex_GetPixels" )]
        private static extern int GetPixels( IntPtr image, out IntPtr pixels, out nuint size, out nuint rowPitch );

        [DllImport( Library, EntryPoint = "DirectXTex_Blob_GetBufferPointer" )]
        private static extern IntPtr BlobGetBufferPointer( IntPtr blob );

        [DllImport( Library, EntryPoint = "DirectXTex_Blob_GetBufferSize" )]
        private static extern nuint BlobGetBufferSize( IntPtr blob );

        [DllImport( Library, EntryPoint = "DirectXTex_ScratchImage_Release" )]
        private static extern void ScratchImageRelease( IntPtr image );

        [DllImport( Library, EntryPoint = "DirectXTex_Blob_Release" )]
        private static extern void BlobRelease( IntPtr blob );

        /// <summary>A DirectXTex ScratchImage. Dispose releases the native allocation.</summary>
        internal sealed class ScratchImage : IDisposable
        {
            private IntPtr mHandle;

            private ScratchImage( IntPtr handle ) => mHandle = handle;

            public static unsafe ScratchImage FromDDS( ReadOnlySpan<byte> data )
            {
                fixed ( byte* p = Require( data ) )
                {
                    Check( LoadFromDDSMemory( ( IntPtr )p, ( nuint )data.Length, out var handle ) );
                    return new ScratchImage( handle );
                }
            }

            public static unsafe ScratchImage FromTGA( ReadOnlySpan<byte> data )
            {
                fixed ( byte* p = Require( data ) )
                {
                    Check( LoadFromTGAMemory( ( IntPtr )p, ( nuint )data.Length, out var handle ) );
                    return new ScratchImage( handle );
                }
            }

            public static unsafe ScratchImage FromBlocks( ReadOnlySpan<byte> data, int width, int height, DDSDxgiFormat format )
            {
                fixed ( byte* p = Require( data ) )
                {
                    Check( LoadFromBlocks( ( IntPtr )p, ( nuint )data.Length, ( nuint )width, ( nuint )height,
                                           ( int )format, out var handle ) );
                    return new ScratchImage( handle );
                }
            }

            /// <summary>Wraps tightly packed pixels of the given uncompressed format.</summary>
            public static unsafe ScratchImage FromPixels( ReadOnlySpan<byte> pixels, int width, int height, DDSDxgiFormat format )
            {
                fixed ( byte* p = Require( pixels ) )
                {
                    Check( Initialize2D( ( IntPtr )p, ( nuint )width, ( nuint )height, ( int )format, out var handle ) );
                    return new ScratchImage( handle );
                }
            }

            public static ScratchImage FromPNGFile( string path )
            {
                Check( LoadFromPNGFile( path, out var handle ) );
                return new ScratchImage( handle );
            }

            public void SaveToPNGFile( string path )
            {
                Check( DirectXTex.SaveToPNGFile( Handle, path ) );
            }

            private static ReadOnlySpan<byte> Require( ReadOnlySpan<byte> data )
                => !data.IsEmpty ? data : throw new ArgumentException( "No data", nameof( data ) );

            public TexMetadata Metadata
            {
                get
                {
                    Check( GetMetadata( Handle, out var metadata ) );
                    return metadata;
                }
            }

            public int Width => ( int )Metadata.Width;

            public int Height => ( int )Metadata.Height;

            public ScratchImage Decompress( DDSDxgiFormat format )
            {
                Check( DirectXTex.Decompress( Handle, ( int )format, out var result ) );
                return new ScratchImage( result );
            }

            /// <summary>Reinterprets the pixels as another format of the same size.</summary>
            public void OverrideFormat( DDSDxgiFormat format )
            {
                Check( DirectXTex.OverrideFormat( Handle, ( int )format ) );
            }

            public ScratchImage Convert( DDSDxgiFormat format )
            {
                Check( DirectXTex.Convert( Handle, ( int )format, out var result ) );
                return new ScratchImage( result );
            }

            public ScratchImage Compress( DDSDxgiFormat format )
            {
                Check( DirectXTex.Compress( Handle, ( int )format, out var result ) );
                return new ScratchImage( result );
            }

            /// <summary>Generates the full chain; returns null when the size cannot be mipped.</summary>
            public ScratchImage GenerateMipMaps()
            {
                var hr = DirectXTex.GenerateMipMaps( Handle, 0, out var result );
                return hr < 0 ? null : new ScratchImage( result );
            }

            public byte[] SaveToDDS()
            {
                Check( SaveToDDSMemory( Handle, out var blob ) );

                try
                {
                    var size = ( int )BlobGetBufferSize( blob );
                    var bytes = new byte[size];
                    Marshal.Copy( BlobGetBufferPointer( blob ), bytes, 0, size );
                    return bytes;
                }
                finally
                {
                    BlobRelease( blob );
                }
            }

            /// <summary>Top level RGBA32 pixels, repacked without the native row padding.</summary>
            public byte[] GetPixels()
            {
                Check( DirectXTex.GetPixels( Handle, out var pixels, out _, out var rowPitch ) );

                var metadata = Metadata;
                var height = ( int )metadata.Height;
                var stride = ( int )rowPitch;
                var packed = ( int )metadata.Width * 4;

                var bytes = new byte[packed * height];
                for ( var y = 0; y < height; y++ )
                    Marshal.Copy( pixels + ( y * stride ), bytes, y * packed, packed );

                return bytes;
            }

            /// <summary>The whole top level slice, padding included, for block data.</summary>
            public byte[] GetBlocks()
            {
                Check( DirectXTex.GetPixels( Handle, out var pixels, out var size, out _ ) );

                var bytes = new byte[( int )size];
                Marshal.Copy( pixels, bytes, 0, bytes.Length );
                return bytes;
            }

            private IntPtr Handle => mHandle != IntPtr.Zero
                ? mHandle
                : throw new ObjectDisposedException( nameof( ScratchImage ) );

            public void Dispose()
            {
                if ( mHandle == IntPtr.Zero )
                    return;

                ScratchImageRelease( mHandle );
                mHandle = IntPtr.Zero;
            }
        }

        private static void Check( int hr )
        {
            if ( hr < 0 )
                throw new InvalidOperationException( $"DirectXTex call failed with 0x{hr:X8}" );
        }
    }
}
