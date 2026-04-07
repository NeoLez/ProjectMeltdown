using Unity.Mathematics;
using UnityEngine;

namespace Root {
    public class Train : MonoBehaviour {
        [SerializeField] private TrainSpeedController speedController;
        [SerializeField] private BatterySlot batterySlot;

        [SerializeField] private float batteryDrain;
        
        [SerializeField] private float _maxEngineStrain;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _accelerationRate;
        [SerializeField] private float _decelerationRate;
        [SerializeField] private AudioSource engineSound;
        [SerializeField] private float engineSoundPitchLow;
        [SerializeField] private float engineSoundPitchHigh;
        [SerializeField] private AudioSource rattleSound;
        [SerializeField] private AudioSource strainSound;
        [SerializeField] private float strainSoundVolumeLow;
        [SerializeField] private float strainSoundVolumeHigh;

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

            var battery = batterySlot.GetBattery();
            if (battery != null) {
                battery.energy -= batteryDrain * Time.deltaTime;
            }

            engineSound.pitch = math.lerp(engineSoundPitchLow, engineSoundPitchHigh, _currentSpeed / speedController.maxTrainSpeed);
            engineSound.volume = math.lerp(0.1f, 1, targetSpeed / speedController.maxTrainSpeed);
            rattleSound.volume = math.lerp(0, 1, _currentSpeed / speedController.maxTrainSpeed);
            rattleSound.pitch = math.lerp(0.5f, 3, _currentSpeed / speedController.maxTrainSpeed);
            strainSound.volume = math.lerp(strainSoundVolumeLow, strainSoundVolumeHigh, speedDifference / _maxEngineStrain);
            transform.position += Vector3.forward * _currentSpeed * Time.deltaTime;
        }

        public Vector3 GetSpeed() {
            return _currentSpeed * Vector3.forward;
        }
    }
}