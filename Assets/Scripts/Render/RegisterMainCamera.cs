using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Camera))]
    public class RegisterMainCamera : MonoBehaviour {
        private void Awake() {
            GameManager.Camera = GetComponent<Camera>();
        }
    }
}