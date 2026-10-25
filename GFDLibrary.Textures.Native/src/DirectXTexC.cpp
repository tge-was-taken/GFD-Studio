#include "DirectXTexC.h"

#include <cstring>
#include <memory>
#include <string>
#include <new>

#include <DirectXTex.h>
#include <DirectXTexPNG.h>

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

namespace
{
    /// DirectXTex takes wide paths; the managed side hands over UTF-8.
    std::wstring ToWide( const char* path )
    {
        std::wstring wide;
        if ( !path )
            return wide;

        // The paths here come from the host filesystem, so a plain widening of the UTF-8 code
        // units is not enough; decode the sequences properly.
        for ( const unsigned char* p = reinterpret_cast<const unsigned char*>( path ); *p; )
        {
            char32_t cp;
            if ( *p < 0x80 ) { cp = *p++; }
            else if ( ( *p & 0xE0 ) == 0xC0 ) { cp = ( *p & 0x1F ); cp = ( cp << 6 ) | ( p[1] & 0x3F ); p += 2; }
            else if ( ( *p & 0xF0 ) == 0xE0 ) { cp = ( *p & 0x0F ); cp = ( cp << 6 ) | ( p[1] & 0x3F ); cp = ( cp << 6 ) | ( p[2] & 0x3F ); p += 3; }
            else { cp = ( *p & 0x07 ); cp = ( cp << 6 ) | ( p[1] & 0x3F ); cp = ( cp << 6 ) | ( p[2] & 0x3F ); cp = ( cp << 6 ) | ( p[3] & 0x3F ); p += 4; }

            if ( sizeof( wchar_t ) == 2 && cp > 0xFFFF )
            {
                cp -= 0x10000;
                wide.push_back( static_cast<wchar_t>( 0xD800 + ( cp >> 10 ) ) );
                wide.push_back( static_cast<wchar_t>( 0xDC00 + ( cp & 0x3FF ) ) );
            }
            else
            {
                wide.push_back( static_cast<wchar_t>( cp ) );
            }
        }

        return wide;
    }
}

HRESULT DirectXTex_LoadFromPNGFile( const char* path, DirectXTex_ScratchImage** image )
{
    if ( !path || !image )
        return E_INVALIDARG;

    ScratchImage loaded;
    HRESULT hr = LoadFromPNGFile( ToWide( path ).c_str(), PNG_FLAGS_NONE, nullptr, loaded );
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

    return SaveToPNGFile( *top, PNG_FLAGS_NONE, ToWide( path ).c_str() );
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
