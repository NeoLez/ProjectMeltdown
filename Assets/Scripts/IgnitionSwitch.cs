using UnityEngine;

namespace Root
{
    public class IgnitionSwitch : MonoBehaviour
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;
        [SerializeField] private Button button;
        private bool _engineOn = false;

        private void Awake()
        {
            button.OnClicked += Toggle;

            onVisual.SetActive(_engineOn);
            offVisual.SetActive(!_engineOn);
        }

        private void OnDestroy()
        {
            button.OnClicked -= Toggle;
        }

        private void Toggle()
        {
            _engineOn = !_engineOn;

            GameManager.Train.SetEnginePower(_engineOn);

            onVisual.SetActive(_engineOn);     
            offVisual.SetActive(!_engineOn);    
        }
    }
}

//por ahora hago que prenda el tren con un boton pero dps hare que sea con un objeto fisico (una llave), tambien tengo que ver la forma de bloquear las interraciones cuando esta el tren apagado.