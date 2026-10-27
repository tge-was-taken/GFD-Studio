using System;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class PRSKey : Key
    {
        public Vector3 Position 
        {
            get => mPosition;
            set
            {
                mPosition = value;
                HasPosition = true;
            }
        }

        private Vector3 mPosition;

        public Quaternion Rotation
        {
            get => mRotation;
            set
            {
                mRotation = value;
                HasRotation = true;
            }
        }

        private Quaternion mRotation;

        public Vector3 Scale
        {
            get => mScale;
            set
            {
                mScale = value;
                HasScale = true;
            }
        }

        private Vector3 mScale;

        public bool HasPosition { get; set; }

        public bool HasRotation { get; set; }

        public bool HasScale { get; set; }

        public virtual bool IsCompressed => Type != KeyType.NodePR && Type != KeyType.NodePRS;

        public PRSKey( KeyType type ) : base( type )
        {
            mPosition = Vector3.Zero;
            mRotation = Quaternion.Identity;
            mScale = Vector3.One;
        }

        public PRSKey() : this( KeyType.NodePRS ) { }

        internal override void Read( ResourceReader reader )
        {
            switch ( Type )
            {
                case KeyType.NodePR:
                case KeyType.NodePRS:
                case KeyType.NodePRSByte:
                    Position = reader.ReadVector3();
                    Rotation = reader.ReadQuaternion();
                    if ( Type == KeyType.NodePRS || Type == KeyType.NodePRSByte ) Scale = reader.ReadVector3();
                    break;

                case KeyType.NodePRHalf:
                case KeyType.NodePRSHalf:
                case KeyType.NodePRHalf_2:
                    Position = reader.ReadVector3Half();
                    Rotation = reader.ReadQuaternionHalf();
                    if (Type == KeyType.NodePRSHalf) Scale = reader.ReadVector3Half();
                    break;

                case KeyType.NodeRHalf:
                    Rotation = reader.ReadQuaternionHalf();
                    break;

                case KeyType.NodeSHalf:
                    Scale = reader.ReadVector3Half();
                    break;

                case KeyType.NodeRSHalf:
                    Rotation = reader.ReadQuaternionHalf();
                    Scale = reader.ReadVector3Half();
                    break;

                case KeyType.NodePSHalf:
                    Position = reader.ReadVector3Half();
                    Scale = reader.ReadVector3Half();
                    break;
                case KeyType.Type31:
                    Position = reader.ReadVector3();
                    Rotation = reader.ReadQuaternion();
                    break;

                default:
                    throw new InvalidOperationException(nameof(Type));
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            switch ( Type )
            {
                case KeyType.NodePR:
                case KeyType.NodePRS:
                case KeyType.NodePRSByte:
                    writer.WriteVector3( Position );
                    writer.WriteQuaternion( Rotation );
                    if ( Type == KeyType.NodePRS || Type == KeyType.NodePRSByte )
                        writer.WriteVector3( Scale );
                    break;
                case KeyType.NodePRHalf:
                case KeyType.NodePRSHalf:
                case KeyType.NodePRHalf_2:
                    writer.WriteVector3Half( Position );
                    writer.WriteQuaternionHalf( Rotation );
                    if ( Type == KeyType.NodePRSHalf )
                        writer.WriteVector3Half( Scale );
                    break;
                case KeyType.NodeRHalf:
                    writer.WriteQuaternionHalf( Rotation );
                    break;
                case KeyType.NodeSHalf:
                    writer.WriteVector3Half( Scale );
                    break;
                case KeyType.NodeRSHalf:
                    writer.WriteQuaternionHalf( Rotation );
                    writer.WriteVector3Half( Scale );
                    break;

                case KeyType.NodePSHalf:
                    writer.WriteVector3Half( Position );
                    writer.WriteVector3Half( Scale );
                    break;
                case KeyType.Type31:
                    writer.WriteVector3( Position );
                    writer.WriteQuaternion( Rotation );
                    break;

                default:
                    throw new InvalidOperationException( nameof( Type ) );
            }
        }
    }
}