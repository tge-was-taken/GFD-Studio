#include "DirectXTexC.h"

#include <cstring>
#include <memory>
#include <new>

#include <DirectXTex.h>

#include <png.h>
#include <cstdio>

using namespace DirectX;

namespace
{
    // Opaque to the caller; these just own the C++ objects.
    struct ScratchImageHandle { ScratchImage value; };
    struct BlobHandle { Blob value; };

    const ScratchImage* Unwrap( const DirectXTex_ScratchImage* image )
    {
        return image ? &reinterpret_cast<const ScratchImageHandle*>( image )->value : nullptr;
    }

    /// Moves a finished ScratchImage into a handle for the caller to release.
    HRESULT Wrap( ScratchImage&& value, DirectXTex_ScratchImage** out )
    {
        auto handle = new ( std::nothrow ) ScratchImageHandle();
        if ( !handle )
            return E_OUTOFMEMORY;

        handle->value = std::move( value );
        *out = reinterpret_cast<DirectXTex_ScratchImage*>( handle );
        return S_OK;
    }
}

HRESULT DirectXTex_LoadFromDDSMemory( const void* source, size_t size, DirectXTex_ScratchImage** image )
{
    if ( !source || !size || !image )
        return E_INVALIDARG;

    ScratchImage loaded;
    HRESULT hr = LoadFromDDSMemory( static_cast<const uint8_t*>( source ), size, DDS_FLAGS_NONE, nullptr, loaded );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( loaded ), image );
}

HRESULT DirectXTex_LoadFromTGAMemory( const void* source, size_t size, DirectXTex_ScratchImage** image )
{
    if ( !source || !size || !image )
        return E_INVALIDARG;

    ScratchImage loaded;
    HRESULT hr = LoadFromTGAMemory( static_cast<const uint8_t*>( source ), size, TGA_FLAGS_NONE, nullptr, loaded );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( loaded ), image );
}

HRESULT DirectXTex_LoadFromBlocks( const void* source, size_t size, size_t width, size_t height,
                                   int format, DirectXTex_ScratchImage** image )
{
    if ( !source || !size || !width || !height || !image )
        return E_INVALIDARG;

    const auto dxgiFormat = static_cast<DXGI_FORMAT>( format );

    size_t rowPitch, slicePitch;
    HRESULT hr = ComputePitch( dxgiFormat, width, height, rowPitch, slicePitch );
    if ( FAILED( hr ) )
        return hr;

    if ( size < slicePitch )
        return E_INVALIDARG;

    Image source_image = {};
    source_image.width = width;
    source_image.height = height;
    source_image.format = dxgiFormat;
    source_image.rowPitch = rowPitch;
    source_image.slicePitch = slicePitch;
    source_image.pixels = const_cast<uint8_t*>( static_cast<const uint8_t*>( source ) );

    ScratchImage loaded;
    hr = loaded.InitializeFromImage( source_image );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( loaded ), image );
}

// PNG goes straight to libpng rather than through DirectXTex's Auxiliary codec, which hardcodes
// compression level 0 and runs png_set_alpha_mode on read, destroying the colour of fully
// transparent pixels.
//
// Nothing with a destructor may be live across the setjmp error paths below, so the buffers here
// are raw allocations freed by hand.

