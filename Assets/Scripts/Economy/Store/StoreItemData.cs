using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "SO/Store/Store Item")]
    public class StoreItemData : ScriptableObject {
        public ItemSo item;

        public int minPrice = 100;
        public int maxPrice = 150;
    }
}