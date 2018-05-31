namespace GFDLibrary
{
    public sealed class Morph
    {
        public int TargetCount => TargetInts.Length;

        public int[] TargetInts { get; set; }

        public string MaterialName { get; set; }
    }
}