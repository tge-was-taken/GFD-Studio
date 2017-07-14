using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace UsefulThings
{
    /// <summary>
    /// General C# helpers.
    /// </summary>
    public static class General
    {
        internal static readonly char[] InvalidPathingChars;  // Characters disallowed in paths.

        static General()
        {
            // KFreon: Setup some constants
            List<char> vals = new List<char>();
            vals.AddRange(Path.GetInvalidFileNameChars());
            vals.AddRange(Path.GetInvalidPathChars());

            InvalidPathingChars = vals.ToArray(vals.Count);
        }



        /// <summary>
        /// Counts the number of bits set in a uint.
        /// From here https://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
        /// </summary>
        /// <param name="i">Number to determine set bits in.</param>
        /// <returns>Number of bits set to 1.</returns>
        public static int CountSetBits(uint i)
        {
            i = i - ((i >> 1) & 0x5555_5555);
            i = (i & 0x3333_3333) + ((i >> 2) & 0x3333_3333);
            return (int)((((i + (i >> 4)) & 0x0F0F_0F0F) * 0x0101_0101) >> 24);
        }



        /// <summary>
        /// Creates string representation of object in format:
        /// --- CLASS NAME ---
        /// Property = value
        /// ...
        /// --- END CLASS NAME ---
        /// </summary>
        /// <param name="obj">Object to get property description of.</param>
        /// <param name="IsSubClass">True = Adds whitespace before and after, and uses less ---.</param>
        /// <returns>String of object.</returns>
        public static string StringifyObject(object obj, bool IsSubClass = false)
        {
            StringBuilder sb = new StringBuilder();
            var classname = TypeDescriptor.GetClassName(obj);
            string tags = IsSubClass ? "--" : "---";

            if (IsSubClass)
                sb.AppendLine();

            sb.AppendLine($"{tags} {classname} {tags}");
            sb.AppendLine((IsSubClass ? "    " : "") + "PROPERTIES");
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
                sb.AppendLine((IsSubClass ? "    " : "") + $"{descriptor.Name} = {descriptor.GetValue(obj)}");

            sb.AppendLine((IsSubClass ? "    " : "") + "FIELDS");
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
                sb.AppendLine((IsSubClass ? "    " : "") + $"{descriptor.Name} = {descriptor.GetValue(obj)}");

            sb.AppendLine($"{tags} END {classname} {tags}");
            sb.AppendLine();

            if (IsSubClass)
                sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Gets Descriptions on Enum members.
        /// </summary>
        /// <param name="theEnum">Enum to get descriptions from.</param>
        /// <returns>Description of enum member.</returns>
        public static string GetEnumDescription(Enum theEnum)
        {
            FieldInfo info = theEnum.GetType().GetField(theEnum.ToString());
            object[] attribs = info.GetCustomAttributes(false);
            if (attribs.Length == 0)
                return theEnum.ToString();
            else
                return (attribs[0] as DescriptionAttribute)?.Description;
        }

        #region DPI
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        /// <summary>
        /// Gets mouse pointer location relative to top left of monitor, scaling for DPI as required.
        /// </summary>
        /// <param name="relative">Window on monitor.</param>
        /// <returns>Mouse location scaled for DPI.</returns>
        public static Point GetDPIAwareMouseLocation(Window relative)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            var scale = UsefulThings.General.GetDPIScalingFactorFOR_CURRENT_MONITOR(relative);
            Point location = new Point(w32Mouse.X / scale, w32Mouse.Y / scale);
            return location;
        }

        /// <summary>
        /// Gets DPI scaling factor for main monitor from registry keys. 
        /// Returns 1 if key is unavailable.
        /// </summary>
        /// <returns>Returns scale or 1 if not found.</returns>
        public static double GetDPIScalingFactorFROM_REGISTRY()
        {
            var currentDPI = (int)(Registry.GetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop\\WindowMetrics", "AppliedDPI", 96) ?? 96);
            return currentDPI / 96.0;
        }


        /// <summary>
        /// Gets DPI Scaling factor for monitor app is currently on. 
        /// NOT actual DPI, the scaling factor relative to standard 96 DPI.
        /// </summary>
        /// <param name="current">Main window to get DPI for.</param>
        /// <returns>DPI scaling factor.</returns>
        public static double GetDPIScalingFactorFOR_CURRENT_MONITOR(Window current)
        {
            PresentationSource source = PresentationSource.FromVisual(current);
            Matrix m = source.CompositionTarget.TransformToDevice;
            return m.M11;
        }

        /// <summary>
        /// Returns actual DPI of given visual object. Application DPI is constant across it's visuals.
        /// </summary>
        /// <param name="anyVisual">Any visual from the Application UI to test.</param>
        /// <returns>DPI of Application.</returns>
        public static int GetAbsoluteDPI(Visual anyVisual)
        {
            PresentationSource source = PresentationSource.FromVisual(anyVisual);
            if (source != null)
                return (int)(96.0 * source.CompositionTarget.TransformToDevice.M11);

            return 96;
        }
        #endregion DPI



        /// <summary>
        /// Does bit conversion from streams
        /// </summary>
        public static class StreamBitConverter
        {
            /// <summary>
            /// Reads a UInt32 from a stream at given offset.
            /// </summary>
            /// <param name="stream">Stream to read from.</param>
            /// <param name="offset">Offset to start reading from in stream.</param>
            /// <returns>Number read from stream.</returns>
            public static UInt32 ToUInt32(Stream stream, int offset)
            {
                // KFreon: Seek to specified offset
                byte[] fourBytes = new byte[4];
                stream.Seek(offset, SeekOrigin.Begin);

                // KFreon: Read 4 bytes from stream at offset and convert to UInt32
                stream.Read(fourBytes, 0, 4);
                UInt32 retval = BitConverter.ToUInt32(fourBytes, 0);

                // KFreon: Clear array and reset stream position
                fourBytes = null;
                return retval;
            }
        }

        /// <summary>
        /// Changes a filename in a full filepath string.
        /// </summary>
        /// <param name="fullPath">Original full filepath.</param>
        /// <param name="newFilenameWithoutExt">New filename to use.</param>
        /// <returns>Filepath with changed filename.</returns>
        public static string ChangeFilename(string fullPath, string newFilenameWithoutExt)
        {
            return fullPath.Replace(Path.GetFileNameWithoutExtension(fullPath), newFilenameWithoutExt);
        }

        /// <summary>
        /// Determines if number is a power of 2. 
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns>True if number is a power of 2.</returns>
        public static bool IsPowerOfTwo(int number)
        {
            return (number & (number - 1)) == 0;
        }


        /// <summary>
        /// Determines if number is a power of 2. 
        /// </summary>
        /// <param name="number">Number to check.</param>
        /// <returns>True if number is a power of 2.</returns>
        public static bool IsPowerOfTwo(long number)
        {
            return (number & (number - 1)) == 0;
        }


        /// <summary>
        /// Rounds number to the nearest power of 2. Doesn't use Math. Uses bitshifting (not my method).
        /// </summary>
        /// <param name="number">Number to round.</param>
        /// <returns>Nearest power of 2.</returns>
        public static int RoundToNearestPowerOfTwo(int number)
        {
            // KFreon: Gets next Highest power
            int next = number - 1;
            next |= next >> 1;
            next |= next >> 2;
            next |= next >> 4;
            next |= next >> 8;
            next |= next >> 16;
            next++;

            // KFreon: Compare previous and next for the closest
            int prev = next >> 1;
            return number - prev > next - number ? next : prev;
        }


        /// <summary>
        /// Extends on substring functionality to extract string between two other strings. e.g. ExtractString("indigo", "in", "go") == "di"
        /// </summary>
        /// <param name="str">String to extract from.</param>
        /// <param name="left">Extraction starts after this string.</param>
        /// <param name="right">Extraction ends before this string.</param>
        /// <returns>String between left and right strings.</returns>
        public static string ExtractString(string str, string left, string right)
        {
            int startIndex = str.IndexOf(left) + left.Length;
            int endIndex = str.IndexOf(right, startIndex);
            return str.Substring(startIndex, endIndex - startIndex);
        }


        /// <summary>
        /// Extends on substring functionality to extract string between a delimiter. e.g. ExtractString("I like #snuffles# and things", "#") == "snuffles"
        /// </summary>
        /// <param name="str">String to extract from.</param>
        /// <param name="enclosingElement">Element to extract between. Must be present twice in str.</param>
        /// <returns>String between two enclosingElements.</returns>
        public static string ExtractString(string str, string enclosingElement)
        {
            return ExtractString(str, enclosingElement, enclosingElement);
        }


        #region Stream Compression/Decompression
        /// <summary>
        /// Decompresses stream using GZip. Returns decompressed Stream.
        /// Returns null if stream isn't compressed.
        /// </summary>
        /// <param name="compressedStream">Stream compressed with GZip.</param>
        public static MemoryStream DecompressStream(Stream compressedStream)
        {
            MemoryStream newStream = new MemoryStream();
            compressedStream.Seek(0, SeekOrigin.Begin);

            GZipStream Decompressor = null;
            try
            {
                Decompressor = new GZipStream(compressedStream, CompressionMode.Decompress, true);
                Decompressor.CopyTo(newStream);
            }
            catch (InvalidDataException invdata)
            {
                return null;
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if (Decompressor != null)
                    Decompressor.Dispose();
            }
            
            return newStream;
        }


        /// <summary>
        /// Compresses stream with GZip. Returns new compressed stream.
        /// </summary>
        /// <param name="DecompressedStream">Stream to compress.</param>
        /// <param name="compressionLevel">Level of compression to use.</param>
        public static MemoryStream CompressStream(Stream DecompressedStream, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            MemoryStream ms = new MemoryStream();
            using (GZipStream Compressor = new GZipStream(ms, compressionLevel, true))
            {
                DecompressedStream.Seek(0, SeekOrigin.Begin);
                DecompressedStream.CopyTo(Compressor);
            }

            return ms;
        }
        #endregion Stream Compression/Decompression
        


        /// <summary>
        /// Converts given double to filesize with appropriate suffix.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <param name="FullSuffix">True = Bytes, KiloBytes, etc. False = B, KB, etc</param>
        public static string GetFileSizeAsString(double size, bool FullSuffix = false)
        {
            string[] sizes = null;
            if (FullSuffix)
                sizes = new string[] { "Bytes", "Kilobytes", "Megabytes", "Gigabytes" };
            else
                sizes = new string[] { "B", "KB", "MB", "GB" };
            
            int order = 0;
            while (size >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                size = size / 1024;
            }

            return size.ToString("F1") + " " + sizes[order];
        }


        /// <summary>
        /// Gets file extensions as filter string for SaveFileDialog and OpenFileDialog as a SINGLE filter entry.
        /// </summary>
        /// <param name="exts">List of extensions to use.</param>
        /// <param name="filterName">Name of filter entry. e.g. 'Images|*.jpg;*.bmp...', Images is the filter name</param>
        /// <returns>Filter string from extensions.</returns>
        public static string GetExtsAsFilter(List<string> exts, string filterName)
        {
            StringBuilder sb = new StringBuilder(filterName + "|");
            foreach (string str in exts)
                sb.Append("*" + str + ";");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Gets file extensions as filter string for SaveFileDialog and OpenFileDialog as MULTIPLE filter entries.
        /// </summary>
        /// <param name="exts">List of file extensions. Must have same number as filterNames.</param>
        /// <param name="filterNames">List of file names. Must have same number as exts.</param>
        /// <returns>Filter string of names and extensions.</returns>
        public static string GetExtsAsFilter(List<string> exts, List<string> filterNames)
        {
            // KFreon: Flip out if number of extensions is different to number of names of said extensions
            if (exts.Count != filterNames.Count)
                return null;

            // KFreon: Build filter string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < exts.Count; i++)
                sb.Append(filterNames[i] + "|*" + exts[i] + "|");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Gets version of assembly calling this function.
        /// </summary>
        /// <returns>String of assembly version.</returns>
        public static string GetCallingVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Gets version of main assembly that started this process.
        /// </summary>
        /// <returns></returns>
        public static string GetStartingVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Gets location of assembly calling this function.
        /// </summary>
        /// <returns>Path to location.</returns>
        public static string GetExecutingLoc()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }


        #region File IO
        /// <summary>
        /// Read text from file as single string.
        /// </summary>
        /// <param name="filename">Path to filename.</param>
        /// <param name="result">Contents of file.</param>
        /// <returns>Null if successful, error as string otherwise.</returns>
        public static string ReadTextFromFile(string filename, out string result)
        {
            result = null;
            string err = null;

            // Try to read file, but fail safely if necessary
            try
            {
                if (filename.isFile())
                    result = File.ReadAllText(filename);
                else
                    err = "Not a file.";
            }
            catch (Exception e)
            {
                err = e.Message;
            }

            return err;
        }


        /// <summary>
        /// Reads lines of file into List.
        /// </summary>
        /// <param name="filename">File to read from.</param>
        /// <param name="Lines">Contents of file.</param>
        /// <returns>Null if success, error message otherwise.</returns>
        public static string ReadLinesFromFile(string filename, out List<string> Lines)
        {
            Lines = null;
            string err = null;

            try
            {
                // KFreon: Only bother if it is a file
                if (filename.isFile())
                {
                    string[] lines = File.ReadAllLines(filename);
                    Lines = lines.ToList(lines.Length);
                }
                    
                else
                    err = "Not a file.";
            }
            catch (Exception e)
            {
                err = e.Message;
            }

            return err;
        }
        
         /// <summary>
        /// Gets external image data as byte[] with some buffering i.e. retries if fails up to 20 times.
        /// </summary>
        /// <param name="file">File to get data from.</param>
        /// <param name="OnFailureSleepTime">Time (in ms) between attempts for which to sleep.</param>
        /// <param name="retries">Number of attempts to read.</param>
        /// <returns>byte[] of image.</returns>
        public static byte[] GetExternalData(string file, int retries = 20, int OnFailureSleepTime = 300)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    // KFreon: Try readng file to byte[]
                    return File.ReadAllBytes(file);
                }
                catch (IOException e)
                {
                    // KFreon: Sleep for a bit and try again
                    System.Threading.Thread.Sleep(OnFailureSleepTime);
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Failed to get external data: {e.Message}");
                }
            }
            return null;
        }


        /// <summary>
        /// Tests given file path for existence. If exists, adjusts filename until the new filename doesn't exist.
        /// </summary>
        /// <param name="baseName">Original desired name.</param>
        /// <returns>Filename based on original that doesn't already exist.</returns>
        public static string FindValidNewFileName(string baseName)
        {
            if (!baseName.isFile())
                throw new ArgumentOutOfRangeException($"{nameof(baseName)} must be a testable file path, not a directory path.");

            if (!File.Exists(baseName))
                return baseName;

            int count = 1;
            string ext = Path.GetExtension(baseName);
            string pathWithoutExtension = GetFullPathWithoutExtension(baseName);

            // Detect if a similar path was provided i.e. <path>_#.ext - Remove the _# and start incrementation at #.
            char last = pathWithoutExtension.Last();
            if (pathWithoutExtension[pathWithoutExtension.Length - 2] == '_' && last.isDigit())
            {
                count = int.Parse(last + "");
                pathWithoutExtension = pathWithoutExtension.Substring(0, pathWithoutExtension.Length - 2);
            }

            string tempName = pathWithoutExtension;
            while(File.Exists(tempName + ext))
            {
                tempName = pathWithoutExtension;
                tempName += "_" + count;
                count++;
            }

            return tempName + ext;
        }

        /// <summary>
        /// Gets a full file path (not just the name) without the file extension.
        /// </summary>
        /// <param name="fullPath">Full path to remove extension from.</param>
        /// <returns>File path without extension.</returns>
        public static string GetFullPathWithoutExtension(string fullPath)
        {
            if (!fullPath.isFile())
                throw new ArgumentOutOfRangeException($"{nameof(fullPath)} must be a testable file path, not a directory path.");

            return fullPath.Substring(0, fullPath.LastIndexOf('.'));
        }
        #endregion File IO


        static string[] CapitalExcluded = new string[] { "in", "the", "at" };
        /// <summary>
        /// Capitalises all words in a string unless they're joining words (in, the, at)
        /// </summary>
        /// <param name="str">String to capitalise.</param>
        /// <returns>Capitalised string.</returns>
        public static string CapitaliseString(string str)
        {
            StringBuilder sb = new StringBuilder();

            // Idea here is that we split up all words, capitalise all starting chars except the words in the CapitalExcluded list, unless those words are the first word.
            string[] words = str.Split(' ');
            bool first = true;
            foreach (var word in words)
            {
                // Don't capitalise certain words unless they're first
                if (!first && CapitalExcluded.Contains(word, StringComparison.OrdinalIgnoreCase))
                    continue;

                sb.Append(CapitaliseWord(word));
                sb.Append(' ');
            }

            return sb.ToString();
        }

        static string CapitaliseWord(string word)
        {
            if (String.IsNullOrEmpty(word))
                return "";

            char first = word[0];

            // Check case
            char caps = char.ToUpper(first);
            if (caps == first)
                return word;
            else
                return caps + word.Substring(1);
        }

        static void CapitaliseWord(string word, StringBuilder destination)
        {
            if (String.IsNullOrEmpty(word))
                return;

            char first = word[0];

            // Check case
            char caps = char.ToUpper(first);
            if (caps == first)
                destination.Append(word);
            else
                destination.Append(caps + word.Substring(1));
        }
    }
}
