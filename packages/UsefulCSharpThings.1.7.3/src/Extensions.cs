using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UsefulThings.WPF;

namespace UsefulThings
{
    /// <summary>
    /// Extension methods for various things, both WPF and WinForms
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Performs Distinct on a particular property. Credit: http://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property
        /// </summary>
        /// <typeparam name="TSource">Type of items.</typeparam>
        /// <typeparam name="TKey">Parameter to filter on.</typeparam>
        /// <param name="source">Enumerable to make distinct.</param>
        /// <param name="keySelector">Selector to chose property to make distinct.</param>
        /// <returns>Enumerable distinct on keySelector.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }


        #region Arrays
        /// <summary>
        /// Extracts a sub array from another array with a specified number of elements.
        /// </summary>
        /// <typeparam name="T">Content of array.</typeparam>
        /// <param name="oldArray">Current array.</param>
        /// <param name="offset">Start index in oldArray.</param>
        /// <param name="length">Length to extract.</param>
        /// <returns>New array containing elements within the specified range.</returns>
        public static T[] GetRange<T>(this T[] oldArray, int offset, int length)
        {
            T[] newArray = new T[length];
            Array.Copy(oldArray, offset, newArray, 0, length);
            return newArray;
        }


        /// <summary>
        /// Extracts a sub array from another array starting at offset and reading to end.
        /// </summary>
        /// <typeparam name="T">Content of array.</typeparam>
        /// <param name="oldArray">Current array.</param>
        /// <param name="offset">Start index in oldArray.</param>
        /// <returns>New array containing elements within the specified range.</returns>
        public static T[] GetRange<T>(this T[] oldArray, int offset)
        {
            return oldArray.GetRange(offset, oldArray.Length - offset);
        }
        #endregion Arrays


        #region Stream IO 
        #region Reading
        /// <summary>
        /// Write data to this stream at the current position from another stream at it's current position.
        /// </summary>
        /// <param name="TargetStream">Stream to copy from.</param>
        /// <param name="SourceStream">Stream to copy to.</param>
        /// <param name="Length">Number of bytes to read.</param>
        /// <param name="bufferSize">Size of buffer to use while copying.</param>
        /// <returns>Number of bytes read.</returns>
        public static int ReadFrom(this Stream TargetStream, Stream SourceStream, long Length, int bufferSize = 4096)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            int numRead = 0;
            do
            {
                read = SourceStream.Read(buffer, 0, (int)Math.Min(bufferSize, Length));
                if (read == 0)
                    break;
                Length -= read;
                TargetStream.Write(buffer, 0, read);
                numRead += read;

            } while (Length > 0);

