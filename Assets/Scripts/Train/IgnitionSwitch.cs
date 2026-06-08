using UnityEngine;

namespace Root
{
    public class IgnitionSwitch : MonoBehaviour
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;

        [SerializeField] private PanelButton panelButton;

        private bool _engineOn = true;

        private void Awake()
        {
            panelButton.OnClicked += Toggle;
            onVisual.SetActive(_engineOn);
            offVisual.SetActive(!_engineOn);
        }

        private void OnDestroy()
        {
            panelButton.OnClicked -= Toggle;
        }

        private void Toggle()
        {
            Debug.Log("Toggle");

            _engineOn = !_engineOn;

            Debug.Log("EngineOn: " + _engineOn);

            GameManager.Train.SetEnginePower(_engineOn);

            onVisual.SetActive(_engineOn);
            offVisual.SetActive(!_engineOn);
        }

        public bool IsEngineOn()
        {
            return _engineOn;
        }
    }
}

//por ahora hago que prenda el tren con un boton pero dps hare que sea con un objeto fisico (una llave), tambien tengo que ver la forma de bloquear las interraciones cuando esta el tren apagado.