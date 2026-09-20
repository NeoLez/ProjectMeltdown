using UnityEngine;

namespace Root
{
    public class AdvertisementBoard : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private Renderer boardRenderer;
        [SerializeField] private int materialIndex = 1; 
        [SerializeField] private Material materialOff;
        [SerializeField] private Material materialOn;

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

        private void TurnOn() => SetMaterial(materialOn);  
        private void TurnOff() => SetMaterial(materialOff); 

        private void SetMaterial(Material m)
        {
            if (boardRenderer == null || m == null) return;
            Material[] mats = boardRenderer.sharedMaterials;
            if (materialIndex < 0 || materialIndex >= mats.Length) return;
            mats[materialIndex] = m;
            boardRenderer.sharedMaterials = mats;
        }
    }
}