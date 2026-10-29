using System;
using System.IO;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Shaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    /// <summary>
    /// The shader cache tests that come with game files are skipped without them, so these build
    /// a cache byte by byte instead and hold the format in place on its own.
    /// </summary>
    [TestClass]
    public class ShaderCacheTests
    {
        private const uint Persona5CacheVersion = 0x01087178;

        private static void WriteBigEndian( BinaryWriter writer, uint value )
        {
            var bytes = BitConverter.GetBytes( value );
            Array.Reverse( bytes );
            writer.Write( bytes );
        }

        private static void WriteBigEndian( BinaryWriter writer, ushort value )
        {
            var bytes = BitConverter.GetBytes( value );
            Array.Reverse( bytes );
            writer.Write( bytes );
        }

        private static byte[] BuildCache( int shaderCount )
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter( stream );

            WriteBigEndian( writer, 0x47534330u ); // GSC0
            WriteBigEndian( writer, Persona5CacheVersion );
            WriteBigEndian( writer, (uint)ResourceType.ShaderCachePS3 );
            WriteBigEndian( writer, 0u );

            for ( var i = 0; i < shaderCount; i++ )
            {
                var data = Enumerable.Range( 0, 8 ).Select( x => (byte)( i * 16 + x ) ).ToArray();

                WriteBigEndian( writer, (ushort)( i % 2 ) );   // ShaderType
                WriteBigEndian( writer, (uint)data.Length );
                WriteBigEndian( writer, (ushort)( 0xA0 + i ) ); // Field06
                WriteBigEndian( writer, 0x11111111u * (uint)( i + 1 ) );
                WriteBigEndian( writer, 0x22222222u );
                WriteBigEndian( writer, 0x33333333u );
                WriteBigEndian( writer, 1u );
                WriteBigEndian( writer, 2u );
                writer.Write( data );
            }

            return stream.ToArray();
        }

        private static ShaderCachePS3 Load( byte[] bytes )
            => Resource.Load<ShaderCachePS3>( new MemoryStream( bytes ), true );

        [TestMethod]
        public void LoadReadsEveryShader()
        {
            var cache = Load( BuildCache( 3 ) );

            Assert.AreEqual( 3, cache.Count );
            Assert.AreEqual( Persona5CacheVersion, cache.Version );
            Assert.AreEqual( Persona5CacheVersion, cache.CacheVersion );

            Assert.AreEqual( ShaderType.Vertex, cache[0].ShaderType );
            Assert.AreEqual( ShaderType.Fragment, cache[1].ShaderType );
            Assert.AreEqual( 0xA2, cache[2].Field06 );
            Assert.AreEqual( 0x33333333u, cache[1].MatFlags2 );
            CollectionAssert.AreEqual( new byte[] { 16, 17, 18, 19, 20, 21, 22, 23 }, cache[1].Data );
        }

        [TestMethod]
        public void LoadReadsAnEmptyCache()
        {
            var cache = Load( BuildCache( 0 ) );

            Assert.AreEqual( 0, cache.Count );
            Assert.AreEqual( Persona5CacheVersion, cache.Version );
        }

        [TestMethod]
        public void SaveWritesTheFileBackUnchanged()
        {
            var original = BuildCache( 3 );
            var cache = Load( original );

            var saved = new MemoryStream();
            cache.Save( saved, true );

            CollectionAssert.AreEqual( original, saved.ToArray() );
        }

        [TestMethod]
        public void SaveKeepsAnEditedShader()
        {
            var cache = Load( BuildCache( 2 ) );
            cache[1].Data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            cache[1].MatFlags0 = 0x44444444;

            var saved = new MemoryStream();
            cache.Save( saved, true );

            var reloaded = Load( saved.ToArray() );
            Assert.AreEqual( 2, reloaded.Count );
            Assert.AreEqual( 4, reloaded[1].DataLength );
            Assert.AreEqual( 0x44444444u, reloaded[1].MatFlags0 );
            CollectionAssert.AreEqual( new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, reloaded[1].Data );
        }

        /// <summary>
        /// Metaphor's cache names a resource type nothing here recognises, so it is picked out
        /// by its version instead.
        /// </summary>
        [TestMethod]
        public void LoadReadsAMetaphorCacheByItsVersion()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter( stream );

            WriteBigEndian( writer, 0x47534330u );
            WriteBigEndian( writer, ResourceVersion.MetaphorRefantazioDX11ShaderCache );
            WriteBigEndian( writer, 0xFFu ); // a resource type nothing maps
            WriteBigEndian( writer, 0u );

            var cache = Resource.Load( new MemoryStream( stream.ToArray() ), true );

            Assert.IsInstanceOfType( cache, typeof( ShaderCacheMetaphorDX11 ) );
            Assert.AreEqual( ResourceVersion.MetaphorRefantazioDX11ShaderCache, cache.Version );
        }

        [TestMethod]
        public void SaveDropsARemovedShader()
        {
            var cache = Load( BuildCache( 3 ) );
            cache.RemoveAt( 1 );

            var saved = new MemoryStream();
            cache.Save( saved, true );

            var reloaded = Load( saved.ToArray() );
            Assert.AreEqual( 2, reloaded.Count );
            Assert.AreEqual( 0xA0, reloaded[0].Field06 );
            Assert.AreEqual( 0xA2, reloaded[1].Field06 );
        }
    }
}
