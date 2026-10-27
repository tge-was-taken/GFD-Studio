# Scarlet (vendored)

The console texture container readers GFD Studio needs, taken from Scarlet and reduced to the
minimum. Scarlet is unmaintained, so the code lives here rather than as a submodule.

- Upstream: https://github.com/xdanieldzd/Scarlet
- Fork it came from: https://github.com/tge-was-taken/Scarlet
- Commit: 5532fd36835e017bc45e602aacfbb0e97d075c98
- License: MIT, see LICENSE.md in this directory

## What is here

`TMX` (PlayStation 2), `GXT` (PlayStation Vita) and `GNF` (PlayStation 4), with the pixel
format conversion, swizzling and compression code they depend on. The other 40 or so format
readers were left behind.

## Changes from upstream

- Added `ImagePixels` and `GetPixels`, returning decoded pixels directly.
- Removed every `Bitmap` returning member. They needed System.Drawing, which is Windows only,
  and `GetPixels` covers the same ground. `GXT.GetBUVBitmaps` went with them, unused here.
- Removed the unused `using` directives those changes left behind.

Block compressed formats are decoded by DirectXTex rather than by the code here; what remains
of Scarlet's compression support is for the formats DirectXTex does not cover, such as PVRTC.
