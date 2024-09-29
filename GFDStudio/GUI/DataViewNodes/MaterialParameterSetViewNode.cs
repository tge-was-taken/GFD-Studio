using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class MaterialParameterSetViewNodeBase<T> : DataViewNode<T> where T : MaterialParameterSetBase
    {
        protected MaterialParameterSetViewNodeBase( string text, T data ) : base( text, data )
        {
        }
    }
}
