using System;
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

        private void Awake() {
            _initialPosition = transform.localPosition;
        }

        private void Start()
        {
            SetCamera(GameManager.Camera);
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
            
            UpdateMousePosition();
        }

        public float GetTargetSpeed() {
            return maxTrainSpeed * percentage;
        }
    }
}