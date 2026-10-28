using Avalonia;
using Avalonia.OpenGL;

namespace GFDStudio.Avalonia
{
    internal static class Program
    {
        public static void Main( string[] args )
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime( args );

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         // Windows picks ANGLE by default, which gives a GL ES context the
                         // model viewer's "#version 330 core" shaders cannot compile. WGL
                         // gives desktop OpenGL; the others stay as fallbacks.
                         .With( new Win32PlatformOptions
                         {
                             RenderingMode = new[]
                             {
                                 Win32RenderingMode.Wgl,
                                 Win32RenderingMode.AngleEgl,
                                 Win32RenderingMode.Software,
                             },
                             // The shaders are "#version 330 core", so ask for a context that
                             // can compile them rather than taking whatever the default is.
                             WglProfiles = new[]
                             {
                                 new GlVersion( GlProfileType.OpenGL, 4, 1 ),
                                 new GlVersion( GlProfileType.OpenGL, 3, 3 ),
                             },
                         } )
                         .LogToTrace();
    }
}
