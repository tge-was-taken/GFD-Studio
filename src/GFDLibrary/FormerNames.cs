using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace GFDLibrary
{
    /// <summary>
    /// The old name to new name map a type carries through <see cref="FormerNameAttribute"/>.
    /// </summary>
    public static class FormerNames
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, string>> sCache = new();

        private static readonly IReadOnlyDictionary<string, string> sNone = new Dictionary<string, string>();

        public static IReadOnlyDictionary<string, string> For( Type type )
            => sCache.GetOrAdd( type, Build );

        private static IReadOnlyDictionary<string, string> Build( Type type )
        {
            var map = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

            foreach ( var member in Members( type ) )
                foreach ( var former in member.GetCustomAttributes<FormerNameAttribute>() )
                    map[former.Name] = member.Name;

            return map.Count == 0 ? sNone : map;
        }

        private static IEnumerable<MemberInfo> Members( Type type )
            => type.IsEnum
                ? type.GetFields( BindingFlags.Public | BindingFlags.Static )
                : type.GetProperties( BindingFlags.Public | BindingFlags.Instance );
    }
}
