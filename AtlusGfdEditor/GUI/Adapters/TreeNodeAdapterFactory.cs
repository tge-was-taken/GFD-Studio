using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdEditor.Modules;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    /// <summary>
    /// Factory class for creation of TreeNodeAdapters
    /// </summary>
    public static class TreeNodeAdapterFactory
    {
        private delegate TreeNodeAdapter AdapterCreator( string text, object resource, object[] extraArguments );

        private static readonly Dictionary<Type, AdapterCreator> sCreateAdapterByResourceType = new Dictionary<Type, AdapterCreator>();

        static TreeNodeAdapterFactory()
        {
            // generic adapters
            Register<StreamAdapter>();
            Register<BitmapAdapter>();
            Register( typeof( ListAdapter<> ), typeof( List<> ) );

            // archive adapters
            Register<ArchiveAdapter>();

            // gfd resource adapters
            Register<ModelAdapter>();

            // gfd texture resource adapters
            Register<TextureDictionaryAdapter>();
            Register<TextureAdapter>();

            // gfd material resource adapters
            Register<MaterialDictionaryAdapter>();
            Register<MaterialAdapter>();
            Register<TextureMapAdapter>();
        }

        /// <summary>
        /// Creates an adapter from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string filepath )
        {
            return Create( filepath, (object[])null );
        }

        /// <summary>
        /// Creates an adapter from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string filepath, object[] extraArguments )
        {
            if ( !TryCreate( filepath, extraArguments, out var adapter ) )
            {
                throw new Exception( "Failed to create adapter" );
            }

            return adapter;
        }

        /// <summary>
        /// Creates an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string text, Stream stream )
        {
            return Create( text, stream, null );
        }

        /// <summary>
        /// Creates an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string text, Stream stream, object[] extraArguments )
        {
            if ( !TryCreate( text, stream, extraArguments, out var adapter ) )
            {
                throw new Exception( "Unable to create adapter" );
            }

            return adapter;
        }

        /// <summary>
        /// Creates an adapter from a given resource.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="resource">The resource for which to create an adapter for.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string text, object resource )
        {
            return Create( text, resource, null );
        }

        /// <summary>
        /// Creates an adapter from a given resource.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="resource">The resource for which to create an adapter for.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string text, object resource, object[] extraArguments )
        {
            if ( !TryCreate( text, resource, extraArguments, out var adapter ) )
            {
                throw new Exception( "Failed to create adapter" );
            }

            return adapter;
        }

        /// <summary>
        /// Tries to create an adapter from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string filepath, out TreeNodeAdapter adapter )
        {
            return TryCreate( filepath, (object[])null, out adapter );
        }

        /// <summary>
        /// Tries to create an adapter from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string filepath, object[] extraArguments, out TreeNodeAdapter adapter )
        {
            // first and foremost check if the file exists at all
            if ( !File.Exists( filepath ) )
            {
                adapter = null;
                return false;
            }

            // get filename and stream from file
            var filename = Path.GetFileName( filepath );
            var stream = File.OpenRead( filepath );

            // try create adapter
            if ( !TryCreate( filename, stream, extraArguments, out adapter ) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, out TreeNodeAdapter adapter )
        {
            return TryCreate( text, stream, null, out adapter );
        }

        /// <summary>
        /// Tries to create an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, object[] extraArguments, out TreeNodeAdapter adapter )
        {
            // check if the given text is likely a filename
            string filename = null;
            if ( stream is FileStream fileStream )
            {
                filename = fileStream.Name;
            }
            else
            {           
                if ( Path.HasExtension( text ) )
                    filename = text;
            }

            // try get module for importing
            if ( !ModuleImportUtillities.TryGetModuleForImport( stream, out var module, filename ) )
            {
                adapter = new StreamAdapter( text, stream );
                adapter.Initialize();
                return true;
            }

            // import resource w/ module
            var resource = module.Import( stream, filename );

            // try to create an adapter
            if ( !TryCreate( text, resource, module.ObjectType, extraArguments, out adapter ) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="resource">The resource for which to create an adapter for.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, out TreeNodeAdapter adapter )
        {
            return TryCreate( text, resource, null, out adapter );
        }

        /// <summary>
        /// Tries to create an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="resource">The resource for which to create an adapter for.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, object[] extraArguments, out TreeNodeAdapter adapter )
        {
            return TryCreate( text, resource, resource.GetType(), extraArguments, out adapter );
        }

        /// <summary>
        /// Tries to create an adapter from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeAdapter"/>.</param>
        /// <param name="resource">The resource for which to create an adapter for.</param>
        /// <param name="adapter">The resulting adapter for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, Type type, object[] extraArguments, out TreeNodeAdapter adapter )
        {
            if ( !sCreateAdapterByResourceType.TryGetValue( type, out var adapterCreator ) )
            {
                // check if the type is a constructed generic type
                if ( type.IsConstructedGenericType )
                {
                    // try looking up the unbound generic type definition
                    var typeGenericUnbound = type.GetGenericTypeDefinition();
                    if ( !sCreateAdapterByResourceType.TryGetValue( typeGenericUnbound, out adapterCreator ) )
                    {
                        adapter = null;
                        return false;
                    }
                }
                else
                {
                    adapter = null;
                    return false;
                }
            }

            adapter = adapterCreator( text, resource, extraArguments );
            return true;
        }

        private static void Register<T>() where T : TreeNodeAdapter
        {
            var adapterType = typeof( T );

            Register( adapterType );
        }

        private static void Register( Type adapterType )
        {
            var adapterBaseType = adapterType.BaseType;
            var resourceType = adapterBaseType.GenericTypeArguments[0];

            Register( adapterType, resourceType );
        }

        private static void Register( Type adapterType, Type resourceType )
        {
            Trace.TraceInformation( $"Registering adapter {adapterType} for {resourceType}" );

            sCreateAdapterByResourceType[resourceType] = CreateAdapterCreator( adapterType );
        }

        private static AdapterCreator CreateAdapterCreator( Type type )
        {
            return ( text, resource, extraArguments ) =>
            {
                // set up arguments for CreateInstance
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                Binder binder = null;
                var arguments = new List<object> { text, resource };
                if ( extraArguments != null && extraArguments.Length != 0 )
                {
                    arguments.AddRange( extraArguments );
                }

                CultureInfo culture = null;
                TreeNodeAdapter adapter;

                if ( type.IsGenericType )
                {
                    // create the adapter
                    var resourceType = resource.GetType();
                    Type genericType = null;

                    if ( resourceType.GenericTypeArguments.Length == 0 )
                    {
                        genericType = type.MakeGenericType( resourceType );
                    }
                    else if ( resourceType.GenericTypeArguments.Length == 1 )
                    {
                        genericType = type.MakeGenericType( resourceType.GenericTypeArguments[0] );
                    }
                    else
                    {
                        throw new Exception( "More than 1 generic type arguments" );
                    }

                    adapter = ( TreeNodeAdapter )Activator.CreateInstance( genericType, flags, binder, arguments.ToArray(), culture );
                }
                else
                {

                    // create the adapter
                    adapter = ( TreeNodeAdapter )Activator.CreateInstance( type, flags, binder, arguments.ToArray(), culture );
                }

                // important: call the internal initialize method 
                adapter.Initialize();

                return adapter;
            };
        }
    }
}
