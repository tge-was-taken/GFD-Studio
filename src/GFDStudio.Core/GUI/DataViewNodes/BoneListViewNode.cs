using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class BoneListViewNode : ListViewNode<Bone>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete;

        public BoneListViewNode( string text, List<Bone> data ) : base( text, data, (bone, i) => $"Bone {i}" )
        {
        }
    }
}