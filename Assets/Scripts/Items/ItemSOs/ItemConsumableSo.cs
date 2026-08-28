using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Consumable Item")]
    public class ItemConsumableSo : ItemSo{
        public override ItemState CreateState() {
            return new ItemConsumableState(this);
        }
    }
}