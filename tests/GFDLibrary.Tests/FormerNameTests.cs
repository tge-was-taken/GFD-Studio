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
