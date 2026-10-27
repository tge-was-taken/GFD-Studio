using System.Linq;
using System.Reflection;

namespace GFDStudio.GUI
{
    public static class Release
    {
        /// <summary>
        /// The version as built, including any prerelease suffix, which the assembly version
        /// itself cannot carry.
        /// </summary>
        public static string Version { get; } =
            typeof( Release ).Assembly
                             .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                             .FirstOrDefault()?.InformationalVersion
                             // Strip the source revision the SDK appends after a plus sign.
                             ?.Split( '+' )[0]
            ?? typeof( Release ).Assembly.GetName().Version?.ToString( 3 )
            ?? "unknown";
    }
}
