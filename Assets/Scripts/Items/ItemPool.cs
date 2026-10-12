using System;
using Root.Factories;
using Timers;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/ItemPool")]
    public class ItemPool : WeightedPool<ItemPool.ItemPoolEntry> {
        [Serializable]
        public struct ItemPoolEntry {
            public ItemSo So;
            public ItemStateInitializer Initializer;
        }
        
        
        
    }
}