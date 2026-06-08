using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : InteractableNormalCamera
    {
        [SerializeField] private TrainBrakeController brakeController;
        public override void Interact()
        {
            PlayerItemHolder holder =
                GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            if (!holder.HasItem)
                return;

            BrakeFluid fluid =
                holder.HeldItem.GetComponent<BrakeFluid>();

            if (fluid == null)
                return;

            brakeController.Repair(fluid.repairAmount);

            holder.ForceClearHeldItem();

            Destroy(fluid.gameObject);
        }
    }

}
