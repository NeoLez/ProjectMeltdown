using TMPro;
using UnityEngine;

namespace Root
{
    public class BatteryIndicator : MonoBehaviour
    {
        [SerializeField] private BatteryCharcher batteryCharcher;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "{0}%";

        private bool _canRestoreCharge;

        private void Awake()
        {
            batteryCharcher.OnInterruptCharge += DisableCharge;
            batteryCharcher.OnContinueCharge += EnableCharge;
        }

        private void Start()
        {
            label.text = string.Format(format, 0);
        }

        private void EnableCharge()
        {
            _canRestoreCharge = true;
        }

        private void DisableCharge()
        {
            _canRestoreCharge = false;
        }


        private void Update()
        {
            if (!_canRestoreCharge) return;

            var battery = batteryCharcher.GetBattery();
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