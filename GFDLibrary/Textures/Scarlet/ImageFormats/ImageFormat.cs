using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scarlet.Drawing;

namespace Scarlet.IO.ImageFormats
{
    public abstract class ImageFormat : FileFormat
    {
        protected ImageFormat() : base() { }

        public abstract int GetImageCount();
        public abstract int GetPaletteCount();

        public virtual string GetImageName(int imageIndex)
        {
            return null;
        }

        public ImagePixels GetPixels()
        {
            return OnGetPixels(0, 0);
        }

        public ImagePixels GetPixels(int imageIndex, int paletteIndex)
        {
            if (imageIndex < 0 || (GetImageCount() != 0 && imageIndex >= GetImageCount())) throw new IndexOutOfRangeException("Image index out of range");
            if (paletteIndex < 0 || (GetPaletteCount() != 0 && paletteIndex >= GetPaletteCount())) throw new IndexOutOfRangeException("Palette index out of range");

            return OnGetPixels(imageIndex, paletteIndex);
        }

        protected abstract ImagePixels OnGetPixels(int imageIndex, int paletteIndex);
    }
}
