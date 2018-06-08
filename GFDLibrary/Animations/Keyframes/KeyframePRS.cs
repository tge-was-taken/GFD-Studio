using System;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframePRS : Keyframe
    {
        public override KeyframeType Type { get; internal set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public KeyframePRS( KeyframeType type )
        {
            Type = type;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }

        public KeyframePRS() : this( KeyframeType.NodePRS ) { }

        internal override void Read( ResourceReader reader )
        {
            switch ( Type )
            {
                case KeyframeType.NodePR:
                case KeyframeType.NodePRS:
                    Position = reader.ReadVector3();
                    Rotation = reader.ReadQuaternion();
                    Scale = Type == KeyframeType.NodePRS ? reader.ReadVector3() : Vector3.One;
                    break;
                case KeyframeType.NodePRHalf:
                case KeyframeType.NodePRSHalf:
                case KeyframeType.NodePRHalf_2:
                    Position = reader.ReadVector3Half();
                    Rotation = reader.ReadQuaternionHalf();
                    Scale = Type == KeyframeType.NodePRSHalf ? reader.ReadVector3Half() : Vector3.One;
                    break;
                case KeyframeType.NodePHalf:
                    Position = reader.ReadVector3Half();
                    break;
                case KeyframeType.NodeRHalf:
                    Rotation = reader.ReadQuaternionHalf();
                    break;
                case KeyframeType.NodeSHalf:
                    Scale = reader.ReadVector3();
                    break;
                default:
                    throw new InvalidOperationException(nameof(Type));
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            switch ( Type )
            {
                case KeyframeType.NodePR:
                case KeyframeType.NodePRS:
                    writer.WriteVector3( Position );
                    writer.WriteQuaternion( Rotation );
                    if ( Type == KeyframeType.NodePRS )
                        writer.WriteVector3( Scale );
                    break;
                case KeyframeType.NodePRHalf:
                case KeyframeType.NodePRSHalf:
                case KeyframeType.NodePRHalf_2:
                    writer.WriteVector3Half( Position );
                    writer.WriteQuaternionHalf( Rotation );
                    if ( Type == KeyframeType.NodePRSHalf )
                        writer.WriteVector3Half( Scale );
                    break;
                case KeyframeType.NodePHalf:
                    writer.WriteVector3Half( Position );
                    break;
                case KeyframeType.NodeRHalf:
                    writer.WriteQuaternionHalf( Rotation );
                    break;
                case KeyframeType.NodeSHalf:
                    writer.WriteVector3Half( Scale );
                    break;
                default:
                    throw new InvalidOperationException( nameof( Type ) );
            }
        }
    }
}