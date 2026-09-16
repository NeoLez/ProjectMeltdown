using System.Collections.Generic;
using Root.Log;
using UnityEngine;
using LogType = Root.Log.LogType;

namespace Root.Managers {
    public static class PoolManager {
        private static readonly Dictionary<Poolable, Queue<Poolable>> Pools = new();
        
        public static Poolable GetObject(Poolable prefab) {
            if (!Pools.TryGetValue(prefab, out var pool)) {
                pool = new();
                Pools[prefab] = pool;
                Log.Logger.Log("Pool not found", LogType.Pool);
            }
            if (!pool.TryDequeue(out Poolable obj)) {
                obj = GameObject.Instantiate(prefab);
                obj.SetPrefab(prefab);
                obj.Initialize();
                Log.Logger.Log("Pool empty, instantiating new object", LogType.Pool);
            }
            Log.Logger.Log(obj.GetPrefab().name, LogType.Pool);
            obj.TurnOn();
            return obj;
        }

        public static void ReturnObjectToPool(Poolable obj) {
            Log.Logger.Log(obj.GetPrefab().name,  LogType.Pool);
            obj.TurnOff();
            if (!Pools.TryGetValue(obj.GetPrefab(), out var pool)) {
                Log.Logger.Log("Trying to return a poolable object to a non existent pool", LogType.Pool, LogSeverity.Warning);
                pool = new();
                Pools[obj.GetPrefab()] = pool;
            }
            pool.Enqueue(obj);
            Log.Logger.Log(pool.Count.ToString(), LogType.Pool);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Reset() {
            Pools.Clear();
        }
    }
}