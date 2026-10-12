using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Chargeable Item")]
    public class ItemChargeStateSo : ItemSo {
        public override ItemState CreateUninitializedState() {
            return new ItemChargeState(this);
        }
    }
}