# GFD Studio [![Build status](https://github.com/tge-was-taken/GFD-Studio/actions/workflows/ci.yml/badge.svg)](https://github.com/tge-was-taken/GFD-Studio/actions/workflows/ci.yml)
**GFD Studio** is a tool for viewing, editing and converting models in **GMD**/**GFS** format.  
## Latest builds
* [Github release](https://github.com/tge-was-taken/GFD-Studio/releases), published for every master build
* [CI artifacts](https://github.com/tge-was-taken/GFD-Studio/actions/workflows/ci.yml) for branch builds, kept for 90 days

Each release has two archives: `GFDStudio-win-x64.zip` is self contained and needs nothing
installed, `GFDStudio-dotnet.zip` is smaller but needs the .NET 10 desktop runtime.

## Features
- View a rendered preview of the opened model
- View, export, replace and add **Textures** (automatic conversion to and from PNG/DDS)
- Export, replace and edit **Materials** and their maps & properties
- Export and import models using assimp (automatic conversion to and from DAE/FBX)
## Requirements
- .NET 10 desktop runtime installed, unless you use the self contained build
- A videocard that supports at least OpenGL 3.3 to use the model viewer.
(This is required for compiling shaders)
## Building

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) and CMake 3.24 or newer.

- Clone with `git clone https://github.com/tge-was-taken/GFD-Studio`
- Open the solution in Visual Studio, or `dotnet build GFDStudio.sln -c Release`

### Native libraries

Texture and FBX handling run through two small C libraries, built with CMake and loaded by
`GFDLibrary` at runtime. The managed build does not depend on either, so it succeeds without them;
the feature each one backs fails at runtime instead.

`GFDLibrary.Native.DirectXTex` fetches its own dependencies, so it needs nothing installed:

```
cmake -S src/native/GFDLibrary.Native.DirectXTex -B src/native/GFDLibrary.Native.DirectXTex/out -DCMAKE_BUILD_TYPE=Release
cmake --build src/native/GFDLibrary.Native.DirectXTex/out --config Release
```

`GFDLibrary.Native.FbxSdk` needs the Autodesk FBX SDK 2020.3.7, which is a licensed download.
Install it to the standard path for your platform or pass `-DFbxSdkRoot=<path>`:

```
cmake -S src/native/GFDLibrary.Native.FbxSdk -B src/native/GFDLibrary.Native.FbxSdk/out -DCMAKE_BUILD_TYPE=Release
cmake --build src/native/GFDLibrary.Native.FbxSdk/out --config Release
```

A build in each project's `out` directory is copied next to the application automatically. Without
`FbxSdkC`, export a `.dae` to go through assimp instead.

### Platforms

Everything builds and runs on Windows, Linux and macOS. The GUI is Avalonia and the model viewer
draws with OpenTK on the context Avalonia provides.

```
dotnet build GFDStudio.sln -c Release
```

### Publishing

Every push to master publishes a release tagged `v0.10.<patch>-<commit>`, numbering the patch from
the tags already published, and pushing a `v*` tag publishes one under that tag. Archives are built
for `win-x64`, `linux-x64` and `osx-arm64`.

```
dotnet publish src/GFDStudio.Avalonia/GFDStudio.Avalonia.csproj -c Release -r win-x64 --self-contained -o out
```
## Usage
### Model Conversion
For best results, use the [GMD Maxscript](https://github.com/tge-was-taken/GFD-Studio/blob/master/Resources/GfdImporter/GfdImporter.ms) to import models directly into 3ds Max.
Alternatively, you can use GFD studio to export as DAE, which you can import into your program of choice.
1. Skin your new model to the existing bones and export as an **ASCII 2011 FBX**.
2. In GFD Studio, navigate to **New > Model** and select your FBX.
3. Choose a material preset and change the version if needed. (Hover over the options for more info)
### Replacing Materials and Textures
By default, after importing a new model from FBX, all materials will have the same properties.
You can edit these properties manually, or export them from another model and reuse them.
1. Right click a material and choose Replace.
2. Select a gmt file to replace it with.
3. **Be sure to change the material's name back** to what it was before replacing. It has to match the material's name from the FBX.
4. Also be sure to update the bitmap names for the newly replaced material. **They need to match a texture that's part of the model.**
5. You can right click the Textures or Materials to export or replace them all at once as one file, or add individual textures or materials that are missing.
5. Click the filename at the top of the list to refresh the preview. If a material name is wrong or references a texture that can't be found, parts of the model will be shaded black.

### P5R Animation backport

GFD Studio can now convert P5R animations to P5. There are two ways to do it:

On a single file
1. Load a P5R .GAP file in GFD Studio.
2. Right-click on the animation pack then select **Tools -> Convert to P5**.
3. Export.

On a folder
1. In GFD Studio navigate to **Tools -> Convert P5R animations to P5 in directory**.
2. Select a directory that contains P5R animations.
3. Beware, all the files will be overwritten so make backups.
