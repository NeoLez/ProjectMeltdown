using UnityEngine;

namespace Root
{
    public class GeneratorLights : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private Transform lightsParent;
        private Light[] lights;

        private void Start()
        {
            lights = lightsParent.GetComponentsInChildren<Light>();

            generatorSlot.OnPowerRestored += TurnOn;
            generatorSlot.OnPowerLost += TurnOff;

            TurnOff();
        }

        private void OnDestroy()
        {
            generatorSlot.OnPowerRestored -= TurnOn;
            generatorSlot.OnPowerLost -= TurnOff;
        }

        private void TurnOn()
        {
            foreach (var l in lights)
                l.enabled = true;
        }

        private void TurnOff()
        {
            foreach (var l in lights)
                l.enabled = false;
        }
    }
}