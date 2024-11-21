﻿using Scarlet.IO.ImageFormats;

namespace GFDLibrary
{
    public static class ResourceVersion
    {
        public const uint Persona4Dancing = 0x01105030;
        public const uint Persona5        = 0x01105070;
        public const uint Persona5Royal1 = 0x01105080;
        public const uint Persona3Dancing = 0x01105090;
        public const uint Persona5Dancing = 0x01105090;
        public const uint Persona5Royal     = 0x01105100;
        public const uint CatherineFullBody = 0x01105100;
        public const uint MetaphorRefantazio = 0x02110221;

        public const uint Persona4DancingShaderCache        = 0x01087158;
        public const uint Persona5ShaderCache               = 0x01087178;
        public const uint Persona3DancingShaderCache        = 0x01087159;
        public const uint Persona5DancingShaderCache        = 0x01087159;
        public const uint MetaphorRefantazioDX11ShaderCache = 0x02000577;

        public static bool IsV2(uint version)
        {
            return version >= 0x02000000;
        }
    }
}