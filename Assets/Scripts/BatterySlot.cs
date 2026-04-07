using System;
using UnityEngine;

namespace Root {
    public class BatterySlot : MonoBehaviour {
        [SerializeField] private Transform pivot;
        private Battery _battery;
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.TryGetComponent<Battery>(out var battery)) {
                _battery = battery;
                var rb = battery.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.position = pivot.position;
            }
        }

        private void LateUpdate() {
            if (_battery != null) _battery.transform.position = pivot.position;
        }

        public Battery GetBattery() {
            return _battery;
        }
    }
}