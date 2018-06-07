namespace GFDLibrary.IO.Keyframes
{
    public struct MaterialKeyframeType12 : IKeyframe
    {
        public KeyframeKind Kind => KeyframeKind.MaterialType12;

        public float Field00 { get; set; }
    }
}