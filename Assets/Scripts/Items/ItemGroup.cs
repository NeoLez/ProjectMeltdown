using System.Collections.Generic;
using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "SO/ItemGroup")]
    public class ItemGroup : ScriptableObject {
        [SerializeField] private List<ItemSo> items = new ();
        
        public bool IsItemIncluded(ItemSo so) {
            return items.Contains(so);
        }
        
        public IEnumerable<ItemSo> Items => items;
    }
}