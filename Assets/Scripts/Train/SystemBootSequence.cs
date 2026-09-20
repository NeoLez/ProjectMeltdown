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
        [SerializeField] private GameObject emergencyLight;

        [SerializeField] private float bootDuration = 2f;
        [SerializeField] private float activatedDuration = 1f;
        private bool _booted;

        private void Awake()
        {
            batterySlot.OnBatteryInserted += StartBootSequence;
            batterySlot.OnBatteryRemoved += ShutdownSystems;
        }

        private void Start()
        {
            ShutdownSystems();
        }

        private void Update() 
        {
            if (_booted && !HasEnergy())
                ShutdownSystems();
        }

        private void OnDestroy()
        {
            batterySlot.OnBatteryInserted -= StartBootSequence;
            batterySlot.OnBatteryRemoved -= ShutdownSystems;
        }

        private bool HasEnergy()
        {
            TrainBatteryItem battery = batterySlot.GetBattery();
            return battery != null && battery.So.currentCharge > 0f;
        }

        private void StartBootSequence()
        {
            StopAllCoroutines();

            if (!HasEnergy()) 
            {
                ShutdownSystems();
                return;
            }

            StartCoroutine(BootRoutine());
        }

        private IEnumerator BootRoutine()
        {
            systemsCanvas.SetActive(false);
            lightsObject.SetActive(false);
            emergencyLight.SetActive(true);
            bootCanvas.SetActive(true);
            bootText.text = "ACTIVATING SYSTEMS.....";
            yield return new WaitForSeconds(bootDuration);
            bootText.text = "ACTIVATED";
            yield return new WaitForSeconds(activatedDuration);

            if (!HasEnergy()) 
            {
                ShutdownSystems();
                yield break;
            }

            bootCanvas.SetActive(false);
            systemsCanvas.SetActive(true);
            lightsObject.SetActive(true);
            emergencyLight.SetActive(false);
            _booted = true; 
            batterySlot.PowerReady = true; 
        }

        private void ShutdownSystems()
        {
            StopAllCoroutines();

            _booted = false; 
            batterySlot.PowerReady = false; 
            bootCanvas.SetActive(false);
            systemsCanvas.SetActive(false);
            lightsObject.SetActive(false);
            emergencyLight.SetActive(true);
        }
    }
}