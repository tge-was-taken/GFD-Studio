using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GFDLibrary.IO;

namespace GFDLibrary
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
        public List<KeyframeTrack> Tracks { get; set; }

        public AnimationController(uint version) : base(version)
        {
            Tracks = new List<KeyframeTrack>();
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
            Tracks = reader.ReadResourceList<KeyframeTrack>( Version );
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt16( ( short ) TargetKind );
            writer.WriteInt32( TargetId );
            writer.WriteStringWithHash( Version, TargetName );
            writer.WriteResourceList( Tracks );
        }

        public bool FixTargetIds( Scene scene )
        {
            if ( TargetKind != TargetKind.Node )
                return true;

            var nodes = scene.Nodes.ToList();
            int targetId = nodes.FindIndex( x => x.Name == TargetName );
            if ( targetId == -1 )
                return false;

            TargetId = targetId;

            return true;
        }
    }
}