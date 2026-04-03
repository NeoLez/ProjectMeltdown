using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class Train : MonoBehaviour {
        [SerializeField] private TrainSpeedController speedController;
        [SerializeField] private float _maxEngineStrain;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _accelerationRate;
        [SerializeField] private float _decelerationRate;

        private void Update() {
            float targetSpeed = speedController.GetTargetSpeed();
            float speedDifference = targetSpeed - _currentSpeed;
            if (speedDifference > _maxEngineStrain) {
                Debug.Log("Straining Engine: "+targetSpeed+" "+ _currentSpeed);
            }
            
            if (_currentSpeed > targetSpeed) {
                _currentSpeed -= _decelerationRate * Time.deltaTime;
                if (_currentSpeed < targetSpeed) {
                    _currentSpeed = targetSpeed;
                } 
            }
            else if (_currentSpeed < targetSpeed) {
                _currentSpeed += _accelerationRate * Time.deltaTime;
                if (_currentSpeed > targetSpeed) {
                    _currentSpeed = targetSpeed;
                } 
            }

            transform.position += Vector3.forward * _currentSpeed * Time.deltaTime;
        }

        public Vector3 GetSpeed() {
            return _currentSpeed * Vector3.forward;
        }
    }
}