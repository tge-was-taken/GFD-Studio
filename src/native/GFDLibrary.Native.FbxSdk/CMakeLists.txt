cmake_minimum_required(VERSION 3.21)

project(FbxSdkC VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# The Autodesk FBX SDK is a licensed download, so it cannot be fetched like the other
# dependencies. Install it and point FbxSdkRoot at it.
if(NOT DEFINED FbxSdkRoot AND DEFINED ENV{FBXSDK_ROOT})
    set(FbxSdkRoot $ENV{FBXSDK_ROOT})
endif()

if(NOT DEFINED FbxSdkRoot)
    if(WIN32)
        set(FbxSdkRoot "C:/Program Files/Autodesk/FBX/FBX SDK/2020.3.7")
    elseif(APPLE)
        set(FbxSdkRoot "/Applications/Autodesk/FBX SDK/2020.3.7")
    else()
        set(FbxSdkRoot "/usr/local/lib/fbxsdk")
    endif()
endif()

find_path(FBXSDK_INCLUDE_DIR fbxsdk.h PATHS "${FbxSdkRoot}/include" NO_DEFAULT_PATH)
set(FBXSDK_LIB_PATHS "${FbxSdkRoot}/lib/release" "${FbxSdkRoot}/lib/gcc/x64/release"
                     "${FbxSdkRoot}/lib/clang/release" "${FbxSdkRoot}/lib/vs2019/x64/release"
                     "${FbxSdkRoot}/lib/x64/release")

# The Windows SDK ships both an import library for its DLL and a static one. Taking the
# static library keeps FbxSdkC self contained, as it already is on the other platforms, and
# avoids the FBXSDK_SHARED declspec dance the import library needs for its data symbols.
if(WIN32)
    find_library(FBXSDK_LIBRARY NAMES libfbxsdk-md PATHS ${FBXSDK_LIB_PATHS} NO_DEFAULT_PATH)
    find_library(FBXSDK_XML2 NAMES libxml2-md PATHS ${FBXSDK_LIB_PATHS} NO_DEFAULT_PATH)
    find_library(FBXSDK_ZLIB NAMES zlib-md PATHS ${FBXSDK_LIB_PATHS} NO_DEFAULT_PATH)
else()
    find_library(FBXSDK_LIBRARY NAMES fbxsdk libfbxsdk PATHS ${FBXSDK_LIB_PATHS} NO_DEFAULT_PATH)
endif()

if(NOT FBXSDK_INCLUDE_DIR OR NOT FBXSDK_LIBRARY)
    message(FATAL_ERROR "FBX SDK not found under ${FbxSdkRoot}. Set -DFbxSdkRoot=<path>.")
endif()

add_library(FbxSdkC SHARED src/FbxSdkC.cpp)

target_include_directories(FbxSdkC PUBLIC include PRIVATE ${FBXSDK_INCLUDE_DIR})
target_link_libraries(FbxSdkC PRIVATE ${FBXSDK_LIBRARY})

if(WIN32)
    target_link_libraries(FbxSdkC PRIVATE ${FBXSDK_XML2} ${FBXSDK_ZLIB})
endif()

if(UNIX)
    find_package(Threads REQUIRED)
    target_link_libraries(FbxSdkC PRIVATE Threads::Threads ${CMAKE_DL_LIBS})
endif()

# DllImport looks for the same file name on every platform.
set_target_properties(FbxSdkC PROPERTIES
    OUTPUT_NAME FbxSdkC
    PREFIX ""
    CXX_VISIBILITY_PRESET hidden
)

# Multi config generators would otherwise put the binary in a per configuration subdirectory,
# leaving the managed build and CI with a different path on Windows than everywhere else.
foreach(config "" _DEBUG _RELEASE _RELWITHDEBINFO _MINSIZEREL)
    set_target_properties(FbxSdkC PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY${config} "${CMAKE_BINARY_DIR}"
        LIBRARY_OUTPUT_DIRECTORY${config} "${CMAKE_BINARY_DIR}"
    )
endforeach()
