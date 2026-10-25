// C surface over DirectXTex. Names and semantics follow the C++ API in the DirectX namespace,
// flattened for P/Invoke: ScratchImage and Blob become opaque handles the caller releases.
// Every function returns an HRESULT.

#ifndef DIRECTXTEX_C_H
#define DIRECTXTEX_C_H

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32)
  #define DIRECTXTEX_C_API __declspec(dllexport)
#else
  #define DIRECTXTEX_C_API __attribute__((visibility("default")))
  typedef int32_t HRESULT;
#endif

#ifdef __cplusplus
extern "C" {
#endif

typedef struct DirectXTex_ScratchImage DirectXTex_ScratchImage;
typedef struct DirectXTex_Blob DirectXTex_Blob;

/// Mirrors DirectX::TexMetadata, limited to the fields a 2D texture needs.
typedef struct
{
    size_t width;
    size_t height;
    size_t mipLevels;
    int    format;
} DirectXTex_TexMetadata;

DIRECTXTEX_C_API HRESULT DirectXTex_LoadFromDDSMemory( const void* source, size_t size,
                                                       DirectXTex_ScratchImage** image );

DIRECTXTEX_C_API HRESULT DirectXTex_LoadFromTGAMemory( const void* source, size_t size,
                                                       DirectXTex_ScratchImage** image );

/// Wraps raw block compressed pixels, as the console texture formats store them once
/// unswizzled, in a ScratchImage without copying any decoding work.
DIRECTXTEX_C_API HRESULT DirectXTex_LoadFromBlocks( const void* source, size_t size,
                                                    size_t width, size_t height, int format,
                                                    DirectXTex_ScratchImage** image );

DIRECTXTEX_C_API HRESULT DirectXTex_Initialize2D( const void* pixels, size_t width, size_t height,
                                                  int format, DirectXTex_ScratchImage** image );

DIRECTXTEX_C_API HRESULT DirectXTex_Decompress( const DirectXTex_ScratchImage* image, int format,
                                                DirectXTex_ScratchImage** out );

DIRECTXTEX_C_API HRESULT DirectXTex_Convert( const DirectXTex_ScratchImage* image, int format,
                                             DirectXTex_ScratchImage** out );

DIRECTXTEX_C_API HRESULT DirectXTex_Compress( const DirectXTex_ScratchImage* image, int format,
                                              DirectXTex_ScratchImage** out );

DIRECTXTEX_C_API HRESULT DirectXTex_GenerateMipMaps( const DirectXTex_ScratchImage* image,
                                                     size_t levels, DirectXTex_ScratchImage** out );

DIRECTXTEX_C_API HRESULT DirectXTex_SaveToDDSMemory( const DirectXTex_ScratchImage* image,
                                                     DirectXTex_Blob** blob );

DIRECTXTEX_C_API HRESULT DirectXTex_GetMetadata( const DirectXTex_ScratchImage* image,
                                                 DirectXTex_TexMetadata* metadata );

/// Pixels of the top level image. rowPitch may exceed width times the pixel size.
DIRECTXTEX_C_API HRESULT DirectXTex_GetPixels( const DirectXTex_ScratchImage* image,
                                               const void** pixels, size_t* size, size_t* rowPitch );

DIRECTXTEX_C_API const void* DirectXTex_Blob_GetBufferPointer( const DirectXTex_Blob* blob );
DIRECTXTEX_C_API size_t      DirectXTex_Blob_GetBufferSize( const DirectXTex_Blob* blob );

DIRECTXTEX_C_API void DirectXTex_ScratchImage_Release( DirectXTex_ScratchImage* image );
DIRECTXTEX_C_API void DirectXTex_Blob_Release( DirectXTex_Blob* blob );

#ifdef __cplusplus
}
#endif

#endif
