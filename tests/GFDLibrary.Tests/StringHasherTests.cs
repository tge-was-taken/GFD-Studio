using System.Text;
using GFDLibrary.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    [TestClass]
    public class StringHasherTests
    {
        /// <summary>
        /// Taken from a P5 model: the name "グループ02" is stored as UTF-8 with this hash beside
        /// it, which only comes out right when the encoded bytes are hashed.
        /// </summary>
        [TestMethod]
        public void Hashes_a_non_ascii_name_the_way_the_game_does()
        {
            Assert.AreEqual( unchecked((int)0x7F64DE82), StringHasher.GenerateStringHash( "グループ02" ) );
        }

        [TestMethod]
        public void Hashes_ascii_names_unchanged()
        {
            Assert.AreEqual( unchecked((int)0x65D62AED), StringHasher.GenerateStringHash( "mat_body" ) );
            Assert.AreEqual( unchecked((int)0x76946249), StringHasher.GenerateStringHash( "Bip01" ) );
            Assert.AreEqual( unchecked((int)0xB83EFD78), StringHasher.GenerateStringHash( "eye_l" ) );
        }

        [TestMethod]
        public void An_empty_name_hashes_to_zero()
        {
            Assert.AreEqual( 0, StringHasher.GenerateStringHash( "" ) );
            Assert.AreEqual( 0, StringHasher.GenerateStringHash( (string)null ) );
        }

        [TestMethod]
        public void The_string_and_byte_overloads_agree()
        {
            const string name = "グループ02";

            Assert.AreEqual( StringHasher.GenerateStringHash( name ),
                             StringHasher.GenerateStringHash( Encoding.UTF8.GetBytes( name ) ) );
        }
    }
}
