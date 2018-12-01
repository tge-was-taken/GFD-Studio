using System;
using System.Collections.Generic;
using System.Linq;
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

        public AnimationExtraData ExtraData { get; set; }

        public AnimationPack( uint version )
            : base( version )
        {
            Animations = new List<Animation>();
            BlendAnimations = new List<Animation>();
        }

        public AnimationPack()
        {
            Flags = AnimationPackFlags.Flag8;
            Animations = new List<Animation>();
            BlendAnimations = new List<Animation>();
        }

        internal override void Read( ResourceReader reader )
        {
            if ( Version > 0x1104950 )
                Flags = ( AnimationPackFlags )reader.ReadInt32(); // r26

            Animations = ReadAnimations( reader );

            // TODO: fix this mess
            var start = reader.Position;
            try
            {
                // Try to read blend animations
                BlendAnimations = ReadAnimations( reader );
            }
            catch ( Exception e )
            {
                ErrorsOccuredDuringLoad = true;

                if ( Flags.HasFlag( AnimationPackFlags.Flag4 ) )
                {
                    // If it failed, and the extra data flag is set then try reading it as extra data instead
                    BlendAnimations = new List<Animation>();
                    reader.SeekBegin( start - 8 );
                }
                else
                {
                    // We tried
                    return;
                }
            }

            try
            {
                // Try reading extra data if present
                if ( Flags.HasFlag( AnimationPackFlags.Flag4 ) )
                    ExtraData = reader.ReadResource<AnimationExtraData>( Version );
            }
            catch ( Exception e )
            {
                // Make sure to clear the flag for the extra data so we don't crash during writing
                Flags &= ~AnimationPackFlags.Flag4;
                ErrorsOccuredDuringLoad = true;
            }
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
                try
                {
                    list.Add( reader.ReadResource<Animation>( Version ) );
                }
                catch ( Exception e )
                {
                    ErrorsOccuredDuringLoad = true;

                    while ( i++ < count )
                        list.Add( new Animation() );

                    break;
                }
            }

            return list;
        }

        internal override void Write( ResourceWriter writer )
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

            if ( Flags.HasFlag( AnimationPackFlags.Flag4 ) )
                writer.WriteResource( ExtraData );
        }

        public void FixTargetIds( Model model )
        {
            foreach ( var animation in Animations )
                animation.FixTargetIds( model );

            foreach ( var animation in BlendAnimations )
                animation.FixTargetIds( model );

            ExtraData?.FixTargetIds( model );
        }

        public void Retarget( Model originalModel, Model newModel, bool fixArms )
        {
            var originalNodeLookup = originalModel.Nodes.ToDictionary( x => x.Name );
            var newNodeLookup = newModel.Nodes.ToDictionary( x => x.Name );

            foreach ( var animation in Animations )
                animation.Retarget( originalNodeLookup, newNodeLookup, fixArms );

            foreach ( var animation in BlendAnimations )
                animation.FixTargetIds( newModel.Nodes ); // blend animations are already relative, they only need their ids fixed

            ExtraData?.Retarget( originalNodeLookup, newNodeLookup, fixArms );
        }
    }
}