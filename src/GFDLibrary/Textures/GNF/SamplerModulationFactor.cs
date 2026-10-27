namespace GFDLibrary.Textures.GNF
{
    public enum SamplerModulationFactor
    {
        /// <summary>Scale associated values by 0/16. This setting invalidates the parameter.</summary>
        Factor0_0000 = 0x0,
        /// <summary>Scale associated values by 2/16.</summary>
        Factor0_1250 = 0x1,
        /// <summary>Scale associated values by 5/16.</summary>
        Factor0_3125 = 0x2,
        /// <summary>Scale associated values by 7/16.</summary>
        Factor0_4375 = 0x3,
        /// <summary>Scale associated values by 9/16.</summary>
        Factor0_5625 = 0x4,
        /// <summary>Scale associated values by 11/16.</summary>
        Factor0_6875 = 0x5,
        /// <summary>Scale associated values by 14/16.</summary>
        Factor0_8750 = 0x6,
        /// <summary>Scale associated values by 16/16. This setting effectively leaves the associated tweaks unscaled.</summary>
        Factor1_0000 = 0x7,
    }
}