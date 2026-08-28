using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Root {
    public class LightSwitchButton : PanelButton {
        [SerializeField] private List<Light> lights;

        private bool _on = false;
        
        public Transform lightObject;
        public Transform rotationOn;
        public Transform rotationOff;
        public float rotationTime;
        private bool isAnimating;
        
        private void Awake()
        {
            // Nos suscribimos al evento del boton
            //OnClicked += Toggle;
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
            OnClicked -= Toggle;
            GameManager.Train.OnPowerLost -= TurnOff;
            GameManager.Train.OnPowerRestored -= TurnOn;
        }

        public bool IsOn() => _on; // Devuelve si las luces est�n prendidas

        private void Toggle()
        {
            if(_on)
                TurnOff();
            else
                TurnOn();
        }

        private void TurnOff()
        {
            if (isAnimating) return;
            
            if(lightObject != null) {
                isAnimating = true;
                Tween.LocalRotation(lightObject, rotationOff.localRotation, rotationTime).OnComplete(() => isAnimating = false);
            }
            
            // Se apaga cuando el tren pierde energia
            _on = false;
            foreach (var light in lights)
            {
                light.enabled = false;
            }
        }

        private void TurnOn()
        {
            if (isAnimating) return;
            if (lightObject != null) {
                isAnimating = true;
                Tween.LocalRotation(lightObject, rotationOn.localRotation, rotationTime)
                    .OnComplete(() => isAnimating = false);
            }

            // Se vuelve a encender cuando el tren recupera energia
            _on = true;
            foreach (var light in lights)
            {
                light.enabled = true;
            }
        }
    }
}