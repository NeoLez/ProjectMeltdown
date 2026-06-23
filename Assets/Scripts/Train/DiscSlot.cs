using System;
using UnityEngine;

namespace Root
{
    public class DiscSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [SerializeField] private EmergencyStopButton _emergencyDisc;
        public BreakDisc Disc;

        private bool _animationEnd = false;

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
            PlayerItemHolder holder =
                GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            if (!holder.HasItem && Disc != null)
            {
                BreakDisc battery = TakeDisc();

                if (battery != null)
                {
                    PickupItem pickup =
                        battery.GetComponent<PickupItem>();

                    holder.Pickup(pickup);
                }
                return;
            }

            if (!holder.HasItem)
                return;

            Disc = holder.HeldItem.GetComponent<BreakDisc>();

            if (Disc == null)
                return;

            _emergencyDisc.Repair(Disc.DiscUsage);

            if (TryInsertDisc(Disc))
            {
                holder.ForceClearHeldItem();
            }
        }
        System.Collections.IEnumerator AnimTrigger(BreakDisc fluid)
        {
            yield return new WaitForSeconds(0.02f);
            //fluid.AnimatorOn();
            _animationEnd = true;
            yield return new WaitForSeconds(0.02f);
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
            _emergencyDisc.Repair(0);
            Disc = null;
            return _disc;
        }
        public bool TryInsertDisc(BreakDisc Disc)
        {

            Rigidbody rb = Disc.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.isKinematic = true;
            }

            Disc.transform.SetParent(transform);
            Disc.transform.position = pivot.position;
            Disc.transform.rotation = pivot.rotation;
            StartCoroutine(AnimTrigger(Disc));
            return true;
        }

        private void LateUpdate()
        {
            if (Disc != null && _animationEnd)
            {
                Disc.transform.position = pivot.position;
                Disc.transform.rotation = pivot.rotation;
            }
        }
    }
}
