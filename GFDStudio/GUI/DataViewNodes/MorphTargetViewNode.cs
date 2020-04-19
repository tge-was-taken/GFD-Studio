using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MorphTargetViewNode : DataViewNode<MorphTarget>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        public int Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        [DisplayName( "Vertex count" )]
        public int VertexCount => Vertices.Count;

        public List<Vector3> Vertices
        {
            get => Data.Vertices;
            set => SetDataProperty( value );
        }

        public MorphTargetViewNode( string text, MorphTarget data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<MorphTarget>( path => Data.Save( path ) );
            RegisterReplaceHandler<MorphTarget>( Resource.Load<MorphTarget> );
        }
    }
}