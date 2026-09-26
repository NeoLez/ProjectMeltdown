using System;
using UnityEngine;

namespace Root
{
    public class GeneratorPowerFeed : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        public event Action<float> OnPowerPercentChanged;
        public event Action OnDepleted;
        public event Action OnPowerRestored;
        public event Action OnBatteryRemoved;
        public float CurrentPercent01 { get; private set; }
        private bool _powered; 

        private void Start()
        {
            generatorSlot.OnPowerRestored += HandlePowerRestored; 
            generatorSlot.OnPowerLost += HandlePowerLost;
        }

        private void OnDestroy()
        {
            if (generatorSlot != null)
            {
                generatorSlot.OnPowerRestored -= HandlePowerRestored; 
                generatorSlot.OnPowerLost -= HandlePowerLost;
            }
        }

        private void Update()
        {
            TrainBatteryItem battery = generatorSlot.GetBattery();

            float percent = 0f;
            if (_powered && battery != null && battery.State.maxCharge > 0f) 
                percent = Mathf.Clamp01(battery.State.currentCharge / battery.State.maxCharge);

            if (!Mathf.Approximately(percent, CurrentPercent01))
            {
                CurrentPercent01 = percent;
                OnPowerPercentChanged?.Invoke(CurrentPercent01);
            }
        }
        private void HandlePowerRestored()
        {
            _powered = true;
            OnPowerRestored?.Invoke();
        }

        private void HandlePowerLost()
        {
            _powered = false; 

            if (generatorSlot.GetBattery() != null)
                OnDepleted?.Invoke();
            else
                OnBatteryRemoved?.Invoke();
        }
    }
}
