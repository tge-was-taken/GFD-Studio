using System;

namespace GFDLibrary
{
    /// <summary>
    /// A name this property or enum value used to have. YAML written before the rename still
    /// loads; what is written back always uses the current name.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true )]
    public sealed class FormerNameAttribute : Attribute
    {
        public FormerNameAttribute( string name ) => Name = name;

        public string Name { get; }
    }
}
