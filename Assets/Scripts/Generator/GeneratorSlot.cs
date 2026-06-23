using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class GeneratorSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private VisualEffect visualEffect;

        [SerializeField] private float batteryDrain = 0.5f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundInsert;
        [SerializeField] private AudioClip _soundRemove;

        private Battery _battery;
        private bool _animationEnd;

        public event Action OnPowerRestored;
        public event Action OnPowerLost;

        private bool _powered = false;

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();
            if (holder == null) return;

            if (!holder.HasItem && _battery != null)
            {
                Battery battery = TakeBattery();
                if (battery != null)
                {
                    PickupItem pickup = battery.GetComponent<PickupItem>();
                    holder.Pickup(pickup);
                }
                return;
            }

            if (!holder.HasItem) return;

            Battery batteryToInsert = holder.HeldItem.GetComponent<Battery>();
            if (batteryToInsert == null) return;

            VisualContainer visual = batteryToInsert.GetComponentInChildren<VisualContainer>();
            if (visual == null) return;

            if (TryInsertBattery(batteryToInsert))
            {
                holder.ForceClearHeldItem();
            }
        }

        private void Update()
        {
            if (_battery == null || _battery.energy <= 0f)
            {
                if (_powered)
                {
                    _powered = false;
                    OnPowerLost?.Invoke();
                }
                return;
            }

            _battery.energy -= batteryDrain * Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_battery != null && _animationEnd)
            {
                _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;
            }
        }

        public bool TryInsertBattery(Battery battery)
        {
            if (_battery != null) return false;

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

            StartCoroutine(AnimTrigger(battery));
            return true;
        }

        public Battery TakeBattery()
        {
            if (_battery == null) return null;

            Battery battery = _battery;

            Rigidbody rb = battery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;
            }

            _battery = null;
            _animationEnd = false;

            if (_powered)
            {
                _powered = false;
                OnPowerLost?.Invoke();
            }

            if (_soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);

            return battery;
        }

        public Battery GetBattery() => _battery;

        System.Collections.IEnumerator AnimTrigger(Battery battery)
        {
            _animationEnd = false;
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);
            if (visualEffect != null)
                visualEffect.SendEvent("OnPlay");
            if (_soundInsert != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundInsert, transform.position, GameManager.AudioSystem.VFX);
            if (!_powered)
            {
                _powered = true;
                OnPowerRestored?.Invoke();
            }
            yield return new WaitForSeconds(0.10f);
            _animationEnd = true;
        }
    }
}