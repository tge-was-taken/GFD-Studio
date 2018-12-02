using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GFDLibrary.Animations;

namespace GFDLibrary.Playground
{
    class Program
    {
        static void Main( string[] args )
        {
            var modelPack = Resource.Load<ModelPack>( @"D:\Modding\Persona 5 EU\Main game\ExtractedClean\data\model\character\0001\c0001_002_00.GMD" );
            var model = modelPack.Model;
            var nodes = model.Nodes.ToList();
            var nodeLookup = nodes.ToDictionary( x => x.Name );

            var animationPack = Resource.Load<AnimationPack>( @"D:\Modding\Persona 5 EU\Main game\ExtractedClean\data\model\character\0001\field\bf0001_002.GAP" );
            Debug.Assert( animationPack.BlendAnimations.Count == 0 );

            //var animations = animationPack.Animations.ToList();
            //animationPack.Animations.Clear();
            //animationPack.ExtraData = null;
            //animationPack.Flags &= ~AnimationPackFlags.Flag4;

            //foreach ( var animation in animations )
            //{
            //    var controllers = animation.Controllers.ToList();
            //    foreach ( var controller in controllers )
            //    {
            //        var nodeName = controller.TargetName;
            //        if ( !nodeLookup.TryGetValue( nodeName, out var node ) )
            //        {
            //            animation.Controllers.Remove( controller );
            //            continue;
            //        }

            //        var nodeRotationInv = Quaternion.Inverse( node.Rotation );
            //        var layers = controller.Layers.ToList();

            //        foreach ( var layer in layers )
            //        {
            //            if ( !layer.HasPRSKeyFrames() )
            //            {
            //                controller.Layers.Remove( layer );
            //                continue;
            //            }

            //            //if ( layer.KeyType == KeyType.NodePRS )
            //            //    layer.KeyType = KeyType.NodePR;
            //            //else if ( layer.KeyType == KeyType.NodePRSHalf )
            //            //    layer.KeyType = KeyType.NodePRHalf;
            //            //else if ( layer.KeyType == KeyType.NodeSHalf )
            //            //{
            //            //    controller.Layers.Remove( layer );
            //            //    continue;
            //            //}

            //            var positionScale = layer.PositionScale;
            //            var scaleScale = layer.ScaleScale;

            //            foreach ( var key in layer.Keys )
            //            {
            //                var prsKey = ( PRSKey )key;

            //                // Make position relative
            //                var position         = prsKey.Position * positionScale;
            //                var relativePosition = position - node.Translation;
            //                prsKey.Position = relativePosition / positionScale;

            //                // Make rotation relative
            //                var relativeRotation = nodeRotationInv * prsKey.Rotation;
            //                prsKey.Rotation = relativeRotation;

            //                // Make scale relative
            //                //var scale = prsKey.Scale * scaleScale;
            //                //var relativeScale = scale - node.Scale;
            //                //prsKey.Scale = relativeScale / scaleScale;
            //                prsKey.Scale = Vector3.Zero;
            //                layer.ScaleScale = Vector3.Zero;
            //            }
            //        }

            //        if ( controller.Layers.Count == 0 )
            //        {
            //            animation.Controllers.Remove( controller );
            //            continue;
            //        }
            //    }

            //    animationPack.Animations.Add( new Animation() );
            //    animationPack.BlendAnimations.Add( animation );
            //}

            animationPack.Save( @"D:\Programming\Repos\Mod-Compendium\Source\ModCompendium\bin\Debug\Mods\Persona5\Test Mod\Data\model\character\0001\field\bf0001_002.GAP" );
        }
    }
}
