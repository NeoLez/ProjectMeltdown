using UnityEngine;
using TMPro;

namespace Root
{
    public class BatterySlotUI : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private TextMeshProUGUI batteryPercentText;

        private void Update()
        {
            if (batteryPercentText == null) return;

            TrainBatteryItem battery = generatorSlot.GetBattery();

            if (battery == null || battery.State.maxCharge <= 0f)
            {
                batteryPercentText.text = "0%";
                return;
            }

            int percent = Mathf.RoundToInt((battery.State.currentCharge / battery.State.maxCharge) * 100f);
            percent = Mathf.Clamp(percent, 0, 100);
            batteryPercentText.text = $"{percent}%";
        }
    }
}