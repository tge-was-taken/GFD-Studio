using System.IO;
using GFDLibrary;
using GFDLibrary.Shaders;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Every platform writes its shader cache to a .GSC, so they are told apart by the resource
    /// type in the file header rather than by the extension.
    /// </summary>
    public abstract class ShaderCacheFormatModule<TCache, TShader> : FormatModule<TCache>
        where TCache : ShaderCache<TShader>, new()
        where TShader : Resource, new()
    {
        public override string[] Extensions
            => new[] { "gsc" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected abstract ResourceType CacheResourceType { get; }

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == CacheResourceType;
        }

        protected override void ExportCore( TCache obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override TCache ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<TCache>( stream );
        }
    }

    public class ShaderCachePS3FormatModule : ShaderCacheFormatModule<ShaderCachePS3, ShaderPS3>
    {
        public override string Name => "Shader cache (PS3)";

        protected override ResourceType CacheResourceType => ResourceType.ShaderCachePS3;
    }

    public class ShaderCachePSP2FormatModule : ShaderCacheFormatModule<ShaderCachePSP2, ShaderPSP2>
    {
        public override string Name => "Shader cache (Vita)";

        protected override ResourceType CacheResourceType => ResourceType.ShaderCachePSP2;
    }

    public class ShaderCachePS4FormatModule : ShaderCacheFormatModule<ShaderCachePS4, ShaderPS4>
    {
        public override string Name => "Shader cache (PS4)";

        protected override ResourceType CacheResourceType => ResourceType.ShaderCachePS4;
    }

    public class ShaderCacheMetaphorDX11FormatModule : ShaderCacheFormatModule<ShaderCacheMetaphorDX11, ShaderMetaphor>
    {
        public override string Name => "Shader cache (DX11)";

        protected override ResourceType CacheResourceType => ResourceType.ShaderCacheMetaphorDX11;
    }
}
