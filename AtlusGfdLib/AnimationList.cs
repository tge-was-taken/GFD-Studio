using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class AnimationPackage : Resource, IList<Animation>
    {
        private List<Animation> mAnimations;

        internal AnimationPackage(uint version)
            : base(ResourceType.AnimationList, version)
        {
            mAnimations = new List<Animation>();
        }

        #region IList implementation

        public Animation this[int index]
        {
            get
            {
                return mAnimations[index];
            }

            set
            {
                mAnimations[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return mAnimations.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Animation item)
        {
            mAnimations.Add(item);
        }

        public void Clear()
        {
            mAnimations.Clear();
        }

        public bool Contains(Animation item)
        {
            return mAnimations.Contains(item);
        }

        public void CopyTo(Animation[] array, int arrayIndex)
        {
            mAnimations.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Animation> GetEnumerator()
        {
            return mAnimations.GetEnumerator();
        }

        public int IndexOf(Animation item)
        {
            return mAnimations.IndexOf(item);
        }

        public void Insert(int index, Animation item)
        {
            mAnimations.Insert(index, item);
        }

        public bool Remove(Animation item)
        {
            return mAnimations.Remove(item);
        }

        public void RemoveAt(int index)
        {
            mAnimations.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mAnimations.GetEnumerator();
        }

        #endregion
    }
}