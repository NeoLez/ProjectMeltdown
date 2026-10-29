using Root;
using UnityEngine;

namespace Timers
{
    public class WaterOutputPipe : MonoBehaviour
    {
        [SerializeField] private Pipe pipe;

        private void Awake()
        {
            pipe.OnStateChange += (state) =>
            {
                Debug.Log("OnStateChange");
                if (!state) return;
                OnActivated();
            };
        }

        private void OnActivated()
        {
            Debug.Log("Activated");
        }
    }
}