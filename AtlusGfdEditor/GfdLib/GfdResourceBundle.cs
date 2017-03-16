using System.Collections.Generic;
using System;

using AtlusGfdEditor.GfdLib.Internal;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdResourceBundle : GfdResource
    {
        private List<GfdResource> m_Resources;

        public List<GfdResource> GetResources()
        {
            var list = new List<GfdResource>(m_Resources.Capacity);

            foreach (var item in m_Resources)
            {
                list.Add(item);
            }

            return list;
        }

        public T GetResource<T>()
            where T : GfdResource
        {
            GfdResourceType resType;
            if (!GfdConstants.TypeToGfdResourceTypeMap.TryGetValue(typeof(T), out resType))
                throw new ArgumentException(nameof(T));

            return (T)m_Resources.Find(x => x.Type == resType);
        }

        public void AddResource(GfdResource resource)
        {
            switch (resource.Type)
            {
                case GfdResourceType.AnimationList:
                case GfdResourceType.TextureDictionary:
                case GfdResourceType.MaterialDictionary:
                case GfdResourceType.Scene:
                    if (m_Resources.Find(x => x.Type == resource.Type) == null)
                        m_Resources.Add(resource);
                    return;
            }

            throw new ArgumentException(nameof(resource));
        }

        public void AddResources(params GfdResource[] resources)
        {
            foreach (var item in resources)
            {
                AddResource(item);
            }
        }

        internal GfdResourceBundle(uint version)
            : base(GfdResourceType.ResourceBundle, version)
        {
            m_Resources = new List<GfdResource>();
        }
    }
}
