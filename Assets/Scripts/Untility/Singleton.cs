using UnityEngine;
using System.Collections;

namespace Untility
{
    public abstract class Singleton<T> where T : new()
    {
        static T m_Instance;
        
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }
    }
}
