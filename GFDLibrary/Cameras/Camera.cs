using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Cameras
{
    public sealed class Camera : Resource
    {
        private Matrix4x4 mViewMatrix;

        public override ResourceType ResourceType => ResourceType.Camera;

        public Matrix4x4 ViewMatrix
        {
            get => mViewMatrix;
            set => mViewMatrix = value;
        }

        public Vector3 Position
        {
            get => mViewMatrix.Translation;
            set => mViewMatrix.Translation = value;
        }

        public Vector3 Up
        {
            get => new Vector3( mViewMatrix.M21, mViewMatrix.M22, mViewMatrix.M23 );
            set
            {
                mViewMatrix.M21 = value.X;
                mViewMatrix.M22 = value.Y;
                mViewMatrix.M23 = value.Z;
            }
        }

        public Vector3 Direction
        {
            get => new Vector3( mViewMatrix.M31, mViewMatrix.M32, mViewMatrix.M33 );
            set
            {
                mViewMatrix.M31 = value.X;
                mViewMatrix.M32 = value.Y;
                mViewMatrix.M33 = value.Z;
            }
        }

        public float ClipPlaneNear { get; set; }

        public float ClipPlaneFar { get; set; }

        public float FieldOfView { get; set; }

        public float AspectRatio { get; set; }

        public float Field190 { get; set; }
        public byte Field198 { get; set; }
        public float Field19C { get; set; }
        public float Field1A0 { get; set; }

        public Camera()
        {
            ViewMatrix = Matrix4x4.Identity;
            ClipPlaneNear = 10f;
            ClipPlaneFar = 4000f;
            FieldOfView = 45f;
            AspectRatio = 16 / 9f;
            Field190 = 0;
        }

        public Camera(uint version) :base(version)
        {
            
        }

        public Camera( Vector3 direction, Vector3 up, Vector3 position, float clipPlaneNear, float clipPlaneFar, float fieldOfView, float aspectRatio, float field190 = 0f )
        {
            var zAxis = Vector3.Normalize( direction );
            var yAxis = Vector3.Normalize( up );
            var xAxis = Vector3.Normalize( Vector3.Cross( up, direction ) );

            mViewMatrix.M11 = xAxis.X;
            mViewMatrix.M12 = xAxis.Y;
            mViewMatrix.M13 = xAxis.Z;
            mViewMatrix.M14 = 0;

            mViewMatrix.M21 = yAxis.X;
            mViewMatrix.M22 = yAxis.Y;
            mViewMatrix.M23 = yAxis.Z;
            mViewMatrix.M24 = 0;

            mViewMatrix.M31 = zAxis.X;
            mViewMatrix.M32 = zAxis.Y;
            mViewMatrix.M33 = zAxis.Z;
            mViewMatrix.M34 = 0;

            mViewMatrix.M41 = -( Vector3.Dot( xAxis, position ) );
            mViewMatrix.M42 = -( Vector3.Dot( yAxis, position ) );
            mViewMatrix.M43 = -( Vector3.Dot( zAxis, position ) );
            mViewMatrix.M44 = 1.0f;

            ClipPlaneNear = clipPlaneNear;
            ClipPlaneFar  = clipPlaneFar;
            FieldOfView   = fieldOfView;
            AspectRatio   = aspectRatio;
            Field190      = field190;
        }

        public Camera(Matrix4x4 viewMatrix, float clipPlaneNear, float clipPlaneFar, float fieldOfView, float aspectRatio, float field190 = 0f )
        {
            ViewMatrix = viewMatrix;
            ClipPlaneNear = clipPlaneNear;
            ClipPlaneFar = clipPlaneFar;
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            Field190 = field190;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            ViewMatrix = reader.ReadMatrix4x4();
            ClipPlaneNear = reader.ReadSingle();
            ClipPlaneFar = reader.ReadSingle();
            FieldOfView = reader.ReadSingle();
            AspectRatio = reader.ReadSingle();

            if ( Version > 0x1104060 )
            {
                Field190 = reader.ReadSingle();
            }
            if ( Version > 0x2110050 )
            {
                Field198 = reader.ReadByte();
                Field19C = reader.ReadSingle();
                Field1A0 = reader.ReadSingle();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteMatrix4x4( ViewMatrix );
            writer.WriteSingle( ClipPlaneNear );
            writer.WriteSingle( ClipPlaneFar );
            writer.WriteSingle( FieldOfView );
            writer.WriteSingle( AspectRatio );

            if ( Version > 0x1104060 )
            {
                writer.WriteSingle( Field190 );
            }
        }
    }
}