using System.IO;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Shaders;
using GFDStudio.GUI.DataViewNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDStudio.Core.Tests
{
    [TestClass]
    public class ShaderCacheViewNodeTests
    {
        private const uint Version = 0x01087178;

        private static ShaderCachePS3 BuildCache()
        {
            var cache = new ShaderCachePS3( Version );

            for ( var i = 0; i < 3; i++ )
            {
                cache.Add( new ShaderPS3( Version )
                {
                    ShaderType = i % 2 == 0 ? ShaderType.Vertex : ShaderType.Fragment,
                    Field06 = (ushort)( 0xA0 + i ),
                    MatFlags0 = 0x11111111u * (uint)( i + 1 ),
                    Data = new byte[] { (byte)i, 1, 2, 3 },
                } );
            }

            return cache;
        }

        private static ShaderCachePS3ViewNode BuildTree()
        {
            var node = (ShaderCachePS3ViewNode)DataViewNodeFactory.Create( "shaders.GSC", BuildCache() );
            Expand( node );
            return node;
        }

        private static void Expand( DataViewNode node )
        {
            node.InitializeView();

            foreach ( var child in node.Nodes.ToList() )
                Expand( child );
        }

        [TestMethod]
        public void TheTreeListsOneNodePerShader()
        {
            var tree = BuildTree();

            Assert.AreEqual( 3, tree.Nodes.Count );
            CollectionAssert.AreEqual(
                new[] { "Shader 0", "Shader 1", "Shader 2" },
                tree.Nodes.Select( x => x.Text ).ToArray() );
            Assert.IsInstanceOfType( tree.Nodes[0], typeof( ShaderPS3ViewNode ) );
        }

        [TestMethod]
        public void EditingAShaderReachesTheRebuiltCache()
        {
            var tree = BuildTree();
            var shader = (ShaderPS3ViewNode)tree.Nodes[1];

            shader.MatFlags0 = 0x44444444;
            shader.ShaderType = ShaderType.Vertex;

            var cache = (ShaderCachePS3)tree.Data;
            Assert.AreEqual( 0x44444444u, cache[1].MatFlags0 );
            Assert.AreEqual( ShaderType.Vertex, cache[1].ShaderType );
        }

        [TestMethod]
        public void DeletingAShaderReachesTheRebuiltCache()
        {
            var tree = BuildTree();
            tree.Nodes[1].Delete();

            var cache = (ShaderCachePS3)tree.Data;
            Assert.AreEqual( 2, cache.Count );
            Assert.AreEqual( 0xA0, cache[0].Field06 );
            Assert.AreEqual( 0xA2, cache[1].Field06 );
        }

        [TestMethod]
        public void AnEditedCacheSavesAndLoadsBack()
        {
            var tree = BuildTree();
            ( (ShaderPS3ViewNode)tree.Nodes[0] ).Field06 = 0xBEEF;
            tree.Nodes[2].Delete();

            var saved = new MemoryStream();
            ( (ShaderCachePS3)tree.Data ).Save( saved, true );

            var reloaded = Resource.Load<ShaderCachePS3>( new MemoryStream( saved.ToArray() ), true );
            Assert.AreEqual( 2, reloaded.Count );
            Assert.AreEqual( 0xBEEF, reloaded[0].Field06 );
            Assert.AreEqual( Version, reloaded.Version );
        }
    }
}
