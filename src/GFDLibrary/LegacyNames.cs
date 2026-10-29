using System;
using System.Collections.Generic;
using GFDLibrary.Effects;
using GFDLibrary.Models;

namespace GFDLibrary
{
    /// <summary>
    /// Names that changed meaning rather than spelling. A former name alias cannot express one
    /// of these, because the old name is still a property name, just a different property, so
    /// the file format version decides which reading applies.
    /// </summary>
    public static class LegacyNames
    {
        private static readonly IReadOnlyDictionary<string, string> sNone = new Dictionary<string, string>();

        private static readonly Dictionary<Type, IReadOnlyDictionary<string, string>> sByType = new()
        {
            // Officially the pair is category then type. We had the category under Type and the
            // variant under Field04, so a version 1 file means the other one by each name.
            [typeof( EplLeafDataHeader )] = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                ["Type"] = nameof( EplLeafDataHeader.Category ),
                ["Field04"] = nameof( EplLeafDataHeader.Type ),
                ["Variant"] = nameof( EplLeafDataHeader.Type ),
            },
        };

        /// <summary>
        /// Tags that name a different type than they used to. <c>Mesh</c> was the geometry and
        /// is now the node tree that holds it, so the tag alone cannot say which is meant.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, Type> Tags = new Dictionary<string, Type>
        {
            ["Mesh"] = typeof( Geometry ),
            ["Model"] = typeof( Mesh ),
            ["NodeMeshAttachment"] = typeof( NodeGeometryAttachment ),
            ["NodeModelAttachment"] = typeof( NodeMeshAttachment ),
        };

        public static IReadOnlyDictionary<string, string> For( Type type )
            => sByType.TryGetValue( type, out var names ) ? names : sNone;
    }
}
