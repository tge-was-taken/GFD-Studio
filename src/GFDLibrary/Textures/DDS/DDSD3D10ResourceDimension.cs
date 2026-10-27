using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Textures.DDS
{
    [Flags]
    /// <summary>Only for FourCC with DX10 https://www.sweetscape.com/010editor/repository/files/DDS.bt</summary>
    public enum DDSD3D10ResourceDimension : uint
    {
        UNKNOWN   = 0,
        BUFFER    = 1,
        TEXTURE1D = 2,
        TEXTURE2D = 3,
        TEXTURE3D = 4,
    }
}
