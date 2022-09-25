using System.Collections.Generic;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public class MorphTarget : Resource
    {
        public override ResourceType ResourceType => ResourceType.MorphTarget;


        public int Flags { get; set; }

        public int VertexCount => Vertices.Count;

        public List<Vector3> Vertices { get; set; }

        public MorphTarget()
        {
            Flags = 2;
            Vertices = new List<Vector3>();
        }

 
        public MorphTarget(uint version) :base(version)
        {
            Flags = 2;
            Vertices = new List<Vector3>();
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Flags = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();

            for ( int j = 0; j < vertexCount; j++ )
            {
                var vertex = reader.ReadVector3();
                Vertices.Add( vertex );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( Flags );
            writer.WriteInt32( VertexCount );

            foreach ( var vertex in Vertices )
            {
                writer.WriteVector3( vertex );
            }
        }
    }
}