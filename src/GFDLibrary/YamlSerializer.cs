using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    .IgnoreUnmatchedProperties();
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
