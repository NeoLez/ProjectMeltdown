using UnityEngine;

namespace Root
{
    public class DiscSlot : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [SerializeField] private EmergencyStopButton _emergencyStopButton;
        [SerializeField] private ItemSo BrakeDiscSO;
        private BrakeDiscItem _disc;

        private void Awake() {
            train.OnTrainStartedMoving += (() => {
                if (_disc == null) return;
                _disc.GetComponent<VisualContainer>().goal = train.GetTrainPosition();
            });
            train.OnTrainStoppedMoving += (() => {
                if (_disc == null) return;
                _disc.GetComponent<VisualContainer>().goal = null;
            });
        }

        public override void Interact()
        {
            if (!GameManager.Player.TryGetComponent(out PlayerItemHolder holder))
                return;

            if (!holder.HasItem) {
                if (_disc != null) {
                    BrakeDiscItem disc = TakeDisc();

                    if (disc != null) {
                        holder.Pickup(disc);
                    }
                }
            }
            else {
                if(_disc != null || holder.HeldItem.ItemSo != BrakeDiscSO)
                    return;

                TryInsertDisc(holder.HeldItem);
                holder.ForceClearHeldItem();
            }
        }
        
        public BrakeDiscItem TakeDisc()
        {
            if (_disc == null)
                return null;

            BrakeDiscItem disc = _disc;

            _disc.VisualOnly(false);
            
            _emergencyStopButton.Repair(0);
            _disc = null;
            return disc;
        }
        public void TryInsertDisc(ItemState state) {
            var Disc = state.ItemSo.CreatePhysicalItem() as BrakeDiscItem;
            Disc.itemState = state;
                    
            VisualContainer visual = Disc.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();
                    
            _emergencyStopButton.Repair(Disc.GetDiscUsage());
            
            _disc = Disc;

            _disc.VisualOnly(true);

            Disc.transform.SetParent(transform);
            Disc.transform.position = pivot.position;
            Disc.transform.rotation = pivot.rotation;
        }

        public BrakeDiscItem GetBrakeDisc()
        {
            return _disc;
        }

        private void LateUpdate()
        {
            if (_disc != null)
            {
                _disc.transform.position = pivot.position;
                _disc.transform.rotation = pivot.rotation;
            }
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return _disc == null && item.itemState.ItemSo == BrakeDiscSO;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            TryInsertDisc(item.itemState);
            return true;
        }

        public void ClearFeedback() {
        }
    }
}
