using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(fileName = "ItemGenerationPool", menuName = "SO/ItemGenerationPool")]
    public class ItemGenerationPoolSO : ScriptableObject {
        public List<ItemSO> items;

        public ItemSO GetRandom() {
            return items[Random.Range(0, items.Count)];
        }
    }
}