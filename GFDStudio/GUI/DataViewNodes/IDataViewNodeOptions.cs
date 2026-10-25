using System;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// User toggles the view nodes consult, which the application surfaces however it likes.
    /// </summary>
    public interface IDataViewNodeOptions
    {
        /// <summary>
        /// Keep the replaced material's own name and texture names rather than taking the
        /// replacement's.
        /// </summary>
        bool RetainMaterialTextureNames { get; }

        /// <summary>
        /// Keep the replaced material's colour parameters rather than taking the replacement's.
        /// </summary>
        bool RetainMaterialColorValues { get; }
    }

    public static class DataViewNodeOptions
    {
        private static IDataViewNodeOptions mCurrent;

        public static IDataViewNodeOptions Current
        {
            get => mCurrent ?? throw new InvalidOperationException( "No options implementation has been registered" );
            set => mCurrent = value;
        }
    }
}
