using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class HealingInventoryItemDisplay : InventoryItemDisplay
    {
        private ItemAction _healAction;

        public override void Initialize(InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position,
            InventoryItem.InventoryItemRotation rotation)
        {
            base.Initialize(item, itemIcon, size, position, rotation);
            _healAction = new ItemAction("Heal", Heal);
        }

        public override List<ItemAction> GetItemActions()
        {
            var actions = base.GetItemActions();
            actions.Add(_healAction);
            return actions;
        }

        private void Heal()
        {
            var itemSo = (HealingItemSo)_inventoryItem.itemState.ItemSo;
            var health = GameManager.Player.GetComponent<HealthControl>();
            if (!health.CanCure()) return;
            _inventoryItem.Inventory.RemoveItem(_inventoryItem);
            health.Heal(itemSo.HealAmount);
            health.Cure(itemSo.InfectionHealAmount);
        }
    }
}