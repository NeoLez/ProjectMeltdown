using Unity.Mathematics;
using UnityEngine;

namespace Root
{
    public class SpeedometerHorizontal : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float angleWidth;
        public float maxSpeed;

        private void Start()
        {
            // Se suscribe al evento de perdida de energia
            GameManager.Train.OnPowerLost += ResetPosition;
        }

        private void OnDestroy()
        {
            GameManager.Train.OnPowerLost -= ResetPosition;
        }

        public void SetSpeed(float speed)
        {
            pivot.localPosition = Vector3.forward * math.lerp(0, angleWidth, speed / maxSpeed);
        }

        private void ResetPosition()
        {
            // Vuelve a la posicion original cuando hay un corte
            pivot.localPosition = Vector3.zero;
        }
    }
}