using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace GFDLibrary
{
    public static class YamlSerializer
    {
        /// <summary>
        /// The layout of what we write. A file without the header is version 1, which is
        /// everything written before members started being renamed into their official names.
        /// </summary>
        public const int FormatVersion = 2;

        private const string FormatVersionKey = "FormatVersion";

        private static Lazy<ISerializer> sSerializer = new Lazy<ISerializer>( () =>
         {
             var builder = new SerializerBuilder();
             AddAbstractTypeTagMappings( ref builder, false );
             return builder.Build();
         } );

        private static readonly Lazy<IDeserializer> sDeserializer = new( () => BuildDeserializer( FormatVersion ) );

        private static readonly Lazy<IDeserializer> sLegacyDeserializer = new( () => BuildDeserializer( 1 ) );

        private static IDeserializer BuildDeserializer( int version )
        {
            var builder = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithTypeInspector( inner => new FormerNameTypeInspector( inner, version ) )
                    .WithTypeConverter( new FormerNameEnumConverter() );
            AddAbstractTypeTagMappings( ref builder, true );
            return builder.Build();
        }

        /// <summary>
        /// Maps a tag to each concrete resource type. Only the reader takes the tags a type used
        /// to go by, so a renamed class still loads but is always written under its new name.
        /// </summary>
        private static void AddAbstractTypeTagMappings<T>( ref T builder, bool includeFormerNames )
            where T : BuilderSkeleton<T>
        {
            
            var assemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( x => x.GetTypes() )
                .Where( x => x.Namespace != null && x.Namespace.StartsWith( "GFDLibrary" ) );
            var abstractClasses = assemblyTypes.Where( x => x.IsAbstract ).ToList();
            var addedTypes = new HashSet<string>();
            foreach ( var abstractClass in abstractClasses )
            {
                foreach ( Type type in assemblyTypes
                    .Where( myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf( abstractClass ) )
                    .Distinct() )
                {
                    var name = type.Name;
                    if ( addedTypes.Add( name ) )
                        builder = builder.WithTagMapping( "!" + name, type );

                    if ( !includeFormerNames )
                        continue;

                    foreach ( var former in type.GetCustomAttributes<FormerNameAttribute>( inherit: false ) )
                    {
                        if ( addedTypes.Add( former.Name ) )
                            builder = builder.WithTagMapping( "!" + former.Name, type );
                    }
                }
            }
        }

        public static void SaveYamlFile( this object o, string filePath )
        {
            using ( var writer = File.CreateText( filePath ) )
            {
                // The header is its own document so the payload keeps whatever shape it has,
                // which is not always a mapping.
                writer.Write( $"{FormatVersionKey}: {FormatVersion}\n---\n" );
                writer.Write( sSerializer.Value.Serialize( o ) );
            }
        }

        public static T LoadYamlFile<T>( string filePath ) where T : Resource
        {
            var (version, body) = ReadBody( filePath );
            return DeserializerFor( version ).Deserialize<T>( body );
        }

        /// <summary>
        /// Takes the header off the front, if there is one. A file without it predates the
        /// header and is read under the older meanings.
        /// </summary>
        private static (int Version, string Body) ReadBody( string filePath )
        {
            var text = File.ReadAllText( filePath ).Replace( '\t', ' ' );
            var firstBreak = text.IndexOf( '\n' );

            if ( firstBreak < 0 )
                return (1, text);

            var firstLine = text[..firstBreak].TrimEnd( '\r' ).Trim();
            if ( !firstLine.StartsWith( FormatVersionKey + ":", StringComparison.Ordinal ) ||
                 !int.TryParse( firstLine[( FormatVersionKey.Length + 1 )..].Trim(), out var version ) )
                return (1, text);

            var rest = text[( firstBreak + 1 )..];
            var secondBreak = rest.IndexOf( '\n' );
            if ( secondBreak < 0 || rest[..secondBreak].TrimEnd( '\r' ).Trim() != "---" )
                return (1, text);

            return (version, rest[( secondBreak + 1 )..]);
        }

        private static IDeserializer DeserializerFor( int version )
            => version >= 2 ? sDeserializer.Value : sLegacyDeserializer.Value;

        /// <summary>
        /// Lets a file written before a rename still find its property. Only reading goes
        /// through here, so what is written out is always the current name.
        /// </summary>
        private sealed class FormerNameTypeInspector : ITypeInspector
        {
            private readonly ITypeInspector mInner;
            private readonly int mVersion;

            public FormerNameTypeInspector( ITypeInspector inner, int version )
            {
                mInner = inner;
                mVersion = version;
            }

            public IEnumerable<IPropertyDescriptor> GetProperties( Type type, object container )
                => mInner.GetProperties( type, container );

            public IPropertyDescriptor GetProperty( Type type, object container, string name, bool ignoreUnmatched )
            {
                // A name that changed meaning rather than just spelling cannot be resolved by
                // the former name map, because both spellings are still in use.
                if ( mVersion < 2 && LegacyNames.For( type ).TryGetValue( name, out var legacy ) )
                    name = legacy;

                var found = mInner.GetProperty( type, container, name, true );

                if ( found == null && FormerNames.For( type ).TryGetValue( name, out var current ) )
                    found = mInner.GetProperty( type, container, current, true );

                if ( found != null || ignoreUnmatched )
                    return found;

                return mInner.GetProperty( type, container, name, false );
            }
        }

        /// <summary>
        /// The same for enum values, including the comma separated lists a flags enum is
        /// written as.
        /// </summary>
        private sealed class FormerNameEnumConverter : IYamlTypeConverter
        {
            public bool Accepts( Type type ) => type.IsEnum && FormerNames.For( type ).Count > 0;

            public object ReadYaml( IParser parser, Type type )
            {
                var former = FormerNames.For( type );

                var names = parser.Consume<Scalar>().Value
                                  .Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries )
                                  .Select( x => former.TryGetValue( x, out var current ) ? current : x );

                return Enum.Parse( type, string.Join( ", ", names ), true );
            }

            public void WriteYaml( IEmitter emitter, object value, Type type )
                => emitter.Emit( new Scalar( value.ToString() ) );
        }

        public static object LoadYamlFile( string filePath, Type type )
        {
            using ( var reader = File.OpenText( filePath ) )
            {
                var str = reader.ReadToEnd().Replace( '\t', ' ' );
                return sDeserializer.Value.Deserialize( str, type );
            }
        }
    }
}
