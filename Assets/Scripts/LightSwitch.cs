using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class LightSwitch : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private List<Light> lights;
        private bool _on = false;

        private void Awake()
        {
            button.OnClicked += Toggle;
        }

        private void OnDestroy()
        {
            button.OnClicked -= Toggle;
        }

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