using System;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace Root
{
    public class BatteryCharcher : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private Transform chargablePivot;
        [SerializeField] private VisualEffect visualEffect;
        [SerializeField] private float chargeAmount = 0.5f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _soundInsert;
        [SerializeField] private AudioClip _soundRemove;
        private TrainBatteryItem _battery;
        private bool _animationEnd;
        public event Action OnFullyCharged;
        public event Action OnInterruptCharge;
        public event Action OnContinueCharge;

        [SerializeField] private ItemSo _batteryItemSO;
        [SerializeField] private CarBattery batteryElement;
        private bool _powered = false;

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();
            if (holder == null) return;

            if (!holder.HasItem && _battery != null)
            {
                TrainBatteryItem battery = TakeBattery();
                if (battery != null)
                {
                    OnInterruptCharge?.Invoke();
                    holder.Pickup(battery);
                }
                return;
            }

            if (!holder.HasItem) return;
            if (_batteryItemSO != holder.HeldItem.ItemSo) return;

            if (TryInsertBattery(holder.HeldItem))
            {
                holder.ForceClearHeldItem();
                OnContinueCharge?.Invoke();
            }
        }

        private void Update()
        {
            if (_battery == null || batteryElement.GetBattery() == null)
            {
                if (_powered)
                {
                    _powered = false;
                }
                return;
            }
            if (batteryElement.GetChargeAmount() < 0) return;

            Charge();
        }

        private void Charge()
        {
            if (_battery.State.currentCharge >= _battery.State.maxCharge)
            {
                OnFullyCharged?.Invoke();
                return;
            }

            _battery.State.currentCharge = Mathf.Min(_battery.State.maxCharge, _battery.State.currentCharge + (chargeAmount * Time.deltaTime));

            batteryElement.DrainCharge(chargeAmount);
        }

        public bool TryInsertBattery(ItemState item)
        {
            if (_batteryItemSO != item.ItemSo || _battery != null) return false;

            TrainBatteryItem batteryToInsert = (TrainBatteryItem)item.ItemSo.CreatePhysicalItem(item);
            _battery = batteryToInsert;
            batteryToInsert.VisualOnly(true);
            _battery.transform.position = chargablePivot.position;
            _battery.transform.rotation = chargablePivot.rotation;
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
            }
            /*if (hadPower && _soundRemove != null)
                GameManager.AudioSystem.PlaySoundPositional(_soundRemove, transform.position, GameManager.AudioSystem.VFX);*/

            return battery;
        }

        public TrainBatteryItem GetBattery() => _battery;

        public bool IsPowered() => _powered;

        System.Collections.IEnumerator AnimTrigger(TrainBatteryItem battery)
        {
            _animationEnd = false;
            battery.AnimatorOn();
            yield return new WaitForSeconds(0.70f);

            if (_battery != battery) yield break;

            if (!_powered && battery.State.currentCharge > 0f)
            {
                if (visualEffect != null)
                    visualEffect.SendEvent("OnPlay");

                /*if (_soundInsert != null)
                    GameManager.AudioSystem.PlaySoundPositional(_soundInsert, transform.position, GameManager.AudioSystem.VFX);*/

                _powered = true;
            }

            yield return new WaitForSeconds(0.10f);

            if (_battery != battery) yield break;

            _animationEnd = true;
            battery.GetComponent<BatteryLightsIndicator>()?.SetDisplayActive(true);
        }


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
