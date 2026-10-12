using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Consumable Item")]
    public class ItemConsumableSo : ItemSo{
        public override ItemState CreateUninitializedState() {
            return new ItemConsumableState(this);
        }
    }
}