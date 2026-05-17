using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class LightSwitch : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private List<Light> lights;

        private bool _on = true; 

        private void Awake()
        {
            button.OnClicked += Toggle;
        }

        private void Start()
        {
            foreach (var light in lights)
            {
                light.enabled = _on;
            }
        }

        private void OnDestroy()
        {
            button.OnClicked -= Toggle;
        }

        public bool IsOn() => _on;

        private void Toggle()
        {
            _on = !_on;

            foreach (var light in lights)
            {
                light.enabled = _on;
            }
        }
    }
}