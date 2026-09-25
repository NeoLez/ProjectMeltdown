using UnityEngine;

namespace Root
{
    public class BatteryLightsIndicator : MonoBehaviour
    {
        [SerializeField] private Transform lightsRoot;
        [SerializeField] private Color colorCharged = Color.green;
        [SerializeField] private Color colorConsumed = Color.red;

        private TrainBatteryItem _battery;
        private Light[] _lights;
        private bool[] _isCharged;
        private bool _displayActive; 

        private void Awake()
        {
            _battery = GetComponent<TrainBatteryItem>();

            _lights = lightsRoot != null
                ? lightsRoot.GetComponentsInChildren<Light>(true)
                : System.Array.Empty<Light>();

            _isCharged = new bool[_lights.Length];

            for (int i = 0; i < _isCharged.Length; i++)
                _isCharged[i] = true;

            SetDisplayActive(false); 
        }

        private void Update()
        {
            if (!_displayActive) return;

            float percent = 0f;

            if (_battery.So != null && _battery.So.maxCharge > 0f)
                percent = Mathf.Clamp01(_battery.So.currentCharge / _battery.So.maxCharge);

            for (int i = 0; i < _lights.Length; i++)
            {
                float threshold = 1f - (float)(i + 1) / _lights.Length;
                bool shouldBeCharged = percent > threshold;

                if (shouldBeCharged != _isCharged[i])
                {
                    _isCharged[i] = shouldBeCharged;
                    ApplyColor(_lights[i], shouldBeCharged);
                }
            }
        }

        public void SetDisplayActive(bool active)
        {
            _displayActive = active;

            for (int i = 0; i < _lights.Length; i++)
            {
                if (_lights[i] == null) continue;
                _lights[i].enabled = active;
            }

            if (active)
                RefreshAll();
        }

        private void RefreshAll()
        {
            float percent = 0f;

            if (_battery.So != null && _battery.So.maxCharge > 0f)
                percent = Mathf.Clamp01(_battery.So.currentCharge / _battery.So.maxCharge);

            for (int i = 0; i < _lights.Length; i++)
            {
                float threshold = 1f - (float)(i + 1) / _lights.Length;
                _isCharged[i] = percent > threshold;
                ApplyColor(_lights[i], _isCharged[i]);
            }
        }
        private void ApplyColor(Light light, bool charged)
        {
            if (light == null) return;
            light.color = charged ? colorCharged : colorConsumed;
        }
    }
}