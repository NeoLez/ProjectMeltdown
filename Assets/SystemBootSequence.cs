using System.Collections;
using TMPro;
using UnityEngine;

namespace Root
{
    public class SystemBootSequence : MonoBehaviour
    {
        [SerializeField] private BatterySlot batterySlot;

        [Header("Pantalla de boot")]
        [SerializeField] private GameObject bootCanvas;
        [SerializeField] private TMP_Text bootText;

        [Header("Sistemas reales")]
        [SerializeField] private GameObject systemsCanvas;
        [SerializeField] private GameObject lightsObject;

        [SerializeField] private float bootDuration = 2f;
        [SerializeField] private float activatedDuration = 1f;

        private void Awake()
        {
            batterySlot.OnBatteryInserted += StartBootSequence;
            batterySlot.OnBatteryRemoved += ShutdownSystems;
        }

        private void Start()
        {
            ShutdownSystems();
        }

        private void OnDestroy()
        {
            batterySlot.OnBatteryInserted -= StartBootSequence;
            batterySlot.OnBatteryRemoved -= ShutdownSystems;
        }

        private void StartBootSequence()
        {
            StopAllCoroutines();
            StartCoroutine(BootRoutine());
        }

        private IEnumerator BootRoutine()
        {
            // Apagar sistemas reales
            systemsCanvas.SetActive(false);
            lightsObject.SetActive(false);

            // Mostrar boot
            bootCanvas.SetActive(true);

            bootText.text = "ACTIVANDO SISTEMAS.....";

            yield return new WaitForSeconds(bootDuration);

            bootText.text = "ACTIVADO";

            yield return new WaitForSeconds(activatedDuration);

            // Encender sistemas
            bootCanvas.SetActive(false);

            systemsCanvas.SetActive(true);
            lightsObject.SetActive(true);
        }

        private void ShutdownSystems()
        {
            StopAllCoroutines();

            bootCanvas.SetActive(false);

            systemsCanvas.SetActive(false);
            lightsObject.SetActive(false);
        }
    }
}