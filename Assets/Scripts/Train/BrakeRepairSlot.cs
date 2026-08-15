using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
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

            if(brakeController.GetDamageAmount() <= 0)
            {
                return;
            }

            fluid.Consume(brakeController.GetDamageAmount());
            brakeController.Repair(fluid.RepairAmount);

            if (TryInsertBrakeFluid(fluid, holder))
            {
                holder.ForceClearHeldItem();
            }
        }
        System.Collections.IEnumerator AnimTrigger(BrakeFluid fluid, PlayerItemHolder holder)
        {
            PickupItem item = fluid.GetComponent<PickupItem>();

            yield return new WaitForSeconds(0.02f);
            fluid.AnimatorOn();
            yield return new WaitForSeconds(2f);
            
            if(fluid.GetRepairAmountLeft() >= 0)
            {
                Rigidbody rb = fluid.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.isKinematic = false;
                }

                holder.Pickup(item);
            }
            else
            {
                Destroy(fluid.gameObject);
            }
        }

        public bool TryInsertBrakeFluid(BrakeFluid fluid, PlayerItemHolder holder)
        {
            Rigidbody rb = fluid.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.isKinematic = true;
            }

            fluid.transform.SetParent(transform);
            fluid.transform.position = pivot.position;
            fluid.transform.rotation = pivot.rotation;
            StartCoroutine(AnimTrigger(fluid, holder));
            return true;
        }

    }

}
