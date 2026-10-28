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

        [Header("Visuals")]
        [SerializeField] GameObject[] toggleOpenTwesers;
        [SerializeField] GameObject[] toggleCloseTwesers;

        [Header("Overcharge")]
        [SerializeField] private float maxShakeIntensity = 0.02f;
        [SerializeField] private float shakeFrequency = 40f;
        [SerializeField] private VisualEffect explosionVFX;

        private TrainBatteryItem _battery;

        private void Awake()
        {
            batteryCharcher.OnInterruptCharge += DisableCharge;
        }

        private void Start()
        {
            for (int i = 0; i < toggleOpenTwesers.Length; i++)
            {
                toggleOpenTwesers[i].SetActive(false);
            }
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

                    for (int i = 0; i < toggleOpenTwesers.Length; i++)
                    {
                        toggleOpenTwesers[i].SetActive(false);
                    }
                    for (int i = 0; i < toggleCloseTwesers.Length; i++)
                    {
                        toggleCloseTwesers[i].SetActive(true);
                    }
                }
                return;
            }

            if (!holder.HasItem) return;
            if (_batteryItemSO != holder.HeldItem.ItemSo) return;

            if (TryInsertBattery(holder.HeldItem))
            {
                holder.ForceClearHeldItem();

                for (int i = 0; i < toggleOpenTwesers.Length; i++)
                {
                    toggleOpenTwesers[i].SetActive(true);
                }
                for (int i = 0; i < toggleCloseTwesers.Length; i++)
                {
                    toggleCloseTwesers[i].SetActive(false);
                }
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

        public void ConsumeCharge(float amount)
        {
            if (_battery == null) return;
            _battery.State.currentCharge = Mathf.Max(0f, _battery.State.currentCharge - amount);
        }

        public void Shake(float intensity)
        {
            if (_battery == null) return;

            if (intensity <= 0f)
            {
                _battery.transform.position = chargerPivot.position;
                return;
            }

            float amplitude = intensity * maxShakeIntensity;
            float time = Time.time * shakeFrequency;
            Vector3 offset = new Vector3(Mathf.PerlinNoise(time, 0f) - 0.5f, Mathf.PerlinNoise(0f, time) - 0.5f, Mathf.PerlinNoise(time, time) - 0.5f) * 2f * amplitude;
            _battery.transform.position = chargerPivot.position + offset;
        }

        public void Explode()
        {
            if (_battery == null) return;

            TrainBatteryItem battery = _battery;
            _battery = null;

            if (explosionVFX != null)
            {
                explosionVFX.transform.position = chargerPivot.position;
                explosionVFX.gameObject.SetActive(true);
                explosionVFX.SendEvent("OnPlay");
            }

            Destroy(battery.gameObject);
        }

        private void DisableCharge()
        {
            if (_battery != null)
                _battery.transform.position = chargerPivot.position;
        }

        public bool TryInsertBattery(ItemState item)
        {
            if (_batteryItemSO != item.ItemSo || _battery != null) return false;

            TrainBatteryItem batteryToInsert = (TrainBatteryItem)item.ItemSo.CreatePhysicalItem(item);
            _battery = batteryToInsert;
            batteryToInsert.VisualOnly(true);
            _battery.transform.position = chargerPivot.position;
            _battery.transform.rotation = chargerPivot.rotation;

            for (int i = 0; i < toggleOpenTwesers.Length; i++)
            {
                toggleOpenTwesers[i].SetActive(true);
            }
            for (int i = 0; i < toggleCloseTwesers.Length; i++)
            {
                toggleOpenTwesers[i].SetActive(false);
            }
            return true;
        }

        public TrainBatteryItem TakeBattery()
        {
            if (_battery == null) return null;

            TrainBatteryItem battery = _battery;
            battery.VisualOnly(false);
            _battery = null;
            for (int i = 0; i < toggleOpenTwesers.Length; i++)
            {
                toggleOpenTwesers[i].SetActive(false);
            }
            for (int i = 0; i < toggleCloseTwesers.Length; i++)
            {
                toggleOpenTwesers[i].SetActive(true);
            }
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

