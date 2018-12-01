using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Cameras
{
    public sealed class Camera : Resource
    {
        public override ResourceType ResourceType => ResourceType.Camera;

        public Matrix4x4 Transform { get; set; }

        public float Field180 { get; set; }

        public float Field184 { get; set; }

        public float Field188 { get; set; }

        public float Field18C { get; set; }

        public float Field190 { get; set; }

        public Camera()
        {
            
        }

        public Camera(uint version) :base(version)
        {
            
        }

        internal override void Read( ResourceReader reader )
        {
            Transform = reader.ReadMatrix4x4();
            Field180 = reader.ReadSingle();
            Field184 = reader.ReadSingle();
            Field188 = reader.ReadSingle();
            Field18C = reader.ReadSingle();

            if ( Version > 0x1104060 )
            {
                Field190 = reader.ReadSingle();
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteMatrix4x4( Transform );
            writer.WriteSingle( Field180 );
            writer.WriteSingle( Field184 );
            writer.WriteSingle( Field188 );
            writer.WriteSingle( Field18C );

            if ( Version > 0x1104060 )
            {
                writer.WriteSingle( Field190 );
            }
        }
    }
}