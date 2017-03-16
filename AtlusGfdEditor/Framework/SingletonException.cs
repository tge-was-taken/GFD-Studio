using System;

namespace AtlusGfdEditor.Framework
{
    [Serializable]
    class SingletonException : Exception
    {
        public SingletonException(string className)
            : base($"Attempted to create a new instance of singleton {className}")
        {
        }
    }
}
