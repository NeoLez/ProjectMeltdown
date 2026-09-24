using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class BatteryBarsDisplay : MonoBehaviour
    {
        [System.Serializable]
        public class Bar
        {
            public GameObject visual;
            [HideInInspector] public bool isOn = true;
            [HideInInspector] public float nextToggleTime;
        }

        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private List<Bar> bars = new List<Bar>();
        [SerializeField, Range(0.01f, 1f)] private float thresholdStep = 0.25f;
        [SerializeField, Range(0f, 0.25f)] private float warningRange = 0.05f;
        [SerializeField] private float flickerIntervalAtStart = 0.5f;
        [SerializeField] private float flickerIntervalAtThreshold = 0.05f;

        private void Update()
        {
            var battery = batterySlot.GetBattery();
            float percent = 0f;

            if (battery != null && battery.So.maxCharge > 0f)
                percent = Mathf.Clamp01(battery.So.currentCharge / battery.So.maxCharge);

            for (int i = 0; i < bars.Count; i++)
                UpdateBar(bars[i], i, percent);
        }

        private void UpdateBar(Bar bar, int index, float percent)
        {
            if (bar.visual == null) return;

            float threshold = 1f - thresholdStep * (index + 1); // 0.75, 0.50, 0.25, 0
            float warningStart = threshold + warningRange;

            if (percent <= threshold)
            {
                SetBar(bar, false);
                return;
            }

            if (percent >= warningStart)
            {
                SetBar(bar, true);
                return;
            }

            float t = (percent - threshold) / warningRange; // 1 al entrar, 0 en el umbral
            float interval = Mathf.Lerp(flickerIntervalAtThreshold, flickerIntervalAtStart, t);

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