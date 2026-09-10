using UnityEngine;

namespace Root
{
    public class MapSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;

        [SerializeField] private ItemSo _mapItemSO;

        private PhysicalItem _map;
        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            if (!holder.HasItem && _map != null)
            {
                PhysicalItem map = TakeMap();

                if (map != null)
                {
                    holder.Pickup(map);
                }

                return;
            }

            if (!holder.HasItem)
                return;


            if (TryInsertMap(holder.HeldItem))
            {
                holder.ForceClearHeldItem();
            }
        }


        private void LateUpdate()
        {
            if (_map != null)
            {
                _map.transform.position = pivot.position;
                _map.transform.rotation = pivot.rotation;
            }
        }

        public PhysicalItem TakeMap()
        {
            if (_map == null)
                return null;

            PhysicalItem map = _map;

            map.VisualOnly(false);

            _map = null;
            return map;
        }

        public bool TryInsertMap(ItemState item)
        {
            if (_mapItemSO != item.ItemSo || _map != null) return false;

            PhysicalItem mapToInsert = item.ItemSo.CreatePhysicalItem();
            mapToInsert.itemState = item;


            VisualContainer visual = mapToInsert.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();

            _map = mapToInsert;

            mapToInsert.VisualOnly(true);

            mapToInsert.transform.SetParent(transform);
            mapToInsert.transform.position = pivot.position;
            mapToInsert.transform.rotation = pivot.rotation;

            return true;
        }


        public void ClearFeedback()
        {
        }
    }
}
