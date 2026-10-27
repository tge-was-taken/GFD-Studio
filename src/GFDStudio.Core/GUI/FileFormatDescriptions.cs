using System.Collections.Generic;

namespace GFDStudio.GUI
{
    /// <summary>
    /// What to call each file format. A module's own name has to serve every extension it
    /// handles, so it cannot tell a Collada file from an FBX one.
    /// </summary>
    public static class FileFormatDescriptions
    {
        private static readonly Dictionary<string, string> NameByExtension = new()
        {
            ["gfs"] = "Model pack",
            ["gmd"] = "Model pack",
            ["dae"] = "Collada",
            ["obj"] = "Wavefront OBJ",
            ["fbx"] = "Autodesk FBX",
            ["ascii.fbx"] = "Autodesk FBX, text",
            ["yml"] = "YAML",
            ["png"] = "PNG image",
            ["dds"] = "DDS texture",
            ["gnf"] = "GNF texture",
            ["bin"] = "Archive",
        };

        /// <summary>
        /// The name for an extension, and a line about it only where the name leaves something
        /// worth saying.
        /// </summary>
        public static (string Name, string Description) For( string extension, string moduleName )
        {
            var name = NameByExtension.TryGetValue( extension, out var known ) ? known : moduleName;
            var description = extension == "yml" ? "Every field as text, for editing by hand." : string.Empty;

            return (name, description);
        }
    }
}
