using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Root
{
    public class SystemBootSequence : MonoBehaviour
    {
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private IgnitionSwitch ignitionSwitch; 

        [Header("Pantalla de boot")]
        [SerializeField] private GameObject bootCanvas;
        [SerializeField] private TMP_Text bootText;
        [SerializeField] private LocalizedString activatingText;
        [SerializeField] private LocalizedString activatedText;

        [Header("Sistemas reales")]
        [SerializeField] private GameObject systemsCanvas;
        [SerializeField] private GameObject lightsObject;
        [SerializeField] private GameObject emergencyLight;

        [SerializeField] private float bootDuration = 2f;
        [SerializeField] private float activatedDuration = 1f;
        private bool _booted;
        private bool _ignitionWasOn; 

        private void Awake()
        {
            batterySlot.OnBatteryInserted += StartBootSequence;
            batterySlot.OnBatteryRemoved += ShutdownSystems;
        }

        private void Start()
        {
            ShutdownSystems();
            _ignitionWasOn = IsIgnitionOn();
        }

        private void Update()
        {
            bool ignitionOn = IsIgnitionOn();

            if (ignitionOn != _ignitionWasOn)
            {
                _ignitionWasOn = ignitionOn;

                if (!ignitionOn)
                    ShutdownSystems();
                else if (batterySlot.GetBattery() != null)
                    StartBootSequence();
            }

            if (_booted && !HasEnergy())
                ShutdownSystems();
        }

        private void OnDestroy()
        {
            batterySlot.OnBatteryInserted -= StartBootSequence;
            batterySlot.OnBatteryRemoved -= ShutdownSystems;
        }

        private bool IsIgnitionOn()
        {
            return ignitionSwitch == null || ignitionSwitch.IsEngineOn();
        }

        private bool HasEnergy()
        {
            TrainBatteryItem battery = batterySlot.GetBattery();
            return battery != null && battery.So.currentCharge > 0f;
        }

        private void StartBootSequence()
        {
            StopAllCoroutines();

            if (!HasEnergy()||!IsIgnitionOn()) 
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
            var activating = activatingText.GetLocalizedStringAsync(); 
            yield return activating; 
            bootText.text = activating.Result; 
            yield return new WaitForSeconds(bootDuration);
            var activated = activatedText.GetLocalizedStringAsync();
            yield return activated; 
            bootText.text = activated.Result; 
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