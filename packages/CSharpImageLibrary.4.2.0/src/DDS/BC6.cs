using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static CSharpImageLibrary.DDS.DDS_BlockHelpers;
using static CSharpImageLibrary.DDS.DX10_Helpers;

namespace CSharpImageLibrary.DDS
{
    public static class BC6
    {

        static int[] ModeToInfo = { 0, 1, 2, 10, -1, -1, 3, 11, -1, -1, 4, 12, -1, -1, 5, 13, -1, -1, 6, -1, -1, -1, 7, -1, -1, -1, 8, -1, -1, -1, 9, -1 };

        const int BC6H_MAX_REGIONS = 2;
        const ushort HALF_FLOAT_MASK = 32768;
        const int BC6H_MAX_SHAPES = 32;
        const int BC6H_MAX_INDICIES = 16;

        enum EField
        {
            NA,
            M,
            D,
            RW,
            RX,
            RY,
            RZ,
            GW,
            GX,
            GY,
            GZ,
            BW,
            BX,
            BY,
            BZ
        }

        #region Structs
        struct ModeDescriptor
        {
            public int m_uBit;
            public EField eField;

            public ModeDescriptor(EField field, int bit)
            {
                this.eField = field;
                this.m_uBit = bit;
            }
        }

        struct ModeInfo
        {
            public int modeIndex;
            public int uMode;
            public int Partitions;
            public bool Transformed;
            public int IndexPrecision;
            public LDRColour[][] RGBAPrec;  // [BC6 max regions][2]

            public ModeInfo(int modeIndex, int mode, int partitions, bool transformed, int indexPrecision, LDRColour[] first, LDRColour[] second)
            {
                this.modeIndex = modeIndex;
                this.uMode = mode;
                this.Partitions = partitions;
                this.Transformed = transformed;
                this.IndexPrecision = indexPrecision;
                RGBAPrec = new LDRColour[BC6H_MAX_REGIONS][] { first, second };
            }
        }

        static INTColour SignExtend(INTColour c, LDRColour prec)
        {
            return new INTColour()
            {
                R = SignExtend(c.R, prec.R),
                G = SignExtend(c.G, prec.G),
                B = SignExtend(c.B, prec.B)
            };
        }

        static int SignExtend(int colour, int precision)
        {
            return ((colour & (1 << (precision - 1))) != 0 ? (~0 << precision) : 0) | colour;
        }

        internal struct INTColour
        {
            public int R, G, B, Pad;

            public INTColour(int nr, int ng, int nb)
            {
                R = nr;
                G = ng;
                B = nb;
                Pad = 0;
            }

            public INTColour(RGBColour colour, int pad, bool isSigned)
            {
                R = F16ToInt(FloatToHalf(colour.r), isSigned);
                G = F16ToInt(FloatToHalf(colour.g), isSigned);
                B = F16ToInt(FloatToHalf(colour.b), isSigned);
                Pad = pad;
            }

            public INTColour(INTColour c)
            {
                R = c.R;
                G = c.G;
                B = c.B;
                Pad = c.Pad;
            }

            public override string ToString()
            {
                return $"R: {R} G: {G} B: {B}, Pad: {Pad}";
            }



            public static INTColour operator -(INTColour first, INTColour second)
            {
                return new INTColour(first.R - second.R, first.G - second.G, first.B - second.B);
            }

            public static INTColour operator +(INTColour first, INTColour second)
            {
                return new INTColour(first.R + second.R, first.G + second.G, first.B + second.B);
            }

            public static INTColour operator &(INTColour first, INTColour second)
            {
                return new INTColour(first.R & second.R, first.G & second.G, first.B & second.B);
            }

            internal LDRColour ToLDRColour(bool isSigned)
            {
                var r = IntToFloatIsh(R, isSigned);
                var g = IntToFloatIsh(G, isSigned);
                var b = IntToFloatIsh(B, isSigned);

                var c = Color.FromScRgb(1f, r, g, b);

                LDRColour colour = new LDRColour()
                {
                    R = c.R,
                    G = c.G,
                    B = c.B
                };

                return colour;
            }

            static float IntToFloatIsh(int input, bool isSigned)
            {
                ushort outVal;
                if (isSigned)
                {
                    ushort s = 0;
                    if (input < 0)
                    {
                        s = HALF_FLOAT_MASK;
                        input *= -1;
                    }

                    outVal = (ushort)(s | input);
                }
                else
                {
                    outVal = (ushort)input;
                }

                // outVal is a 'half float' now apparently

                return HalfFloatToFloat(outVal);
            }

            const uint FloatMantissasMask = 0x03FF;
            const uint FloatExponentMask = 0x7C00;
            const uint FloatSignMask = 0x8000;

            static int F16ToInt(ushort halfFloat, bool isSigned)
            {
                int result, s;
                if (isSigned)
                {
                    s = (int)(halfFloat & FloatSignMask);
                    halfFloat &= (ushort)(FloatMantissasMask & FloatExponentMask);
                    if (halfFloat > F16MAX)
                        result = F16MAX;
                    else
                        result = halfFloat;

                    result = s != 0 ? -result : result;
                }
                else
                {
                    if ((halfFloat & FloatSignMask) != 0)
                        result = 0;
                    else
                        result = halfFloat;
                }

                return result;
            }

            static unsafe float HalfFloatToFloat(ushort halfFloat)
            {
                uint mantissa = halfFloat & FloatMantissasMask;
                uint exponent = halfFloat & FloatExponentMask;
                if (exponent == FloatExponentMask)  // Inf/NAN
                    exponent = 0x8f;
                else if (exponent != 0)  // normalised
                    exponent = (uint)((halfFloat >> 10) & 0x1F);
                else if (mantissa != 0)  // denormalised
                {
                    // Normalise
                    exponent = 1;
                    do
                    {
                        exponent--;
                        mantissa <<= 1;
                    } while ((mantissa & 0x0400) == 0);

                    mantissa &= 0x03FF;
                }
                else  // == 0
                {
                    unchecked
                    {
                        exponent = (uint)(-112);    
                    }
                }

                uint longResult = ((halfFloat & FloatSignMask) << 16) | // Sign
                                ((exponent + 112) << 23) |  // Exponent
                                (mantissa << 13);  // Mantissa
                

                // Reinterpret cast
                return *(float*)&longResult;
            }

