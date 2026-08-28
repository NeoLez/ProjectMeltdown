using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class Speedometer : MonoBehaviour {
        [SerializeField] private Transform pivot;
        [SerializeField] private float angleWidth;
        public float maxSpeed;

        public void SetSpeed(float speed) {
            pivot.transform.localRotation = Quaternion.Euler(Vector3.up * math.lerp(-angleWidth, angleWidth, speed/maxSpeed));
        }
        
    }
}