            return numRead;
        }

        /// <summary>
        /// Reads an int from stream at the current position and advances 4 bytes.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Integer read from stream.</returns>
        public static int ReadInt32(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadInt32();
        }

        /// <summary>
        /// Reads an int from stream at the current position and advances 4 bytes.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Unsigned integer read from stream.</returns>
        public static uint ReadUInt32(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadUInt32();
        }

        /// <summary>
        /// Reads an uint from stream at the current position and advances 4 bytes.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Integer read from stream.</returns>
        public static uint ReadUInt32FromStream(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadUInt32();
        }

        
        /// <summary>
        /// Reads a long from stream at the current position.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Long read from stream.</returns>
        public static long ReadInt64FromStream(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadInt64();
        }

        /// <summary>
        /// Reads a number of bytes from stream at the current position and advances that number of bytes.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="Length">Number of bytes to read.</param>
        /// <returns>Bytes read from stream.</returns>
        public static byte[] ReadBytes(this Stream stream, int Length)
        {
            byte[] bytes = new byte[Length];
            stream.Read(bytes, 0, Length);
            return bytes;
        }


        /// <summary>
        /// Reads a string from a stream. Must be null terminated or have the length written at the start (Pascal strings or something?)
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="HasLengthWritten">True = Attempt to read string length from stream first.</param>
        /// <returns>String read from stream.</returns>
        public static string ReadString(this Stream stream, bool HasLengthWritten = false)
        {
            if (stream == null || !stream.CanRead)
                throw new IOException("Stream cannot be read.");

            int length = -1;
            List<char> chars = new List<char>();
            if (HasLengthWritten)
            {
                length = stream.ReadInt32();
                for (int i = 0; i < length; i++)
                    chars.Add((char)stream.ReadByte());
            }
            else
            {
                char c = 'a';
                while ((c = (char)stream.ReadByte()) != '\0')
                {
                    chars.Add(c);
                }
            }

            return new String(chars.ToArray(chars.Count));
        }


        /// <summary>
        /// KFreon: Borrowed this from the DevIL C# Wrapper found here: https://code.google.com/p/devil-net/
        /// 
        /// Reads a stream until the end is reached into a byte array. Based on
        /// <a href="http://www.yoda.arachsys.com/csharp/readbinary.html">Jon Skeet's implementation</a>.
        /// It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read all bytes from</param>
        /// <param name="initialLength">Initial buffer length, default is 32K</param>
        /// <returns>The byte array containing all the bytes from the stream</returns>
        public static byte[] ReadStreamFully(this Stream stream, int initialLength = 32768)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (initialLength < 1)
            {
                initialLength = 32768; //Init to 32K if not a valid initial length
            }

            byte[] buffer = new byte[initialLength];
            int position = 0;
            int chunk;

            while ((chunk = stream.Read(buffer, position, buffer.Length - position)) > 0)
            {
                position += chunk;

                //If we reached the end of the buffer check to see if there's more info
                if (position == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    //If -1 we reached the end of the stream
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    //Not at the end, need to resize the buffer
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[position] = (byte)nextByte;
                    buffer = newBuffer;
                    position++;
                }
            }

            //Trim the buffer before returning
            byte[] toReturn = new byte[position];
            Array.Copy(buffer, toReturn, position);
            return toReturn;
        }
        #endregion Reading

        #region Writing
        /// <summary>
        /// Writes byte array to current position in stream.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="data">Data to write to stream.</param>
        public static void WriteBytes(this Stream stream, byte[] data)
        {
            if (!stream.CanWrite)
                throw new IOException("Stream is read only.");

            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a long (int64) to stream at its current position.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Int64 to write to stream.</param>
        public static void WriteInt64(this Stream stream, long value)
        {
            using (BinaryWriter br = new BinaryWriter(stream, Encoding.Default, true))
                br.Write(value);
        }


        /// <summary>
        /// FROM GIBBED.
        /// Writes an int to stream at the current position.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Integer to write.</param>
        public static void WriteInt32(this Stream stream, int value)
        {
            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.Default, true))
                bw.Write(value);
        }
        

        /// <summary>
        /// FROM GIBBED.
        /// Writes uint to stream at current position.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">uint to write.</param>
        public static void WriteUInt32(this Stream stream, uint value)
        {
            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.Default, true))
                bw.Write(value);
        }

        /// <summary>
        /// FROM GIBBED.
        /// Writes a float to stream at the current position.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Float to write to stream.</param>
        public static void WriteFloat32(this Stream stream, float value)
        {
            if (stream?.CanWrite != true)
                throw new IOException("Stream is null or read only.");

            stream.WriteBytes(BitConverter.GetBytes(value));
        }
        
        /// <summary>
        /// Writes string to stream. Terminated by a null char, and optionally writes string length at start of string. (Pascal strings?)
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="str">String to write.</param>
        /// <param name="WriteLength">True = Writes str length before writing string.</param>
        public static void WriteString(this Stream stream, string str, bool WriteLength = false)
        {
            if (WriteLength)
                stream.WriteInt32(str.Length);
                
            foreach (char c in str)
                stream.WriteByte((byte)c);
                
            stream.WriteByte((byte)'\0');
        }

        #endregion Writing
        #endregion Stream IO


        #region All Collections e.g. List, Dictionary
        /// <summary>
        /// Sorts list using Natural Compare i.e. 1 is before 12, etc
        /// </summary>
        /// <param name="list"></param>
        public static void NaturalSort(this List<string> list)
        {
            NaturalSortComparer comparer = new NaturalSortComparer();
            list.Sort(comparer);
        }


        /// <summary>
        /// Returns index of minimum value based on comparer.
        /// </summary>
        /// <param name="enumerable">Collection to search.</param>
        /// <param name="comparer">Comparer to use. e.g. item => item - x</param>
        /// <returns>Index of minimum value in enumerable based on comparer.</returns>
        public static int IndexOfMin(this IEnumerable<int> enumerable, Func<int, int> comparer)
        {
            int min = int.MaxValue;
            int index = 0;
            int minIndex = 0;
            foreach (int item in enumerable)
            {
                int check = comparer(item);
                if (check < min)
                {
                    min = check;
                    minIndex = index;
                }

                index++;
            }
            return minIndex;
        }

        public static void SIMDMinMax(this int[] input, out int min, out int max)
        {
            min = 0;
            max = 0;
        }

        public static void SIMDMinMax(this ushort[] input, out ushort min, out ushort max)
        {
            min = 0;
            max = 0;
        }

        public static void SIMDMinMax(this double[] input, out double min, out double max)
        {
            //if (!System.Numerics.Vector.IsHardwareAccelerated)
              //  throw new InvalidOperationException("SIMD is not supported. Ensure x64 build (not 'Prefer 32 bit') and 'Optimise Code' is selected.");

            int simdLength = Vector<double>.Count;
            int i = 0;
            var vmin = new Vector<double>(double.MaxValue);
            var vmax = new Vector<double>(double.MinValue);

            // Get min/max sub-arrays
            for (i = 0; i <= input.Length - simdLength; i += simdLength)
            {
                var va = new Vector<double>(input, i);
                vmin = System.Numerics.Vector.Min(va, vmin);
                vmax = System.Numerics.Vector.Max(va, vmax);
            }

            // Find min/max of sub-arrays
            min = double.MaxValue;
            max = double.MinValue;
            for (int j = 0; j < simdLength; j++)
            {
                min = Math.Min(min, vmin[j]);
                max = Math.Max(max, vmax[j]);
            }


            // Do the extra bits if it's not a multiple of Vector<T>.Count
            for (; i < input.Length; i++)
            {
                min = Math.Min(min, input[i]);
                max = Math.Max(max, input[i]);
            }
        }

        public static void SIMDMinMax(this byte[] input, out byte min, out byte max)
        {
            //if (!System.Numerics.Vector.IsHardwareAccelerated)
            //  throw new InvalidOperationException("SIMD is not supported. Ensure x64 build (not 'Prefer 32 bit') and 'Optimise Code' is selected.");

            int simdLength = Vector<byte>.Count;
            int i = 0;
            var vmin = new Vector<byte>(byte.MaxValue);
            var vmax = new Vector<byte>(byte.MinValue);

            // Get min/max sub-arrays
            for (i = 0; i <= input.Length - simdLength; i += simdLength)
            {
                var va = new Vector<byte>(input, i);
                vmin = System.Numerics.Vector.Min(va, vmin);
                vmax = System.Numerics.Vector.Max(va, vmax);
            }

            // Find min/max of sub-arrays
            min = byte.MaxValue;
            max = byte.MinValue;
            for (int j = 0; j < simdLength; j++)
            {
                min = Math.Min(min, vmin[j]);
                max = Math.Max(max, vmax[j]);
            }


            // Do the extra bits if it's not a multiple of Vector<T>.Count
            for (; i < input.Length; i++)
            {
                min = Math.Min(min, input[i]);
                max = Math.Max(max, input[i]);
            }
        }


        /// <summary>
        /// Returns index of minimum value based on comparer.
        /// </summary>
        /// <param name="enumerable">Collection to search.</param>
        /// <param name="comparer">Comparer to use. e.g. item => item - x</param>
        /// <returns>Index of minimum value in enumerable based on comparer.</returns>
        public static int IndexOfMin(this IEnumerable<byte> enumerable, Func<byte, int> comparer)
        {
            int min = int.MaxValue;
            int index = 0;
            int minIndex = 0;
            foreach (byte item in enumerable)
            {
                int check = comparer(item);
                if (check < min)
                {
                    min = check;
                    minIndex = index;
                }

                index++;
            }
            return minIndex;
        }


        /// <summary>
        /// Adds elements of a Dictionary to another Dictionary. No checking for duplicates.
        /// </summary>
        /// <typeparam name="T">Key.</typeparam>
        /// <typeparam name="U">Value.</typeparam>
        /// <param name="mainDictionary">Dictionary to add to.</param>
        /// <param name="newAdditions">Dictionary of elements to be added.</param>
        public static void AddRange<T, U>(this Dictionary<T, U> mainDictionary, Dictionary<T, U> newAdditions)
        {
            if (newAdditions == null)
                throw new ArgumentNullException();

            foreach (var item in newAdditions)
                mainDictionary.Add(item.Key, item.Value);
        }


        /// <summary>
        /// Add range of elements to given collection.
        /// </summary>
        /// <typeparam name="T">Type of items in collection.</typeparam>
        /// <param name="collection">Collection to add to.</param>
        /// <param name="additions">Elements to add.</param>
        public static void AddRangeKinda<T>(this ConcurrentBag<T> collection, IEnumerable<T> additions)
        {
            foreach (var item in additions)
                collection.Add(item);
        }


        /// <summary>
        /// Add range of elements to given collection.
        /// </summary>
        /// <typeparam name="T">Type of items in collection.</typeparam>
        /// <param name="collection">Collection to add to.</param>
        /// <param name="additions">Elements to add.</param>
        public static void AddRangeKinda<T>(this ICollection<T> collection, IEnumerable<T> additions)
        {
            foreach (var item in additions)
                collection.Add(item);
        }
        
        /// <summary>
        /// Removes element from collection at index.
        /// </summary>
        /// <typeparam name="T">Type of objects in collection.</typeparam>
        /// <param name="collection">Collection to remove from.</param>
        /// <param name="index">Index to remove from.</param>
        /// <returns>Removed element.</returns>
        public static T Pop<T>(this ICollection<T> collection, int index)
        {
            T item = collection.ElementAt(index);
            collection.Remove(item);
            return item;
        }


        /// <summary>
        /// Converts enumerable to List in a more memory efficient way by providing size of list.
        /// </summary>
        /// <typeparam name="T">Type of elements in lists.</typeparam>
        /// <param name="enumerable">Enumerable to convert to list.</param>
        /// <param name="size">Size of list.</param>
        /// <returns>List containing enumerable contents.</returns>
        public static List<T> ToList<T>(this IEnumerable<T> enumerable, int size)
        {
            return new List<T>(enumerable);
        }


        /// <summary>
        /// Converts enumerable to array in a more memory efficient way by providing size of list.
        /// </summary>
        /// <typeparam name="T">Type of elements in list.</typeparam>
        /// <param name="enumerable">Enumerable to convert to array.</param>
        /// <param name="size">Size of lists.</param>
        /// <returns>Array containing enumerable elements.</returns>
        public static T[] ToArray<T>(this IEnumerable<T> enumerable, int size)
        {
            T[] newarr = new T[size];
            int count = 0;

            foreach (T item in enumerable)
                newarr[count++] = item;

            return newarr;
        }
        #endregion All Collections e.g. List, Dictionary


        #region Strings
        /// <summary>
        /// Splits string on (possibly) multiple elements.
        /// </summary>
        /// <param name="str">String to split.</param>
        /// <param name="options">Options to use while splitting.</param>
        /// <param name="splitStrings">Elements to split string on. (Delimiters)</param>
        /// <returns></returns>
        public static string[] Split(this string str, StringSplitOptions options, params string[] splitStrings)
        {
            return str.Split(splitStrings, options);
        }
        

        /// <summary>
        /// Compares strings with culture and case sensitivity.
        /// </summary>
        /// <param name="str">Main string to check in.</param>
        /// <param name="toCheck">Substring to check for in Main String.</param>
        /// <param name="CompareType">Type of comparison.</param>
        /// <returns>True if toCheck found in str, false otherwise.</returns>
        public static bool Contains(this String str, string toCheck, StringComparison CompareType)
        {
            return str.IndexOf(toCheck, CompareType) >= 0;
        }


        /// <summary>
        /// Removes invalid characters from path.
        /// </summary>
        /// <param name="str">String to remove chars from.</param>
        /// <returns>New string containing no invalid characters.</returns>
        public static string GetPathWithoutInvalids(this string str)
        {
            StringBuilder newstr = new StringBuilder(str);
            foreach (char c in General.InvalidPathingChars)
                newstr.Replace(c + "", "");

            return newstr.ToString();
        }


        /// <summary>
        /// Gets parent directory, optionally to a certain depth (or height?)
        /// </summary>
        /// <param name="str">String (hopefully path) to get parent of.</param>
        /// <param name="depth">Depth to get parent of.</param>
        /// <returns>Parent of string.</returns>
        public static string GetDirParent(this string str, int depth = 1)
        {
            string retval = null;

            try
            {
                // Strip directory separators before starting or getdirname will say that C:\users is the parent of c:\users\
                string workingString = str.Trim(Path.DirectorySeparatorChar);                
                retval = Path.GetDirectoryName(workingString);

                for (int i = 1; i < depth; i++)
                    retval = Path.GetDirectoryName(retval);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to get parent directory: " + e.Message);
            }

            return retval;
        }

        /// <summary>
        /// Determines if string is a Directory.
        /// Returns True if directory, false otherwise.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if is a directory, false if not.</returns>
        public static bool isDirectory(this string str)
        {
            // KFreon: Check if things exist first
            if (str == null || !File.Exists(str) && !Directory.Exists(str))
                return false;


            FileAttributes attr = File.GetAttributes(str);
            if (attr.HasFlag(FileAttributes.Directory))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if string is a file.
        /// Returns True if file, false otherwise.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if a file, false if not</returns>
        public static bool isFile(this string str)
        {
            return !str.isDirectory();
        }
        #endregion Strings


        #region Digit or letter determination
        /// <summary>
        /// Determines if string is a number.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if string is a number.</returns>
        public static bool isDigit(this string str)
        {
            int res = -1;
            return Int32.TryParse(str, out res);
        }
        
        /// <summary>
        /// Determines if character is a number.
        /// </summary>
        /// <param name="c">Character to check.</param>
        /// <returns>True if c is a number.</returns>
        public static bool isDigit(this char c)
            {
            return ("" + c).isDigit();
            }


        /// <summary>
        /// Determines if character is a letter.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool isLetter(this char c)
            {
            return !c.isDigit();
        }      


        /// <summary>
        /// Determines if string is a letter.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if str is a letter.</returns>
        public static bool isLetter(this string str)
                {
            if (str.Length == 1)
                return !str.isDigit();

            return false;
                    }
        #endregion Digit or letter determination
        

        #region WPF Documents
        /// <summary>
        /// Adds text to a FixedPage.
        /// </summary>
        /// <param name="page">Page to add text to.</param>
        /// <param name="text">Text to add.</param>
        public static void AddTextToPage(this FixedPage page, string text)
        {
            TextBlock block = new TextBlock();
            block.Inlines.Add(text);
            page.Children.Add(block);
        }


        /// <summary>
        /// Add page to a FixedDocument from string.
        /// </summary>
        /// <param name="document">Document to add to.</param>
        /// <param name="text">Text to add as page.</param>
        public static void AddPageFromText(this FixedDocument document, string text)
        {
            PageContent page = WPF.Documents.GeneratePageFromText(text);
            document.Pages.Add(page);
        }

        
        /// <summary>
        /// Add page to a FixedDocument from a file.
        /// </summary>
        /// <param name="document">Document to add to.</param>
        /// <param name="filename">Filename to load from.</param>
        /// <returns>Null if successful, error as string otherwise.</returns>
        public static string AddPageFromFile(this FixedDocument document, string filename)
        {
            string retval = null;
            PageContent page = WPF.Documents.GeneratePageFromFile(filename, out retval);
            document.Pages.Add(page);
            return retval;
        }
        #endregion WPF Documents


        #region Misc
        /// <summary>
        /// A simple WPF threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// Which can be used with DispatcherObject types
        /// </summary>
        /// <param name="disp">The Dispatcher object on which to do the Invoke</param>
        /// <param name="dotIt">The delegate to run</param>
        /// <param name="priority">The DispatcherPriority</param>
        public static void InvokeIfRequired(this Dispatcher disp,
            Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
                disp.Invoke(priority, dotIt);
            else
                dotIt();
        }

        /// <summary>
        /// Returns pixels of image as RGBA channels in a stream. (R, G, B, A). 1 byte each.
        /// Allows writing.
        /// </summary>
        /// <param name="bmp">Image to extract pixels from.</param>
        /// <returns>RGBA channels as stream.</returns>
        public static MemoryStream GetPixelsAsStream(this BitmapSource bmp)
        {
            return new MemoryStream(bmp.GetPixels(), true);
        }


        /// <summary>
        /// Gets pixels of image as byte[].
        /// </summary>
        /// <param name="bmp">Image to extract pixels from.</param>
        /// <returns>Pixels of image.</returns>
        public static byte[] GetPixels(this BitmapSource bmp)
        {
            // KFreon: Read pixel data from image.
            bool hasAlpha = bmp.Format.ToString().Contains("a", StringComparison.OrdinalIgnoreCase);
            int size = (int)((hasAlpha ? 4 : 3) * bmp.PixelWidth * bmp.PixelHeight);
            byte[] pixels = new byte[size];
            int stride = (int)bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
            bmp.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        /// <summary>
        /// Gets pixels of image as byte[] formatted as BGRA32.
        /// </summary>
        /// <param name="bmp">Bitmap to extract pixels from. Can be any supported pixel format.</param>
        /// <returns>Pixels as BGRA32.</returns>
        public static byte[] GetPixelsAsBGRA32(this BitmapSource bmp)
        {
            // KFreon: Read pixel data from image.
            int size = (int)(4 * bmp.PixelWidth * bmp.PixelHeight);
            byte[] pixels = new byte[size];
            BitmapSource source = bmp;

            // Convert if required.
            if (bmp.Format != PixelFormats.Bgra32)
            {
                Debug.WriteLine($"Getting pixels as BGRA32 required conversion from: {bmp.Format}.");
                bmp = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent, 0);
            }

            int stride = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
            bmp.CopyPixels(pixels, stride, 0);
            return pixels;
        }


        /// <summary>
        /// Begins an animation that automatically sets final value to be held. Used with FillType.Stop rather than default FillType.Hold.
        /// </summary>
        /// <param name="element">Content Element to animate.</param>
        /// <param name="anim">Animation to use on element.</param>
        /// <param name="dp">Property of element to animate using anim.</param>
        /// <param name="To">Final value of element's dp.</param>
        public static void BeginAdjustableAnimation(this ContentElement element, DependencyProperty dp, GridLengthAnimation anim, object To)
        {
            if (dp.IsValidType(To))
            {
                element.SetValue(dp, To);
                element.BeginAnimation(dp, anim);
            }
            else
            {
                throw new Exception("To object value passed is of the wrong Type. Given: " + To.GetType() + "  Expected: " + dp.PropertyType);
            }
        }


        /// <summary>
        /// Begins adjustable animation for a GridlengthAnimation. 
        /// Holds animation end value without Holding it. i.e. Allows it to change after animation without resetting it. Should be possible in WPF...maybe it is.
        /// </summary>
        /// <param name="element">Element to start animation on.</param>
        /// <param name="dp">Property to animate.</param>
        /// <param name="anim">Animation to perform. GridLengthAnimation only for now.</param>
        public static void BeginAdjustableAnimation(this ContentElement element, DependencyProperty dp, GridLengthAnimation anim)
        {
            element.BeginAdjustableAnimation(dp, anim, anim.To);
        }


        /// <summary>
        /// Randomly shuffles a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        /// <summary>
        /// Gets all text from a document.
        /// </summary>
        /// <param name="document">FlowDocument to extract text from.</param>
        /// <returns>All text as a string.</returns>
        public static string GetText(this FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }


        /// <summary>
        /// Gets drag/drop data as string[].
        /// </summary>
        /// <param name="e">Argument from Drop Handler.</param>
        /// <returns>Contents of drop.</returns>
        public static string[] GetDataAsStringArray(this DragEventArgs e)
        {
            return (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
        }
        #endregion Misc


        /// <summary>
        /// Tests if <paramref name="item"/> is contained within <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">Enumerable to check.</param>
        /// <param name="item">Item to search for.</param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<string> enumerable, string item, StringComparison comparisonType)
        {
            return enumerable.Contains(item, new StringCaselessComparer(comparisonType));
        }

        class StringCaselessComparer : IEqualityComparer<string>
        {
            static StringComparison ComparisonType = StringComparison.OrdinalIgnoreCase;

            public StringCaselessComparer(StringComparison comparisonType)
            {
                ComparisonType = comparisonType;
            }

            public bool Equals(string x, string y)
            {
                return String.Equals(x, y, ComparisonType);
            }

            public int GetHashCode(string obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
