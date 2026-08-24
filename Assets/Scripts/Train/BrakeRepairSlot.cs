using NUnit.Framework;
using UnityEngine;

namespace Root
{
    public class BrakeRepairSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private ItemSo BrakeFluidItem;

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();
            Debug.Log("a");
            if (holder == null || !holder.HasItem)
                return;
            Debug.Log("b");
            Assert.AreEqual(holder.HeldItem.ItemSo, BrakeFluidItem);
            Debug.Log("c");
            BrakeFluidItem fluid = holder.HeldItem.ItemSo.CreatePhysicalItem() as BrakeFluidItem;
            fluid.VisualOnly(true);
            Debug.Log("d");
            if (fluid == null)
                return;
            Debug.Log("e");
            if(brakeController.GetDamageAmount() <= 0)
            {
                return;
            }
            Debug.Log("f");
            fluid.Consume(brakeController.GetDamageAmount());
            brakeController.Repair(fluid.State.currentCharge);

            if (TryInsertBrakeFluid(fluid, holder))
            {
                holder.ForceClearHeldItem();
            }
        }
        System.Collections.IEnumerator AnimTrigger(BrakeFluidItem fluid, PlayerItemHolder holder)
        {
            yield return new WaitForSeconds(0.02f);
            fluid.AnimatorOn();
            yield return new WaitForSeconds(2f);
            
            if(fluid.GetRepairAmountLeft() >= 0)
            {
                holder.Pickup(fluid);
            }
            else
            {
                Destroy(fluid.gameObject);
            }
        }

        public bool TryInsertBrakeFluid(BrakeFluidItem fluid, PlayerItemHolder holder)
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
