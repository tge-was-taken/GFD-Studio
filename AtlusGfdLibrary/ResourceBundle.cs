using System.Collections.Generic;
using System;

namespace AtlusGfdLib
{
    public sealed class ResourceBundle : Resource
    {
        private List<Resource> m_Resources;

        public List<Resource> GetResources()
        {
            var list = new List<Resource>(m_Resources.Capacity);

            foreach (var item in m_Resources)
            {
                list.Add(item);
            }

            return list;
        }

        public T GetResource<T>()
            where T : Resource
        {
            ResourceType resType;
            if (!Constants.TypeToGfdResourceTypeMap.TryGetValue(typeof(T), out resType))
                throw new ArgumentException(nameof(T));

            return (T)m_Resources.Find(x => x.Type == resType);
        }

        public void AddResource(Resource resource)
        {
            switch (resource.Type)
            {
                case ResourceType.AnimationList:
                case ResourceType.TextureDictionary:
                case ResourceType.MaterialDictionary:
                case ResourceType.Scene:
                    if (m_Resources.Find(x => x.Type == resource.Type) == null)
                        m_Resources.Add(resource);
                    return;
            }

            throw new ArgumentException(nameof(resource));
        }

        public void AddResources(params Resource[] resources)
        {
            foreach (var item in resources)
            {
                AddResource(item);
            }
        }

        internal ResourceBundle(uint version)
            : base(ResourceType.ResourceBundle, version)
        {
            m_Resources = new List<Resource>();
        }
    }
}
