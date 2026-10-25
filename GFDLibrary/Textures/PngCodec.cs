using System.IO;
using GFDLibrary.Graphics;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.Native;

namespace GFDLibrary.Textures
{
    /// <summary>
    /// PNG reading and writing, over the DirectXTex codec. That codec works on files, so the
    /// stream overloads spill to a temporary file.
    /// </summary>
    public static class PngCodec
    {
        public static Image Load( string path )
        {
            using var image = DirectXTex.ScratchImage.FromPNGFile( path );
            return DDSCodec.ToImage( image );
        }

        public static Image Load( Stream stream )
        {
            var path = Path.GetTempFileName();

            try
            {
                using ( var file = File.Create( path ) )
                    stream.CopyTo( file );

                return Load( path );
            }
            finally
            {
                File.Delete( path );
            }
        }

        public static void Save( Image image, string path )
        {
            using var scratch = DDSCodec.FromImage( image );
            scratch.SaveToPNGFile( path );
        }

        public static void Save( Image image, Stream stream )
        {
            var path = Path.GetTempFileName();

            try
            {
                Save( image, path );

                using var file = File.OpenRead( path );
                file.CopyTo( stream );
            }
            finally
            {
                File.Delete( path );
            }
        }
    }
}
