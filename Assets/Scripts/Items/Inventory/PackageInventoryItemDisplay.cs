using TMPro;
using UnityEngine;

namespace Root {
    public class PackageInventoryItemDisplay : InventoryItemDisplay {
        [SerializeField] private TMP_Text itemPriceText;
        [SerializeField] private TMP_Text destinationPriceText;
        public override void UpdateVisuals() {
            var state = (PackageItemState)_inventoryItem.itemState;
            itemPriceText.text = $"${state.price}";
            string destinationText = $"{state.DestinationNode.line}{state.DestinationNode.dist}";
            destinationPriceText.text = destinationText;
        }
    }
}