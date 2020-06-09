using System.Collections.Generic;
using System.Linq;
using GFDLibrary.IO;
using GFDLibrary.Models;

namespace GFDLibrary.Animations
{
    public sealed class AnimationController : Resource
    {
        public override ResourceType ResourceType => ResourceType.AnimationController;

        // 00
        public TargetKind TargetKind { get; set; }

        // 04
        public int TargetId { get; set; }

        // 08
        public string TargetName { get; set; }

        // 1C
        public List<AnimationLayer> Layers { get; set; }

        public AnimationController(uint version) : base(version)
        {
            Layers = new List<AnimationLayer>();
        }

        public AnimationController() : this(ResourceVersion.Persona5)
        {
            
        }

        public override string ToString()
        {
            return $"{TargetKind} {TargetId} {TargetName}";
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            TargetKind = ( TargetKind )reader.ReadInt16();
            TargetId = reader.ReadInt32();
            TargetName = reader.ReadStringWithHash( Version, true );
            Layers = reader.ReadResourceList<AnimationLayer>( Version );

            AnimationLayer mergedLayer = ConvertToPRS( Layers );
            if ( mergedLayer != null )
            {
                Layers.Clear();
                Layers.Add( mergedLayer );
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt16( ( short ) TargetKind );
            writer.WriteInt32( TargetId );
            writer.WriteStringWithHash( Version, TargetName, true );
            writer.WriteResourceList( Layers );
        }

        public bool FixTargetIds( Model model )
        {
            return FixTargetIds( model.Nodes.ToList() );
        }

        internal bool FixTargetIds( IEnumerable<Node> nodes )
        {
            if ( TargetKind != TargetKind.Node )
                return true;

            TargetId = -1;
            int index = 0;
            foreach ( var node in nodes )
            {
                if ( node.Name == TargetName )
                {
                    TargetId = index;
                    break;
                }

                ++index;
            }

            return TargetId != -1;
        }

        /// <summary>
        /// Converts splitted Position, Rotation and Scale layers into one NodePRSHalf layer.
        /// </summary>
        /// <param name="layers">List of layers.</param>
        /// <returns>The merged PRS layer. null if fails to merge layers.</returns>
        internal AnimationLayer ConvertToPRS( List<AnimationLayer> layers )
        {
            if ( layers.Count < 2 || layers.Count > 3 ) return null;

            AnimationLayer p = null;
            AnimationLayer r = null;
            AnimationLayer s = null;
            AnimationLayer pr = null;
            AnimationLayer ps = null;
            AnimationLayer rs = null;

            // Cannot assume layers order

            switch ( layers[0].KeyType )
            {
                case KeyType.Type31:
                    p = layers[0];
                    break;

                case KeyType.NodeRHalf:
                    r = layers[0];
                    break;

                case KeyType.NodeSHalf:
                    s = layers[0];
                    break;

                case KeyType.NodePRHalf:
                    pr = layers[0];
                    break;

                case KeyType.NodeRSHalf:
                    rs = layers[0];
                    break;

                case KeyType.NodePSHalf:
                    ps = layers[0];
                    break;

                default:
                    return null;
            }

            switch ( layers[1].KeyType )
            {
                case KeyType.Type31:
                    p = layers[1];
                    break;

                case KeyType.NodeRHalf:
                    r = layers[1];
                    break;

                case KeyType.NodeSHalf:
                    s = layers[1];
                    break;

                case KeyType.NodePRHalf:
                    pr = layers[1];
                    break;

                case KeyType.NodeRSHalf:
                    rs = layers[1];
                    break;

                case KeyType.NodePSHalf:
                    ps = layers[1];
                    break;

                default:
                    return null;
            }

            if ( layers.Count > 2 )
            {
                switch ( layers[2].KeyType )
                {
                    case KeyType.Type31:
                        p = layers[2];
                        break;

                    case KeyType.NodeRHalf:
                        r = layers[2];
                        break;

                    case KeyType.NodeSHalf:
                        s = layers[2];
                        break;

                    case KeyType.NodePRHalf:
                        pr = layers[2];
                        break;

                    case KeyType.NodeRSHalf:
                        rs = layers[2];
                        break;

                    case KeyType.NodePSHalf:
                        ps = layers[2];
                        break;

                    default:
                        return null;
                }
            }

            // Create new PRS layer

            AnimationLayer prsLayer = new AnimationLayer( Version )
            {
                KeyType = KeyType.NodePRSHalf
            };

            // Position layer

            if ( p != null )
            {
                for ( int i = 0; i < p.Keys.Count; i++ )
                {
                    PRSKey prsKey = new PRSKey(prsLayer.KeyType)
                    {
                        Time = ((PRSKey)p.Keys[i]).Time,
                        Position = ((KeyType31Dancing)p.Keys[i]).Position,
                        IsSetPosition = true
                    };

                    prsLayer.Keys.Add( prsKey );
                }
            }

            // Position/Rotation layer

            if ( pr != null )
            {
                for ( int i = 0; i < pr.Keys.Count; i++ )
                {
                    PRSKey prsKey = new PRSKey( prsLayer.KeyType )
                    {
                        Time = ((PRSKey)pr.Keys[i]).Time,
                        Position = ((PRSKey)pr.Keys[i]).Position,
                        IsSetPosition = true,
                        Rotation = ((PRSKey)pr.Keys[i]).Rotation,
                        IsSetRotation = true
                    };

                    prsLayer.Keys.Add( prsKey );
                }
            }

			// Position/Scale layer

            if ( ps != null )
            {
                for ( int i = 0; i < ps.Keys.Count; i++ )
                {
                    PRSKey prsKey = new PRSKey( prsLayer.KeyType )
                    {
                        Time = ((PRSKey)ps.Keys[i]).Time,
                        Position = ((PRSKey)ps.Keys[i]).Position,
                        IsSetPosition = true,
                        Scale = ((PRSKey)ps.Keys[i]).Scale,
                        IsSetScale = true
                    };

                    prsLayer.Keys.Add( prsKey );
                }
            }

            // Rotation layer

            if ( r != null )
            {
                for ( int i = 0; i < r.Keys.Count; i++ )
                {
                    float time = r.Keys[i].Time;
                    int index = prsLayer.Keys.FindIndex( k => time == k.Time );
                    if ( index != -1 )
                    {
                        ((PRSKey)prsLayer.Keys[index]).Rotation = ((PRSKey)r.Keys[i]).Rotation;
                        ((PRSKey)prsLayer.Keys[index]).IsSetRotation = true;
                    }
                    else
                    {
                        PRSKey prsKey = new PRSKey( KeyType.NodePRSHalf )
                        {
                            Time = ((PRSKey)r.Keys[i]).Time,
                            Rotation = ((PRSKey)r.Keys[i]).Rotation,
                            IsSetRotation = true
                        };

                        // find index to insert new key
                        index = prsLayer.Keys.Count - 1;
                        for ( int j = 0; j < prsLayer.Keys.Count() - 1; j++ )
                        {
                            if ( prsLayer.Keys[j].Time < time && prsLayer.Keys[j + 1].Time > time )
                            {
                                index = j;
                                break;
                            }
                        }

                        if ( index < prsLayer.Keys.Count - 1 )
                        {
                            prsLayer.Keys.Insert( index + 1, prsKey );
                        }
                        else
                        {
                            prsLayer.Keys.Add( prsKey );
                        }
                    }
                }
            }

            // Rotatation/Scale layer

            if (rs != null)
            {
                for ( int i = 0; i < rs.Keys.Count; i++ )
                {
                    float time = rs.Keys[i].Time;
                    int index = prsLayer.Keys.FindIndex( k => time == k.Time );
                    if ( index != -1 )
                    {
                        ((PRSKey)prsLayer.Keys[index]).Rotation = ((PRSKey)rs.Keys[i]).Rotation;
                        ((PRSKey)prsLayer.Keys[index]).IsSetRotation = true;
                        ((PRSKey)prsLayer.Keys[index]).Scale = ((PRSKey)rs.Keys[i]).Scale;
                        ((PRSKey)prsLayer.Keys[index]).IsSetScale = true;
                    }
                    else
                    {
                        PRSKey prsKey = new PRSKey( KeyType.NodePRSHalf )
                        {
                            Time = ((PRSKey)rs.Keys[i]).Time,
                            Rotation = ((PRSKey)rs.Keys[i]).Rotation,
                            IsSetRotation = true,
                            Scale = ((PRSKey)rs.Keys[i]).Scale,
                            IsSetScale = true
                        };

                        // find index to insert new key
                        index = prsLayer.Keys.Count - 1;
                        for ( int j = 0; j < prsLayer.Keys.Count() - 1; j++ )
                        {
                            if ( prsLayer.Keys[j].Time < time && prsLayer.Keys[j + 1].Time > time )
                            {
                                index = j;
                                break;
                            }
                        }

                        if ( index < prsLayer.Keys.Count - 1 )
                        {
                            prsLayer.Keys.Insert(index + 1, prsKey);
                        }
                        else
                        {
                            prsLayer.Keys.Add(prsKey);
                        }
                    }
                }
            }

            // Scale layer

            if ( s != null )
            {
                for ( int i = 0; i < s.Keys.Count; i++ )
                {
                    float time = s.Keys[i].Time;
                    int index = prsLayer.Keys.FindIndex( k => time == k.Time );
                    if ( index != -1 )
                    {
                        ((PRSKey)prsLayer.Keys[index]).Scale = ((PRSKey)s.Keys[i]).Scale;
                        ((PRSKey)prsLayer.Keys[index]).IsSetScale = true;
                    }
                    else
                    {
                        PRSKey prsKey = new PRSKey(KeyType.NodePRSHalf)
                        {
                            Time = ((PRSKey)s.Keys[i]).Time,
                            Scale = ((PRSKey)s.Keys[i]).Scale,
                            IsSetScale = true
                        };

                        // find index to insert new key
                        index = prsLayer.Keys.Count - 1;
                        for ( int j = 0; j < prsLayer.Keys.Count() - 1; j++ )
                        {
                            if ( prsLayer.Keys[j].Time < time && prsLayer.Keys[j + 1].Time > time )
                            {
                                index = j;
                                break;
                            }
                        }

                        if ( index < prsLayer.Keys.Count - 1 )
                        {
                            prsLayer.Keys.Insert(index + 1, prsKey);
                        }
                        else
                        {
                            prsLayer.Keys.Add(prsKey);
                        }
                    }
                }

                prsLayer.PositionScale = p != null ? p.PositionScale : pr.PositionScale;
                prsLayer.ScaleScale = s != null ? s.ScaleScale : rs.ScaleScale;
            }

            for ( int i = 1; i < prsLayer.Keys.Count; i++ )
            {
                if ( !((PRSKey)prsLayer.Keys[i]).IsSetPosition )
                {
                    ((PRSKey)prsLayer.Keys[i]).Position = ((PRSKey)prsLayer.Keys[i-1]).Position;
                }

                if ( !((PRSKey)prsLayer.Keys[i]).IsSetRotation )
                {
                    ((PRSKey)prsLayer.Keys[i]).Rotation = ((PRSKey)prsLayer.Keys[i - 1]).Rotation;
                }

                if ( !((PRSKey)prsLayer.Keys[i]).IsSetScale )
                {
                    ((PRSKey)prsLayer.Keys[i]).Scale = ((PRSKey)prsLayer.Keys[i - 1]).Scale;
                }
            }

            return prsLayer;
        }
    }
}