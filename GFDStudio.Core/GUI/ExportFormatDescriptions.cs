using System.Collections.Generic;

namespace GFDStudio.GUI
{
    /// <summary>
    /// What to call each export format and what picking it costs. A module's own name has to
    /// serve every extension it handles, so it cannot tell a Collada file from an FBX one.
    /// </summary>
    public static class ExportFormatDescriptions
    {
        private static readonly Dictionary<string, (string Name, string Description)> ByExtension = new()
        {
            ["gfs"] = ("Model pack", "Model, textures, materials and animations together."),
            ["gmd"] = ("Model pack", "Same as GFS, different extension."),
            ["dae"] = ("Collada", "Keeps the skeleton and skinning. Read by most tools."),
            ["obj"] = ("Wavefront OBJ", "Geometry only. No skeleton or animation."),
            ["fbx"] = ("Autodesk FBX", "Keeps the skeleton, skinning and bind pose."),
            ["ascii.fbx"] = ("Autodesk FBX, text", "FBX as readable text. Much larger."),
            ["yml"] = ("YAML", "Every field as text, for editing by hand."),
            ["png"] = ("PNG image", "Decoded. Loses the original encoding and mipmaps."),
            ["dds"] = ("DDS texture", "Stored as is, with compression and mipmaps."),
            ["gnf"] = ("GNF texture", "PlayStation 4 texture, stored as is."),
            ["bin"] = ("Archive", "The container and its contents."),
        };

        /// <summary>
        /// The name and description for an extension. Most formats need no describing, and get
        /// the module's own name with nothing after it.
        /// </summary>
        public static (string Name, string Description) For( string extension, string moduleName )
            => ByExtension.TryGetValue( extension, out var described )
                ? described
                : (moduleName, string.Empty);
    }
}