HRESULT DirectXTex_LoadFromPNGFile( const char* path, DirectXTex_ScratchImage** image )
{
    if ( !path || !image )
        return E_INVALIDARG;

    FILE* file = fopen( path, "rb" );
    if ( !file )
        return E_FAIL;

    png_structp png = png_create_read_struct( PNG_LIBPNG_VER_STRING, nullptr, nullptr, nullptr );
    if ( !png )
    {
        fclose( file );
        return E_OUTOFMEMORY;
    }

    png_infop info = png_create_info_struct( png );
    if ( !info )
    {
        png_destroy_read_struct( &png, nullptr, nullptr );
        fclose( file );
        return E_OUTOFMEMORY;
    }

    png_bytep pixels = nullptr;
    png_bytepp rows = nullptr;
    png_uint_32 width = 0, height = 0;

    if ( setjmp( png_jmpbuf( png ) ) )
    {
        free( rows );
        free( pixels );
        png_destroy_read_struct( &png, &info, nullptr );
        fclose( file );
        return E_FAIL;
    }

    png_init_io( png, file );
    png_read_info( png, info );

    width = png_get_image_width( png, info );
    height = png_get_image_height( png, info );
    const png_byte colorType = png_get_color_type( png, info );
    const png_byte bitDepth = png_get_bit_depth( png, info );

    // Normalise everything to 8 bit RGBA.
    if ( colorType == PNG_COLOR_TYPE_PALETTE )
        png_set_palette_to_rgb( png );
    if ( colorType == PNG_COLOR_TYPE_GRAY && bitDepth < 8 )
        png_set_expand_gray_1_2_4_to_8( png );
    if ( png_get_valid( png, info, PNG_INFO_tRNS ) )
        png_set_tRNS_to_alpha( png );
    if ( bitDepth == 16 )
        png_set_scale_16( png );
    if ( colorType == PNG_COLOR_TYPE_GRAY || colorType == PNG_COLOR_TYPE_GRAY_ALPHA )
        png_set_gray_to_rgb( png );

    png_set_filler( png, 0xFF, PNG_FILLER_AFTER );
    png_set_interlace_handling( png );
    png_read_update_info( png, info );

    const size_t rowBytes = png_get_rowbytes( png, info );

    pixels = static_cast<png_bytep>( malloc( rowBytes * height ) );
    rows = static_cast<png_bytepp>( malloc( sizeof( png_bytep ) * height ) );
    if ( !pixels || !rows )
        png_error( png, "out of memory" );

    for ( png_uint_32 y = 0; y < height; ++y )
        rows[y] = pixels + ( rowBytes * y );

    png_read_image( png, rows );
    png_read_end( png, nullptr );

    free( rows );
    rows = nullptr;
    png_destroy_read_struct( &png, &info, nullptr );
    fclose( file );

    ScratchImage loaded;
    HRESULT hr = loaded.Initialize2D( DXGI_FORMAT_R8G8B8A8_UNORM, width, height, 1, 1 );
    if ( SUCCEEDED( hr ) )
    {
        const Image* target = loaded.GetImage( 0, 0, 0 );
        for ( png_uint_32 y = 0; y < height; ++y )
            memcpy( target->pixels + ( target->rowPitch * y ), pixels + ( rowBytes * y ), width * 4 );
    }

    free( pixels );

    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( loaded ), image );
}

HRESULT DirectXTex_SaveToPNGFile( const DirectXTex_ScratchImage* image, const char* path )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !path )
        return E_INVALIDARG;

    const Image* top = source->GetImage( 0, 0, 0 );
    if ( !top )
        return E_FAIL;

    if ( top->format != DXGI_FORMAT_R8G8B8A8_UNORM && top->format != DXGI_FORMAT_R8G8B8A8_UNORM_SRGB )
        return E_INVALIDARG;

    FILE* file = fopen( path, "wb" );
    if ( !file )
        return E_FAIL;

    png_structp png = png_create_write_struct( PNG_LIBPNG_VER_STRING, nullptr, nullptr, nullptr );
    if ( !png )
    {
        fclose( file );
        return E_OUTOFMEMORY;
    }

    png_infop info = png_create_info_struct( png );
    if ( !info )
    {
        png_destroy_write_struct( &png, nullptr );
        fclose( file );
        return E_OUTOFMEMORY;
    }

    png_bytepp rows = nullptr;

    if ( setjmp( png_jmpbuf( png ) ) )
    {
        free( rows );
        png_destroy_write_struct( &png, &info );
        fclose( file );
        return E_FAIL;
    }

    png_init_io( png, file );
    png_set_IHDR( png, info, static_cast<png_uint_32>( top->width ), static_cast<png_uint_32>( top->height ),
                  8, PNG_COLOR_TYPE_RGBA, PNG_INTERLACE_NONE,
                  PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT );

    rows = static_cast<png_bytepp>( malloc( sizeof( png_bytep ) * top->height ) );
    if ( !rows )
        png_error( png, "out of memory" );

    for ( size_t y = 0; y < top->height; ++y )
        rows[y] = top->pixels + ( top->rowPitch * y );

    png_set_rows( png, info, rows );
    png_write_png( png, info, PNG_TRANSFORM_IDENTITY, nullptr );

    free( rows );
    png_destroy_write_struct( &png, &info );
    fclose( file );

    return S_OK;
}

HRESULT DirectXTex_Initialize2D( const void* pixels, size_t width, size_t height, int format,
                                 DirectXTex_ScratchImage** image )
{
    if ( !pixels || !width || !height || !image )
        return E_INVALIDARG;

    const auto dxgiFormat = static_cast<DXGI_FORMAT>( format );
    if ( IsCompressed( dxgiFormat ) )
        return E_INVALIDARG;

    ScratchImage created;
    HRESULT hr = created.Initialize2D( dxgiFormat, width, height, 1, 1 );
    if ( FAILED( hr ) )
        return hr;

    const Image* target = created.GetImage( 0, 0, 0 );
    const size_t rowBytes = ( width * BitsPerPixel( dxgiFormat ) + 7 ) / 8;
    const auto source = static_cast<const uint8_t*>( pixels );

    // The caller's rows are tightly packed; the destination's may not be.
    for ( size_t y = 0; y < height; ++y )
        memcpy( target->pixels + ( target->rowPitch * y ), source + ( rowBytes * y ), rowBytes );

    return Wrap( std::move( created ), image );
}

