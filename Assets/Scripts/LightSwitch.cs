using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Root
{
    public class LightSwitch : MonoBehaviour
    {
        [FormerlySerializedAs("button")] [SerializeField] private PanelButton panelButton;         // Boton que activa el switch
        [SerializeField] private List<Light> lights;    // Lista de spotlights a controlar
        private bool _on = false;                       // Estado actual de las luces

        private void Awake()
        {
            // Nos suscribimos al evento del boton
            panelButton.OnClicked += Toggle;
        }

        private void Start()
        {
            // Nos suscribimos a los eventos de energia del tren
            GameManager.Train.OnPowerLost += TurnOff;
            GameManager.Train.OnPowerRestored += TurnOn;
        }

        private void OnDestroy()
        {
            // Nos desuscribimos para evitar errores al destruir el objeto
            panelButton.OnClicked -= Toggle;
            GameManager.Train.OnPowerLost -= TurnOff;
            GameManager.Train.OnPowerRestored -= TurnOn;
        }

        public bool IsOn() => _on; // Devuelve si las luces est�n prendidas

        private void Toggle()
        {
            // Alternamos el estado y lo aplicamos a cada luz
            _on = !_on;
            foreach (var light in lights)
            {
                light.enabled = _on;
            }
        }

        private void TurnOff()
        {
            // Se apaga cuando el tren pierde energia
            _on = false;
            foreach (var light in lights)
            {
                light.enabled = false;
            }
        }

        private void TurnOn()
        {
            // Se vuelve a encender cuando el tren recupera energia
            _on = true;
            foreach (var light in lights)
            {
                light.enabled = true;
            }
        }
    }
}