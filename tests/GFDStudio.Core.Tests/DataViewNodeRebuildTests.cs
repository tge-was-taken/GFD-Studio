using System.Collections.Generic;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using GFDStudio.GUI.DataViewNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDStudio.Core.Tests
{
    /// <summary>
    /// The tree rebuilds a node's resource from its child nodes on demand. These cover the cases
    /// where an edit used to be dropped because the rebuild was skipped or read stale data.
    /// </summary>
    [TestClass]
    public class DataViewNodeRebuildTests
    {
        private const uint Version = 0x01105070;

        private static ModelPack BuildModelPack()
        {
            var pack = new ModelPack( Version );

            pack.Textures = new TextureDictionary( Version );
            pack.Textures.Add( new Texture( "first.dds", TextureFormat.DDS, new byte[16] ) );
            pack.Textures.Add( new Texture( "second.dds", TextureFormat.DDS, new byte[16] ) );

            var root = new Node { Name = "root" };
            var branch = new Node { Name = "branch" };
            branch.AddChildNode( new Node { Name = "leaf" } );
            root.AddChildNode( branch );
            root.AddChildNode( new Node { Name = "sibling" } );

            pack.Mesh = new Mesh( Version ) { RootNode = root };
            return pack;
        }

        /// <summary>
        /// Stands in for the tree view, which fills a node in when it is selected or expanded.
        /// </summary>
        private static ModelPackViewNode BuildTree()
        {
            var node = (ModelPackViewNode)DataViewNodeFactory.Create( "model.GMD", BuildModelPack() );
            ExpandAll( node );
            return node;
        }

        private static void ExpandAll( DataViewNode node )
        {
            node.InitializeView();

            foreach ( var child in node.Nodes.ToList() )
                ExpandAll( child );
        }

        private static NodeViewNode FindNode( DataViewNode node, string name )
        {
            if ( node is NodeViewNode match && match.Name == name )
                return match;

            return node.Nodes.Select( x => FindNode( x, name ) ).FirstOrDefault( x => x != null );
        }

        [TestMethod]
        public void RenamingATextureReachesTheRebuiltPack()
        {
            var tree = BuildTree();
            var texture = tree.Textures.Nodes.Cast<TextureViewNode>().First();

            texture.Name = "renamed.dds";

            var pack = (ModelPack)tree.Data;
            Assert.IsTrue( pack.Textures.ContainsTexture( "renamed.dds" ) );
            Assert.IsFalse( pack.Textures.ContainsTexture( "first.dds" ) );
        }

        /// <summary>
        /// A rebuild used to leave the node marked as needing its view rebuilt, which also
        /// switched its own rebuild off, so only the first one took effect.
        /// </summary>
        [TestMethod]
        public void ASecondEditReachesTheRebuiltPack()
        {
            var tree = BuildTree();
            var texture = tree.Textures.Nodes.Cast<TextureViewNode>().First();

            texture.Name = "first-edit.dds";
            Assert.IsTrue( ( (ModelPack)tree.Data ).Textures.ContainsTexture( "first-edit.dds" ) );

            texture.Name = "second-edit.dds";

            var pack = (ModelPack)tree.Data;
            Assert.IsTrue( pack.Textures.ContainsTexture( "second-edit.dds" ) );
            Assert.IsFalse( pack.Textures.ContainsTexture( "first-edit.dds" ) );
        }

        /// <summary>
        /// The list of child nodes holds its items as the base type, which used to read the
        /// resource without rebuilding it, so an edit below a list went missing.
        /// </summary>
        [TestMethod]
        public void DeletingAGrandchildNodeReachesTheRebuiltPack()
        {
            var tree = BuildTree();
            var leaf = FindNode( tree, "leaf" );
            Assert.IsNotNull( leaf );

            leaf.Delete();

            var pack = (ModelPack)tree.Data;
            var branch = pack.Mesh.RootNode.Children.Single( x => x.Name == "branch" );
            Assert.AreEqual( 0, branch.ChildCount );
        }

        [TestMethod]
        public void RenamingAGrandchildNodeReachesTheRebuiltPack()
        {
            var tree = BuildTree();
            var leaf = FindNode( tree, "leaf" );
            Assert.IsNotNull( leaf );

            leaf.Name = "renamed";

            var pack = (ModelPack)tree.Data;
            var branch = pack.Mesh.RootNode.Children.Single( x => x.Name == "branch" );
            Assert.AreEqual( "renamed", branch.Children.Single().Name );
        }

        [TestMethod]
        public void ReorderingANodeReachesTheRebuiltPack()
        {
            var tree = BuildTree();
            var sibling = FindNode( tree, "sibling" );
            Assert.IsNotNull( sibling );

            sibling.MoveUp();

            var pack = (ModelPack)tree.Data;
            CollectionAssert.AreEqual(
                new[] { "sibling", "branch" },
                pack.Mesh.RootNode.Children.Select( x => x.Name ).ToArray() );
        }

        /// <summary>
        /// A node that was never expanded has no child nodes to rebuild from, so its resource
        /// has to be left alone rather than rebuilt into an empty one.
        /// </summary>
        [TestMethod]
        public void AnUnexpandedNodeKeepsItsResource()
        {
            var tree = (ModelPackViewNode)DataViewNodeFactory.Create( "model.GMD", BuildModelPack() );
            tree.InitializeView();

            var pack = (ModelPack)tree.Data;
            Assert.AreEqual( 2, pack.Textures.Count );
            Assert.AreEqual( "root", pack.Mesh.RootNode.Name );
            Assert.AreEqual( 2, pack.Mesh.RootNode.ChildCount );
        }
    }
}
