using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class BatterySlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [SerializeField] private IgnitionSwitch ignitionSwitch;
        [SerializeField] private VisualEffect visualEffect;
        private Battery _battery;
        private bool _animationEnd;
        public event Action OnBatteryInserted;
        public event Action OnBatteryRemoved;

        public override void Interact()
        {
            PlayerItemHolder holder =
                GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            // Retirar bateria
            if (!holder.HasItem && _battery != null)
            {
                Battery battery = TakeBattery();

                if (battery != null)
                {
                    PickupItem pickup =
                        battery.GetComponent<PickupItem>();

                    holder.Pickup(pickup);
                }

                return;
            }

            // Insertar bateria
            if (!holder.HasItem)
                return;

            Battery batteryToInsert =
                holder.HeldItem.GetComponent<Battery>();

            if (batteryToInsert == null)
                return;

            VisualContainer visual =
                batteryToInsert.GetComponentInChildren<VisualContainer>();

            if (visual == null)
                return;

            if (TryInsertBattery(batteryToInsert))
            {
                holder.ForceClearHeldItem();
            }
        }

        System.Collections.IEnumerator AnimTrigger(Battery battery)
        {
            yield return new WaitForSeconds(0.02f);
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);
            visualEffect.SendEvent("OnPlay");
            yield return new WaitForSeconds(0.10f);

            _animationEnd = true;
        }

        private void Update()
        {
            if (_battery == null)
            {
                train.SetEnginePower(false);
            }
        }

        private void LateUpdate()
        {
            if (_battery != null && _animationEnd)
            {
                _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;
            }
        }

        public void RemoveBattery()
        {
            if (_battery == null)
                return;

            var rb = _battery.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            _battery = null;
            train.SetEnginePower(false);
            OnBatteryRemoved?.Invoke();
        }

        public Battery TakeBattery()
        {
            if (_battery == null)
                return null;

            Battery battery = _battery;

            var rb = battery.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;
            }

            _battery = null;
            train.SetEnginePower(false);
            OnBatteryRemoved?.Invoke();
            return battery;
        }

        public Battery GetBattery()
        {
            return _battery;
        }

        public bool TryInsertBattery(Battery battery)
        {
            if (_battery != null)
                return false;

            _battery = battery;

            Rigidbody rb = battery.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.isKinematic = true;
            }

            battery.transform.SetParent(transform);
            battery.transform.position = pivot.position;
            battery.transform.rotation = pivot.rotation;
            OnBatteryInserted?.Invoke();
            StartCoroutine(AnimTrigger(battery));

            if (ignitionSwitch.IsEngineOn())
            {
                train.SetEnginePower(true);
            }

            return true;
        }
    }
}
