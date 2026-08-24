using UnityEngine;

namespace Root
{
    public class DiscSlot : InteractableNormalCamera
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
                if (_disc == null) {
                    if(holder.HeldItem.ItemSo != BrakeDiscSO)
                        return;

                    var Disc = holder.HeldItem.ItemSo.CreatePhysicalItem() as BrakeDiscItem;
                    Disc.itemState = holder.HeldItem;
                    
                    _emergencyStopButton.Repair(Disc.GetDiscUsage());

                    TryInsertDisc(Disc);
                    holder.ForceClearHeldItem();
                }
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
        public void TryInsertDisc(BrakeDiscItem Disc) {
            _disc = Disc;

            _disc.VisualOnly(true);

            Disc.transform.SetParent(transform);
            Disc.transform.position = pivot.position;
            Disc.transform.rotation = pivot.rotation;
        }

        public BrakeDiscItem GetBreakDisc()
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
    }
}
