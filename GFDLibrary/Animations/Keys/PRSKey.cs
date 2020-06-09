using System;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class PRSKey : Key
    {
        public Vector3 Position { get; set; }

        public bool IsSetPosition { get; set; }

        public Quaternion Rotation { get; set; }

        public bool IsSetRotation { get; set; }

        public Vector3 Scale { get; set; }

        public bool IsSetScale { get; set; }

        public virtual bool HasPosition => Type != KeyType.NodeRHalf && Type != KeyType.NodeSHalf;

        public virtual bool HasRotation => Type != KeyType.NodeSHalf;

        public virtual bool HasScale => Type == KeyType.NodeSHalf || Type == KeyType.NodePRS || Type == KeyType.NodePRSHalf;

        public virtual bool IsCompressed => Type != KeyType.NodePR && Type != KeyType.NodePRS;

        public PRSKey( KeyType type ) : base( type )
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }

        public PRSKey() : this( KeyType.NodePRS ) { }

        internal override void Read( ResourceReader reader )
        {
            switch ( Type )
            {
                case KeyType.NodePR:
                case KeyType.NodePRS:
                    Position = reader.ReadVector3();
                    Rotation = reader.ReadQuaternion();
                    Scale = Type == KeyType.NodePRS ? reader.ReadVector3() : Vector3.One;
                    break;
                case KeyType.NodePRHalf:
                case KeyType.NodePRSHalf:
                case KeyType.NodePRHalf_2:
                    Position = reader.ReadVector3Half();
                    Rotation = reader.ReadQuaternionHalf();
                    Scale = Type == KeyType.NodePRSHalf ? reader.ReadVector3Half() : Vector3.One;
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
                    writer.WriteVector3( Position );
                    writer.WriteQuaternion( Rotation );
                    if ( Type == KeyType.NodePRS )
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

                default:
                    throw new InvalidOperationException( nameof( Type ) );
            }
        }
    }
}