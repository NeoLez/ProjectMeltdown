using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class TrainSpeedController : Interactable {
        private bool active;
        private Vector3 _initialPosition;
        [SerializeField] private float sensitivity;
        [SerializeField] private float maxSwitchSpeed;
        [SerializeField] private float percentage;
        
        [SerializeField] private float maxTransformY;
        [SerializeField] private float maxTrainSpeed;
        [SerializeField] private Transform visuals;

        private void Awake() {
            _initialPosition = transform.localPosition;
        }

        public override void Interact(bool state) {
            active = state;
            if (active) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private void Update() {
            if (active) {
                float yMovement = GameManager.Input.CameraMovement.MouseY.ReadValue<float>() * sensitivity;
                if (yMovement > maxSwitchSpeed)
                    yMovement = maxSwitchSpeed;
                else if (yMovement < -maxSwitchSpeed)
                    yMovement = -maxSwitchSpeed;
                
                percentage += yMovement;
                            
                percentage = math.clamp(percentage , 0, 1);

                visuals.localPosition = _initialPosition + maxTransformY * percentage * Vector3.forward;
            }
        }

        public float GetTargetSpeed() {
            return maxTrainSpeed * percentage;
        }
    }
}