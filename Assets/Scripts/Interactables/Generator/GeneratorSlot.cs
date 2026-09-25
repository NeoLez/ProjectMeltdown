using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace Root
{
    public class GeneratorSlot : InteractableNormalCamera, IItemDragReceiver
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
            if (_batteryItemSO != holder.HeldItem.ItemSo) return;
            
            if (TryInsertBattery(holder.HeldItem))
            {
                holder.ForceClearHeldItem();
            }
        }

        private void Update()
        {
            if (_battery == null || _battery.So.currentCharge <= 0f)
            {
                if (_powered)
                {
                    _powered = false;
                    SetEmergencyLights(true); 
                    OnPowerLost?.Invoke();
                }
                return;
            }

            _battery.So.currentCharge -= batteryDrain * Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_battery != null && _animationEnd)
            {
                _battery.transform.position = pivot.position;
                _battery.transform.rotation = pivot.rotation;
            }
        }

        public bool TryInsertBattery(ItemState item)
        {
            if (_batteryItemSO != item.ItemSo || _battery != null) return false;

            TrainBatteryItem batteryToInsert = (TrainBatteryItem)item.ItemSo.CreatePhysicalItem(item);
            VisualContainer visual = batteryToInsert.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();
            _battery = batteryToInsert;
            _battery.VisualOnly(true);
            _battery.transform.SetParent(transform);
            _battery.transform.position = pivot.position;
            _battery.transform.rotation = pivot.rotation;
            StartCoroutine(AnimTrigger(_battery));
            return true;
        }

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null || !_animationEnd) return null;

            TrainBatteryItem battery = _battery;
            battery.GetComponent<BatteryLightsIndicator>()?.SetDisplayActive(false);
            battery.VisualOnly(false);
            _battery = null;
            _animationEnd = false;
            bool hadPower = _powered; 

            if (_powered)
            {
                _powered = false;

                SetEmergencyLights(true);
                OnPowerLost?.Invoke();
            }
            if (hadPower && _soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);

            return battery;
        }

        public TrainBatteryItem GetBattery() => _battery;

        System.Collections.IEnumerator AnimTrigger(TrainBatteryItem battery)
        {
            _animationEnd = false;
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);

            if (_battery != battery) yield break; 

            if (!_powered && battery.So.currentCharge > 0f) // no hay efecto, sonido ni energía
            {
                if (visualEffect != null) 
                    visualEffect.SendEvent("OnPlay");

                if (_soundInsert != null) 
                    GameManager.AudioSystem.PlaySoundPositional(_soundInsert, transform.position, GameManager.AudioSystem.VFX);

                _powered = true;
                SetEmergencyLights(false);
                OnPowerRestored?.Invoke();
            }

            yield return new WaitForSeconds(0.10f);

            if (_battery != battery) yield break; 

            _animationEnd = true;
            battery.GetComponent<BatteryLightsIndicator>()?.SetDisplayActive(true);
        }

        private void SetEmergencyLights(bool active)
        {
            if (emergencyLight != null)
                emergencyLight.enabled = active;
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return item.itemState.ItemSo == _batteryItemSO && _battery == null;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            return TryInsertBattery(item.itemState);
        }

        public void ClearFeedback() {
        }
    }
}