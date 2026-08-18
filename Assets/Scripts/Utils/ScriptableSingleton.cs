using UnityEngine;

namespace Timers
{
    public abstract class ScriptableSingleton<T>  where T : ScriptableObject
    {
        private static T _instance;

        protected static T Instance => _instance;

        
        protected static void LoadAsset()
        {
            if (_instance == null)
            {
                T[] results = Resources.LoadAll<T>("");
                if (results.Length > 0)
                {
                    _instance = results[0];
                    if (results.Length > 1) {
                        Debug.LogWarning($"Multiple instances of {typeof(T).Name} found in Resources. Using the first one.");
                    }
                }
                else {
                    Debug.LogError($"No asset of type {typeof(T).Name} found in Resources.");
                }
            }
        }
    }
}