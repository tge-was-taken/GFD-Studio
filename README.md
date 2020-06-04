# GFD Studio
**GFD Studio** is a tool for viewing, editing and converting models in **GMD**/**GFS** format.
## Features
- View a rendered preview of the opened model
- View, export, replace and add **Textures** (automatic conversion to and from PNG/DDS)
- Export, replace and edit **Materials** and their maps & properties
- Export and import models using assimp (automatic conversion to and from DAE/FBX)
## Requirements
- A videocard that supports at least OpenGL 3.3 to use the model viewer.
(This is required for compiling shaders)
## Usage
### Model Conversion
For best results, use the [GMD Maxscript](https://github.com/TGEnigma/GFD-Studio/blob/master/Resources/GfdImporter/GfdImporter.ms) to import models directly into 3ds Max.
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

**Warning:** Work In Progress, keep saves of your files.

GFD Studio can now convert P5R animations to P5.
There are still issues with bones (weird movement, rotation, etc), you can fix them by following this process:

1. Load animation file in GFD Studio
2. Export it as .GAP
3. Load the GAP in 3dsmax using maxscript (can be found here https://amicitia.github.io/tools/p5)
4. Export the animation as FBX
5. Replace the animation in GFD Studio
6. Once done with all animations you want export again as .GAP