HRESULT DirectXTex_Decompress( const DirectXTex_ScratchImage* image, int format,
                               DirectXTex_ScratchImage** out )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !out )
        return E_INVALIDARG;

    ScratchImage result;
    HRESULT hr = Decompress( source->GetImages(), source->GetImageCount(), source->GetMetadata(),
                             static_cast<DXGI_FORMAT>( format ), result );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( result ), out );
}

HRESULT DirectXTex_Convert( const DirectXTex_ScratchImage* image, int format,
                            DirectXTex_ScratchImage** out )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !out )
        return E_INVALIDARG;

    ScratchImage result;
    HRESULT hr = Convert( source->GetImages(), source->GetImageCount(), source->GetMetadata(),
                          static_cast<DXGI_FORMAT>( format ), TEX_FILTER_DEFAULT,
                          TEX_THRESHOLD_DEFAULT, result );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( result ), out );
}

HRESULT DirectXTex_Compress( const DirectXTex_ScratchImage* image, int format,
                             DirectXTex_ScratchImage** out )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !out )
        return E_INVALIDARG;

    ScratchImage result;
    HRESULT hr = Compress( source->GetImages(), source->GetImageCount(), source->GetMetadata(),
                           static_cast<DXGI_FORMAT>( format ), TEX_COMPRESS_PARALLEL,
                           TEX_THRESHOLD_DEFAULT, result );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( result ), out );
}

HRESULT DirectXTex_GenerateMipMaps( const DirectXTex_ScratchImage* image, size_t levels,
                                    DirectXTex_ScratchImage** out )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !out )
        return E_INVALIDARG;

    ScratchImage result;
    HRESULT hr = GenerateMipMaps( *source->GetImage( 0, 0, 0 ), TEX_FILTER_DEFAULT, levels, result );
    if ( FAILED( hr ) )
        return hr;

    return Wrap( std::move( result ), out );
}

HRESULT DirectXTex_SaveToDDSMemory( const DirectXTex_ScratchImage* image, DirectXTex_Blob** blob )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !blob )
        return E_INVALIDARG;

    auto handle = new ( std::nothrow ) BlobHandle();
    if ( !handle )
        return E_OUTOFMEMORY;

    HRESULT hr = SaveToDDSMemory( source->GetImages(), source->GetImageCount(),
                                  source->GetMetadata(), DDS_FLAGS_NONE, handle->value );
    if ( FAILED( hr ) )
    {
        delete handle;
        return hr;
    }

    *blob = reinterpret_cast<DirectXTex_Blob*>( handle );
    return S_OK;
}

HRESULT DirectXTex_OverrideFormat( DirectXTex_ScratchImage* image, int format )
{
    if ( !image )
        return E_INVALIDARG;

    auto self = reinterpret_cast<ScratchImageHandle*>( image );
    return self->value.OverrideFormat( static_cast<DXGI_FORMAT>( format ) ) ? S_OK : E_FAIL;
}

HRESULT DirectXTex_GetMetadata( const DirectXTex_ScratchImage* image, DirectXTex_TexMetadata* metadata )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !metadata )
        return E_INVALIDARG;

    const auto& value = source->GetMetadata();
    metadata->width = value.width;
    metadata->height = value.height;
    metadata->mipLevels = value.mipLevels;
    metadata->format = static_cast<int>( value.format );
    return S_OK;
}

HRESULT DirectXTex_GetPixels( const DirectXTex_ScratchImage* image, const void** pixels,
                              size_t* size, size_t* rowPitch )
{
    const ScratchImage* source = Unwrap( image );
    if ( !source || !pixels || !size || !rowPitch )
        return E_INVALIDARG;

    const Image* top = source->GetImage( 0, 0, 0 );
    if ( !top )
        return E_FAIL;

    *pixels = top->pixels;
    *size = top->slicePitch;
    *rowPitch = top->rowPitch;
    return S_OK;
}

const void* DirectXTex_Blob_GetBufferPointer( const DirectXTex_Blob* blob )
{
    return blob ? reinterpret_cast<const BlobHandle*>( blob )->value.GetConstBufferPointer() : nullptr;
}

size_t DirectXTex_Blob_GetBufferSize( const DirectXTex_Blob* blob )
{
    return blob ? reinterpret_cast<const BlobHandle*>( blob )->value.GetBufferSize() : 0;
}

void DirectXTex_ScratchImage_Release( DirectXTex_ScratchImage* image )
{
    delete reinterpret_cast<ScratchImageHandle*>( image );
}

void DirectXTex_Blob_Release( DirectXTex_Blob* blob )
{
    delete reinterpret_cast<BlobHandle*>( blob );
}
