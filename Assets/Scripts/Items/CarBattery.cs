using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class CarBattery : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private BatteryCharcher batteryCharcher;

        [SerializeField] private Transform chargerPivot;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundInsert;
        [SerializeField] private AudioClip _soundRemove;

        [SerializeField] private ItemSo _batteryItemSO;

        private TrainBatteryItem _battery;
        private bool _canRestoreCharge;

        private void Awake()
        {
            batteryCharcher.OnInterruptCharge += DisableCharge;
            batteryCharcher.OnContinueCharge += EnableCharge;
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
            if (_battery == null || _battery.State.currentCharge <= 0f)
            {
                return;
            }            
        }

        public float GetChargeAmount()
        {
            return _battery.State.currentCharge;
        }

        private void EnableCharge()
        {
            _canRestoreCharge = true;
        }

        private void DisableCharge()
        {
            _canRestoreCharge = false;
        }

        public void DrainCharge(float drainAmount)
        {
            if (!_canRestoreCharge) return;
            if (_battery.State.currentCharge <= 0f) return;

            _battery.State.currentCharge = MathF.Max(0, _battery.State.currentCharge - ( drainAmount * Time.deltaTime));
        }

        public bool TryInsertBattery(ItemState item)
        {
            if (_batteryItemSO != item.ItemSo || _battery != null) return false;

            TrainBatteryItem batteryToInsert = (TrainBatteryItem)item.ItemSo.CreatePhysicalItem(item);
            _battery = batteryToInsert;
            batteryToInsert.VisualOnly(true);
            _battery.transform.position = chargerPivot.position;
            _battery.transform.rotation = chargerPivot.rotation;
            return true;
        }

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null) return null;

            TrainBatteryItem battery = _battery;
            battery.VisualOnly(false);
            _battery = null;

           /* if (_soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);*/

            return battery;
        }

        public TrainBatteryItem GetBattery() => _battery;


        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item)
        {
            return item.itemState.ItemSo == _batteryItemSO && _battery == null;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item)
        {
            return TryInsertBattery(item.itemState);
        }

        public void ClearFeedback()
        {
        }
    }
}

