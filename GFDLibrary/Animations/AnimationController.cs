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

        internal override void Read( ResourceReader reader )
        {
            TargetKind = ( TargetKind )reader.ReadInt16();
            TargetId = reader.ReadInt32();
            TargetName = reader.ReadStringWithHash( Version );
            Layers = reader.ReadResourceList<AnimationLayer>( Version );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt16( ( short ) TargetKind );
            writer.WriteInt32( TargetId );
            writer.WriteStringWithHash( Version, TargetName );
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
    }
}