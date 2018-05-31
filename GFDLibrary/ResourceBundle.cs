using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GFDLibrary
{
    public class ResourceBundle : Resource
    {
        private readonly List<Resource> mResources;

        public ResourceBundle( uint version ) : base( ResourceType.Bundle, version )
        {
            mResources = new List< Resource >();
        }

        public Resource GetResource( ResourceType type )
        {
            return mResources.SingleOrDefault( x => x.Type == type );
        }

        public TResource GetResource<TResource>() where TResource : Resource
        {
            return ( TResource )mResources.Single( x => x is TResource );
        }

        public ReadOnlyCollection< Resource > GetResources( ResourceType type )
        {
            return mResources.Where( x => x.Type == type )
                             .ToList()
                             .AsReadOnly();
        }

        public ReadOnlyCollection< TResource > GetResources< TResource >()
        {
            return mResources.Where( x => x is TResource )
                             .Cast< TResource >()
                             .ToList()
                             .AsReadOnly();
        }

        public ReadOnlyCollection<Resource> GetResources()
        {
            return mResources.AsReadOnly();
        }

        public void AddResource( Resource resource )
        {
            mResources.Add( resource );
        }

        public void RemoveResource( Resource resource )
        {
            mResources.Remove( resource );
        }

        public void ReplaceResource( Resource resource )
        {
            for ( int i = 0; i < mResources.Count; i++ )
            {
                if ( mResources[i].Type == resource.Type )
                    mResources[i] = resource;
            }
        }
    }
}