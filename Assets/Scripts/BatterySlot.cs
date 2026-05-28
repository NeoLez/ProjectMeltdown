using System;
using UnityEngine;

namespace Root
{
    public class BatterySlot : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;

        [SerializeField] private IgnitionSwitch ignitionSwitch;

        private Battery _battery;

        public event Action OnBatteryInserted;
        public event Action OnBatteryRemoved;

        private void OnTriggerEnter(Collider other)
        {
            if (_battery != null)
                return;

            if (other.gameObject.TryGetComponent<Battery>(out var battery))
            {
                _battery = battery;

                var rb = battery.GetComponent<Rigidbody>();

                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.position = pivot.position;

                OnBatteryInserted?.Invoke();

                if (ignitionSwitch.IsEngineOn())
                {
                    train.SetEnginePower(true);
                }
            }
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
            if (_battery != null)
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

        public Battery GetBattery()
        {
            return _battery;
        }
    }
}