using UnityEngine;
using TMPro;

namespace Root
{
    public class BatterySlotUI : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private TextMeshProUGUI batteryPercentText;
        private int _lastPercent = -1; 

        private void Update()
        {
            if (batteryPercentText == null) return;

            TrainBatteryItem battery = generatorSlot.GetBattery();

            int percent = 0;
            if (battery != null && battery.State.maxCharge > 0f)
                percent = Mathf.Clamp(Mathf.RoundToInt(battery.State.currentCharge / battery.State.maxCharge * 100f), 0, 100);

            if (percent == _lastPercent) return; 
            _lastPercent = percent;              
            batteryPercentText.text = $"{percent}%";
        }
    }
}