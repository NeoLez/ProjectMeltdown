using System;
using UnityEngine;

namespace Root
{
    public class TrainPowerFeed : MonoBehaviour
    {
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private Train train;

        public event Action<float> OnPowerPercentChanged;
        public float CurrentPercent01 { get; private set; }

        private void Update()
        {
            var battery = batterySlot.GetBattery();

            float percent = 0f;
            if (train.HasPower && battery != null && battery.State.maxCharge > 0f)
                percent = Mathf.Clamp01(battery.State.currentCharge / battery.State.maxCharge);

            if (!Mathf.Approximately(percent, CurrentPercent01))
            {
                CurrentPercent01 = percent;
                OnPowerPercentChanged?.Invoke(CurrentPercent01);
            }
        }
    }
}