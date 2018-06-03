using System.Collections.Generic;

namespace GFDLibrary
{
    public sealed class Model : Resource
    {
        public TextureDictionary TextureDictionary { get; set; }

        public MaterialDictionary MaterialDictionary { get; set; }

        public Scene Scene { get; set; }

        public AnimationPackage AnimationPackage { get; set; }

        public ChunkType000100F9 ChunkType000100F9 { get; set; }

        public Model(uint version) : base( ResourceType.Model, version )
        {
        }

        public void ReplaceWith( Model other )
        {
            if ( TextureDictionary == null || other.TextureDictionary == null )
                TextureDictionary = other.TextureDictionary;
            else
                TextureDictionary.ReplaceWith( other.TextureDictionary );

            if ( MaterialDictionary == null || other.MaterialDictionary == null )
                MaterialDictionary = other.MaterialDictionary;
            else
                MaterialDictionary.ReplaceWith( other.MaterialDictionary );

            if ( Scene == null || other.Scene == null )
                Scene = other.Scene;
            else
                Scene.ReplaceWith( other.Scene );

            if ( other.AnimationPackage != null )
            {
                if ( AnimationPackage == null )
                {
                    AnimationPackage = other.AnimationPackage;
                }
                else
                {
                    // TODO
                }
            }

            if ( other.ChunkType000100F9 != null )
            {
                if ( ChunkType000100F9 == null )
                {
                    ChunkType000100F9 = other.ChunkType000100F9;
                }
                else
                {
                    // TODO
                }
            }
        }
    }
}
