using System.Collections.Generic;
using System.Numerics;

namespace AtlusGfdLibrary
{
    public class MorphTarget
    {
        public MorphTarget()
        {
            Flags = 2;
            Vertices = new List<Vector3>();
        }

        public int Flags { get; set; }

        public int VertexCount => Vertices.Count;

        public List<Vector3> Vertices { get; set; }
    }
}