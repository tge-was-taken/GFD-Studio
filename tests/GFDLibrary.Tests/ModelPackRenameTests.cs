using System.Linq;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    /// <summary>
    /// A mesh names its material and a material names its textures by string, so a rename that
    /// does not follow through leaves the model shading black.
    /// </summary>
    [TestClass]
    public class ModelPackRenameTests
    {
        private static ModelPack Pack()
        {
            var mesh = new Mesh { MaterialName = "mat_body" };
            var other = new Mesh { MaterialName = "mat_face" };

            var node = new Node( "root" );
            node.Attachments.Add( NodeAttachment.Create( mesh ) );
            node.Attachments.Add( NodeAttachment.Create( other ) );

            var material = new Material( "mat_body" ) { DiffuseMap = new TextureMap( "body.dds" ) };

            var pack = new ModelPack { Model = new Model { RootNode = node }, Materials = new MaterialDictionary() };
            pack.Materials.Add( material );

            return pack;
        }

        [TestMethod]
        public void Renaming_a_material_follows_through_to_its_meshes()
        {
            var pack = Pack();

            pack.RenameMaterialReferences( "mat_body", "mat_torso" );

            CollectionAssert.AreEquivalent( new[] { "mat_torso", "mat_face" },
                                            pack.Model.Meshes.Select( x => x.MaterialName ).ToArray() );
        }

        [TestMethod]
        public void Renaming_a_texture_follows_through_to_the_texture_maps()
        {
            var pack = Pack();

            pack.RenameTextureReferences( "body.dds", "torso.dds" );

            Assert.AreEqual( "torso.dds", pack.Materials.Materials.Single().DiffuseMap.Name );
        }

        [TestMethod]
        public void A_rename_that_changes_nothing_is_left_alone()
        {
            var pack = Pack();

            pack.RenameMaterialReferences( "mat_body", "mat_body" );
            pack.RenameMaterialReferences( "", "mat_torso" );
            pack.RenameMaterialReferences( null, "mat_torso" );

            CollectionAssert.AreEquivalent( new[] { "mat_body", "mat_face" },
                                            pack.Model.Meshes.Select( x => x.MaterialName ).ToArray() );
        }
    }
}