            static unsafe ushort FloatToHalf(float value)
            {
                uint val = *((uint*)&value);
                uint sign = (val & 0x80000_000U) >> 16;
                val = val & 0x7FFFF_FFF;  // Remove sign
                uint result = 0;

                if (val > 0x477FE000U)
                {
                    // Too large for HALF, set to infinity.
                    if (((val & 0x7F800000) == 0x7F800000) && ((val & 0x7FFFFF) != 0))
                        result = 0x7FFF; // NAN
                    else
                        result = 0x7C00; // INF
                }
                else
                {
                    if (val < 0x38800000U)
                    {
                        // Too small for normalised half. Convert to denormalised.
                        int shift = (int)(113U - (val >> 23));
                        val = (0x800000U | (val & 0x7FFFFFU)) >> shift;
                    }
                    else
                        val += 0xC8000000U;  // Rebias exponent to represent value as normalised.

                    result = ((val + 0x0FFFU + ((val >> 13) & 1U)) >> 13) & 0x7FFFU;
                }


                return (ushort)(result | sign);
            }

            internal INTColour Clamp(int min, int max)
            {
                return new INTColour()
                {
                    R = Math.Min(max, Math.Max(min, R)),
                    G = Math.Min(max, Math.Max(min, G)),
                    B = Math.Min(max, Math.Max(min, B))
                };
            }
        }

        

        internal struct INTColourPair
        {
            public INTColour A;
            public INTColour B;
        }
        #endregion Structs


        #region Tables
        static List<List<ModeDescriptor>> ms_aDesc = new List<List<ModeDescriptor>>()
        {
            // Mode 1 (0x00) - 10 5 5 5
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 2 (0x01) - 7 6 6 6
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },
            
