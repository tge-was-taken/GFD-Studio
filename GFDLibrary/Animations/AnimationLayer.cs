﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public sealed class AnimationLayer : Resource
    {
        public override ResourceType ResourceType => ResourceType.AnimationLayer;

        public bool IsCatherineFullBodyData { get; set; } = false;

        // 00
        public KeyType KeyType { get; set; }

        // 04 = key count

        // 08
        public List<Key> Keys { get; set; }

        // 0C = key timings list

        // 10
        public Vector3 PositionScale { get; set; }

        // 1C
        public Vector3 ScaleScale { get; set; }

        public bool UsesScaleVectors => KeyType == KeyType.NodePRSHalf ||
                                        KeyType == KeyType.NodePRHalf  || KeyType == KeyType.NodePRHalf_2  || KeyType == KeyType.NodeRSHalf || KeyType == KeyType.NodePSHalf ||
                                        KeyType == KeyType.Type31      || KeyType == KeyType.NodeRHalf     || KeyType == KeyType.NodeSHalf;

        public bool HasPRSKeyFrames
        {
            get
            {
                switch ( KeyType )
                {
                    case KeyType.NodePR:
                    case KeyType.NodePRS:
                    case KeyType.NodePRHalf:
                    case KeyType.NodePRSHalf:
                    case KeyType.NodePRHalf_2:
                    case KeyType.NodeRHalf:
                    case KeyType.NodeSHalf:
                        return true;

                    case KeyType.Type31:
                        return !IsCatherineFullBodyData;
                }

                return false;
            }
        }

        public bool HasSingleKeyFrames
        {
            get
            {
                switch ( KeyType )
                {
                    case KeyType.Single:
                    case KeyType.Single_2:
                    case KeyType.Single_3:
                    case KeyType.MaterialSingle_4:
                    case KeyType.Single_5:
                    case KeyType.Single_6:
                    case KeyType.CameraFieldOfView:
                    case KeyType.Single_8:
                    case KeyType.SingleAlt_2:
                    case KeyType.MaterialSingle_9:
                    case KeyType.SingleAlt_3:
                        return true;

                    default:
                        return false;
                }
            }
        }

        public AnimationLayer(uint version) : base(version)
        {
            Keys = new List< Key >();
            PositionScale = Vector3.One;
            ScaleScale = Vector3.One;
        }

        public AnimationLayer() : this(ResourceVersion.Persona5)
        {         
        }

        protected override void ReadCore( ResourceReader reader )
        {
            KeyType = ( KeyType )reader.ReadInt32();

            var keyCount = reader.ReadInt32();
            var keyTimings = reader.ReadSingles( keyCount );

            Logger.Debug( $"AnimationLayer: Reading type {KeyType} with {keyCount} keys" );

            for ( int i = 0; i < keyCount; i++ )
            {
                Key key;

                switch ( KeyType )
                {
                    case KeyType.NodePR:
                    case KeyType.NodePRS:
                    case KeyType.NodePRHalf:
                    case KeyType.NodePRSHalf:
                    case KeyType.NodePRHalf_2:
                    case KeyType.NodeRHalf:
                    case KeyType.NodeSHalf:
                    case KeyType.NodeRSHalf:
                    case KeyType.NodePSHalf:
                        key = new PRSKey( KeyType );
                        break;
                    case KeyType.Vector3:
                    case KeyType.Vector3_2:
                    case KeyType.Vector3_3:
                    case KeyType.Vector3_4:
                    case KeyType.MaterialVector3_5:
                        key = new Vector3Key( KeyType );
                        break;
                    case KeyType.Quaternion:
                    case KeyType.Quaternion_2:
                        key = new QuaternionKey( KeyType );
                        break;
                    case KeyType.Single:
                    case KeyType.Single_2:
                    case KeyType.Single_3:
                    case KeyType.MaterialSingle_4:
                    case KeyType.Single_5:
                    case KeyType.Single_6:
                    case KeyType.CameraFieldOfView:
                    case KeyType.Single_8:
                    case KeyType.SingleAlt_2:
                    case KeyType.MaterialSingle_9:
                    case KeyType.SingleAlt_3:
                        key = new SingleKey( KeyType );
                        break;
                    case KeyType.Single5:
                    case KeyType.Single5_2:
                    case KeyType.Single5Alt:
                        key = new Single5Key( KeyType );
                        break;
                    case KeyType.NodePRSByte:
                        key = new PRSByteKey();
                        break;
                    case KeyType.Single4Byte:
                        key = new Single4ByteKey();
                        break;
                    case KeyType.SingleByte:
                        key = new SingleByteKey();
                        break;
                    case KeyType.Type22:
                        key = new KeyType22();
                        break;
                    case KeyType.Type31:
                        {
                            if (IsCatherineFullBodyData)
                            {
                                key = new KeyType31FullBody();
                            }
                            else
                            {
                                key = new KeyType31Dancing();
                            }
                        }
                        break;
                    default:
                        throw new InvalidDataException( $"Unknown/Invalid Key frame type: {KeyType}" );
                }

                key.Time = keyTimings[ i ];
                key.Read( reader );
                Keys.Add( key );
            }

            if ( UsesScaleVectors )
            {
                PositionScale = reader.ReadVector3();

                if ( !IsCatherineFullBodyData || KeyType != KeyType.Type31 )
                    ScaleScale = reader.ReadVector3();
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( ( int ) KeyType );
            writer.WriteInt32( Keys.Count );
            Keys.ForEach( x => writer.WriteSingle( x.Time ) );
            Keys.ForEach( x => x.Write( writer ) );

            if ( UsesScaleVectors )
            {
                writer.WriteVector3( PositionScale );

                if ( !IsCatherineFullBodyData || KeyType != KeyType.Type31 )
                    writer.WriteVector3( ScaleScale );
            }
        }
    }
}