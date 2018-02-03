namespace AtlusGfdLibrary
{
    /// <summary>
    /// Represents the different kinds of entities a controller can controler.
    /// </summary>
    public enum TargetKind
    {
        /// <summary>
        /// Invalid target type.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The target is a node.
        /// </summary>
        Node = 1,

        /// <summary>
        /// The target is a material.
        /// </summary>
        Material = 2,

        /// <summary>
        /// The target is a camera.
        /// </summary>
        Camera = 3,

        /// <summary>
        /// The target is a morph set.
        /// </summary>
        Morph = 4
    }
}