using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Store/Store Item")]
    public class StoreItemData : ScriptableObject
    {
        public string itemName;

        public GameObject prefab;

        public int minPrice = 100;
        public int maxPrice = 150;

        [Tooltip("Mientras mas alto sea el weight = mas fracuente sera el objeto en la tienda")]
        public int weight = 1;
    }
}