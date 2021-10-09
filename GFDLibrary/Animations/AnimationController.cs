using System;
using System.Collections.Generic;
using System.Linq;
using GFDLibrary.IO;
using GFDLibrary.Models;
using System.Numerics;
using System.Diagnostics;

namespace GFDLibrary.Animations
{
    public sealed class AnimationController : Resource
    {
        [Flags]
        enum PRSFlags
        {
            Unknown = 0,
            P = 1,
            R = 2,
            S = 4
        }

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

        protected override void ReadCore( ResourceReader reader )
        {
            TargetKind = ( TargetKind )reader.ReadInt16();
            TargetId = reader.ReadInt32();
            TargetName = reader.ReadStringWithHash( Version, true );
            Logger.Debug( $"AnimationController: Reading {TargetName} kind:{TargetKind} id:{TargetId}" );
            Layers = reader.ReadResourceList<AnimationLayer>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
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

        public void ConvertToP5()
        {
            AnimationLayer mergedLayer = ConvertToPRS( Layers );
            if ( mergedLayer != null )
            {
                Layers.Clear();
                Layers.Add( mergedLayer );
            }
        }

        /// <summary>
        /// Converts splitted Position, Rotation and Scale layers into one NodePRSHalf layer.
        /// </summary>
        /// <param name="layers">List of layers.</param>
        /// <returns>The merged PRS layer. null if fails to merge layers.</returns>
        internal AnimationLayer ConvertToPRS( List<AnimationLayer> layers )
        {
            if ( layers.Count < 2 || layers.Count > 3 ) return null;

            // Exit if we have an unhandled controller
            foreach ( AnimationLayer layer in layers )
            {
                switch ( layer.KeyType )
                {
                    case KeyType.NodePRHalf:
                    case KeyType.NodePRHalf_2:
                    case KeyType.Type31:
                    case KeyType.NodeRHalf:
                    case KeyType.NodeSHalf:
                    case KeyType.NodePSHalf:
                    case KeyType.NodeRSHalf:
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

            // Feed keys to the new layer
            foreach ( AnimationLayer layer in layers )
            {
                for ( int i = 0; i < layer.Keys.Count; i++ )
                {
                    var isNewKey = true;
                    float time = layer.Keys[i].Time;

                    // Get existing key or create new one
                    var prsKey = (PRSKey)prsLayer.Keys.FirstOrDefault( k => k.Time == time );

                    if ( prsKey != null ) isNewKey = false;
                    else prsKey = new PRSKey( prsLayer.KeyType ) { Time = time };

                    // Set key data
                    if ( ((PRSKey)layer.Keys[i]).HasPosition )
                    {
                        prsKey.Position = ((PRSKey)layer.Keys[i]).Position;
                    }

                    if ( ((PRSKey)layer.Keys[i]).HasRotation )
                    {
                        prsKey.Rotation = ((PRSKey)layer.Keys[i]).Rotation;
                    }

                    if ( ((PRSKey)layer.Keys[i]).HasScale )
                    {
                        prsKey.Scale = ((PRSKey)layer.Keys[i]).Scale;
                    }

                    if ( isNewKey )
                    {
                        // Define where to insert the key
                        int count = prsLayer.Keys.Count;
                        for ( int j = 0; j < prsLayer.Keys.Count() - 1; j++ )
                        {
                            if ( prsLayer.Keys[j].Time <= time && prsLayer.Keys[j + 1].Time > time )
                            {
                                prsLayer.Keys.Insert(j + 1, prsKey);
                                break;
                            }
                        }
                        
                        if ( count == prsLayer.Keys.Count ) prsLayer.Keys.Add(prsKey);
                    }
                }
            }

            // Just in case
            ((PRSKey)prsLayer.Keys[0]).HasPosition = true;
            ((PRSKey)prsLayer.Keys[0]).HasRotation = true;
            ((PRSKey)prsLayer.Keys[0]).HasScale = true;

            // PRS Keys completion
            for ( int i = 1; i < prsLayer.Keys.Count; i++ )
            {
                if ( !((PRSKey)prsLayer.Keys[i]).HasPosition )
                {
                    ((PRSKey)prsLayer.Keys[i]).Position = ((PRSKey)prsLayer.Keys[i - 1]).Position;
                }

                if ( !((PRSKey)prsLayer.Keys[i]).HasRotation )
                {
                    ((PRSKey)prsLayer.Keys[i]).Rotation = ((PRSKey)prsLayer.Keys[i - 1]).Rotation;
                }

                if ( !((PRSKey)prsLayer.Keys[i]).HasScale )
                {
                    ((PRSKey)prsLayer.Keys[i]).Scale = ((PRSKey)prsLayer.Keys[i - 1]).Scale;
                }
            }

            prsLayer.PositionScale = layers.FirstOrDefault( l => ((PRSKey)l.Keys[0]).HasPosition )?.PositionScale ?? Vector3.One;
            prsLayer.ScaleScale = layers.FirstOrDefault( l => ((PRSKey)l.Keys[0]).HasScale )?.ScaleScale ?? Vector3.One;

            return prsLayer;
        }
    }
}