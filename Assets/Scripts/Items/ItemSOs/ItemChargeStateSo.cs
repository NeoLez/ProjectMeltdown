using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Chargeable Item")]
    public class ItemChargeStateSo : ItemSo {
        public override ItemState CreateState() {
            return new ItemChargeState(this);
        }
    }
}