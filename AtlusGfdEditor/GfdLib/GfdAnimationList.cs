using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdAnimationList : GfdResource, IList<GfdAnimation>
    {
        private List<GfdAnimation> m_Animations;

        internal GfdAnimationList(uint version)
            : base(GfdResourceType.AnimationList, version)
        {
            m_Animations = new List<GfdAnimation>();
        }

        #region IList implementation

        public GfdAnimation this[int index]
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

        public void Add(GfdAnimation item)
        {
            m_Animations.Add(item);
        }

        public void Clear()
        {
            m_Animations.Clear();
        }

        public bool Contains(GfdAnimation item)
        {
            return m_Animations.Contains(item);
        }

        public void CopyTo(GfdAnimation[] array, int arrayIndex)
        {
            m_Animations.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GfdAnimation> GetEnumerator()
        {
            return m_Animations.GetEnumerator();
        }

        public int IndexOf(GfdAnimation item)
        {
            return m_Animations.IndexOf(item);
        }

        public void Insert(int index, GfdAnimation item)
        {
            m_Animations.Insert(index, item);
        }

        public bool Remove(GfdAnimation item)
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