using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using GFDLibrary.Common;
using GFDLibrary.Effects;
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

        private List<AnimationBit28DataEntry> mField10;
        private AnimationBit29Data mField14;
        private BoundingBox? mBoundingBox;
        private AnimationBit31Data mField1C;
        private UserPropertyDictionary mProperties;
        private float? mSpeed;
        private int mUnknown1;
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
        public List<AnimationBit28DataEntry> Field10
        {
            get => mField10;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Bit28, value );
                mField10 = value;
            }
        }

        public byte[] Flag10000000Data { get; set; }

        // 14
        public AnimationBit29Data Field14
        {
            get => mField14;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Bit29, value );
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
        public AnimationBit31Data Field1C
        {
            get => mField1C;
            set
            {
                Flags = FlagsHelper.ClearOrSet( Flags, AnimationFlags.Bit31, value );
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
            Flags = AnimationFlags.Bit0 | AnimationFlags.Bit1;
            Controllers = new List< AnimationController >();
        }

        public Animation() : this(ResourceVersion.Persona5)
        {          
        }

        protected override void ReadCore( ResourceReader reader )
        {
            Logger.Debug( $"Animation: Reading" );
            var animationStart = reader.Position;

            if ( Version > 0x1104110 )
                Flags = ( AnimationFlags )reader.ReadInt32();

            Duration = reader.ReadSingle();

            var controllerCount = reader.ReadInt32();
            if (IsCatherineFullBodyData || Version >= 0x2000000 )
            {
                mUnknown1 = reader.ReadInt32(); // TODO
                mUnknown2 = reader.ReadInt32(); // TODO: no idea what this is
            }

            Logger.Debug( $"Animation: Reading {controllerCount} controllers" );
            for ( var i = 0; i < controllerCount; i++ )
            {
                Logger.Debug( $"Animation: Reading animation controller #{i}" );
                var controller = reader.ReadResource<AnimationController>( Version );
                Controllers.Add( controller );
            }

            if ( Flags.HasFlag( AnimationFlags.Bit28 ) )
            {
                Field10 = new List<AnimationBit28DataEntry>();

                var count = reader.ReadInt32();
                Logger.Debug( $"Animation: Reading {count} bit 28 data entries" );
                for ( var i = 0; i < count; i++ )
                {
                    Logger.Debug( $"Animation: Reading animation bit 28 data entry #{i}" );
                    var epl = new Epl( Version );
                    epl.Read( reader );
                    Field10.Add( new AnimationBit28DataEntry
                    {
                        Field00 = epl,
                        Field04 = reader.ReadStringWithHash( Version )
                    } );;
                }
            }

            if ( Flags.HasFlag( AnimationFlags.Bit29 ) )
            {
                Logger.Debug( $"Animation: Reading bit 29 data" );
                Field14 = reader.ReadResource<AnimationBit29Data>( Version );
            }

            if ( Flags.HasFlag( AnimationFlags.Bit31 ) )
            {
                Logger.Debug( $"Animation: Reading bit 31 data" );
                Field1C = new AnimationBit31Data
                {
                    Field00 = reader.ReadInt32(),
                    Field04 = reader.ReadStringWithHash( Version, Version < 0x2000000 ),
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

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( Version > 0x1104110 )
                writer.WriteInt32( ( int ) Flags );

            writer.WriteSingle( Duration );
            writer.WriteInt32( Controllers.Count );

            if ( IsCatherineFullBodyData || Version >= 0x2000000 )
            {
                writer.WriteInt32( mUnknown1 );
                writer.WriteInt32( mUnknown2 ); 
            }

            foreach ( var controller in Controllers )
                writer.WriteResource( controller );

            if ( Flags.HasFlag( AnimationFlags.Bit28 ) )
            {
                writer.WriteInt32( Field10.Count );
                foreach ( var entry in Field10 )
                {
                    writer.WriteResource( entry.Field00 );
                    writer.WriteStringWithHash( Version, entry.Field04, Version < 0x2000000 );
                }
            }

            if ( Flags.HasFlag( AnimationFlags.Bit29 ) )
                writer.WriteResource( Field14 );

            if ( Flags.HasFlag( AnimationFlags.Bit31 ) )
            {
                writer.WriteInt32( Field1C.Field00 );
                writer.WriteStringWithHash( Version, Field1C.Field04, Version < 0x2000000 );
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