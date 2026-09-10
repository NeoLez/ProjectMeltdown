using System.Collections.Generic;
using UnityEngine;

namespace Root.Managers {
    public static class PoolManager {
        private static readonly Dictionary<Poolable, Queue<Poolable>> Pools = new();
        
        public static Poolable GetObject(Poolable prefab) {
            if (!Pools.TryGetValue(prefab, out var pool)) {
                pool = new();
                Pools[prefab] = pool;
                Debug.Log("Pool not found");
            }
            if (!pool.TryDequeue(out Poolable obj)) {
                obj = GameObject.Instantiate(prefab);
                obj.SetPrefab(prefab);
                obj.Initialize();
                Debug.Log("Pool empty, instantiating new object");
            }
            Debug.Log(obj.GetPrefab());
            obj.TurnOn();
            return obj;
        }

        public static void ReturnObjectToPool(Poolable obj) {
            Debug.Log(obj.GetPrefab());
            obj.TurnOff();
            if (!Pools.TryGetValue(obj.GetPrefab(), out var pool)) {
                Debug.LogWarning("Trying to return a poolable object to a non existent pool");
                pool = new();
                Pools[obj.GetPrefab()] = pool;
            }
            pool.Enqueue(obj);
            Debug.Log(pool.Count);
        }
    }
}