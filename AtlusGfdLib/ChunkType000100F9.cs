using System.Collections.Generic;

namespace AtlusGfdLib
{
    public class ChunkType000100F9 : Resource
    {
        public ChunkType000100F9( uint version ) : base( ResourceType.ChunkType000100F9, version )
        {
            Entry1List = new List< ChunkType000100F9Entry1 >();
            Entry2List = new List< ChunkType000100F9Entry2 >();
            Entry3List = new List< ChunkType000100F9Entry3 >();
        }

        public int Field140 { get; set; }

        public float Field13C { get; set; }

        public float Field138 { get; set; }

        public float Field134 { get; set; }

        public float Field130 { get; set; }

        public List<ChunkType000100F9Entry1> Entry1List { get; set; }

        public List<ChunkType000100F9Entry2> Entry2List { get; set; }

        public List<ChunkType000100F9Entry3> Entry3List { get; set; }
    }
}