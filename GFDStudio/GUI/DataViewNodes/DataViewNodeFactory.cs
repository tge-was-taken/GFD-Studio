using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Models;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// Factory class for creation of DataViewNode instances
    /// </summary>
    public static class DataViewNodeFactory
    {
        private delegate DataViewNode DataViewCreator( string text, object resource, object[] extraArguments );

        private static readonly Dictionary<Type, DataViewCreator> sCreatorByType = new Dictionary<Type, DataViewCreator>();

        static DataViewNodeFactory()
        {
            // generic view models
            Register<StreamViewNode>();
            Register<BitmapViewNode>();
            Register( typeof( ListViewNode<> ), typeof( List<> ) );

            // archive view models
            Register<ArchiveViewNode>();

            // gfd resource view models
            Register<ModelPackViewNode>();

            // gfd texture resource view models
            Register<TextureDictionaryViewNode>();
            Register<TextureViewNode>();

            // gfd material resource view models
            Register<MaterialDictionaryViewNode>();
            Register<MaterialViewNode>();
            Register<TextureMapViewNode>();
            Register<TextureMapListViewNode>();
            Register<MaterialAttributeType0ViewNode>();
            Register<MaterialAttributeType1ViewNode>();
            Register<MaterialAttributeType2ViewNode>();
            Register<MaterialAttributeType3ViewNode>();
            Register<MaterialAttributeType4ViewNode>();
            Register<MaterialAttributeType5ViewNode>();
            Register<MaterialAttributeType6ViewNode>();
            Register<MaterialAttributeType7ViewNode>();

            // gfd model view models
            Register<ModelViewNode>();
            Register( typeof( BoneListViewNode ), typeof( List<Bone> ) );
            Register<BoneViewNode>();
            Register<NodeViewNode>();
            Register( typeof( AttachmentListViewNode ), typeof( List<Resource> ) );
            Register( typeof( NodeListViewNode ), typeof( List<Node> ) );
            Register<MeshViewNode>();
            Register<CameraViewNode>();
            Register<LightViewNode>();
            Register<EplViewNode>();
            Register<MorphViewNode>();
            Register<MorphTargetListViewNode>();
            Register<MorphTargetViewNode>();

            // gfd chunk type 000100F9 view models
            Register<ChunkType000100F9ViewNode>();
            Register<ChunkType000100F9Entry1ViewNode>();
            Register<ChunkType000100F9Entry2ViewNode>();
            Register<ChunkType000100F9Entry3ViewNode>();

            Register<ChunkType000100F8ViewNode>();

            // gfd animation view models
            Register<AnimationPackViewNode>();
            Register<AnimationViewNode>();
            Register( typeof( AnimationListViewNode ), typeof( List<Animation> ) );
        }

        /// <summary>
        /// Creates a view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>A view model for the given resource.</returns>
        public static DataViewNode Create( string filepath )
        {
            return Create( filepath, ( object[] )null );
        }

        /// <summary>
        /// Creates a view model from a given file.
        /// </summary>
        /// <param name="filepath">File path pointing to a resource to import.</param>
        /// <returns>A view model for the given resource.</returns>
        public static DataViewNode Create( string filepath, object[] extraArguments )
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
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An view model for the given resource.</returns>
        public static DataViewNode Create( string text, Stream stream )
        {
            return Create( text, stream, null );
        }

        /// <summary>
        /// Creates an view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <returns>An view model for the given resource.</returns>
        public static DataViewNode Create( string text, Stream stream, object[] extraArguments )
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
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="resource">The resource for which to create an view model for.</param>
        /// <returns>A view model for the given resource.</returns>
        public static DataViewNode Create( string text, object resource )
        {
            return Create( text, resource, null );
        }

        /// <summary>
        /// Creates an view model from a given resource.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="resource">The resource for which to create an view model for.</param>
        /// <param name="extraArguments"></param>
        /// <returns>A view model for the given resource.</returns>
        public static DataViewNode Create( string text, object resource, object[] extraArguments )
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
        public static bool TryCreate( string filepath, out DataViewNode viewModel )
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
        public static bool TryCreate( string filepath, object[] extraArguments, out DataViewNode viewModel )
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
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, out DataViewNode viewModel )
        {
            return TryCreate( text, stream, null, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="stream">Stream containing resource data.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, Stream stream, object[] extraArguments, out DataViewNode viewModel )
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
                viewModel = new StreamViewNode( text, stream );
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
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, out DataViewNode viewModel )
        {
            return TryCreate( text, resource, null, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, object[] extraArguments, out DataViewNode viewModel )
        {
            return TryCreate( text, resource, resource.GetType(), extraArguments, out viewModel );
        }

        /// <summary>
        /// Tries to create a view model from a given stream.
        /// </summary>
        /// <param name="text">Display text or filename of the resource for the resulting <see cref="DataViewNode"/>.</param>
        /// <param name="resource">The resource for which to create a view model for.</param>
        /// <param name="viewModel">The resulting view model for the given resource.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreate( string text, object resource, Type type, object[] extraArguments, out DataViewNode viewModel )
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

        private static void Register<T>() where T : DataViewNode
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
            Trace.TraceInformation( $"Registering DataViewNode {viewModelType} for {modelType}" );

            sCreatorByType[modelType] = CreateViewModelCreator( viewModelType );
        }

        private static DataViewCreator CreateViewModelCreator( Type type )
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
                DataViewNode viewModel;

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

                    viewModel = ( DataViewNode )Activator.CreateInstance( genericType, flags, binder, arguments.ToArray(), culture );
                }
                else
                {

                    // create the viewModel
                    viewModel = ( DataViewNode )Activator.CreateInstance( type, flags, binder, arguments.ToArray(), culture );
                }

                // important: call the internal initialize method 
                viewModel.Initialize();

                return viewModel;
            };
        }
    }
}
