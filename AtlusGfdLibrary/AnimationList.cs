using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class AnimationList : Resource, IList<Animation>
    {
        private List<Animation> m_Animations;

        internal AnimationList(uint version)
            : base(ResourceType.AnimationList, version)
        {
            m_Animations = new List<Animation>();
        }

        #region IList implementation

        public Animation this[int index]
        {
            get
            {
                return m_Animations[index];
            }

            set
            {
                m_Animations[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Animations.Count;
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
            m_Animations.Add(item);
        }

        public void Clear()
        {
            m_Animations.Clear();
        }

        public bool Contains(Animation item)
        {
            return m_Animations.Contains(item);
        }

        public void CopyTo(Animation[] array, int arrayIndex)
        {
            m_Animations.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Animation> GetEnumerator()
        {
            return m_Animations.GetEnumerator();
        }

        public int IndexOf(Animation item)
        {
            return m_Animations.IndexOf(item);
        }

        public void Insert(int index, Animation item)
        {
            m_Animations.Insert(index, item);
        }

        public bool Remove(Animation item)
        {
            return m_Animations.Remove(item);
        }

        public void RemoveAt(int index)
        {
            m_Animations.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Animations.GetEnumerator();
        }

        #endregion
    }
}