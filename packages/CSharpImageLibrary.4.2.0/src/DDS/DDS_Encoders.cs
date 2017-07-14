using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpImageLibrary.DDS.DDSGeneral;
using static CSharpImageLibrary.DDS.DDS_BlockHelpers;
using CSharpImageLibrary.Headers;

namespace CSharpImageLibrary.DDS
{
    internal static class DDS_Encoders
    {
        #region Compressed
        internal static void CompressBC1Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            CompressRGBTexel(imgData, sourcePosition, sourceLineLength, destination, destPosition, true, (alphaSetting == AlphaSettings.RemoveAlphaChannel ? 0 : DXT1AlphaThreshold), alphaSetting, formatDetails);
        }


        internal static void CompressBC2Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            // Compress Alpha
            if (alphaSetting == AlphaSettings.RemoveAlphaChannel)
            {
                // No alpha so fill with opaque alpha - has to be an alpha value, so make it so RGB is 100% visible.
                for (int i = 0; i < 8; i++)
                    destination[destPosition + i] = 0xFF;
            }
            else
            {
                int position = sourcePosition + 3;  // Only want to read alphas
                for (int i = 0; i < 8; i += 2)
                {
                    destination[destPosition + i] = (byte)((imgData[position] & 0xF0) | (imgData[position + 4] >> 4));
                    destination[destPosition + i + 1] = (byte)((imgData[position + 8] & 0xF0) | (imgData[position + 12] >> 4));

                    position += sourceLineLength;
                }
            }
            

