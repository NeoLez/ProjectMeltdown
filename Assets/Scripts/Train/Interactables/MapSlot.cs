using UnityEngine;

namespace Root
{
    public class MapSlot : InteractableNormalCamera, IItemDragReceiver
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
            //TODO: Really bad, fix this garbage later maybe by not using the physical item at all and just applying the map texture to another plane
            map.transform.localScale = Vector3.one;

            _map = null;
            return map;
        }

        public bool TryInsertMap(ItemState item)
        {
            if (_mapItemSO != item.ItemSo || _map != null) return false;

            PhysicalItem mapToInsert = item.ItemSo.CreatePhysicalItem(item);
            
            VisualContainer visual = mapToInsert.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();

            _map = mapToInsert;

            mapToInsert.VisualOnly(true);

            mapToInsert.transform.SetParent(transform);
            mapToInsert.transform.position = pivot.position;
            mapToInsert.transform.rotation = pivot.rotation;
            mapToInsert.transform.localScale = transform.localScale;
            
            return true;
        }


        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return _map == null && item.itemState.ItemSo == _mapItemSO;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            if (!CanTakeItem(position, InventoryItem.GetRotationCorrectedSize(item.Size, rotation), item)) return false;
            TryInsertMap(item.itemState);
            return true;
        }

        public void ClearFeedback()
        {
        }
    }
}
