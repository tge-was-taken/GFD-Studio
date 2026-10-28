namespace GFDLibrary.Animations
{
    /// <summary>
    /// The on disk key type, which is an index into the game's own key type table. The values are
    /// written out, so they are pinned rather than left to the order of the members.
    /// </summary>
    public enum KeyType
    {
        Invalid = 0,
        NodePR = 1,
        NodePRS = 2,
        Vector3 = 3,
        Quaternion = 4,
        Single = 5,
        [FormerName( "Vector3_2" )]
        AmbientColor = 6,
        [FormerName( "Vector3_3" )]
        DiffuseColor = 7,
        [FormerName( "Vector3_4" )]
        SpecularColor = 8,
        Single_2 = 9,
        Quaternion_2 = 10,
        Single_3 = 11,
        [FormerName( "MaterialSingle_4" )]
        Opacity = 12,
        [FormerName( "Single5" )]
        UV0Transform = 13,
        [FormerName( "MaterialVector3_5" )]
        EmissiveColor = 14,
        Single_5 = 15,
        Single_6 = 16,
        NodePRSByte = 17,
        [FormerName( "Single4Byte" )]
        MeshColor = 18,
        SingleByte = 19,
        [FormerName( "Single5_2" )]
        UV1Transform = 20,
        [FormerName( "Single5Alt" )]
        UV0TransformNoInterp = 21,
        Type22 = 22,
        CameraFieldOfView = 23,
        Single_8 = 24,
        SingleAlt_2 = 25,
        NodePRHalf = 26,
        NodePRSHalf = 27,
        NodePRHalf_2 = 28,
        MaterialSingle_9 = 29,
        SingleAlt_3 = 30,
        Type31 = 31,
        NodeRHalf = 32,
        NodeSHalf = 33,
        NodePSHalf = 34,
        NodeRSHalf = 35,
        [FormerName( "Type36_P5R" )]
        UV1TransformNoInterp = 36
    }
}
