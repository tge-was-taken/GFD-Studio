using System.Linq;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    /// <summary>
    /// Merging one model pack into another takes the textures and materials it does not already
    /// have, matched by name.
    /// </summary>
    [TestClass]
    public class MergeTests
    {
        private static ModelPack Pack( string materialName, string textureName )
        {
            var pack = new ModelPack
            {
                Version = ResourceVersion.Persona5,
                Materials = new MaterialDictionary(),
                Textures = new TextureDictionary(),
                Model = new Model { RootNode = new Node( "root" ) },
            };

            pack.Materials.Add( new Material( materialName ) );
            pack.Textures.Add( new Texture( textureName, TextureFormat.DDS, new byte[] { 1, 2, 3, 4 } ) );

            return pack;
        }

        [TestMethod]
        public void A_merge_takes_the_materials_and_textures_it_is_missing()
        {
            var into = Pack( "mat_body", "body.dds" );

            into.MergeWith( Pack( "mat_face", "face.dds" ) );

            CollectionAssert.AreEquivalent( new[] { "mat_body", "mat_face" },
                                            into.Materials.Materials.Select( x => x.Name ).ToArray() );
            CollectionAssert.AreEquivalent( new[] { "body.dds", "face.dds" },
                                            into.Textures.Textures.Select( x => x.Name ).ToArray() );
        }

        [TestMethod]
        public void A_merge_keeps_what_is_already_there()
        {
            var into = Pack( "mat_body", "body.dds" );

            into.MergeWith( Pack( "mat_body", "body.dds" ) );

            Assert.AreEqual( 1, into.Materials.Count );
            Assert.AreEqual( 1, into.Textures.Count );
        }

        [TestMethod]
        public void Merging_across_versions_is_refused()
        {
            var into = Pack( "mat_body", "body.dds" );
            var other = Pack( "mat_face", "face.dds" );
            other.Version = ResourceVersion.Persona5Royal;

            Assert.ThrowsException<System.InvalidOperationException>( () => into.MergeWith( other ) );
        }
    }
}
