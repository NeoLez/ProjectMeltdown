using UnityEngine;
using UnityEngine.Serialization;

namespace Root
{
    public class DiscSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [FormerlySerializedAs("_emergencyDisc")] [SerializeField] private EmergencyStopButton _emergencyStopButton;
        public BreakDisc Disc;

        private void Awake() {
            train.OnTrainStartedMoving += (() => {
                if (Disc == null) return;
                Disc.GetComponent<VisualContainer>().goal = train.GetTrainPosition();
            });
            train.OnTrainStoppedMoving += (() => {
                if (Disc == null) return;
                Disc.GetComponent<VisualContainer>().goal = null;
            });
        }

        public override void Interact()
        {
            if (!GameManager.Player.TryGetComponent(out PlayerItemHolder holder))
                return;

            if (!holder.HasItem) {
                if (Disc != null) {
                    BreakDisc disc = TakeDisc();

                    if (disc != null) {
                        PickupItem pickup =
                            disc.GetComponent<PickupItem>();

                        holder.Pickup(pickup);
                    }
                }
            }
            else {
                if (Disc == null) {
                    if(!holder.HeldItem.TryGetComponent(out BreakDisc Disc))
                        return;

                    _emergencyStopButton.Repair(Disc.DiscUsage);

                    if (TryInsertDisc(Disc))
                    {
                        holder.ForceClearHeldItem();
                    }
                }
            }
        }
        
        public BreakDisc TakeDisc()
        {
            if (Disc == null)
                return null;

            BreakDisc _disc = Disc;

            var rb = _disc.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;
            }
            _emergencyStopButton.Repair(0);
            Disc = null;
            return _disc;
        }
        public bool TryInsertDisc(BreakDisc Disc) {
            this.Disc = Disc;

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

        private void LateUpdate()
        {
            if (Disc != null)
            {
                Disc.transform.position = pivot.position;
                Disc.transform.rotation = pivot.rotation;
            }
        }
    }
}
