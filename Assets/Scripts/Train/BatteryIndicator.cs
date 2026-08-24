using TMPro;
using UnityEngine;

namespace Root
{
    public class BatteryIndicator : MonoBehaviour
    {
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "{0}%";

        private void Update()
        {
            var battery = batterySlot.GetBattery();
            if (battery == null)
            {
                label.text = string.Format(format, 0);
                return;
            }
            int percentage = Mathf.RoundToInt(battery.State.currentCharge / battery.State.maxCharge * 100);
            label.text = string.Format(format, percentage);
        }
    }
}