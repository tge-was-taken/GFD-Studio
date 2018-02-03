using System.Collections.Generic;

namespace AtlusGfdLibrary
{
    public sealed class Controller
    {
        // 00
        public TargetKind TargetKind { get; set; }

        // 04
        public int TargetId { get; set; }

        // 08
        public string TargetName { get; set; }

        // 1C
        public List<KeyframeTrack> Tracks { get; set; }

        public Controller()
        {
            Tracks = new List<KeyframeTrack>();
        }

        public override string ToString()
        {
            return $"{TargetKind} {TargetId} {TargetName}";
        }
    }
}