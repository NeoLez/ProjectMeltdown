using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class GeneratorLights : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private List<GameObject> poweredObjects; 

        private void Start()
        {
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
            foreach (var obj in poweredObjects)
                if (obj != null) obj.SetActive(true);
        }

        private void TurnOff() 
        {
            foreach (var obj in poweredObjects)
                if (obj != null) obj.SetActive(false);
        }
    }
}