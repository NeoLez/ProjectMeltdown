using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class BatteryLights : MonoBehaviour
    {
        [SerializeField] private List<Light> lights;

        private void Start()
        {
            GameManager.Train.OnPowerLost += TurnOff;
            GameManager.Train.OnPowerRestored += TurnOn;

            TurnOff();
        }

        private void OnDestroy()
        {
            GameManager.Train.OnPowerLost -= TurnOff;
            GameManager.Train.OnPowerRestored -= TurnOn;
        }

        private void TurnOn()
        {
            foreach (var light in lights)
                light.enabled = true;
        }

        private void TurnOff()
        {
            foreach (var light in lights)
                light.enabled = false;
        }
    }
}