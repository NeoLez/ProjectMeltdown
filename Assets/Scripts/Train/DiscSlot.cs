using UnityEngine;
using UnityEngine.Serialization;

namespace Root
{
    public class DiscSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [SerializeField] private EmergencyStopButton _emergencyStopButton;
        private BreakDisc _disc;

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
                    BreakDisc disc = TakeDisc();

                    if (disc != null) {
                        PickupItem pickup =
                            disc.GetComponent<PickupItem>();

                        holder.Pickup(pickup);
                    }
                }
            }
            else {
                if (_disc == null) {
                    if(!holder.HeldItem.TryGetComponent(out BreakDisc Disc))
                        return;

                    _emergencyStopButton.Repair(Disc.GetDiscUsage());

                    if (TryInsertDisc(Disc))
                    {
                        holder.ForceClearHeldItem();
                    }
                }
            }
        }
        
        public BreakDisc TakeDisc()
        {
            if (this._disc == null)
                return null;

            BreakDisc disc = _disc;

            var rb = disc.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;
            }
            _emergencyStopButton.Repair(0);
            this._disc = null;
            return disc;
        }
        public bool TryInsertDisc(BreakDisc Disc) {
            this._disc = Disc;

            Rigidbody rb = Disc.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.isKinematic = true;
            }

            Disc.transform.SetParent(transform);
            Disc.transform.position = pivot.position;
            Disc.transform.rotation = pivot.rotation;
            return true;
        }

        public BreakDisc GetBreakDisc()
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
