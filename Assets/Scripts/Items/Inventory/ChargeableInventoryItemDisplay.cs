using UnityEngine;
using UnityEngine.UI;

namespace Root {
    public class ChargeableInventoryItemDisplay : InventoryItemDisplay {
        [SerializeField] private Slider _slider;

        public override void UpdateVisuals() {
            var state = ((ItemChargeState)_inventoryItem.itemState);
            _slider.value = state.currentCharge / state.maxCharge;
        }
    }
}