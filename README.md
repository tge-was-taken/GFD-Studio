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

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) and, for the FBX exporter, the
Autodesk FBX SDK 2020.3.7 and the MSVC C++ toolset.

- Install FBX SDK 2020.3.7 to the standard path (C:\Program Files\Autodesk\FBX\FBX SDK\2020.3.7),
  or point `FbxSdkRoot` at wherever you installed it
- Clone with `git clone https://github.com/tge-was-taken/GFD-Studio`
- Navigate to the repo, and clone submodules with `git submodule update --init --recursive`
- Open the solution in Visual Studio, or `dotnet build GFDStudio.sln -c Release`

Without the FBX SDK installed, build the `GFDStudio.NoFbx.slnf` filter instead, which leaves the
exporter out:

```
dotnet build GFDStudio.NoFbx.slnf -c Release -p:EnableFbxSdk=false
```

FBX export then goes through assimp instead of the FBX SDK exporter.

### Publishing

Every push to master publishes a release tagged `v0.10.<patch>-<commit>`, numbering the patch from
the tags already published, and pushing a `v*` tag publishes one under that tag.

```
dotnet publish GFDStudio/GFDStudio.csproj -c Release -r win-x64 --self-contained -o out
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
