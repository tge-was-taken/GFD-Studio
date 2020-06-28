using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Common;
using GFDLibrary.IO;
using GFDLibrary.Models;

namespace GFDLibrary.Animations
{
    public sealed class Animation : Resource
    {
        private static readonly HashSet<string> sArmsFixNodeNameBlacklist = new HashSet<string>
        {
            "Bip01 L UpperArm",
            "Bip01 R UpperArm",
            "Bip01 L Clavicle",
            "Bip01 R Clavicle",
        };

        private List<AnimationFlag10000000DataEntry> mField10;
        private AnimationExtraData mField14;
        private BoundingBox? mBoundingBox;
        private AnimationFlag80000000Data mField1C;
        private UserPropertyDictionary mProperties;
        private float? mSpeed;
        private int mUnknown2;

        public override ResourceType ResourceType => ResourceType.Animation;

        public bool IsCatherineFullBodyData { get; set; } = false;

        // 00
        public AnimationFlags Flags { get; set; }

        // 04
        public float Duration { get; set; }

        // 08 = Controller count

        // 0C
        public List<AnimationController> Controllers { get; set; }

        // 10
        public List<AnimationFlag10000000DataEntry> Field10
        {
            get => mField10;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Flag10000000, value );
                mField10 = value;
            }
        }

        public byte[] Flag10000000Data { get; set; }

        // 14
        public AnimationExtraData Field14
        {
            get => mField14;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Flag20000000, value );
                mField14 = value;
            }
        }

        // 18
        public BoundingBox? BoundingBox
        {
            get => mBoundingBox;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.HasBoundingBox, value );
                mBoundingBox = value;
            }
        }

        // 1C
        public AnimationFlag80000000Data Field1C
        {
            get => mField1C;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Flag80000000, value );
                mField1C = value;
            }
        }

        // 20
        public UserPropertyDictionary Properties
        {
            get => mProperties;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.HasProperties, value );
                mProperties = value;
            }
        }

        // 24
        public float? Speed
        {
            get => mSpeed;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.HasSpeed, value );
                mSpeed = value;
            }
        }

        public Animation( uint version ) : base(version)
        {
            Flags = AnimationFlags.Flag1 | AnimationFlags.Flag2;
            Controllers = new List< AnimationController >();
        }

        public Animation() : this(ResourceVersion.Persona5)
        {          
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            var animationStart = reader.Position;

            if ( Version > 0x1104110 )
                Flags = ( AnimationFlags )reader.ReadInt32();

            Duration = reader.ReadSingle();

            var controllerCount = reader.ReadInt32();
            if (IsCatherineFullBodyData)
            {
                var unknown1 = reader.ReadInt32(); // same as controller count
                mUnknown2 = reader.ReadInt32(); // TODO: no idea what this is
                Trace.Assert( unknown1 == controllerCount, "unknown1 != controllerCount" );
            }

            for ( var i = 0; i < controllerCount; i++ )
            {
                var controller = reader.ReadResource<AnimationController>( Version );
                Controllers.Add( controller );
            }

            if ( Flags.HasFlag( AnimationFlags.Flag10000000 ) )
            {
                // This contains particle data... let's hack our way around that
                var start = reader.Position;

                while ( true )
                {
                    var next = reader.Position + 1;

                    // 0x0001000 is a good match for the controller target type and part of the target ID
                    if ( reader.ReadUInt32() == 0x00010000 && reader.ReadInt16() < 255 )
                    {
                        // Try and read the controller target name
                        var nameLength = reader.ReadInt16();
                        var nextAnimationOffset = reader.Position - 12 - nameLength;
                        if ( nameLength > 0 && nameLength < 32 && reader.ReadBytes( nameLength ).All( x => x != 0 ) )
                        {
                            // If the controller target name is valid, we might be onto something
                            // Now verify if the data associated with the next animation makes somewhat sense
                            reader.SeekBegin( nextAnimationOffset + 4 );
                            var nextAnimationDuration = reader.ReadSingle();
                            var nextAnimationControllerCount = reader.ReadInt32();
                            if ( ( nextAnimationDuration == 0f || ( nextAnimationDuration >= 0.01f && nextAnimationDuration <= 1000f ) ) && nextAnimationControllerCount > 0 && nextAnimationControllerCount <= 1000 )
                            {
                                // If the length of the name isn't 8 (RootNode), usually there's a problem
                                //if ( nameLength != 8 )
                                //    Debugger.Break();

                                // Just read the remainder of the animation data as raw bytes so we can write it back later
                                reader.SeekBegin( start );
                                Flag10000000Data = reader.ReadBytes( ( int ) ( nextAnimationOffset - start ) );
                                return;
                            }
                            else
                            {
                                // This is just here for setting breakpoints
                            }
                        }
                    }

                    // Try again at the next position
                    reader.SeekBegin( next );
                }

                //Field10 = new List<AnimationFlag10000000DataEntry>();

                //var count = reader.ReadInt32();
                //for ( var i = 0; i < count; i++ )
                //{
                //    Field10.Add( new AnimationFlag10000000DataEntry
                //    {
                //        Field00 = Epl.Read( reader, Version, out _ ),
                //        Field04 = reader.ReadStringWithHash( Version )
                //    } );
                //}
            }

            if ( Flags.HasFlag( AnimationFlags.Flag20000000 ) )
                Field14 = reader.ReadResource<AnimationExtraData>( Version );

            if ( Flags.HasFlag( AnimationFlags.Flag80000000 ) )
            {
                Field1C = new AnimationFlag80000000Data
                {
                    Field00 = reader.ReadInt32(),
                    Field04 = reader.ReadStringWithHash( Version, true ),
                    Field20 = reader.ReadResource<AnimationLayer>( Version )
                };
            }

            if ( Flags.HasFlag( AnimationFlags.HasBoundingBox ) )
                BoundingBox = reader.ReadBoundingBox();

            if ( Flags.HasFlag( AnimationFlags.HasSpeed ) )
                Speed = reader.ReadSingle();

            if ( Flags.HasFlag( AnimationFlags.HasProperties ) )
                Properties = reader.ReadResource<UserPropertyDictionary>( Version );
        }

        internal override void Write( ResourceWriter writer )
        {
            if ( Version > 0x1104110 )
                writer.WriteInt32( ( int ) Flags );

            writer.WriteSingle( Duration );
            writer.WriteInt32( Controllers.Count );

            if (IsCatherineFullBodyData)
            {
                writer.WriteInt32( Controllers.Count );
                writer.WriteInt32( mUnknown2 ); 
            }

            foreach ( var controller in Controllers )
                writer.WriteResource( controller );

            if ( Flag10000000Data != null )
            {
                // If we have data associated with flag 10000000, we write it and stop there 
                // because it contains the rest of the animation data until the end.
                writer.WriteBytes( Flag10000000Data );
                return;
            }

            if ( Flags.HasFlag( AnimationFlags.Flag10000000 ) )
            {
                writer.WriteInt32( Field10.Count );
                foreach ( var entry in Field10 )
                {
                    writer.WriteResource( entry.Field00 );
                    writer.WriteStringWithHash( Version, entry.Field04, true );
                }
            }

            if ( Flags.HasFlag( AnimationFlags.Flag20000000 ) )
                writer.WriteResource( Field14 );

            if ( Flags.HasFlag( AnimationFlags.Flag80000000 ) )
            {
                writer.WriteInt32( Field1C.Field00 );
                writer.WriteStringWithHash( Version, Field1C.Field04, true );
                writer.WriteResource( Field1C.Field20 );
            }

            if ( Flags.HasFlag( AnimationFlags.HasBoundingBox ) )
                writer.WriteBoundingBox( BoundingBox.Value );

            if ( Flags.HasFlag( AnimationFlags.HasSpeed ) )
                writer.WriteSingle( Speed.Value );

            if ( Flags.HasFlag( AnimationFlags.HasProperties ) )
                writer.WriteResource( Properties );
        }

        public void FixTargetIds( Model model )
        {
            FixTargetIds( model.Nodes );
        }

        internal void FixTargetIds( IEnumerable<Node> nodes )
        {
            foreach ( var controller in Controllers.ToList() )
            {
                if ( !controller.FixTargetIds( nodes ) )
                    Controllers.Remove( controller );
            }
        }

        public void ConvertToP5()
        {
            Controllers.ForEach( c => c.ConvertToP5() );
        }

        public void Retarget( Model originalModel, Model newModel, bool fixArms )
        {
            Retarget( originalModel.Nodes.ToDictionary( x => x.Name ), newModel.Nodes.ToDictionary( x => x.Name ), fixArms );
        }

        internal void Retarget( Dictionary<string, Node> originalNodeLookup, Dictionary<string, Node> newNodeLookup, bool fixArms )
        {
            FixTargetIds( newNodeLookup.Values );

            foreach ( var controller in Controllers )
            {
                if ( !originalNodeLookup.TryGetValue( controller.TargetName, out var originalNode ) || !newNodeLookup.TryGetValue( controller.TargetName, out var newNode ) )
                    continue;

                var nodeName                = originalNode.Name;
                var originalNodeInvRotation = Quaternion.Inverse( originalNode.Rotation );

                foreach ( var track in controller.Layers )
                {
                    if ( !track.HasPRSKeyFrames )
                        continue;

                    var positionScale = track.PositionScale;

                    foreach ( var key in track.Keys )
                    {
                        var prsKey = ( PRSKey )key;

                        // Make position relative
                        var position         = prsKey.Position * positionScale;
                        var relativePosition = position - originalNode.Translation;
                        var newPosition      = newNode.Translation + relativePosition;
                        prsKey.Position = newPosition / positionScale;

                        // Don't make rotation relative if we're attempting to fix the arms
                        if ( !fixArms || !sArmsFixNodeNameBlacklist.Contains( nodeName ) )
                        {
                            // Make rotation relative
                            var relativeRotation = originalNodeInvRotation * prsKey.Rotation;
                            prsKey.Rotation = newNode.Rotation * relativeRotation;
                        }
                    }
                }
            }
        }
    }

    public static class FlagsHelper
    {
        public static AnimationFlags ClearOrSet( AnimationFlags value, AnimationFlags flag, object obj )
        {
            return obj == null ? value & ~flag : value | flag;
        }
    }
}