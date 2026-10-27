using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace GFDLibrary
{
    public static class YamlSerializer
    {
        private static Lazy<ISerializer> sSerializer = new Lazy<ISerializer>( () =>
         {
             var builder = new SerializerBuilder();
             AddAbstractTypeTagMappings( ref builder );
             return builder.Build();
         } );

        private static Lazy<IDeserializer> sDeserializer = new Lazy<IDeserializer>( () =>
        {
            var builder = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithTypeInspector( inner => new FormerNameTypeInspector( inner ) )
                    .WithTypeConverter( new FormerNameEnumConverter() );
            AddAbstractTypeTagMappings( ref builder );
            return builder.Build();
        } );

        private static void AddAbstractTypeTagMappings<T>( ref T builder )
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
                }
            }
        }

        public static void SaveYamlFile( this object o, string filePath )
        {
            using ( var writer = File.CreateText( filePath ) )
            {
                var str = sSerializer.Value.Serialize( o );
                writer.Write( str );
            }
        }

        public static T LoadYamlFile<T>( string filePath ) where T : Resource
        {
            using ( var reader = File.OpenText( filePath ) )
            {
                var str = reader.ReadToEnd().Replace('\t', ' ');
                return sDeserializer.Value.Deserialize<T>( str );
            }
        }

        /// <summary>
        /// Lets a file written before a rename still find its property. Only reading goes
        /// through here, so what is written out is always the current name.
        /// </summary>
        private sealed class FormerNameTypeInspector : ITypeInspector
        {
            private readonly ITypeInspector mInner;

            public FormerNameTypeInspector( ITypeInspector inner ) => mInner = inner;

            public IEnumerable<IPropertyDescriptor> GetProperties( Type type, object container )
                => mInner.GetProperties( type, container );

            public IPropertyDescriptor GetProperty( Type type, object container, string name, bool ignoreUnmatched )
            {
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
