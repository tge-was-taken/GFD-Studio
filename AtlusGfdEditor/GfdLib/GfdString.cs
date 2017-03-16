namespace AtlusGfdEditor.GfdLib
{
    public struct GfdString
    {
        private readonly string m_String;
        private uint m_Hash;
        private bool m_IsHashCalculated;

        public GfdString(string value)
        {
            m_String = value;
            m_Hash = 0;
            m_IsHashCalculated = false;
        }

        internal GfdString(string value, uint hash)
        {
            m_String = value;
            m_Hash = hash;
            m_IsHashCalculated = true;
        }

        public override int GetHashCode()
        {
            if (!m_IsHashCalculated)
            {
                m_Hash = GfdStringHasher.GenerateStringHash(m_String);
                m_IsHashCalculated = true;
            }

            return (int)m_Hash;
        }

        public override string ToString()
        {
            return m_String;
        }

        public static implicit operator string(GfdString value)
        {
            if (value == null)
                return null;

            return value.m_String;
        }

        public static implicit operator GfdString(string value)
        {
            if (value == null)
                return null;

            return new GfdString(value);
        }
    }
}
