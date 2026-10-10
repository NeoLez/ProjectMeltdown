using System.Collections.Generic;
using UnityEngine;

namespace Root {
    public class BillInventoryItemDisplay : InventoryItemDisplay {
        private ItemAction _putInWallet;
        
        public override void Initialize(InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            base.Initialize(item, itemIcon, size, position, rotation);
            _putInWallet = new("Put in wallet", PutInWallet);
        }

        private void PutInWallet() {
            var itemSo = (BillItemSo)_inventoryItem.itemState.ItemSo;
            _inventoryItem.Inventory.RemoveItem(_inventoryItem);
            EconomyManager.Instance.AddMoney(itemSo.BillDenomination);
            MoneyFeedback.Instance.GrabbedBill(itemSo);
            Wallet.Instance.AddBill(itemSo);
        }

        public override List<ItemAction> GetItemActions() {
            var actions = base.GetItemActions();
            actions.Add(_putInWallet);
            return actions;
        }
    }
}