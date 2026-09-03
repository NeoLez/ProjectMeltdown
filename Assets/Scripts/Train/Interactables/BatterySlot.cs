using System;
using UnityEngine;
using UnityEngine.Assertions;
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
            yield return new WaitForSeconds(0.02f);
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);
            visualEffect.SendEvent("OnPlay");
            GameManager.CameraController.Shake(0.15f, 0.1f);
            if (_soundInsert != null)
            {
                GameManager.AudioSystem.PlaySoundPositional(_soundInsert, transform.position, GameManager.AudioSystem.VFX);
                GameManager.AudioSystem.PlaySoundPositional(_soundInsert2, transform.position, GameManager.AudioSystem.VFX);
            }
               
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

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null)
                return null;

            TrainBatteryItem battery = _battery;

            battery.VisualOnly(false);

            _battery = null;
            train.SetEnginePower(false);
            OnBatteryRemoved?.Invoke();
            if (_soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX, 0.6f);
            return battery;
        }

        public TrainBatteryItem GetBattery()
        {
            return _battery;
        }

        public bool TryInsertBattery(ItemState item)
        {
            Assert.AreEqual(_batteryItemSO, item.ItemSo);
            TrainBatteryItem batteryToInsert = item.ItemSo.CreatePhysicalItem() as TrainBatteryItem;
            batteryToInsert.itemState = item;

            if (batteryToInsert == null)
                return false;

            VisualContainer visual = batteryToInsert.GetComponentInChildren<VisualContainer>();
            visual.goal = GameManager.Train.GetTrainPosition();
            
            if (visual == null)
                return false;
            
            if (_battery != null)
                return false;

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
    }
}