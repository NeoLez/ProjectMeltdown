using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class BatteryChargeBarsDisplay : MonoBehaviour
    {
        [System.Serializable]
        public class Bar
        {
            public GameObject visual;
            [HideInInspector] public bool isOn;
            [HideInInspector] public float nextToggleTime;
        }

        [SerializeField] private BatteryCharcher batteryCharcher;
        [SerializeField] private List<Bar> bars = new List<Bar>();
        [SerializeField, Range(0.01f, 1f)] private float thresholdStep = 0.25f;
        [SerializeField] private float flickerIntervalAtStart = 0.5f;
        [SerializeField] private float flickerIntervalAtFull = 0.05f;

        private void Update()
        {
            var battery = batteryCharcher.IsReady ? batteryCharcher.GetBattery() : null;
            float percent = 0f;

            if (battery != null && battery.State.maxCharge > 0f)
                percent = Mathf.Clamp01(battery.State.currentCharge / battery.State.maxCharge);

            bool charging = batteryCharcher.IsCharging;

            for (int i = 0; i < bars.Count; i++)
                UpdateBar(bars[i], i, percent, charging);
        }

        private void UpdateBar(Bar bar, int index, float percent, bool charging)
        {
            if (bar.visual == null) return;

            float lower = 1f - thresholdStep * (index + 1);
            float upper = lower + thresholdStep;

            if (percent <= lower)
            {
                SetBar(bar, false);
                return;
            }

            if (percent >= upper || !charging)
            {
                SetBar(bar, true);
                return;
            }

            float t = (percent - lower) / thresholdStep;
            float interval = Mathf.Lerp(flickerIntervalAtStart, flickerIntervalAtFull, t);

            if (Time.time >= bar.nextToggleTime)
            {
                SetBar(bar, !bar.isOn);
                bar.nextToggleTime = Time.time + interval;
            }
        }

        private void SetBar(Bar bar, bool on)
        {
            if (bar.isOn == on && bar.visual.activeSelf == on) return;
            bar.isOn = on;
            bar.visual.SetActive(on);
        }
    }
}