            // Compress Colour
            CompressRGBTexel(imgData, sourcePosition, sourceLineLength, destination, destPosition + 8, false, 0f, alphaSetting, formatDetails);
        }

        internal static void CompressBC3Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            // Compress Alpha
            if (alphaSetting == AlphaSettings.RemoveAlphaChannel)
            {
                // No alpha so fill with opaque alpha - has to be an alpha value, so make it so RGB is 100% visible.
                for (int i = 0; i < 8; i++)
                    destination[destPosition + i] = 0xFF;
            }
            else
                Compress8BitBlock(imgData, sourcePosition, sourceLineLength, destination, destPosition, 3, false, formatDetails);

            // Compress Colour
            CompressRGBTexel(imgData, sourcePosition, sourceLineLength, destination, destPosition + 8, false, 0f, alphaSetting, formatDetails);
        }

        // ATI1
        internal static void CompressBC4Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            Compress8BitBlock(imgData, sourcePosition, sourceLineLength, destination, destPosition, 2, false, formatDetails);
        }


        // ATI2 3Dc
        internal static void CompressBC5Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            // Green: Channel 1.
            Compress8BitBlock(imgData, sourcePosition, sourceLineLength, destination, destPosition, 1, false, formatDetails);

            // Red: Channel 0, 8 destination offset to be after Green.
            Compress8BitBlock(imgData, sourcePosition, sourceLineLength, destination, destPosition + 8, 2, false, formatDetails);
        }

        internal static void CompressBC6Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            BC6.CompressBC6Block(imgData, sourcePosition, sourceLineLength, destination, destPosition);
        }

        internal static void CompressBC7Block(byte[] imgData, int sourcePosition, int sourceLineLength, byte[] destination, int destPosition, AlphaSettings alphaSetting, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            BC7.CompressBC7Block(imgData, sourcePosition, sourceLineLength, destination, destPosition);
        }
        #endregion Compressed

        internal static void WriteUncompressed(byte[] source, byte[] destination, int destStart, DDS_Header.DDS_PIXELFORMAT dest_ddspf, ImageFormats.ImageEngineFormatDetails sourceFormatDetails, ImageFormats.ImageEngineFormatDetails destFormatDetails)
        {
            int byteCount = dest_ddspf.dwRGBBitCount / 8;
            bool requiresSignedAdjust = (dest_ddspf.dwFlags & DDS_Header.DDS_PFdwFlags.DDPF_SIGNED) == DDS_Header.DDS_PFdwFlags.DDPF_SIGNED;
            bool oneChannel = (dest_ddspf.dwFlags & DDS_Header.DDS_PFdwFlags.DDPF_LUMINANCE) == DDS_Header.DDS_PFdwFlags.DDPF_LUMINANCE;
            bool twoChannel = oneChannel && (dest_ddspf.dwFlags & DDS_Header.DDS_PFdwFlags.DDPF_ALPHAPIXELS) == DDS_Header.DDS_PFdwFlags.DDPF_ALPHAPIXELS;

            uint AMask = dest_ddspf.dwABitMask;
            uint RMask = dest_ddspf.dwRBitMask;
            uint GMask = dest_ddspf.dwGBitMask;
            uint BMask = dest_ddspf.dwBBitMask;

            ///// Figure out channel existance and ordering.
            // Setup array that indicates channel offset from pixel start.
            // e.g. Alpha is usually first, and is given offset 0.
            // NOTE: Ordering array is in ARGB order, and the stored indices change depending on detected channel order.
            // A negative index indicates channel doesn't exist in data and sets channel to 0xFF.

            if (dest_ddspf.dwFourCC == DDS_Header.FourCC.A32B32G32R32F)
            {
                AMask = 4;
                BMask = 3;
                GMask = 2;
                RMask = 1;
            }

            List<uint> maskOrder = new List<uint>(4) { AMask, RMask, GMask, BMask };
            maskOrder.Sort();
            maskOrder.RemoveAll(t => t == 0);  // Required, otherwise indicies get all messed up when there's only two channels, but it's not indicated as such.

            // Determine channel ordering
            int destAIndex = AMask == 0 ? -1 : maskOrder.IndexOf(AMask) * destFormatDetails.ComponentSize;
            int destRIndex = RMask == 0 ? -1 : maskOrder.IndexOf(RMask) * destFormatDetails.ComponentSize;
            int destGIndex = GMask == 0 ? -1 : maskOrder.IndexOf(GMask) * destFormatDetails.ComponentSize;
            int destBIndex = BMask == 0 ? -1 : maskOrder.IndexOf(BMask) * destFormatDetails.ComponentSize;

            int sourceAInd = 3 * sourceFormatDetails.ComponentSize;
            int sourceRInd = 2 * sourceFormatDetails.ComponentSize; 
            int sourceGInd = 1 * sourceFormatDetails.ComponentSize;
            int sourceBInd = 0;


            var sourceInds = new int[] { sourceBInd, sourceGInd, sourceRInd, sourceAInd };
            var destInds = new int[] { destBIndex, destGIndex, destRIndex, destAIndex };
            var masks = new uint[] { BMask, GMask, RMask, AMask };

            int sourceIncrement = 4 * sourceFormatDetails.ComponentSize;

            if (ImageEngine.EnableThreading)
                Parallel.For(0, source.Length / sourceIncrement, new ParallelOptions { MaxDegreeOfParallelism = ImageEngine.NumThreads }, (ind, loopState) =>
                {
                    if (ImageEngine.IsCancellationRequested)
                        loopState.Stop();

                    WriteUncompressedPixel(source, ind * sourceIncrement, sourceInds, sourceFormatDetails, masks, destination, destStart + ind * byteCount, destInds, destFormatDetails, oneChannel, twoChannel, requiresSignedAdjust);
                });
            else
                for (int i = 0; i < source.Length; i += 4 * sourceFormatDetails.ComponentSize, destStart += byteCount)
                {
                    if (ImageEngine.IsCancellationRequested)
                        break;

                    WriteUncompressedPixel(source, i, sourceInds, sourceFormatDetails, masks, destination, destStart, destInds, destFormatDetails, oneChannel, twoChannel, requiresSignedAdjust);
                }
        }

        static void WriteUncompressedPixel(byte[] source, int sourceStart, int[] sourceInds, ImageFormats.ImageEngineFormatDetails sourceFormatDetails, uint[] masks,
            byte[] destination, int destStart, int[] destInds, ImageFormats.ImageEngineFormatDetails destFormatDetails, bool oneChannel, bool twoChannel, bool requiresSignedAdjust)
        {
            if (twoChannel) // No large components - silly spec...
            {
                byte red = sourceFormatDetails.ReadByte(source, sourceStart);
                byte alpha = sourceFormatDetails.ReadByte(source, sourceStart + 3 * sourceFormatDetails.ComponentSize);

                destination[destStart] = masks[3] > masks[2] ? red : alpha;
                destination[destStart + 1] = masks[3] > masks[2] ? alpha : red;
            }
            else if (oneChannel) // No large components - silly spec...
            {
                byte blue = sourceFormatDetails.ReadByte(source, sourceStart);
                byte green = sourceFormatDetails.ReadByte(source, sourceStart + 1 * sourceFormatDetails.ComponentSize);
                byte red = sourceFormatDetails.ReadByte(source, sourceStart + 2 * sourceFormatDetails.ComponentSize);
                byte alpha = sourceFormatDetails.ReadByte(source, sourceStart + 3 * sourceFormatDetails.ComponentSize);

                destination[destStart] = (byte)(blue * 0.082 + green * 0.6094 + blue * 0.3086); // Weightings taken from ATI Compressonator. Dunno if this changes things much.
            }
            else
            {
                // Handle weird conditions where array isn't long enough...
                if (sourceInds[3] + sourceStart >= source.Length)
                    return;

                for (int i = 0; i < 4; i++)
                {
                    uint mask = masks[i];
                    if (mask != 0)
                        destFormatDetails.WriteColour(source, sourceStart + sourceInds[i], sourceFormatDetails, destination, destStart + destInds[i]);
                }

                // Signed adjustments - Only happens for bytes for now. V8U8
                if (requiresSignedAdjust)
                {
                    destination[destStart + destInds[2]] += 128;
                    destination[destStart + destInds[1]] += 128;
                }
            }
        }
    }
}
