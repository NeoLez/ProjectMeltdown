using UnityEngine;

namespace Root
{
    public class AdvertisementBoard : MonoBehaviour
    {
        [SerializeField] private GeneratorSlot generatorSlot;
        [SerializeField] private Renderer boardRenderer;
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

        private void TurnOn()
        {
            if (boardRenderer == null || materialOn == null) return;
            Material[] mats = boardRenderer.sharedMaterials;
            mats[1] = materialOn;
            boardRenderer.sharedMaterials = mats;
        }

        private void TurnOff()
        {
            if (boardRenderer == null || materialOff == null) return;
            Material[] mats = boardRenderer.sharedMaterials;
            mats[1] = materialOff;
            boardRenderer.sharedMaterials = mats;
        }
    }
}