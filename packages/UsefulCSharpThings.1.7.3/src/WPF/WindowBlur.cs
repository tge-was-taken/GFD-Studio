using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides operations to blur behind transparent windows in Windows 10.
    /// This provides a way to get the old Windows Vista and 7 "Aero Glass" effect back where windows could be semi-transparent, with the content "behind" the window blurred (as if you were seeing it through dodgy glass).
    /// Pretty much none of this is mine. Got it from: http://withinrafael.com/adding-the-aero-glass-blur-to-your-windows-10-apps/
    /// </summary>
    public static class WindowBlur
    {
        [DllImport("user32.dll")]
        static private extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        /// <summary>
        /// Enables blurring of a semi-transparent window background.
        /// </summary>
        /// <param name="window">The Window you want the background blurred in.</param>
        public static void EnableBlur(System.Windows.Window window)
        {
            SetAccentPolicy(window, AccentState.ACCENT_ENABLE_BLURBEHIND);
        }

        /// <summary>
        /// Disables blurring of a semi-transparent window background.
        /// </summary>
        /// <param name="window">The Window you want the background blurred in.</param>
        public static void DisableBlur(System.Windows.Window window)
        {
            SetAccentPolicy(window, AccentState.ACCENT_DISABLED);
        }

        static void SetAccentPolicy(System.Windows.Window window, AccentState state)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = state;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
