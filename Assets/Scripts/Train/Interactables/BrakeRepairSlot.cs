using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private ItemSo BrakeFluidItem;

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();
            if (holder == null || !holder.HasItem || holder.HeldItem.ItemSo != BrakeFluidItem || brakeController.GetDamageAmount() <= 0)
                return;

            if (TryInsertBrakeFluid(holder.HeldItem))
            {
                holder.ForceClearHeldItem();
            }
        }
        System.Collections.IEnumerator AnimTrigger(BrakeFluidItem fluid)
        {
            yield return new WaitForSeconds(0.02f);
            fluid.AnimatorOn();
            yield return new WaitForSeconds(2f);
            
            if(fluid.GetRepairAmountLeft() >= 0)
            {
                Debug.Log("Fluid should be returned to player or dropped, not implemented for now");
                Destroy(fluid.gameObject);
            }
            else
            {
                Destroy(fluid.gameObject);
            }
        }

        public bool TryInsertBrakeFluid(ItemState state)
        {
            BrakeFluidItem fluid = state.ItemSo.CreatePhysicalItem() as BrakeFluidItem;
            fluid.VisualOnly(true);
            
            VisualContainer visual = fluid.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();

            fluid.Consume(brakeController.GetDamageAmount());
            brakeController.Repair(-fluid.State.currentCharge);
            
            fluid.transform.SetParent(transform);
            fluid.transform.position = pivot.position;
            fluid.transform.rotation = pivot.rotation;
            StartCoroutine(AnimTrigger(fluid));
            return true;
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return item.itemState.ItemSo == BrakeFluidItem && brakeController.GetDamageAmount() > 0;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            return TryInsertBrakeFluid(item.itemState);
        }
    }

}
