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
        private bool _animationEnd = false;

        public event Action OnBatteryInserted;
        public event Action OnBatteryRemoved;

        private void OnTriggerEnter(Collider other)
        {
            if (_battery != null)
                return;

            if (other.gameObject.TryGetComponent<Battery>(out var battery))
            {
                _battery = battery;
                Animator anim = _battery.GetComponentInChildren<Animator>(); //la forma mas clunky de hacer esto, perdonen chicos </3
                var rb = battery.GetComponent<Rigidbody>();

                rb.constraints = RigidbodyConstraints.FreezeAll; _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;

                OnBatteryInserted?.Invoke();
                StartCoroutine(AnimTrigger(anim));
            }
        }
        System.Collections.IEnumerator AnimTrigger(Animator anim)
        {
            anim.enabled = true;
            yield return new WaitForSeconds(1f);

            _animationEnd = true;

            if (ignitionSwitch.IsEngineOn()) { train.SetEnginePower(true); }
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

        public Battery GetBattery()
        {
            return _battery;
        }
    }
}