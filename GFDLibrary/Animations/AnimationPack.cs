using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.IO;
using GFDLibrary.Models;

namespace GFDLibrary.Animations
{
    public sealed class AnimationPack : Resource
    {
        public override ResourceType ResourceType => ResourceType.AnimationPack;

        public byte[] RawData { get; set; }

        public bool ErrorsOccuredDuringLoad { get; set; }

        public AnimationPackFlags Flags { get; set; }

        public List<Animation> Animations { get; set; }

        public List<Animation> BlendAnimations { get; set; }

        public AnimationBit29Data Bit29Data { get; set; }

        public AnimationPack( uint version )
            : base( version )
        {
            Animations = new List<Animation>();
            BlendAnimations = new List<Animation>();
        }

        public AnimationPack()
        {
            Flags = AnimationPackFlags.Bit3;
            Animations = new List<Animation>();
            BlendAnimations = new List<Animation>();
        }

        protected override void ReadCore( ResourceReader reader )
        {
            if ( Version > 0x1104950 )
                Flags = ( AnimationPackFlags )reader.ReadInt32(); // r26

            Animations = ReadAnimations( reader );

            // TODO: fix this mess
            var start = reader.Position;
            //try
            {
                // Try to read blend animations
                BlendAnimations = ReadAnimations( reader );
            }
            //catch ( Exception )
            //{
            //    ErrorsOccuredDuringLoad = true;

            //    if ( Flags.HasFlag( AnimationPackFlags.Bit2 ) )
            //    {
            //        // If it failed, and the extra data flag is set then try reading it as extra data instead
            //        BlendAnimations = new List<Animation>();
            //        reader.SeekBegin( start - 8 );
            //    }
            //    else
            //    {
            //        // We tried
            //        return;
            //    }
            //}

            //try
            {
                // Try reading extra data if present
                if ( Flags.HasFlag( AnimationPackFlags.Bit2 ) )
                    Bit29Data = reader.ReadResource<AnimationBit29Data>( Version );
            }
            //catch ( Exception )
            //{
            //    // Make sure to clear the flag for the extra data so we don't crash during writing
            //    Flags &= ~AnimationPackFlags.Bit2;
            //    ErrorsOccuredDuringLoad = true;
            //}
        }

        private List<Animation> ReadAnimations( ResourceReader reader )
        {
            var count = reader.ReadInt32();
            var list = new List<Animation>( count );
            for ( int i = 0; i < count; i++ )
            {
                // Animations are super broken at the moment
                // so try to read one, and give up if it doesn't work out
                // usually the exception will happen at either the last, or the second to last animation
                //try
                {
                    Logger.Debug( $"AnimationPack: Reading animation #{i}" );
                    list.Add( reader.ReadResource<Animation>( Version ) );
                }
                //catch ( Exception )
                //{
                //    ErrorsOccuredDuringLoad = true;

                //    while ( i++ < count )
                //        list.Add( new Animation() );

                //    break;
                //}
            }

            return list;
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( RawData != null )
            {
                writer.Write( RawData );
                return;
            }

            if ( Version > 0x1104950 )
                writer.WriteInt32( ( int )Flags );

            writer.WriteResourceList( Animations );
            writer.WriteResourceList( BlendAnimations );

            if ( Flags.HasFlag( AnimationPackFlags.Bit2 ) )
                writer.WriteResource( Bit29Data );
        }

        public void FixTargetIds( Model model )
        {
            foreach ( var animation in Animations )
                animation.FixTargetIds( model );

            foreach ( var animation in BlendAnimations )
                animation.FixTargetIds( model );

            Bit29Data?.FixTargetIds( model );
        }

        public void Retarget( Model originalModel, Model newModel, bool fixArms )
        {
            var originalNodeLookup = originalModel.Nodes.ToDictionary( x => x.Name );
            var newNodeLookup = newModel.Nodes.ToDictionary( x => x.Name );

            foreach ( var animation in Animations )
                animation.Retarget( originalNodeLookup, newNodeLookup, fixArms );

            foreach ( var animation in BlendAnimations )
                animation.FixTargetIds( newModel.Nodes ); // blend animations are already relative, they only need their ids fixed

            Bit29Data?.Retarget( originalNodeLookup, newNodeLookup, fixArms );
        }

        public void Rescale(Vector3 scale, Vector3 position, Quaternion rotation)
        {
            for (int w = 0; w < this.Animations.Count; w++)
            {
                Animation animation = this.Animations[w];
                for (int x = 0; x < animation.Controllers.Count; x++)
                {
                    AnimationController controller = animation.Controllers[x];
                    for (int y = 0; y < controller.Layers.Count; y++)
                    {
                        AnimationLayer layer = controller.Layers[y];
                        if (layer.HasPRSKeyFrames && controller.TargetId == 0)
                        {
                            var newLayer = new AnimationLayer(layer.Version) { KeyType = KeyType.NodePRS };
                            for (int z = 0; z < layer.Keys.Count; z++)
                            {
                                foreach (var key in layer.Keys)
                                {
                                    PRSKey prsKey = (PRSKey)key;

                                    var newKey = new PRSKey(KeyType.NodePRS)
                                    {
                                        Time = prsKey.Time,
                                        Position = (prsKey.Position * layer.PositionScale) + position,
                                        Rotation = rotation,
                                        Scale = (prsKey.Scale * layer.ScaleScale) * scale
                                    };

                                    newLayer.Keys.Add(newKey);
                                }
                            }
                            controller.Layers[y] = newLayer;
                        
                        }
                    }
                }
            }
        }

        public void ConvertToP5()
        {
            Animations.ForEach( a => a.ConvertToP5() );
            BlendAnimations.ForEach( ba => ba.ConvertToP5() );
        }
    }
}