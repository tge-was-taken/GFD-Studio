using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.ViewModels
{
    /// <summary>
    /// Factory class for creation of TreeNodeViewModel instances
    /// </summary>
    public static class TreeNodeViewModelFactory
    {
        private delegate TreeNodeViewModel ViewModelCreator( string text, object resource, object[] extraArguments );

        private static readonly Dictionary<Type, ViewModelCreator> sCreatorByType = new Dictionary<Type, ViewModelCreator>();

        static TreeNodeViewModelFactory()
        {
            // generic view models
            Register<StreamViewModel>();
            Register<BitmapViewModel>();
            Register( typeof( ListViewModel<> ), typeof( List<> ) );

            // archive view models
            Register<ArchiveViewModel>();

            // gfd resource view models
            Register<ModelViewModel>();

            // gfd texture resource view models
            Register<TextureDictionaryViewModel>();
            Register<TextureViewModel>();

            // gfd material resource view models
            Register<MaterialDictionaryViewModel>();
            Register<MaterialViewModel>();
            Register<TextureMapViewModel>();
            Register<TextureMapListViewModel>();
            Register<MaterialAttributeType0ViewModel>();
            Register<MaterialAttributeType1ViewModel>();
            Register<MaterialAttributeType2ViewModel>();
            Register<MaterialAttributeType3ViewModel>();
            Register<MaterialAttributeType4ViewModel>();
            Register<MaterialAttributeType5ViewModel>();
            Register<MaterialAttributeType6ViewModel>();
            Register<MaterialAttributeType7ViewModel>();

            // gfd scene view models
            Register<SceneViewModel>();
            Register<BonePaletteViewModel>();
            Register<NodeViewModel>();

            // gfd chunk type 000100F9 view models
            Register<ChunkType000100F9ViewModel>();
            Register<ChunkType000100F9Entry1ViewModel>();
            Register<ChunkType000100F9Entry2ViewModel>();
            Register<ChunkType000100F9Entry3ViewModel>();

            Register<ChunkType000100F8ViewModel>();

            // gfd animation view models
            Register<AnimationPackageViewModel>();
        }

        /// <summary>
        /// Creates a view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>A view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string filepath )
        {
            return Create( filepath, ( object[] )null );
        }

        /// <summary>
        /// Creates a view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>A view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string filepath, object[] extraArguments )
        {
            if ( !TryCreate( filepath, extraArguments, out var viewModel ) )
            {
                throw new Exception( "Failed to create viewModel" );
            }

            return viewModel;
        }

        /// <summary>
        /// Creates an view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string text, Stream stream )
        {
            return Create( text, stream, null );
        }

        /// <summary>
        /// Creates an view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string text, Stream stream, object[] extraArguments )
        {
            if ( !TryCreate( text, stream, extraArguments, out var viewModel ) )
            {
                throw new Exception( "Unable to create viewModel" );
            }

            return viewModel;
        }

        /// <summary>
        /// Creates an view model from a given resource.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="resource">The resource for which to create an view model for.</param>
        /// <returns>A view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string text, object resource )
        {
            return Create( text, resource, null );
        }

        /// <summary>
        /// Creates an view model from a given resource.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="resource">The resource for which to create an view model for.</param>
        /// <param name="extraArguments"></param>
        /// <returns>A view model for the given resource.</returns>
        public static TreeNodeViewModel Create( string text, object resource, object[] extraArguments )
        {
            if ( !TryCreate( text, resource, extraArguments, out var viewModel ) )
            {
                throw new Exception( "Failed to create viewModel" );
            }

            return viewModel;
        }

        /// <summary>
        /// Tries to create an view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string filepath, out TreeNodeViewModel viewModel )
        {
            return TryCreate( filepath, ( object[] )null, out viewModel );
        }

        /// <summary>
        /// Tries to create an view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <param name="extraArguments"></param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string filepath, object[] extraArguments, out TreeNodeViewModel viewModel )
        {
            // first and foremost check if the file exists at all
            if ( !File.Exists( filepath ) )
            {
                viewModel = null;
                return false;
            }

            // get filename and stream from file
            var filename = Path.GetFileName( filepath );
            var stream = File.OpenRead( filepath );

            // try create viewModel
            if ( !TryCreate( filename, stream, extraArguments, out viewModel ) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, out TreeNodeViewModel viewModel )
        {
            return TryCreate( text, stream, null, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, object[] extraArguments, out TreeNodeViewModel viewModel )
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
            if ( !ModuleImportUtilities.TryGetModuleForImport( stream, out var module, filename ) )
            {
                // fallback
                viewModel = new StreamViewModel( text, stream );
                viewModel.Initialize();
                return true;
            }

            // import resource w/ module
            var resource = module.Import( stream, filename );

            // try to create a view model
            if ( !TryCreate( text, resource, module.ModelType, extraArguments, out viewModel ) )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, out TreeNodeViewModel viewModel )
        {
            return TryCreate( text, resource, null, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, object[] extraArguments, out TreeNodeViewModel viewModel )
        {
            return TryCreate( text, resource, resource.GetType(), extraArguments, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="TreeNodeViewModel"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, Type type, object[] extraArguments, out TreeNodeViewModel viewModel )
        {
            if ( !sCreatorByType.TryGetValue( type, out var adapterCreator ) )
            {
                // check if the type is a constructed generic type
                if ( type.IsConstructedGenericType )
                {
                    // try looking up the unbound generic type definition
                    var typeGenericUnbound = type.GetGenericTypeDefinition();
                    if ( !sCreatorByType.TryGetValue( typeGenericUnbound, out adapterCreator ) )
                    {
                        viewModel = null;
                        return false;
                    }
                }
                else
                {
                    viewModel = null;
                    return false;
                }
            }

            viewModel = adapterCreator( text, resource, extraArguments );
            return true;
        }

        private static void Register<T>() where T : TreeNodeViewModel
        {
            var adapterType = typeof( T );

            Register( adapterType );
        }

        private static void Register( Type type )
        {
            var baseType = type.BaseType;
            var modelType = baseType.GenericTypeArguments[0];

            Register( type, modelType );
        }

        private static void Register( Type viewModelType, Type modelType )
        {
            Trace.TraceInformation( $"Registering TreeNodeViewModel {viewModelType} for {modelType}" );

            sCreatorByType[modelType] = CreateViewModelCreator( viewModelType );
        }

        private static ViewModelCreator CreateViewModelCreator( Type type )
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
                TreeNodeViewModel viewModel;

                if ( type.IsGenericType )
                {
                    // create the viewModel
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
                        throw new NotImplementedException( "More than 1 generic type argument" );
                    }

                    viewModel = ( TreeNodeViewModel )Activator.CreateInstance( genericType, flags, binder, arguments.ToArray(), culture );
                }
                else
                {

                    // create the viewModel
                    viewModel = ( TreeNodeViewModel )Activator.CreateInstance( type, flags, binder, arguments.ToArray(), culture );
                }

                // important: call the internal initialize method 
                viewModel.Initialize();

                return viewModel;
            };
        }
    }
}
