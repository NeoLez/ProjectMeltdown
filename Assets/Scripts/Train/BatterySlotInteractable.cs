using UnityEngine;

namespace Root
{
    public class BatterySlotInteractable : InteractableNormalCamera
    {
        [SerializeField] private BatterySlot batterySlot;

        public override void Interact()
        {
            PlayerItemHolder holder =
                GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null || !holder.HasItem)
                return;

            Battery battery =
                holder.HeldItem.GetComponent<Battery>();

            if (battery == null)
                return;

            VisualContainer visual =
                battery.GetComponent<VisualContainer>();

            if (visual == null)
                return;

            if (batterySlot.TryInsertBattery(battery, visual))
            {
                holder.ForceClearHeldItem();
            }
        }
    }
}