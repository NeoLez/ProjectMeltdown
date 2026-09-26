using System;
using Root.Managers;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class BatterySlot : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Train train;
        [SerializeField] private IgnitionSwitch ignitionSwitch;
        [SerializeField] private VisualEffect visualEffect;
        private TrainBatteryItem _battery;
        private bool _animationEnd;
        public event Action OnBatteryInserted;
        public event Action OnBatteryRemoved;
        public bool PowerReady { get; set; }

        [SerializeField] private AudioClip _soundInsert;
        [SerializeField] private AudioClip _soundInsert2;
        [SerializeField] private AudioClip _soundRemove;

        [SerializeField] private ItemSo _batteryItemSO;
        
        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            if (!holder.HasItem && _battery != null)
            {
                TrainBatteryItem battery = TakeBattery();

                if (battery != null)
                {
                    holder.Pickup(battery);
                }

                return;
            }

            if (!holder.HasItem)
                return;
            
            
            if (TryInsertBattery(holder.HeldItem))
            {
                holder.ForceClearHeldItem();
            }
        }

        System.Collections.IEnumerator AnimTrigger(TrainBatteryItem battery)
        {
            _animationEnd = false; 
            yield return new WaitForSeconds(0.02f);
            if (_battery != battery) yield break; 
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);
            if (_battery != battery) yield break; 
            bool hasEnergy = battery.State.currentCharge > 0f; 
            if (visualEffect != null && hasEnergy) 
                visualEffect.SendEvent("OnPlay");
            if (hasEnergy) 
                CameraShakeManager.Instance.Shake(0.15f, 0.1f);
            if (_soundInsert != null && hasEnergy) 
            {
                GameManager.AudioSystem.PlaySoundPositional(_soundInsert, transform.position, GameManager.AudioSystem.VFX);
                if (_soundInsert2 != null) 
                    GameManager.AudioSystem.PlaySoundPositional(_soundInsert2, transform.position, GameManager.AudioSystem.VFX);
            }

            yield return new WaitForSeconds(0.10f);
            if (_battery != battery) yield break; 

            _animationEnd = true;
            battery.GetComponent<BatteryLightsIndicator>()?.SetDisplayActive(true);
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

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null || !_animationEnd)
                return null;

            TrainBatteryItem battery = _battery;
            battery.GetComponent<BatteryLightsIndicator>()?.SetDisplayActive(false);
            battery.VisualOnly(false);
            _battery = null;
            _animationEnd = false; 
            PowerReady = false; 
            train.SetEnginePower(false);
            OnBatteryRemoved?.Invoke();
            if (_soundRemove != null && battery.State.currentCharge > 0f)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX, 0.6f);
            return battery;
        }

        public TrainBatteryItem GetBattery()
        {
            return _battery;
        }

        public bool TryInsertBattery(ItemState item) {
            if (_batteryItemSO != item.ItemSo || _battery != null) return false; 
            
            TrainBatteryItem batteryToInsert = (TrainBatteryItem)item.ItemSo.CreatePhysicalItem(item);
            VisualContainer visual = batteryToInsert.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();           
            _battery = batteryToInsert;
            batteryToInsert.VisualOnly(true);
            batteryToInsert.transform.SetParent(transform);
            batteryToInsert.transform.position = pivot.position;
            batteryToInsert.transform.rotation = pivot.rotation;
            OnBatteryInserted?.Invoke();
            StartCoroutine(AnimTrigger(batteryToInsert));

            if (ignitionSwitch.IsEngineOn())
            {
                train.SetEnginePower(true);
            }

            return true;
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