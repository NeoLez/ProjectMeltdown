using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Root
{
    public class BatteryIndicator : MonoBehaviour
    {
        [SerializeField] private BatteryCharcher batteryCharcher;
        [SerializeField] private TMP_Text label;
        [SerializeField] private LocalizedString placeBatteriesText;
        [SerializeField] private LocalizedString rechargingText;
        [SerializeField] private LocalizedString completedText;
        [SerializeField] private LocalizedString fullText;
        [SerializeField] private float completedDuration = 2f;
        [SerializeField] private float dotsInterval = 0.4f;
        [SerializeField] private float blinkInterval = 0.6f;

        private float _completedUntil;

        private void Awake()
        {
            batteryCharcher.OnCarBatteryFinished += ShowCompleted;
        }

        private void OnDestroy()
        {
            if (batteryCharcher != null)
                batteryCharcher.OnCarBatteryFinished -= ShowCompleted;
        }

        private void ShowCompleted()
        {
            _completedUntil = Time.time + completedDuration;
            label.text = completedText.GetLocalizedString();
        }

        private void Update()
        {
            if (Time.time < _completedUntil)
            {
                label.enabled = true;
                return;
            }

            var battery = batteryCharcher.IsReady ? batteryCharcher.GetBattery() : null;

            if (battery != null && battery.State.currentCharge >= battery.State.maxCharge)
            {
                label.enabled = true;
                label.text = fullText.GetLocalizedString();
                return;
            }

            if (batteryCharcher.IsReady && batteryCharcher.IsCharging)
            {
                label.enabled = true;
                int dots = (int)(Time.time / dotsInterval) % 4;
                label.text = rechargingText.GetLocalizedString() + new string('.', dots);
                return;
            }

            label.text = placeBatteriesText.GetLocalizedString();
            label.enabled = (int)(Time.time / blinkInterval) % 2 == 0;
        }
    }
}