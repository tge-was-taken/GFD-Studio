using System;
using System.Collections.Generic;
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
        private delegate TreeNodeAdapter AdapterCreator( string text, object resource );

        private static Dictionary<Type, AdapterCreator> sCreateAdapterByResourceType = new Dictionary<Type, AdapterCreator>();

        static TreeNodeAdapterFactory()
        {
            // generic adapters
            Register<StreamAdapter>();
            Register<BitmapAdapter>();

            // archive adapters
            Register<ArchiveAdapter>();

            // gfd resource adapters
            Register<ModelAdapter>();
            Register<TextureDictionaryAdapter>();
            Register<TextureAdapter>();
        }

        /// <summary>
        /// Creates an adapter from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>An adapter for the given resource.</returns>
        public static TreeNodeAdapter Create( string filepath )
        {
            if ( !TryCreate( filepath, out var adapter ))
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
            if ( !TryCreate( text, stream, out var adapter ) )
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
            if ( !TryCreate( text, resource, out var adapter ) )
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
            if ( !TryCreate( filename, stream, out adapter ) )
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
            // check if the given text is likely a filename
            string filename = null;
            if ( Path.HasExtension( text ) )
                filename = text;

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
            if ( !TryCreate( text, resource, module.ObjectType, out adapter ) )
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
            return TryCreate( text, resource, resource.GetType(), out adapter );
        }

        private static bool TryCreate( string text, object resource, Type type, out TreeNodeAdapter adapter )
        {
            if ( !sCreateAdapterByResourceType.TryGetValue( type, out var adapterCreator ) )
            {
                adapter = null;
                return false;
            }

            adapter = adapterCreator( text, resource );
            return true;
        }

        private static void Register<T>() where T : TreeNodeAdapter
        {
            var adapterType = typeof( T );
            var adapterBaseType = adapterType.BaseType;
            var resourceType = adapterBaseType.GenericTypeArguments[0];

            sCreateAdapterByResourceType[resourceType] = CreateAdapterCreatorDelegate( adapterType );
        }

        private static AdapterCreator CreateAdapterCreatorDelegate( Type type )
        {
            return ( text, resource ) =>
            {
                // set up arguments for CreateInstance
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                Binder binder = null;
                object[] arguments = new[] { text, resource };
                CultureInfo culture = null;

                // create the adapter
                var adapter = ( TreeNodeAdapter )Activator.CreateInstance( type, flags, binder, arguments, culture );

                // important: call the internal initialize method 
                adapter.Initialize();

                return adapter;
            };
        }
    }
}
