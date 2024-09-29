using GFDLibrary.Animations;
using GFDLibrary.IO;
using System.Collections.Generic;
namespace GFDLibrary.Effects
{
    public sealed class EplAnimation : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplAnimation;

        public uint Field00 { get; set; }
        public float Field04 { get; set; }
        public Animation Animation { get; set; }
        public List<EplAnimationController> Controllers { get; set; } = new List<EplAnimationController>();

        public EplAnimation() { }
        public EplAnimation( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "EplAnimation: Reading" );
            Field00 = reader.ReadUInt32();
            Field04 = reader.ReadSingle();
            Animation = reader.ReadResource<Animation>( Version );
            Controllers = reader.ReadResourceList<EplAnimationController>( Version );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteUInt32( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteResource( Animation );
            //     local u32 i;
            //     local u32 controllerCount = Animation.ControllerCount;
            //     //for ( i = 0; i < controllerCount; ++i )
            //     //    keyCountsBuffer[i] = Animation.Controllers.Controllers[i].Layers[0].KeyCount;
            writer.WriteResourceList( Controllers );
        }
    }

    public class EplAnimationKey { }

    public sealed class EplAnimationController : Resource
    {
        public override ResourceType ResourceType => ResourceType.EplAnimationController;

        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public int ControllerIndex { get; set; }
        public EplAnimationKey Keys { get; set; }
        public int Field0C { get; set; }

        public EplAnimationController() { }
        public EplAnimationController( uint Version ) : base( Version ) { }


        protected override void ReadCore( ResourceReader reader )
        {
            //     SetRandomBackColor();
            Logger.Debug( "EplAnimationController: Reading" );
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            ControllerIndex = reader.ReadInt32();
            //     if ( ControllerIndex != -1 )
            //     {
            //         //local u32 keyCount = Animation.Controller[ControllerIndex].KeyCount;
            //         local u32 keyCount = keyCounts[ControllerIndex];
            //         if ( keyCount != 0 )
            //         {
            //				Keys = reader.ReadResource<EplAnimationKey>( Version );
            //         }
            //     }
            Field0C = reader.ReadInt32();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            //     SetRandomBackColor();
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteInt32( ControllerIndex );
            //     if ( ControllerIndex != -1 )
            //     {
            //         //local u32 keyCount = Animation.Controller[ControllerIndex].KeyCount;
            //         local u32 keyCount = keyCounts[ControllerIndex];
            //         if ( keyCount != 0 )
            //         {
            //				writer.WriteResource( Keys );
            //         }
            //     }
            writer.WriteInt32( Field0C );
        }
    }
}
