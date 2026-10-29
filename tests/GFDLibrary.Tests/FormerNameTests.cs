using System;
using System.IO;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFDLibrary.Tests
{
    /// <summary>
    /// Material presets saved before the texture slots were given their official names still
    /// have to load, and what is written back has to use the new names.
    /// </summary>
    [TestClass]
    public class FormerNameTests
    {
        private static T RoundtripYaml<T>( string yaml ) where T : Resource
        {
            var path = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() + ".yml" );

            try
            {
                File.WriteAllText( path, yaml );
                return YamlSerializer.LoadYamlFile<T>( path );
            }
            finally
            {
                File.Delete( path );
            }
        }

        /// <summary>
        /// The leaf header's two numbers swapped names, so which one a file means depends on
        /// the version it was written under rather than on the key alone.
        /// </summary>
        /// <summary>
        /// Mesh used to be the geometry and is now the node tree that holds it, so the same tag
        /// means different types either side of the version.
        /// </summary>
        [TestMethod]
        public void A_version_1_mesh_tag_is_the_geometry()
        {
            var node = RoundtripYaml<Models.Node>(
                "Name: root\nAttachments:\n- !NodeMeshAttachment\n    Mesh: !Mesh\n      MaterialName: mat\n" );

            var attachment = (Models.NodeGeometryAttachment)node.Attachments[0];
            Assert.AreEqual( "mat", attachment.Geometry.MaterialName );
        }

        [TestMethod]
        public void A_version_2_mesh_tag_is_the_node_tree()
        {
            var node = RoundtripYaml<Models.Node>(
                "FormatVersion: 2\n---\nName: root\nAttachments:\n- !NodeMeshAttachment\n    Mesh: !Mesh\n      Flags: 0\n" );

            var attachment = (Models.NodeMeshAttachment)node.Attachments[0];
            Assert.IsInstanceOfType( attachment.Mesh, typeof( Models.Mesh ) );
        }

        [TestMethod]
        public void A_version_1_model_pack_still_finds_its_mesh()
        {
            var pack = RoundtripYaml<ModelPack>( "Model:\n  Flags: 0\n" );

            Assert.IsNotNull( pack.Mesh, "the old Model key did not reach Mesh" );
        }

        [TestMethod]
        public void Former_animation_member_names_still_load()
        {
            var animation = RoundtripYaml<Animations.Animation>(
                "Duration: 2\nSpeed: 1.5\nControllers:\n- TargetName: bone\n" );

            Assert.AreEqual( 1.5f, animation.Frequency );
            Assert.AreEqual( 1, animation.Joints.Count );
            Assert.AreEqual( "bone", animation.Joints[0].TargetName );
        }

        [TestMethod]
        public void A_version_1_leaf_header_keeps_the_old_meanings()
        {
            var leaf = RoundtripYaml<Effects.EplLeaf>(
                "Name: leaf\nData: !EplParticle\n  Header:\n    Type: 1\n    Field04: 2\n" );

            var header = ( (Effects.EplParticle)leaf.Data ).Header;
            Assert.AreEqual( 1u, header.Category, "the old Type is the category" );
            Assert.AreEqual( 2u, header.Type, "the old Field04 is the variant" );
        }

        [TestMethod]
        public void A_version_2_leaf_header_takes_the_keys_as_written()
        {
            var leaf = RoundtripYaml<Effects.EplLeaf>(
                "FormatVersion: 2\n---\nName: leaf\nData: !EplParticle\n  Header:\n    Category: 1\n    Type: 2\n" );

            var header = ( (Effects.EplParticle)leaf.Data ).Header;
            Assert.AreEqual( 1u, header.Category );
            Assert.AreEqual( 2u, header.Type );
        }

        [TestMethod]
        public void Saving_writes_the_format_version_and_reads_back()
        {
            var path = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() + ".yml" );

            try
            {
                var header = new Effects.EplLeafDataHeader { Category = 7, Type = 3 };
                header.SaveYamlFile( path );

                var text = File.ReadAllText( path );
                StringAssert.StartsWith( text, $"FormatVersion: {YamlSerializer.FormatVersion}" );

                var loaded = YamlSerializer.LoadYamlFile<Effects.EplLeafDataHeader>( path );
                Assert.AreEqual( 7u, loaded.Category );
                Assert.AreEqual( 3u, loaded.Type );
            }
            finally
            {
                File.Delete( path );
            }
        }

        [TestMethod]
        public void Former_class_name_still_loads_as_a_tag()
        {
            // An Epl leaf names the concrete body type as a YAML tag, so a renamed class has to
            // keep answering to the tag files were written with.
            var leaf = RoundtripYaml<Effects.EplLeaf>(
                "Name: leaf\nData: !EplParticle\n  Emitter:\n    Params: !EplExplosionEffectParams\n      Accele: 2\n" );

            var emitter = ( (Effects.EplParticle)leaf.Data ).Emitter;
            Assert.IsInstanceOfType( emitter.Params, typeof( Effects.EplSparkEffectParams ) );
            Assert.AreEqual( 2f, ( (Effects.EplSparkEffectParams)emitter.Params ).Accele );
        }

        [TestMethod]
        public void Current_class_name_loads_as_a_tag()
        {
            var leaf = RoundtripYaml<Effects.EplLeaf>(
                "Name: leaf\nData: !EplParticle\n  Emitter:\n    Params: !EplSparkEffectParams\n      Accele: 3\n" );

            var emitter = ( (Effects.EplParticle)leaf.Data ).Emitter;
            Assert.IsInstanceOfType( emitter.Params, typeof( Effects.EplSparkEffectParams ) );
        }

        [TestMethod]
        public void Former_property_name_still_loads()
        {
            var material = RoundtripYaml<Material>( "Name: test\nSpecularMap:\n  Name: gloss.dds\n" );

            Assert.IsNotNull( material.GlossMap, "SpecularMap did not reach GlossMap" );
            Assert.AreEqual( "gloss.dds", material.GlossMap.Name );
        }

        [TestMethod]
        public void Current_property_name_still_loads()
        {
            var material = RoundtripYaml<Material>( "Name: test\nGlossMap:\n  Name: gloss.dds\n" );

            Assert.AreEqual( "gloss.dds", material.GlossMap.Name );
        }

        /// <summary>
        /// A preset written before the rename, as the shipped ones all were. The flags follow
        /// the maps, so naming the maps is what has to carry across.
        /// </summary>
        [TestMethod]
        public void Former_names_load_a_whole_preset()
        {
            var material = RoundtripYaml<Material>(
                "Name: test\n" +
                "SpecularMap:\n  Name: gloss.dds\n" +
                "HighlightMap:\n  Name: multi.dds\n" +
                "NightMap:\n  Name: dark.dds\n" +
                "ShadowMap:\n  Name: light.dds\n" +
                "TextureMap10:\n  Name: reserve.dds\n" );

            Assert.AreEqual( "gloss.dds", material.GlossMap?.Name );
            Assert.AreEqual( "multi.dds", material.MultiMap?.Name );
            Assert.AreEqual( "dark.dds", material.DarkMap?.Name );
            Assert.AreEqual( "light.dds", material.LightMap?.Name );
            Assert.AreEqual( "reserve.dds", material.ReserveMap?.Name );

            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasGlossMap ) );
            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasMultiMap ) );
            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasDarkMap ) );
            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasLightMap ) );
            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasReserveMap ) );
        }

        [TestMethod]
        public void Former_flag_names_still_parse()
        {
            var material = RoundtripYaml<Material>(
                "Name: test\nFlags: HasSpecularMap, CastShadow\nSpecularMap:\n  Name: gloss.dds\n" );

            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.HasGlossMap ), "HasSpecularMap did not reach HasGlossMap" );
            Assert.IsTrue( material.Flags.HasFlag( MaterialFlags.CastShadow ), "an unrenamed flag beside a renamed one was lost" );
        }

        [TestMethod]
        public void Former_and_current_names_mix()
        {
            var material = RoundtripYaml<Material>(
                "Name: test\nSpecularMap:\n  Name: gloss.dds\nDarkMap:\n  Name: dark.dds\n" );

            Assert.AreEqual( "gloss.dds", material.GlossMap?.Name );
            Assert.AreEqual( "dark.dds", material.DarkMap?.Name );
        }

        [TestMethod]
        public void Former_names_outside_the_texture_slots_still_load()
        {
            var material = RoundtripYaml<Material>( "Name: test\nField98: 4278190080\nDisableBackfaceCulling: 1\n" );

            Assert.AreEqual( 4278190080u, material.ConstantColor );
            Assert.AreEqual( (short)1, material.Culling );
        }

        [TestMethod]
        public void Saving_uses_the_current_names()
        {
            var material = new Material( "test" ) { GlossMap = new TextureMap( "gloss.dds" ) };
            var path = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() + ".yml" );

            try
            {
                material.SaveYamlFile( path );
                var text = File.ReadAllText( path );

                StringAssert.Contains( text, "GlossMap" );
                StringAssert.Contains( text, "HasGlossMap" );
                Assert.IsFalse( text.Contains( "SpecularMap" ), "the former name was written out" );
            }
            finally
            {
                File.Delete( path );
            }
        }
    }
}
