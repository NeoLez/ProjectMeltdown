using UnityEngine;

namespace Root
{
    public class BrakeRepairSlotInteractable : InteractableNormalCamera
    {
        [SerializeField] private BrakeRepairSlot repairSlot;

        public override void Interact()
        {
            PlayerItemHolder holder =
                GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null || !holder.HasItem)
                return;

            BrakeFluid fluid =
                holder.HeldItem.GetComponent<BrakeFluid>();

            if (fluid == null)
                return;

            if (repairSlot.TryUseBrakeFluid(fluid))
            {
                holder.ForceClearHeldItem();
            }
        }
    }
}