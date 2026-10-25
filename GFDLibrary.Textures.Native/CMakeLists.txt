# 3.24 for FetchContent's OVERRIDE_FIND_PACKAGE, which lets DirectXTex's find_package
# calls resolve to the fetched sources.
cmake_minimum_required(VERSION 3.24)

project(DirectXTexC VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# The fetched projects are consumed in tree, so their install and export rules are skipped;
# only the shared library this project builds is ever deployed.
set(CMAKE_SKIP_INSTALL_RULES ON)

include(FetchContent)

# Pinned upstream releases. Fetching them keeps the build to CMake and a compiler, with no
# package manager for contributors to install.
set(DIRECTXMATH_TAG      jun2026     CACHE STRING "DirectXMath release tag")
set(DIRECTXHEADERS_TAG   v1.619.5    CACHE STRING "DirectX-Headers release tag")
set(DIRECTXTEX_TAG       may2026     CACHE STRING "DirectXTex release tag")
set(ZLIB_TAG             v1.3.1      CACHE STRING "zlib release tag")
set(LIBPNG_TAG           v1.6.50     CACHE STRING "libpng release tag")

if(NOT WIN32)
    # DirectXMath includes sal.h, which only ships with the Windows SDK. This is the same MIT
    # licensed copy that vcpkg's directxmath port installs on other platforms.
    FetchContent_Declare(sal_header
        URL https://raw.githubusercontent.com/dotnet/runtime/v9.0.2/src/coreclr/pal/inc/rt/sal.h
        URL_HASH SHA512=8085f67bfa4ce01ae89461cadf72454a9552fde3f08b2dcc3de36b9830e29ce7a6192800f8a5cb2a66af9637be0017e85719826a4cfdade508ae97f319e0ee8e
        DOWNLOAD_NO_EXTRACT TRUE
    )
    FetchContent_MakeAvailable(sal_header)
    set(GFDTEX_SAL_DIR ${sal_header_SOURCE_DIR})

    FetchContent_Declare(directx-headers
        GIT_REPOSITORY https://github.com/microsoft/DirectX-Headers.git
        GIT_TAG ${DIRECTXHEADERS_TAG}
        GIT_SHALLOW TRUE
        OVERRIDE_FIND_PACKAGE
    )
    set(DXHEADERS_INSTALL OFF CACHE BOOL "" FORCE)
    set(DXHEADERS_BUILD_TEST OFF CACHE BOOL "" FORCE)
    set(DXHEADERS_BUILD_GOOGLE_TEST OFF CACHE BOOL "" FORCE)
    FetchContent_MakeAvailable(directx-headers)
endif()

FetchContent_Declare(directxmath
    GIT_REPOSITORY https://github.com/microsoft/DirectXMath.git
    GIT_TAG ${DIRECTXMATH_TAG}
    GIT_SHALLOW TRUE
    OVERRIDE_FIND_PACKAGE
)
FetchContent_MakeAvailable(directxmath)

# DirectXTex links the namespaced name, which only exists in DirectXMath's installed export.
if(NOT TARGET Microsoft::DirectXMath)
    add_library(Microsoft::DirectXMath ALIAS DirectXMath)
endif()

# PNG is the interchange format the tool imports and exports, so libpng comes along. DirectXTex
# reaches it through its Auxiliary PNG codec, which works the same on every platform.
FetchContent_Declare(ZLIB
    GIT_REPOSITORY https://github.com/madler/zlib.git
    GIT_TAG ${ZLIB_TAG}
    GIT_SHALLOW TRUE
    OVERRIDE_FIND_PACKAGE
)
set(ZLIB_BUILD_EXAMPLES OFF CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(ZLIB)

if(NOT TARGET ZLIB::ZLIB)
    add_library(ZLIB::ZLIB ALIAS zlibstatic)
endif()
set(ZLIB_LIBRARY zlibstatic CACHE STRING "" FORCE)
set(ZLIB_INCLUDE_DIR "${zlib_SOURCE_DIR};${zlib_BINARY_DIR}" CACHE STRING "" FORCE)

FetchContent_Declare(PNG
    GIT_REPOSITORY https://github.com/pnggroup/libpng.git
    GIT_TAG ${LIBPNG_TAG}
    GIT_SHALLOW TRUE
    OVERRIDE_FIND_PACKAGE
)
set(PNG_SHARED OFF CACHE BOOL "" FORCE)
set(PNG_TESTS OFF CACHE BOOL "" FORCE)
set(PNG_TOOLS OFF CACHE BOOL "" FORCE)
set(PNG_FRAMEWORK OFF CACHE BOOL "" FORCE)
set(SKIP_INSTALL_ALL ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(PNG)

if(NOT TARGET PNG::PNG)
    add_library(PNG::PNG ALIAS png_static)
endif()

set(ENABLE_LIBPNG_SUPPORT ON CACHE BOOL "" FORCE)

# GFD Studio only needs the DDS, TGA, PNG and block codecs, so the Direct3D paths and the
# command line tools stay out of the build.
set(BUILD_TOOLS OFF CACHE BOOL "" FORCE)
set(BUILD_SAMPLE OFF CACHE BOOL "" FORCE)
set(BUILD_DX11 OFF CACHE BOOL "" FORCE)
set(BUILD_DX12 OFF CACHE BOOL "" FORCE)
set(BUILD_SHARED_LIBS OFF CACHE BOOL "" FORCE)

FetchContent_Declare(directxtex
    GIT_REPOSITORY https://github.com/microsoft/DirectXTex.git
    GIT_TAG ${DIRECTXTEX_TAG}
    GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(directxtex)

if(NOT WIN32)
    target_include_directories(DirectXTex PUBLIC ${GFDTEX_SAL_DIR})
endif()

add_library(DirectXTexC SHARED src/DirectXTexC.cpp)

target_include_directories(DirectXTexC PUBLIC include)
# The PNG codec lives in DirectXTex's Auxiliary directory, which it does not export.
target_include_directories(DirectXTexC PRIVATE ${directxtex_SOURCE_DIR}/Auxiliary)
target_link_libraries(DirectXTexC PRIVATE DirectXTex)

# DllImport looks for the same file name on every platform.
set_target_properties(DirectXTexC PROPERTIES
    OUTPUT_NAME DirectXTexC
    PREFIX ""
    CXX_VISIBILITY_PRESET hidden
)
