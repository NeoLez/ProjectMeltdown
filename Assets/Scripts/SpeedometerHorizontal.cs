using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class SpeedometerHorizontal : MonoBehaviour {
        [SerializeField] private Transform pivot;
        [SerializeField] private float angleWidth;
        public float maxSpeed;

        public void SetSpeed(float speed) {
            pivot.localPosition = Vector3.forward * math.lerp(0, angleWidth, speed/maxSpeed);
        }
    }
}