using TMPro;
using UnityEngine;

namespace Root {
    public class PackageInventoryItemDisplay : InventoryItemDisplay {
        [SerializeField] private TMP_Text itemPriceText;
        public override void UpdateVisuals() {
            itemPriceText.text = $"${((PackageItemState)_inventoryItem.itemState).price}";
        }
    }
}