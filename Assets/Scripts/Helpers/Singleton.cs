using UnityEngine;

namespace Helpers
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;
        
        public Singleton()
        {
            if (Instance is null)
                Instance = this as T;
        }
    }
}