using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(PickupItem))]
    public class PickupInteractable : InteractableNormalCamera
    {
        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            PickupItem item = GetComponent<PickupItem>();

            if (holder.HasItem)
            {
                holder.Drop();
                return;
            }

            holder.Pickup(item);
        }
    }
}