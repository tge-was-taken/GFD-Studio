using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.Modules
{
    /// <summary>
    /// Represents the registry of modules. Modules are registered and kept here.
    /// </summary>
    public static class ModuleRegistry
    {
        private static Dictionary<Type, IModule> sModules = new Dictionary<Type, IModule>();

        /// <summary>
        /// Gets the registered modules by type.
        /// </summary>
        public static IReadOnlyDictionary<Type, IModule> ModuleByType => sModules;

        /// <summary>
        /// Gets the registered modules.
        /// </summary>
        public static IEnumerable<IModule> Modules => sModules.Values;

        /// <summary>
        /// Initializes the module registry.
        /// </summary>
        static ModuleRegistry()
        {
            RegisterMany
            (
                // generic modules
                new StreamModule(),
                new BitmapModule(),

                // archive modules
                new ArchiveModule(),

                // gfd resource modules
                new ModelModule(),

                // gfd texture resource modules 
                new TextureDictionaryModule(),
                new TextureModule()
            );
        }

        //
        // Methods for registration of modules
        //

        /// <summary>
        /// Registers a format IO module.
        /// </summary>
        /// <param name="module">The module to register.</param>
        public static void Register( IModule module )
        {
            if ( sModules.ContainsKey(module.ObjectType) )
            {
                throw new ArgumentException( $"Duplicate module with object type {module.ObjectType}" );
            }

            sModules[module.ObjectType] = module;
        }

        /// <summary>
        /// Registers multiple format IO modules.
        /// </summary>
        /// <param name="modules">The modules to register.</param>
        /// <remarks>Equivalent to registering each module seperately.</remarks>
        public static void RegisterMany( params IModule[] modules )
        {
            foreach ( var module in modules )
            {
                Register( module );
            }
        }
    }
}
