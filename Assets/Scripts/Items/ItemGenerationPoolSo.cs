using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(fileName = "ItemGenerationPool", menuName = "SO/ItemGenerationPool")]
    public class ItemGenerationPoolSo : ScriptableObject {
        public List<ItemSo> items;

        public ItemSo GetRandom() {
            return items[Random.Range(0, items.Count)];
        }
        
        public ItemSo GetRandom(System.Random random) {
            return items[random.Next(0, items.Count)];
        }
    }
}