            // Mode 3 (0x02) - 11 5 4 4
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,10),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,10),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 4 (0x06) - 11 4 5 4
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,10),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,10),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.BZ, 0),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 5 (0x0a) - 11 4 4 5
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,10),
                new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,10),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.BZ, 1),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 6 (0x0e) - 9 5 5 5
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 7 (0x12) - 8 6 5 5
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 8 (0x16) - 8 5 6 5
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 9 (0x1a) - 8 5 5 6
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 10 (0x1e) - 6 6 6 6
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4),
            },

            // Mode 11 (0x03) - 10 10
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RX, 8), new ModeDescriptor(EField.RX, 9), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GX, 8), new ModeDescriptor(EField.GX, 9), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BX, 8), new ModeDescriptor(EField.BX, 9), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
            },

            // Mode 12 (0x07) - 11 9
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RX, 8), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GX, 8), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BX, 8), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
            },

            // Mode 13 (0x0b) - 12 8
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RW,11), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GW,11), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BW,11), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
            },

            // Mode 14 (0x0f) - 16 4
            new List<ModeDescriptor>()
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,15),
                new ModeDescriptor(EField.RW,14), new ModeDescriptor(EField.RW,13), new ModeDescriptor(EField.RW,12), new ModeDescriptor(EField.RW,11), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,15),
                new ModeDescriptor(EField.GW,14), new ModeDescriptor(EField.GW,13), new ModeDescriptor(EField.GW,12), new ModeDescriptor(EField.GW,11), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,15),
                new ModeDescriptor(EField.BW,14), new ModeDescriptor(EField.BW,13), new ModeDescriptor(EField.BW,12), new ModeDescriptor(EField.BW,11), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
            },
        };

        static ModeInfo[] ms_aInfo = new ModeInfo[]
        {
            new ModeInfo(0, 0x00, 1, true,  3, new LDRColour[] { new LDRColour(10,10,10,0), new LDRColour( 5, 5, 5,0) },    new LDRColour[] { new LDRColour( 5, 5, 5,0), new LDRColour( 5, 5, 5,0) }),
            new ModeInfo(1, 0x01, 1, true,  3, new LDRColour[] { new LDRColour(7,7,7,0),    new LDRColour( 6, 6, 6,0) },    new LDRColour[] { new LDRColour( 6, 6, 6,0), new LDRColour( 6, 6, 6,0) }),
            new ModeInfo(2, 0x02, 1, true,  3, new LDRColour[] { new LDRColour(11,11,11,0), new LDRColour( 5, 4, 4,0) },    new LDRColour[] { new LDRColour( 5, 4, 4,0), new LDRColour( 5, 4, 4,0) }),
            new ModeInfo(3, 0x06, 1, true,  3, new LDRColour[] { new LDRColour(11,11,11,0), new LDRColour( 4, 5, 4,0) },    new LDRColour[] { new LDRColour( 4, 5, 4,0), new LDRColour( 4, 5, 4,0) }),
            new ModeInfo(4, 0x0a, 1, true,  3, new LDRColour[] { new LDRColour(11,11,11,0), new LDRColour( 4, 4, 5,0) },    new LDRColour[] { new LDRColour( 4, 4, 5,0), new LDRColour( 4, 4, 5,0) }),
            new ModeInfo(5, 0x0e, 1, true,  3, new LDRColour[] { new LDRColour(9,9,9,0),    new LDRColour( 5, 5, 5,0) },    new LDRColour[] { new LDRColour( 5, 5, 5,0), new LDRColour( 5, 5, 5,0) }),
            new ModeInfo(6, 0x12, 1, true,  3, new LDRColour[] { new LDRColour(8,8,8,0),    new LDRColour( 6, 5, 5,0) },    new LDRColour[] { new LDRColour( 6, 5, 5,0), new LDRColour( 6, 5, 5,0) }),
            new ModeInfo(7, 0x16, 1, true,  3, new LDRColour[] { new LDRColour(8,8,8,0),    new LDRColour( 5, 6, 5,0) },    new LDRColour[] { new LDRColour( 5, 6, 5,0), new LDRColour( 5, 6, 5,0) }),
            new ModeInfo(8, 0x1a, 1, true,  3, new LDRColour[] { new LDRColour(8,8,8,0),    new LDRColour( 5, 5, 6,0) },    new LDRColour[] { new LDRColour( 5, 5, 6,0), new LDRColour( 5, 5, 6,0) }),
            new ModeInfo(9, 0x1e, 1, false, 3, new LDRColour[] { new LDRColour(6,6,6,0),    new LDRColour( 6, 6, 6,0) },    new LDRColour[] { new LDRColour( 6, 6, 6,0), new LDRColour( 6, 6, 6,0) }),
            new ModeInfo(10, 0x03, 0, false, 4, new LDRColour[] { new LDRColour(10,10,10,0), new LDRColour( 10, 10, 10,0) }, new LDRColour[] { new LDRColour( 0, 0, 0,0), new LDRColour( 0, 0, 0,0) }),
            new ModeInfo(11, 0x07, 0, true,  4, new LDRColour[] { new LDRColour(11,11,11,0), new LDRColour( 9, 9, 9,0) },    new LDRColour[] { new LDRColour( 0, 0, 0,0), new LDRColour( 0, 0, 0,0) }),
            new ModeInfo(12, 0x0b, 0, true,  4, new LDRColour[] { new LDRColour(12,12,12,0), new LDRColour( 8, 8, 8,0) },    new LDRColour[] { new LDRColour( 0, 0, 0,0), new LDRColour( 0, 0, 0,0) }),
            new ModeInfo(13, 0x0f, 0, true,  4, new LDRColour[] { new LDRColour(16,16,16,0), new LDRColour( 4,4, 4,0) },     new LDRColour[] { new LDRColour( 0,0, 0,0),  new LDRColour( 0,0, 0,0) } )
        };
        #endregion Tables

        #region Decompression
        internal static LDRColour[] DecompressBC6(byte[] source, int sourceStart, bool isSigned)
        {
            LDRColour[] block = new LDRColour[NUM_PIXELS_PER_BLOCK];

            int startBit = 0;
            int mode = GetBits(source, sourceStart, ref startBit, 2);
            if (mode != 0 && mode != 0x01)
                mode = (GetBits(source, sourceStart, ref startBit, 3) << 2) | mode;


            if (ModeToInfo[mode] >= 0)
            {
                List<ModeDescriptor> desc = ms_aDesc[ModeToInfo[mode]];
                ModeInfo info = ms_aInfo[ModeToInfo[mode]];
                int shape = 0;
                INTColourPair[] endPoints = new INTColourPair[BC6H_MAX_REGIONS];

                // Header?
                int headerBits = info.Partitions > 0 ? 82 : 65;
                while (startBit < headerBits)
                {
                    int currBit = startBit;
                    if (GetBit(source, sourceStart, ref startBit) != 0)
                    {
                        switch (desc[currBit].eField)
                        {
                            case EField.D: shape |= 1 << desc[currBit].m_uBit; break;
                            case EField.RW: endPoints[0].A.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RX: endPoints[0].B.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RY: endPoints[1].A.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RZ: endPoints[1].B.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.GW: endPoints[0].A.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GX: endPoints[0].B.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GY: endPoints[1].A.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GZ: endPoints[1].B.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.BW: endPoints[0].A.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BX: endPoints[0].B.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BY: endPoints[1].A.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BZ: endPoints[1].B.B |= 1 << desc[currBit].m_uBit; break;
                            default:
                                Debugger.Break();
                                break;
                        }               
                    }
                }

                // Sign extend necessary end points
                if (isSigned)
                    endPoints[0].A = SignExtend(endPoints[0].A, info.RGBAPrec[0][0]);
                
                if (isSigned || info.Transformed)
                {
                    for (int p = 0; p <= info.Partitions; p++)
                    {
                        if (p != 0)
                            endPoints[p].A = SignExtend(endPoints[p].A, info.RGBAPrec[p][0]);

                        endPoints[p].B = SignExtend(endPoints[p].B, info.RGBAPrec[p][1]);
                    }
                }

                // Inverse transform end points
                if (info.Transformed)
                    TransformInverse(endPoints, info.RGBAPrec[0][0], isSigned);

                // Read indicies
                int prec = info.IndexPrecision;
                int partitions = info.Partitions;
                byte[] partTable = PartitionTable[partitions][shape];

                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                {
                    int numBits = IsFixUpOffset(partitions, shape, i) ? prec - 1 : prec;
                    if (startBit + numBits > 128)
                        Debugger.Break();

                    int index = GetBits(source, sourceStart, ref startBit, numBits);
                    if (index >= ((partitions > 0) ? 8 : 16))
                        Debugger.Break();

                    int region = partTable[i];

                    // Unquantise endpoints and interpolate
                    int r1 = Unquantise(endPoints[region].A.R, info.RGBAPrec[0][0].R, isSigned);
                    int g1 = Unquantise(endPoints[region].A.G, info.RGBAPrec[0][0].G, isSigned);
                    int b1 = Unquantise(endPoints[region].A.B, info.RGBAPrec[0][0].B, isSigned);
                    int r2 = Unquantise(endPoints[region].B.R, info.RGBAPrec[0][0].R, isSigned);
                    int g2 = Unquantise(endPoints[region].B.G, info.RGBAPrec[0][0].G, isSigned);
                    int b2 = Unquantise(endPoints[region].B.B, info.RGBAPrec[0][0].B, isSigned);

                    int[] aWeights = info.Partitions > 0 ? AWeights3 : AWeights4;
                    INTColour fc = new INTColour()
                    {
                        R = FinishUnquantise((r1 * (BC67_WEIGHT_MAX - aWeights[index]) + r2 * aWeights[index] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned),
                        G = FinishUnquantise((g1 * (BC67_WEIGHT_MAX - aWeights[index]) + g2 * aWeights[index] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned),
                        B = FinishUnquantise((b1 * (BC67_WEIGHT_MAX - aWeights[index]) + b2 * aWeights[index] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned),
                    };

                    LDRColour colour = fc.ToLDRColour(isSigned);
                    colour.A = 255;
                    block[i] = colour;
                }
            }

            return block;
        }

        private static int FinishUnquantise(int comp, bool isSigned)
        {
            if (isSigned)
                return (comp < 0) ? -((-comp * 31) >> 5) : (comp * 31) >> 5;  // Scale magnitude by 31/32
            else
                return (comp * 31) >> 6;  // Scale magnitude by 31/64
        }

        private static int Unquantise(int comp, int bitsPerComp, bool isSigned)
        {
            int unq = 0, s = 0;
            if (isSigned)
            {
                if (bitsPerComp >= 16)
                    unq = comp;
                else
                {
                    if (comp < 0)
                    {
                        s = 1;
                        comp *= -1;
                    }

                    if (comp == 0)
                        unq = 0;
                    else if (comp >= ((1 << (bitsPerComp - 1)) - 1))
                        unq = 0x7FFF;
                    else
                        unq = ((comp << 15) + 0x4000) >> (bitsPerComp - 1);

                    if (s != 0)
                        unq *= -1;
                }
            }
            else
            {
                if (bitsPerComp >= 15)
                    unq = comp;
                else if (comp == 0)
                    unq = 0;
                else if (comp == ((1 << bitsPerComp) - 1))
                    unq = 0xFFFF;
                else
                    unq = ((comp << 16) + 0x8000) >> bitsPerComp;
            }

            return unq;
        }

        private static void TransformInverse(INTColourPair[] endPoints, LDRColour prec, bool isSigned)
        {
            INTColour wrapMask = new INTColour((1 << prec.R) - 1, (1 << prec.G) - 1, (1 << prec.B) - 1);
            endPoints[0].B += endPoints[0].A;
            endPoints[0].B &= wrapMask;

            endPoints[1].A += endPoints[0].A;
            endPoints[1].A &= wrapMask;

            endPoints[1].B += endPoints[0].A;
            endPoints[1].B &= wrapMask;

            if (isSigned)
            {
                endPoints[0].B = SignExtend(endPoints[0].B, prec);
                endPoints[1].A = SignExtend(endPoints[1].A, prec);
                endPoints[1].B = SignExtend(endPoints[1].B, prec);
            }
        }
        #endregion Decompression


        const int F16MIN = -31743;
        const ushort F16MAX = 31743;
        #region Compression

        internal static void CompressBC6Block(byte[] source, int sourceStart, int sourceLineLength, byte[] destination, int destStart, INTColour[] overrides = null, RGBColour[] overrides2 = null)
        {
            int modeVal = 0;
            float bestErr = float.MaxValue;

            INTColourPair[][] AllEndPoints = new INTColourPair[BC6H_MAX_SHAPES][];
            for (int i = 0; i < BC6H_MAX_SHAPES; i++)
                AllEndPoints[i] = new INTColourPair[BC6H_MAX_REGIONS];


            // Populate pixel structures
            INTColour[] block = new INTColour[NUM_PIXELS_PER_BLOCK];
            RGBColour[] pixels = new RGBColour[NUM_PIXELS_PER_BLOCK];

            if (overrides != null)
            {
                block = overrides;
                pixels = overrides2;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        var offset = sourceStart + (i * sourceLineLength) + j * 4;  // TODO Component sizes

                        var r = source[offset + 2];  // Red
                        var g = source[offset + 1];  // Green
                        var b = source[offset];      // Blue
                        var a = source[offset + 3]; // Alpha

                        var c = Color.FromArgb(a, r, g, b);
                        var pixel = new RGBColour(c.ScR, c.ScG, c.ScB, c.ScA);
                        pixels[i * 4 + j] = pixel;
                        block[i * 4 + j] = new INTColour(pixel, 0, false); // TODO Signed 
                    }
                }
            }


            for (modeVal = 0; modeVal < ms_aInfo.Length && bestErr > 0; modeVal++)
            {
                ModeInfo mode = ms_aInfo[modeVal];
                int maxShapes = mode.Partitions != 0 ? 32 : 1;
                int shape = 0;
                int items = Math.Max(1, maxShapes >> 2);
                float[] roughMSEs = new float[BC6H_MAX_SHAPES];
                int[] auShape = new int[BC6H_MAX_SHAPES];


                // Pick best items shapes and refine them
                for (shape = 0; shape < maxShapes; shape++)
                {
                    roughMSEs[shape] = RoughMSE(ref AllEndPoints[shape], mode, shape, block, pixels, false);   // TODO signed
                    auShape[shape] = shape;
                }

                // Bubble up the first items item.
                for (int i = 0; i < items; i++)
                {
                    for (int j = i + 1; j < maxShapes; j++)
                    {
                        if (roughMSEs[i] > roughMSEs[j])
                        {
                            var temp = roughMSEs[i];
                            roughMSEs[i] = roughMSEs[j];
                            roughMSEs[j] = temp;

                            var temp2 = auShape[i];
                            auShape[i] = auShape[j];
                            auShape[j] = temp2;
                        }
                    }
                }

                for (int i = 0; i < items && bestErr > 0; i++)
                {
                    shape = auShape[i];
                    Refine(mode, ref bestErr, AllEndPoints[shape], block, shape, destination, destStart);
                }
            }
        }

        static void Refine(ModeInfo mode, ref float bestErr, INTColourPair[] unqantisedEndPts, INTColour[] block, int shape, byte[] destination, int destStart)
        {
            float[] orgErr = new float[BC6H_MAX_REGIONS];
            float[] optErr = new float[BC6H_MAX_REGIONS];
            INTColourPair[] orgEndPoints = new INTColourPair[BC6H_MAX_REGIONS];
            INTColourPair[] optEndPoints = new INTColourPair[BC6H_MAX_REGIONS];
            int[] orgIdx = new int[NUM_PIXELS_PER_BLOCK];
            int[] optIdx = new int[NUM_PIXELS_PER_BLOCK];

            QuantiseEndPts(mode, orgEndPoints, unqantisedEndPts);
            AssignIndicies(mode, orgEndPoints, orgErr, shape, block, orgIdx);
            SwapIndicies(mode, shape, orgIdx, orgEndPoints);

            if (mode.Transformed)
                TransformForward(orgEndPoints);

            if (EndPointsFit(mode, orgEndPoints))
            {
                if (mode.Transformed)
                    TransformInverse(orgEndPoints, mode.RGBAPrec[0][0], false);  // TODO Signed

                OptimiseEndPoints(mode, shape, block, orgErr, optEndPoints, orgEndPoints);
                AssignIndicies(mode, optEndPoints, optErr, shape, block, optIdx);
                SwapIndicies(mode, shape, optIdx, optEndPoints);

                float orgTotalErr = 0f;
                float optTotalErr = 0f;
                for (int p = 0; p <= mode.Partitions; p++)
                {
                    orgTotalErr += orgErr[p];
                    optTotalErr += optErr[p];
                }

                if (mode.Transformed)
                    TransformForward(optEndPoints);

                if (EndPointsFit(mode, optEndPoints) && optTotalErr < orgTotalErr && optTotalErr < bestErr)
                {
                    bestErr = optTotalErr;
                    EmitBlock(mode, destination, destStart, shape, optEndPoints, optIdx);
                }
                else if (orgTotalErr < bestErr)
                {
                    // either it stopped fitting when we optimized it, or there was no improvement
                    // so go back to the unoptimized endpoints which we know will fit

                    if (mode.Transformed)
                        TransformForward(orgEndPoints);
                    bestErr = orgTotalErr;
                    EmitBlock(mode, destination, destStart, shape, orgEndPoints, orgIdx);
                }
            }

        }



        static void EmitBlock(ModeInfo mode, byte[] destination, int destStart, int shape, INTColourPair[] endPts, int[] pixelIndicies)
        {
            int headerBits = mode.Partitions > 0 ? 82 : 65;
            List<ModeDescriptor> desc = ms_aDesc[mode.modeIndex];
            int startBit = 0;

            while (startBit < headerBits)
            {
                switch (desc[startBit].eField)
                {
                    case EField.M:
                        SetBit(ref startBit, destination, destStart, (mode.uMode >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.D:
                        SetBit(ref startBit, destination, destStart, (shape >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.RW:
                        SetBit(ref startBit, destination, destStart, (endPts[0].A.R >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.RX:
                        SetBit(ref startBit, destination, destStart, (endPts[0].B.R >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.RY:
                        SetBit(ref startBit, destination, destStart, (endPts[1].A.R >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.RZ:
                        SetBit(ref startBit, destination, destStart, (endPts[1].B.R >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.GW:
                        SetBit(ref startBit, destination, destStart, (endPts[0].A.G >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.GX:
                        SetBit(ref startBit, destination, destStart, (endPts[0].B.G >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.GY:
                        SetBit(ref startBit, destination, destStart, (endPts[1].A.G >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.GZ:
                        SetBit(ref startBit, destination, destStart, (endPts[1].B.G >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.BW:
                        SetBit(ref startBit, destination, destStart, (endPts[0].A.B >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.BX:
                        SetBit(ref startBit, destination, destStart, (endPts[0].B.B >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.BY:
                        SetBit(ref startBit, destination, destStart, (endPts[1].A.B >> desc[startBit].m_uBit) & 0x01);
                        break;
                    case EField.BZ:
                        SetBit(ref startBit, destination, destStart, (endPts[1].B.B >> desc[startBit].m_uBit) & 0x01);
                        break;
                }
            }

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
            {
                if (IsFixUpOffset(mode.Partitions, shape, i))
                    SetBits(ref startBit, mode.IndexPrecision - 1, pixelIndicies[i], destination, destStart);
                else
                    SetBits(ref startBit, mode.IndexPrecision, pixelIndicies[i], destination, destStart);
            }
        }

        static void SetBits(ref int startBit, int length, int value, byte[] destination, int destStart)
        {
            if (length == 0)
                return;

            int index = startBit >> 3;
            int uBase = startBit - (index << 3);
            if (uBase + length > 8)
            {
                int firstIndexBits = 8 - uBase;
                int nextiIndexBits = length - firstIndexBits;
                destination[destStart + index] &= (byte)~(((1 << firstIndexBits) - 1) << uBase);
                destination[destStart + index] |= (byte)(value << uBase);
                destination[destStart + index + 1] &= (byte)~((1 << nextiIndexBits) - 1);
                destination[destStart + index + 1] |= (byte)(value >> firstIndexBits);
            }
            else
            {
                destination[destStart + index] &= (byte)~(((1 << length) - 1) << uBase);
                destination[destStart + index] |= (byte)(value << uBase);
            }

            startBit += length;
        }

        static void SetBit(ref int startBit, byte[] destination, int destStart, int bit)
        {
            int index = startBit >> 3;
            int uBase = startBit - (index << 3);
            destination[destStart + index] &= (byte)~(1 << uBase);
            destination[destStart + index] |= (byte)(bit << uBase);
            startBit++;
        }

        static void OptimiseEndPoints(ModeInfo mode, int shape, INTColour[] block, float[] orgErr, INTColourPair[] optEndPts, INTColourPair[] orgEndPts)
        {
            INTColour[] pixels = new INTColour[NUM_PIXELS_PER_BLOCK];

            for (int p = 0; p <= mode.Partitions; p++)
            {
                int np = 0;
                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                    if (PartitionTable[p][shape][i] == p)
                        pixels[np++] = block[i];

                OptimiseOne(mode, orgErr[p], ref optEndPts[p], orgEndPts[p], np, pixels);
            }
        }

        static unsafe void OptimiseOne(ModeInfo mode, float orgErr, ref INTColourPair opt, INTColourPair orgEndPts, int np, INTColour[] block)
        {
            float optErr = orgErr;
            opt.A = orgEndPts.A;
            opt.B = orgEndPts.B;

            INTColourPair optEndPts = opt;

            INTColourPair new_a = new INTColourPair();
            INTColourPair new_b = new INTColourPair();
            INTColourPair newEndPoints = new INTColourPair();
            bool do_b = false;

            // Optimise each separately
            for (int ch = 0; ch < 3; ch++)
            {
                int* optEndPtsAChannel = null;
                int* optEndPtsBChannel = null;
                int* newEndPtsAChannel = null;
                int* newEndPtsBChannel = null;
                int* new_bBChannel = null;
                int* new_aAChannel = null;

                switch (ch)
                {
                    case 0:
                        optEndPtsAChannel = &optEndPts.A.R;
                        optEndPtsBChannel = &optEndPts.B.R;
                        newEndPtsAChannel = &newEndPoints.A.R;
                        newEndPtsBChannel = &newEndPoints.B.R;
                        new_aAChannel = &new_a.A.R;
                        new_bBChannel = &new_b.B.R;
                        break;
                    case 1:
                        optEndPtsAChannel = &optEndPts.A.G;
                        optEndPtsBChannel = &optEndPts.B.G;
                        newEndPtsAChannel = &newEndPoints.A.G;
                        newEndPtsBChannel = &newEndPoints.B.G;
                        new_aAChannel = &new_a.A.G;
                        new_bBChannel = &new_b.B.G;
                        break;
                    case 2:
                        optEndPtsAChannel = &optEndPts.A.B;
                        optEndPtsBChannel = &optEndPts.B.B;
                        newEndPtsAChannel = &newEndPoints.A.B;
                        newEndPtsBChannel = &newEndPoints.B.B;
                        new_aAChannel = &new_a.A.B;
                        new_bBChannel = &new_b.B.B;
                        break;
                }



                // figure out which endpoint when perturbed gives the most improvement and start there
                // if we just alternate, we can easily end up in a local minima
                float err0 = PerturbOne(mode, ch, optErr, ref optEndPts, ref new_a, false, block, np);
                float err1 = PerturbOne(mode, ch, optErr, ref optEndPts, ref new_b, true, block, np);

                if (err0 < err1)
                {
                    if (err0 >= optErr)
                        continue;

                    *optEndPtsAChannel = *new_aAChannel;
                    optErr = err0;
                    do_b = true;
                }
                else
                {
                    if (err1 >= optErr)
                        continue;

                    *optEndPtsBChannel = *new_bBChannel;
                    optErr = err1;
                    do_b = false;
                }

                while (true)
                {
                    float err = PerturbOne(mode, ch, optErr, ref optEndPts, ref newEndPoints, do_b, block, np);
                    if (err >= optErr)
                        break;

                    if (!do_b)
                        *optEndPtsAChannel = *newEndPtsAChannel;
                    else
                        *optEndPtsBChannel = *newEndPtsBChannel;

                    optErr = err;
                    do_b = !do_b;
                }
            }

            opt = optEndPts;
        }

        static unsafe float PerturbOne(ModeInfo mode, int ch, float oldErr, ref INTColourPair olds, ref INTColourPair news, bool do_b, INTColour[] block, int np)
        {
            int prec = 0;
            int* tempAChannel = null;
            int* tempBChannel = null;

            INTColourPair newEndPoints = news;
            INTColourPair oldEndPoints = olds;

            int* newAChannel = null;
            int* newBChannel = null;


            INTColourPair tempEndPoints = new INTColourPair();
            float minErr = oldErr;
            int bestStep = 0;


            switch (ch)
            {
                case 0:
                    prec = mode.RGBAPrec[0][0].R;

                    tempAChannel = &tempEndPoints.A.R;
                    tempBChannel = &tempEndPoints.B.R;
                    newAChannel = &newEndPoints.A.R;
                    newBChannel = &newEndPoints.B.R;
                    break;
                case 1:
                    prec = mode.RGBAPrec[0][0].G;

                    tempAChannel = &tempEndPoints.A.G;
                    tempBChannel = &tempEndPoints.B.G;
                    newAChannel = &newEndPoints.A.G;
                    newBChannel = &newEndPoints.B.G;
                    break;
                case 2:
                    prec = mode.RGBAPrec[0][0].B;

                    tempAChannel = &tempEndPoints.A.B;
                    tempBChannel = &tempEndPoints.B.B;
                    newAChannel = &newEndPoints.A.B;
                    newBChannel = &newEndPoints.B.B;
                    break;
            }

            

            // save endpoints
            tempEndPoints = newEndPoints = oldEndPoints;
            
            for (int step = 1 << (prec - 1); step != 0; step >>= 1)
            {
                bool improved = false;
                for (int sign = -1; sign <= 1; sign += 2)
                {
                    if (!do_b)
                    {
                        *tempAChannel = *newAChannel + sign * step;
                        if (*tempAChannel < 0 || *tempAChannel >= (1 << prec))
                            continue;
                    }
                    else
                    {
                        *tempBChannel = *newBChannel + sign * step;
                        if (*tempBChannel < 0 || *tempBChannel >= (1 << prec))
                            continue;
                    }

                    float err = MapColoursQuantised(mode, tempEndPoints, np, block);
                    if (err < minErr)
                    {
                        improved = true;
                        minErr = err;
                        bestStep = sign * step;
                    }
                }

                if (improved)
                {
                    if (!do_b)
                        *newAChannel += bestStep;
                    else
                        *newBChannel += bestStep;
                }
            }


            // Update outsides
            news = newEndPoints;
            olds = oldEndPoints;

            return minErr;
        }

        static float MapColoursQuantised(ModeInfo mode, INTColourPair endPts, int np, INTColour[] block)
        {
            int numIndicies = 1 << mode.IndexPrecision;

            INTColour[] palette = new INTColour[BC6H_MAX_INDICIES];
            GeneratePaletteQuantised(mode, endPts, palette);

            float totErr = 0;
            for (int i = 0; i < np; i++)
            {
                var rDiff = block[i].R - palette[0].R;
                var gDiff = block[i].G - palette[0].G;
                var bDiff = block[i].B - palette[0].B;

                float bestErr = rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;

                for (int j = 1; j < numIndicies && bestErr > 0; j++)
                {
                    rDiff = block[i].R - palette[0].R;
                    gDiff = block[i].G - palette[0].G;
                    bDiff = block[i].B - palette[0].B;

                    float err = rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;

                    if (err > bestErr)
                        break;

                    if (err < bestErr)
                        bestErr = err;
                }

                totErr += bestErr;
            }

            return totErr;
        }

        static bool EndPointsFit(ModeInfo mode, INTColourPair[] endPts)
        {
            bool isSigned = false;  // TODO signed

            var prec0 = mode.RGBAPrec[0][0];
            var prec1 = mode.RGBAPrec[0][1];
            var prec2 = mode.RGBAPrec[1][0];
            var prec3 = mode.RGBAPrec[1][1];

            INTColour[] aBits = new INTColour[4];
            aBits[0].R = NBits(endPts[0].A.R, isSigned);
            aBits[0].G = NBits(endPts[0].A.G, isSigned);
            aBits[0].B = NBits(endPts[0].A.B, isSigned);

            aBits[1].R = NBits(endPts[0].B.R, mode.Transformed || isSigned);
            aBits[1].G = NBits(endPts[0].B.G, mode.Transformed || isSigned);
            aBits[1].B = NBits(endPts[0].B.B, mode.Transformed || isSigned);

            if (aBits[0].R > prec0.R || aBits[1].R > prec1.R ||
                aBits[0].G > prec0.G || aBits[1].G > prec1.G ||
                aBits[0].B > prec0.B || aBits[1].B > prec1.B)
                return false;

            if (mode.Partitions != 0)
            {
                aBits[2].R = NBits(endPts[1].A.R, mode.Transformed || isSigned);
                aBits[2].G = NBits(endPts[1].A.G, mode.Transformed || isSigned);
                aBits[2].B = NBits(endPts[1].A.B, mode.Transformed || isSigned);

                aBits[3].R = NBits(endPts[1].B.R, mode.Transformed || isSigned);
                aBits[3].G = NBits(endPts[1].B.G, mode.Transformed || isSigned);
                aBits[3].B = NBits(endPts[1].B.B, mode.Transformed || isSigned);

                if (aBits[2].R > prec2.R || aBits[3].R > prec3.R ||
                    aBits[2].G > prec2.G || aBits[3].G > prec3.G ||
                    aBits[2].B > prec2.B || aBits[3].B > prec3.B)
                    return false;
            }

            return true;
        }

        static int NBits(int n, bool isSigned)
        {
            int nb = 0;
            if (n == 0)
                return 0;
            else if (n > 0)
            {
                for (nb = 0; n != 0; nb++, n >>= 1)
                {
                    // Nothing
                }
                return nb + (isSigned ? 1 : 0);
            }
            else
            {
                for (nb = 0; n < -1; nb++, n >>= 1)
                {
                    // Nothing
                }
                return nb + 1;
            }
        }

        static void TransformForward(INTColourPair[] endPts)
        {
            endPts[0].B -= endPts[0].A;
            endPts[1].A -= endPts[0].A;
            endPts[1].B -= endPts[0].A;
        }

        static void SwapIndicies(ModeInfo mode, int shape, int[] pixelIndicies, INTColourPair[] endPts)
        {
            int numIndicies = 1 << mode.IndexPrecision;
            int highIndexBit = numIndicies >> 1;

            for (int p = 0; p <= mode.Partitions; p++)
            {
                int i = FixUpTable[mode.Partitions][shape][p];
                if ((pixelIndicies[i] & highIndexBit) != 0)
                {
                    var temp = endPts[p].A;
                    endPts[p].A = endPts[p].B;
                    endPts[p].B = temp;

                    for (int j = 0; j < NUM_PIXELS_PER_BLOCK; j++)
                        if (PartitionTable[mode.Partitions][shape][j] == p)
                            pixelIndicies[j] = numIndicies - 1 - pixelIndicies[j];
                }
            }
        }
        
        static void AssignIndicies(ModeInfo mode, INTColourPair[] endPts, float[] totalErr, int shape, INTColour[] block, int[] pixelIndicies)
        {
            int numIndicies = 1 << mode.IndexPrecision;

            // build list of possibles
            INTColour[][] palette = new INTColour[BC6H_MAX_REGIONS][];
            for (int i = 0; i < BC6H_MAX_REGIONS; i++)
                palette[i] = new INTColour[BC6H_MAX_INDICIES];

            for (int p = 0; p <= mode.Partitions; p++)
            {
                GeneratePaletteQuantised(mode, endPts[p], palette[p]);
                totalErr[p] = 0;
            }

            for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
            {
                int region = PartitionTable[mode.Partitions][shape][i];
                float bestErr = Norm(block[i], palette[region][0]);
                pixelIndicies[i] = 0;
                for (int j = 1; j < numIndicies && bestErr > 0; j++)
                {
                    float err = Norm(block[i], palette[region][j]);
                    if (err > bestErr)
                        break;

                    if (err < bestErr)
                    {
                        bestErr = err;
                        pixelIndicies[i] = j;
                    }
                }

                totalErr[region] += bestErr;
            }
        }

        static void GeneratePaletteQuantised(ModeInfo mode, INTColourPair endPoints, INTColour[] palette)
        {
            bool isSigned = false;  // TODO signed

            int numIndicies = 1 << mode.IndexPrecision;
            var prec = mode.RGBAPrec[0][0];

            // Scale endpts
            INTColourPair unqEndPts = new INTColourPair();
            unqEndPts.A.R = Unquantise(endPoints.A.R, prec.R, isSigned);
            unqEndPts.A.G = Unquantise(endPoints.A.G, prec.G, isSigned);
            unqEndPts.A.B = Unquantise(endPoints.A.B, prec.B, isSigned);
            unqEndPts.B.R = Unquantise(endPoints.B.R, prec.R, isSigned);
            unqEndPts.B.G = Unquantise(endPoints.B.G, prec.G, isSigned);
            unqEndPts.B.B = Unquantise(endPoints.B.B, prec.B, isSigned);

            //interpolate
            int[] weights = null;
            if (mode.IndexPrecision == 3)
                weights = AWeights3;
            else if (mode.IndexPrecision == 4)
                weights = AWeights4;

            for (int i = 0; i < numIndicies; i++)
            {
                palette[i].R =FinishUnquantise((unqEndPts.A.R * (BC67_WEIGHT_MAX - weights[i]) + unqEndPts.B.R * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned);
                palette[i].G =FinishUnquantise((unqEndPts.A.G * (BC67_WEIGHT_MAX - weights[i]) + unqEndPts.B.G * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned);
                palette[i].B =FinishUnquantise((unqEndPts.A.B * (BC67_WEIGHT_MAX - weights[i]) + unqEndPts.B.B * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT, isSigned);
            }
        }

        static void QuantiseEndPts(ModeInfo mode, INTColourPair[] quantisedEndPts, INTColourPair[] unqantisedEndPts)
        {
            bool isSigned = false;

            var prec = mode.RGBAPrec[0][0];
            for (int p = 0; p <= mode.Partitions; p++)
            {
                quantisedEndPts[p].A.R = Quantise(unqantisedEndPts[p].A.R, prec.R, isSigned);
                quantisedEndPts[p].A.G = Quantise(unqantisedEndPts[p].A.G, prec.G, isSigned);
                quantisedEndPts[p].A.B = Quantise(unqantisedEndPts[p].A.B, prec.B, isSigned);

                quantisedEndPts[p].B.R = Quantise(unqantisedEndPts[p].B.R, prec.R, isSigned);
                quantisedEndPts[p].B.G = Quantise(unqantisedEndPts[p].B.G, prec.G, isSigned);
                quantisedEndPts[p].B.B = Quantise(unqantisedEndPts[p].B.B, prec.B, isSigned);
            }
        }

        private static int Quantise(int value, int prec, bool isSigned)
        {
            int q, s = 0;
            if (isSigned)
            {
                if (value < 0)
                {
                    s = 1;
                    value *= -1;
                }

                q = (prec >= 16) ? value : (value << (prec - 1)) / (F16MAX + 1);

                if (s != 0)
                    q *= -1;
            }
            else
            {
                q = (prec >= 15) ? value : (value << prec) / (F16MAX + 1);
            }

            return q;
        }

        static float RoughMSE(ref INTColourPair[] endPoints, ModeInfo mode, int shape, INTColour[] block, RGBColour[] pixels, bool isSigned)
        {
            int[] pixelIndicies = new int[NUM_PIXELS_PER_BLOCK];

            float err = 0f;
            for (int p = 0; p <= mode.Partitions; p++)
            {
                int np = 0;
                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                    if (PartitionTable[mode.Partitions][shape][i] == p)
                        pixelIndicies[np++] = i;

                // Simple cases
                if (np == 1)
                {
                    endPoints[p].A = block[pixelIndicies[0]];
                    endPoints[p].B = block[pixelIndicies[0]];
                    continue;
                }
                else if (np == 2)
                {
                    endPoints[p].A = block[pixelIndicies[0]];
                    endPoints[p].B = block[pixelIndicies[1]];
                    continue;
                }

                RGBColour[] minMax = OptimiseRGB_BC67(pixels, 4, np, pixelIndicies);
                endPoints[p].A = new INTColour(minMax[0], endPoints[p].A.Pad, isSigned);
                endPoints[p].B = new INTColour(minMax[1], endPoints[p].B.Pad, isSigned);


                if (isSigned)
                {
                    endPoints[p].A = endPoints[p].A.Clamp(F16MIN, F16MAX);
                    endPoints[p].B = endPoints[p].B.Clamp(F16MIN, F16MAX);
                }
                else
                {
                    endPoints[p].A = endPoints[p].A.Clamp(0, F16MAX);
                    endPoints[p].B = endPoints[p].B.Clamp(0, F16MAX);
                }


                err += MapColours(mode, np, p, endPoints[p], block, pixelIndicies);
            }


            return err;
        }


        static float MapColours(ModeInfo mode, int np, int region, INTColourPair endPoints, INTColour[] block, int[] pixelIndicies)
        {
            int indexPrecision = mode.IndexPrecision;
            int numIndicies = 1 << indexPrecision;

            INTColour[] aPalette = new INTColour[BC6H_MAX_INDICIES];
            GeneratePaletteUnquantised(endPoints, indexPrecision, aPalette);

            float totalErr = 0f;
            for (int i = 0; i < np; i++)
            {
                float bestErr = Norm(block[pixelIndicies[i]], aPalette[0]);
                for (int j = 1; j < numIndicies && bestErr > 0f; j++)
                {
                    float err = Norm(block[pixelIndicies[i]], aPalette[j]);
                    if (err > bestErr)
                        break;

                    if (err < bestErr)
                        bestErr = err;
                }

                totalErr += bestErr;
            }

            return totalErr;
        }

        static float Norm(INTColour a, INTColour b)
        {
            float dr = a.R - b.R;
            float dg = a.G - b.G;
            float db = a.B - b.B;
            return dr * dr + dg * dg + db * db;
        }

        static void GeneratePaletteUnquantised(INTColourPair endPoints, int indexPrecision, INTColour[] palette)
        {
            int numIndicies = 1 << indexPrecision;
            int[] weights = indexPrecision == 3 ? AWeights3 : indexPrecision == 4 ? AWeights4 : null;

            for (int i = 0; i < numIndicies; i++)
            {
                if (weights == null)
                    palette[i] = new INTColour();
                else
                {
                    palette[i].R = (endPoints.A.R * (BC67_WEIGHT_MAX - weights[i]) + endPoints.B.R * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
                    palette[i].G = (endPoints.A.G * (BC67_WEIGHT_MAX - weights[i]) + endPoints.B.G * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
                    palette[i].B = (endPoints.A.B * (BC67_WEIGHT_MAX - weights[i]) + endPoints.B.B * weights[i] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
                }
            }
        }
        #endregion Compression
    }
}
