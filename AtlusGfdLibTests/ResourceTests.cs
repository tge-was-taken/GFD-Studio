using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtlusGfdLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace AtlusGfdLib.Tests
{
    [TestClass()]
    public class ResourceTests
    {
        [TestMethod()]
        public void LoadFromFileTest()
        {
            var res = Resource.Load( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" );
        }

        [TestMethod()]
        public void LoadFromStreamTest()
        {
            using ( var fileStream = File.OpenRead( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_001_00.GMD" ) )
                Resource.Load( fileStream );
        }

        [TestMethod()]
        public void SaveToFileTest()
        {
            var model = (Model)Resource.Load( @"D:\Modding\Persona 4 Dancing CPK RIP\data\dance\player\pc001_01.GMD" );
            foreach ( var material in model.MaterialDictionary.Materials )
            {
                if ( material.DiffuseMap == null )
                    continue;

                var newMat = new Material( material.Name );
                newMat.Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.Flag80 | MaterialFlags.Flag800 | MaterialFlags.Flag4000 | MaterialFlags.HasDiffuseMap;
                newMat.Ambient = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                newMat.Diffuse = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                newMat.DiffuseMap = material.DiffuseMap;
                newMat.Emissive = new Vector4( 0, 0, 0, 0 );
                newMat.Field40 = 1;
                newMat.Field44 = 0.1f;
                newMat.Field48 = 0;
                newMat.Field49 = 1;
                newMat.Field4A = 0;
                newMat.Field4B = 1;
                newMat.Field4C = 0;
                newMat.Field50 = 0;
                newMat.Field5C = 0;
                newMat.Field6C = unchecked(( int )0xfffffff8);
                newMat.Field70 = unchecked(( int )0xfffffff8);
                newMat.Field90 = 0;
                newMat.Field92 = 4;
                newMat.Field94 = 1;
                newMat.Field96 = 0;
                newMat.Field98 = -1;
                newMat.GlowMap = null;
                newMat.HighlightMap = null;
                newMat.NightMap = null;
                newMat.Properties = null;
                newMat.ReflectionMap = null;
                newMat.ShadowMap = null;
                newMat.Specular = new Vector4( 0, 0, 0, 0 );
                newMat.SpecularMap = null;

                model.MaterialDictionary[material.Name] = newMat;
            }

            Resource.Save( model, @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00_new.GMD" );
        }
    }
}