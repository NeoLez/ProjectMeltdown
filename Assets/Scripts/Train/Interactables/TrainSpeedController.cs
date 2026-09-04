using Root.Controller;
using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class TrainSpeedController : InteractableDraggable {
        private Vector3 _initialPosition;
        [SerializeField] private float sensitivity;
        [SerializeField] private float maxSwitchSpeed;
        [SerializeField] private float percentage;
        
        [SerializeField] private float maxTransformY;
        [SerializeField] public float maxTrainSpeed;
        [SerializeField] private Transform visuals;
        [SerializeField] private float automaticSwitchSpeed;
        private bool _loweringToZero;

        private void Awake() {
            _initialPosition = transform.localPosition;
        }

        private void Start()
        {
            SetCamera(GameManager.Camera);
            GameManager.Train.emergencyStopButton.OnBrake += LowerToZero;
        }

        private void Update() {
            if (active) {
                _loweringToZero = false;
                float yMovement = GameManager.Input.CameraMovement.MouseY.ReadValue<float>() * sensitivity;
                if (yMovement > maxSwitchSpeed)
                    yMovement = maxSwitchSpeed;
                else if (yMovement < -maxSwitchSpeed)
                    yMovement = -maxSwitchSpeed;
                
                percentage += yMovement;
                            
                percentage = math.saturate(percentage);

                visuals.localPosition = _initialPosition + maxTransformY * percentage * Vector3.forward;
            }
            else if (_loweringToZero) {
                percentage = math.saturate(percentage - automaticSwitchSpeed * Time.deltaTime);
                visuals.localPosition = _initialPosition + maxTransformY * percentage * Vector3.forward;
            }
            
            UpdateMousePosition();
        }

        public void LowerToZero() {
            _loweringToZero = true;
        }

        public float GetTargetSpeed() {
            return maxTrainSpeed * percentage;
        }
    }
}