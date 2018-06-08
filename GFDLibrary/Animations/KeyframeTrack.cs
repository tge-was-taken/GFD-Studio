using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class KeyframeTrack : Resource
    {
        public override ResourceType ResourceType => ResourceType.KeyframeTrack;

        // 00
        public KeyframeType KeyframeType { get; set; }

        // 04 = Keyframe count

        // 08
        public List<Keyframe> Keyframes { get; set; }

        // 0C
        public List<float> KeyframeTimings { get; set; }

        // 10
        public Vector3 BasePosition { get; set; }

        // 1C
        public Vector3 BaseScale { get; set; }

        public KeyframeTrack(uint version) : base(version)
        {
            KeyframeTimings = new List< float >();
            Keyframes = new List< Keyframe >();
        }

        public KeyframeTrack() : this(ResourceVersion.Persona5)
        {
            
        }

        internal override void Read( ResourceReader reader )
        {
            KeyframeType = ( KeyframeType )reader.ReadInt32();

            int keyframeCount = reader.ReadInt32();

            for ( int i = 0; i < keyframeCount; i++ )
                KeyframeTimings.Add( reader.ReadSingle() );

            for ( int i = 0; i < keyframeCount; i++ )
            {
                Keyframe keyframe;

                switch ( KeyframeType )
                {
                    case KeyframeType.NodePR:
                    case KeyframeType.NodePRS:
                    case KeyframeType.NodePRHalf:
                    case KeyframeType.NodePRSHalf:
                    case KeyframeType.NodePRHalf_2:
                    case KeyframeType.NodePHalf:
                    case KeyframeType.NodeRHalf:
                    case KeyframeType.NodeSHalf:
                        keyframe = new KeyframePRS( KeyframeType );
                        break;
                    case KeyframeType.Vector3:
                    case KeyframeType.Vector3_2:
                    case KeyframeType.Vector3_3:
                    case KeyframeType.Vector3_4:
                    case KeyframeType.MaterialVector3_5:
                        keyframe = new KeyframeVector3( KeyframeType );
                        break;
                    case KeyframeType.Quaternion:
                    case KeyframeType.Quaternion_2:
                        keyframe = new KeyframeQuaternion( KeyframeType );
                        break;
                    case KeyframeType.Single:
                    case KeyframeType.Single_2:
                    case KeyframeType.Single_3:
                    case KeyframeType.MaterialSingle_4:
                    case KeyframeType.Single_5:
                    case KeyframeType.Single_6:
                    case KeyframeType.Single_7:
                    case KeyframeType.Single_8:
                    case KeyframeType.SingleAlt_2:
                    case KeyframeType.MaterialSingle_9:
                    case KeyframeType.SingleAlt_3:
                        keyframe = new KeyframeSingle( KeyframeType );
                        break;
                    case KeyframeType.Single5:
                    case KeyframeType.Single5_2:
                    case KeyframeType.Single5Alt:
                        keyframe = new KeyframeSingle5( KeyframeType );
                        break;
                    case KeyframeType.PRSByte:
                        keyframe = new KeyframePRSByte();
                        break;
                    case KeyframeType.Single4Byte:
                        keyframe = new KeyframeSingle4Byte();
                        break;
                    case KeyframeType.SingleByte:
                        keyframe = new KeyframeSingleByte();
                        break;
                    case KeyframeType.Type22:
                        keyframe = new KeyframeType22();
                        break;
                    default:
                        throw new InvalidDataException( $"Unknown/Invalid Key frame type: {KeyframeType}" );
                }

                keyframe.Read( reader );
                Keyframes.Add( keyframe );
            }

            if ( HasBasePositionAndScale( KeyframeType ) )
            {
                BasePosition = reader.ReadVector3();
                BaseScale = reader.ReadVector3();
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( ( int ) KeyframeType );
            writer.WriteInt32( Keyframes.Count );
            KeyframeTimings.ForEach( writer.WriteSingle );
            Keyframes.ForEach( x => x.Write( writer ) );

            if ( HasBasePositionAndScale( KeyframeType ) )
            {
                writer.WriteVector3( BasePosition );
                writer.WriteVector3( BaseScale );
            }
        }

        internal static bool HasBasePositionAndScale( KeyframeType type )
        {
            return type == KeyframeType.NodePRHalf   || type == KeyframeType.NodePRSHalf ||
                   type == KeyframeType.NodePRHalf_2 || type == KeyframeType.NodePHalf ||
                   type == KeyframeType.NodeRHalf    || type == KeyframeType.NodeSHalf;
        }
    }
}