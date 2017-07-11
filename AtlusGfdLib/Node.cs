using System.Collections.Generic;
using System.Numerics;

namespace AtlusGfdLib
{
    public class Node
    {
        public string Name { get; set; }

        // 90
        public Vector3 Position { get; set; }

        // A0
        public Quaternion Rotation { get; set; }

        // B0
        public Vector3 Scale { get; set; }

        // 
        public List<NodeAttachment> Attachments { get; set; }

        // EC
        public Dictionary<string, NodeProperty> Properties { get; set; }

        // E0
        public float FieldE0 { get; set; }
    }
}