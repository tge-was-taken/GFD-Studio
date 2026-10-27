using System.IO;
using System.Linq;
using GFDLibrary.Common;
using GFDLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    /// <summary>
    /// Exporting a model to YAML and reading it back has to give the same tree. The children
    /// used to be written out and then dropped on the way back in.
    /// </summary>
    [TestClass]
    public class YamlModelTests
    {
        private static Model Roundtrip( Model model )
        {
            var path = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() + ".yml" );

            try
            {
                model.SaveYamlFile( path );
                return YamlSerializer.LoadYamlFile<Model>( path );
            }
            finally
            {
                File.Delete( path );
            }
        }

        private static Model ThreeDeep()
        {
            var root = new Node( "root" );
            var middle = new Node( "middle" );
            var leaf = new Node( "leaf" );

            middle.AddChildNode( leaf );
            root.AddChildNode( middle );
            root.AddChildNode( new Node( "sibling" ) );

            return new Model { RootNode = root };
        }

        [TestMethod]
        public void The_node_tree_survives()
        {
            var back = Roundtrip( ThreeDeep() );

            Assert.AreEqual( 4, back.Nodes.Count() );
            CollectionAssert.AreEquivalent( new[] { "root", "middle", "sibling", "leaf" },
                                            back.Nodes.Select( x => x.Name ).ToArray() );
        }

        [TestMethod]
        public void Children_keep_their_parent()
        {
            var back = Roundtrip( ThreeDeep() );
            var middle = back.Nodes.Single( x => x.Name == "middle" );

            Assert.AreEqual( "root", middle.Parent?.Name );
            Assert.AreEqual( "middle", middle.Children.Single().Parent?.Name );
        }

        [TestMethod]
        public void User_properties_survive()
        {
            var model = ThreeDeep();
            var root = model.RootNode;

            root.Properties = new UserPropertyDictionary
            {
                new UserIntProperty( "gfdHelperID", 3 ),
                new UserStringProperty( "label", "hand_l" ),
                new UserBoolProperty( "visible", true ),
            };

            var back = Roundtrip( model ).RootNode;

            Assert.AreEqual( 3, back.Properties.Count );
            Assert.AreEqual( 3, back.Properties["gfdHelperID"].GetValue<int>() );
            Assert.AreEqual( "hand_l", back.Properties["label"].GetValue<string>() );
            Assert.AreEqual( true, back.Properties["visible"].GetValue<bool>() );
        }
    }
}
