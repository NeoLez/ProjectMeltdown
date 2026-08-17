using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timers {
    
    public abstract class WeightedPool<T> : ScriptableObject {
        [Serializable] protected class PoolEntry {
            public T entry;
            public float weight;
        }

        [SerializeField] protected List<PoolEntry> entries = new();

        public T GetEntry() {
            float totalWeight = 0;

            foreach (var entry in entries)
                totalWeight += entry.weight;

            float randomWeight = UnityEngine.Random.Range(0, totalWeight);

            foreach (var entry in entries) {
                randomWeight -= entry.weight;
                if (randomWeight <= 0f) {
                    return entry.entry;
                }
            }

            return entries[^1].entry;
        }
    }
}