using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace Root
{
    public class GeneratorSlot : InteractableNormalCamera
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private VisualEffect visualEffect;

        [SerializeField] private float batteryDrain = 0.5f;

        [Header("Emergency Lights")]
        [SerializeField] private Flicker emergencyLight; 

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundInsert;
        [SerializeField] private AudioClip _soundRemove;

        private TrainBatteryItem _battery;
        private bool _animationEnd;

        public event Action OnPowerRestored;
        public event Action OnPowerLost;

        private bool _powered = false;
        
        [SerializeField] private ItemSo _batteryItemSO;

        private void Start()
        {
            SetEmergencyLights(true); 
        }

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();
            if (holder == null) return;

            if (!holder.HasItem && _battery != null)
            {
                TrainBatteryItem battery = TakeBattery();
                if (battery != null)
                {
                    holder.Pickup(battery);
                }
                return;
            }

            if (!holder.HasItem) return;

            Assert.AreEqual(_batteryItemSO, holder.HeldItem.ItemSo);
            TrainBatteryItem batteryToInsert = holder.HeldItem.ItemSo.CreatePhysicalItem() as TrainBatteryItem;
            if (batteryToInsert == null) return;
            batteryToInsert.itemState = holder.HeldItem;

            VisualContainer visual = batteryToInsert.GetComponentInChildren<VisualContainer>();
            if (visual == null) return;

            if (TryInsertBattery(batteryToInsert))
            {
                holder.ForceClearHeldItem();
            }
        }

        private void Update()
        {
            if (_battery == null || _battery.State.currentCharge <= 0f)
            {
                if (_powered)
                {
                    _powered = false;

                    SetEmergencyLights(true); 
                    OnPowerLost?.Invoke();
                }

                return;
            }

            _battery.State.currentCharge -= batteryDrain * Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_battery != null && _animationEnd)
            {
                _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;
            }
        }

        public bool TryInsertBattery(TrainBatteryItem battery)
        {
            if (_battery != null)
                return false;

            _battery = battery;

            battery.VisualOnly(true);

            battery.transform.SetParent(transform);
            battery.transform.position = pivot.position;
            battery.transform.rotation = pivot.rotation;

            StartCoroutine(AnimTrigger(battery));

            return true;
        }

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null)
                return null;

            TrainBatteryItem battery = _battery;

            battery.VisualOnly(false);

            _battery = null;
            _animationEnd = false;

            if (_powered)
            {
                _powered = false;

                SetEmergencyLights(true); 
                OnPowerLost?.Invoke();
            }

            if (_soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);

            return battery;
        }

        public TrainBatteryItem GetBattery() => _battery;

        System.Collections.IEnumerator AnimTrigger(TrainBatteryItem battery)
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

                SetEmergencyLights(false);
                OnPowerRestored?.Invoke();
            }

            yield return new WaitForSeconds(0.10f);

            _animationEnd = true;
        }

        private void SetEmergencyLights(bool active)
        {
            if (emergencyLight != null)
                emergencyLight.enabled = active;
        }
    }
}