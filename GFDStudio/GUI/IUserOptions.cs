using System;

namespace GFDStudio.GUI
{
    /// <summary>
    /// User toggles the view nodes consult, which the application surfaces however it likes.
    /// </summary>
    public interface IUserOptions
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

    public static class UserOptions
    {
        private static IUserOptions mCurrent;

        public static IUserOptions Current
        {
            get => mCurrent ?? throw new InvalidOperationException( "No options implementation has been registered" );
            set => mCurrent = value;
        }
    }
}
