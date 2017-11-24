using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AtlusGfdEditor.FormatModules
{
    /// <summary>
    /// Represents the registry of modules. Modules are registered and kept here.
    /// </summary>
    public static class FormatModuleRegistry
    {
        private static readonly Dictionary<Type, IFormatModule> sModules = new Dictionary<Type, IFormatModule>();

        /// <summary>
        /// Gets the registered modules by type.
        /// </summary>
        public static IReadOnlyDictionary<Type, IFormatModule> ModuleByType => sModules;

        /// <summary>
        /// Gets the registered modules.
        /// </summary>
        public static IEnumerable< IFormatModule > Modules => sModules.Values;

        /// <summary>
        /// Initializes the module registry.
        /// </summary>
        static FormatModuleRegistry()
        {
            RegisterMany
            (
                // generic modules
                new StreamFormatModule(),
                new BitmapFormatModule(),
                new AssimpSceneFormatModule(),

                // archive modules
                new ArchiveFormatModule(),

                // gfd resource modules
                new ModelFormatModule(),

                // gfd texture resource modules 
                new TextureDictionaryFormatModule(),
                new TextureFormatModule(),

                // gfd material resource modules
                new MaterialDictionaryFormatModule(),
                new MaterialFormatModule()
            );
        }

        //
        // Methods for registration of modules
        //

        /// <summary>
        /// Registers a format IO module.
        /// </summary>
        /// <param name="module">The module to register.</param>
        public static void Register( IFormatModule module )
        {
            Trace.TraceInformation( $"Registering module {module.GetType()} for object type {module.ModelType}" );

            if ( sModules.ContainsKey( module.ModelType ) )
            {
                throw new Exception( $"FormatModule registry already contains module for type: {module.ModelType}" );
            }

            sModules[module.ModelType] = module;
        }

        /// <summary>
        /// Registers multiple format IO modules.
        /// </summary>
        /// <param name="modules">The modules to register.</param>
        /// <remarks>Equivalent to registering each module seperately.</remarks>
        public static void RegisterMany( params IFormatModule[] modules )
        {
            foreach ( var module in modules )
            {
                Register( module );
            }
        }
    }
}
