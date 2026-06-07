using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class BatterySlot : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;

        [SerializeField] private IgnitionSwitch ignitionSwitch;
        [SerializeField] private VisualEffect _visualEffect;

        private Battery _battery;
        private VisualContainer _visualBattery;
        private bool _animationEnd = false;

        public event Action OnBatteryInserted;
        public event Action OnBatteryRemoved;

        private void OnTriggerEnter(Collider other)
        {
            if (_battery != null)
                return;

            if (other.gameObject.TryGetComponent<Battery>(out var battery) && other.gameObject.TryGetComponent<VisualContainer>(out var visual))
            {
                _battery = battery;
                _visualBattery = visual;
                var rb = battery.GetComponent<Rigidbody>();

                rb.constraints = RigidbodyConstraints.FreezeAll; _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;

                OnBatteryInserted?.Invoke();
                StartCoroutine(AnimTrigger(_visualBattery));

                if (ignitionSwitch.IsEngineOn()) { train.SetEnginePower(true); }
            }
        }
        System.Collections.IEnumerator AnimTrigger(VisualContainer battery)
        {
            battery.PlayAnimation(true);

            yield return new WaitForSeconds(0.70f);

            _visualEffect.SendEvent("OnPlay");
            yield return new WaitForSeconds(0.10f);

            _animationEnd = true;
            battery.PlayAnimation(false);
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

        public bool TryInsertBattery( Battery battery, VisualContainer visual)
        {
            if (_battery != null)
                return false;

            _battery = battery;
            _visualBattery = visual;

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

            StartCoroutine(AnimTrigger(_visualBattery));

            if (ignitionSwitch.IsEngineOn())
            {
                train.SetEnginePower(true);
            }

            return true;
        }
    }
}